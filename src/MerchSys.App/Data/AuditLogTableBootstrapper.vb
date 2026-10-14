Imports Microsoft.Extensions.Logging
Imports MySqlConnector
Imports MerchSys.SharedKernel.Data

Namespace Data

    ''' <summary>
    ''' Creates the shared audit-log table.
    '''
    ''' Part of the central schema, but deliberately neither versioned SQL nor an EF entity: all four
    ''' module contexts write the table and none of them owns it, and <c>CreateTables()</c> is not
    ''' idempotent, so a table mapped into more than one context would be created by the first and abort
    ''' the second. See <see cref="AuditLogSchema"/>.
    '''
    ''' The DDL lives in <see cref="AuditLogSchema"/> so the test fixture creates the table exactly the
    ''' same way this does.
    ''' </summary>
    Public NotInheritable Class AuditLogTableBootstrapper

        Private Sub New()
        End Sub

        Public Shared Sub EnsureTable(connectionString As String, logger As ILogger)
            If String.IsNullOrWhiteSpace(connectionString) Then
                Throw New ArgumentException("A non-empty MariaDB connection string is required.", NameOf(connectionString))
            End If

            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Using cmd = conn.CreateCommand()
                    cmd.CommandText = AuditLogSchema.CreateTableSql
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            If logger IsNot Nothing Then
                logger.LogInformation("Audit log table {Table} is present.", AuditLogSchema.TableName)
            End If
        End Sub

    End Class

End Namespace
