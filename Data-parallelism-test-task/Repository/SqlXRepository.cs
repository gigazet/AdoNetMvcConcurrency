using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DataParallelismTest.Models;

namespace WebApplication1.Repository {

    public class SqlXRepository : ICrudRepository<EntityX> {

        SqlConnection _cn;

        public SqlXRepository() {
            this._cn = new SqlConnection();
        }

        public IEnumerable<EntityX> GetList() {
            return db.Books;
        }

        public EntityX Read(int id) {
            return db.Books.Find(id);
        }

        public void Create(EntityX item) {
            var cmd = new SqlCommand("proc_AddX",_cn);
            cmd.Parameters.AddWithValue()
        }

        public void Update(EntityX item) {
            //db.Entry(book).State = EntityState.Modified;
        }

        public void Delete(int id) {
            //Book book = db.Books.Find(id);
            //if (book != null)
            //    db.Books.Remove(book);
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
