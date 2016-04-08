using ConcurrencyTest.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyTest.Repository {

    /// <summary>
    /// Defines repository pattern for CRUD operations.
    /// </summary>
    /// <typeparam name="T">Entity to work with.</typeparam>
    interface ICrudRepository<T> : IDisposable
        where T : class {

        /// <summary>
        /// Read all entries from database.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetListAsync();


        /// <summary>
        /// Read single entry from database.
        /// </summary>
        /// <returns></returns>
        Task<T> ReadAsync(int id); 

        /// <summary>
        /// Add new entry to database. Automatically generate Id increment.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task CreateAsync(T item); 

        /// <summary>
        /// Update object in database with concurrency
        /// </summary>
        /// <param name="item">item model to update</param>
        /// <returns></returns>
        /// <exception cref="ConcurrencyException"></exception>
        Task UpdateAsync(T item); 

        /// <summary>
        /// Delete entry from database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="SqlException">If record not exists</exception>
        Task DeleteAsync(int id); 
    }

    
}
