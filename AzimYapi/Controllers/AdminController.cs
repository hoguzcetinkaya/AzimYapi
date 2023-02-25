using AzimYapi.Db;
using AzimYapi.Identity;
using AzimYapi.Models;
using AzimYapi.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzimYapi.Controllers
{
    [Authorize(Users = "beraavsar,fazliavsar,mehmetatigan")]
    public class AdminController : Controller
    {
        DataContext db = new DataContext();

        private UserManager<ApplicationUser> userManager;
        public AdminController()
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new IdentityContext()));

        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            if(User.Identity.IsAuthenticated)//kullanıcı giriş yaptıysa ve çıkış yapmadıysa ürünlere yönlendir
            {
                return RedirectToAction("Urunler");
            }
                
            var list = userManager.Users.ToList();

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Index(IdentityModel model)
        {
            if (ModelState.IsValid)
            {
                var kullanici = userManager.Find(model.KullaniciAdi, model.Sifre);
                if (kullanici != null)
                {
                    var authManager = HttpContext.GetOwinContext().Authentication;// kullanıcı girdi çıktılarını yönetmek için
                    var identityclaims = userManager.CreateIdentity(kullanici, "ApplicationCookie"); // kullanıcı için cookie oluşturmak için
                    var authProperties = new AuthenticationProperties();
                    authProperties.IsPersistent = true;//hatırlamak için
                    authManager.SignOut();
                    authManager.SignIn(authProperties, identityclaims);
                    return RedirectToAction("Urunler");
                }
                else
                {
                    var adminSayi = userManager.Users.ToList();
                    if (adminSayi.Count() >= 3)
                    {
                        return View(model);
                    }
                    else
                    {
                        ApplicationUser akulanici = new ApplicationUser();
                        akulanici.UserName = model.KullaniciAdi;
                        akulanici.sifre = model.Sifre;


                        IdentityResult result = userManager.Create(akulanici, model.Sifre);
                        if (result.Succeeded)
                        {
                            TempData["kayitBasarili"] = "Kayıt işlemi başarıyla tamamlanmıştır giriş yapabilirsiniz";
                            return RedirectToAction("Index");
                        }
                    }



                }
            }
            return View(model);
        }
        // URUN İŞLEMLERİ
        [HttpGet]
        public ActionResult UrunEkle()
        {
            var logo = db.Gorsel.FirstOrDefault(x => x.Aktif == true && x.GorselTuru == "logo");
            if (logo != null)
            {
                ViewBag.Logo = logo.GorselAdi;
            }
            else
            {
                ViewBag.Logo = "0";
            }
            var kategoriler = db.Kategori.ToList();
            if (kategoriler != null)
            {
                ViewBag.Kategoriler = kategoriler;
            }
            return View();
        }

        [HttpPost]
        public ActionResult UrunEkle(UrunlerModel.Urun model, int kategoriId)
        {
            if (model.urunGorselAdi != null)
            {
                if (Request.Files.Count > 0)
                {

                    string dosyaAdi = Path.GetFileName(Request.Files[0].FileName);
                    string uzanti = Path.GetExtension(Request.Files[0].FileName);
                    string yol = "~/Content/img/" + dosyaAdi;
                    Request.Files[0].SaveAs(Server.MapPath(yol));

                    var urun = new UrunlerModel.Urun();
                    urun.UrunAdi = model.UrunAdi;
                    urun.fiyat = model.fiyat;
                    urun.enCokSatan = false;
                    urun.urunGorselAdi = dosyaAdi;
                    urun.stok = model.stok;
                    urun.KategoriId = kategoriId;
                    db.Urun.Add(urun);
                    db.SaveChanges();
                    TempData["UrunEklemeBasarili"] = "Ürün eklenmiştir.";
                    return RedirectToAction("UrunEkle");
                }
                else
                {
                    TempData["UrunEklemeBasarisiz"] = "Ürün eklenememiştir tekrar deneyiniz.";
                    return RedirectToAction("UrunEkle");
                }
            }
            else
            {
                TempData["UrunEklemeBasarisiz"] = "Ürün eklenememiştir tekrar deneyiniz.";
                return RedirectToAction("UrunEkle");
            }


        }

        [HttpGet]
        public ActionResult Urunler()
        {
            var logo = db.Gorsel.FirstOrDefault(x => x.Aktif == true && x.GorselTuru == "logo");
            if (logo != null)
            {
                ViewBag.Logo = logo.GorselAdi;
            }
            else
            {
                ViewBag.Logo = "0";
            }

            var urunler = db.Urun.Where(x => x.urunGorselAdi != null).ToList();
            var kategoriler = new List<KategoriModel.Kategori>();
            foreach (var item in urunler)
            {
                //view model çözersen 
                // ürünler tablosuna kategori adlarını yazdır
                if (item.KategoriId != 0)
                {
                    var kategori = db.Kategori.FirstOrDefault(x => x.Id == item.KategoriId);
                    if (kategori != null)
                    {
                        kategoriler.Add(kategori);
                    }
                }
            }


            return View(urunler);
        }

        [HttpGet]   
        public ActionResult EnCokSatanEkle(int id)
        {
            var urun = db.Urun.FirstOrDefault(x => x.Id == id);
            if (urun != null)
            {
                urun.enCokSatan = true;

                db.SaveChanges();
                TempData["basarili"] = "İşlem Başarılı";
                return RedirectToAction("Urunler");

            }
            else
            {
                return RedirectToAction("Urunler");

            }
        }

        [HttpGet]
        public ActionResult EnCokSatanCikar(int id)
        {
            var urun = db.Urun.FirstOrDefault(x => x.Id == id);
            if (urun != null)
            {
                urun.enCokSatan = false;

                db.SaveChanges();
                TempData["basarili"] = "İşlem Başarılı";
                return RedirectToAction("Urunler");

            }
            else
            {
                return RedirectToAction("Urunler");

            }
        }

        [HttpGet]
        public ActionResult UrunSil(int id)
        {
            var urun = db.Urun.FirstOrDefault(x => x.Id == id);
            if (urun != null)
            {
                string dosyaAdi = urun.urunGorselAdi;
                var folderPath = Server.MapPath("~/Content/img/");
                var imagePath = folderPath + dosyaAdi;
                try
                {
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                        Console.WriteLine("Dosya başarıyla silindi.");
                    }
                    else
                    {
                        Console.WriteLine("Dosya bulunamadı.");
                    }
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine("Dosya, başka bir işlem tarafından kullanılıyor.");
                }
                catch (System.UnauthorizedAccessException e)
                {
                    Console.WriteLine("Bu işlem için gerekli izne sahip değilsiniz.");
                }



                db.Urun.Remove(urun);
                db.SaveChanges();
                TempData["basarili"] = "İşlem Başarılı";

                return RedirectToAction("Urunler");

            }
            else
            {
                return RedirectToAction("Urunler");

            }
        }

        [HttpGet]
        public ActionResult UrunGuncelle(int id, string ad)
        {
            var logo = db.Gorsel.FirstOrDefault(x => x.Aktif == true && x.GorselTuru == "logo");
            if (logo != null)
            {
                ViewBag.Logo = logo.GorselAdi;
            }
            else
            {
                ViewBag.Logo = "0";
            }
            var urun = db.Urun.FirstOrDefault(x => x.Id == id);
            return View(urun);
        }
        [HttpPost]
        public ActionResult UrunGuncelle(UrunlerModel.Urun model)
        {

            var urun = db.Urun.FirstOrDefault(x => x.Id == model.Id);
            if (Request.Files.Count > 0)
            {

                string dosyaAdi = Path.GetFileName(Request.Files[0].FileName);
                if (urun.urunGorselAdi != dosyaAdi)
                {
                    string yol = "~/Content/img/" + dosyaAdi;
                    Request.Files[0].SaveAs(Server.MapPath(yol));
                    urun.urunGorselAdi = dosyaAdi;
                }
                urun.UrunAdi = model.UrunAdi;
                urun.fiyat = model.fiyat;
                urun.stok = model.stok;
                db.SaveChanges();
                TempData["basarili"] = "İşlem başarılı";
            }
            return RedirectToAction("Urunler");
        }


        // KATEGORİ İŞLEMLERİ
        [HttpGet]
        public ActionResult KategoriEkle()
        {
            var logo = db.Gorsel.FirstOrDefault(x => x.Aktif == true && x.GorselTuru == "logo");
            if (logo != null)
            {
                ViewBag.Logo = logo.GorselAdi;
            }
            else
            {
                ViewBag.Logo = "0";
            }
            return View();
        }
        [HttpPost]
        public ActionResult KategoriEkle(KategoriModel.Kategori model)
        {
            var kategoriKontrol = db.Kategori.FirstOrDefault(x => x.KategoriAd == model.KategoriAd);
            if (kategoriKontrol == null)
            {
                var kategori = new KategoriModel.Kategori();
                kategori.KategoriAd = model.KategoriAd;
                db.Kategori.Add(kategori);
                db.SaveChanges();
                TempData["KategoriEklemeBasarili"] = "Kategori eklenmiştir.";
                return RedirectToAction("KategoriEkle");
            }
            else
            {
                TempData["KategoriEklemeBasarisiz"] = "Kategori mevcuttur.";
                return RedirectToAction("KategoriEkle");
            }
        }

        [HttpGet]
        public ActionResult Kategoriler()
        {
            var logo = db.Gorsel.FirstOrDefault(x => x.Aktif == true && x.GorselTuru == "logo");
            if (logo != null)
            {
                ViewBag.Logo = logo.GorselAdi;
            }
            else
            {
                ViewBag.Logo = "0";
            }
            var kategoriler = db.Kategori.ToList();
            return View(kategoriler);
        }

        [HttpGet]
        public ActionResult KategoriSil(int id)
        {
            var kategori = db.Kategori.FirstOrDefault(x => x.Id == id);
            if (kategori != null)
            {
                db.Kategori.Remove(kategori);
                var KategoriyeAitUrunler = db.Urun.Where(x => x.KategoriId == id).ToList();
                foreach (var item in KategoriyeAitUrunler)
                {
                    string dosyaAdi = item.urunGorselAdi;
                    var folderPath = Server.MapPath("~/Content/img/");
                    var imagePath = folderPath + dosyaAdi;
                    try
                    {
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                            Console.WriteLine("Dosya başarıyla silindi.");
                        }
                        else
                        {
                            Console.WriteLine("Dosya bulunamadı.");
                        }
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine("Dosya, başka bir işlem tarafından kullanılıyor.");
                    }
                    catch (System.UnauthorizedAccessException e)
                    {
                        Console.WriteLine("Bu işlem için gerekli izne sahip değilsiniz.");
                    }
                    db.Urun.Remove(item);
                }
                db.SaveChanges();
                TempData["basarili"] = "İşlem Başarılı";
                return RedirectToAction("Kategoriler");

            }
            else
            {
                return RedirectToAction("Kategoriler");

            }
        }


        // GÖRSEL İŞLEMLER
        public ActionResult Karosel()
        {
            var logo = db.Gorsel.FirstOrDefault(x => x.Aktif == true && x.GorselTuru == "logo");
            if (logo != null)
            {
                ViewBag.Logo = logo.GorselAdi;
            }
            else
            {
                ViewBag.Logo = "0";
            }
            var gorselKarosel = db.Gorsel.Where(x => x.GorselTuru == "karosel").ToList();
            return View(gorselKarosel);
        }

        public ActionResult GorselEkle(GorselModel.Gorsel model)
        {
            var logo = db.Gorsel.FirstOrDefault(x => x.Aktif == true && x.GorselTuru == "logo");
            if (logo != null)
            {
                ViewBag.Logo = logo.GorselAdi;
            }
            else
            {
                ViewBag.Logo = "0";
            }
            if (Request.Files.Count > 0)
            {

                string dosyaAdi = Path.GetFileName(Request.Files[0].FileName);
                var gorselKontrol = db.Gorsel.FirstOrDefault(x => x.GorselAdi == dosyaAdi);
                if (gorselKontrol == null)
                {
                    var gorsel = new GorselModel.Gorsel();

                    if (model.GorselTuru == "karosel")
                    {
                        gorsel.GorselTuru = "karosel";
                        string yol = "~/Content/img/karosel/" + dosyaAdi;
                        Request.Files[0].SaveAs(Server.MapPath(yol));
                    }
                    if (model.GorselTuru == "logo")
                    {
                        gorsel.GorselTuru = "logo";
                        string yol = "~/Content/img/logo/" + dosyaAdi;
                        Request.Files[0].SaveAs(Server.MapPath(yol));
                    }
                    gorsel.GorselAdi = dosyaAdi;
                    db.Gorsel.Add(gorsel);
                    db.SaveChanges();
                    if (model.GorselTuru == "karosel")
                    {
                        TempData["GorselEklemeBasarili"] = "Görsel eklenmiştir.";
                        return RedirectToAction("Karosel");
                    }
                    if (model.GorselTuru == "logo")
                    {
                        TempData["GorselEklemeBasarili"] = "Görsel eklenmiştir.";
                        return RedirectToAction("Logo");
                    }

                }
                else
                {
                    if (model.GorselTuru == "karosel")
                    {
                        TempData["GorselEklemeBasarisiz"] = "Görsel eklenememiştir.";
                        return RedirectToAction("Karosel");
                    }
                    if (model.GorselTuru == "logo")
                    {
                        TempData["GorselEklemeBasarisiz"] = "Görsel eklenememiştir.";
                        return RedirectToAction("Logo");
                    }


                }


            }


            else
            {
                TempData["KategoriEklemeBasarisiz"] = "Kategori mevcuttur.";
                return RedirectToAction("KategoriEkle");
            }
            return View();
        }

        [HttpGet]
        public ActionResult KaroselSec(int id)
        {
            var logo = db.Gorsel.FirstOrDefault(x => x.Aktif == true && x.GorselTuru == "logo");
            if (logo != null)
            {
                ViewBag.Logo = logo.GorselAdi;
            }
            else
            {
                ViewBag.Logo = "0";
            }
            var karosel = db.Gorsel.FirstOrDefault(x => x.Id == id);
            if (karosel != null)
            {
                karosel.Aktif = true;

                db.SaveChanges();
                TempData["basarili"] = "İşlem Başarılı";
                return RedirectToAction("Karosel");

            }
            else
            {
                return RedirectToAction("Karosel");

            }
        }

        [HttpGet]
        public ActionResult KaroselCikar(int id)
        {
            var karosel = db.Gorsel.FirstOrDefault(x => x.Id == id);
            if (karosel != null)
            {
                karosel.Aktif = false;

                db.SaveChanges();
                TempData["basarili"] = "İşlem Başarılı";
                return RedirectToAction("Karosel");

            }
            else
            {
                return RedirectToAction("Karosel");

            }
        }

        [HttpGet]
        public ActionResult KaroselSil(int id)
        {
            var karosel = db.Gorsel.FirstOrDefault(x => x.Id == id);
            if (karosel != null)
            {
                string dosyaAdi = karosel.GorselAdi;
                var folderPath = Server.MapPath("~/Content/img/karosel/");
                var imagePath = folderPath + dosyaAdi;
                try
                {
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                        Console.WriteLine("Dosya başarıyla silindi.");
                    }
                    else
                    {
                        Console.WriteLine("Dosya bulunamadı.");
                    }
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine("Dosya, başka bir işlem tarafından kullanılıyor.");
                }
                catch (System.UnauthorizedAccessException e)
                {
                    Console.WriteLine("Bu işlem için gerekli izne sahip değilsiniz.");
                }



                db.Gorsel.Remove(karosel);
                db.SaveChanges();
                TempData["basarili"] = "İşlem Başarılı";
                return RedirectToAction("Karosel");
            }
            else
            {
                TempData["basarisiz"] = "İşlem başarısız";
                return RedirectToAction("Karosel");
            }
        }

        // LOGO İŞLEMLERİ
        public ActionResult Logo()
        {
            var logo = db.Gorsel.FirstOrDefault(x => x.Aktif == true && x.GorselTuru == "logo");
            if (logo != null)
            {
                ViewBag.Logo = logo.GorselAdi;
            }
            else
            {
                ViewBag.Logo = "0";
            }
            var gorselLogo = db.Gorsel.Where(x => x.GorselTuru == "logo").ToList();
            return View(gorselLogo);
        }

        [HttpGet]
        public ActionResult LogoSec(int id)
        {
            var logoResim = db.Gorsel.FirstOrDefault(x => x.Aktif == true && x.GorselTuru == "logo");
            if (logoResim != null)
            {
                ViewBag.Logo = logoResim.GorselAdi;
            }
            else
            {
                ViewBag.Logo = "0";
            }

            var aktifLogo = db.Gorsel.Where(x => x.Aktif == true && x.GorselTuru == "logo").ToList();
            if (aktifLogo.Count() >= 1)
            {
                TempData["basarisiz"] = "Aktif logoyu kaldırmanız gerekmektedir";
                return RedirectToAction("Logo");
            }
            else
            {
                var logo = db.Gorsel.FirstOrDefault(x => x.Id == id);
                if (logo != null)
                {
                    logo.Aktif = true;

                    db.SaveChanges();
                    TempData["basarili"] = "İşlem Başarılı";
                    return RedirectToAction("Logo");

                }
                else
                {
                    return RedirectToAction("Logo");

                }
            }

        }

        [HttpGet]
        public ActionResult LogoCikar(int id)
        {

            var logo = db.Gorsel.FirstOrDefault(x => x.Id == id);
            if (logo != null)
            {
                logo.Aktif = false;

                db.SaveChanges();
                TempData["basarili"] = "İşlem Başarılı";
                return RedirectToAction("Logo");

            }
            else
            {
                return RedirectToAction("Logo");

            }
        }

        [HttpGet]
        public ActionResult LogoSil(int id)
        {
            var logo = db.Gorsel.FirstOrDefault(x => x.Id == id);
            if (logo != null)
            {
                string dosyaAdi = logo.GorselAdi;
                var folderPath = Server.MapPath("~/Content/img/logo/");
                var imagePath = folderPath + dosyaAdi;
                try
                {
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                        Console.WriteLine("Dosya başarıyla silindi.");
                    }
                    else
                    {
                        Console.WriteLine("Dosya bulunamadı.");
                    }
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine("Dosya, başka bir işlem tarafından kullanılıyor.");
                }
                catch (System.UnauthorizedAccessException e)
                {
                    Console.WriteLine("Bu işlem için gerekli izne sahip değilsiniz.");
                }



                db.Gorsel.Remove(logo);
                db.SaveChanges();
                TempData["basarili"] = "İşlem Başarılı";
                return RedirectToAction("Logo");
            }
            else
            {
                TempData["basarisiz"] = "İşlem başarısız";
                return RedirectToAction("Karosel");
            }
        }

        [AllowAnonymous]
        public ActionResult Cikis()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();
            return RedirectToAction("Index");
        }
    }
}