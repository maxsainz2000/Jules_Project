Imports System.Drawing

Namespace Services.Theming
    Public Class ThemeTokens
        Public Property WindowBackground As Color
        Public Property SurfaceBackground As Color
        Public Property SidebarBackground As Color
        Public Property PrimaryText As Color
        Public Property SecondaryText As Color
        Public Property BorderDefault As Color
        Public Property Accent As Color
        Public Property Danger As Color
        Public Property Success As Color
        Public Property Warning As Color

        Public Property DefaultFont As Font
        Public Property HeaderFont As Font
        Public Property CaptionFont As Font

        Public Shared Function CreateLightTokens() As ThemeTokens
            Return New ThemeTokens() With {
                .WindowBackground = ColorTranslator.FromHtml("#F5F5F7"),
                .SurfaceBackground = ColorTranslator.FromHtml("#FFFFFF"),
                .SidebarBackground = ColorTranslator.FromHtml("#ECECEE"),
                .PrimaryText = ColorTranslator.FromHtml("#1D1D1F"),
                .SecondaryText = ColorTranslator.FromHtml("#6E6E73"),
                .BorderDefault = ColorTranslator.FromHtml("#D2D2D7"),
                .Accent = ColorTranslator.FromHtml("#0071E3"),
                .Danger = ColorTranslator.FromHtml("#FF3B30"),
                .Success = ColorTranslator.FromHtml("#34C759"),
                .Warning = ColorTranslator.FromHtml("#FF9500"),
                .DefaultFont = New Font("Segoe UI", 9.0F),
                .HeaderFont = New Font("Segoe UI", 14.0F, FontStyle.Bold),
                .CaptionFont = New Font("Segoe UI", 8.5F)
            }
        End Function

        Public Shared Function CreateDarkTokens() As ThemeTokens
            Return New ThemeTokens() With {
                .WindowBackground = ColorTranslator.FromHtml("#1E1E1E"),
                .SurfaceBackground = ColorTranslator.FromHtml("#2D2D30"),
                .SidebarBackground = ColorTranslator.FromHtml("#181818"),
                .PrimaryText = ColorTranslator.FromHtml("#F5F5F7"),
                .SecondaryText = ColorTranslator.FromHtml("#A1A1A6"),
                .BorderDefault = ColorTranslator.FromHtml("#38383A"),
                .Accent = ColorTranslator.FromHtml("#0A84FF"),
                .Danger = ColorTranslator.FromHtml("#FF453A"),
                .Success = ColorTranslator.FromHtml("#30D158"),
                .Warning = ColorTranslator.FromHtml("#FF9F0A"),
                .DefaultFont = New Font("Segoe UI", 9.0F),
                .HeaderFont = New Font("Segoe UI", 14.0F, FontStyle.Bold),
                .CaptionFont = New Font("Segoe UI", 8.5F)
            }
        End Function
    End Class
End Namespace
