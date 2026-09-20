CREATE TABLE IF NOT EXISTS `Pos_OfficialReceiptArchive` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `ReceiptNumber` VARCHAR(20) NOT NULL,
    `Amount` DECIMAL(18,2) NOT NULL,
    `Status` VARCHAR(20) NOT NULL DEFAULT 'Issued',
    `IssuedAt` DATETIME(6) NOT NULL,
    `IntegrityHash` VARCHAR(64) NULL,
    PRIMARY KEY (`Id`)
);

DELIMITER //

CREATE TRIGGER pos_receipt_archive_no_update
BEFORE UPDATE ON Pos_OfficialReceiptArchive
FOR EACH ROW
BEGIN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: BIR-archive-immutable';
END //

CREATE TRIGGER pos_receipt_archive_no_delete
BEFORE DELETE ON Pos_OfficialReceiptArchive
FOR EACH ROW
BEGIN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: BIR-archive-immutable';
END //

DELIMITER ;
