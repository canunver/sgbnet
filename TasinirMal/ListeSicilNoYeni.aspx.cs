using System;
using System.Data;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.TMM;
using TNS.UZY;
using Ext1.Net;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Text;
using Newtonsoft.Json.Linq;
using Arac = TNS.TMM.Arac;

namespace TasinirMal
{
    public partial class ListeSicilNoYeni : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = "Seçim Formu";
                SayfaUstAltBolumYaz(this);
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                string iliskiliMalzemeEkle = Request.QueryString["iliskiliMalzemeEkle"] + "";
                //if (iliskiliMalzemeEkle == "1" && TNS.TMM.Arac.MerkezBankasiKullaniyor())
                //    hdnIliskiliMalzemeEkle.Value = 1;


                hdnCagiran.Value = Request.QueryString["cagiran"] + "";
                hdnCagiranLabel.Value = Request.QueryString["cagiranLabel"] + "";

                pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());
                if (Request.QueryString["mb"] + "" != "")
                    pgFiltre.UpdateProperty("prpMuhasebe", Request.QueryString["mb"] + "");
                else
                    pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));

                if (Request.QueryString["hb"] + "" != "")
                    pgFiltre.UpdateProperty("prpHarcamaBirimi", Request.QueryString["hb"] + "");
                else
                    pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));

                string ambar = Request.QueryString["ak"] + "";
                pgFiltre.UpdateProperty("prpAmbar", ambar);
                if (Arac.MerkezBankasiKullaniyor() && Path.GetFileName(Request.UrlReferrer.AbsolutePath).ToLower() != "sicilnodegerartisform.aspx")
                    pgFiltre.Source["prpAmbar"].Editor.Editor.ReadOnly = true;


                if (Request.QueryString["hk"] + "" != "")
                    pgFiltre.UpdateProperty("prpHesapKod", Request.QueryString["hk"] + "");

                string listeTur = Request.QueryString["listeTur"] + "";
                if (listeTur == "ambar")
                {
                    pgFiltre.UpdateProperty("prpDurumKod", "1");
                    pgFiltre.Source["prpDurumKod"].Editor.Editor.ReadOnly = true;
                }
                else if (listeTur.StartsWith("zimmet"))
                {
                    string kisiKod = Request.QueryString["kk"] + "";
                    if (string.IsNullOrEmpty(kisiKod))
                    {
                        GenelIslemler.MesajKutusu("Uyarý", "Kiþi bilgisi alýnamadý. Lütfen kiþi bilgisini doldurduktan sonra listeleme yapýn.");
                        return;
                    }

                    if (listeTur == "zimmetORT") hdnZimmetTur.Value = "ORT";
                    else hdnZimmetTur.Value = "KISI";

                    pgFiltre.UpdateProperty("prpDurumKod", "2");
                    pgFiltre.UpdateProperty("prpKisiKod", Request.QueryString["kk"] + "");
                    pgFiltre.UpdateProperty("prpOdaKod", Request.QueryString["ok"] + "");
                    pgFiltre.Source["prpDurumKod"].Editor.Editor.ReadOnly = true;
                    pgFiltre.Source["prpKisiKod"].Editor.Editor.ReadOnly = true;
                    //pgFiltre.Source["prpOdaKod"].Editor.Editor.ReadOnly = true;
                }

                if (listeTur != "")
                {
                    btnIndir.Hide();
                    btnYazdir.Hide();

                    pgFiltre.Source["prpDurumKod"].Editor.Editor.Hide();
                    grdListe.ColumnModel.Columns[0].Hidden = true;
                    grdListe.ColumnModel.Columns[9].Hidden = true;
                    grdListe.ColumnModel.Columns[10].Hidden = true;
                }
                else
                    btnSecKapat.Hide();
            }
        }

        protected void btnIndir_Click(object sender, DirectEventArgs e)
        {
            SicilNoHareket shBilgi = KriterTopla();
            ObjectArray bilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, new Sayfalama());

            Tablo XLS = GenelIslemler.NewTablo();
            string sablonAd = "TopluIslem.xlt";
            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string raporSablonYol = System.Configuration.ConfigurationManager.AppSettings.Get("RaporSablonYol");
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            int satir = 1;

            string hedefMuhasebe = pgFiltre.Source["prpGonMuhasebe"].Value.Trim();
            string hedefHarcamaBirimi = pgFiltre.Source["prpGonHarcamaBirimi"].Value.Trim().Replace(".", "");
            string hedefAmbar = pgFiltre.Source["prpGonAmbar"].Value.Trim();

            foreach (SicilNoHareket sh in bilgi.objeler)
            {
                XLS.HucreDegerYaz(satir, 0, OrtakFonksiyonlar.ConvertToStr(sh.sicilNo));
                XLS.HucreDegerYaz(satir, 1, OrtakFonksiyonlar.ConvertToStr(sh.muhasebeKod));
                XLS.HucreDegerYaz(satir, 2, OrtakFonksiyonlar.ConvertToStr(sh.harcamaBirimKod));
                XLS.HucreDegerYaz(satir, 3, OrtakFonksiyonlar.ConvertToStr(sh.ambarKod));
                XLS.HucreDegerYaz(satir, 4, OrtakFonksiyonlar.ConvertToStr(sh.kisiAd));
                XLS.HucreDegerYaz(satir, 5, OrtakFonksiyonlar.ConvertToStr(sh.odaKod));

                XLS.HucreDegerYaz(satir, 6, OrtakFonksiyonlar.ConvertToStr(hedefMuhasebe));
                XLS.HucreDegerYaz(satir, 7, OrtakFonksiyonlar.ConvertToStr(hedefHarcamaBirimi));
                XLS.HucreDegerYaz(satir, 8, OrtakFonksiyonlar.ConvertToStr(hedefAmbar));

                XLS.HucreDegerYaz(satir, 9, OrtakFonksiyonlar.ConvertToStr(sh.kisiAd));
                XLS.HucreDegerYaz(satir, 10, OrtakFonksiyonlar.ConvertToStr(sh.odaKod));

                satir++;
            }

            XLS.SatirYukseklikAyarla(1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        protected void btnListe_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel sm = grdListe.SelectionModel.Primary as RowSelectionModel;
            if (sm != null)
            {
                sm.SelectedRows.Clear();
                sm.UpdateSelection();
            }
            PagingToolbar1.PageSize = OrtakFonksiyonlar.ConvertToInt(cmbPageSize.Text, 0);
            OturumBilgisiIslem.BilgiYazDegisken("ListeSicilNo", KriterTopla());
        }

        private SicilNoHareket KriterTopla()
        {
            SicilNoHareket shBilgi = new SicilNoHareket();

            shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpYil"].Value, 0);
            shBilgi.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            shBilgi.harcamaBirimKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            shBilgi.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            //shBilgi.fisNo = pgFiltre.Source["prpBelgeNo"].Value == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNo"].Value.Trim().PadLeft(6, '0');
            shBilgi.sorguFisNoTif = pgFiltre.Source["prpBelgeNoTif"].Value == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNoTif"].Value.Trim().PadLeft(6, '0');
            shBilgi.sorguFisNoZimmet = pgFiltre.Source["prpBelgeNoZimmet"].Value == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNoZimmet"].Value.Trim().PadLeft(6, '0');
            shBilgi.sicilNo = pgFiltre.Source["prpSicilNo"].Value.Trim();
            shBilgi.hesapPlanKod = pgFiltre.Source["prpHesapKod"].Value.Trim();
            shBilgi.hesapPlanAd = pgFiltre.Source["prpHesapAdi"].Value.Trim();
            shBilgi.kimeGitti = pgFiltre.Source["prpKisiKod"].Value.Trim();
            shBilgi.nereyeGitti = pgFiltre.Source["prpOdaKod"].Value.Trim();

            shBilgi.gonMuhasebeKod = pgFiltre.Source["prpGonMuhasebe"].Value.Trim();
            shBilgi.gonHarcamaBirimKod = pgFiltre.Source["prpGonHarcamaBirimi"].Value.Trim().Replace(".", "");
            shBilgi.gonAmbarKod = pgFiltre.Source["prpGonAmbar"].Value.Trim();

            shBilgi.ozellik.markaKod = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpMarkaKod"].Value.Trim(), 0);
            shBilgi.ozellik.modelKod = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpModelKod"].Value.Trim(), 0);
            shBilgi.ozellik.saseNo = pgFiltre.Source["prpSeriNo"].Value.Trim();
            shBilgi.ozellik.plaka = pgFiltre.Source["prpPlaka"].Value.Trim();
            shBilgi.ozellik.adi = pgFiltre.Source["prpEserYayinAdi"].Value.Trim();
            shBilgi.ozellik.disSicilNo = pgFiltre.Source["prpEskiSicilNo"].Value.Trim();

            shBilgi.durum = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpDurumKod"].Value.Trim(), 0);

            return shBilgi;
        }

        private int SicilNumarasiDoldur(SicilNoHareket shBilgi, Sayfalama sayfa)
        {

            string sicilNo = OrtakFonksiyonlar.ConvertToStr(OturumBilgisiIslem.BilgiOkuDegisken("ZIMMETSICILNOLAR", false));//Request.QueryString["sicilNoListe"] + "";
            OturumBilgisiIslem.BilgiYazDegisken("ZIMMETSICILNOLAR", "");

            if (!string.IsNullOrEmpty(sicilNo))
                hdnEngelliSicilNolar.Value = sicilNo;
            else
                sicilNo = hdnEngelliSicilNolar.Text;//Önceki listelemede zimmet ekranýndan gelen sicil no var ise

            shBilgi.sorguHaricPrSicilNoListe = sicilNo;
            //string[] sicilNoLar = sicilNo.Replace("'","").Split(',');

            ObjectArray bilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, sayfa);

            TNSCollection prSiciller = new TNSCollection();
            if (bilgi.objeler.Count == 0)
                GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMBRK002);

            DataTable dt = new DataTable();
            dt.Columns.Add("tip");
            dt.Columns.Add("prSicilNo");
            dt.Columns.Add("sicilno");
            dt.Columns.Add("kod");
            dt.Columns.Add("ad");
            dt.Columns.Add("kimeGitti");
            dt.Columns.Add("oda");
            dt.Columns.Add("kaynakMuhasebe");
            dt.Columns.Add("kaynakHB");
            dt.Columns.Add("kaynakAmbar");
            dt.Columns.Add("kaynakTC");
            dt.Columns.Add("kaynakOda");
            dt.Columns.Add("eskiSicilNo");
            dt.Columns.Add("kdv");
            dt.Columns.Add("birimFiyat", typeof(decimal));
            dt.Columns.Add("marka");
            dt.Columns.Add("model");
            dt.Columns.Add("saseNo");
            dt.Columns.Add("islemTipAdi");
            dt.Columns.Add("islemDurumAciklama");
            dt.Columns.Add("amortismanOran", typeof(double));
            dt.Columns.Add("alimTarihi");
            dt.Columns.Add("islemTarih");


            int eklenenSatir = 0;
            double amortismanOran = 0.0;
            foreach (SicilNoHareket sh in bilgi.objeler)
            {
                string ozellik = "";

                if (!String.IsNullOrEmpty(sh.ozellik.markaAd))
                    ozellik = sh.ozellik.markaAd;
                if (!String.IsNullOrEmpty(sh.ozellik.modelAd))
                {
                    if (ozellik != "") ozellik += "-";
                    ozellik += sh.ozellik.modelAd;
                }
                if (!String.IsNullOrEmpty(sh.ozellik.plaka))
                {
                    if (ozellik != "") ozellik += "-";
                    ozellik += sh.ozellik.plaka;
                }
                if (!String.IsNullOrEmpty(sh.ozellik.adi))
                {
                    if (ozellik != "") ozellik += "-";
                    ozellik += sh.ozellik.adi;
                }
                if (!String.IsNullOrEmpty(sh.ozellik.yeriKonusu))
                {
                    if (ozellik != "") ozellik += "-";
                    ozellik += sh.ozellik.yeriKonusu;
                }
                if (!String.IsNullOrEmpty(sh.ozellik.saseNo))
                {
                    if (ozellik != "") ozellik += "-";
                    ozellik += sh.ozellik.saseNo;
                }

                if (!String.IsNullOrEmpty(sh.ozellik.ekNo))
                {
                    if (ozellik != "") ozellik += "-";
                    ozellik += sh.ozellik.ekNo;
                }

                //if (ozellik != "") ozellik = " (" + ozellik + ")";

                string tip = "TÝF";
                if (sh.kisiAd != "")
                    tip = "ZÝMMET";

                //bool varMi = false;
                //if (sicilNo != "")
                //{
                //    for (int i = 0; i < sicilNoLar.Length; i++)
                //    {
                //        if (sicilNoLar[i] == sh.sicilNo) { varMi = true; break; }
                //    }
                //}
                //if (varMi) continue;

                if (hdnZimmetTur.Value.ToString() == "ORT" && sh.islemTipiAd.IndexOf("Kiþi") > -1) continue;
                if (hdnZimmetTur.Value.ToString() == "KISI" && sh.islemTipiAd.IndexOf("Ortak") > -1) continue;

                if (sh.amortismanYil > 0) amortismanOran = Math.Round(100 / sh.amortismanYil, 2);
                else amortismanOran = 0;

                dt.Rows.Add(tip, sh.prSicilNo, sh.sicilNo, sh.hesapPlanKod, sh.hesapPlanAd, sh.kimeGitti, sh.odaAd, sh.muhasebeKod, sh.harcamaBirimKod, sh.ambarKod, sh.kisiAd, sh.odaKod, sh.ozellik.disSicilNo, sh.kdvOran, sh.fiyat, sh.ozellik.markaAd, sh.ozellik.modelAd, ozellik, sh.islemTipiAd, sh.islemDurumAciklama, (decimal)amortismanOran, sh.ozellik.hesapTarih, sh.islemTarih.Oku().ToString("dd.MM.yyyy HH:mm:ss"));
                prSiciller.Add(sh.prSicilNo);
                eklenenSatir++;
            }

            strListe.DataSource = dt;
            strListe.DataBind();

            return bilgi.sonuc.kayitSay;
        }

        protected void strListe_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            SicilNoHareket kriter = (SicilNoHareket)OturumBilgisiIslem.BilgiOkuDegisken("ListeSicilNo", false);
            if (kriter == null)
                return;

            Sayfalama sayfa = new Sayfalama();

            sayfa.sayfaNo = (e.Start / e.Limit) + 1;
            sayfa.kayitSayisi = e.Limit;
            sayfa.siralamaAlani = e.Sort;
            if (e.Dir == Ext1.Net.SortDirection.Default || e.Dir == Ext1.Net.SortDirection.ASC)
                sayfa.siralamaYon = "ASC";
            else
                sayfa.siralamaYon = "DESC";

            if (sayfa.siralamaAlani == "prSicilNo") sayfa.siralamaAlani = "PRSICILNO";
            else if (sayfa.siralamaAlani == "sicilno") sayfa.siralamaAlani = "SICILNO + 0";
            else if (sayfa.siralamaAlani == "kod") sayfa.siralamaAlani = "HESAPPLANKOD";
            else if (sayfa.siralamaAlani == "ad") sayfa.siralamaAlani = "HESAPPLANAD";
            else if (sayfa.siralamaAlani == "kimeGitti") sayfa.siralamaAlani = "KIMEGITTI";
            else if (sayfa.siralamaAlani == "kdv") sayfa.siralamaAlani = "KDVORAN";
            else if (sayfa.siralamaAlani == "birimFiyat") sayfa.siralamaAlani = "FIYAT";
            else if (sayfa.siralamaAlani == "oda") sayfa.siralamaAlani = "ODAAD";
            else if (sayfa.siralamaAlani == "marka") sayfa.siralamaAlani = "MARKAAD";
            else if (sayfa.siralamaAlani == "model") sayfa.siralamaAlani = "MODELAD";
            else if (sayfa.siralamaAlani == "islemTarih") sayfa.siralamaAlani = "ISLEMTARIH";
            else
                sayfa.siralamaAlani = "SICILNO";

            int kayitSayisi = SicilNumarasiDoldur(kriter, sayfa);
            e.Total = kayitSayisi;

            if (kayitSayisi == 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMBRK002);
            }
        }

        protected void btnYazdir_Click(object sender, DirectEventArgs e)
        {
            string rapor = e.ExtraParams["RAPORBILGI"];
            if (!string.IsNullOrEmpty(rapor))
            {
                Tablo XLS = GenelIslemler.NewTablo();
                string sablonAd = "TopluIslem.xlt";
                string sonucDosyaAd = System.IO.Path.GetTempFileName();
                string raporSablonYol = System.Configuration.ConfigurationManager.AppSettings.Get("RaporSablonYol");
                XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

                int satir = 1;

                foreach (Newtonsoft.Json.Linq.JContainer jc in (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(rapor))
                {
                    XLS.HucreDegerYaz(satir, 0, OrtakFonksiyonlar.ConvertToStr(jc.Value<string>("sicilno")));
                    XLS.HucreDegerYaz(satir, 1, OrtakFonksiyonlar.ConvertToStr(jc.Value<string>("kaynakMuhasebe")));
                    XLS.HucreDegerYaz(satir, 2, OrtakFonksiyonlar.ConvertToStr(jc.Value<string>("kaynakHB")));
                    XLS.HucreDegerYaz(satir, 3, OrtakFonksiyonlar.ConvertToStr(jc.Value<string>("kaynakAmbar")));
                    XLS.HucreDegerYaz(satir, 4, OrtakFonksiyonlar.ConvertToStr(jc.Value<string>("kaynakTC")));
                    XLS.HucreDegerYaz(satir, 5, OrtakFonksiyonlar.ConvertToStr(jc.Value<string>("kaynakOda")));

                    satir++;
                }

                XLS.SatirYukseklikAyarla(1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
        }


        [DirectMethod]
        public void IliskiliMalzemelerListeyeEklensinMi(JArray liste)
        {
            List<int> prSicilNoListe = new List<int>();
            foreach (Newtonsoft.Json.Linq.JContainer jc in liste)
            {
                int prSicilNo = jc.Value<int>("prSicilNo");
                if (prSicilNo > 0)
                    prSicilNoListe.Add(prSicilNo);
            }

            int adet = servisTMM.IliskiliMalzemeSayisi(kullanan, prSicilNoListe.ToArray());  //IliskiliMalzemeVarMi
            if (adet > 0)
            {
                X.AddScript("extKontrol.getBody().unmask(); Ext1.Msg.confirm(\"Onay\", " + JSON.Serialize("Seçilen malzemelerin iliþkili olduðu malzemeler var. Bu malzemelerde listeye eklensin mi?") +
                    ", function (btn) {  if (btn == \"yes\") { Ext1.net.DirectMethods.IliskiliMalzemeleriListeyeEkle(" + JSON.Serialize(prSicilNoListe.ToArray()) +
                    ",{eventMask: { showMask: true }, timeout: 60000, success: function (script) { MaskAktar(); eval(script); setTimeout('GrdSatirYaz();', 100); }, failure: function (errorMsg) { Ext1.Msg.alert('Hata Oluþtu', errorMsg); } }) } " +
                    " else { MaskAktar();setTimeout('GrdSatirYaz();', 100); } });");
            }
            else
                X.AddScript("GrdSatirYaz();");
        }

        [DirectMethod]
        public string IliskiliMalzemeleriListeyeEkle(int[] prSicilNoliste)
        {
            string script = "";

            ObjectArray liste = servisTMM.IliskiliMalzemeListele(kullanan, prSicilNoliste);
            if (liste.sonuc.islemSonuc)
            {
                foreach (SicilNoHareket item in liste.objeler)
                {
                    JsonObject j = new JsonObject();
                    j.Add("gorSicilNo", item.sicilNo);
                    j.Add("hesapPlanKod", item.hesapPlanKod);
                    j.Add("hesapPlanAd", item.hesapPlanAd);
                    j.Add("kdvOran", item.kdvOran);
                    j.Add("birimFiyat", item.fiyat);
                    j.Add("miktar", 1);
                    j.Add("prSicilNo", item.prSicilNo);

                    script += "liste.push(" + JSON.Serialize(j) + ");";
                }
            }
            else
                GenelIslemler.MesajKutusu("Hata", liste.sonuc.hataStr);

            return script;
        }

    }
}