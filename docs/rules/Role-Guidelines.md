# Application Roles & Permissions Guidelines

## Strict Role Definitions
- **Manager**: Has full read and write (CRUD) access to all business modules and operations.
- **Owner**: Has **read-only** access. They can view KPIs, financial reports, and dashboards, but they CANNOT add, edit, or delete any records.
- **Developer**: A superset of Manager (full CRUD) plus exclusive access to the Developer Tools module.

## UI Implementation Rules
- When writing `IsManager` or similar permission checks for write operations (Add/Edit/Delete buttons, editors), you MUST strictly exclude `UserRole.Owner`. 
- `UserRole.Owner` should NEVER be granted permission to open edit panels or execute mutating commands.
