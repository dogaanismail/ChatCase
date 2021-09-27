using ChatCase.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ChatCase.Repository.Generic
{
    /// <summary>
    /// Generic repository interface implementations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public partial interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Gets a collection with queryable
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<T> Get(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// Gets a collection with predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Get collection list with predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// Gets a collection by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync(string id);

        /// <summary>
        /// Adds an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Adds entites with bulk
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<bool> AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Updates an entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> UpdateAsync(string id, T entity);

        /// <summary>
        /// Updates an entity with predicate
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T> UpdateAsync(T entity, Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Deletes an entity 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> DeleteAsync(T entity);

        /// <summary>
        /// Deletes an entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> DeleteAsync(string id);

        /// <summary>
        /// Deletes an entity by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<T> DeleteAsync(Expression<Func<T, bool>> filter);
    }
}
