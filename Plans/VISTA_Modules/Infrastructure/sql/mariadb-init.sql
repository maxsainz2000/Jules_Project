-- MerchSys Central MariaDB Init Script
-- Schema Version: 1.1.0
-- Initial setup for central synchronization database

CREATE TABLE IF NOT EXISTS `Pos_OfficialReceipts` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `ReceiptNumber` VARCHAR(20) NOT NULL,
    `Amount` DECIMAL(18,2) NOT NULL,
    `Status` VARCHAR(20) NOT NULL DEFAULT 'Issued',
    `IssuedAt` DATETIME(6) NOT NULL,
    `IntegrityHash` VARCHAR(64) NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `UX_Pos_OfficialReceipts_ReceiptNumber` (`ReceiptNumber`),
    KEY `IX_OfficialReceipts_IssuedAt` (`IssuedAt`)
);
