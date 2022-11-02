using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocMaui.Core.Contract.Infra.Repositories
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task<TEntity> GetByIdAsync(object obj);
        Task<TEntity> GetByFirstAsync(string condition, object obj);
        Task<IEnumerable<TEntity>> GetAllAsync(string orderBy = "");
        Task<IEnumerable<TEntity>> GetAllAsync(string condition, object obj, string orderBy = "");
        Task<IEnumerable<TEntity>> GetAllByIdAsync(object obj, string orderBy = "");
        Task<int> GetCountAsync();
        Task<int> GetCountAsync(string condition, object obj);
        Task<(IEnumerable<TEntity> Records, int TotalRecords)> GetAllPagingAsync(int page = 1, int perPage = 10, string orderBy = "");
        Task<(IEnumerable<TEntity> Records, int TotalRecords)> GetAllPagingAsync(string condition, object obj, int page = 1, int perPage = 10, string orderBy = "");
        Task<IEnumerable<int>> GetAllIdsPagingAsync(int page = 1, int perPage = 10, string orderBy = "");
        Task<IEnumerable<int>> GetAllIdsPagingAsync(string condition, object obj, int page = 1, int perPage = 10, string orderBy = "");

        Task<bool> IsValidateAsync(string condition, object obj);

        Task<TEntity> AddGetEntityAsync(TEntity entity);
        Task<bool> AddAsync(TEntity entity);
        IAsyncEnumerable<TEntity> AddRangeGetEntitiesAsync(IEnumerable<TEntity> entities);
        Task<bool> AddRangeAsync(IEnumerable<TEntity> entities);

        Task<bool> RemoveAsync(object obj);
        Task<bool> RemoveAsync(TEntity entity);
        Task<bool> RemoveRangeAsync(IEnumerable<TEntity> entities);

        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> UpdateRangeAsync(IEnumerable<TEntity> entities);
    }
}
