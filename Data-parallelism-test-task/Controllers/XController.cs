using ConcurrencyTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConcurrencyTest.Repository;
using System.Configuration;
using System.Threading.Tasks;

namespace ConcurrencyTest.Controllers {
    [Authorize]
    public class XController : Controller {
        // GET: X
        public async Task<ActionResult> Index() {
            ViewBag.Message = TempData["Message"];
            TempData["Message"] = string.Empty;
            using (SqlXRepository repository = new SqlXRepository()) {

                var items = (await repository.GetListAsync()).ToList();
                return View(items);
            }

        }


        // GET: X/Create
        public ActionResult Create() {
            return View();
        }

        // POST: X/Create
        [HttpPost]
        public async Task<ActionResult> Create(EntityX model) {
            try {
                using (SqlXRepository repository = new SqlXRepository()) {
                    await repository.CreateAsync(model).ConfigureAwait(false);
                }
                TempData["Message"] = $"{model.Name} created.";
                return RedirectToAction("Index");
            } catch {
                return View();
            }
        }

        // GET: X/Edit/5
        public async Task<ActionResult> Edit(int id) {
            using (SqlXRepository repository = new SqlXRepository()) {
                try {
                    var model = await repository.ReadAsync(id).ConfigureAwait(false);
                    return View(model);

                } catch (Exception ex) {
                    TempData["Message"] = $"There was an error updating record: {id}";
                    return RedirectToAction("Index");
                }
            }
        }

        // POST: X/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(EntityX model) {
            try {
                using (SqlXRepository repository = new SqlXRepository()) {
                    try {
                        await repository.UpdateAsync(model).ConfigureAwait(false);
                        TempData["Message"] = $"{model.Name} updated successfully.";
                    } catch (ConcurrencyException ex) {
                        var merge = new MergeEntityX {
                            NewItem = model,
                            SavedItem = await repository.ReadAsync(model.Id),
                        };
                        return View("EditMerge", merge);
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
        [HttpPost]
        public async Task<ActionResult> EditMerge(MergeEntityX model) {


            var newModel = model.SavedItem;
            if (model.UpdateName)
                newModel.Name = model.NewItem.Name;
            if (model.UpdatePrice)
                newModel.Price = model.NewItem.Price;
            return await Edit(newModel);
            
        }
        // GET: X/Delete/5
        public async Task<ActionResult> Delete(int id) {
            using (SqlXRepository repository = new SqlXRepository()) {
                try {
                    await repository.DeleteAsync(id).ConfigureAwait(false);
                    TempData["Message"] = "Record deleted successfully.";
                } catch (Exception ex) {
                    TempData["Message"] = $"There was an error deleting record: {id}";
                }
            }
            return RedirectToAction("Index");
        }
    }
}
