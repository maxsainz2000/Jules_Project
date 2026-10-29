Namespace Models
    Public Enum CommandPaletteItemKind
        Screen
        Product
    End Enum

    Public Class CommandPaletteItem
        Public Property Title As String
        Public Property Subtitle As String
        Public Property Category As String
        Public Property Kind As CommandPaletteItemKind
        Public Property NavigationItem As NavigationItem
        Public Property ProductId As Integer?

        Public Overrides Function ToString() As String
            If Not String.IsNullOrEmpty(Subtitle) Then
                Return $"[{Category}] {Title} - {Subtitle}"
            End If
            Return $"[{Category}] {Title}"
        End Function
    End Class
End Namespace
