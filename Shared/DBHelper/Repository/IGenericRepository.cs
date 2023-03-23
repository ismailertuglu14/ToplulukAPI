﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Topluluk.Shared.Dtos;

namespace DBHelper.Repository
{
    public interface IGenericRepository<T> where T : AbstractEntity
    {
        int Count();
        Task<int> Count(Expression<Func<T, bool>> predicate);
        DatabaseResponse GetById(string id);
        Task<DatabaseResponse> GetListById(List<string> ids);

        Task<DatabaseResponse> GetByIdAsync(string id);

        DatabaseResponse GetByIdWithDeleted(string id);
        Task<DatabaseResponse> GetByIdWithDeletedAsync(string id);

        T GetFirst(Expression<Func<T, bool>>? predicate = null);
        Task<T> GetFirstAsync(Expression<Func<T, bool>>? predicate = null);

        T GetFirstWithDeleted(Expression<Func<T, bool>>? predicate = null);
        Task<T> GetFirstWithDeletedAsync(Expression<Func<T, bool>>? predicate = null);

        T GetByExpression(Expression<Func<T, bool>>? predicate = null);
        Task<T> GetByExpressionAsync(Expression<Func<T, bool>>? predicate = null);

        IEnumerable<T> GetListByExpression(string searchQuery);

        DatabaseResponse GetAll();
        IEnumerable<T> GetAll(string tableName);
        DatabaseResponse GetAll(int pageSize, int pageNumber, Expression<Func<T, bool>> predicate = null);
        Task<DatabaseResponse> GetAllAsync(int pageSize, int pageNumber, Expression<Func<T, bool>> predicate = null);

        DatabaseResponse GetAllWithDeleted();
        Task<DatabaseResponse> GetAllWithDeletedAsync();

        IEnumerable<T> Page(int pageSize, int pageNumber, int count);

        dynamic GetMultipleQuery(string query);

        DatabaseResponse Insert(T entity);
        Task<DatabaseResponse> InsertAsync(T entity);

        DatabaseResponse Update(T entity);
        DatabaseResponse BulkUpdate(List<T> entityList);

        DatabaseResponse DeleteById(T entity);
        DatabaseResponse DeleteById(string id);
        void DeleteCompletely(string id);
        int Delete(T entity);
        void Delete(string id);
        int Delete(string[] id);
        bool Delete(List<T> entities);
        bool Delete(params T[] entities);

        void ExecuteScript(string script);
        T GetByLangId(int langId, int id);

        DatabaseResponse GetListByExpression(Expression<Func<T, bool>> predicate = null);
        DatabaseResponse GetListByExpressionWithDeleted(Expression<Func<T, bool>> predicate = null);

    }
}

