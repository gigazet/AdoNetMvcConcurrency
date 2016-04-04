using DataParallelismTest.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Repository {
    interface ICrudRepository<T> : IDisposable
        where T : class {
        IEnumerable<T> GetList(); // получение всех объектов
        T Read(int id); // получение одного объекта по id
        void Create(T item); // создание объекта
        void Update(T item); // обновление объекта
        void Delete(int id); // удаление объекта по id
    }

    
}
