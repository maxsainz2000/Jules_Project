-- ==============================================================================
-- Migration: Align Pos_OfficialReceipts schema (INFRA-14)
-- Purpose: Adds missing columns introduced by POS-13/14/15 to central MariaDB
-- Idempotent: Yes
-- Compatibility: INFRA-06, INFRA-08, and receipt integrity triggers
-- ==============================================================================

-- Add Status column if it does not exist
SET @dbname = DATABASE();
SET @tablename = 'Pos_OfficialReceipts';
SET @columnname = 'Status';
SET @preparedStatement = (SELECT IF(
  (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @dbname AND TABLE_NAME = @tablename AND COLUMN_NAME = @columnname) > 0,
  'SELECT 1',
  'ALTER TABLE `Pos_OfficialReceipts` ADD COLUMN `Status` VARCHAR(20) NOT NULL DEFAULT \'Issued\''
));
PREPARE addColumn FROM @preparedStatement;
EXECUTE addColumn;
DEALLOCATE PREPARE addColumn;

-- Add IssuedAt column if it does not exist
SET @columnname = 'IssuedAt';
SET @preparedStatement = (SELECT IF(
  (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @dbname AND TABLE_NAME = @tablename AND COLUMN_NAME = @columnname) > 0,
  'SELECT 1',
  'ALTER TABLE `Pos_OfficialReceipts` ADD COLUMN `IssuedAt` DATETIME(6) NOT NULL'
));
PREPARE addColumn FROM @preparedStatement;
EXECUTE addColumn;
DEALLOCATE PREPARE addColumn;

-- Add IntegrityHash column if it does not exist
SET @columnname = 'IntegrityHash';
SET @preparedStatement = (SELECT IF(
  (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @dbname AND TABLE_NAME = @tablename AND COLUMN_NAME = @columnname) > 0,
  'SELECT 1',
  'ALTER TABLE `Pos_OfficialReceipts` ADD COLUMN `IntegrityHash` VARCHAR(64) NULL'
));
PREPARE addColumn FROM @preparedStatement;
EXECUTE addColumn;
DEALLOCATE PREPARE addColumn;

-- Add IX_OfficialReceipts_IssuedAt index if it does not exist
SET @indexname = 'IX_OfficialReceipts_IssuedAt';
SET @preparedStatement = (SELECT IF(
  (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = @dbname AND TABLE_NAME = @tablename AND INDEX_NAME = @indexname) > 0,
  'SELECT 1',
  'ALTER TABLE `Pos_OfficialReceipts` ADD INDEX `IX_OfficialReceipts_IssuedAt` (`IssuedAt`)'
));
PREPARE addIndex FROM @preparedStatement;
EXECUTE addIndex;
DEALLOCATE PREPARE addIndex;
