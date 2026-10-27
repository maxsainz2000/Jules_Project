Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.ComponentModel

Namespace Controls
    Public Enum DeltaDirection
        Up
        Down
        Flat
        None
    End Enum

    Public Class DeltaIndicator
        Inherits Control

        Private _percent As Decimal?
        Private _invertSemantics As Boolean

        Public Sub New()
            DoubleBuffered = True
            SetStyle(ControlStyles.ResizeRedraw, True)
            SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            SetStyle(ControlStyles.UserPaint, True)
            BackColor = Color.Transparent
            Size = New Size(80, 20)
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Percent As Decimal?
            Get
                Return _percent
            End Get
            Set(value As Decimal?)
                _percent = value
                Invalidate()
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property InvertSemantics As Boolean
            Get
                Return _invertSemantics
            End Get
            Set(value As Boolean)
                _invertSemantics = value
                Invalidate()
            End Set
        End Property

        Public ReadOnly Property Direction As DeltaDirection
            Get
                If Not _percent.HasValue Then Return DeltaDirection.None
                If _percent.Value > 0D Then Return DeltaDirection.Up
                If _percent.Value < 0D Then Return DeltaDirection.Down
                Return DeltaDirection.Flat
            End Get
        End Property

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)

            If Not _percent.HasValue Then Return

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit

            Dim dir = Direction
            Dim arrow As String = ""
            Dim textColor As Color = Color.Gray

            Select Case dir
                Case DeltaDirection.Up
                    arrow = "▲"
                    textColor = If(_invertSemantics, Color.FromArgb(220, 53, 69), Color.FromArgb(40, 167, 69))
                Case DeltaDirection.Down
                    arrow = "▼"
                    textColor = If(_invertSemantics, Color.FromArgb(40, 167, 69), Color.FromArgb(220, 53, 69))
                Case DeltaDirection.Flat
                    arrow = "─"
                    textColor = Color.Gray
            End Select

            Dim formattedVal As String
            If _percent.Value > 0D Then
                formattedVal = $"+{_percent.Value:N1}%"
            ElseIf _percent.Value < 0D Then
                formattedVal = $"{_percent.Value:N1}%"
            Else
                formattedVal = "0.0%"
            End If

            Dim text As String = $"{arrow} {formattedVal}"

            Using brush As New SolidBrush(textColor)
                Using font As New Font("Segoe UI", 9.0F, FontStyle.Bold)
                    Dim size = e.Graphics.MeasureString(text, font)
                    Dim x = 0 ' (Width - size.Width) / 2
                    Dim y = (Height - size.Height) / 2
                    e.Graphics.DrawString(text, font, brush, x, y)
                End Using
            End Using
        End Sub
    End Class
End Namespace
