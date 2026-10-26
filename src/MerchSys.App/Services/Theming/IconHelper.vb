Imports System.Drawing
Imports System.Drawing.Drawing2D

Namespace Services.Theming
    Public Enum AppIcon
        Lightbulb
        Warning
        Check
        Refresh
        Eye
        EyeOff
        Folder
        Sun
        Moon
        Close
        Search
    End Enum

    Public Class IconHelper
        Public Shared Function GetIcon(icon As AppIcon, color As Color, Optional size As Integer = 16) As Bitmap
            Dim bmp As New Bitmap(size, size, Imaging.PixelFormat.Format32bppArgb)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.SmoothingMode = SmoothingMode.AntiAlias
                g.PixelOffsetMode = PixelOffsetMode.HighQuality
                g.InterpolationMode = InterpolationMode.HighQualityBicubic

                Dim padding As Single = CSng(size) * 0.1F
                Dim drawSize As Single = size - (padding * 2)
                Dim rect As New RectangleF(padding, padding, drawSize, drawSize)

                Using p As New Pen(color, CSng(size) * 0.1F)
                    p.StartCap = LineCap.Round
                    p.EndCap = LineCap.Round
                    p.LineJoin = LineJoin.Round

                    Using b As New SolidBrush(color)
                        Using gp As New GraphicsPath()
                            Select Case icon
                                Case AppIcon.Lightbulb
                                    ' Draw a simple bulb using GraphicsPath
                                    Dim bulbRect As New RectangleF(rect.X + rect.Width * 0.25F, rect.Y, rect.Width * 0.5F, rect.Height * 0.6F)
                                    gp.AddEllipse(bulbRect)
                                    gp.AddLine(rect.X + rect.Width * 0.35F, rect.Y + rect.Height * 0.6F, rect.X + rect.Width * 0.35F, rect.Y + rect.Height * 0.8F)
                                    gp.AddLine(rect.X + rect.Width * 0.65F, rect.Y + rect.Height * 0.6F, rect.X + rect.Width * 0.65F, rect.Y + rect.Height * 0.8F)
                                    gp.AddLine(rect.X + rect.Width * 0.4F, rect.Bottom, rect.X + rect.Width * 0.6F, rect.Bottom)
                                    gp.CloseFigure()
                                Case AppIcon.Warning
                                    Dim pts() As PointF = {
                                        New PointF(rect.X + rect.Width / 2, rect.Y),
                                        New PointF(rect.Right, rect.Bottom),
                                        New PointF(rect.Left, rect.Bottom)
                                    }
                                    gp.AddPolygon(pts)
                                    gp.AddLine(rect.X + rect.Width / 2, rect.Y + rect.Height * 0.35F, rect.X + rect.Width / 2, rect.Y + rect.Height * 0.7F)
                                    gp.StartFigure()
                                    gp.AddEllipse(rect.X + rect.Width / 2 - p.Width / 2, rect.Y + rect.Height * 0.8F, p.Width, p.Width)
                                Case AppIcon.Check
                                    gp.AddLine(rect.X, rect.Y + rect.Height / 2, rect.X + rect.Width / 3, rect.Bottom)
                                    gp.AddLine(rect.X + rect.Width / 3, rect.Bottom, rect.Right, rect.Y)
                                Case AppIcon.Refresh
                                    gp.AddArc(rect, 45, 270)
                                    gp.StartFigure()
                                    gp.AddLine(rect.Right - p.Width, rect.Y + rect.Height / 2 - p.Width * 2, rect.Right, rect.Y + rect.Height / 2)
                                    gp.AddLine(rect.Right + p.Width, rect.Y + rect.Height / 2 - p.Width * 2, rect.Right, rect.Y + rect.Height / 2)
                                Case AppIcon.Eye
                                    gp.AddCurve({New PointF(rect.Left, rect.Y + rect.Height / 2), New PointF(rect.X + rect.Width / 2, rect.Y), New PointF(rect.Right, rect.Y + rect.Height / 2)})
                                    gp.StartFigure()
                                    gp.AddCurve({New PointF(rect.Left, rect.Y + rect.Height / 2), New PointF(rect.X + rect.Width / 2, rect.Bottom), New PointF(rect.Right, rect.Y + rect.Height / 2)})
                                    gp.StartFigure()
                                    gp.AddEllipse(rect.X + rect.Width * 0.35F, rect.Y + rect.Height * 0.35F, rect.Width * 0.3F, rect.Height * 0.3F)
                                Case AppIcon.EyeOff
                                    gp.AddCurve({New PointF(rect.Left, rect.Y + rect.Height / 2), New PointF(rect.X + rect.Width / 2, rect.Y), New PointF(rect.Right, rect.Y + rect.Height / 2)})
                                    gp.StartFigure()
                                    gp.AddCurve({New PointF(rect.Left, rect.Y + rect.Height / 2), New PointF(rect.X + rect.Width / 2, rect.Bottom), New PointF(rect.Right, rect.Y + rect.Height / 2)})
                                    gp.StartFigure()
                                    gp.AddEllipse(rect.X + rect.Width * 0.35F, rect.Y + rect.Height * 0.35F, rect.Width * 0.3F, rect.Height * 0.3F)
                                    gp.StartFigure()
                                    gp.AddLine(rect.Left, rect.Bottom, rect.Right, rect.Y)
                                Case AppIcon.Folder
                                    Dim fRect As New RectangleF(rect.X, rect.Y + rect.Height * 0.2F, rect.Width, rect.Height * 0.8F)
                                    gp.AddRectangle(fRect)
                                    gp.StartFigure()
                                    gp.AddLine(rect.X, rect.Y + rect.Height * 0.2F, rect.X + rect.Width * 0.4F, rect.Y + rect.Height * 0.2F)
                                    gp.AddLine(rect.X + rect.Width * 0.4F, rect.Y + rect.Height * 0.2F, rect.X + rect.Width * 0.5F, rect.Y)
                                    gp.AddLine(rect.X + rect.Width * 0.5F, rect.Y, rect.Right, rect.Y)
                                    gp.AddLine(rect.Right, rect.Y, rect.Right, rect.Y + rect.Height * 0.2F)
                                Case AppIcon.Sun
                                    gp.AddEllipse(rect.X + rect.Width * 0.25F, rect.Y + rect.Height * 0.25F, rect.Width * 0.5F, rect.Height * 0.5F)
                                    gp.StartFigure()
                                    gp.AddLine(rect.X + rect.Width / 2, rect.Y, rect.X + rect.Width / 2, rect.Y + rect.Height * 0.15F)
                                    gp.StartFigure()
                                    gp.AddLine(rect.X + rect.Width / 2, rect.Bottom - rect.Height * 0.15F, rect.X + rect.Width / 2, rect.Bottom)
                                    gp.StartFigure()
                                    gp.AddLine(rect.X, rect.Y + rect.Height / 2, rect.X + rect.Width * 0.15F, rect.Y + rect.Height / 2)
                                    gp.StartFigure()
                                    gp.AddLine(rect.Right - rect.Width * 0.15F, rect.Y + rect.Height / 2, rect.Right, rect.Y + rect.Height / 2)
                                Case AppIcon.Moon
                                    gp.AddArc(rect, 90, 270)
                                    gp.StartFigure()
                                    gp.AddArc(New RectangleF(rect.X + rect.Width * 0.25F, rect.Y, rect.Width * 0.75F, rect.Height), 90, 270)
                                Case AppIcon.Close
                                    gp.AddLine(rect.X, rect.Y, rect.Right, rect.Bottom)
                                    gp.StartFigure()
                                    gp.AddLine(rect.Right, rect.Y, rect.X, rect.Bottom)
                                Case AppIcon.Search
                                    Dim sRect As New RectangleF(rect.X, rect.Y, rect.Width * 0.6F, rect.Height * 0.6F)
                                    gp.AddEllipse(sRect)
                                    gp.StartFigure()
                                    gp.AddLine(rect.X + rect.Width * 0.5F, rect.Y + rect.Height * 0.5F, rect.Right, rect.Bottom)
                            End Select
                            g.DrawPath(p, gp)
                        End Using
                    End Using
                End Using
            End Using
            Return bmp
        End Function

        Public Shared Function GetThemedIcon(icon As AppIcon, tokens As ThemeTokens, Optional size As Integer = 16) As Bitmap
            Dim c As Color = Color.Gray ' default
            If tokens IsNot Nothing Then
                Select Case icon
                    Case AppIcon.Lightbulb, AppIcon.Warning
                        c = tokens.Warning
                    Case AppIcon.Check
                        c = tokens.Success
                    Case AppIcon.Close
                        c = tokens.Danger
                    Case Else
                        c = tokens.PrimaryText
                End Select
            Else
                Select Case icon
                    Case AppIcon.Lightbulb, AppIcon.Warning
                        c = Color.Goldenrod
                    Case AppIcon.Check
                        c = Color.Green
                    Case AppIcon.Close
                        c = Color.Red
                    Case Else
                        c = Color.Black
                End Select
            End If

            Return GetIcon(icon, c, size)
        End Function
    End Class
End Namespace
