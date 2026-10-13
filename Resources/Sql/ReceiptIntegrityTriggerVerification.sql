-- Diagnostic query bundle for receipt-archive trigger verification.
--
-- Both archive tables are append-only: the triggers reject UPDATE and DELETE and allow INSERT.
-- Every probe below must fail with:  Error: BIR-archive-immutable
--
-- A probe that fails with "Unknown column" instead means the column names here no longer match the
-- EF model — fix this file, not the schema. An earlier revision of this file probed Amount / Status /
-- IntegrityHash on Pos_OfficialReceiptArchive; none of those columns exist there, so the probes
-- "failed" for the wrong reason and proved nothing.

-- 1. Trigger inventory — the authoritative check. Expect FOUR rows: an UPDATE and a DELETE trigger
--    on each of the two archive tables.
SELECT
    TRIGGER_SCHEMA,
    TRIGGER_NAME,
    EVENT_MANIPULATION,
    EVENT_OBJECT_TABLE
FROM
    information_schema.TRIGGERS
WHERE
    EVENT_OBJECT_TABLE IN ('Pos_OfficialReceiptArchive', 'Pos_ReceiptIntegrityArchive')
ORDER BY
    EVENT_OBJECT_TABLE, EVENT_MANIPULATION;

-- 2. Negative-path probes. The triggers are BEFORE ... FOR EACH ROW, so a statement only fails when
--    it actually matches a row: run these against a database that holds archive rows, and read a
--    "0 rows affected" result as inconclusive rather than as a pass.

-- Pos_OfficialReceiptArchive — UPDATE (columns per OfficialReceiptArchive)
UPDATE Pos_OfficialReceiptArchive SET ReceiptNumber = 'tampered' WHERE Id = 1;
UPDATE Pos_OfficialReceiptArchive SET ArchivedHash = 'new_fake_hash' WHERE Id = 1;
UPDATE Pos_OfficialReceiptArchive SET SerializedLines = '[]' WHERE Id = 1;

-- Pos_OfficialReceiptArchive — DELETE
DELETE FROM Pos_OfficialReceiptArchive WHERE Id = 1;

-- Pos_ReceiptIntegrityArchive — UPDATE (columns per ReceiptIntegrityArchive)
UPDATE Pos_ReceiptIntegrityArchive SET IntegrityHash = 'tampered' WHERE Id = 1;
UPDATE Pos_ReceiptIntegrityArchive SET CanonicalPayload = '{}' WHERE Id = 1;

-- Pos_ReceiptIntegrityArchive — DELETE
DELETE FROM Pos_ReceiptIntegrityArchive WHERE Id = 1;
