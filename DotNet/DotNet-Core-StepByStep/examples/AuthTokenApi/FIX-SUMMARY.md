# Fix Summary: 405 Method Not Allowed Error

## Problem
The error message `:7136/api/authorize/token:1 Failed to load resource: the server responded with a status of 405 (Method Not Allowed)` indicated that an HTTP endpoint was being accessed with an incorrect HTTP method.

## Root Cause
A 405 (Method Not Allowed) error occurs when:
1. An endpoint exists but doesn't support the HTTP method being used
2. Most commonly, trying to use GET instead of POST for endpoints that require POST (like authentication endpoints)

## Solution Implemented

Created a complete ASP.NET Core Web API example (`AuthTokenApi`) that demonstrates:

1. **Proper POST endpoint** for `/api/authorize/token` that:
   - Accepts POST requests with username and password
   - Returns an authorization token on success
   - Returns proper error messages for invalid requests
   - Correctly returns 405 for GET requests

2. **Configuration**:
   - Application runs on port 7136 (as specified in the problem)
   - Includes Swagger UI for API documentation
   - Uses ASP.NET Core Minimal API pattern

3. **Documentation**:
   - Comprehensive README explaining the 405 error
   - Example curl commands for testing
   - Explanation of correct vs incorrect HTTP methods

4. **Testing**:
   - Automated test script (`test.sh`) that validates:
     - GET request returns 405 (demonstrates the error)
     - POST request with valid credentials returns 200 (correct usage)
     - POST request with empty credentials returns 400 (validation)
     - Swagger UI is accessible

## Files Changed/Added

### New Files
- `DotNet/DotNet-Core-StepByStep/examples/AuthTokenApi/Program.cs` - Main application
- `DotNet/DotNet-Core-StepByStep/examples/AuthTokenApi/README.md` - Documentation
- `DotNet/DotNet-Core-StepByStep/examples/AuthTokenApi/test.sh` - Test script
- `DotNet/DotNet-Core-StepByStep/examples/AuthTokenApi/.gitignore` - Build exclusions
- `.gitignore` - Repository-wide build artifact exclusions

### Modified Files
- `interview-fullstackdeveloper.sln` - Added AuthTokenApi project
- `DotNet/DotNet-Core-StepByStep/examples/AuthTokenApi/AuthTokenApi.http` - HTTP test requests
- `DotNet/DotNet-Core-StepByStep/examples/AuthTokenApi/Properties/launchSettings.json` - Port configuration

## Key Features

### Endpoint: POST /api/authorize/token

**Request:**
```json
{
  "username": "string",
  "password": "string"
}
```

**Success Response (200 OK):**
```json
{
  "accessToken": "user_token_here",
  "tokenType": "Bearer",
  "expiresIn": 3600
}
```

**Error Response (400 Bad Request):**
```json
{
  "error": "Username and password are required"
}
```

**Wrong Method (405 Method Not Allowed):**
- Using GET instead of POST returns 405 with `Allow: POST` header

## Testing Results

All tests pass successfully:
- ✓ GET request correctly returns 405 Method Not Allowed
- ✓ POST request with valid credentials returns 200 OK with token
- ✓ POST request with empty credentials returns 400 Bad Request
- ✓ Swagger UI is accessible at http://localhost:7136/swagger

## Security

- CodeQL security scan passed with 0 vulnerabilities
- Credentials are sent in request body (not URL) as per best practices
- Simple token generation used for demo (production should use JWT)
- Input validation implemented for credentials

## Running the Example

```bash
# Build
cd DotNet/DotNet-Core-StepByStep/examples/AuthTokenApi
dotnet build

# Run
dotnet run

# Test (in another terminal)
./test.sh
```

## Educational Value

This example demonstrates:
1. Understanding HTTP methods and status codes
2. Proper API endpoint design for authentication
3. Why authentication endpoints use POST instead of GET
4. How to diagnose and fix 405 errors
5. ASP.NET Core Minimal API patterns
6. API testing and documentation

## Next Steps

For production use, consider:
- Implementing proper JWT token generation with signing
- Using ASP.NET Core Identity for user management
- Adding password hashing (BCrypt, PBKDF2)
- Implementing token refresh mechanisms
- Adding rate limiting for authentication endpoints
- Always use HTTPS in production
