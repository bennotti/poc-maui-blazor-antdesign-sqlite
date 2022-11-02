using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocMaui.Core.Contract.Infra.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        T GetRepository<T, TEntity>()
            where TEntity : class
            where T : IRepository<TEntity>;
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
