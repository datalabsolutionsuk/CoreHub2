# CoreHub CRM API Documentation

## Base URL
```
https://localhost:5001/api
```

## Authentication

The API uses JWT (JSON Web Token) bearer authentication. Most endpoints require authentication via the `Authorization` header.

### Authentication Flow

1. **Register** or **Login** to get an access token
2. Include the token in all subsequent requests:
   ```
   Authorization: Bearer <your-access-token>
   ```
3. When the access token expires, use the refresh token to get a new one

### Example Request with Authentication

```http
GET /api/patients
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## Authentication Endpoints

### 1. Register a New User

```http
POST /api/auth/register
Content-Type: application/json
```

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecureP@ssw0rd!",
  "firstName": "John",
  "lastName": "Doe",
  "organizationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Password Requirements:**
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit
- At least one special character
- At least 4 unique characters

**Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "xYz123AbC456...",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "user": {
    "id": "user-id",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "organizationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "roles": ["User"]
  }
}
```

**Error Response (400 Bad Request):**
```json
{
  "error": "Registration failed",
  "details": [
    "Passwords must have at least one uppercase letter.",
    "Passwords must have at least one digit."
  ]
}
```

---

### 2. Login

```http
POST /api/auth/login
Content-Type: application/json
```

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecureP@ssw0rd!"
}
```

**Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "xYz123AbC456...",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "user": {
    "id": "user-id",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "organizationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "roles": ["User", "Practitioner"]
  }
}
```

**Error Response (401 Unauthorized):**
```json
{
  "error": "Invalid email or password"
}
```

**Account Lockout (401 Unauthorized):**
After 5 failed login attempts, the account is locked for 15 minutes.
```json
{
  "error": "Account is locked. Please try again later."
}
```

---

### 3. Refresh Token

When your access token expires, use the refresh token to get a new one without logging in again.

```http
POST /api/auth/refresh-token
Content-Type: application/json
```

**Request Body:**
```json
{
  "accessToken": "expired-access-token",
  "refreshToken": "your-refresh-token"
}
```

**Response (200 OK):**
```json
{
  "accessToken": "new-access-token",
  "refreshToken": "new-refresh-token",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "user": {
    "id": "user-id",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "organizationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "roles": ["User"]
  }
}
```

---

### 4. Get Current User

Get information about the currently authenticated user.

```http
GET /api/auth/me
Authorization: Bearer <your-access-token>
```

**Response (200 OK):**
```json
{
  "id": "user-id",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "organizationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "roles": ["User", "Practitioner"]
}
```

---

## Token Details

### Access Token
- **Lifetime**: 60 minutes (configurable)
- **Type**: JWT Bearer token
- **Contains**: User ID, email, organization ID, roles
- **Use**: Include in `Authorization` header for all authenticated requests

### Refresh Token
- **Lifetime**: Long-lived (typically 7-30 days)
- **Type**: Random secure string
- **Use**: Exchange for new access token when current one expires
- **Storage**: Should be stored securely (not in localStorage)

---

## Authorization & Data Isolation

### Organization-Level Isolation

All API endpoints automatically filter data by the user's organization. Users can only access and modify data belonging to their own organization.

**Example:**
```http
GET /api/patients
Authorization: Bearer <token-with-org-A>
```
Returns only patients from Organization A, even if Organization B has patients in the database.

### Roles

- **User**: Basic authenticated access
- **Practitioner**: Can manage patients, appointments, and clinical notes
- **Admin**: Full administrative access

### Protected Endpoints

All endpoints except authentication endpoints require a valid JWT token:

```
✓ Public:   POST /api/auth/register
✓ Public:   POST /api/auth/login
✓ Public:   POST /api/auth/refresh-token
✗ Protected: GET  /api/auth/me
✗ Protected: GET  /api/patients
✗ Protected: POST /api/patients
✗ Protected: PUT  /api/patients/{id}
✗ Protected: DELETE /api/patients/{id}
```

---

## Patients API

### Endpoints

#### 1. Get All Patients (Paginated)
```http
GET /api/patients
```

**Query Parameters:**
- `pageNumber` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10, max: 100)
- `searchTerm` (string, optional): Search by name, email, or phone
- `status` (string, optional): Filter by status (Active, Inactive, Archived)

**Response:**
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "firstName": "John",
      "lastName": "Doe",
      "middleName": null,
      "fullName": "John Doe",
      "dateOfBirth": "1985-06-15",
      "gender": "Male",
      "email": "john.doe@example.com",
      "phone": "555-1234",
      "mobilePhone": "555-5678",
      "address": "123 Main St",
      "city": "New York",
      "state": "NY",
      "postalCode": "10001",
      "country": "USA",
      "emergencyContactName": "Jane Doe",
      "emergencyContactPhone": "555-9999",
      "emergencyContactRelationship": "Spouse",
      "bloodType": "O+",
      "allergies": "Penicillin",
      "medicalHistory": "Hypertension",
      "currentMedications": "Lisinopril 10mg",
      "insuranceProvider": "Blue Cross",
      "insurancePolicyNumber": "BC123456",
      "insuranceExpiryDate": "2025-12-31",
      "primaryPractitionerId": "8fa85f64-5717-4562-b3fc-2c963f66afa7",
      "primaryPractitionerName": "Dr. Sarah Smith",
      "referralSource": "Google Search",
      "notes": "Prefers morning appointments",
      "tags": "VIP,Regular",
      "status": "Active",
      "createdAt": "2025-01-15T10:30:00Z",
      "updatedAt": "2025-01-28T14:20:00Z"
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 125,
  "totalPages": 13,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

**Status Codes:**
- `200 OK`: Success
- `400 Bad Request`: Invalid parameters

---

#### 2. Get Patient by ID
```http
GET /api/patients/{id}
```

**Path Parameters:**
- `id` (guid, required): Patient ID

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  ...
}
```

**Status Codes:**
- `200 OK`: Success
- `404 Not Found`: Patient not found
- `400 Bad Request`: Invalid ID format

---

#### 3. Create Patient
```http
POST /api/patients
```

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "middleName": "M",
  "dateOfBirth": "1985-06-15",
  "gender": "Male",
  "email": "john.doe@example.com",
  "phone": "555-1234",
  "mobilePhone": "555-5678",
  "address": "123 Main St",
  "city": "New York",
  "state": "NY",
  "postalCode": "10001",
  "country": "USA",
  "emergencyContactName": "Jane Doe",
  "emergencyContactPhone": "555-9999",
  "emergencyContactRelationship": "Spouse",
  "bloodType": "O+",
  "allergies": "Penicillin",
  "medicalHistory": "Hypertension",
  "currentMedications": "Lisinopril 10mg",
  "insuranceProvider": "Blue Cross",
  "insurancePolicyNumber": "BC123456",
  "insuranceExpiryDate": "2025-12-31",
  "primaryPractitionerId": "8fa85f64-5717-4562-b3fc-2c963f66afa7",
  "referralSource": "Google Search",
  "notes": "Prefers morning appointments",
  "tags": "VIP,Regular",
  "status": "Active"
}
```

**Required Fields:**
- `firstName` (string, max 100 chars)
- `lastName` (string, max 100 chars)
- `email` (valid email, max 255 chars)

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Status Codes:**
- `201 Created`: Patient created successfully
- `400 Bad Request`: Validation error or duplicate email

---

#### 4. Update Patient
```http
PUT /api/patients/{id}
```

**Path Parameters:**
- `id` (guid, required): Patient ID

**Request Body:**
Same as Create Patient, but with `id` field included

**Status Codes:**
- `204 No Content`: Update successful
- `400 Bad Request`: Validation error, ID mismatch, or duplicate email
- `404 Not Found`: Patient not found

---

#### 5. Delete Patient
```http
DELETE /api/patients/{id}
```

**Path Parameters:**
- `id` (guid, required): Patient ID

**Status Codes:**
- `204 No Content`: Deletion successful
- `404 Not Found`: Patient not found
- `400 Bad Request`: Invalid ID or cascade constraint violation

---

## Error Response Format

All error responses follow this structure:

```json
{
  "error": "Error message description",
  "details": "Additional technical details (development mode only)",
  "validationErrors": [
    {
      "field": "email",
      "message": "Invalid email address"
    }
  ]
}
```

---

## Validation Rules

### Patient Entity

**Email:**
- Must be valid email format
- Must be unique
- Maximum 255 characters

**Names:**
- First and last names are required
- Maximum 100 characters each

**Phone Numbers:**
- Must match pattern: `^\+?[\d\s\-\(\)]+$`
- Optional fields

**Date of Birth:**
- Must be in the past

**Status:**
- Must be one of: `Active`, `Inactive`, `Archived`
- Default: `Active`

**Insurance Expiry Date:**
- Should not be more than 1 month in the past

---

## Common Use Cases

### 1. Search for Patients
```http
GET /api/patients?searchTerm=john&status=Active&pageSize=20
```

### 2. Create a New Patient
```http
POST /api/patients
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  "phone": "555-1111",
  "status": "Active"
}
```

### 3. Update Patient Status
```http
PUT /api/patients/3fa85f64-5717-4562-b3fc-2c963f66afa6
Content-Type: application/json

{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  "status": "Inactive"
}
```

### 4. Get Patient Details with Related Data
```http
GET /api/patients/3fa85f64-5717-4562-b3fc-2c963f66afa6
```
Returns patient with appointments and primary practitioner information.

---

## Coming Soon (Phase 2)

- JWT Authentication
- Authorization roles (Admin, Practitioner, Staff)
- Appointments API
- Clinical Notes API
- Invoicing API
- Document Management API
- Advanced search and filtering
- Bulk operations
- Export functionality

---

## Rate Limiting

Currently, there are no rate limits. Rate limiting will be implemented in Phase 2.

---

## Support

For API support and questions, contact: support@corehub.com
