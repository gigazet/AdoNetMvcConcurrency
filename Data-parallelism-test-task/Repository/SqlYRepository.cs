using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ConcurrencyTest.Models;
using System.Configuration;
using System.Threading.Tasks;

namespace ConcurrencyTest.Repository {

    /// <summary>
    /// Repository pattern layer to access entities database
    /// </summary>
    public class SqlYRepository : ICrudRepository<EntityY> {

        readonly SqlConnection _cn;

        public SqlYRepository() {
            _cn = new SqlConnection(ConfigurationManager.ConnectionStrings[1].ToString());
            _cn.Open();
        }
        public SqlYRepository(string connectionString) {
            //Incapsulate connection manipulation here for simplicity. For better performance it is better to create connection at controller level. 
            _cn = new SqlConnection(connectionString);
            _cn.Open();
        }
        public async Task<IEnumerable<EntityY>> GetListAsync() {
            var result = new List<EntityY>();
            using (var cmd = MsSql.NewCommand(_cn, "procListY")) {

                using (var rs = await cmd.ExecuteReaderAsync()) {
                    while (await rs.ReadAsync()) {
                        result.Add(new EntityY {
                            Id = (int)rs[0],
                            Name = (string)rs[1],
                            Price = (decimal)rs[2],
                            LockedBy = GetValueOrEmpty(rs[3])
                        });
                    }
                }
                return result;
            }
        }
        private static string GetValueOrEmpty(object obj) {
            if (obj == DBNull.Value)
                return string.Empty;
            return (string)obj;
        }
        /// <summary>
        /// Read single entity and lock
        /// </summary>
        /// <param name="id">Id of record</param>
        /// <param name="lockBy">lock for username</param>
        /// <returns></returns>
        public async Task<EntityY> ReadAsync(int id, string lockBy = "") {
            using (var cmd = MsSql.NewCommand(_cn, "procReadY")) {
                cmd.Parameters["@id"].Value = id;
                cmd.Parameters["@lockby"].Value = lockBy;
                try {
                    using (var rs = await cmd.ExecuteReaderAsync()) {
                        if (await rs.ReadAsync()) {
                            return new EntityY {
                                Id = (int)rs[0],
                                Name = (string)rs[1],
                                Price = (decimal)rs[2],
                                LockedBy = GetValueOrEmpty(rs[3])
                            };
                        }
                    }
                } catch (SqlException ex) {
                    if (ex.State == 11)
                        throw new ConcurrencyException(ex.Message, ex);
                    else
                        throw;
                }
                throw new Exception("Record not exists");

            }
        }

        public async Task CreateAsync(EntityY item) {
            using (var cmd = MsSql.NewCommand(_cn, "procAddY")) {
                cmd.Parameters["@name"].Value = item.Name;
                cmd.Parameters["@price"].Value = item.Price;
                await cmd.ExecuteNonQueryAsync();
                item.Id = (int)cmd.Parameters[0].Value;
            }
        }

        public async Task UpdateAsync(EntityY item) {
            //Update record in database. Record is actually updated only if lock is set by matching user
            //If not, SQL SP throws an error.
            using (var cmd = MsSql.NewCommand(_cn, "procUpdateY")) {
                cmd.Parameters["@id"].Value = item.Id;
                cmd.Parameters["@name"].Value = item.Name;
                cmd.Parameters["@price"].Value = item.Price;
                cmd.Parameters["@lockedby"].Value = item.LockedBy;
                try {
                    await cmd.ExecuteNonQueryAsync();
                } catch (SqlException ex) {
                    if (ex.State == 10)
                        throw new ConcurrencyException(ex.Message, ex);
                    else
                        throw;
                }

            }
        }

        public async Task DeleteAsync(int id, string lockBy = "") {
            //Delete do not check for concurrency here,it just deletes the record.
            using (var cmd = MsSql.NewCommand(_cn, "procDeleteY")) {
                cmd.Parameters["@id"].Value = id;
                cmd.Parameters["@lockedby"].Value = lockBy;
                var result = await cmd.ExecuteNonQueryAsync();
                if (result == 0) throw new Exception("Record with this primary key cannot be deleted");

            }
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing) {
            if (!this.disposed) {
                if (disposing) {
                    _cn.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
