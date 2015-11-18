using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHibernate.vNext
{
    public interface IDatabaseRequest : IDisposable
    {
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        void Finish(bool forceRollback = false);
    }
}
