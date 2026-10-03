Imports MediatR

Namespace Queries
    Public Class GetProductCatalogQuery
        Implements IRequest(Of GetProductCatalogResult)

        Public Property SearchTerm As String
    End Class
End Namespace
