

using ConcurrencyTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ConcurrencyTest.Repository;
using System.Reflection;
using System.IO;

[TestClass()]
public class SqlYRepositoryTests {
    public string _connectionString;
    [TestInitialize]
    public void Init() {
        var dbFile = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\..\\..\\..\\Data-parallelism-test-task\\App_Data\\aspnet-ConcurrencyTest-20160403112450.mdf";
        _connectionString = $"Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename={Path.GetFullPath(dbFile)} ;Initial Catalog=aspnet-ConcurrencyTest-20160403112450;Integrated Security=True";
    }
    [TestMethod()]
    public async Task CreateListRemove_Crud_Validate() {

        using (var repository = new SqlYRepository(_connectionString)) {
            var newY = new EntityY {
                Name = "Test",
                Price = 1
            };
            await repository.CreateAsync(newY);
            Assert.IsTrue(newY.Id > 0);
            //read and set lock on this record
            var readX = await repository.ReadAsync(newY.Id, "user");

            Assert.AreEqual(newY.Name, readX.Name);
            Assert.AreEqual(newY.Id, readX.Id);
            Assert.AreEqual(newY.Price, readX.Price);
            //delete record with lock
            await repository.DeleteAsync(newY.Id, "user");
        }

    }
    [TestMethod()]
    public async Task GetList_ReadAllRecords_RecordsFound() {

        using (SqlYRepository repository = new SqlYRepository(_connectionString)) {

            var items = (await repository.GetListAsync()).ToList();
            Assert.IsTrue(items.Count > 0);
        }

    }
    [TestMethod()]
    public async Task Update_UpdateRecord_ValidateResult() {

        using (SqlYRepository repository = new SqlYRepository(_connectionString)) {
            var newY = new EntityY {
                Name = "Update1",
                Price = 1
            };
            try {
                await repository.CreateAsync(newY);
                newY = await repository.ReadAsync(newY.Id, "user");
                newY.Name = "Update2";
                newY.Price = 2;
                await repository.UpdateAsync(newY);

                var readX = await repository.ReadAsync(newY.Id, "user");

                Assert.AreEqual(newY.Name, readX.Name);
                Assert.AreEqual(newY.Id, readX.Id);
                Assert.AreEqual(newY.Price, readX.Price);
            } finally {
                await repository.DeleteAsync(newY.Id, "user");
            }
        }

    }

    [TestMethod()]
    public async Task Update_UpdateRecordParallelPessimistic_ValidateResult() {
        var newY = new EntityY {
            Name = "Update1",
            Price = 1
        };
        using (SqlYRepository repository1 = new SqlYRepository(_connectionString))
        using (SqlYRepository repository2 = new SqlYRepository(_connectionString)) {

            try {
                await repository1.CreateAsync(newY);

                //Read Item by 1-st user
                newY = await repository1.ReadAsync(newY.Id, "user1");

                //read should fail here
                EntityY readY2=null;
                try {
                    readY2 = await repository2.ReadAsync(newY.Id, "user2");
                } catch (ConcurrencyException ex) {
                    Assert.IsTrue(ex.Message.StartsWith("Lock already set"));
                } catch (Exception ex) {
                    Assert.Fail();
                }
                //Read the same item by 2-nd user, should fail here


                //Modify and save updated by first user
                newY.Name = "Update2";
                newY.Price = 2;
                newY.LockedBy = "user1";
                await repository1.UpdateAsync(newY);

               //the result should be from the 1-st user update
                var readY = await repository1.ReadAsync(newY.Id,"user1");

                Assert.AreEqual(newY.Name, readY.Name);
                Assert.AreEqual(newY.Id, readY.Id);
                Assert.AreEqual(newY.Price, readY.Price);
            } finally {
                await repository1.DeleteAsync(newY.Id, "user1");
            }
        }

    }
}