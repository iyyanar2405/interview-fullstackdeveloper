# AuthTokenApi - Authorization Token Endpoint Example

This example demonstrates a proper implementation of an authorization token endpoint in ASP.NET Core, specifically addressing the **405 Method Not Allowed** error that occurs when using the wrong HTTP method.

## Problem Statement

When accessing `/api/authorize/token` with the wrong HTTP method (e.g., GET instead of POST), you'll receive:

```
Failed to load resource: the server responded with a status of 405 (Method Not Allowed)
```

This is because the endpoint only accepts POST requests, as is standard for authentication/authorization endpoints.

## Solution

The endpoint is implemented as a **POST** endpoint using ASP.NET Core Minimal API:

```csharp
app.MapPost("/api/authorize/token", (TokenRequest request) =>
{
    // Validate and return token
});
```

## Running the Application

### Build and Run

```bash
dotnet build
dotnet run
```

The application will start on **http://localhost:7136**

### Test the Endpoint

**Incorrect (GET - Returns 405):**
```bash
curl http://localhost:7136/api/authorize/token
```

Response:
```
HTTP/1.1 405 Method Not Allowed
Allow: POST
```

**Correct (POST - Returns 200):**
```bash
curl -X POST http://localhost:7136/api/authorize/token \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"testpass"}'
```

Response:
```json
{
  "accessToken": "testuser_xxx...",
  "tokenType": "Bearer",
  "expiresIn": 3600
}
```

**Error Case (Missing Credentials):**
```bash
curl -X POST http://localhost:7136/api/authorize/token \
  -H "Content-Type: application/json" \
  -d '{"username":"","password":""}'
```

Response:
```json
{
  "error": "Username and password are required"
}
```

## API Endpoints

### POST /api/authorize/token

Generates an authorization token for valid credentials.

**Request:**
```json
{
  "username": "string",
  "password": "string"
}
```

**Response (Success - 200 OK):**
```json
{
  "accessToken": "string",
  "tokenType": "Bearer",
  "expiresIn": 3600
}
```

**Response (Error - 400 Bad Request):**
```json
{
  "error": "Username and password are required"
}
```

### GET /weatherforecast

Sample endpoint to verify the API is running.

## Key Concepts

### HTTP Methods and 405 Error

- **405 Method Not Allowed**: Returned when a valid endpoint is accessed with an unsupported HTTP method
- Authorization endpoints typically require **POST** to securely send credentials in the request body
- **GET** requests expose data in URLs (logged in proxies, browser history) - insecure for credentials

### Token-Based Authentication Flow

1. Client sends credentials via POST to `/api/authorize/token`
2. Server validates credentials
3. Server generates and returns an access token
4. Client includes token in subsequent requests via `Authorization: Bearer <token>` header

## Notes

- This is a **demo implementation** using simple token generation
- In production, use proper JWT tokens with signing and validation
- Consider using ASP.NET Core Identity or IdentityServer for production authentication
- Always use HTTPS in production to encrypt credentials in transit
- Store passwords securely using proper hashing (e.g., BCrypt, PBKDF2)

## Swagger UI

The API includes Swagger documentation. Access it at:
```
http://localhost:7136/swagger
```

You can test the endpoints interactively through the Swagger UI.

## Related Documentation

- [ASP.NET Core Authentication](https://learn.microsoft.com/aspnet/core/security/authentication/)
- [ASP.NET Core Authorization](https://learn.microsoft.com/aspnet/core/security/authorization/)
- [JWT Bearer Authentication](https://learn.microsoft.com/aspnet/core/security/authentication/jwt-authn)
