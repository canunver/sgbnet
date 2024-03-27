using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using TNS.KYM;
using TNS.UZY;

namespace TasinirMal
{
    public class AdGetir : IHttpHandler, IRequiresSessionState
    {
        static IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        public void ProcessRequest(HttpContext context)
        {
            Kullanici kullanan = OrtakClass.OturumBilgisiIslem.KullaniciBilgiOku(true);

            context.Response.ContentType = "application/json";

            string kod = context.Request.QueryString["kod"] + "";
            string adi = KodAdGetir(kullanan, context, kod);

            context.Response.Write(adi);
            context.Response.End();
        }

        private string KodAdGetir(Kullanici kullanan, HttpContext context, string kod)
        {
            string alanDeger = context.Request.QueryString["alanDeger"] + "";
            string uzayAd = "";
            string uzayAlan = "";
            string deger = "";
            string kosul = "";
            bool esitle = true;

            string[] alanDegerleri = alanDeger.Split('|');
            if (alanDegerleri.Length > 0) kosul = alanDegerleri[0];

            if (kod == "MUHASEBE")
            {
                uzayAd = "TASMUHASEBE";
            }
            else if (kod == "HARCAMABIRIMI")
            {
                uzayAd = "TASHARCAMABIRIM";
                if (alanDegerleri.Length > 1) kosul += "-" + alanDegerleri[1];//hbkod
            }
            else if (kod == "AMBAR")
            {
                uzayAd = "TASAMBAR";
                if (alanDegerleri.Length > 1) kosul += "-" + alanDegerleri[1];//hbkod
                if (alanDegerleri.Length > 2) kosul += "-" + alanDegerleri[2];//ambarkod
            }
            else if (kod == "OLCUBIRIM")
            {
                uzayAd = "TASOLCUBIRIMAD";
            }
            else if (kod == "ODA")
            {
                uzayAd = "TASODA";
                if (alanDegerleri.Length > 1) kosul += "-";// + alanDegerleri[1];//hbkod harcama birimkod boş olsun. Merkez Bankasında birim bazlı oda sorun yaratıyor Melih 01.03.2018
                if (alanDegerleri.Length > 2) kosul += "-" + alanDegerleri[2];//odaKod
            }
            else if (kod == "PERSONEL")
            {
                uzayAd = "TASPERSONEL";
            }
            else if (kod == "MARKA")
            {
                uzayAd = "TASMARKA";
            }
            else if (kod == "MODEL")
            {
                uzayAd = "TASMODEL";
                if (alanDegerleri.Length > 1) kosul += "-" + alanDegerleri[1];//marka
            }
            else if (kod == "BOLGE")
            {
                uzayAd = "TASBOLGE";
            }
            else if (kod == "HESAPPLANI")
            {
                uzayAd = "TASHESAPPLAN";
            }

            if (string.IsNullOrEmpty(kosul)) return "";

            deger = servisUZY.UzayDegeriStr(kullanan, uzayAd, kosul, esitle, uzayAlan);

            if (deger.IndexOf("<!DOCTYPE") > -1) deger = "";

            return deger.Replace(",", " ");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}