# CoreHub Database Schema

This document describes the database schema for the CoreHub Healthcare Practice Management System.

## Database Information

- **Database Type**: PostgreSQL 16
- **Database Name**: CoreHubDb
- **ORM**: Entity Framework Core 9.0

## Entity Relationship Diagram

```
Organizations ───┬──< Practitioners ──┬──< Appointments >──┬── Patients
                 │                    │                    │
                 ├──< Locations       │                    ├──< ClinicalNotes
                 │                    │                    │
                 ├──< NoteTemplates   ├──< ClinicalNotes   ├──< Invoices
                 │                    │                    │
                 └──< Invoices        └──< NoteTemplates   ├──< Documents
                                                           │
                                                           └──< Payments
```

## Tables

### Organizations

The root tenant entity for multi-tenancy support.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| Id | uuid | NOT NULL | Primary key |
| Name | varchar(200) | NOT NULL | Organization display name |
| LegalName | text | NULL | Legal registered name |
| RegistrationNumber | text | NULL | Business registration number |
| TaxId | text | NULL | Tax identification number |
| Email | varchar(255) | NOT NULL | Contact email |
| Phone | text | NULL | Contact phone |
| Website | text | NULL | Website URL |
| Address | text | NULL | Street address |
| City | text | NULL | City |
| State | text | NULL | State/Province |
| PostalCode | text | NULL | Postal/ZIP code |
| Country | text | NULL | Country |
| OrganizationType | varchar(100) | NOT NULL | Type (Clinic, Hospital, etc.) |
| Specialty | text | NULL | Primary specialty |
| LogoUrl | text | NULL | Logo image URL |
| PrimaryColor | text | NULL | Brand primary color |
| SecondaryColor | text | NULL | Brand secondary color |
| TimeZone | text | NULL | Organization timezone |
| Currency | text | NULL | Default currency |
| DateFormat | text | NULL | Date format preference |
| TimeFormat | text | NULL | Time format preference |
| SubscriptionTier | varchar(50) | NOT NULL | Subscription level |
| SubscriptionStartDate | timestamptz | NULL | Subscription start |
| SubscriptionEndDate | timestamptz | NULL | Subscription end |
| IsActive | boolean | NOT NULL | Active status |
| IsISO27001Certified | boolean | NOT NULL | ISO 27001 compliance |
| IsHIPAACompliant | boolean | NOT NULL | HIPAA compliance |
| ComplianceCertificates | text | NULL | JSON compliance data |
| CreatedAt | timestamptz | NOT NULL | Record creation time |
| UpdatedAt | timestamptz | NULL | Last update time |
| CreatedBy | text | NULL | Creator user ID |
| UpdatedBy | text | NULL | Last updater user ID |
| IsDeleted | boolean | NOT NULL | Soft delete flag |

**Indexes:**
- `PK_Organizations` PRIMARY KEY (Id)
- `IX_Organizations_Email` (Email)

---

### Practitioners

Healthcare practitioners belonging to an organization.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| Id | uuid | NOT NULL | Primary key |
| FirstName | varchar(100) | NOT NULL | First name |
| LastName | varchar(100) | NOT NULL | Last name |
| MiddleName | text | NULL | Middle name |
| Email | varchar(255) | NOT NULL | Unique email address |
| Phone | text | NULL | Phone number |
| MobilePhone | text | NULL | Mobile phone |
| Specialty | varchar(100) | NOT NULL | Medical specialty |
| LicenseNumber | text | NULL | License number |
| LicenseExpiryDate | timestamptz | NULL | License expiry |
| Qualifications | text | NULL | Professional qualifications |
| Bio | text | NULL | Biography/description |
| OrganizationId | uuid | NOT NULL | FK to Organizations |
| Title | text | NULL | Professional title |
| Designation | text | NULL | Designation |
| IsActive | boolean | NOT NULL | Active status |
| JoinedDate | timestamptz | NULL | Date joined organization |
| WorkingHours | text | NULL | JSON working hours |
| DefaultAppointmentDuration | integer | NOT NULL | Default apt duration (minutes) |
| PreferredColor | text | NULL | Calendar color |
| UserId | text | NULL | Associated user account ID |
| Role | text | NOT NULL | Practitioner role |
| CreatedAt | timestamptz | NOT NULL | Record creation time |
| UpdatedAt | timestamptz | NULL | Last update time |
| CreatedBy | text | NULL | Creator user ID |
| UpdatedBy | text | NULL | Last updater user ID |
| IsDeleted | boolean | NOT NULL | Soft delete flag |

**Indexes:**
- `PK_Practitioners` PRIMARY KEY (Id)
- `IX_Practitioners_Email` UNIQUE (Email)
- `IX_Practitioners_OrganizationId` (OrganizationId)

**Foreign Keys:**
- `FK_Practitioners_Organizations_OrganizationId` → Organizations(Id) ON DELETE RESTRICT

---

### Patients

Patient records with contact and medical information.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| Id | uuid | NOT NULL | Primary key |
| FirstName | varchar(100) | NOT NULL | First name |
| LastName | varchar(100) | NOT NULL | Last name |
| MiddleName | text | NULL | Middle name |
| DateOfBirth | timestamptz | NULL | Date of birth |
| Gender | text | NULL | Gender |
| Email | varchar(255) | NOT NULL | Email address |
| Phone | text | NULL | Phone number |
| MobilePhone | text | NULL | Mobile phone |
| Address | text | NULL | Street address |
| City | text | NULL | City |
| State | text | NULL | State/Province |
| PostalCode | text | NULL | Postal/ZIP code |
| Country | text | NULL | Country |
| EmergencyContactName | text | NULL | Emergency contact name |
| EmergencyContactPhone | text | NULL | Emergency contact phone |
| EmergencyContactRelationship | text | NULL | Emergency contact relationship |
| BloodType | text | NULL | Blood type |
| Allergies | text | NULL | Known allergies |
| MedicalHistory | text | NULL | Medical history |
| CurrentMedications | text | NULL | Current medications |
| InsuranceProvider | text | NULL | Insurance provider name |
| InsurancePolicyNumber | text | NULL | Insurance policy number |
| InsuranceExpiryDate | timestamptz | NULL | Insurance expiry date |
| PrimaryPractitionerId | uuid | NULL | FK to Practitioners |
| ReferralSource | text | NULL | How patient was referred |
| Notes | text | NULL | General notes |
| Tags | text | NULL | JSON tags |
| Status | varchar(50) | NOT NULL | Patient status |
| CreatedAt | timestamptz | NOT NULL | Record creation time |
| UpdatedAt | timestamptz | NULL | Last update time |
| CreatedBy | text | NULL | Creator user ID |
| UpdatedBy | text | NULL | Last updater user ID |
| IsDeleted | boolean | NOT NULL | Soft delete flag |

**Indexes:**
- `PK_Patients` PRIMARY KEY (Id)
- `IX_Patients_Email` (Email)
- `IX_Patients_FirstName_LastName` (FirstName, LastName)
- `IX_Patients_PrimaryPractitionerId` (PrimaryPractitionerId)

**Foreign Keys:**
- `FK_Patients_Practitioners_PrimaryPractitionerId` → Practitioners(Id) ON DELETE SET NULL

---

### Appointments

Scheduled appointments between patients and practitioners.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| Id | uuid | NOT NULL | Primary key |
| PatientId | uuid | NOT NULL | FK to Patients |
| PractitionerId | uuid | NOT NULL | FK to Practitioners |
| LocationId | uuid | NULL | FK to Locations |
| StartTime | timestamptz | NOT NULL | Appointment start time |
| EndTime | timestamptz | NOT NULL | Appointment end time |
| DurationMinutes | integer | NOT NULL | Duration in minutes |
| AppointmentType | varchar(100) | NOT NULL | Type of appointment |
| Status | varchar(50) | NOT NULL | Appointment status |
| ReminderSent | boolean | NOT NULL | Whether reminder sent |
| ReminderSentAt | timestamptz | NULL | When reminder was sent |
| ConfirmationSent | boolean | NOT NULL | Whether confirmation sent |
| ConfirmationSentAt | timestamptz | NULL | When confirmation sent |
| IsTelehealth | boolean | NOT NULL | Telehealth appointment flag |
| TelehealthLink | text | NULL | Telehealth meeting link |
| TelehealthProvider | text | NULL | Telehealth provider |
| Notes | text | NULL | Appointment notes |
| CancellationReason | text | NULL | Reason for cancellation |
| CancelledAt | timestamptz | NULL | When cancelled |
| EstimatedCost | numeric | NULL | Estimated cost |
| IsRecurring | boolean | NOT NULL | Recurring appointment flag |
| RecurrencePattern | text | NULL | Recurrence pattern JSON |
| ParentAppointmentId | uuid | NULL | Parent for recurring apts |
| IsWaitlisted | boolean | NOT NULL | Waitlist status |
| WaitlistPriority | integer | NULL | Waitlist priority |
| CreatedAt | timestamptz | NOT NULL | Record creation time |
| UpdatedAt | timestamptz | NULL | Last update time |
| CreatedBy | text | NULL | Creator user ID |
| UpdatedBy | text | NULL | Last updater user ID |
| IsDeleted | boolean | NOT NULL | Soft delete flag |

**Indexes:**
- `PK_Appointments` PRIMARY KEY (Id)
- `IX_Appointments_StartTime` (StartTime)
- `IX_Appointments_PatientId_StartTime` (PatientId, StartTime)
- `IX_Appointments_PractitionerId_StartTime` (PractitionerId, StartTime)
- `IX_Appointments_LocationId` (LocationId)

**Foreign Keys:**
- `FK_Appointments_Patients_PatientId` → Patients(Id) ON DELETE CASCADE
- `FK_Appointments_Practitioners_PractitionerId` → Practitioners(Id) ON DELETE RESTRICT
- `FK_Appointments_Locations_LocationId` → Locations(Id) ON DELETE SET NULL

---

### Locations

Physical locations belonging to an organization.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| Id | uuid | NOT NULL | Primary key |
| OrganizationId | uuid | NOT NULL | FK to Organizations |
| Name | varchar(200) | NOT NULL | Location name |
| Description | text | NULL | Description |
| LocationType | varchar(50) | NOT NULL | Type (Office, Clinic, etc.) |
| Address | text | NULL | Street address |
| City | text | NULL | City |
| State | text | NULL | State/Province |
| PostalCode | text | NULL | Postal/ZIP code |
| Country | text | NULL | Country |
| Phone | text | NULL | Phone number |
| Email | text | NULL | Email address |
| Capacity | integer | NULL | Maximum capacity |
| Facilities | text | NULL | JSON facilities list |
| IsAccessible | boolean | NOT NULL | Accessibility flag |
| IsActive | boolean | NOT NULL | Active status |
| WorkingHours | text | NULL | JSON working hours |
| CreatedAt | timestamptz | NOT NULL | Record creation time |
| UpdatedAt | timestamptz | NULL | Last update time |
| CreatedBy | text | NULL | Creator user ID |
| UpdatedBy | text | NULL | Last updater user ID |
| IsDeleted | boolean | NOT NULL | Soft delete flag |

**Indexes:**
- `PK_Locations` PRIMARY KEY (Id)
- `IX_Locations_OrganizationId` (OrganizationId)

**Foreign Keys:**
- `FK_Locations_Organizations_OrganizationId` → Organizations(Id) ON DELETE CASCADE

---

### ClinicalNotes

Clinical notes for patient encounters.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| Id | uuid | NOT NULL | Primary key |
| PatientId | uuid | NOT NULL | FK to Patients |
| PractitionerId | uuid | NOT NULL | FK to Practitioners |
| AppointmentId | uuid | NULL | FK to Appointments |
| TemplateId | uuid | NULL | FK to NoteTemplates |
| Title | varchar(500) | NOT NULL | Note title |
| Content | text | NOT NULL | Note content |
| NoteType | varchar(100) | NULL | Type of note |
| NoteDate | timestamptz | NOT NULL | Date of note |
| IsSigned | boolean | NOT NULL | Whether signed |
| SignedAt | timestamptz | NULL | When signed |
| SignedBy | text | NULL | Who signed |
| IsLocked | boolean | NOT NULL | Whether locked |
| LockedAt | timestamptz | NULL | When locked |
| CreatedAt | timestamptz | NOT NULL | Record creation time |
| UpdatedAt | timestamptz | NULL | Last update time |
| CreatedBy | text | NULL | Creator user ID |
| UpdatedBy | text | NULL | Last updater user ID |
| IsDeleted | boolean | NOT NULL | Soft delete flag |

**Indexes:**
- `PK_ClinicalNotes` PRIMARY KEY (Id)
- `IX_ClinicalNotes_PatientId` (PatientId)
- `IX_ClinicalNotes_NoteDate` (NoteDate)

**Foreign Keys:**
- `FK_ClinicalNotes_Patients_PatientId` → Patients(Id) ON DELETE CASCADE
- `FK_ClinicalNotes_Practitioners_PractitionerId` → Practitioners(Id) ON DELETE RESTRICT
- `FK_ClinicalNotes_Appointments_AppointmentId` → Appointments(Id)
- `FK_ClinicalNotes_NoteTemplates_TemplateId` → NoteTemplates(Id) ON DELETE SET NULL

---

### Invoices

Financial invoices for patient services.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| Id | uuid | NOT NULL | Primary key |
| PatientId | uuid | NOT NULL | FK to Patients |
| AppointmentId | uuid | NULL | FK to Appointments (unique) |
| OrganizationId | uuid | NOT NULL | FK to Organizations |
| InvoiceNumber | varchar(50) | NOT NULL | Unique invoice number |
| InvoiceDate | timestamptz | NOT NULL | Invoice date |
| DueDate | timestamptz | NOT NULL | Payment due date |
| Status | varchar(50) | NOT NULL | Invoice status |
| SubTotal | decimal(18,2) | NOT NULL | Subtotal amount |
| TaxAmount | decimal(18,2) | NOT NULL | Tax amount |
| TaxRate | numeric | NOT NULL | Tax rate |
| DiscountAmount | decimal(18,2) | NOT NULL | Discount amount |
| TotalAmount | decimal(18,2) | NOT NULL | Total amount |
| AmountPaid | decimal(18,2) | NOT NULL | Amount paid |
| AmountDue | decimal(18,2) | NOT NULL | Amount due |
| LineItems | text | NOT NULL | JSON line items |
| PaidAt | timestamptz | NULL | When paid |
| PaymentMethod | text | NULL | Payment method |
| PaymentReference | text | NULL | Payment reference |
| IsInsuranceClaim | boolean | NOT NULL | Insurance claim flag |
| InsuranceProvider | text | NULL | Insurance provider |
| ClaimNumber | text | NULL | Claim number |
| ClaimSubmittedAt | timestamptz | NULL | Claim submission date |
| ClaimStatus | text | NULL | Claim status |
| ReminderSent | boolean | NOT NULL | Reminder sent flag |
| ReminderSentAt | timestamptz | NULL | When reminder sent |
| LastReminderSentAt | timestamptz | NULL | Last reminder date |
| ReminderCount | integer | NOT NULL | Number of reminders |
| Notes | text | NULL | Invoice notes |
| Terms | text | NULL | Payment terms |
| CreatedAt | timestamptz | NOT NULL | Record creation time |
| UpdatedAt | timestamptz | NULL | Last update time |
| CreatedBy | text | NULL | Creator user ID |
| UpdatedBy | text | NULL | Last updater user ID |
| IsDeleted | boolean | NOT NULL | Soft delete flag |

**Indexes:**
- `PK_Invoices` PRIMARY KEY (Id)
- `IX_Invoices_InvoiceNumber` UNIQUE (InvoiceNumber)
- `IX_Invoices_AppointmentId` UNIQUE (AppointmentId)
- `IX_Invoices_PatientId` (PatientId)
- `IX_Invoices_OrganizationId` (OrganizationId)
- `IX_Invoices_InvoiceDate` (InvoiceDate)

**Foreign Keys:**
- `FK_Invoices_Patients_PatientId` → Patients(Id) ON DELETE CASCADE
- `FK_Invoices_Organizations_OrganizationId` → Organizations(Id) ON DELETE RESTRICT
- `FK_Invoices_Appointments_AppointmentId` → Appointments(Id)

---

### Payments

Payments received against invoices.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| Id | uuid | NOT NULL | Primary key |
| InvoiceId | uuid | NOT NULL | FK to Invoices |
| PatientId | uuid | NOT NULL | FK to Patients |
| OrganizationId | uuid | NOT NULL | FK to Organizations |
| PaymentNumber | varchar(50) | NOT NULL | Unique payment number |
| Amount | decimal(18,2) | NOT NULL | Payment amount |
| PaymentDate | timestamptz | NOT NULL | Payment date |
| PaymentMethod | varchar(50) | NOT NULL | Payment method |
| Status | varchar(50) | NOT NULL | Payment status |
| Reference | text | NULL | Payment reference |
| Notes | text | NULL | Payment notes |
| ProcessedBy | text | NULL | Processor user ID |
| ProcessedAt | timestamptz | NULL | Processing date |
| CreatedAt | timestamptz | NOT NULL | Record creation time |
| UpdatedAt | timestamptz | NULL | Last update time |
| CreatedBy | text | NULL | Creator user ID |
| UpdatedBy | text | NULL | Last updater user ID |
| IsDeleted | boolean | NOT NULL | Soft delete flag |

**Indexes:**
- `PK_Payments` PRIMARY KEY (Id)
- `IX_Payments_PaymentNumber` UNIQUE (PaymentNumber)
- `IX_Payments_InvoiceId` (InvoiceId)
- `IX_Payments_PaymentDate` (PaymentDate)

**Foreign Keys:**
- `FK_Payments_Invoices_InvoiceId` → Invoices(Id) ON DELETE CASCADE
- `FK_Payments_Patients_PatientId` → Patients(Id) ON DELETE CASCADE
- `FK_Payments_Organizations_OrganizationId` → Organizations(Id) ON DELETE CASCADE

---

### Documents

File attachments for patients.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| Id | uuid | NOT NULL | Primary key |
| PatientId | uuid | NULL | FK to Patients |
| AppointmentId | uuid | NULL | FK to Appointments |
| OrganizationId | uuid | NULL | FK to Organizations |
| FileName | varchar(500) | NOT NULL | Original filename |
| FileType | varchar(50) | NOT NULL | File extension |
| MimeType | varchar(100) | NOT NULL | MIME type |
| FileSize | bigint | NOT NULL | File size in bytes |
| StoragePath | text | NOT NULL | Storage location path |
| DocumentType | varchar(100) | NULL | Type of document |
| Description | text | NULL | Description |
| Tags | text | NULL | JSON tags |
| IsConfidential | boolean | NOT NULL | Confidential flag |
| CreatedAt | timestamptz | NOT NULL | Record creation time |
| UpdatedAt | timestamptz | NULL | Last update time |
| CreatedBy | text | NULL | Creator user ID |
| UpdatedBy | text | NULL | Last updater user ID |
| IsDeleted | boolean | NOT NULL | Soft delete flag |

**Indexes:**
- `PK_Documents` PRIMARY KEY (Id)
- `IX_Documents_PatientId` (PatientId)
- `IX_Documents_CreatedAt` (CreatedAt)

**Foreign Keys:**
- `FK_Documents_Patients_PatientId` → Patients(Id)
- `FK_Documents_Appointments_AppointmentId` → Appointments(Id)
- `FK_Documents_Organizations_OrganizationId` → Organizations(Id)

---

### NoteTemplates

Reusable templates for clinical notes.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| Id | uuid | NOT NULL | Primary key |
| OrganizationId | uuid | NULL | FK to Organizations |
| PractitionerId | uuid | NULL | FK to Practitioners |
| Name | varchar(200) | NOT NULL | Template name |
| Content | text | NOT NULL | Template content |
| NoteType | varchar(100) | NULL | Type of note |
| Description | text | NULL | Description |
| IsGlobal | boolean | NOT NULL | Global template flag |
| IsActive | boolean | NOT NULL | Active status |
| CreatedAt | timestamptz | NOT NULL | Record creation time |
| UpdatedAt | timestamptz | NULL | Last update time |
| CreatedBy | text | NULL | Creator user ID |
| UpdatedBy | text | NULL | Last updater user ID |
| IsDeleted | boolean | NOT NULL | Soft delete flag |

**Indexes:**
- `PK_NoteTemplates` PRIMARY KEY (Id)
- `IX_NoteTemplates_OrganizationId` (OrganizationId)
- `IX_NoteTemplates_IsGlobal` (IsGlobal)

**Foreign Keys:**
- `FK_NoteTemplates_Organizations_OrganizationId` → Organizations(Id) ON DELETE CASCADE
- `FK_NoteTemplates_Practitioners_PractitionerId` → Practitioners(Id)

---

## Migrations History

| Migration | Date | Description |
|-----------|------|-------------|
| InitialCreate | 2026-01-28 | Initial database schema with all core entities |

---

## Notes

### Soft Delete Pattern
All entities implement soft delete via the `IsDeleted` boolean field. Queries should filter by `IsDeleted = false` unless explicitly including deleted records.

### Audit Fields
All entities include audit fields: `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy` for tracking changes.

### Multi-Tenancy
Multi-tenancy is implemented at the Organization level. Most entities are scoped to an organization through either direct `OrganizationId` foreign key or through parent relationships.

### Time Zones
All timestamps are stored in UTC (timestamptz). Client applications should convert to local time zones for display.
