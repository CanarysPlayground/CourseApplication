# Certificate Validation Rules

This document formalizes all certificate validation requirements based on the official instructions. Each rule includes its purpose, enforcement logic, and examples.

## Table of Contents
1. [Required Data Fields](#required-data-fields)
2. [Validation Rules](#validation-rules)
3. [Generation Workflow](#generation-workflow)
4. [Security Rules](#security-rules)
5. [Revocation Rules](#revocation-rules)
6. [Versioning Rules](#versioning-rules)
7. [Error Handling Rules](#error-handling-rules)
8. [Testing Requirements](#testing-requirements)

---

## Required Data Fields

### Rule 1.1: CertificateId (GUID)
**Purpose**: Immutable primary identifier for each certificate.

**Enforcement Logic**:
- Must be a valid GUID
- Generated once during creation
- Never modified after creation

**Valid Examples**:
```
550e8400-e29b-41d4-a716-446655440000
123e4567-e89b-12d3-a456-426614174000
```

**Invalid Examples**:
```
null
empty string
"not-a-guid"
```

### Rule 1.2: SerialNumber
**Purpose**: Unique, sequential, human-readable identifier with no PII.

**Enforcement Logic**:
- Must be unique across all certificates
- Enforced at persistence layer (database constraint)
- Sequential allocation within transactional scope
- No personally identifiable information
- Format: String, max length 50 characters

**Valid Examples**:
```
CERT-2024-001
CERT-2024-002
```

**Invalid Examples**:
```
null
duplicate serial number
"JohnDoe-CERT-2024" (contains PII)
```

### Rule 1.3: HolderName
**Purpose**: Full official name of the certificate holder.

**Enforcement Logic**:
- Must pass Unicode normalization (NFC - Canonical Decomposition, followed by Canonical Composition)
- Length: 1-120 characters
- Plain text only (no HTML)
- Must be sanitized before storage and display

**Valid Examples**:
```
"John Doe"
"María García"
"李明" (Chinese characters)
"A" (single character)
```

**Invalid Examples**:
```
"" (empty string)
null
"<script>alert('xss')</script>" (HTML/script)
"This is a very long name that exceeds the maximum allowed length of 120 characters and should be rejected by the validation logic" (>120 chars)
```

### Rule 1.4: CourseTitle
**Purpose**: Exact course name from catalog.

**Enforcement Logic**:
- Must match an existing course in the catalog
- Plain text only
- No modification allowed after certificate creation

**Valid Examples**:
```
"Introduction to Programming"
"Web Development Advanced"
```

**Invalid Examples**:
```
null
"" (empty)
"Non-existent Course"
```

### Rule 1.5: IssueDateUtc
**Purpose**: UTC timestamp when certificate was issued.

**Enforcement Logic**:
- Must be in UTC timezone
- Must be a valid DateTime
- Should not be in the future
- ISO 8601 format for display

**Valid Examples**:
```
2024-01-15T10:30:00Z
2024-12-31T23:59:59Z
```

**Invalid Examples**:
```
null
future date (when validating)
non-UTC timestamp
```

### Rule 1.6: ExpiryDateUtc
**Purpose**: Optional UTC timestamp for certificate expiration.

**Enforcement Logic**:
- Nullable (omit if certificate is non-expiring)
- When present, must be in UTC
- Must be greater than IssueDateUtc
- ISO 8601 format for display

**Valid Examples**:
```
null (non-expiring certificate)
2025-01-15T10:30:00Z (after issue date)
```

**Invalid Examples**:
```
2024-01-14T10:30:00Z (before issue date of 2024-01-15)
non-UTC timestamp
```

### Rule 1.7: IssuedBy
**Purpose**: Authority identifier (organization unit code).

**Enforcement Logic**:
- Must be a valid organization unit code
- Required field
- Max length: 100 characters

**Valid Examples**:
```
"ORG-EDU-001"
"DEPT-CS"
```

**Invalid Examples**:
```
null
"" (empty)
```

### Rule 1.8: VerificationUrl
**Purpose**: HTTPS endpoint to validate certificate authenticity.

**Enforcement Logic**:
- Must be a valid HTTPS URL
- Must embed CertificateId or SerialNumber
- Required field

**Valid Examples**:
```
"https://example.com/verify/550e8400-e29b-41d4-a716-446655440000"
"https://example.com/verify?serial=CERT-2024-001"
```

**Invalid Examples**:
```
null
"http://example.com/verify" (HTTP, not HTTPS)
"not-a-url"
```

### Rule 1.9: SignatureHash
**Purpose**: SHA-256 hash of canonical certificate payload.

**Enforcement Logic**:
- Must be SHA-256 hash (64 hexadecimal characters)
- Computed over canonical payload
- Recomputed on any mutable field change
- Validation fails if hash doesn't match

**Valid Examples**:
```
"e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"
```

**Invalid Examples**:
```
null
"invalid-hash"
"abc123" (too short)
```

### Rule 1.10: Version
**Purpose**: Semantic version of certificate schema.

**Enforcement Logic**:
- Must follow semantic versioning (MAJOR.MINOR.PATCH)
- Incremented on schema changes
- Included in signature scope
- Backward compatibility required for all versions

**Valid Examples**:
```
"1.0.0"
"2.1.0"
"1.0.1"
```

**Invalid Examples**:
```
null
"v1"
"1.0"
```

### Rule 1.11: Status
**Purpose**: Current status of the certificate.

**Enforcement Logic**:
- Must be one of: Active, Revoked, Expired
- Default: Active
- Transitions:
  - Active → Revoked (manual revocation)
  - Active → Expired (automatic on expiry)
  - Revoked/Expired → No further transitions

**Valid Examples**:
```
Active
Revoked
Expired
```

**Invalid Examples**:
```
null
"Pending"
"Invalid"
```

---

## Validation Rules

### Rule 2.1: UTC Timestamp Storage
**Purpose**: Ensure consistent timezone handling.

**Enforcement Logic**:
- All timestamps stored in UTC
- All timestamps displayed in UTC (ISO 8601)
- No local timezone conversions in storage

**Test Cases**:
- Store timestamp in UTC
- Retrieve and verify it's still UTC
- Display in ISO 8601 format

### Rule 2.2: SerialNumber Uniqueness
**Purpose**: Prevent duplicate certificates.

**Enforcement Logic**:
- Database unique constraint on SerialNumber
- Constraint violation should raise exception
- Transaction rollback on duplicate

**Test Cases**:
- Create certificate with unique SerialNumber - Success
- Attempt to create certificate with duplicate SerialNumber - Failure
- Verify database constraint prevents duplicates

### Rule 2.3: HolderName Unicode Normalization
**Purpose**: Ensure consistent name representation.

**Enforcement Logic**:
- Apply Unicode Normalization Form C (NFC)
- Validate length after normalization
- Length: 1-120 characters

**Test Cases**:
- Names with accented characters (e.g., "José")
- Names with Unicode combining characters
- Names with emoji or special symbols
- Empty string - Failure
- String > 120 chars - Failure

### Rule 2.4: HTML Sanitization
**Purpose**: Prevent XSS and injection attacks.

**Enforcement Logic**:
- No HTML tags allowed in any text field
- Strip or encode HTML entities
- Plain text only

**Test Cases**:
- Input with `<script>` tags - Rejected/Sanitized
- Input with HTML entities - Sanitized
- Plain text - Accepted

### Rule 2.5: ExpiryDate > IssueDate
**Purpose**: Ensure logical date ordering.

**Enforcement Logic**:
- When ExpiryDateUtc is present, it must be > IssueDateUtc
- Validation performed before persistence

**Test Cases**:
- ExpiryDateUtc > IssueDateUtc - Success
- ExpiryDateUtc = IssueDateUtc - Failure
- ExpiryDateUtc < IssueDateUtc - Failure
- ExpiryDateUtc = null - Success

### Rule 2.6: SignatureHash Validation
**Purpose**: Detect tampering or corruption.

**Enforcement Logic**:
- Recompute hash from canonical payload
- Compare with stored SignatureHash
- Any mismatch invalidates certificate

**Test Cases**:
- Verify newly created certificate hash - Valid
- Modify certificate data, verify hash - Invalid
- Recompute hash after valid change - Valid again

---

## Generation Workflow

### Rule 3.1: Course Completion Verification
**Purpose**: Only issue certificates for completed courses.

**Enforcement Logic**:
1. Load course completion record
2. Verify record is finalized and verified
3. Reject if course not completed

**Test Cases**:
- Completed course - Certificate issued
- Incomplete course - Certificate rejected
- Unverified completion - Certificate rejected

### Rule 3.2: Canonical Payload Construction
**Purpose**: Ensure consistent signature computation.

**Enforcement Logic**:
1. Create stable, ordered JSON representation
2. Include all immutable fields
3. Exclude mutable fields (e.g., Status)
4. Use consistent key ordering

**Test Cases**:
- Same data produces same canonical payload
- Different field order produces same canonical payload
- Verify JSON is valid and parseable

### Rule 3.3: GUID Generation
**Purpose**: Create unique identifier.

**Enforcement Logic**:
- Generate new GUID for CertificateId
- Ensure GUID is non-empty and valid

**Test Cases**:
- Generated GUID is valid
- Generated GUID is unique

### Rule 3.4: SerialNumber Allocation
**Purpose**: Ensure sequential, unique serial numbers.

**Enforcement Logic**:
- Allocate within transactional scope
- Increment counter atomically
- Format: CERT-{year}-{sequence:D3}

**Test Cases**:
- First certificate gets CERT-2024-001
- Second certificate gets CERT-2024-002
- Concurrent requests don't get duplicates

### Rule 3.5: SignatureHash Computation
**Purpose**: Create tamper-proof signature.

**Enforcement Logic**:
- Compute SHA-256 hash over canonical payload
- Use organization private key or HMAC
- Store hash with certificate

**Test Cases**:
- Hash is 64 hex characters
- Same payload produces same hash
- Different payload produces different hash

### Rule 3.6: Atomic Persistence
**Purpose**: Prevent partial certificate creation.

**Enforcement Logic**:
- All data persisted in single transaction
- Rollback on any failure
- No partial certificates in database

**Test Cases**:
- Success: All data persisted
- Failure: Nothing persisted (rollback)

### Rule 3.7: Event Emission
**Purpose**: Notify downstream systems.

**Enforcement Logic**:
- Emit CertificateIssued event after persistence
- Include CertificateId and SerialNumber
- Event emission should not fail certificate creation

**Test Cases**:
- Event emitted on success
- Event contains correct data
- Event failure doesn't rollback certificate

### Rule 3.8: VerificationUrl Generation
**Purpose**: Enable certificate verification.

**Enforcement Logic**:
- Embed CertificateId or SerialNumber
- Use HTTPS protocol
- Return URL with certificate

**Test Cases**:
- URL is valid HTTPS
- URL contains CertificateId or SerialNumber
- URL is accessible

---

## Security Rules

### Rule 4.1: Private Key Storage
**Purpose**: Protect signing material.

**Enforcement Logic**:
- Private keys stored in secure vault (e.g., Azure Key Vault, AWS KMS)
- Never in source control
- Never in configuration files
- Access restricted by role

**Test Cases**:
- Verify no keys in source code
- Verify keys not in config files
- Verify secure vault usage

### Rule 4.2: Verification Endpoint Rate Limiting
**Purpose**: Prevent abuse and enumeration attacks.

**Enforcement Logic**:
- Rate limit verification requests
- Track by IP address and/or user
- Return 429 Too Many Requests when exceeded

**Test Cases**:
- Normal request rate - Success
- Excessive requests - Rate limited
- Different IPs - Independent limits

### Rule 4.3: Anti-Enumeration
**Purpose**: Prevent certificate enumeration.

**Enforcement Logic**:
- Require full SerialNumber or GUID for lookup
- Don't provide sequential hints
- Return 404 for invalid identifiers (not differentiate between non-existent and invalid)

**Test Cases**:
- Valid SerialNumber - Found
- Invalid SerialNumber - 404
- Partial SerialNumber - 404
- No hints about valid ranges

### Rule 4.4: HTTPS Only
**Purpose**: Protect data in transit.

**Enforcement Logic**:
- All verification endpoints use HTTPS
- HTTP Strict Transport Security (HSTS) enabled
- Redirect HTTP to HTTPS

**Test Cases**:
- HTTPS request - Success
- HTTP request - Redirect to HTTPS
- HSTS header present

---

## Revocation Rules

### Rule 5.1: Revocation Data Storage
**Purpose**: Track revoked certificates and reasons.

**Enforcement Logic**:
- Store RevocationReason (string, required on revocation)
- Store RevokedDateUtc (UTC timestamp, required on revocation)
- Store RevokedBy (user identifier, required on revocation)

**Test Cases**:
- Revoke with reason - All fields populated
- Revoke without reason - Rejected

### Rule 5.2: Audit Trail
**Purpose**: Maintain accountability.

**Enforcement Logic**:
- Log who revoked the certificate
- Log when it was revoked
- Log justification/reason
- Immutable audit log

**Test Cases**:
- Revocation creates audit entry
- Audit entry contains all required fields
- Audit entry cannot be modified

### Rule 5.3: No Deletion
**Purpose**: Maintain historical records.

**Enforcement Logic**:
- Never delete revoked certificates
- Mark Status = Revoked
- Keep in database with revocation metadata

**Test Cases**:
- Revoked certificate still in database
- Revoked certificate has Status = Revoked
- Revoked certificate includes revocation metadata

### Rule 5.4: Verification Response
**Purpose**: Inform verification requests of revocation.

**Enforcement Logic**:
- Return explicit "revoked" state
- Include revocation date (not reason for privacy)
- HTTP 200 with revoked status (not 404)

**Test Cases**:
- Verify revoked certificate - Returns "revoked" status
- Verify active certificate - Returns "active" status
- Response includes appropriate metadata

---

## Versioning Rules

### Rule 6.1: Version Increment
**Purpose**: Track schema evolution.

**Enforcement Logic**:
- Increment version on schema changes
- MAJOR: Breaking changes
- MINOR: Backward-compatible additions
- PATCH: Backward-compatible fixes

**Test Cases**:
- New certificate created with current version
- Version follows semantic versioning format

### Rule 6.2: Backward Compatibility
**Purpose**: Support old certificates.

**Enforcement Logic**:
- Verification must handle all prior versions
- Version-specific parsing logic
- Graceful degradation for missing fields

**Test Cases**:
- Verify v1.0.0 certificate - Success
- Verify v2.0.0 certificate - Success
- Unsupported version - Explicit error

### Rule 6.3: Signature Scope
**Purpose**: Include version in tamper protection.

**Enforcement Logic**:
- Version included in canonical payload
- Version included in SignatureHash computation
- Version change invalidates signature

**Test Cases**:
- Hash includes version
- Changing version invalidates hash
- Same data with different version = different hash

---

## Error Handling Rules

### Rule 7.1: Generation Failure Rollback
**Purpose**: Prevent partial state.

**Enforcement Logic**:
- On any failure during generation, rollback transaction
- No partial persistence
- Return error to caller

**Test Cases**:
- Database failure - No certificate created
- Validation failure - No certificate created
- Hash computation failure - No certificate created

### Rule 7.2: Verification Error Responses
**Purpose**: Provide clear error messages.

**Enforcement Logic**:
- Unknown certificate - 404 Not Found
- Revoked certificate - 200 OK with "revoked" status
- Expired certificate - 200 OK with "expired" status
- Tampered certificate - 400 Bad Request or specific "invalid" status

**Test Cases**:
- Unknown ID - 404
- Revoked - 200 with revoked status
- Expired - 200 with expired status
- Invalid hash - 400 or invalid status

---

## Testing Requirements

### Rule 8.1: Unit Tests for Payload Canonicalization
**Purpose**: Ensure consistent canonical form.

**Test Cases**:
- Same data, different order - Same canonical payload
- Unicode normalization in canonical form
- Null fields excluded from canonical form

### Rule 8.2: Unit Tests for Signature Generation
**Purpose**: Verify hash computation.

**Test Cases**:
- Same payload - Same hash
- Different payload - Different hash
- Hash is valid SHA-256

### Rule 8.3: Integration Tests for Issuance
**Purpose**: Verify end-to-end workflow.

**Test Cases**:
- Complete course → Issue certificate → Verify success
- Incomplete course → Reject issuance
- Duplicate serial number → Reject issuance

### Rule 8.4: Integration Tests for Verification
**Purpose**: Verify verification workflow.

**Test Cases**:
- Valid certificate → Verification succeeds
- Tampered certificate → Verification fails
- Revoked certificate → Verification returns "revoked"
- Expired certificate → Verification returns "expired"

### Rule 8.5: Negative Tests
**Purpose**: Verify error handling.

**Test Cases**:
- Tampered payload - Detected
- Invalid date range - Rejected
- Missing required fields - Rejected
- SQL injection attempts - Sanitized/Rejected
- XSS attempts - Sanitized/Rejected

---

## Summary

This document defines all validation rules for certificate generation and verification. Each rule includes:
- Clear purpose and intent
- Specific enforcement logic
- Valid and invalid examples
- Test cases to verify compliance

All code implementations must adhere to these rules, and all test suites must verify compliance with these rules.
