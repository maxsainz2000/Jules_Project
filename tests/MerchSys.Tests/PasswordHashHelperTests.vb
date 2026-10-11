Imports Xunit
Imports MerchSys.App.Services

Public Class PasswordHashHelperTests

    <Fact>
    Public Sub HashPassword_ProducesSelfDescribingPhcString()
        Dim hash = PasswordHashHelper.HashPassword("Vista2026!")
        Assert.StartsWith("$argon2id$v=19$m=19456,t=2,p=1$", hash)
    End Sub

    <Fact>
    Public Sub VerifyPassword_AcceptsCorrectPassword()
        Dim hash = PasswordHashHelper.HashPassword("Vista2026!")
        Assert.True(PasswordHashHelper.VerifyPassword("Vista2026!", hash))
    End Sub

    <Fact>
    Public Sub VerifyPassword_RejectsWrongPassword()
        Dim hash = PasswordHashHelper.HashPassword("Vista2026!")
        Assert.False(PasswordHashHelper.VerifyPassword("vista2026!", hash))
    End Sub

    <Fact>
    Public Sub HashPassword_UsesADistinctSaltPerHash()
        Dim first = PasswordHashHelper.HashPassword("same-password")
        Dim second = PasswordHashHelper.HashPassword("same-password")

        Assert.NotEqual(first, second)
        Assert.True(PasswordHashHelper.VerifyPassword("same-password", first))
        Assert.True(PasswordHashHelper.VerifyPassword("same-password", second))
    End Sub

    <Fact>
    Public Sub VerifyPassword_RejectsMalformedInput()
        Assert.False(PasswordHashHelper.VerifyPassword("x", Nothing))
        Assert.False(PasswordHashHelper.VerifyPassword("x", ""))
        Assert.False(PasswordHashHelper.VerifyPassword("x", "not-a-hash"))
        Assert.False(PasswordHashHelper.VerifyPassword("x", "$argon2id$v=19$m=19456,t=2,p=1$bogus$bogus"))
    End Sub
End Class
