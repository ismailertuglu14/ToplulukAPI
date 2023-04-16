﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DBHelper.Connection;
using MongoDB.Bson;
using MongoDB.Driver;
using SharpCompress.Common;
using Topluluk.Shared.Dtos;


namespace DBHelper.Repository.Mongo
{
    public class MongoGenericRepository<T> : IGenericRepository<T> where T : AbstractEntity
    {
        private readonly IConnectionFactory _connectionFactory;

        public MongoGenericRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private IMongoDatabase GetConnection() => (MongoDB.Driver.IMongoDatabase)_connectionFactory.GetConnection;

        private string GetCollectionName() => string.Format("{0}Collection", typeof(T).Name);

        //todo: Need performence improvements
        public DatabaseResponse BulkUpdate(List<T> entityList)
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();

            foreach (T entity in entityList)
            {
                var data = database.GetCollection<T>(collectionName).ReplaceOne(x => x.Id == entity.Id, entity);
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
            var database = GetConnection();
            var collectionName = GetCollectionName();

            database.GetCollection<T>(collectionName).DeleteOne(x=>x.Id == id);
        }

        public void DeleteByExpression(Expression<Func<T, bool>> predicate)
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();

            var filter = Builders<T>.Filter.Where(predicate);
            var update = Builders<T>.Update.Set(x => x.IsDeleted, true);
            database.GetCollection<T>(collectionName).UpdateMany(filter, update);
        }

        public int Delete(string[] id)
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();

            foreach (string userId in id)
            {
                database.GetCollection<T>(collectionName).DeleteOne(x => x.Id == userId);
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
            var database = GetConnection();
            var collectionName = GetCollectionName();

            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(entity.Id));
            var update = Builders<T>.Update.Set(x => x.IsDeleted, true);
            var deleteResponse = database.GetCollection<T>(collectionName).UpdateOne(filter,update);
            DatabaseResponse response = new();
            response.IsSuccess = true;
            response.Data = deleteResponse.ToString();
            return response;
            
        }

        // Find by id and update IsDeleted to true
        public DatabaseResponse DeleteById(string id)
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();

            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<T>.Update.Set(x => x.IsDeleted, true);
            var deleteResponse = database.GetCollection<T>(collectionName).UpdateOne(filter,update);
            DatabaseResponse response = new();
            response.IsSuccess = true;
            response.Data = deleteResponse.ToString();
            return response;
        }

        public void DeleteCompletely(string id)
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();

            database.GetCollection<T>(collectionName).DeleteOne(x => x.Id == id);
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
            var database = GetConnection();
            var collectionName = GetCollectionName();

            var defaultFilter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
            var finalFilter = Builders<T>.Filter.And(defaultFilter, predicate);

            var result = await database.GetCollection<T>(collectionName).Find(finalFilter).Skip(pageNumber * pageSize).Limit(pageSize).ToListAsync();

            DatabaseResponse response = new();
            response.Data = result;
            response.IsSuccess = true;

            return response;
        }

        public DatabaseResponse GetAllWithDeleted()
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();

            var filter = Builders<T>.Filter.Empty;

            var result = database.GetCollection<T>(collectionName).Find(filter).ToList();

            DatabaseResponse response = new();
            response.Data = result;
            response.IsSuccess = true;

            return response;
        }

        public async Task<DatabaseResponse> GetAllWithDeletedAsync()
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();

            var filter = Builders<T>.Filter.Empty;

            var result = await database.GetCollection<T>(collectionName).Find(filter).ToListAsync();

            DatabaseResponse response = new();
            response.Data = result;
            response.IsSuccess = true;

            return response;
        }

        public T GetByExpression(Expression<Func<T, bool>>? predicate = null)
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();

            T entity = database.GetCollection<T>(collectionName).Find(predicate).FirstOrDefault();
            return entity;
        }

        public async Task<T> GetByExpressionAsync(Expression<Func<T, bool>>? predicate = null)
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();

            T entity = await database.GetCollection<T>(collectionName).Find(predicate).FirstOrDefaultAsync();

            return entity;
        }


        public DatabaseResponse GetById(string id)
        {
            var defaultFilter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);

            var database = GetConnection();
            var collectionName = GetCollectionName();

            var data = database.GetCollection<T>(collectionName).Find(x => x.Id == id).FirstOrDefault();

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
            var database = GetConnection();
            var collectionName = GetCollectionName();

            return database.GetCollection<T>(collectionName).Find(predicate).AnyAsync();
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

            var database = GetConnection();
            var collectionName = GetCollectionName();

            return database!.GetCollection<T>(collectionName).Find(finalFilter).FirstOrDefault();
        }

        public async Task<T> GetFirstAsync(Expression<Func<T, bool>>? predicate = null)
        {
            var defaultFilter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
            var finalFilter = Builders<T>.Filter.And(defaultFilter, predicate);

            var database = GetConnection();
            var collectionName = GetCollectionName();

            return await database!.GetCollection<T>(collectionName).Find(finalFilter).FirstOrDefaultAsync();
        }

        public T GetFirstWithDeleted(Expression<Func<T, bool>>? predicate = null)
        {
            var database = GetConnection();
            var collectionName = string.Format("{0}Collection", typeof(T).Name);
            return database.GetCollection<T>(collectionName).Find(predicate).FirstOrDefault();
        }

        public async Task<T> GetFirstWithDeletedAsync(Expression<Func<T, bool>>? predicate = null)
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();
            return await database.GetCollection<T>(collectionName).Find(predicate).FirstOrDefaultAsync();

        }

        public IEnumerable<T> GetListByExpression(string searchQuery)
        {
            throw new NotImplementedException();
        }

        public List<T> GetListByExpression(Expression<Func<T, bool>>? predicate = null)
        {
            var defaultFilter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
            var finalFilter = Builders<T>.Filter.And(defaultFilter, predicate);

            var database = GetConnection();
            var collectionName = GetCollectionName();

            var data = database.GetCollection<T>(collectionName).Find(finalFilter).ToList();

            return data;
        }

        public List<T> GetListByExpressionPaginated(int skip, int take, Expression<Func<T, bool>> predicate = null)
        {
            var defaultFilter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
            var finalFilter = Builders<T>.Filter.And(defaultFilter, predicate);

            var database = GetConnection();
            var collectionName = GetCollectionName();

            var data = database.GetCollection<T>(collectionName).Find(finalFilter).Skip(skip * take).Limit(take).ToList();

            return data;
            
        }

        public List<T>  GetListByExpressionWithDeleted(Expression<Func<T, bool>>? predicate = null)
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();

            var data = database.GetCollection<T>(collectionName).Find(predicate).ToList();
            
            return data;
        }

        public dynamic GetMultipleQuery(string query)
        {
            throw new NotImplementedException();
        }

        public DatabaseResponse Insert(T entity)
        {
            var database = GetConnection();
            var collectionName = string.Format("{0}Collection", typeof(T).Name);

            entity.CreatedAt = DateTime.Now;

            database.GetCollection<T>(collectionName).InsertOne(entity);

            var dbResponse = new DatabaseResponse();
            dbResponse.IsSuccess = true;
            dbResponse.Data = entity.Id;

            return dbResponse;
        }

        public async Task<DatabaseResponse> InsertAsync(T entity)
        {
            var database = GetConnection();
            var collectionName = string.Format("{0}Collection", typeof(T).Name);

            entity.CreatedAt = DateTime.Now;

            await database.GetCollection<T>(collectionName).InsertOneAsync(entity);

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

            var database = GetConnection();
            var collectionName = string.Format("{0}Collection", typeof(T).Name);

            var data = database.GetCollection<T>(collectionName).ReplaceOne(x => x.Id == entity.Id, entity);
            var dbResponse = new DatabaseResponse();
            dbResponse.IsSuccess = true;
            dbResponse.Data = data;

            return dbResponse;
        }

        public async Task<DatabaseResponse> GetListByIdAsync(List<string> ids)
        {
            DatabaseResponse response = new();
            var database = GetConnection();
            var collectionName = GetCollectionName();
            var filter = Builders<T>.Filter.In(x => x.Id, ids);
         
            response.Data = await database.GetCollection<T>(collectionName).Find(filter).ToListAsync();

            response.IsSuccess = true;
            return await Task.FromResult(response);
        }

        public async Task<int> Count(Expression<Func<T, bool>> predicate)
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();
            var response = await database.GetCollection<T>(collectionName).Find(predicate).ToListAsync();

            return response.Count;
        }
    }
}

