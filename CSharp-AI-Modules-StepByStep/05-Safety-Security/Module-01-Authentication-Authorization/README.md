# Module 01: Authentication & Authorization

Complete authentication and authorization module for .NET 8.0 applications with JWT, OAuth 2.0, RBAC, API Keys, and MFA.

## Features

### 🔐 Authentication Methods
- **JWT (JSON Web Tokens)**: Secure access and refresh tokens with configurable expiration
- **OAuth 2.0**: Integration with Google, Microsoft, and GitHub
- **Username/Password**: BCrypt password hashing with account lockout
- **API Keys**: Scoped API keys with rate limiting and IP whitelisting
- **Multi-Factor Authentication (MFA)**: TOTP-based 2FA with backup codes

### 👥 Authorization
- **Role-Based Access Control (RBAC)**: Flexible role and permission management
- **Custom Permissions**: Fine-grained permission system
- **Policy-Based Authorization**: Attribute-based access control

### 🛡️ Security Features
- Password reset with secure tokens
- Account lockout after failed login attempts
- Refresh token rotation
- IP address and user agent tracking
- Security audit logging
- Rate limiting per endpoint
- CORS configuration
- Security headers

## Project Structure

```
Module-01-Authentication-Authorization/
├── Models/
│   └── AuthenticationModels.cs          # All domain models
├── Services/
│   ├── JwtTokenService.cs               # JWT token management
│   ├── AuthenticationService.cs         # Core authentication logic
│   ├── OAuthService.cs                  # OAuth 2.0 providers
│   ├── MfaService.cs                    # Multi-factor authentication
│   ├── RbacService.cs                   # Role-based access control
│   └── ApiKeyService.cs                 # API key management
├── Controllers/
│   └── AuthenticationController.cs      # REST API endpoints
├── Extensions/
│   └── ServiceExtensions.cs             # Dependency injection setup
├── appsettings.json                     # Configuration
├── Program.cs                           # Application entry point
└── Module-01-Authentication-Authorization.csproj
```

## Dependencies

- Microsoft.AspNetCore.Authentication.JwtBearer 8.0.0
- Microsoft.AspNetCore.Authentication.Google 8.0.0
- Microsoft.AspNetCore.Authentication.MicrosoftAccount 8.0.0
- AspNet.Security.OAuth.GitHub 8.0.0
- Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.0
- BCrypt.Net-Next 4.0.3
- System.IdentityModel.Tokens.Jwt 7.0.3
- Otp.NET 1.3.0 (TOTP)
- QRCoder 1.4.3
- AspNetCoreRateLimit 5.0.0
- NWebsec.AspNetCore.Middleware 3.0.0
- FluentValidation.AspNetCore 11.3.0

## Installation

1. **Install NuGet packages:**
```bash
dotnet restore
```

2. **Configure settings in appsettings.json:**
```json
{
  "Jwt": {
    "Secret": "your-secret-key-min-32-characters",
    "Issuer": "YourApp",
    "Audience": "YourApp-Users",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "OAuth": {
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    }
  }
}
```

3. **Run the application:**
```bash
dotnet run
```

## API Endpoints

### Authentication

#### Register
```http
POST /api/authentication/register
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890"
}
```

#### Login
```http
POST /api/authentication/login
Content-Type: application/json

{
  "username": "john_doe",
  "password": "SecurePass123!",
  "mfaCode": "123456"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "a1b2c3d4e5f6...",
  "expiresAt": "2024-01-01T12:00:00Z",
  "user": {
    "id": "guid",
    "username": "john_doe",
    "email": "john@example.com",
    "roles": ["User"],
    "permissions": ["profile.read", "profile.write"]
  }
}
```

#### Refresh Token
```http
POST /api/authentication/refresh
Content-Type: application/json

{
  "refreshToken": "a1b2c3d4e5f6..."
}
```

#### Logout
```http
POST /api/authentication/logout
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "refreshToken": "a1b2c3d4e5f6..."
}
```

#### Change Password
```http
POST /api/authentication/change-password
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "currentPassword": "OldPass123!",
  "newPassword": "NewPass456!"
}
```

#### Forgot Password
```http
POST /api/authentication/forgot-password
Content-Type: application/json

{
  "email": "john@example.com"
}
```

#### Reset Password
```http
POST /api/authentication/reset-password
Content-Type: application/json

{
  "token": "reset-token-from-email",
  "newPassword": "NewPass456!"
}
```

### OAuth 2.0

#### Google Authentication
```http
POST /api/authentication/oauth/google
Content-Type: application/json

{
  "code": "google-auth-code",
  "redirectUri": "https://yourapp.com/callback"
}
```

#### Microsoft Authentication
```http
POST /api/authentication/oauth/microsoft
Content-Type: application/json

{
  "code": "microsoft-auth-code",
  "redirectUri": "https://yourapp.com/callback"
}
```

#### GitHub Authentication
```http
POST /api/authentication/oauth/github
Content-Type: application/json

{
  "code": "github-auth-code",
  "redirectUri": "https://yourapp.com/callback"
}
```

#### Get OAuth Providers
```http
GET /api/authentication/oauth/providers
Authorization: Bearer {access_token}
```

#### Link OAuth Provider
```http
POST /api/authentication/oauth/link
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "provider": "Google",
  "providerUserId": "google-user-id"
}
```

#### Unlink OAuth Provider
```http
POST /api/authentication/oauth/unlink
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "provider": "Google"
}
```

### Multi-Factor Authentication (MFA)

#### Setup MFA
```http
POST /api/authentication/mfa/setup
Authorization: Bearer {access_token}
```

**Response:**
```json
{
  "success": true,
  "message": "Scan the QR code with your authenticator app",
  "secret": "BASE32_SECRET",
  "qrCodeUrl": "data:image/png;base64,...",
  "backupCodes": [
    "ABCD1234",
    "EFGH5678",
    "..."
  ]
}
```

#### Enable MFA
```http
POST /api/authentication/mfa/enable
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "code": "123456"
}
```

#### Disable MFA
```http
POST /api/authentication/mfa/disable
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "password": "YourPassword123!"
}
```

#### Regenerate Backup Codes
```http
POST /api/authentication/mfa/regenerate-backup-codes
Authorization: Bearer {access_token}
```

### Role-Based Access Control (RBAC)

#### Assign Role (Admin Only)
```http
POST /api/authentication/roles/assign
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "userId": "user-guid",
  "roleName": "Manager"
}
```

#### Remove Role (Admin Only)
```http
POST /api/authentication/roles/remove
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "userId": "user-guid",
  "roleName": "Manager"
}
```

#### Get All Roles (Admin Only)
```http
GET /api/authentication/roles
Authorization: Bearer {access_token}
```

#### Assign Permission (Admin Only)
```http
POST /api/authentication/permissions/assign
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "userId": "user-guid",
  "permission": "reports.read"
}
```

#### Check Permission
```http
POST /api/authentication/permissions/check
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "permission": "reports.read"
}
```

**Response:**
```json
{
  "hasPermission": true,
  "message": "Permission granted",
  "userId": "user-guid",
  "permission": "reports.read",
  "checkedAt": "2024-01-01T12:00:00Z"
}
```

### API Keys

#### Create API Key
```http
POST /api/authentication/api-keys
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "name": "Production API Key",
  "scopes": ["api.read", "api.write"],
  "ipWhitelist": ["192.168.1.1", "10.0.0.0/8"],
  "rateLimitConfig": {
    "requestsPerMinute": 60,
    "requestsPerHour": 1000,
    "requestsPerDay": 10000
  },
  "expiresAt": "2025-01-01T00:00:00Z"
}
```

**Response:**
```json
{
  "id": "api-key-guid",
  "key": "sk_AbCdEfGh123456...",
  "name": "Production API Key",
  "keyPrefix": "sk_AbCdE",
  "scopes": ["api.read", "api.write"],
  "expiresAt": "2025-01-01T00:00:00Z",
  "createdAt": "2024-01-01T12:00:00Z"
}
```

#### Get API Keys
```http
GET /api/authentication/api-keys
Authorization: Bearer {access_token}
```

#### Revoke API Key
```http
POST /api/authentication/api-keys/{id}/revoke
Authorization: Bearer {access_token}
```

#### Rotate API Key
```http
POST /api/authentication/api-keys/{id}/rotate
Authorization: Bearer {access_token}
```

## Usage Examples

### C# Client Example

```csharp
using System.Net.Http.Json;

var client = new HttpClient { BaseAddress = new Uri("https://localhost:5001") };

// Register
var registerRequest = new
{
    username = "john_doe",
    email = "john@example.com",
    password = "SecurePass123!",
    firstName = "John",
    lastName = "Doe"
};

var registerResponse = await client.PostAsJsonAsync("/api/authentication/register", registerRequest);
var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

// Login
var loginRequest = new
{
    username = "john_doe",
    password = "SecurePass123!"
};

var loginResponse = await client.PostAsJsonAsync("/api/authentication/login", loginRequest);
authResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

// Use access token for authenticated requests
client.DefaultRequestHeaders.Authorization = 
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.AccessToken);

// Make authenticated request
var userProfile = await client.GetFromJsonAsync<UserInfo>("/api/profile");
```

### JavaScript/TypeScript Example

```typescript
// Register
const registerResponse = await fetch('https://localhost:5001/api/authentication/register', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'john_doe',
    email: 'john@example.com',
    password: 'SecurePass123!',
    firstName: 'John',
    lastName: 'Doe'
  })
});

const authResult = await registerResponse.json();

// Login
const loginResponse = await fetch('https://localhost:5001/api/authentication/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'john_doe',
    password: 'SecurePass123!'
  })
});

const loginResult = await loginResponse.json();

// Store tokens
localStorage.setItem('accessToken', loginResult.accessToken);
localStorage.setItem('refreshToken', loginResult.refreshToken);

// Make authenticated request
const response = await fetch('https://localhost:5001/api/profile', {
  headers: {
    'Authorization': `Bearer ${loginResult.accessToken}`
  }
});
```

### OAuth 2.0 Flow Example

```csharp
// 1. Redirect user to OAuth provider
var googleAuthUrl = "https://accounts.google.com/o/oauth2/v2/auth?" +
    "client_id=YOUR_CLIENT_ID&" +
    "redirect_uri=https://yourapp.com/callback&" +
    "response_type=code&" +
    "scope=openid email profile";

// 2. Handle callback and exchange code for tokens
var oauthRequest = new
{
    code = codeFromCallback,
    redirectUri = "https://yourapp.com/callback"
};

var response = await client.PostAsJsonAsync("/api/authentication/oauth/google", oauthRequest);
var authResult = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

// User is now authenticated with JWT tokens
```

### MFA Setup Example

```csharp
// 1. Setup MFA
var setupResponse = await client.PostAsync("/api/authentication/mfa/setup", null);
var mfaSetup = await setupResponse.Content.ReadFromJsonAsync<MfaSetupResponse>();

// 2. Display QR code to user
Console.WriteLine($"Secret: {mfaSetup.Secret}");
Console.WriteLine($"QR Code: {mfaSetup.QrCodeUrl}");
Console.WriteLine("Backup Codes:");
foreach (var code in mfaSetup.BackupCodes)
{
    Console.WriteLine(code);
}

// 3. Verify and enable MFA
var verifyRequest = new { code = "123456" }; // From authenticator app
var enableResponse = await client.PostAsJsonAsync("/api/authentication/mfa/enable", verifyRequest);

// 4. Login with MFA
var loginRequest = new
{
    username = "john_doe",
    password = "SecurePass123!",
    mfaCode = "123456" // From authenticator app
};
```

## Security Best Practices

### 1. JWT Secret Configuration
- Use a strong, random secret key (minimum 32 characters)
- Store secrets in environment variables or Azure Key Vault
- Rotate JWT secrets periodically
- Use different secrets for development and production

### 2. Password Security
- Enforce strong password policies
- Use BCrypt with appropriate work factor
- Implement account lockout after failed attempts
- Support password history to prevent reuse

### 3. OAuth Configuration
- Register your application with each OAuth provider
- Use HTTPS for redirect URIs
- Validate state parameter to prevent CSRF
- Store OAuth tokens securely

### 4. MFA Implementation
- Require MFA for admin accounts
- Provide backup codes for account recovery
- Support multiple MFA methods (TOTP, SMS, email)
- Rate limit MFA verification attempts

### 5. API Key Management
- Generate cryptographically secure keys
- Hash keys before storage
- Implement key rotation
- Use scoped permissions
- Monitor API key usage

### 6. Rate Limiting
- Apply rate limits per endpoint
- Use stricter limits for authentication endpoints
- Implement IP-based blocking for abuse
- Monitor rate limit violations

### 7. Audit Logging
- Log all authentication attempts
- Track permission changes
- Monitor suspicious activities
- Implement log retention policies

## Configuration Guide

### JWT Configuration

```json
{
  "Jwt": {
    "Secret": "your-256-bit-secret-key",
    "Issuer": "YourApp",
    "Audience": "YourApp-Users",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

### OAuth Provider Setup

#### Google OAuth Setup
1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project
3. Enable Google+ API
4. Create OAuth 2.0 credentials
5. Add authorized redirect URIs
6. Copy Client ID and Secret to appsettings.json

#### Microsoft OAuth Setup
1. Go to [Azure Portal](https://portal.azure.com/)
2. Register a new application
3. Configure redirect URIs
4. Create client secret
5. Copy Application ID and Secret to appsettings.json

#### GitHub OAuth Setup
1. Go to GitHub Settings → Developer settings
2. Create new OAuth App
3. Set authorization callback URL
4. Copy Client ID and Secret to appsettings.json

### Rate Limiting Configuration

```json
{
  "RateLimit": {
    "EnableEndpointRateLimiting": true,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 60
      },
      {
        "Endpoint": "*/api/authentication/login",
        "Period": "5m",
        "Limit": 5
      }
    ]
  }
}
```

## Troubleshooting

### Common Issues

1. **JWT token validation fails**
   - Verify JWT secret matches in configuration
   - Check token expiration time
   - Ensure issuer and audience match

2. **OAuth authentication fails**
   - Verify OAuth provider credentials
   - Check redirect URI configuration
   - Ensure OAuth app is not in development mode

3. **MFA QR code not scanning**
   - Verify TOTP configuration (period, digits)
   - Check QR code image format
   - Try manual entry with secret key

4. **API key validation fails**
   - Check key format (should start with sk_)
   - Verify key is not revoked or expired
   - Check IP whitelist if configured

5. **Rate limiting too strict**
   - Adjust limits in appsettings.json
   - Consider implementing token bucket algorithm
   - Add endpoint-specific limits

## Production Deployment Checklist

- [ ] Use strong JWT secret (environment variable)
- [ ] Configure OAuth providers with production credentials
- [ ] Enable HTTPS/TLS
- [ ] Set up database for persistent storage
- [ ] Configure email service for notifications
- [ ] Enable security headers (CSP, HSTS, etc.)
- [ ] Implement proper error handling
- [ ] Set up monitoring and alerting
- [ ] Configure log aggregation
- [ ] Implement database migrations
- [ ] Test failover scenarios
- [ ] Document API endpoints
- [ ] Set up rate limiting
- [ ] Configure CORS properly
- [ ] Enable audit logging
- [ ] Test backup and recovery

## Testing

### Run Tests
```bash
dotnet test
```

### Test Coverage
```bash
dotnet test /p:CollectCoverage=true
```

### Example Unit Test

```csharp
[Fact]
public async Task Login_WithValidCredentials_ReturnsAuthenticationResponse()
{
    // Arrange
    var authService = new AuthenticationService(logger, jwtService, mfaService);
    var request = new LoginRequest
    {
        Username = "test_user",
        Password = "TestPass123!"
    };

    // Act
    var result = await authService.LoginAsync(request, null, null);

    // Assert
    Assert.True(result.Success);
    Assert.NotNull(result.AccessToken);
    Assert.NotNull(result.RefreshToken);
}
```

## Support

For issues, questions, or contributions, please open an issue on the repository.

## License

This module is provided as-is for educational and commercial use.

## Version History

- **1.0.0** - Initial release
  - JWT authentication
  - OAuth 2.0 (Google, Microsoft, GitHub)
  - RBAC with roles and permissions
  - API key management
  - Multi-factor authentication (TOTP)
  - Password management
  - Rate limiting
  - Security headers

## Related Modules

- Module 02: Data Encryption & Decryption
- Module 03: SSL/TLS Implementation
- Module 04: Security Scanning & Vulnerability Assessment
- Module 05: Secrets Management (Azure Key Vault, AWS Secrets Manager)
