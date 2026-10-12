# Production Deployment Configuration Runbook

This runbook covers the production deployment configuration for MerchSys.

1. **Prerequisites**
   - Ensure you have MariaDB installed and running.
   - The application relies on `appsettings.json` for base settings and `appsettings.Production.json` for production overrides.

2. **Configuration Overlay**
   - Base settings are in `appsettings.json`.
   - Create `%LOCALAPPDATA%\VISTA\appsettings.Production.json` — the canonical operator
     location that the application reads. A file placed next to the executable in the
     application directory is read only as a developer fallback, so following the canonical
     path is what makes the file effective.
   - The `appsettings.Production.json` file should contain credentials and production-specific values.
   - This file is ignored by Git.

3. **Template**
   - A template is provided in `appsettings.Production.template.json`.
   - Copy this template to `appsettings.Production.json` and fill in the `<Placeholder>` values.

4. **Startup Logic**
   - The application uses `ConnectionStringLoader` to load the production configuration path and layer it onto the configuration builder.
   - If `Sync:MariaDbConnection` is missing, the application fails loudly: it shows a
     "Production Configuration Not Found" dialog naming the expected path, and exits with
     code 1. It does not continue to a login that cannot work.
