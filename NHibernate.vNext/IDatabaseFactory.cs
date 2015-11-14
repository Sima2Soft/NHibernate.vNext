namespace NHibernate.vNext
{
    public interface IDatabaseFactory
    {
        ISession Session { get; }
        void BeginRequest(bool beginTransaction = true);
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        void EndRequest(bool errors = false, bool reopen = false);
    }
}
