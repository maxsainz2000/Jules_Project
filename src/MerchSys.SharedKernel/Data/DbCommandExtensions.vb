Imports System
Imports System.Data.Common
Imports System.Runtime.CompilerServices

Namespace Data

    ''' <summary>
    ''' Provider-agnostic parameter helpers for raw-ADO commands built on an EF Core connection.
    '''
    ''' The project's EF provider is Oracle's <c>MySql.EntityFrameworkCore</c>, which is built on
    ''' <c>MySql.Data</c> — while the rest of the codebase also references <c>MySqlConnector</c>.
    ''' They are unrelated types, so a command obtained from <c>DbContext.Database.GetDbConnection()</c>
    ''' must be driven through the <see cref="DbConnection"/> / <see cref="DbCommand"/> base classes.
    ''' Casting it to <c>MySqlConnector.MySqlConnection</c> throws <c>InvalidCastException</c>, and
    ''' adding a <c>MySqlConnector.MySqlParameter</c> to a <c>MySql.Data</c> command is just as wrong.
    '''
    ''' <see cref="DbParameterCollection"/> has no <c>AddWithValue</c> — that convenience method
    ''' exists only on the concrete provider collections — so parameters are created through
    ''' <see cref="DbCommand.CreateParameter"/>, which yields the correct type for whichever
    ''' provider is configured.
    ''' </summary>
    Public Module DbCommandExtensions

        ''' <summary>
        ''' Adds a named parameter, letting the provider create the parameter object.
        ''' A null value (including a null <c>Nullable(Of T)</c>) becomes <see cref="DBNull.Value"/>.
        ''' </summary>
        <Extension()>
        Public Sub AddParameter(command As DbCommand, name As String, value As Object)
            Dim parameter = command.CreateParameter()
            parameter.ParameterName = name
            parameter.Value = If(value, CObj(DBNull.Value))
            command.Parameters.Add(parameter)
        End Sub

    End Module

End Namespace
