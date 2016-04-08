using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ConcurrencyTest.Models;
using System.Configuration;
using System.Threading.Tasks;

namespace ConcurrencyTest.Repository {

    public class SqlXRepository : ICrudRepository<EntityX> {

        SqlConnection _cn;

        public SqlXRepository() {
            _cn = new SqlConnection(ConfigurationManager.ConnectionStrings[1].ToString());
            _cn.Open();
        }
        public SqlXRepository(string connectionString) {
            //Incapsulate connection manipulation here for simplicity. For better performance it is better to create connection at controller level. 
            _cn = new SqlConnection(connectionString);
            _cn.Open();
        }
        public async Task<IEnumerable<EntityX>> GetListAsync() {
            var result = new List<EntityX>();
            using (var cmd = MsSql.NewCommand(_cn, "procListX")) {

                using (var rs = await cmd.ExecuteReaderAsync()) {
                    while (await rs.ReadAsync()) {
                        result.Add(new EntityX {
                            Id = (int)rs[0],
                            Name = (string)rs[1],
                            Price = (decimal)rs[2],
                            RowVersion = (Guid)rs[3]
                        });
                    }
                }
                return result;
            }
        }

        public async Task<EntityX> ReadAsync(int id) {
            using (var cmd = MsSql.NewCommand(_cn, "procReadX")) {
                cmd.Parameters["@id"].Value = id;

                using (var rs = await cmd.ExecuteReaderAsync()) {
                    if (await rs.ReadAsync()) {
                        return new EntityX {
                            Id = (int)rs[0],
                            Name = (string)rs[1],
                            Price = (decimal)rs[2],
                            RowVersion = (Guid)rs[3]
                        };
                    }
                }
                throw new Exception("Record not exists");

            }
        }

        public async Task CreateAsync(EntityX item) {
            using (var cmd = MsSql.NewCommand(_cn, "procAddX")) {
                cmd.Parameters["@name"].Value = item.Name;
                cmd.Parameters["@price"].Value = item.Price;
                await cmd.ExecuteNonQueryAsync();
                item.Id = (int)cmd.Parameters[0].Value;
            }
        }

        public async Task UpdateAsync(EntityX item) {
            //Update record in database. Record is actually updated only if rowVersion matches.
            //If not, SQL SP throws an error.
            using (var cmd = MsSql.NewCommand(_cn, "procUpdateX")) {
                cmd.Parameters["@id"].Value = item.Id;
                cmd.Parameters["@name"].Value = item.Name;
                cmd.Parameters["@price"].Value = item.Price;
                cmd.Parameters["@rowversion"].Value = item.RowVersion;
                try {
                    await cmd.ExecuteNonQueryAsync();
                } catch(SqlException ex) {
                    if (ex.State == 10)
                        throw new ConcurrencyException(ex.Message, ex);
                    else
                        throw;
                }
                
            }
        }

        public async Task DeleteAsync(int id) {
            //Delete do not check for concurrency here,it just deletes the record.
            using (var cmd = MsSql.NewCommand(_cn, "procDeleteX")) {
                cmd.Parameters["@id"].Value = id;

                var result = await cmd.ExecuteNonQueryAsync();
                if (result == 0) throw new Exception("Record with this primary key not exists");

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
