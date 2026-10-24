# Host Laptop Setup Runbook (VISTA Server)

This runbook details the setup process for the Host Laptop, which acts as the central server for the VISTA platform.

## 1. Hardware Specification
- **CPU:** Intel Core i5 (8th Gen or newer) or AMD equivalent.
- **RAM:** Minimum 8GB (16GB recommended for database performance).
- **Storage:** 256GB SSD or larger (NVMe preferred).
- **Network:** Gigabit Ethernet adapter (preferred) or stable Wi-Fi 5/6.
- **Power:** Must be connected to a UPS (Uninterruptible Power Supply).

## 2. OS and Network Preparation
- **OS:** Windows 10 Pro or Windows 11 Pro.
- **Updates:** Ensure all Windows updates are applied.
- **Power Plan:** Set Windows Power Plan to "High Performance" to prevent sleeping.
- **Static IP:** Configure a static IP address on the local network (e.g., `192.168.1.100`).
- **Computer Name:** Set the computer name to a recognizable identifier (e.g., `VISTA-HOST`).

## 3. XAMPP Installation
- Download the latest XAMPP installer from the official Apache Friends website.
- Run the installer. You only need to select **MySQL** (MariaDB) and **Apache** (optional, for web tools).
- Install to the default directory (usually `C:\xampp`).
- Start the XAMPP Control Panel as Administrator.
- Install the MySQL/MariaDB service by clicking the red 'X' next to MySQL in the control panel.

## 4. MariaDB Configuration
- Open the MariaDB configuration file (usually `C:\xampp\mysql\bin\my.ini`).
- Under `[mysqld]`, add/verify the following settings for optimal performance and VISTA compatibility:
  ```ini
  max_allowed_packet = 64M
  innodb_buffer_pool_size = 1G
  character-set-server = utf8mb4
  collation-server = utf8mb4_unicode_ci
  default-time-zone = '+00:00'
  ```
- Restart the MariaDB service via the XAMPP Control Panel.
- Create the database and the application account before the first launch. Connect to the local
  MariaDB instance as `root` (for example with HeidiSQL or `mysql.exe`) and run:
  ```sql
  CREATE DATABASE IF NOT EXISTS vista_central CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
  CREATE USER IF NOT EXISTS 'merchsys_sync'@'%' IDENTIFIED BY '<your_database_password>';
  GRANT ALL PRIVILEGES ON vista_central.* TO 'merchsys_sync'@'%';
  FLUSH PRIVILEGES;
  ```
  The application cannot create the database itself: it opens a connection to `vista_central`
  during startup and fails with "Unknown database" if the schema does not already exist. Once
  the database exists, the first launch creates the central tables and every module table.
- **Do not create any application table by hand before the first launch.** The startup schema
  step treats a module as "already present" if *any* of its tables exist, and skips that module
  entirely. Creating even one POS table manually therefore leaves the other ten POS tables
  unbuilt, and the failure only appears later as "table doesn't exist" on the first POS query.
  The EF model is the single source of truth; `DatabaseBootstrapper` logs an error naming any
  tables it finds missing.
- **The receipt-archive immutability triggers are applied automatically.** EF Core cannot create
  triggers, and the tables they guard are created by the application, so the application applies them
  itself as the *last* startup schema step (after the EF model has created the tables) and then
  verifies they exist. No manual step is required, and a database cannot run with immutability
  silently unenforced — the failure mode that let NIRC sec. 235 go unenforced before.
  - Two BIR retention tables are covered, each with an UPDATE and a DELETE trigger:
    `Pos_OfficialReceiptArchive` and `Pos_ReceiptIntegrityArchive`. **INSERT stays allowed** — the
    archival service inserts rows into both.
  - The database account therefore needs the `TRIGGER` privilege; the `GRANT ALL` above covers it.
    If it is missing, startup fails with a clear message rather than continuing unprotected.
  - The triggers are the **only** enforcement that survives a direct-SQL client:
    `ImmutableReceiptInterceptor` is application code, bypassed by raw SQL, by another client, and by
    EF bulk operations (`ExecuteDelete`/`ExecuteUpdate` raise no SaveChanges interceptor).
  - To apply them by hand instead (e.g. to pre-seed before the first launch, or to inspect the
    DDL), the canonical script is
    `Plans/VISTA_Modules/Infrastructure/sql/02-pos-receipt-integrity-triggers.sql`:
    ```
    mysql -u root -p vista_central < Plans/VISTA_Modules/Infrastructure/sql/02-pos-receipt-integrity-triggers.sql
    ```
    `Resources/Sql/ReceiptIntegrityTriggerVerification.sql` confirms the result.

## 4b. Existing databases: schema changes the application cannot apply itself
The application derives its schema from the EF model, but **only when it creates the tables** — there
are no migrations, so a model change never reaches a database that already exists. A database created
before one of the changes below needs the column added by hand, or the affected screen fails.

- **`Pos_SalesReturns.SerializedLines`** (LONGTEXT NULL) — records which products a return covered.
  Without it, processing a return fails and the transaction history screen cannot load returns.
  ```sql
  ALTER TABLE vista_central.Pos_SalesReturns ADD COLUMN SerializedLines LONGTEXT NULL;
  ```
- **`Pur_PurchaseOrders.RowVersion`** — the optimistic concurrency token on the order's status. A
  database created before it exists will fail every purchase-order write with "Unknown column
  'RowVersion'". The `ON UPDATE` clause must be written **explicitly** here: MariaDB applies it
  implicitly only to the first `TIMESTAMP` column of a table it is creating, not to an added column,
  and without it the token never changes and two clients can close the same order and book the
  payable twice.
  ```sql
  ALTER TABLE vista_central.Pur_PurchaseOrders
      ADD COLUMN RowVersion TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);
  ```

Check for anything else the model expects but the database lacks by comparing the two — a table that
is missing a column surfaces as "Unknown column" at runtime, not at startup.

## 5. Firewall Rule Configuration
- Open **Windows Defender Firewall with Advanced Security**.
- Create a new **Inbound Rule**.
- Select **Port** -> **TCP** -> Specific local ports: **3306** (default MariaDB port).
- Action: **Allow the connection**.
- Profile: Domain, Private, Public (select appropriate based on network).
- Name: **MariaDB - VISTA Host**.

## 6. UPS Wiring
- Ensure the Host Laptop power adapter is plugged directly into a battery-backed outlet on the UPS.
- Connect the UPS USB data cable to the Host Laptop.
- Install the UPS management software (e.g., PowerChute) and configure it to perform a graceful shutdown of the host laptop when battery capacity reaches 15%.

## 7. First VISTA Launch
- Copy the compiled VISTA application folder to the Host Laptop (e.g., `C:\VISTA`).
- Configure the `%LOCALAPPDATA%\VISTA\appsettings.Production.json` file with the localhost database credentials.
- Launch `MerchSys.App.exe`.
- The application will automatically perform schema initializations and seeding.
- Log in using the default credentials (`manager` / `Vista2026!`).
- Complete the mandatory first-login password change.
- **Delete the seeded `developer` account from any database that is not for development.** It is
  seeded **only by Debug builds** — Release compiles the seeding out — but a database that has ever
  been opened by a Debug build still carries it, with a role-3 (Manager-superset) account whose
  password is committed to the repository. Prune it once the database is no longer used for
  development:
  ```sql
  DELETE FROM `UserAccounts` WHERE `Username` = 'developer';
  ```
  The deletion is not self-healing in the other direction — nothing re-creates the account in Release
  — but a single Debug launch pointed at that database re-inserts it (`INSERT IGNORE`), so do this
  last, and never point a Debug build at production.

## 8. Smoke Test
- Verify you can log in with the newly set password.
- Check the **Connection Status Indicator** in the bottom left corner to ensure it shows "Online".
- Navigate to the **Stock Dashboard** and ensure reference data (products/categories) are visible.
- Create a test Product and save it. Verify no errors occur.
