using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailTemplates2.Controllers
{
    public class DownloadController : Controller
    {
        // GET: Download
        public ActionResult Index()
        {
            return View();
        }

        // GET: Download/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Download/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Download/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Download/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Download/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Download/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Download/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
