﻿using DataParallelismTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataParallelismTest.Controllers
{
    public class XController : Controller
    {
        // GET: X
        public ActionResult Index()
        {
            var context = ApplicationDbContext.Create();

            return View(new List<EntityX>());
        }

        // GET: X/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: X/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: X/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: X/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: X/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: X/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: X/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
