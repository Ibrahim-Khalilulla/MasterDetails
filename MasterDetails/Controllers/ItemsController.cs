using MasterDetails.Context;
using MasterDetails.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MasterDetails.Controllers
{
    public class ItemsController : Controller
    {
        private MyDBContext db = new MyDBContext();

        public ActionResult Create(long? id)
        {
            if (id == null)
            {
                ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            }
            else
            {
                ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", id);//only those items with given category, like:3
            }
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Name,Description,EntryDate,Quantity,CategoryID")] Item item, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    // To save a image to a folder
                    string picture = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images"), picture);
                    file.SaveAs(path);

                    // To store as byte[] in a Table of Database
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        item.Image = ms.GetBuffer();
                    }
                    db.Items.Add(item);
                    await db.SaveChangesAsync();
                    TempData["id"] = item.CategoryID;
                    return RedirectToAction("Index", "Categories");
                }
                else
                {
                    ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", item.CategoryID);
                    return PartialView(item);
                }
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", item.CategoryID);
            return PartialView(item);
        }

        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = await db.Items.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", item.CategoryID);
            return PartialView(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name,Description,Image,EntryDate,Quantity,CategoryID")] Item item, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    // To save a image to a folder
                    string picture = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images"), picture);
                    file.SaveAs(path);

                    // To store as byte[] in a Table of Database
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        item.Image = ms.GetBuffer();
                    }
                }
                db.Entry(item).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData["id"] = item.CategoryID;
                return RedirectToAction("Index", "Categories");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", item.CategoryID);
            return PartialView(item);
        }

        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = await db.Items.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return PartialView(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Item item = await db.Items.FindAsync(id);
            db.Items.Remove(item);
            await db.SaveChangesAsync();
            TempData["id"] = item.CategoryID;
            return RedirectToAction("Index", "Categories");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}