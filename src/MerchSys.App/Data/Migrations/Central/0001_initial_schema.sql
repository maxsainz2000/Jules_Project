-- 0001_initial_schema.sql
-- Initial Schema for MerchSys
-- Migrated from SQLite to modern MariaDB syntax with microsecond timestamps and concurrency row versions.

CREATE TABLE IF NOT EXISTS `Sys_UserAccounts` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Username` VARCHAR(255) NOT NULL,
    `PasswordHash` VARCHAR(255) NOT NULL,
    `Role` INT NOT NULL,
    `IsActive` BOOLEAN NOT NULL DEFAULT 1,
    `FailedLoginAttempts` INT NOT NULL DEFAULT 0,
    `LockedUntil` DATETIME(6) NULL,
    `LastPasswordChangeAt` DATETIME(6) NULL,
    `CreatedAt` DATETIME(6) NOT NULL,
    `ModifiedAt` DATETIME(6) NULL,
    `CreatedBy` VARCHAR(255) NULL,
    `ModifiedBy` VARCHAR(255) NULL,
    `RowVersion` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    PRIMARY KEY (`Id`),
    UNIQUE KEY `UX_Sys_UserAccounts_Username` (`Username`)
);

CREATE TABLE IF NOT EXISTS `Pos_OfficialReceipts` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `ReceiptNumber` VARCHAR(20) NOT NULL,
    `Amount` DECIMAL(18,2) NOT NULL,
    `Status` VARCHAR(20) NOT NULL DEFAULT 'Issued',
    `IssuedAt` DATETIME(6) NOT NULL,
    `IntegrityHash` VARCHAR(64) NULL,
    `CreatedAt` DATETIME(6) NOT NULL,
    `ModifiedAt` DATETIME(6) NULL,
    `CreatedBy` VARCHAR(255) NULL,
    `ModifiedBy` VARCHAR(255) NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `UX_Pos_OfficialReceipts_ReceiptNumber` (`ReceiptNumber`),
    KEY `IX_OfficialReceipts_IssuedAt` (`IssuedAt`)
);
