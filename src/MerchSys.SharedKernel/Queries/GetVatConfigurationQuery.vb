Imports MediatR
Imports MerchSys.SharedKernel.Queries

Namespace Queries
    Public Class GetVatConfigurationQuery
        Implements MediatR.IRequest(Of GetVatConfigurationResult)
    End Class
End Namespace