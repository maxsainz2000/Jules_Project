# Production Deployment Configuration Runbook

This runbook covers the production deployment configuration for MerchSys.

1. **Prerequisites**
   - Ensure you have MariaDB installed and running.
   - The application relies on `appsettings.json` for base settings and `appsettings.Production.json` for production overrides.

2. **Configuration Overlay**
   - Base settings are in `appsettings.json`.
   - Create an `appsettings.Production.json` file in the application directory.
   - The `appsettings.Production.json` file should contain credentials and production-specific values.
   - This file is ignored by Git.

3. **Template**
   - A template is provided in `appsettings.Production.template.json`.
   - Copy this template to `appsettings.Production.json` and fill in the `<Placeholder>` values.

4. **Startup Logic**
   - The application uses `ConnectionStringLoader` to load the production configuration path and layer it onto the configuration builder.
   - If `Sync:MariaDbConnection` is missing, a warning is logged.
