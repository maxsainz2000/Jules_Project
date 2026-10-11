Namespace Interfaces
    ''' <summary>
    ''' Marks a large transactional entity whose history is read with server-side (keyset)
    ''' pagination, so it needs a secondary index on <c>(CreatedAt, Id)</c>.
    '''
    ''' Previously the index was applied to every <c>IAuditable</c> entity, including tiny
    ''' lookup/config tables, which wasted space and could duplicate indexes already declared
    ''' in entity configurations (M12).
    ''' </summary>
    Public Interface IPaginatedEntity
    End Interface
End Namespace
