using System.Linq.Expressions;
using DBHelper.Connection;
using MongoDB.Bson;
using MongoDB.Driver;
using Topluluk.Shared.Dtos;


namespace DBHelper.Repository.Mongo
{
    public class MongoGenericRepository<T> : IGenericRepository<T> where T : AbstractEntity
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IMongoCollection<T> _collection;


        public MongoGenericRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _collection = GetCollection();
        }

        private IMongoDatabase GetConnection()
        {
            return (MongoDB.Driver.IMongoDatabase)_connectionFactory.GetConnection;
        }

        private IMongoCollection<T> GetCollection()
        {
            var database = GetConnection();
            var collectionName = typeof(T).Name + "Collection";
            return database.GetCollection<T>(collectionName);
        }
        
        //todo: Need performence improvements
        public DatabaseResponse BulkUpdate(List<T> entityList)
        {


            foreach (T entity in entityList)
            {
                var data = _collection.ReplaceOne(x => x.Id == entity.Id, entity);
            }
            var dbResponse = new DatabaseResponse();
            dbResponse.IsSuccess = true;
            dbResponse.Data = "Datas updated successfully";

            return dbResponse;
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

            _collection.DeleteOne(x=>x.Id == id);
        }

        public void DeleteByExpression(Expression<Func<T, bool>> predicate)
        {

            var filter = Builders<T>.Filter.Where(predicate);
            var update = Builders<T>.Update.Set(x => x.IsDeleted, true);
            _collection.UpdateMany(filter, update);
        }

        public int Delete(string[] id)
        {
          
            foreach (string userId in id)
            {
                _collection.DeleteOne(x => x.Id == userId);
            }
            return 1;
        }

        public bool Delete(List<T> entities)
        {
            throw new NotImplementedException();
        }

        public bool Delete(params T[] entities)
        {
            throw new NotImplementedException();
        }

        // Find by entity's id and update with new entity
        public DatabaseResponse DeleteById(T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(entity.Id));
            var update = Builders<T>.Update.Set(x => x.IsDeleted, true);
            var deleteResponse = _collection.UpdateOne(filter,update);
            DatabaseResponse response = new();
            response.IsSuccess = true;
            response.Data = deleteResponse.ToString();
            return response;
            
        }

        // Find by id and update IsDeleted to true
        public DatabaseResponse DeleteById(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<T>.Update.Set(x => x.IsDeleted, true);
            var deleteResponse = _collection.UpdateOne(filter,update);
            DatabaseResponse response = new();
            response.IsSuccess = true;
            response.Data = deleteResponse.ToString();
            return response;
        }

        public void DeleteCompletely(string id)
        {
            _collection.DeleteOne(x => x.Id == id);
        }

        public void ExecuteScript(string script)
        {
            throw new NotImplementedException();
        }

        public DatabaseResponse GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll(string tableName)
        {
            throw new NotImplementedException();
        }

        public DatabaseResponse GetAll(int pageSize, int pageNumber, Expression<Func<T, bool>> predicate = null)
        {
            throw new NotImplementedException();
        }

        public async Task<DatabaseResponse> GetAllAsync(int pageSize, int pageNumber, Expression<Func<T, bool>> predicate = null)
        {
            var defaultFilter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
            var finalFilter = Builders<T>.Filter.And(defaultFilter, predicate);

            var result = await _collection.Find(finalFilter).Skip(pageNumber * pageSize).Limit(pageSize).ToListAsync();

            DatabaseResponse response = new();
            response.Data = result;
            response.IsSuccess = true;

            return response;
        }

        public DatabaseResponse GetAllWithDeleted()
        {
            var filter = Builders<T>.Filter.Empty;

            var result = _collection.Find(filter).ToList();

            DatabaseResponse response = new();
            response.Data = result;
            response.IsSuccess = true;

            return response;
        }

        public async Task<DatabaseResponse> GetAllWithDeletedAsync()
        {
            var filter = Builders<T>.Filter.Empty;

            var result = await _collection.Find(filter).ToListAsync();

            DatabaseResponse response = new();
            response.Data = result;
            response.IsSuccess = true;

            return response;
        }

        public T GetByExpression(Expression<Func<T, bool>>? predicate = null)
        {
            T entity = _collection.Find(predicate).FirstOrDefault();
            return entity;
        }

        public async Task<T> GetByExpressionAsync(Expression<Func<T, bool>>? predicate = null)
        {
            T entity = await _collection.Find(predicate).FirstOrDefaultAsync();

            return entity;
        }

        

        public DatabaseResponse GetById(string id)
        {
            var defaultFilter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);

            var data = _collection.Find(x => x.Id == id).FirstOrDefault();

            DatabaseResponse response = new();
            response.Data = data;
            response.IsSuccess = true;

            return response;
        }

        public  Task<DatabaseResponse> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
       
            return _collection.Find(predicate).AnyAsync();
        }

        public DatabaseResponse GetByIdWithDeleted(string id)
        {
            throw new NotImplementedException();
        }

        public Task<DatabaseResponse> GetByIdWithDeletedAsync(string id)
        {
            throw new NotImplementedException();
        }

        public T GetByLangId(int langId, int id)
        {
            throw new NotImplementedException();
        }

        public T GetFirst(Expression<Func<T, bool>>? predicate = null)
        {
            var defaultFilter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
            var finalFilter = Builders<T>.Filter.And(defaultFilter, predicate);
            
            return _collection.Find(finalFilter).FirstOrDefault();
        }

        public async Task<T> GetFirstAsync(Expression<Func<T, bool>>? predicate = null)
        {
            var defaultFilter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
            var finalFilter = Builders<T>.Filter.And(defaultFilter, predicate);
            
            return await _collection.Find(finalFilter).FirstOrDefaultAsync();
        }

        public T GetFirstWithDeleted(Expression<Func<T, bool>>? predicate = null)
        {
            return _collection.Find(predicate).FirstOrDefault();
        }

        public async Task<T> GetFirstWithDeletedAsync(Expression<Func<T, bool>>? predicate = null)
        {
          
            return await _collection.Find(predicate).FirstOrDefaultAsync();

        }

        public IEnumerable<T> GetListByExpression(string searchQuery)
        {
            throw new NotImplementedException();
        }

        public List<T> GetListByExpression(Expression<Func<T, bool>>? predicate = null)
        {
            var defaultFilter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
            var finalFilter = Builders<T>.Filter.And(defaultFilter, predicate);

            var data = _collection.Find(finalFilter).ToList();

            return data;
        }

        public async Task<List<T>> GetListByExpressionAsync(Expression<Func<T, bool>> predicate = null)
        {
            var defaultFilter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
            var finalFilter = Builders<T>.Filter.And(defaultFilter, predicate);

            var data = _collection.Find(finalFilter).ToList();

            return await Task.FromResult(data);
        }

        public List<T> GetListByExpressionPaginated(int skip, int take, Expression<Func<T, bool>> predicate = null)
        {
            var defaultFilter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
            var finalFilter = Builders<T>.Filter.And(defaultFilter, predicate);
            

            var data = _collection.Find(finalFilter).Skip(skip * take).Limit(take).ToList();

            return data;
            
        }

        public List<T>  GetListByExpressionWithDeleted(Expression<Func<T, bool>>? predicate = null)
        {
            var data = _collection.Find(predicate).ToList();
            return data;
        }

        public dynamic GetMultipleQuery(string query)
        {
            throw new NotImplementedException();
        }

        public DatabaseResponse Insert(T entity)
        {
            entity.CreatedAt = DateTime.Now;

            _collection.InsertOne(entity);

            var dbResponse = new DatabaseResponse();
            dbResponse.IsSuccess = true;
            dbResponse.Data = entity.Id;

            return dbResponse;
        }

        public async Task<DatabaseResponse> InsertAsync(T entity)
        {
            entity.CreatedAt = DateTime.Now;

            await _collection.InsertOneAsync(entity);

            var dbResponse = new DatabaseResponse();
            dbResponse.IsSuccess = true;
            dbResponse.Data = entity.Id;

            return await Task.FromResult(dbResponse);
        }

        public IEnumerable<T> Page(int pageSize, int pageNumber, int count)
        {
            throw new NotImplementedException();
        }

        public DatabaseResponse Update(T entity)
        {
            entity.UpdatedAt = DateTime.Now;
            var data = _collection.ReplaceOne(x => x.Id == entity.Id, entity);
            var dbResponse = new DatabaseResponse();
            dbResponse.IsSuccess = true;
            dbResponse.Data = data;

            return dbResponse;
        }

        public async Task<DatabaseResponse> GetListByIdAsync(List<string> ids)
        {
            DatabaseResponse response = new();
         
            var filter = Builders<T>.Filter.In(x => x.Id, ids);
         
            response.Data = await _collection.Find(filter).ToListAsync();

            response.IsSuccess = true;
            return await Task.FromResult(response);
        }

        public async Task<int> Count(Expression<Func<T, bool>> predicate)
        {
            var response = await _collection.Find(predicate).ToListAsync();
            return response.Count;
        }

    }
}

