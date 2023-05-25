using System.Linq.Expressions;
using DBHelper.Connection;
using Topluluk.Shared.Dtos;

namespace DBHelper.Repository.SQL
{
	public class SqlGenericRepository<T> : IGenericRepository<T> where T : AbstractEntity
	{
        readonly IConnectionFactory _connectionFactory;

        public SqlGenericRepository( IConnectionFactory connectionFactory)
		{
            
            _connectionFactory = connectionFactory;
		}

        public DatabaseResponse BulkUpdate(List<T> entityList)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public void DeleteByExpression(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Delete(string[] id)
        {
            throw new NotImplementedException();
        }

        public bool Delete(List<T> entities)
        {
            throw new NotImplementedException();
        }

        public bool Delete(params T[] entities)
        {
            throw new NotImplementedException();
        }

        public DatabaseResponse DeleteById(T entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteCompletely(string id)
        {
            throw new NotImplementedException();
        }

        public void ExecuteScript(string script)
        {
            throw new NotImplementedException();
        }

        public DatabaseResponse GetAll()
        {
            throw new NotImplementedException();
        }

        public DatabaseResponse GetAll(int pageSize, int pageNumber, Expression<Func<T, bool>> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll(string tableName)
        {
            throw new NotImplementedException();
        }

        public DatabaseResponse GetAllWithDeleted()
        {
            throw new NotImplementedException();
        }

        public T GetByExpression(Expression<Func<T, bool>> predicate = null)
        {
            throw new NotImplementedException();
        }

        public DatabaseResponse GetById(string id)
        {
            throw new NotImplementedException();
        }

        public T GetByLangId(int langId, int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetListByExpression(string searchQuery)
        {
            throw new NotImplementedException();
        }

        public List<T>  GetListByExpression(Expression<Func<T, bool>> predicate = null)
        {
            throw new NotImplementedException();
        }

        public async Task<List<T>> GetListByExpressionAsync(Expression<Func<T, bool>> predicate = null)
        {
            throw new NotImplementedException();
        }

        public List<T> GetListByExpressionPaginated(int skip, int take, Expression<Func<T, bool>> predicate = null)
        {
            throw new NotImplementedException();
        }

        public List<T>  GetListByExpressionWithDeleted(Expression<Func<T, bool>> predicate = null)
        {
            throw new NotImplementedException();
        }

        public dynamic GetMultipleQuery(string query)
        {
            throw new NotImplementedException();
        }

        public DatabaseResponse Insert(T entity)
        {
            throw new NotImplementedException();

        }

        public async Task<DatabaseResponse> InsertAsync(T entity)
        {
            throw new NotImplementedException();


        }

        public async Task<DatabaseResponse> InsertManyAsync(List<T> entities)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Page(int pageSize, int pageNumber, int count)
        {
            throw new NotImplementedException();
        }

        public DatabaseResponse Update(T entity)
        {
            throw new NotImplementedException();
        }

        public T GetFirst(Expression<Func<T, bool>> predicate = null)
        {
            throw new NotImplementedException();

        }

        public async Task<T> GetFirstAsync(Expression<Func<T, bool>> predicate = null)
        {
            throw new NotImplementedException();
        }

        public Task<DatabaseResponse> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public DatabaseResponse GetByIdWithDeleted(string id)
        {
            throw new NotImplementedException();
        }

        public Task<DatabaseResponse> GetByIdWithDeletedAsync(string id)
        {
            throw new NotImplementedException();
        }

        public T GetFirstWithDeleted(Expression<Func<T, bool>>? predicate = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetFirstWithDeletedAsync(Expression<Func<T, bool>>? predicate = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByExpressionAsync(Expression<Func<T, bool>>? predicate = null)
        {
            throw new NotImplementedException();
        }

        public Task<DatabaseResponse> GetAllAsync(int pageSize, int pageNumber, Expression<Func<T, bool>> predicate = null)
        {
            throw new NotImplementedException();
        }

        public Task<DatabaseResponse> GetAllWithDeletedAsync()
        {
            throw new NotImplementedException();
        }

        public DatabaseResponse DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<DatabaseResponse> GetListByIdAsync(List<string> ids)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Count(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

      
    }
}

