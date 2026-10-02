Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports MerchSys.SharedKernel.Queries
Imports Microsoft.Extensions.Configuration
Imports MySqlConnector

Namespace Handlers

    Public Class GetProductsForCatalogQueryHandler
        Implements IRequestHandler(Of GetProductsForCatalogQuery, List(Of CatalogProductDto))

        Private ReadOnly _configuration As IConfiguration

        Public Sub New(configuration As IConfiguration)
            _configuration = configuration
        End Sub

        Public Async Function Handle(request As GetProductsForCatalogQuery, cancellationToken As CancellationToken) As Task(Of List(Of CatalogProductDto)) Implements IRequestHandler(Of GetProductsForCatalogQuery, List(Of CatalogProductDto)).Handle
            Dim resultList = New List(Of CatalogProductDto)()

            Dim connectionString = _configuration.GetSection("Sync")("MariaDbConnection")
            If String.IsNullOrWhiteSpace(connectionString) Then
                Throw New InvalidOperationException("MariaDbConnection string is not configured.")
            End If

            Dim query = "SELECT Id, Name, RetailPrice FROM Inv_Products WHERE IsDeleted = 0"

            Using connection = New MySqlConnection(connectionString)
                Await connection.OpenAsync(cancellationToken)
                Using command = New MySqlCommand(query, connection)
                    Using reader = Await command.ExecuteReaderAsync(cancellationToken)
                        While Await reader.ReadAsync(cancellationToken)
                            Dim id = reader.GetInt32("Id")
                            Dim name = reader.GetString("Name")
                            Dim retailPrice = reader.GetDecimal("RetailPrice")

                            resultList.Add(New CatalogProductDto With {
                                .Id = id,
                                .Name = name,
                                .RetailPrice = retailPrice
                            })
                        End While
                    End Using
                End Using
            End Using

            Return resultList
        End Function

    End Class

End Namespace
