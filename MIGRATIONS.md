# Database Migrations

This document tracks all database migrations for CoreHub2 CRM.

## Migration History

### InitialCreate - 2025-01-29

**Purpose**: Initial database schema creation with all core entities

**Entities Created**:
- **Patient** - Core patient/client records with demographics, medical history, insurance
- **Practitioner** - Healthcare practitioners/staff members
- **Appointment** - Appointment scheduling and management
- **ClinicalNote** - Clinical documentation and notes
- **Invoice** - Billing and invoicing
- **Payment** - Payment processing and tracking
- **Organization** - Multi-tenant organization support
- **Location** - Physical locations/facilities
- **Document** - Document management
- **NoteTemplate** - Reusable note templates

**Key Features**:
- Comprehensive indexing for performance (email, names, dates, foreign keys)
- Proper relationships with cascade/restrict/set null behaviors
- Audit fields (CreatedAt, UpdatedAt) on all entities
- Decimal precision for financial fields (18, 2)
- Unique constraints on critical fields (emails, invoice numbers, payment numbers)

**Relationships**:
- Patient → Practitioner (Primary Practitioner)
- Patient → Appointments, ClinicalNotes, Invoices, Documents
- Practitioner → Organization
- Appointment → Patient, Practitioner, Location
- ClinicalNote → Patient, Practitioner, NoteTemplate
- Invoice → Patient, Organization, Payments
- Payment → Invoice
- Location → Organization

**To Apply Migration**:
```bash
cd src/Presentation/CoreHub.API
dotnet tool run dotnet-ef database update --project ../../Infrastructure/CoreHub.Infrastructure
```

**To Rollback**:
```bash
dotnet tool run dotnet-ef migrations remove --project ../../Infrastructure/CoreHub.Infrastructure
```

## Migration Guidelines

1. **Before Creating Migration**:
   - Ensure all entity changes are complete
   - Review DbContext configurations
   - Test build succeeds

2. **Naming Convention**:
   - Use descriptive names (e.g., AddUserRoles, UpdateInvoiceFields)
   - Use PascalCase

3. **After Creating Migration**:
   - Review generated migration code
   - Test both Up() and Down() methods
   - Document in this file
   - Commit migration files with descriptive message

4. **Production Deployments**:
   - Always backup database before migration
   - Test migration on staging environment first
   - Plan rollback strategy
   - Document any data migration requirements

## Current Schema Version

**Latest Migration**: InitialCreate
**Applied**: Not yet applied (pending PostgreSQL setup)
**Schema Version**: 1.0.0
