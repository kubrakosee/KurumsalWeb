
using KurumsalWeb.Models.DataContext;
using KurumsalWeb.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace KurumsalWeb.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        
        KurumsalDBContext db = new KurumsalDBContext();
        [Route("yonetimpaneli")]
        public ActionResult Index()
        {
            ViewBag.Blogsay = db.Blog.Count();
            ViewBag.KategoriSay = db.Kategori.Count();
            ViewBag.HizmetSay = db.Hizmet.Count();
            ViewBag.YorumSay = db.Yorum.Count();
            //blog yorumları onay bekleyen yorumları admin panelinde Bildirim
            ViewBag.YorumOnay = db.Yorum.Where(x => x.Onay == false).Count();
            var sorgu = db.Kategori.ToList();
            return View(sorgu);
        }

        //Admin giriş  view oluşturma
        //admin girişi ve sayfalara erişim
        //mvc de adminlayout kullanmıyacağız add view eklerken
        [Route("yonetimpaneli/giris")]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        //admin Giriş İşlemlerinin Yapılması
        public ActionResult Login(Admin admin/*,*//*string sifre*/)
        {
            //var md5pass = Crypto.Hash(sifre,"MD5");
            var login = db.Admin.Where(x => x.Eposta == admin.Eposta).SingleOrDefault();
            if (login.Eposta==admin.Eposta && login.Sifre==Crypto.Hash(admin.Sifre,"MD5"))
            //1 yöntem if (login.Eposta==admin.Eposta && login.Sifre==md5pass)
            //2 yöntemli if (login.Eposta==admin.Eposta && login.Sifre==Crypto.hash(admin.Sifre,"MD5pass")
            //sonra yıkardaki  var md5pass = Crypto.Hash(sifre,"MD5"); silip string sifre olanı da silip bu şekilde yapabilirdik
            {
                Session["adminId"] = login.AdminId;
                Session["eposta"] = login.Eposta;
                //admin paneliyetkilendirme ile menü gösterimleri editör ve admin yetki kısıtlama
                Session["yetki"] = login.Yetki;

                return RedirectToAction("Index","Admin");
            }
            ViewBag.uyari = "Kullanıcı Adı veya Şifre Eşleşmedi!Tekrar Deneyin";
            return View(admin);
        }
        //adminden çıkış yapmak için
        public ActionResult Logout()
        {

            Session["adminId"] = null;
            Session["eposta"] = null;
            //tüm session da çıkış yapmak için
            Session.Abandon();
            return RedirectToAction("Login", "Admin");
            //şimdi bura bitti adminlayout kısmına gidelim
        }
        public ActionResult SifremiUnuttum()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SifremiUnuttum(string eposta)
        {
            var mail = db.Admin.Where(x => x.Eposta == eposta).SingleOrDefault();
            if (mail!=null)
            {
                Random rdm = new Random();
                int yenisifre = rdm.Next();

                Admin admin = new Admin();
                mail.Sifre = Crypto.Hash(Convert.ToString(yenisifre),"MD5");
                db.SaveChanges();
                //bunu çalıştırdığımızda smpt hatası vericek onu google hesabından Daha az güvenli uygulama erişimi tıklayıp
                //kapalı olan açıp tekrar denediğinde gönderildiğini göreceksin https://www.google.com/settings/security/lesssecureapps   buna tıklayıp 
                //Gelen aşağıdaki sayfadan "Daha az güvenli uygulamalar" linkine tıklanır. sonra bu şekilde hata çözülecektir.
                WebMail.SmtpServer = "smtp.gmail.com";
                //güvenli bağlantı oluşturma işlemi
                WebMail.EnableSsl = true;
                WebMail.UserName = "kubrakosee34@gmail.com";
                WebMail.Password = "160622090kubrak";
                WebMail.SmtpPort = 587;
                WebMail.Send(eposta, "Admin Panel Giriş Şifreniz","Şifreniz :"+yenisifre );
                ViewBag.Uyari = "Şifreniz Başarılı Bir Şekilde Gönderilmiştir.";

            }
            else
            {
                ViewBag.Uyari = "Hata Oluştu.Tekrar Deneyiniz";
            }
            return View();
            
        }


        //admin paneli kullanıcıları Listeleme
        public ActionResult Adminler()
        {
            return View(db.Admin.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Admin admin,string sifre,string eposta)
        {
            if (ModelState.IsValid)
            {
                admin.Sifre = Crypto.Hash(sifre, "MD5");
                db.Admin.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Adminler");

            }
            return View(admin);
        }
        public ActionResult Edit(int id)
        {
            var a = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
            return View(a);
        }
        [HttpPost]
        public ActionResult Edit(int id,Admin admin,string sifre,string eposta)
        {
           
            if (ModelState.IsValid)
            {
                var a = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
                a.Sifre = Crypto.Hash(sifre, "MD5");
                a.Eposta = admin.Eposta;
                a.Yetki = admin.Yetki;
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }
            return View(admin);
        }
        public ActionResult Delete(int id)
        {
            var d = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
            if (d!=null)
            {
                db.Admin.Remove(d);
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }
            return View(d);
        }

    }
}