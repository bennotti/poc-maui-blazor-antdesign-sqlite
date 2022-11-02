using PocMaui.Core.Contract.Infra.Data;
using PocMaui.Core.Contract.Infra.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace PocMaui.Infra.Repositories
{
    public abstract class AbstractSqliteRepository<TEntity> : IUnitOfWork, IRepository<TEntity> where TEntity : class
    {
        protected readonly IDbConnection dbConn;
        protected IDbTransaction dbTransaction;
        protected readonly IServiceProvider _serviceProvider;

        protected abstract string IdColumnName { get; }
        protected abstract string DeletedColumnName { get; }
        protected abstract string ColumnsSelect { get; }
        protected abstract string ColumnsInsert { get; }
        protected abstract string ColumnsInsertValues { get; }
        protected abstract string ColumnsUpdate { get; }
        protected abstract string WhereCondition { get; }

        private readonly string? _tableName = typeof(TEntity).GetCustomAttribute<TableAttribute>()?.Name;
        public string TableName
        {
            get
            {
                if (string.IsNullOrEmpty(_tableName)) return typeof(TEntity).Name;
                return _tableName;
            }
        }
        private string SelectCountByFilterQuery { get { return $"SELECT COUNT({{0}}) FROM {TableName} WHERE {DeletedColumnName} = 0 AND ({{1}})"; } }
        private string SelectCountQuery { get { return $"SELECT COUNT({{0}}) FROM {TableName} WHERE {DeletedColumnName} = 0"; } }
        private string SelectByIdQuery
        {
            get
            {
                return $"SELECT {{0}} FROM {TableName} WHERE {DeletedColumnName} = 0 AND ({{1}}) {{2}}";
            }
        }
        private string SelectAllQuery
        {
            get
            {
                return $"SELECT {{0}} FROM {TableName} WHERE {DeletedColumnName} = 0 {{1}} ";
            }
        }
        private string SelectAllPagingWithFilterQuery
        {
            get
            {
                return $"SELECT {{0}} FROM {TableName} WHERE {DeletedColumnName} = 0 AND ({{1}}) {{2}} LIMIT {{3}} OFFSET {{4}}";
            }
        }
        private string SelectAllPagingQuery
        {
            get
            {
                return $"SELECT {{0}} FROM {TableName} WHERE {DeletedColumnName} = 0 {{1}} LIMIT {{2}} OFFSET {{3}}";
            }
        }
        private string SelectValidateQuery
        {
            get
            {
                return $"SELECT {{0}} FROM {TableName} WHERE {DeletedColumnName} = 0 AND ({{1}}) {{2}}";
            }
        }
        private string InsertQuery { get { return $"INSERT INTO {TableName} ({{0}}) VALUES ({{1}})"; } }
        private string InsertQueryReturnInserted
        {
            get
            {
                return @$"
                    START TRANSACTION;
                        INSERT INTO {TableName} ({{0}}) VALUES ({{1}});
                        SELECT {{2}} from {TableName} where {IdColumnName} = LAST_INSERT_ID();
                    COMMIT;
                ";
            }
        }
        private string UpdateByIdQuery { get { return $"UPDATE {TableName} SET {{0}} WHERE {{1}}"; } }
        private string DeleteByIdQuery { get { return $"DELETE FROM {TableName} WHERE {{0}}"; } }

        protected AbstractSqliteRepository(IDatabaseFactory databaseOptions, IServiceProvider serviceProvider)
        {
            dbConn = databaseOptions.GetDbConnection;
            _serviceProvider = serviceProvider;
        }

        public void DisposeTransaction()
        {
            dbTransaction?.Dispose();
        }

        public void Dispose()
        {
            DisposeTransaction();
            dbConn.Close();
            dbConn.Dispose();
            GC.SuppressFinalize(this);
        }

        public T GetRepository<T, TEntity1>()
            where T : IRepository<TEntity1>
            where TEntity1 : class
        {
            return (T)GetService(typeof(T));
        }

        protected object GetService(Type type)
        {
            var response = _serviceProvider?.GetService(type);
            if (response == null)
            {
                throw new ArgumentNullException("Invalid service.");
            }
            return response;
        }

        public void BeginTransaction()
        {
            dbTransaction = dbConn.BeginTransaction();
        }

        public void Commit()
        {
            dbTransaction.Commit();
            DisposeTransaction();
        }

        public void Rollback()
        {
            dbTransaction.Rollback();
            DisposeTransaction();
        }

        #region Get
        public virtual async Task<TEntity> GetByIdAsync(object obj)
        {
            string query = string.Format(SelectByIdQuery, ColumnsSelect, WhereCondition, "");

            TEntity entity = await dbConn.QueryFirstOrDefaultAsync<TEntity>(query, obj, transaction: dbTransaction);

            return entity;
        }

        public virtual async Task<TEntity> GetByFirstAsync(string condition, object obj)
        {
            string query = string.Format(SelectValidateQuery, ColumnsSelect, condition);

            TEntity entity = await dbConn.QueryFirstOrDefaultAsync<TEntity>(query, obj, transaction: dbTransaction);

            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(string condition, object obj, string orderBy = "")
        {
            string query = string.Format(
                SelectValidateQuery,
                ColumnsSelect,
                condition,
                orderBy
            );

            IEnumerable<TEntity> entity = await dbConn.QueryAsync<TEntity>(query, obj, transaction: dbTransaction);

            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(string orderBy = "")
        {
            string query = string.Format(
                SelectAllQuery,
                ColumnsSelect,
                orderBy
            );

            return await dbConn.QueryAsync<TEntity>(query, transaction: dbTransaction);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllByIdAsync(object obj, string orderBy = "")
        {
            string query = string.Format(
                SelectByIdQuery,
                ColumnsSelect,
                WhereCondition,
                orderBy
            );

            return await dbConn.QueryAsync<TEntity>(query, obj, transaction: dbTransaction);
        }

        public virtual async Task<int> GetCountAsync(string condition, object obj)
        {
            string query = string.Format(SelectCountByFilterQuery, IdColumnName, condition);

            return await dbConn.QueryFirstAsync<int>(query, obj, transaction: dbTransaction);
        }

        public virtual async Task<int> GetCountAsync()
        {
            string query = string.Format(SelectCountQuery, IdColumnName);

            return await dbConn.QueryFirstAsync<int>(query, transaction: dbTransaction);
        }

        public virtual async Task<(IEnumerable<TEntity> Records, int TotalRecords)> GetAllPagingAsync(
            string condition,
            object obj,
            int page = 1,
            int perPage = 10,
            string orderBy = ""
        )
        {
            int offset = (page - 1) * perPage;
            string query = string.Format(
                SelectAllPagingWithFilterQuery,
                ColumnsSelect,
                condition,
                orderBy,
                perPage,
                offset
            );
            var totalRecords = await GetCountAsync(condition, obj);
            var records = await dbConn.QueryAsync<TEntity>(query, obj, transaction: dbTransaction);
            return (
                records,
                totalRecords
            );
        }

        public virtual async Task<(IEnumerable<TEntity> Records, int TotalRecords)> GetAllPagingAsync(
            int page = 1,
            int perPage = 10,
            string orderBy = ""
        )
        {
            int offset = (page - 1) * perPage;
            string query = string.Format(
                SelectAllPagingQuery,
                ColumnsSelect,
                orderBy,
                perPage,
                offset
            );

            var totalRecords = await GetCountAsync();
            var records = await dbConn.QueryAsync<TEntity>(query, transaction: dbTransaction);

            return (
                records,
                totalRecords
            );
        }

        public virtual async Task<IEnumerable<int>> GetAllIdsPagingAsync(
            int page = 1,
            int perPage = 10,
            string orderBy = ""
        )
        {
            int offset = (page - 1) * perPage;
            string query = string.Format(
                SelectAllPagingQuery,
                IdColumnName,
                orderBy,
                perPage,
                offset
            );

            return await dbConn.QueryAsync<int>(query, transaction: dbTransaction);
        }

        public virtual async Task<IEnumerable<int>> GetAllIdsPagingAsync(
            string condition,
            object obj,
            int page = 1,
            int perPage = 10,
            string orderBy = ""
        )
        {
            int offset = (page - 1) * perPage;
            string query = string.Format(
                SelectAllPagingWithFilterQuery,
                IdColumnName,
                condition,
                orderBy,
                perPage,
                offset
            );

            return await dbConn.QueryAsync<int>(query, obj, transaction: dbTransaction);
        }

        #endregion

        #region Validate or Where

        public virtual async Task<bool> IsValidateAsync(string condition, object obj)
        {
            string query = string.Format(SelectValidateQuery, ColumnsSelect, condition);

            TEntity entity = await dbConn.QueryFirstOrDefaultAsync<TEntity>(query, obj, transaction: dbTransaction);

            return entity != null;
        }

        #endregion

        #region Insert
        public virtual async Task<TEntity> AddGetEntityAsync(TEntity entity)
        {
            string query = string.Format(InsertQueryReturnInserted, ColumnsInsert, ColumnsInsertValues, ColumnsSelect);

            TEntity result = await dbConn.QueryFirstOrDefaultAsync<TEntity>(query, entity, transaction: dbTransaction);

            return result;
        }

        public virtual async Task<bool> AddAsync(TEntity entity)
        {
            string query = string.Format(InsertQuery, ColumnsInsert, ColumnsInsertValues);

            return await dbConn.ExecuteAsync(query, entity, transaction: dbTransaction) > 0;
        }

        public virtual async IAsyncEnumerable<TEntity> AddRangeGetEntitiesAsync(IEnumerable<TEntity> entities)
        {
            string query = string.Format(InsertQueryReturnInserted, ColumnsInsert, ColumnsInsertValues, ColumnsSelect);

            foreach (var entity in entities)
            {
                yield return await dbConn.QueryFirstOrDefaultAsync<TEntity>(query, entity, transaction: dbTransaction);
            }
        }


        public virtual async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            string query = string.Format(InsertQuery, ColumnsInsert, ColumnsInsertValues);

            return await dbConn.ExecuteAsync(query, entities, transaction: dbTransaction) > 0;
        }

        #endregion

        #region Delete

        public virtual async Task<bool> RemoveAsync(object obj)
        {
            string query = string.Format(DeleteByIdQuery, WhereCondition);

            return await dbConn.ExecuteAsync(query, obj, transaction: dbTransaction) > 0;
        }


        public virtual async Task<bool> RemoveAsync(TEntity entity)
        {
            string query = string.Format(DeleteByIdQuery, WhereCondition);

            return await dbConn.ExecuteAsync(query, entity, transaction: dbTransaction) > 0;
        }

        public virtual async Task<bool> RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            string query = string.Format(DeleteByIdQuery, WhereCondition);

            return await dbConn.ExecuteAsync(DeleteByIdQuery, entities.Select(obj => obj), transaction: dbTransaction) > 0;

        }
        #endregion

        #region Update
        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            string query = string.Format(UpdateByIdQuery, ColumnsUpdate, WhereCondition);


            return await dbConn.ExecuteAsync(query, entity, transaction: dbTransaction) > 0;
        }

        public virtual async Task<bool> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            string query = string.Format(UpdateByIdQuery, ColumnsUpdate, WhereCondition);

            return await dbConn.ExecuteAsync(query, entities.Select(obj => obj), transaction: dbTransaction) > 0;
        }
        #endregion
    }
}
