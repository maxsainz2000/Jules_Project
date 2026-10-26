Imports Xunit
Imports System.Drawing
Imports MerchSys.App.Services.Theming

Public Class IconHelperTests

    <Fact>
    Public Sub GetIcon_ReturnsValidBitmap_ForAllEnumValues()
        For Each iconVal As AppIcon In [Enum].GetValues(GetType(AppIcon))
            Using bmp = IconHelper.GetIcon(iconVal, Color.Red, 24)
                Assert.NotNull(bmp)
                Assert.Equal(24, bmp.Width)
                Assert.Equal(24, bmp.Height)
            End Using
        Next
    End Sub

    <Fact>
    Public Sub GetThemedIcon_ReturnsValidBitmap_WithTokens()
        Dim tokens = ThemeTokens.GetTokens(AppTheme.Light)
        For Each iconVal As AppIcon In [Enum].GetValues(GetType(AppIcon))
            Using bmp = IconHelper.GetThemedIcon(iconVal, tokens, 16)
                Assert.NotNull(bmp)
                Assert.Equal(16, bmp.Width)
                Assert.Equal(16, bmp.Height)
            End Using
        Next
    End Sub

    <Fact>
    Public Sub GetThemedIcon_ReturnsValidBitmap_WithNullTokens()
        For Each iconVal As AppIcon In [Enum].GetValues(GetType(AppIcon))
            Using bmp = IconHelper.GetThemedIcon(iconVal, Nothing, 32)
                Assert.NotNull(bmp)
                Assert.Equal(32, bmp.Width)
                Assert.Equal(32, bmp.Height)
            End Using
        Next
    End Sub
End Class
