# Client Laptop Setup Runbook

This runbook outlines the steps to configure a Client Laptop to run the VISTA application and connect to the Host Laptop.

## 1. Prerequisites
- **Hardware:** Standard business laptop (Intel Core i3/i5, 8GB RAM, SSD).
- **OS:** Windows 10 or Windows 11.
- **Network:** Must be connected to the same local network (LAN or Wi-Fi) as the Host Laptop.
- **Information needed:** The static IP address of the Host Laptop (e.g., `192.168.1.100`) and the `merchsys_sync` MariaDB password.

## 2. Install
- Create a directory for the VISTA application (e.g., `C:\VISTA`).
- Copy the latest compiled VISTA application binaries into this directory.
- Create a shortcut to `MerchSys.App.exe` on the desktop.

## 3. appsettings.json Configuration
- Create the `%LOCALAPPDATA%\VISTA\` directory.
- Copy `appsettings.Production.template.json` from the application directory to `%LOCALAPPDATA%\VISTA\appsettings.Production.json`.
- Edit `%LOCALAPPDATA%\VISTA\appsettings.Production.json`:
  ```json
  {
    "Connection": {
      "Host": "192.168.1.100",
      "Database": "merchsys_central",
      "User": "merchsys_sync",
      "Password": "<your_database_password>",
      "SslMode": "Required"
    },
    "Client": {
      "WorkstationName": "POS-01"
    }
  }
  ```
  *(Replace `192.168.1.100` with the actual Host IP, update the password, and set a unique WorkstationName).*

## 4. First Launch
- Double-click the VISTA desktop shortcut.
- The application should start and present the login screen.
- Log in using an existing account (e.g., `manager`).

## 5. Smoke Test
- Verify the login succeeds.
- Check the **Connection Status Indicator** in the bottom left corner. It should display "Online" (green pill badge).
- Navigate to the **Transaction History** or **Stock Dashboard** and ensure data loaded from the host database is visible.
- Ensure buttons (like "Process Return" or "New PO") are enabled, confirming the `DisableOnOfflineBehavior` sees the connection as healthy.

## 6. Troubleshooting
- **Cannot connect to Database / Connection Indicator is Offline:**
  - Verify the Host Laptop is powered on and connected to the network.
  - Ping the Host Laptop IP from the Client Laptop using the command prompt (`ping 192.168.1.100`).
  - Verify the firewall rule on the Host Laptop is active and allowing port 3306.
  - Double-check the IP and password in `%LOCALAPPDATA%\VISTA\appsettings.Production.json`.
- **Invalid Credentials:** Ensure you are using the password updated during the Host Laptop setup, not the default `Vista2026!`.
- **UI is Greyed Out:** This is normal if the database connection drops. Check the network connection and wait for the auto-reconnect probe to restore the session.
