# Nightly Backup Automation Runbook

This runbook configures automated nightly backups of the MariaDB central database on the Host Laptop.

## 1. Scope
This automated backup covers the entire `merchsys_central` MariaDB database, including all schema structures and transactional data. It does not back up the application binaries or the Windows OS state.

## 2. Requirements
- Execution on the **Host Laptop**.
- MariaDB installed via XAMPP.
- Windows Task Scheduler enabled.
- PowerShell 5.1+.
- Destination backup directory (e.g., `C:\VISTA_Backups\`) configured.

## 3. Install Script
Create the file `C:\VISTA_Scripts\backup-mysqldump.ps1` with the following content. Update `$DbPassword` as needed.

```powershell
# backup-mysqldump.ps1
$BackupDir = "C:\VISTA_Backups"
$DbUser = "root"
$DbPassword = "your_root_password"
$DbName = "merchsys_central"
$MySqlDumpPath = "C:\xampp\mysql\bin\mysqldump.exe"

$DateStr = Get-Date -Format "yyyyMMdd_HHmmss"
$BackupFile = "$BackupDir\$DbName_$DateStr.sql"
$ZipFile = "$BackupDir\$DbName_$DateStr.zip"

if (!(Test-Path -Path $BackupDir)) {
    New-Item -ItemType Directory -Path $BackupDir | Out-Null
}

# Execute dump
& $MySqlDumpPath --user=$DbUser --password=$DbPassword --single-transaction --routines --triggers $DbName > $BackupFile

# Validate dump size (> 50KB expected for initialized schema)
$FileInfo = Get-Item $BackupFile
if ($FileInfo.Length -lt 50000) {
    Write-Error "Backup failed: File size too small ($($FileInfo.Length) bytes)."
    exit 1
}

# Compress
Compress-Archive -Path $BackupFile -DestinationPath $ZipFile -Force
Remove-Item -Path $BackupFile -Force

# Tiered Retention Cleanup (7 daily, 4 weekly, 6 monthly)
# Note: A full GFS script would be longer; basic 30-day cleanup shown here.
Get-ChildItem -Path $BackupDir -Filter "*.zip" | Where-Object { $_.CreationTime -lt (Get-Date).AddDays(-30) } | Remove-Item -Force

Write-Output "Backup completed successfully: $ZipFile"
```

## 4. Task Scheduler Setup (GUI + CLI)

### GUI Method:
1. Open **Task Scheduler**.
2. Click **Import Task...** in the right pane.
3. Select the `vista-nightly-backup.xml` file (see below).
4. Review the properties, ensure it runs under `SYSTEM` or a high-privilege account, and check "Run whether user is logged on or not".

### CLI Method:
Save the XML below as `vista-nightly-backup.xml` and run this command as Administrator:
`schtasks /create /xml "C:\VISTA_Scripts\vista-nightly-backup.xml" /tn "VISTA\NightlyBackup"`

#### vista-nightly-backup.xml
```xml
<?xml version="1.0" encoding="UTF-16"?>
<Task version="1.2" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
  <RegistrationInfo>
    <Author>VISTA Admin</Author>
    <Description>Runs the VISTA database nightly backup.</Description>
  </RegistrationInfo>
  <Triggers>
    <CalendarTrigger>
      <StartBoundary>2026-01-01T02:00:00</StartBoundary>
      <Enabled>true</Enabled>
      <ScheduleByDay>
        <DaysInterval>1</DaysInterval>
      </ScheduleByDay>
    </CalendarTrigger>
  </Triggers>
  <Principals>
    <Principal id="Author">
      <LogonType>S4U</LogonType>
      <RunLevel>HighestAvailable</RunLevel>
    </Principal>
  </Principals>
  <Settings>
    <ExecutionTimeLimit>PT1H</ExecutionTimeLimit>
    <Hidden>false</Hidden>
    <RunOnlyIfNetworkAvailable>false</RunOnlyIfNetworkAvailable>
    <StartWhenAvailable>true</StartWhenAvailable>
  </Settings>
  <Actions Context="Author">
    <Exec>
      <Command>powershell.exe</Command>
      <Arguments>-NoProfile -ExecutionPolicy Bypass -File "C:\VISTA_Scripts\backup-mysqldump.ps1"</Arguments>
    </Exec>
  </Actions>
</Task>
```

## 5. Test Procedure
1. Open Task Scheduler.
2. Locate `VISTA\NightlyBackup`.
3. Right-click and select **Run**.
4. Check `C:\VISTA_Backups`. A new `.zip` file should appear within a few seconds.
5. Extract the `.zip` and verify the `.sql` file contains table structures and data.

## 6. Retention Policy
The script implements a basic retention policy. It automatically cleans up backups older than 30 days. For production, the script should be expanded to strictly enforce 7-daily, 4-weekly, and 6-monthly archives.

## 7. Quarterly Restore Drill
Every 3 months, you must perform a restore drill:
1. Copy the latest `.zip` backup to a test machine (NOT the production Host Laptop).
2. Extract the `.sql` file.
3. Import the `.sql` file into a fresh MariaDB instance.
4. Launch the VISTA client pointed at this test instance.
5. Verify you can log in and data is intact.

## 8. Backup User Privileges
The backup script currently uses `root`. For enhanced security, create a dedicated `merchsys_backup` user in MariaDB with only `SELECT`, `SHOW VIEW`, `RELOAD`, `REPLICATION CLIENT`, `EVENT`, and `TRIGGER` privileges on `merchsys_central`. Update the `.ps1` script accordingly.
