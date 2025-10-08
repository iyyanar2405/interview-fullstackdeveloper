using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AgentPlayground;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("C# LLM Agent Playground");
        var provider = Env("LLM_PROVIDER", "echo");
        ILLMClient llm = provider switch
        {
            "openai" => new OpenAIClient(Env("OPENAI_API_KEY"), Env("OPENAI_BASE_URL", "https://api.openai.com/v1"), Env("OPENAI_MODEL", "gpt-4o-mini")),
            "anthropic" => new AnthropicClient(Env("ANTHROPIC_API_KEY"), Env("ANTHROPIC_MODEL", "claude-3-5-sonnet-latest")),
            _ => new EchoClient()
        };

        var rag = new SimpleRag();
        rag.AddDocument("doc1", "RAG retrieves relevant context chunks to ground LLM outputs.");
        rag.AddDocument("doc2", "Anthropic Claude excels at tool use and instruction following.");

        var agent = new SimpleAgent(llm, rag);
        Console.WriteLine($"Provider: {provider}. Type a question (or 'exit'). Try: 'calc: 2+2' or 'search: rag'.");
        while (true)
        {
            Console.Write("you> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
            var reply = await agent.ReplyAsync(input);
            Console.WriteLine($"agent> {reply}\n");
        }
    }

    static string Env(string key, string def = "") => Environment.GetEnvironmentVariable(key) ?? def;
}

public interface ILLMClient
{
    Task<string> ChatAsync(string system, string user, CancellationToken ct = default);
}

public class EchoClient : ILLMClient
{
    public Task<string> ChatAsync(string system, string user, CancellationToken ct = default)
        => Task.FromResult($"[echo] system='{system}', user='{user}'");
}

public class OpenAIClient : ILLMClient
{
    private readonly HttpClient _http = new();
    private readonly string _baseUrl;
    private readonly string _model;

    public OpenAIClient(string apiKey, string baseUrl, string model)
    {
        if (!string.IsNullOrWhiteSpace(apiKey))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _baseUrl = baseUrl.TrimEnd('/');
        _model = model;
    }

    public async Task<string> ChatAsync(string system, string user, CancellationToken ct = default)
    {
        var payload = new
        {
            model = _model,
            messages = new object[]
            {
                new { role = "system", content = system },
                new { role = "user", content = user }
            }
        };
        using var res = await _http.PostAsJsonAsync($"{_baseUrl}/chat/completions", payload, ct);
        res.EnsureSuccessStatusCode();
        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync(ct));
        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();
        return content ?? string.Empty;
    }
}

public class AnthropicClient : ILLMClient
{
    private readonly HttpClient _http = new();
    private readonly string _model;

    public AnthropicClient(string apiKey, string model)
    {
        _http.DefaultRequestHeaders.Add("x-api-key", apiKey ?? "");
        _http.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        _model = model;
    }

    public async Task<string> ChatAsync(string system, string user, CancellationToken ct = default)
    {
        var payload = new
        {
            model = _model,
            system = system,
            max_tokens = 256,
            messages = new object[] { new { role = "user", content = user } }
        };
        using var res = await _http.PostAsJsonAsync("https://api.anthropic.com/v1/messages", payload, ct);
        res.EnsureSuccessStatusCode();
        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync(ct));
        var content = doc.RootElement.GetProperty("content")[0].GetProperty("text").GetString();
        return content ?? string.Empty;
    }
}

public class SimpleAgent
{
    private readonly ILLMClient _llm;
    private readonly SimpleRag _rag;
    private const string SystemPrompt = "You are a helpful C# agent. Be concise. Use provided context when relevant.";

    public SimpleAgent(ILLMClient llm, SimpleRag rag)
    {
        _llm = llm;
        _rag = rag;
    }

    public async Task<string> ReplyAsync(string input, CancellationToken ct = default)
    {
        // Tools: calc and search
        if (input.StartsWith("calc:", StringComparison.OrdinalIgnoreCase))
        {
            var expr = input.Substring(5).Trim();
            try { var result = SimpleCalc.Eval(expr); return $"calc={result}"; }
            catch (Exception ex) { return $"calc error: {ex.Message}"; }
        }
        if (input.StartsWith("search:", StringComparison.OrdinalIgnoreCase))
        {
            var q = input.Substring(7).Trim();
            var ctx = _rag.Query(q, topK: 2);
            return $"search results:\n- {string.Join("\n- ", ctx)}";
        }

        var ctxChunks = _rag.Query(input, topK: 2);
        var ctx = string.Join("\n", ctxChunks);
        var user = string.IsNullOrWhiteSpace(ctx) ? input : $"Context:\n{ctx}\n\nQuestion: {input}";
        return await _llm.ChatAsync(SystemPrompt, user, ct);
    }
}

public static class SimpleCalc
{
    // Very tiny evaluator: supports + - * / with double.TryParse and DataTable.Compute fallback removed for safety.
    public static double Eval(string expr)
    {
        // Extremely naive: only handles a op b
        var parts = expr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 3 && double.TryParse(parts[0], out var a) && double.TryParse(parts[2], out var b))
        {
            return parts[1] switch
            {
                "+" => a + b,
                "-" => a - b,
                "*" => a * b,
                "/" => b != 0 ? a / b : throw new DivideByZeroException(),
                _ => throw new InvalidOperationException("Unsupported operator")
            };
        }
        throw new InvalidOperationException("Use format: <a> <op> <b>, e.g., '2 + 3'");
    }
}

public class SimpleRag
{
    private readonly Dictionary<string, string> _docs = new();

    public void AddDocument(string id, string text) => _docs[id] = text;

    public IEnumerable<string> Query(string query, int topK = 3)
    {
        // Cosine on hashed term frequency vectors (tiny, no deps)
        var qv = Vectorize(query);
        return _docs
            .Select(kv => new { kv.Key, kv.Value, Score = Cosine(qv, Vectorize(kv.Value)) })
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .Select(x => x.Value);
    }

    private static Dictionary<int, double> Vectorize(string text)
    {
        var vec = new Dictionary<int, double>();
        foreach (var token in Tokenize(text))
        {
            var h = Hash(token);
            vec[h] = vec.GetValueOrDefault(h) + 1.0;
        }
        // L2 normalize
        var norm = Math.Sqrt(vec.Values.Sum(v => v * v));
        if (norm > 0) foreach (var k in vec.Keys.ToList()) vec[k] /= norm;
        return vec;
    }

    private static IEnumerable<string> Tokenize(string text)
        => text.ToLowerInvariant().Split(new[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '!', '?', '(', ')', '"', '\'', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

    private static int Hash(string token)
        => Math.Abs(token.GetHashCode()) % 2048;

    private static double Cosine(Dictionary<int, double> a, Dictionary<int, double> b)
    {
        double dot = 0;
        if (a.Count < b.Count) { (a, b) = (b, a); }
        foreach (var (k, va) in a) if (b.TryGetValue(k, out var vb)) dot += va * vb;
        return dot; // already normalized
    }
}
