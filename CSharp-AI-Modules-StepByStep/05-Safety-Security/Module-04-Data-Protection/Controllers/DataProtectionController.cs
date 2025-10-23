using Microsoft.AspNetCore.Mvc;
using Module_04_Data_Protection.Models;
using Module_04_Data_Protection.Services;

namespace Module_04_Data_Protection.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataProtectionController : ControllerBase
{
    private readonly IEncryptionService _encryptionService;
    private readonly IKeyManagementService _keyManagementService;
    private readonly IGdprComplianceService _gdprService;
    private readonly IAnonymizationService _anonymizationService;
    private readonly IAuditLoggingService _auditService;
    private readonly ILogger<DataProtectionController> _logger;

    public DataProtectionController(
        IEncryptionService encryptionService,
        IKeyManagementService keyManagementService,
        IGdprComplianceService gdprService,
        IAnonymizationService anonymizationService,
        IAuditLoggingService auditService,
        ILogger<DataProtectionController> logger)
    {
        _encryptionService = encryptionService;
        _keyManagementService = keyManagementService;
        _gdprService = gdprService;
        _anonymizationService = anonymizationService;
        _auditService = auditService;
        _logger = logger;
    }

    // Encryption Endpoints
    [HttpPost("encrypt")]
    public async Task<ActionResult<EncryptionResponse>> Encrypt([FromBody] EncryptionRequest request)
    {
        try
        {
            var result = await _encryptionService.EncryptAsync(request.Data, request.Algorithm);
            
            await _auditService.LogAsync("Encryption", null, null, 
                $"Data encrypted using {request.Algorithm}", AuditSeverity.Information);

            return Ok(new EncryptionResponse
            {
                EncryptedData = result.EncryptedData,
                Algorithm = result.Algorithm,
                KeyId = result.KeyId,
                IV = result.IV,
                Success = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Encryption failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("decrypt")]
    public async Task<ActionResult<DecryptionResponse>> Decrypt([FromBody] DecryptionRequest request)
    {
        try
        {
            var result = await _encryptionService.DecryptAsync(request.EncryptedData, request.KeyId, 
                request.Algorithm, request.IV);

            await _auditService.LogAsync("Decryption", null, request.KeyId, 
                "Data decrypted", AuditSeverity.Information);

            return Ok(new DecryptionResponse
            {
                DecryptedData = result.DecryptedData,
                Success = result.Success,
                ErrorMessage = result.ErrorMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Decryption failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("hash")]
    public async Task<ActionResult<HashResult>> Hash([FromBody] HashRequest request)
    {
        try
        {
            var result = await _encryptionService.HashAsync(request.Data, request.Algorithm);
            
            await _auditService.LogAsync("Hashing", null, null, 
                $"Data hashed using {request.Algorithm}", AuditSeverity.Information);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hashing failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("verify-hash")]
    public async Task<ActionResult<bool>> VerifyHash([FromBody] HashVerificationRequest request)
    {
        try
        {
            var result = await _encryptionService.VerifyHashAsync(request.Data, request.Hash, request.Algorithm);
            return Ok(new { verified = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hash verification failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    // Key Management Endpoints
    [HttpPost("keys/generate")]
    public async Task<ActionResult<KeyManagementResult>> GenerateKey([FromBody] KeyGenerationRequest request)
    {
        try
        {
            var result = await _keyManagementService.GenerateKeyAsync(request.KeyType, request.KeySize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Key generation failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("keys/{keyId}/rotate")]
    public async Task<ActionResult<KeyRotationResult>> RotateKey(string keyId)
    {
        try
        {
            var result = await _keyManagementService.RotateKeyAsync(keyId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Key rotation failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("keys")]
    public async Task<ActionResult<List<KeyManagementResult>>> ListKeys()
    {
        try
        {
            var keys = await _keyManagementService.ListKeysAsync();
            return Ok(keys);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list keys");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("keys/{keyId}")]
    public async Task<ActionResult<KeyManagementResult>> GetKeyInfo(string keyId)
    {
        try
        {
            var result = await _keyManagementService.GetKeyInfoAsync(keyId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get key info");
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("keys/{keyId}/backup")]
    public async Task<ActionResult<KeyBackupResult>> BackupKey(string keyId)
    {
        try
        {
            var result = await _keyManagementService.BackupKeyAsync(keyId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Key backup failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("keys/{keyId}")]
    public async Task<ActionResult> RevokeKey(string keyId)
    {
        try
        {
            var result = await _keyManagementService.RevokeKeyAsync(keyId);
            return result ? Ok(new { message = "Key revoked successfully" }) 
                         : NotFound(new { error = "Key not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Key revocation failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    // GDPR Compliance Endpoints
    [HttpPost("gdpr/request")]
    public async Task<ActionResult<GdprRequest>> SubmitGdprRequest([FromBody] GdprRequestSubmission request)
    {
        try
        {
            var result = await _gdprService.SubmitRequestAsync(request.SubjectId, request.RequestType, request.Reason);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GDPR request submission failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("gdpr/export/{subjectId}")]
    public async Task<ActionResult<GdprDataExport>> ExportData(string subjectId)
    {
        try
        {
            var result = await _gdprService.ExportDataAsync(subjectId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Data export failed");
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpDelete("gdpr/erase/{subjectId}")]
    public async Task<ActionResult> EraseData(string subjectId)
    {
        try
        {
            var result = await _gdprService.EraseDataAsync(subjectId);
            return result ? Ok(new { message = "Data erased successfully" }) 
                         : NotFound(new { error = "Subject not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Data erasure failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("gdpr/consent")]
    public async Task<ActionResult> UpdateConsent([FromBody] ConsentUpdateRequest request)
    {
        try
        {
            var result = await _gdprService.UpdateConsentAsync(request.SubjectId, request.Purpose, request.Granted);
            return Ok(new { success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Consent update failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("gdpr/consents/{subjectId}")]
    public async Task<ActionResult<List<ConsentRecord>>> GetConsents(string subjectId)
    {
        try
        {
            var consents = await _gdprService.GetConsentsAsync(subjectId);
            return Ok(consents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve consents");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("gdpr/compliance-report")]
    public async Task<ActionResult<ComplianceReport>> GetComplianceReport(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        try
        {
            var report = await _gdprService.GenerateComplianceReportAsync(startDate, endDate);
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Compliance report generation failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    // Anonymization Endpoints
    [HttpPost("anonymize")]
    public async Task<ActionResult<AnonymizationResult>> Anonymize([FromBody] AnonymizationRequest request)
    {
        try
        {
            var result = await _anonymizationService.AnonymizeAsync(request);
            
            await _auditService.LogAsync("Anonymization", null, null, 
                $"Data anonymized using {request.Method}", AuditSeverity.Information);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Anonymization failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("anonymize/field")]
    public async Task<ActionResult<string>> AnonymizeField([FromBody] FieldAnonymizationRequest request)
    {
        try
        {
            var result = await _anonymizationService.AnonymizeFieldAsync(request.Value, request.Method);
            return Ok(new { anonymizedValue = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Field anonymization failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("anonymize/bulk")]
    public async Task<ActionResult<Dictionary<string, string>>> BulkAnonymize([FromBody] BulkAnonymizationRequest request)
    {
        try
        {
            var result = await _anonymizationService.BulkAnonymizeAsync(request.Data, request.Method);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bulk anonymization failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    // Audit Logging Endpoints
    [HttpPost("audit/log")]
    public async Task<ActionResult<AuditLogEntry>> CreateAuditLog([FromBody] AuditLogRequest request)
    {
        try
        {
            var result = await _auditService.LogAsync(request.EventType, request.UserId, request.ResourceId, 
                request.Action, request.Severity, request.Metadata);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Audit log creation failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("audit/query")]
    public async Task<ActionResult<AuditQueryResult>> QueryAuditLogs([FromBody] AuditQueryRequest request)
    {
        try
        {
            var result = await _auditService.QueryLogsAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Audit log query failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("audit/verify/{logId}")]
    public async Task<ActionResult> VerifyLogIntegrity(string logId)
    {
        try
        {
            var result = await _auditService.VerifyLogIntegrityAsync(logId);
            return Ok(new { verified = result, message = result ? "Log integrity verified" : "Log tampering detected" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Log verification failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("audit/export")]
    public async Task<ActionResult<List<AuditLogEntry>>> ExportAuditLogs(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        try
        {
            var logs = await _auditService.ExportLogsAsync(startDate, endDate);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Audit log export failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    // Health & Status Endpoints
    [HttpGet("health")]
    public ActionResult GetHealth()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            services = new
            {
                encryption = "operational",
                keyManagement = "operational",
                gdpr = "operational",
                anonymization = "operational",
                audit = "operational"
            }
        });
    }
}

// Request DTOs
public class HashRequest
{
    public string Data { get; set; } = string.Empty;
    public HashAlgorithm Algorithm { get; set; }
}

public class HashVerificationRequest
{
    public string Data { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public HashAlgorithm Algorithm { get; set; }
}

public class KeyGenerationRequest
{
    public string KeyType { get; set; } = "AES";
    public int KeySize { get; set; } = 256;
}

public class GdprRequestSubmission
{
    public string SubjectId { get; set; } = string.Empty;
    public GdprRequestType RequestType { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ConsentUpdateRequest
{
    public string SubjectId { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public bool Granted { get; set; }
}

public class FieldAnonymizationRequest
{
    public string Value { get; set; } = string.Empty;
    public AnonymizationMethod Method { get; set; }
}

public class BulkAnonymizationRequest
{
    public Dictionary<string, string> Data { get; set; } = new();
    public AnonymizationMethod Method { get; set; }
}

public class AuditLogRequest
{
    public string EventType { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? ResourceId { get; set; }
    public string Action { get; set; } = string.Empty;
    public AuditSeverity Severity { get; set; } = AuditSeverity.Information;
    public Dictionary<string, object>? Metadata { get; set; }
}
