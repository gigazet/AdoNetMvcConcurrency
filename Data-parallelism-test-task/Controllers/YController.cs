using ConcurrencyTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConcurrencyTest.Repository;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace ConcurrencyTest.Controllers {
    [Authorize]
    public class YController : Controller {
        // GET: Y
        public async Task<ActionResult> Index() {
            ViewBag.Message = TempData["Message"];
            TempData["Message"] = string.Empty;
            using (var repository = new SqlYRepository()) {
                var items = (await repository.GetListAsync()).ToList();
                return View(items);
            }

        }


        // GET: Y/Create
        public ActionResult Create() {
            return View();
        }

        // POST: Y/Create
        [HttpPost]
        public async Task<ActionResult> Create(EntityY model) {
            try {
                using (var repository = new SqlYRepository()) {
                    await repository.CreateAsync(model).ConfigureAwait(false);
                }
                TempData["Message"] = $"{model.Name} created.";
                return RedirectToAction("Index");
            } catch {
                return View();
            }
        }

        // GET: Y/Edit/5
        public async Task<ActionResult> Edit(int id) {
            using (var repository = new SqlYRepository()) {
                try {
                    var model = await repository.ReadAsync(id, User.Identity.GetUserName()).ConfigureAwait(false);
                    return View(model);
                } catch (ConcurrencyException ex) {
                    TempData["Message"] = ex.Message;
                    return RedirectToAction("Index");
                } catch (Exception ex) {
                    TempData["Message"] = $"There was an error updating record: {id}";
                    return RedirectToAction("Index");
                }
            }
        }

        // POST: Y/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(EntityY model) {
            try {
                using (var repository = new SqlYRepository()) {
                    try {
                        model.LockedBy = User.Identity.GetUserName();
                        await repository.UpdateAsync(model).ConfigureAwait(false);
                        TempData["Message"] = $"{model.Name} updated successfully.";
                    } catch (ConcurrencyException ex) {
                        TempData["Message"] = $"Record {model.Id} is locked by another user";
                    } catch (Exception ex) {
                        TempData["Message"] = $"There was an error updating record: {model.Id}";
                    }
                }
                return RedirectToAction("Index");
            } catch (Exception ex) {
                TempData["Message"] = $"Cannot update {model.Name}.";
                return RedirectToAction("Index");
            }
        }
      
        // GET: Y/Delete/5
        public async Task<ActionResult> Delete(int id) {
            using (var repository = new SqlYRepository()) {
                try {
                    await repository.DeleteAsync(id, User.Identity.GetUserName()).ConfigureAwait(false);
                    TempData["Message"] = "Record deleted successfully.";
                } catch (Exception ex) {
                    TempData["Message"] = $"There was an error deleting record: {id}";
                }
            }
            return RedirectToAction("Index");
        }
    }
}
