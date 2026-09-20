Imports System.Security.Cryptography
Imports System.Text
Imports Konscious.Security.Cryptography

Namespace Services
    Friend Module PasswordHashHelper

        Private Const SaltSize As Integer = 16
        Private Const DegreeOfParallelism As Integer = 1
        Private Const Iterations As Integer = 2
        Private Const MemorySize As Integer = 19456
        Private Const HashSize As Integer = 32

        Public Function HashPassword(password As String) As String
            Dim salt As Byte() = New Byte(SaltSize - 1) {}
            RandomNumberGenerator.Fill(salt)

            Using argon2 = New Argon2id(Encoding.UTF8.GetBytes(password))
                argon2.Salt = salt
                argon2.DegreeOfParallelism = DegreeOfParallelism
                argon2.Iterations = Iterations
                argon2.MemorySize = MemorySize

                Dim hash As Byte() = argon2.GetBytes(HashSize)

                Dim hashBytes As Byte() = New Byte(SaltSize + HashSize - 1) {}
                Array.Copy(salt, 0, hashBytes, 0, SaltSize)
                Array.Copy(hash, 0, hashBytes, SaltSize, HashSize)

                Return Convert.ToBase64String(hashBytes)
            End Using
        End Function

        Public Function VerifyPassword(password As String, hashString As String) As Boolean
            If String.IsNullOrEmpty(hashString) Then Return False

            Dim hashBytes As Byte() = Nothing
            Try
                hashBytes = Convert.FromBase64String(hashString)
            Catch ex As FormatException
                Return False
            End Try

            If hashBytes Is Nothing OrElse hashBytes.Length <> SaltSize + HashSize Then
                Return False
            End If

            Dim salt As Byte() = New Byte(SaltSize - 1) {}
            Array.Copy(hashBytes, 0, salt, 0, SaltSize)

            Dim expectedHash As Byte() = New Byte(HashSize - 1) {}
            Array.Copy(hashBytes, SaltSize, expectedHash, 0, HashSize)

            Using argon2 = New Argon2id(Encoding.UTF8.GetBytes(password))
                argon2.Salt = salt
                argon2.DegreeOfParallelism = DegreeOfParallelism
                argon2.Iterations = Iterations
                argon2.MemorySize = MemorySize

                Dim actualHash As Byte() = argon2.GetBytes(HashSize)
                Return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash)
            End Using
        End Function

    End Module
End Namespace
