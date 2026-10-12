Namespace Interfaces
    ''' <summary>
    ''' Supplies the identity stamped into audit columns (<c>CreatedBy</c> / <c>ModifiedBy</c>).
    ''' Kept deliberately minimal so the data layer does not depend on the full session
    ''' abstraction.
    ''' </summary>
    Public Interface ICurrentUserProvider
        ''' <summary>Username of the signed-in user, or <c>Nothing</c> when no session is active.</summary>
        ReadOnly Property Username As String
    End Interface
End Namespace
