Imports System

Namespace Helpers
    Public Class SequentialNumberGenerator
        Public Shared Function Generate(prefix As String, year As Integer, existingNumbers As IEnumerable(Of String)) As String
            Dim maxSequence = 0
            Dim yearStr = year.ToString("0000")
            Dim fullPrefix = $"{prefix}-{yearStr}-"

            If existingNumbers IsNot Nothing Then
                For Each num In existingNumbers
                    If num IsNot Nothing AndAlso num.StartsWith(fullPrefix) Then
                        Dim suffix = num.Substring(fullPrefix.Length)
                        Dim parsedSequence As Integer
                        If Integer.TryParse(suffix, parsedSequence) Then
                            If parsedSequence > maxSequence Then
                                maxSequence = parsedSequence
                            End If
                        End If
                    End If
                Next
            End If

            maxSequence += 1
            Return $"{fullPrefix}{maxSequence.ToString("0000")}"
        End Function
    End Class
End Namespace
