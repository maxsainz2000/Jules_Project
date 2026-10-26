Imports System.Drawing

Namespace Services.Theming
    Public Class ThemeTokens
        Public Property WindowBackground As Color
        Public Property SurfaceBackground As Color
        Public Property CardBackground As Color
        Public Property SidebarBackground As Color

        Public Property Accent As Color

        Public Property PrimaryText As Color
        Public Property SecondaryText As Color
        Public Property MutedText As Color

        Public Property Border As Color

        Public Property Danger As Color
        Public Property Warning As Color
        Public Property Success As Color

        Public Property DefaultFont As Font

        Public Shared Function GetTokens(theme As AppTheme) As ThemeTokens
            If theme = AppTheme.Dark Then
                Return New ThemeTokens With {
                    .WindowBackground = Color.FromArgb(30, 30, 30),
                    .SurfaceBackground = Color.FromArgb(45, 45, 48),
                    .CardBackground = Color.FromArgb(63, 63, 70),
                    .SidebarBackground = Color.FromArgb(37, 37, 38),
                    .Accent = Color.FromArgb(0, 122, 204),
                    .PrimaryText = Color.FromArgb(241, 241, 241),
                    .SecondaryText = Color.FromArgb(200, 200, 200),
                    .MutedText = Color.FromArgb(153, 153, 153),
                    .Border = Color.FromArgb(85, 85, 85),
                    .Danger = Color.FromArgb(241, 76, 76),
                    .Warning = Color.FromArgb(204, 167, 0),
                    .Success = Color.FromArgb(137, 209, 133),
                    .DefaultFont = New Font("Segoe UI", 9.0F, FontStyle.Regular, GraphicsUnit.Point)
                }
            Else
                Return New ThemeTokens With {
                    .WindowBackground = Color.FromArgb(240, 240, 240),
                    .SurfaceBackground = Color.FromArgb(255, 255, 255),
                    .CardBackground = Color.FromArgb(250, 250, 250),
                    .SidebarBackground = Color.FromArgb(230, 230, 230),
                    .Accent = Color.FromArgb(0, 120, 215),
                    .PrimaryText = Color.FromArgb(0, 0, 0),
                    .SecondaryText = Color.FromArgb(51, 51, 51),
                    .MutedText = Color.FromArgb(118, 118, 118),
                    .Border = Color.FromArgb(204, 204, 204),
                    .Danger = Color.FromArgb(232, 17, 35),
                    .Warning = Color.FromArgb(255, 140, 0),
                    .Success = Color.FromArgb(16, 124, 16),
                    .DefaultFont = New Font("Segoe UI", 9.0F, FontStyle.Regular, GraphicsUnit.Point)
                }
            End If
        End Function
    End Class
End Namespace
