
---
applyTo: "services/backend/**"
---

# Backend Service Review Rules
- Validate all inputs in public handlers.
- Ensure database calls use `DbClient` wrapper.
- Suggest unit tests for new handlers.
