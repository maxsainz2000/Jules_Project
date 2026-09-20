-- Diagnostic query bundle for trigger verification

-- 1. Trigger inventory query
SELECT
    TRIGGER_SCHEMA,
    TRIGGER_NAME,
    EVENT_MANIPULATION,
    EVENT_OBJECT_TABLE,
    ACTION_STATEMENT
FROM
    information_schema.TRIGGERS
WHERE
    EVENT_OBJECT_TABLE = 'Pos_OfficialReceiptArchive';

-- 2. 5 Negative-path UPDATE/DELETE test probes to verify immutability

-- Probe 1: Attempt to update Amount
UPDATE Pos_OfficialReceiptArchive
SET Amount = 100.00
WHERE Id = 1;

-- Probe 2: Attempt to update Status
UPDATE Pos_OfficialReceiptArchive
SET Status = 'Voided'
WHERE Id = 1;

-- Probe 3: Attempt to update IntegrityHash
UPDATE Pos_OfficialReceiptArchive
SET IntegrityHash = 'new_fake_hash'
WHERE Id = 1;

-- Probe 4: Attempt to delete a single row
DELETE FROM Pos_OfficialReceiptArchive
WHERE Id = 1;

-- Probe 5: Attempt to delete multiple rows (mass delete)
DELETE FROM Pos_OfficialReceiptArchive
WHERE Status = 'Voided';
