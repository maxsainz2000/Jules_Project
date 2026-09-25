CREATE TABLE `UserAccounts` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Username` VARCHAR(255) NOT NULL UNIQUE,
    `PasswordHash` VARCHAR(255) NOT NULL,
    `Role` INT NOT NULL,
    `IsActive` BOOLEAN NOT NULL DEFAULT 1,
    `FailedLoginAttempts` INT NOT NULL DEFAULT 0,
    `LockedUntil` DATETIME(6) NULL,
    `LastPasswordChangeAt` DATETIME(6) NULL,
    `CreatedAt` DATETIME(6) NOT NULL,
    `ModifiedAt` DATETIME(6) NULL,
    `CreatedBy` VARCHAR(255) NULL,
    `ModifiedBy` VARCHAR(255) NULL
);
