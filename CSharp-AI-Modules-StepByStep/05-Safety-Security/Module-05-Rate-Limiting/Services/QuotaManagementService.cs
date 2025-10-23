using Module_05_Rate_Limiting.Models;

namespace Module_05_Rate_Limiting.Services;

public interface IQuotaManagementService
{
    Task<QuotaCheckResponse> CheckQuotaAsync(string clientId, string endpoint, int requestCount = 1);
    Task<QuotaUsage> GetUsageAsync(string clientId);
    Task<bool> UpdateQuotaAsync(string clientId, SubscriptionTier tier, int? customDaily = null, int? customMonthly = null);
    Task<QuotaAllocation> GetAllocationAsync(string clientId);
    Task ResetQuotaAsync(string clientId);
}

public class QuotaManagementService : IQuotaManagementService
{
    private readonly ILogger<QuotaManagementService> _logger;
    private readonly Dictionary<string, QuotaAllocation> _allocations;
    private readonly Dictionary<SubscriptionTier, QuotaPolicy> _policies;

    public QuotaManagementService(ILogger<QuotaManagementService> logger)
    {
        _logger = logger;
        _allocations = new Dictionary<string, QuotaAllocation>();
        _policies = InitializePolicies();
    }

    private Dictionary<SubscriptionTier, QuotaPolicy> InitializePolicies()
    {
        return new Dictionary<SubscriptionTier, QuotaPolicy>
        {
            [SubscriptionTier.Free] = new QuotaPolicy
            {
                PolicyId = "free",
                Name = "Free Tier",
                Tier = SubscriptionTier.Free,
                DailyLimit = 1000,
                MonthlyLimit = 10000,
                ConcurrentLimit = 5,
                CostPerRequest = 0,
                OverageCost = 0,
                AllowOverage = false
            },
            [SubscriptionTier.Basic] = new QuotaPolicy
            {
                PolicyId = "basic",
                Name = "Basic Tier",
                Tier = SubscriptionTier.Basic,
                DailyLimit = 10000,
                MonthlyLimit = 250000,
                ConcurrentLimit = 20,
                CostPerRequest = 0.001m,
                OverageCost = 0.002m,
                AllowOverage = true
            },
            [SubscriptionTier.Professional] = new QuotaPolicy
            {
                PolicyId = "professional",
                Name = "Professional Tier",
                Tier = SubscriptionTier.Professional,
                DailyLimit = 100000,
                MonthlyLimit = 2500000,
                ConcurrentLimit = 50,
                CostPerRequest = 0.0008m,
                OverageCost = 0.0015m,
                AllowOverage = true
            },
            [SubscriptionTier.Enterprise] = new QuotaPolicy
            {
                PolicyId = "enterprise",
                Name = "Enterprise Tier",
                Tier = SubscriptionTier.Enterprise,
                DailyLimit = 1000000,
                MonthlyLimit = 25000000,
                ConcurrentLimit = 200,
                CostPerRequest = 0.0005m,
                OverageCost = 0.001m,
                AllowOverage = true
            },
            [SubscriptionTier.Unlimited] = new QuotaPolicy
            {
                PolicyId = "unlimited",
                Name = "Unlimited Tier",
                Tier = SubscriptionTier.Unlimited,
                DailyLimit = int.MaxValue,
                MonthlyLimit = int.MaxValue,
                ConcurrentLimit = 1000,
                CostPerRequest = 0,
                OverageCost = 0,
                AllowOverage = true
            }
        };
    }

    public async Task<QuotaCheckResponse> CheckQuotaAsync(string clientId, string endpoint, int requestCount = 1)
    {
        var allocation = await GetOrCreateAllocationAsync(clientId);
        var policy = _policies[allocation.Tier];

        // Check daily quota
        if (allocation.UsedToday + requestCount > allocation.DailyQuota && !policy.AllowOverage)
        {
            return new QuotaCheckResponse
            {
                HasQuota = false,
                RemainingDaily = 0,
                RemainingMonthly = allocation.MonthlyQuota - allocation.UsedThisMonth,
                QuotaResetTime = GetNextDayReset(),
                Message = "Daily quota exceeded"
            };
        }

        // Check monthly quota
        if (allocation.UsedThisMonth + requestCount > allocation.MonthlyQuota && !policy.AllowOverage)
        {
            return new QuotaCheckResponse
            {
                HasQuota = false,
                RemainingDaily = allocation.DailyQuota - allocation.UsedToday,
                RemainingMonthly = 0,
                QuotaResetTime = allocation.QuotaResetDate,
                Message = "Monthly quota exceeded"
            };
        }

        // Update usage
        allocation.UsedToday += requestCount;
        allocation.UsedThisMonth += requestCount;

        return new QuotaCheckResponse
        {
            HasQuota = true,
            RemainingDaily = Math.Max(0, allocation.DailyQuota - allocation.UsedToday),
            RemainingMonthly = Math.Max(0, allocation.MonthlyQuota - allocation.UsedThisMonth),
            QuotaResetTime = allocation.QuotaResetDate
        };
    }

    public async Task<QuotaUsage> GetUsageAsync(string clientId)
    {
        var allocation = await GetOrCreateAllocationAsync(clientId);
        var policy = _policies[allocation.Tier];

        var cost = allocation.UsedThisMonth * policy.CostPerRequest;
        var overageAmount = Math.Max(0, allocation.UsedThisMonth - allocation.MonthlyQuota);
        cost += overageAmount * policy.OverageCost;

        return new QuotaUsage
        {
            UsageId = Guid.NewGuid().ToString(),
            ClientId = clientId,
            PolicyId = policy.PolicyId,
            Period = DateTime.UtcNow,
            RequestCount = allocation.UsedThisMonth,
            DailyUsed = allocation.UsedToday,
            MonthlyUsed = allocation.UsedThisMonth,
            DailyRemaining = Math.Max(0, allocation.DailyQuota - allocation.UsedToday),
            MonthlyRemaining = Math.Max(0, allocation.MonthlyQuota - allocation.UsedThisMonth),
            CurrentCost = cost,
            IsOverQuota = allocation.UsedThisMonth > allocation.MonthlyQuota,
            LastRequestAt = DateTime.UtcNow
        };
    }

    public async Task<bool> UpdateQuotaAsync(string clientId, SubscriptionTier tier, int? customDaily = null, int? customMonthly = null)
    {
        _logger.LogInformation("Updating quota for client {ClientId} to tier {Tier}", clientId, tier);

        var policy = _policies[tier];
        var allocation = await GetOrCreateAllocationAsync(clientId);

        allocation.Tier = tier;
        allocation.DailyQuota = customDaily ?? policy.DailyLimit;
        allocation.MonthlyQuota = customMonthly ?? policy.MonthlyLimit;
        allocation.HasOverageAllowance = policy.AllowOverage;

        return true;
    }

    public async Task<QuotaAllocation> GetAllocationAsync(string clientId)
    {
        return await GetOrCreateAllocationAsync(clientId);
    }

    public async Task ResetQuotaAsync(string clientId)
    {
        if (_allocations.ContainsKey(clientId))
        {
            var allocation = _allocations[clientId];
            allocation.UsedToday = 0;
            allocation.UsedThisMonth = 0;
            _logger.LogInformation("Reset quota for client: {ClientId}", clientId);
        }

        await Task.CompletedTask;
    }

    private async Task<QuotaAllocation> GetOrCreateAllocationAsync(string clientId)
    {
        if (!_allocations.ContainsKey(clientId))
        {
            var policy = _policies[SubscriptionTier.Free]; // Default to free tier
            _allocations[clientId] = new QuotaAllocation
            {
                ClientId = clientId,
                Tier = SubscriptionTier.Free,
                DailyQuota = policy.DailyLimit,
                MonthlyQuota = policy.MonthlyLimit,
                UsedToday = 0,
                UsedThisMonth = 0,
                QuotaResetDate = GetNextMonthReset(),
                HasOverageAllowance = policy.AllowOverage
            };
        }

        // Reset daily usage if new day
        var allocation = _allocations[clientId];
        if (DateTime.UtcNow.Date > DateTime.UtcNow.Date)
        {
            allocation.UsedToday = 0;
        }

        // Reset monthly usage if new month
        if (DateTime.UtcNow >= allocation.QuotaResetDate)
        {
            allocation.UsedThisMonth = 0;
            allocation.QuotaResetDate = GetNextMonthReset();
        }

        return await Task.FromResult(allocation);
    }

    private DateTime GetNextDayReset()
    {
        return DateTime.UtcNow.Date.AddDays(1);
    }

    private DateTime GetNextMonthReset()
    {
        var now = DateTime.UtcNow;
        return new DateTime(now.Year, now.Month, 1).AddMonths(1);
    }
}
