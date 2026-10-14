Namespace Interfaces

    ''' <summary>
    ''' Opt-in marker for the records whose changes must appear in the audit log.
    '''
    ''' Deliberately not <see cref="IAuditable"/>: that interface is on every table and only stamps row
    ''' metadata. Auditing all of it would bury the log in reference and derived writes — categories,
    ''' price snapshots, reorder suggestions — whereas AGENTS.md asks for "all financial state
    ''' changes", which is a subset. Membership is an explicit decision per entity.
    ''' </summary>
    Public Interface IAuditedEntity
    End Interface

End Namespace
