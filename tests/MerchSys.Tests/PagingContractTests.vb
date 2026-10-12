Imports Xunit
Imports MerchSys.SharedKernel.Paging

Public Class PagingContractTests

    <Fact>
    Public Sub PageRequestDefaultsTo100AndFirstPage()
        Dim request = New PageRequest()

        Assert.Equal(100, request.PageSize)
        Assert.True(request.IsFirstPage)
        Assert.False(request.CursorDate.HasValue)
        Assert.False(request.CursorId.HasValue)
    End Sub

    <Fact>
    Public Sub PagedResultCarriesTheNextCursor()
        Dim page = New PagedResult(Of Integer)()
        page.Items.Add(7)
        page.HasMore = True
        page.NextCursorDate = New System.DateTime(2026, 10, 11)
        page.NextCursorId = 42

        Assert.Equal(1, page.Items.Count)
        Assert.True(page.HasMore)
        Assert.Equal(42, page.NextCursorId)
    End Sub
End Class
