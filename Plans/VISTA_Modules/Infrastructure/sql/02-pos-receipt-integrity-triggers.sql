-- ==============================================================================
-- Pos_OfficialReceiptArchive immutability triggers (BIR / NIRC sec. 235)
--
-- RUN THIS **AFTER** THE APPLICATION'S FIRST LAUNCH.
--
-- The table itself is owned by the EF model —
-- MerchSys.POS/Data/Configurations/OfficialReceiptArchiveConfiguration.vb maps
-- ToTable("Pos_OfficialReceiptArchive") — and is created by DatabaseBootstrapper.
-- EF Core cannot create triggers, so they live here.
--
-- This script deliberately does NOT create the table. An earlier revision did, with a
-- different column set (Amount / Status / IssuedAt / IntegrityHash) than the model's
-- (OriginalReceiptId / TransactionId / ReceiptDate / SerializedLines / ArchivedAt /
-- ArchivedHash). Running it before the application would have created the wrong table,
-- and because DatabaseBootstrapper treats a partially-present schema as "skip this
-- module", the other ten POS tables would then never have been created at all.
--
-- Note: ImmutableReceiptInterceptor enforces immutability for OfficialReceipt and
-- ReceiptIntegrity in application code. It does NOT cover OfficialReceiptArchive, so
-- these triggers are the only enforcement for that table.
--
-- Verify with Resources/Sql/ReceiptIntegrityTriggerVerification.sql
-- Idempotent: yes (drops before creating)
-- ==============================================================================

DELIMITER //

DROP TRIGGER IF EXISTS pos_receipt_archive_no_update //
CREATE TRIGGER pos_receipt_archive_no_update
BEFORE UPDATE ON Pos_OfficialReceiptArchive
FOR EACH ROW
BEGIN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: BIR-archive-immutable';
END //

DROP TRIGGER IF EXISTS pos_receipt_archive_no_delete //
CREATE TRIGGER pos_receipt_archive_no_delete
BEFORE DELETE ON Pos_OfficialReceiptArchive
FOR EACH ROW
BEGIN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: BIR-archive-immutable';
END //

DELIMITER ;
