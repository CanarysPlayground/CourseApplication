---
applyTo: '**'
---
# Certificate Creation Rules

## Required Data Fields
- CertificateId: GUID (immutable primary identifier)
- SerialNumber: Unique, sequential, human-readable (no PII)
- HolderName: Full official name (validated, sanitized)
- CourseTitle: Exact course name from catalog
- IssueDateUtc: UTC timestamp
- ExpiryDateUtc: Nullable UTC timestamp (omit if non-expiring)
- IssuedBy: Authority identifier (organization unit code)
- VerificationUrl: HTTPS endpoint to validate authenticity
- SignatureHash: SHA-256 hash of canonical certificate payload
- Version: Semantic version of certificate schema
- Status: Active | Revoked | Expired

## Validation Rules
- All timestamps stored and displayed in UTC
- SerialNumber uniqueness enforced at persistence layer
- HolderName must pass Unicode normalization + length (1–120 chars)
- No uncontrolled HTML; plain text only
- ExpiryDateUtc > IssueDateUtc when present
- SignatureHash recomputed on any mutable field change and invalidated on mismatch

## Generation Workflow
1. Load course completion record (must be finalized and verified).
2. Construct canonical payload (stable ordered JSON).
3. Generate GUID CertificateId.
4. Allocate next SerialNumber in transactional scope.
5. Compute SignatureHash over canonical payload with organization private key or HMAC.
6. Persist certificate atomically.
7. Emit event CertificateIssued for downstream systems.
8. Provide VerificationUrl embedding CertificateId or SerialNumber.

## Layout / Rendering
- Use server-side template; no client-authority logic.
- Embed QR code pointing to VerificationUrl.
- Include minimal branding assets (cache-busted).
- Do not render internal identifiers (GUID, hashes) except via QR/URL.

## Security
- Private signing material stored in secure vault (not in source control).
- All verification endpoints require rate limiting + abuse detection.
- Prevent enumeration: require full SerialNumber or GUID for lookup.
- Use HTTPS only; HSTS enabled.

## Revocation
- Store RevocationReason + RevokedDateUtc
- Maintain audit trail (who revoked, justification)
- Do not delete revoked certificates; mark Status=Revoked
- Verification endpoint returns explicit revoked state

## Versioning
- Increment Version on schema changes
- Backward compatibility: verification must handle all prior versions
- Include Version in signature scope

## Auditing
- Log issuance, access (verification lookups), and revocations with correlation IDs
- Retain logs per compliance policy

## Performance
- Index by CertificateId, SerialNumber, Status
- Cache positive verification responses briefly (e.g., 60s) excluding revoked status

## Privacy
- Expose only HolderName, CourseTitle, IssueDateUtc, Status, VerificationUrl
- No grades or internal performance metrics

## Testing
- Unit tests for payload canonicalization and signature generation
- Integration tests for issuance + verification round trip
- Negative tests: tampered payload, revoked certificate, expired certificate

## Error Handling
- On generation failure: rollback and no partial persistence
- On verification: return 404 for unknown, explicit state for revoked/expired

## Accessibility
- PDF/HTML output must meet WCAG AA
- Provide alt text for QR code describing purpose

## Internationalization
- Dates displayed in ISO 8601
- Localized course titles supported; store invariant key + localized display string

## Deployment
- Never regenerate historical certificates silently
- Migrations preserve existing SignatureHash values