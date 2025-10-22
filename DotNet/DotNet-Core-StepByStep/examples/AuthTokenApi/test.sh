#!/bin/bash

# Test script for AuthTokenApi
# This script demonstrates the 405 error and the correct usage

API_URL="http://localhost:7136"
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "========================================"
echo "AuthTokenApi Test Suite"
echo "========================================"
echo ""

# Test 1: GET request (should return 405)
echo -e "${YELLOW}Test 1: GET /api/authorize/token (Should return 405)${NC}"
echo "Command: curl -i http://localhost:7136/api/authorize/token"
echo ""
HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:7136/api/authorize/token)
if [ "$HTTP_CODE" == "405" ]; then
    echo -e "${GREEN}✓ PASS: Correctly returns 405 Method Not Allowed${NC}"
    curl -i http://localhost:7136/api/authorize/token 2>&1 | grep -E "HTTP|Allow"
else
    echo -e "${RED}✗ FAIL: Expected 405, got $HTTP_CODE${NC}"
fi
echo ""
echo "========================================"
echo ""

# Test 2: POST request with valid credentials (should return 200)
echo -e "${YELLOW}Test 2: POST /api/authorize/token with valid credentials (Should return 200)${NC}"
echo "Command: curl -X POST http://localhost:7136/api/authorize/token -H 'Content-Type: application/json' -d '{\"username\":\"testuser\",\"password\":\"testpass\"}'"
echo ""
RESPONSE=$(curl -s -w "\n%{http_code}" -X POST http://localhost:7136/api/authorize/token \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"testpass"}')
HTTP_CODE=$(echo "$RESPONSE" | tail -n 1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" == "200" ]; then
    echo -e "${GREEN}✓ PASS: Successfully returned token${NC}"
    echo "Response:"
    echo "$BODY" | python3 -m json.tool 2>/dev/null || echo "$BODY"
else
    echo -e "${RED}✗ FAIL: Expected 200, got $HTTP_CODE${NC}"
    echo "Response: $BODY"
fi
echo ""
echo "========================================"
echo ""

# Test 3: POST request with empty credentials (should return 400)
echo -e "${YELLOW}Test 3: POST /api/authorize/token with empty credentials (Should return 400)${NC}"
echo "Command: curl -X POST http://localhost:7136/api/authorize/token -H 'Content-Type: application/json' -d '{\"username\":\"\",\"password\":\"\"}'"
echo ""
RESPONSE=$(curl -s -w "\n%{http_code}" -X POST http://localhost:7136/api/authorize/token \
  -H "Content-Type: application/json" \
  -d '{"username":"","password":""}')
HTTP_CODE=$(echo "$RESPONSE" | tail -n 1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" == "400" ]; then
    echo -e "${GREEN}✓ PASS: Correctly returns 400 Bad Request${NC}"
    echo "Response:"
    echo "$BODY" | python3 -m json.tool 2>/dev/null || echo "$BODY"
else
    echo -e "${RED}✗ FAIL: Expected 400, got $HTTP_CODE${NC}"
    echo "Response: $BODY"
fi
echo ""
echo "========================================"
echo ""

# Test 4: Verify Swagger is accessible
echo -e "${YELLOW}Test 4: GET /swagger (Should return 200)${NC}"
HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:7136/swagger/index.html)
if [ "$HTTP_CODE" == "200" ]; then
    echo -e "${GREEN}✓ PASS: Swagger UI is accessible at http://localhost:7136/swagger${NC}"
else
    echo -e "${RED}✗ FAIL: Swagger UI not accessible, got $HTTP_CODE${NC}"
fi
echo ""
echo "========================================"
echo ""

echo "All tests completed!"
