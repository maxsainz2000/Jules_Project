# Central Schema Migrations

This directory contains the central MariaDB schema definitions and seed data.

## Rules

- **Execution Order**: Scripts are executed in lexicographical order. E.g., `0001_` before `0002_`.
- **Idempotency**: All scripts must be idempotent (`CREATE TABLE IF NOT EXISTS`, `INSERT IGNORE`, etc.).
- **Drift Prevention**: Each applied script is hashed (SHA-256) upon execution. Any changes to applied scripts will cause application startup to cleanly abort to prevent schema drift and tampering.
