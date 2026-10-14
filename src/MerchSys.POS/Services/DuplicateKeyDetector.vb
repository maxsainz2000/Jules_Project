Imports Microsoft.EntityFrameworkCore

Namespace Services

    ''' <summary>
    ''' Recognises a unique-constraint violation from EF's <see cref="DbUpdateException"/>.
    '''
    ''' The check is on the provider's message rather than on its exception type on purpose: the ADO
    ''' provider is Oracle's <c>MySql.Data</c>, which this module does not reference directly, and
    ''' catching its type would tie POS to the provider. MariaDB reports ER_DUP_ENTRY as
    ''' "Duplicate entry '...' for key '...'".
    '''
    ''' Both sequential-identifier paths need this: the transaction number is derived from the current
    ''' maximum and the receipt sequence from the current year, so two clients can derive the same
    ''' value and the unique index rejects the loser.
    ''' </summary>
    Friend Module DuplicateKeyDetector

        Friend Function IsDuplicateKey(ex As DbUpdateException) As Boolean
            Dim inner = ex.InnerException
            If inner Is Nothing Then Return False

            Return inner.Message.Contains("Duplicate entry")
        End Function

    End Module

End Namespace
