Namespace Interfaces
    Public Enum WriteContextKind
        User
        System
        AuthSelfService
    End Enum

    Public Interface IWriteContextScope
        ReadOnly Property CurrentKind As WriteContextKind
        Function Enter(kind As WriteContextKind) As IDisposable
    End Interface
End Namespace
