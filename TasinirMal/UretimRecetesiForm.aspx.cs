using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.TMM;
using Ext1.Net;
using System.Collections.Generic;
using Aspose.Words;
using System.Data;


namespace TasinirMal
{
    public partial class UretimRecetesiForm : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                AlanTemizle();
            }
        }

        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            Newtonsoft.Json.Linq.JArray detaySatirlari = (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(e.ExtraParams["SATIRLAR"]);

            ObjectArray tk = new ObjectArray();

            foreach (Newtonsoft.Json.Linq.JObject item in detaySatirlari)
            {
                UretimRecetesi detay = new UretimRecetesi();

                double miktar = TasinirGenel.DegerAlDbl(item, "MIKTAR");

                if (miktar == 0)
                    detay.miktar = 1;
                else
                    detay.miktar = miktar;

                detay.anaUrun.hesapKod = OrtakFonksiyonlar.ConvertToStr(ddlAnaUrunKod.Value);
                detay.altUrun.hesapKod = TasinirGenel.DegerAlStr(item, "ALTURUNKOD");

                if (detay.altUrun.hesapKod.Trim() == "") continue;

                tk.objeler.Add(detay);
            }

            if (tk.ObjeSayisi == 0)
            {
                GenelIslemler.MesajKutusu("Hata", "Kayıt edilecek bilgi yok");
                return;
            }

            Sonuc sonuc = servisTMM.UretimRecetesiKaydet(kullanan, tk);

            if (sonuc.islemSonuc)
            {
                UretimRecetesi tkl = new UretimRecetesi();
                tkl.anaUrun.hesapKod = OrtakFonksiyonlar.ConvertToStr(ddlAnaUrunKod.Value);
                Listele(tkl);
                GenelIslemler.ExtNotification(Resources.TasinirMal.FRMSYG017, "Bilgi", Icon.Information);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);

        }

        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            AlanTemizle();
        }

        protected void btnListele_Click(object sender, DirectEventArgs e)
        {
            UretimRecetesi tk = new UretimRecetesi();
            tk.anaUrun.hesapKod = OrtakFonksiyonlar.ConvertToStr(ddlAnaUrunKod.Value);

            Listele(tk);
        }

        private void AlanTemizle()
        {
            List<object> listeDetay = new List<object>();

            for (int i = 0; i < 20; i++)
            {
                listeDetay.Add(new
                {
                    ALTURUNKOD = "",
                    ALTURUNADI = "",
                    MIKTAR = 0,
                });

            }
            ddlAnaUrunKod.Clear();
            lblAnaUrunAdi.Clear();

            strListe.DataSource = listeDetay;
            strListe.DataBind();
        }

        public void Listele(UretimRecetesi tk)
        {
            List<object> listeDetay = new List<object>();

            if (!string.IsNullOrWhiteSpace(tk.anaUrun.hesapKod.Replace(".", "").Trim()))
            {
                ObjectArray bilgi = servisTMM.UretimRecetesiListele(kullanan, tk);

                if (!bilgi.sonuc.islemSonuc || bilgi.objeler.Count == 0)
                {
                    GenelIslemler.MesajKutusu("Uyarı", "Listelenecek kayıt bulunamadı." + bilgi.sonuc.hataStr);
                    return;
                }

                foreach (UretimRecetesi detay in bilgi.objeler)
                {
                    lblAnaUrunAdi.SetValue(detay.anaUrun.aciklama);

                    listeDetay.Add(new
                    {
                        ALTURUNKOD = detay.altUrun.hesapKod,
                        ALTURUNADI = detay.altUrun.aciklama,
                        MIKTAR = detay.miktar,
                    });
                }
            }

            for (int i = 0; i < 10; i++)
            {
                listeDetay.Add(new
                {
                    ALTURUNKOD = "",
                    ALTURUNADI = "",
                    MIKTAR = 0,
                });

            }

            strListe.DataSource = listeDetay;
            strListe.DataBind();
        }

        protected void HesapStore_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Parameters["query"]))
                return;

            List<object> liste = HesapListesiDoldur(e.Parameters["query"]);

            e.Total = 0;
            if (liste != null && liste.Count != 0)
            {
                var limit = e.Limit;
                if ((e.Start + e.Limit) > liste.Count)
                    limit = liste.Count - e.Start;

                e.Total = liste.Count;
                List<object> rangeData = (e.Start < 0 || limit < 0) ? liste : liste.GetRange(e.Start, limit);
                strHesapPlan.DataSource = (object[])rangeData.ToArray();
                strHesapPlan.DataBind();
            }
            else
            {
                strHesapPlan.DataSource = new object[] { };
                strHesapPlan.DataBind();
            }
        }

        List<object> HesapListesiDoldur(string kriter)
        {
            HesapPlaniSatir h = new HesapPlaniSatir();

            h.hesapKodAciklama = kriter;
            h.detay = true;
            ObjectArray hesap = servisTMM.HesapPlaniListele(kullanan, h, new Sayfalama());

            List<object> liste = new List<object>();
            foreach (HesapPlaniSatir detay in hesap.objeler)
            {
                liste.Add(new
                {
                    hesapPlanKod = detay.hesapKod,
                    hesapPlanAd = detay.aciklama,
                    olcuBirimAd = detay.olcuBirimAd,
                    kdvOran = detay.kdv,
                    rfidEtiketKod = detay.rfidEtiketKod,
                    markaKod = detay.markaKod,
                    modelKod = detay.modelKod,
                    vurgula = detay.vurgula
                });
            }
            return liste;
        }

        protected void btnYazdir_Click(object sender, DirectEventArgs e)
        {
            UretimRecetesi tk = new UretimRecetesi();
            tk.anaUrun.hesapKod = OrtakFonksiyonlar.ConvertToStr(ddlAnaUrunKod.Value);

            if (string.IsNullOrEmpty(tk.anaUrun.hesapKod))
            {
                GenelIslemler.MesajKutusu("Uyarı", "Lütfen reçete seçiniz.");
                return;
            }

            string temp = System.IO.Path.GetTempFileName();

            string sablonAd = Server.MapPath("~") + "/RaporSablon/TMM/UretimReceteFormu.dotx";
            Document doc = new Document(sablonAd);
            DocumentBuilder docBuilder = new DocumentBuilder(doc);

            ObjectArray bilgiler = servisTMM.UretimRecetesiListele(kullanan, tk);

            DataTable dt = new DataTable();
            dt.TableName = "Detay";
            dt.Columns.Add("altHesapKodu");
            dt.Columns.Add("hesapAdi");
            dt.Columns.Add("miktar");

            string anaUrunKod = "";
            string anaUrunAdi = "";
            foreach (UretimRecetesi b in bilgiler.objeler)
            {
                anaUrunKod = b.anaUrun.hesapKod;
                anaUrunAdi = b.anaUrun.aciklama;

                dt.Rows.Add(b.altUrun.hesapKod, b.altUrun.aciklama, b.miktar.ToString("#,###.##"));
            }

            if (dt.Rows.Count == 0)
                dt.Rows.Add("", "", "");

            TasinirGenel.AsposeAlanaYaz(docBuilder, "hesapKodu", anaUrunKod);
            TasinirGenel.AsposeAlanaYaz(docBuilder, "aciklama", anaUrunAdi);

            doc.MailMerge.ExecuteWithRegions(dt);

            doc.Save(temp, Aspose.Words.SaveFormat.Pdf);
            OrtakClass.DosyaIslem.DosyaGonder(temp, "UretimRecetesi_" + anaUrunKod.Replace(".", ""), true, "pdf");
        }
    }

}