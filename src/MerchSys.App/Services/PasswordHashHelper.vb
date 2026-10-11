Imports System.Security.Cryptography
Imports System.Text
Imports Konscious.Security.Cryptography

Namespace Services
    ''' <summary>
    ''' Argon2id password hashing.
    '''
    ''' Stored format is a self-describing PHC string:
    ''' <c>$argon2id$v=19$m=19456,t=2,p=1$&lt;base64-salt&gt;$&lt;base64-hash&gt;</c>.
    ''' Storing the parameters makes the cost tunable without invalidating existing
    ''' hashes, and a fresh random salt is generated per hash. Legacy values in the
    ''' old <c>Base64(salt ‖ hash)</c> format are still verified for backwards
    ''' compatibility (they are silently upgraded on the next password change).
    ''' </summary>
    Friend Module PasswordHashHelper

        Private Const SaltSize As Integer = 16
        Private Const DegreeOfParallelism As Integer = 1
        Private Const Iterations As Integer = 2
        Private Const MemorySize As Integer = 19456
        Private Const HashSize As Integer = 32

        Private Const Argon2Version As Integer = 19
        Private Const PhcAlgorithm As String = "argon2id"

        Public Function HashPassword(password As String) As String
            Dim salt As Byte() = New Byte(SaltSize - 1) {}
            RandomNumberGenerator.Fill(salt)

            Dim hash = ComputeHash(password, salt, Iterations, MemorySize, DegreeOfParallelism)

            Return $"${PhcAlgorithm}$v={Argon2Version}$m={MemorySize},t={Iterations},p={DegreeOfParallelism}${ToBase64(salt)}${ToBase64(hash)}"
        End Function

        Public Function VerifyPassword(password As String, hashString As String) As Boolean
            If String.IsNullOrEmpty(hashString) Then Return False

            If hashString.StartsWith("$" & PhcAlgorithm & "$", StringComparison.Ordinal) Then
                Return VerifyPhc(password, hashString)
            End If

            ' Legacy: Base64(salt ‖ hash) using the compile-time parameters.
            Return VerifyLegacy(password, hashString)
        End Function

        Private Function VerifyPhc(password As String, phc As String) As Boolean
            Try
                ' ["", "argon2id", "v=19", "m=19456,t=2,p=1", salt, hash]
                Dim parts = phc.Split("$"c)
                If parts.Length <> 6 Then Return False
                If Not String.Equals(parts(1), PhcAlgorithm, StringComparison.Ordinal) Then Return False
                If Not parts(2).StartsWith("v=", StringComparison.Ordinal) Then Return False
                If Integer.Parse(parts(2).Substring(2)) <> Argon2Version Then Return False

                Dim memorySize = 0
                Dim iterations = 0
                Dim parallelism = 0
                For Each kv In parts(3).Split(","c)
                    Dim pair = kv.Split("="c)
                    If pair.Length <> 2 Then Return False
                    Select Case pair(0)
                        Case "m" : memorySize = Integer.Parse(pair(1))
                        Case "t" : iterations = Integer.Parse(pair(1))
                        Case "p" : parallelism = Integer.Parse(pair(1))
                        Case Else : Return False
                    End Select
                Next

                Dim salt = FromBase64(parts(4))
                Dim expected = FromBase64(parts(5))
                Dim actual = ComputeHash(password, salt, iterations, memorySize, parallelism)

                If actual.Length <> expected.Length Then Return False
                Return CryptographicOperations.FixedTimeEquals(actual, expected)
            Catch ex As Exception When TypeOf ex Is FormatException OrElse TypeOf ex Is ArgumentException OrElse TypeOf ex Is OverflowException
                Return False
            End Try
        End Function

        Private Function VerifyLegacy(password As String, hashString As String) As Boolean
            Dim hashBytes As Byte()
            Try
                hashBytes = Convert.FromBase64String(hashString)
            Catch ex As FormatException
                Return False
            End Try

            If hashBytes Is Nothing OrElse hashBytes.Length <> SaltSize + HashSize Then Return False

            Dim salt As Byte() = New Byte(SaltSize - 1) {}
            Array.Copy(hashBytes, 0, salt, 0, SaltSize)

            Dim expectedHash As Byte() = New Byte(HashSize - 1) {}
            Array.Copy(hashBytes, SaltSize, expectedHash, 0, HashSize)

            Dim actualHash = ComputeHash(password, salt, Iterations, MemorySize, DegreeOfParallelism)
            Return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash)
        End Function

        Private Function ComputeHash(password As String, salt As Byte(), iterations As Integer, memorySize As Integer, parallelism As Integer) As Byte()
            Using argon2 = New Argon2id(Encoding.UTF8.GetBytes(password))
                argon2.Salt = salt
                argon2.DegreeOfParallelism = parallelism
                argon2.Iterations = iterations
                argon2.MemorySize = memorySize
                Return argon2.GetBytes(HashSize)
            End Using
        End Function

        Private Function ToBase64(data As Byte()) As String
            Return Convert.ToBase64String(data).TrimEnd("="c)
        End Function

        Private Function FromBase64(value As String) As Byte()
            Dim normalized = value.Replace("-"c, "+"c).Replace("_"c, "/"c)
            Select Case normalized.Length Mod 4
                Case 2
                    normalized &= "=="
                Case 3
                    normalized &= "="
            End Select
            Return Convert.FromBase64String(normalized)
        End Function

    End Module
End Namespace
