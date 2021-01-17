using KurumsalWeb.Models.DataContext;
using KurumsalWeb.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Policy;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using KurumsalWeb.Models;

namespace KurumsalWeb.Controllers
{
    public class HomeController : Controller
    {
        private KurumsalDBContext db = new KurumsalDBContext();
        // GET: Home
        [Route("")]
        [Route("Anasayfa")]
        
        public ActionResult Index()
        {
            //site kimlik entegrasyonu seo uyumluluk çalışmaları
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();

            //dinamik olarak verileri çekicez hizmetlerin
            ViewBag.Hizmetler = db.Hizmet.ToList().OrderByDescending(x => x.HizmetId);
            return View();
        }
        public ActionResult SliderPartial()
        {
            return View(db.Slider.ToList().OrderByDescending(x=>x.SliderId));
        }
        public ActionResult HizmetPartial()
        {
            return View(db.Hizmet.ToList());
        }
        [Route("Hakkimizda")]
        public ActionResult Hakkimizda()
        {
            //site kimlik entegrasyonu seo uyumluluk çalışmaları
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();

            return View(db.Hakkimizda.SingleOrDefault());
        }
        [Route("Hizmet")]
        public ActionResult Hizmet()

        {
            //site kimlik entegrasyonu seo uyumluluk çalışmaları
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();

            return View(db.Hizmet.ToList().OrderByDescending(x=>x.HizmetId));
        }
        [Route("iletisim")]
        public ActionResult Iletisim()
        {
            //site kimlik entegrasyonu seo uyumluluk çalışmaları
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();

            return View(db.Iletisim.SingleOrDefault());
        }
        [HttpPost]
        //Iletisim mail gönderme işlemi
        public ActionResult Iletisim(string adsoyad=null,string email=null,string konu=null,string mesaj=null)
        {
            if (adsoyad!=null && email!=null)
            {
                //bunu çalıştırdığımızda smpt hatası vericek onu google hesabından Daha az güvenli uygulama erişimi tıklayıp
                //kapalı olan açıp tekrar denediğinde gönderildiğini göreceksin https://www.google.com/settings/security/lesssecureapps   buna tıklayıp 
                //Gelen aşağıdaki sayfadan "Daha az güvenli uygulamalar" linkine tıklanır. sonra bu şekilde hata çözülecektir.
                WebMail.SmtpServer = "smtp.gmail.com";
                //güvenli bağlantı oluşturma işlemi
                WebMail.EnableSsl = false;
                WebMail.UserName = "kubrakosee34@gmail.com";
                WebMail.Password = "160622090kubrak";
                WebMail.SmtpPort = 587;
                WebMail.Send("kubrakosee34@gmail.com", konu, email + "</br>" + mesaj);
                ViewBag.Uyari = "Mesajınız Başarı İle Gönderilmiştir.";

            }
            else
            {
                ViewBag.Uyari = "Hata Oluştu.Tekrar Deneyiniz";
            }
            return View();
        }
        [Route("BlogPost")]
        public ActionResult Blog(int Sayfa=1)
        {
            //site kimlik entegrasyonu seo uyumluluk çalışmaları
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();

            //NUget kısmından Pagedlist kurucaz 2 tane bir tane mvc pagedlist bırde normal pagedlist kur sonra 
            //aşağıda gibi yap yukarı @using pagedlist ekle birde @using pagedlist.mvc diyoruz.
            //sonra tolist yerine topagedlist diyoruz.dedikten sonra blog.cshtml kımına gel

            return View(db.Blog.Include("Kategori").OrderByDescending(x => x.BlogId).ToPagedList(Sayfa,5));
        }
        
        [Route("BlogPost/{kategoriad}/{id:int}")]
        //Blog yazılarını kategoriye göre Listelenmesi
        public ActionResult KategoriBlog(int id,int Sayfa=1)
        {
            //site kimlik entegrasyonu seo uyumluluk çalışmaları
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();

            var b = db.Blog.Include("Kategori").OrderByDescending(x=>x.BlogId).Where(x => x.Kategori.KategoriId == id).ToPagedList(Sayfa,5);
            return View(b);
        }

        public JsonResult YorumYap(string adsoyad,string eposta,string icerik,int blogId)
        {
            if (icerik==null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            db.Yorum.Add(new Yorum { AdSoyad = adsoyad, Eposta = eposta, İcerik = icerik, BlogId = blogId,Onay=false});
            db.SaveChanges();
            
            //yorumların alıp verilebilmesi için allowget dememiz gerekiyor
            return Json(false,JsonRequestBehavior.AllowGet);
        }
        [Route("BlogPost/{baslik}-{id:int}")]
        public ActionResult BlogDetay(int id)
        {
            //site kimlik entegrasyonu seo uyumluluk çalışmaları
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();

            var b = db.Blog.Include("Kategori").Include("Yorums").Where(x => x.BlogId == id).SingleOrDefault();
            return View(b);
        }
        public ActionResult BlogKategoriPartial()
        {
            //site kimlik entegrasyonu seo uyumluluk çalışmaları
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();

            return PartialView(db.Kategori.Include("Blogs").ToList().OrderBy(x=>x.KategoriAd));
        }
        public ActionResult BlogKayitPartial()
        {
            //site kimlik entegrasyonu seo uyumluluk çalışmaları
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();

            return PartialView(db.Blog.ToList().OrderByDescending(x=>x.BlogId));
        }
        public ActionResult FooterPartial()
        {
            

            //Site layout bağımsız bir şekilde footer partial yapıp her yerde çağırabiliriz
            ViewBag.Iletisim = db.Iletisim.SingleOrDefault();

            ViewBag.Blog = db.Blog.ToList().OrderByDescending(x => x.BlogId);
            //dinamik olarak verileri çekicez hizmetlerin
            ViewBag.Hizmetler = db.Hizmet.ToList().OrderByDescending(x => x.HizmetId);
            return PartialView();
        }
       
    }
}