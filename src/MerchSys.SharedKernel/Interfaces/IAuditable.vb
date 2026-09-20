Namespace Interfaces
    Public Interface IAuditable
        Property CreatedAt As DateTime
        Property CreatedBy As String
        Property ModifiedAt As DateTime?
        Property ModifiedBy As String
    End Interface
End Namespace
