using AzimYapi.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AzimYapi.Models;
namespace AzimYapi.Controllers
{
    public class HomeController : Controller
    {
        DataContext db = new DataContext();
        public ActionResult Index()
        {
            var urunler = db.Urun.Where(x => x.urunGorselAdi != null).ToList();
            List<UrunlerModel.Urun> enCokSatanlarList = new List<UrunlerModel.Urun>();

            var enCokSatanlar = db.Urun.Where(x => x.enCokSatan == true).ToList();
            if (enCokSatanlar.Count() == 5)
            {
                ViewBag.sayac = 5;
                foreach (var item in enCokSatanlar)
                {
                    enCokSatanlarList.Add(item);
                }

                ViewBag.EnCokSatanlar = enCokSatanlarList;
            }
            else
            {
                ViewBag.sayac = 0;
            }

            
            //index sayfasına kategorileri gönderme işlemi
            var kategoriler = db.Kategori.ToList();
            if(kategoriler!=null)
            {
                ViewBag.Kategoriler = kategoriler;
            }

            // KAROSELDE OLACAK GÖRSELLER
            var karoselGorselleri = new List<GorselModel.Gorsel>();
            var karoselGorseller = db.Gorsel.Where(x => x.Aktif == true && x.GorselTuru=="karosel").ToList();
            if (karoselGorseller != null)
            {
                foreach (var item in karoselGorseller)
                {
                    karoselGorselleri.Add(item);
                }
            }
            ViewBag.KaroselGorseller = karoselGorselleri;


            // LOGO GÖNDERİMİ

            var logo = db.Gorsel.FirstOrDefault(x => x.Aktif == true && x.GorselTuru == "logo");
            if(logo!=null)
            {
                ViewBag.Logo = logo.GorselAdi;
            }
            else
            {
                ViewBag.Logo = "0";
            }
            return View(urunler);

            //UrunlerModel.Urun urun = new UrunlerModel.Urun();
            //urun.UrunAdi = "a";
            //urun.fiyat = 20;
            //db.Urun.Add(urun);
            //db.SaveChanges();

        }

        

        [HttpGet]
        public ActionResult Iletisim()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}