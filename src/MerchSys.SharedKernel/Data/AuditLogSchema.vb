Namespace Data

    ''' <summary>
    ''' The shared audit-log table's name and DDL.
    '''
    ''' The table is central: all four module contexts write it and no module owns it. It is
    ''' deliberately **not** an EF entity, because <c>IRelationalDatabaseCreator.CreateTables()</c> is not
    ''' idempotent — it aborts on the first table that already exists — so a table mapped into more than
    ''' one context would be created by the first and abort the second, both in the test fixture and on a
    ''' fresh database at startup.
    '''
    ''' The DDL lives here so there is one definition; executing it needs an ADO provider, so that half
    ''' is in <c>MerchSys.App.Data.AuditLogTableBootstrapper</c>.
    ''' </summary>
    Public NotInheritable Class AuditLogSchema

        Private Sub New()
        End Sub

        Public Const TableName As String = "App_AuditLog"

        ''' <summary>Idempotent — safe to run on every startup.</summary>
        Public Const CreateTableSql As String =
            "CREATE TABLE IF NOT EXISTS `App_AuditLog` (" &
            "  `Id` INT AUTO_INCREMENT PRIMARY KEY," &
            "  `OccurredAt` DATETIME(6) NOT NULL," &
            "  `Actor` VARCHAR(256) NULL," &
            "  `WriteContext` VARCHAR(16) NOT NULL," &
            "  `Module` VARCHAR(8) NOT NULL," &
            "  `EntityType` VARCHAR(128) NOT NULL," &
            "  `EntityId` VARCHAR(64) NULL," &
            "  `Action` VARCHAR(8) NOT NULL," &
            "  `ChangesJson` LONGTEXT NULL," &
            "  INDEX `IX_App_AuditLog_OccurredAt` (`OccurredAt`)," &
            "  INDEX `IX_App_AuditLog_Actor` (`Actor`)," &
            "  INDEX `IX_App_AuditLog_Entity` (`EntityType`, `EntityId`)" &
            ");"

    End Class

End Namespace
