# CoreHub CRM API Documentation

## Base URL
```
https://localhost:5001/api
```

## Authentication
Currently, the API does not require authentication. JWT authentication will be implemented in Phase 2.

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
