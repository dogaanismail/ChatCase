using ChatCase.Core.Configuration.Configs;
using ChatCase.Core.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ChatCase.Repository.Generic
{
    /// <summary>
    /// Generic repository base implementations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RepositoryBase<T> : IRepository<T> where T : BaseEntity
    {
        #region Fields
        protected readonly IMongoCollection<T> Collection;
        private readonly AppConfig _appConfig;

        #endregion

        #region Ctor

        public RepositoryBase(AppConfig appConfigs)
        {
            _appConfig = appConfigs;
            var client = new MongoClient(_appConfig.MongoDbConfig.ConnectionString);
            var db = client.GetDatabase(_appConfig.MongoDbConfig.Database);
            this.Collection = db.GetCollection<T>(typeof(T).Name.ToLowerInvariant());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a collection with queryable
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IQueryable<T> Get(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null
                ? Collection.AsQueryable()
                : Collection.AsQueryable().Where(predicate);
        }

        /// <summary>
        /// Gets a collection with predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await Collection.Find(predicate).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get collection list with predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
                return await Collection.AsQueryable().ToListAsync();
            else
                return await Collection.Find(predicate).ToListAsync();
        }

        /// <summary>
        /// Gets a collection by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> GetByIdAsync(string id)
        {
            return await Collection.Find(Builders<T>.Filter.Eq("_id", ObjectId.Parse(id))).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Adds an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<T> AddAsync(T entity)
        {
            var options = new InsertOneOptions { BypassDocumentValidation = false };
            await Collection.InsertOneAsync(entity, options);
            return entity;
        }

        /// <summary>
        /// Adds entites with bulk
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual async Task<bool> AddRangeAsync(IEnumerable<T> entities)
        {
            var options = new BulkWriteOptions { IsOrdered = false, BypassDocumentValidation = false };
            return (await Collection.BulkWriteAsync((IEnumerable<WriteModel<T>>)entities, options)).IsAcknowledged;
        }

        /// <summary>
        /// Updates an entity with predicate
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<T> UpdateAsync(string id, T entity)
        {
            return await Collection.FindOneAndReplaceAsync(x => x.Id == id, entity);
        }

        /// <summary>
        /// Updates an entity with predicate
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<T> UpdateAsync(T entity, Expression<Func<T, bool>> predicate)
        {
            return await Collection.FindOneAndReplaceAsync(predicate, entity);
        }

        /// <summary>
        /// Deletes an entity 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<T> DeleteAsync(T entity)
        {
            return await Collection.FindOneAndDeleteAsync(x => x.Id == entity.Id);
        }

        /// <summary>
        /// Deletes an entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> DeleteAsync(string id)
        {
            return await Collection.FindOneAndDeleteAsync(x => x.Id == id);
        }

        /// <summary>
        /// Deletes an entity by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual async Task<T> DeleteAsync(Expression<Func<T, bool>> filter)
        {
            return await Collection.FindOneAndDeleteAsync(filter);
        }

        #endregion
    }
}
