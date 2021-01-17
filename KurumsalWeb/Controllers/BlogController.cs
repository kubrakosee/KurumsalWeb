using KurumsalWeb.Models.DataContext;
using KurumsalWeb.Models.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;

namespace KurumsalWeb.Controllers
{
    public class BlogController : Controller
    {
        private KurumsalDBContext db = new KurumsalDBContext();
        // GET: Blog
        public ActionResult Index()
        {
            
            return View(db.Blog.ToList().OrderByDescending(x=>x.BlogId));
        }
        public ActionResult Create()
        {
            ViewBag.KategoriId = new SelectList(db.Kategori,"KategoriId","KategoriAd");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Blog blog,HttpPostedFileBase ResimURL)
        {
            //BİZE GELEN MODELİ VERİ TABANINA KAYDETMEMİZ GEREKİYOR
            if (ResimURL != null)
            {
                
                WebImage img = new WebImage(ResimURL.InputStream);
                FileInfo imginfo = new FileInfo(ResimURL.FileName);

                string blogimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                img.Resize(600, 400);
                img.Save("~/Uploads/Blog/" + blogimgname);

                blog.ResimURL = "/Uploads/Blog/" + blogimgname;

            }
            db.Blog.Add(blog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //Blog kayıt düzenleme view oluşturma
        public ActionResult Edit(int id)
        {
            if (id==null)
            {
                return HttpNotFound();

            }
            var b = db.Blog.Where(x => x.BlogId == id).SingleOrDefault();
            if (b==null)
            {
                return HttpNotFound();
            }
            //burda kategori kısmımızı doldurmamız lazım edit den sonra buraya gel
            //Veri taşıma işlemi yapıyoruz aşağı da gördüğünüz gibi
            ViewBag.KategoriId = new SelectList(db.Kategori, "KategoriId", "KategoriAd",b.KategoriId);
            return View(b);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        //Blog kayıt düzenleme action işlemleri
        public ActionResult Edit(int id,Blog blog,HttpPostedFileBase ResimURL)
        {
            //doğrulama işlemi doğruysa ise yani başarılıysa işleme başla
            if (ModelState.IsValid)
            {
                //blog kaydımızı bulmamız gerekiyor
                var b = db.Blog.Where(x => x.BlogId == id).SingleOrDefault();
                //tek kayıt getireceğimiz için singleordefault yapıyoruz.
                if (ResimURL != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(b.ResimURL)))
                    //(System.IO.File.Exists(Server.MapPath(kimlik.LogoURL)))
                    {
                        System.IO.File.Delete(Server.MapPath(b.ResimURL));
                        //System.IO.File.Delete(Server.MapPath(kimlik.LogoURL));
                    }
                    WebImage img = new WebImage(ResimURL.InputStream);
                    FileInfo imginfo = new FileInfo(ResimURL.FileName);

                    string blogimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(600, 400);
                    img.Save("~/Uploads/Blog/" + blogimgname);

                    b.ResimURL = "/Uploads/Blog/" + blogimgname;
                }
                b.Baslik = blog.Baslik;
                b.Icerik = blog.Icerik;
                b.KategoriId = blog.KategoriId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //herhangi bir hata olursa bize blog u döndürsün 
            return View(blog);
        }

                    //Blog kayıt silme işlemi
        [HttpPost]
        public ActionResult Delete(int id)
        {
                    //burda id bulduruyoruz
                    
            var b = db.Blog.Find(id);
            if (b==null)
            {
                return HttpNotFound();
            }
            //resim  var resmide silmemiz gerekiyor
            //kayda bak b de resim var mı 1
            if (System.IO.File.Exists(Server.MapPath(b.ResimURL)))
            //(System.IO.File.Exists(Server.MapPath(kimlik.LogoURL)))
            {
                //varsa git buna bana sil 2
                System.IO.File.Delete(Server.MapPath(b.ResimURL));
                //System.IO.File.Delete(Server.MapPath(kimlik.LogoURL));
            }
           
            db.Blog.Remove(b);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}