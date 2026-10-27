Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.Linq
Imports System.ComponentModel

Namespace Controls
    Public Class Sparkline
        Inherits Control

        Private _points As List(Of Decimal)

        Public Sub New()
            DoubleBuffered = True
            SetStyle(ControlStyles.ResizeRedraw, True)
            SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            SetStyle(ControlStyles.UserPaint, True)
            BackColor = Color.Transparent
            Size = New Size(100, 30)
            _points = New List(Of Decimal)()
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Points As List(Of Decimal)
            Get
                Return _points
            End Get
            Set(value As List(Of Decimal))
                _points = value
                Invalidate()
            End Set
        End Property

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)

            If _points Is Nothing OrElse _points.Count < 2 Then Return

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

            Dim minVal = _points.Min()
            Dim maxVal = _points.Max()
            Dim range = maxVal - minVal
            If range = 0D Then range = 1D ' avoid division by zero

            Dim w = Width - 4 ' padding
            Dim h = Height - 4

            Dim pts(_points.Count - 1) As PointF
            Dim xStep = CSng(w) / (_points.Count - 1)

            For i = 0 To _points.Count - 1
                Dim x = 2 + (i * xStep)
                Dim normalizedY = CSng((_points(i) - minVal) / range)
                Dim y = 2 + (h - (normalizedY * h))
                pts(i) = New PointF(x, y)
            Next

            Using pen As New Pen(Color.FromArgb(0, 122, 204), 2.0F) ' Accent color
                pen.LineJoin = LineJoin.Round
                e.Graphics.DrawLines(pen, pts)
            End Using
        End Sub
    End Class
End Namespace
