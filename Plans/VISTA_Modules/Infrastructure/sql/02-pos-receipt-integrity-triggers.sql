-- ==============================================================================
-- Receipt-archive immutability triggers (BIR / NIRC sec. 235)
--
-- Applied automatically at startup by ReceiptArchiveTriggerBootstrapper (Layer 3 of
-- DatabaseBootstrapper), AFTER the EF model has created the archive tables. EF Core cannot
-- create triggers, so they live here. The runbook also lists this file for manual application.
--
-- Two archive tables are covered, each with an UPDATE and a DELETE trigger:
--   Pos_OfficialReceiptArchive   -> pos_receipt_archive_no_update / _no_delete
--   Pos_ReceiptIntegrityArchive  -> pos_receipt_integrity_archive_no_update / _no_delete
--
-- Both are BIR retention tables (10 years). They are append-only: INSERT must stay allowed,
-- because ReceiptArchivalService inserts rows. ImmutableReceiptInterceptor covers both as well,
-- but it is a SaveChanges interceptor — direct SQL bypasses it — so these triggers are the only
-- enforcement that survives a client that does not go through EF.
--
-- This script deliberately does NOT create the tables. They are owned by the EF model
-- (OfficialReceiptArchiveConfiguration / ReceiptIntegrityArchiveConfiguration). An earlier
-- revision created Pos_OfficialReceiptArchive with a different column set; because
-- DatabaseBootstrapper treats a partially-present schema as "skip this module", doing that
-- would have left the other ten POS tables unbuilt.
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

DROP TRIGGER IF EXISTS pos_receipt_integrity_archive_no_update //
CREATE TRIGGER pos_receipt_integrity_archive_no_update
BEFORE UPDATE ON Pos_ReceiptIntegrityArchive
FOR EACH ROW
BEGIN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: BIR-archive-immutable';
END //

DROP TRIGGER IF EXISTS pos_receipt_integrity_archive_no_delete //
CREATE TRIGGER pos_receipt_integrity_archive_no_delete
BEFORE DELETE ON Pos_ReceiptIntegrityArchive
FOR EACH ROW
BEGIN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: BIR-archive-immutable';
END //

DELIMITER ;
