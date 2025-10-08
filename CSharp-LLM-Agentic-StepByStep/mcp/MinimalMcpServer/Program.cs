using System.Text.Json;
using System.Text;

// Minimal MCP-like stdio JSON-RPC echo server with a ping method.
// Not a full MCP implementation; use as a starting point.

var stdin = Console.OpenStandardInput();
var stdout = Console.OpenStandardOutput();
var reader = new StreamReader(stdin, Encoding.UTF8);
var writer = new StreamWriter(stdout, new UTF8Encoding(false)) { AutoFlush = true };

while (true)
{
    var line = await reader.ReadLineAsync();
    if (line == null) break;
    try
    {
        using var doc = JsonDocument.Parse(line);
        var root = doc.RootElement;
        var id = root.TryGetProperty("id", out var idEl) ? idEl.GetInt32() : 0;
        var method = root.GetProperty("method").GetString();

        if (method == "ping")
        {
            var response = new { jsonrpc = "2.0", id, result = new { ok = true, ts = DateTimeOffset.UtcNow } };
            await writer.WriteLineAsync(JsonSerializer.Serialize(response));
        }
        else
        {
            var err = new { jsonrpc = "2.0", id, error = new { code = -32601, message = $"Method not found: {method}" } };
            await writer.WriteLineAsync(JsonSerializer.Serialize(err));
        }
    }
    catch (Exception ex)
    {
        var err = new { jsonrpc = "2.0", id = 0, error = new { code = -32700, message = $"Parse error: {ex.Message}" } };
        await writer.WriteLineAsync(JsonSerializer.Serialize(err));
    }
}
