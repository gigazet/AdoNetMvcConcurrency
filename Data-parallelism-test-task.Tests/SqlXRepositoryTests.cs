

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
public class SqlXRepositoryTests {
    public string _connectionString;
    [TestInitialize]
     public void Init() {
        var dbFile =$"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\..\\..\\..\\Data-parallelism-test-task\\App_Data\\aspnet-ConcurrencyTest-20160403112450.mdf";
        _connectionString = $"Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename={Path.GetFullPath(dbFile)} ;Initial Catalog=aspnet-ConcurrencyTest-20160403112450;Integrated Security=True";
    }
    [TestMethod()]
    public async Task CreateListRemove_Crud_Validate() {

        using (SqlXRepository repository = new SqlXRepository(_connectionString)) {
            var newX = new EntityX {
                Name = "Test",
                Price = 1
            };
            await repository.CreateAsync(newX);
            Assert.IsTrue(newX.Id > 0);
            var readX = await repository.ReadAsync(newX.Id);

            Assert.AreEqual(newX.Name, readX.Name);
            Assert.AreEqual(newX.Id, readX.Id);
            Assert.AreEqual(newX.Price, readX.Price);
            Assert.IsFalse(readX.RowVersion == Guid.Empty);
            await repository.DeleteAsync(newX.Id);
        }

    }
    [TestMethod()]
    public async Task GetList_ReadAllRecords_RecordsFound() {

        using (SqlXRepository repository = new SqlXRepository(_connectionString)) {

            var items = (await repository.GetListAsync()).ToList();
            Assert.IsTrue(items.Count > 0);
        }

    }
    [TestMethod()]
    public async Task Update_UpdateRecord_ValidateResult() {

        using (SqlXRepository repository = new SqlXRepository(_connectionString)) {
            var newX = new EntityX {
                Name = "Update1",
                Price = 1
            };
            try {
                await repository.CreateAsync(newX);
                newX = await repository.ReadAsync(newX.Id);
                newX.Name = "Update2";
                newX.Price = 2;
                await repository.UpdateAsync(newX);

                var readX = await repository.ReadAsync(newX.Id);

                Assert.AreEqual(newX.Name, readX.Name);
                Assert.AreEqual(newX.Id, readX.Id);
                Assert.AreEqual(newX.Price, readX.Price);
                Assert.IsFalse(newX.RowVersion == readX.RowVersion);
            } finally {
                await repository.DeleteAsync(newX.Id);
            }
        }

    }

    [TestMethod()]
    public async Task Update_UpdateRecordParallelOptimistic_ValidateResult() {
        var newX = new EntityX {
            Name = "Update1",
            Price = 1
        };
        using (SqlXRepository repository1 = new SqlXRepository(_connectionString))
        using (SqlXRepository repository2 = new SqlXRepository(_connectionString)) {

            try {
                await repository1.CreateAsync(newX);

                //Read Item by 1-st user
                newX = await repository1.ReadAsync(newX.Id);

                //Read the same item by 2-nd user
                var readX2 = await repository2.ReadAsync(newX.Id);

                //Modify and save updated by first user
                newX.Name = "Update2";
                newX.Price = 2;
                await repository1.UpdateAsync(newX);

                //try to update the same record by 2-nd user
                readX2.Name = "Update3";
                readX2.Price = 3;
                //update should fail here
                try {
                    await repository2.UpdateAsync(readX2);
                } catch (ConcurrencyException ex) {
                    Assert.IsTrue(ex.Message == "The value was already changed");
                } catch {
                    Assert.Fail();
                }
                //the result should be from the 1-st user update
                var readX = await repository1.ReadAsync(newX.Id);

                Assert.AreEqual(newX.Name, readX.Name);
                Assert.AreEqual(newX.Id, readX.Id);
                Assert.AreEqual(newX.Price, readX.Price);
                Assert.IsFalse(newX.RowVersion == readX.RowVersion);
            } finally {
                await repository1.DeleteAsync(newX.Id);
            }
        }

    }
}