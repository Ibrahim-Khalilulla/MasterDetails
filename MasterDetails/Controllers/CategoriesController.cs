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
    public class CategoriesController : Controller
    {
        private MyDBContext db = new MyDBContext();

        public async Task<ActionResult> Index()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            return View(await db.Categories.ToListAsync());
        }

        public ActionResult GetCategoryWiseItems(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewData["id"] = id;
            List<Item> items = db.Items.Where(e => e.CategoryID == id).ToList();//all items from a category like 3

            if (items == null)
            {
                return HttpNotFound();
            }

            return PartialView("CategoryWiseItems", items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Items")] Category category, HttpPostedFileBase[] Image, string sid)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Image != null)
                    {
                        if (category.Items.Count == Image.Count())
                        {
                            for (int i = 0; i < category.Items.Count; i++)
                            {
                                // To save a image to a folder
                                string picture = System.IO.Path.GetFileName(Image[i].FileName);
                                string path = System.IO.Path.Combine(Server.MapPath("~/Images"), picture);
                                Image[i].SaveAs(path);

                                // To store as byte[] in a Table of Database
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    Image[i].InputStream.CopyTo(ms);
                                    category.Items[i].Image = ms.GetBuffer();
                                }
                            }
                        }
                        db.Categories.Add(category);//Both Categories and Items will insert their records
                        db.SaveChanges();


                        var s = sid;
                        var s1 = s.Split('|');
                        for (int i = 0; i < s1.Length; i++)
                        {
                            var p = s1[i];
                            //if (i==int.Parse(sid))
                            //{
                            if (p != "" && int.Parse(p) >= 0)
                            {
                                var st5 = db.Items.Find(category.Items[int.Parse(p)].ID);
                                db.Items.Remove(st5);

                            }
                            //}
                        }
                        db.SaveChanges();


                        TempData["id"] = category.ID;
                        return RedirectToAction("Index");
                    }
                }
                return View(category);
            }
            catch (Exception)
            {
                return View(category);
            }
        }
        //public JsonResult Edit1(long? id)
        //{
        //  //  Category category = db.Categories.Find(id);
        //    //var category = (from c in db.Categories where c.ID == id select new { c.ID, c.Name, c.Items. });
        //   // return Json(category, JsonRequestBehavior.AllowGet);
        //    //return category;
        //}

        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = await db.Categories.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", id);
            return PartialView(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Category category, HttpPostedFileBase[] file, string sid1)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    for (int i = 0; i < category.Items.Count; i++)
                    {
                        if (file[i] != null)
                        {
                            // To save a image to a folder
                            string picture = System.IO.Path.GetFileName(file[i].FileName);
                            string path = System.IO.Path.Combine(Server.MapPath("~/Images"), picture);
                            file[i].SaveAs(path);

                            // To store as byte[] in a Table of Database
                            using (MemoryStream ms = new MemoryStream())
                            {
                                file[i].InputStream.CopyTo(ms);
                                category.Items[i].Image = ms.GetBuffer();
                            }
                        }
                    }
                }
                db.Entry(category).State = EntityState.Modified;
                foreach (Item item in category.Items)
                {
                    db.Entry(item).State = EntityState.Modified;
                }
                await db.SaveChangesAsync();

                var s = sid1;
                var s1 = s.Split('|');
                for (int i = 0; i < s1.Length; i++)
                {
                    var p = s1[i];
                    //if (i==int.Parse(sid))
                    //{
                    if (p != "" && int.Parse(p) >= 0)
                    {
                        var st5 = db.Items.Find(category.Items[int.Parse(p)].ID);
                        db.Items.Remove(st5);

                    }
                    //}
                }
                db.SaveChanges();
                TempData["id"] = category.ID;
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = await db.Categories.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return PartialView(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Category category = await db.Categories.FindAsync(id);
            db.Categories.Remove(category);
            await db.SaveChangesAsync();//not only category. it will remove associated items also
            return RedirectToAction("Index");
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