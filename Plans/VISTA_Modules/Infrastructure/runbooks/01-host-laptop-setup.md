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
- Use a database tool (like HeidiSQL) to connect to the local MariaDB instance as `root` and execute the `mariadb-init.sql` script to set up the `merchsys_central` database and required user (`merchsys_sync`).

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

## 8. Smoke Test
- Verify you can log in with the newly set password.
- Check the **Connection Status Indicator** in the bottom left corner to ensure it shows "Online".
- Navigate to the **Stock Dashboard** and ensure reference data (products/categories) are visible.
- Create a test Product and save it. Verify no errors occur.
