Imports System.Collections.Generic
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore.Query

Namespace Fakes
    Friend Class TestAsyncQueryProvider(Of TEntity)
        Implements IAsyncQueryProvider

        Private ReadOnly _inner As IQueryProvider

        Public Sub New(inner As IQueryProvider)
            _inner = inner
        End Sub

        Public Function CreateQuery(expression As Expression) As IQueryable Implements IQueryProvider.CreateQuery
            Return New TestAsyncEnumerable(Of TEntity)(expression)
        End Function

        Public Function CreateQuery(Of TElement)(expression As Expression) As IQueryable(Of TElement) Implements IQueryProvider.CreateQuery
            Return New TestAsyncEnumerable(Of TElement)(expression)
        End Function

        Public Function Execute(expression As Expression) As Object Implements IQueryProvider.Execute
            Return _inner.Execute(expression)
        End Function

        Public Function Execute(Of TResult)(expression As Expression) As TResult Implements IQueryProvider.Execute
            Return _inner.Execute(Of TResult)(expression)
        End Function

        Public Function ExecuteAsync(Of TResult)(expression As Expression, Optional cancellationToken As CancellationToken = Nothing) As TResult Implements IAsyncQueryProvider.ExecuteAsync
            Dim expectedResultType = GetType(TResult).GetGenericArguments()(0)
            Dim executionResult = GetType(IQueryProvider).
                GetMethod(NameOf(IQueryProvider.Execute), 1, {GetType(Expression)}).
                MakeGenericMethod(expectedResultType).
                Invoke(Me, New Object() {expression})

            Return CType(GetType(Task).
                GetMethod(NameOf(Task.FromResult)).
                MakeGenericMethod(expectedResultType).
                Invoke(Nothing, New Object() {executionResult}), TResult)
        End Function
    End Class

    Friend Class TestAsyncEnumerable(Of T)
        Implements IAsyncEnumerable(Of T), IQueryable(Of T)

        Private ReadOnly _enumerable As IEnumerable(Of T)
        Private ReadOnly _queryable As IQueryable(Of T)

        Public Sub New(enumerable As IEnumerable(Of T))
            _enumerable = enumerable
            _queryable = enumerable.AsQueryable()
        End Sub

        Public Sub New(expression As Expression)
            _queryable = (New T(-1) {}).AsQueryable().Provider.CreateQuery(Of T)(expression)
        End Sub

        Public Function GetAsyncEnumerator(Optional cancellationToken As CancellationToken = Nothing) As IAsyncEnumerator(Of T) Implements IAsyncEnumerable(Of T).GetAsyncEnumerator
            Return New TestAsyncEnumerator(Of T)(_queryable.GetEnumerator())
        End Function

        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Return _queryable.GetEnumerator()
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _queryable.GetEnumerator()
        End Function

        Public ReadOnly Property ElementType As Type Implements IQueryable.ElementType
            Get
                Return _queryable.ElementType
            End Get
        End Property

        Public ReadOnly Property Expression As Expression Implements IQueryable.Expression
            Get
                Return _queryable.Expression
            End Get
        End Property

        Private ReadOnly Property IQueryable_Provider As IQueryProvider Implements IQueryable.Provider
            Get
                Return New TestAsyncQueryProvider(Of T)(_queryable.Provider)
            End Get
        End Property
    End Class

    Friend Class TestAsyncEnumerator(Of T)
        Implements IAsyncEnumerator(Of T)

        Private ReadOnly _inner As IEnumerator(Of T)

        Public Sub New(inner As IEnumerator(Of T))
            _inner = inner
        End Sub

        Public Function DisposeAsync() As ValueTask Implements IAsyncDisposable.DisposeAsync
            _inner.Dispose()
            Return New ValueTask()
        End Function

        Public Function MoveNextAsync() As ValueTask(Of Boolean) Implements IAsyncEnumerator(Of T).MoveNextAsync
            Return New ValueTask(Of Boolean)(_inner.MoveNext())
        End Function

        Public ReadOnly Property Current As T Implements IAsyncEnumerator(Of T).Current
            Get
                Return _inner.Current
            End Get
        End Property
    End Class
End Namespace
