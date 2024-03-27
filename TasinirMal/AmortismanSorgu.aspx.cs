using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Web.UI;
using TNS;
using TNS.KYM;
using TNS.TMM;
using OrtakClass;
using Ext1.Net;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TasinirMal
{
    /// <summary>
    /// Web formu ile ilgili olaylarý (event) ve fonksiyonlarý tutan sýnýf
    /// </summary>
    public partial class AmortismanSorgu : TMMSayfaV2
    {
        ITMMServis servisTMM = null;

        /// <summary>
        /// Sayfa hazýrlanýrken, çaðrýlan olay (event) fonksiyon.
        /// </summary>
        /// <param name="sender">Olayý uyandýran nesne</param>
        /// <param name="e">Olayýn parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            servisTMM = TNS.TMM.Arac.Tanimla();
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.Amortisman001;
                yardimBag = yardimYol + "#YardimDosyasiAd";
                //kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
                OrtakClass.GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);
                SayfaUstAltBolumYaz(this);
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                if (Request.QueryString["menuYok"] == "1")
                {
                    pnlAna.Margins = "0 0 0 0";
                    pnlAna.Border = false;
                }

                string harcamaBirimi = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");

                pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                pgFiltre.UpdateProperty("prpHarcamaBirimi", harcamaBirimi);
                pgFiltre.UpdateProperty("prpAmbar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR"));

                AmortismanIslemForm kriter = new AmortismanIslemForm();
                kriter.yil = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"], 0);
                kriter.donem = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["donem"], 0);
                kriter.muhasebeKod = Request.QueryString["muhasebeKod"] + "";
                kriter.harcamaKod = Request.QueryString["harcamaKod"] + "";
                kriter.ambarKod = Request.QueryString["ambarKod"] + "";

                if (kriter.yil > 0)
                    pgFiltre.UpdateProperty("prpYil", kriter.yil);
                if (kriter.donem > 0)
                    pgFiltre.UpdateProperty("prpDonem", kriter.donem);
                if (!string.IsNullOrEmpty(kriter.muhasebeKod))
                    pgFiltre.UpdateProperty("prpMuhasebe", kriter.muhasebeKod);
                if (!string.IsNullOrEmpty(kriter.harcamaKod))
                    pgFiltre.UpdateProperty("prpHarcamaBirimi", kriter.harcamaKod);
                if (!string.IsNullOrEmpty(kriter.ambarKod))
                    pgFiltre.UpdateProperty("prpAmbar", kriter.ambarKod);

                DonemListele("", harcamaBirimi);

                Listele(kriter);

                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    //btnBelgeOnayKaldir.Hidden = false;
                    //mnuMuhasebat.Hidden = true;
                }

            }
        }

        [DirectMethod]
        public void DonemListele(string harcamaBirimiEski, string harcamaBirimi)
        {
            int varsayilanYil = DateTime.Now.Year;
            int varsayilanDonem = 12;

            List<Object> donemListe = new List<object>();

            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                if (harcamaBirimi.Replace(".", "") == "010") //Banknot birimi ise aylýk
                {
                    for (int i = 1; i <= 12; i++)
                        donemListe.Add(new { KOD = i, ADI = i });

                    varsayilanDonem = DateTime.Now.AddMonths(-1).Month;
                    varsayilanYil = DateTime.Now.AddMonths(-1).Year;
                }
                else //Diðer birimler 3'er aylýk
                {
                    for (int i = 3; i <= 12; i += 3)
                        donemListe.Add(new { KOD = i, ADI = i });

                    if (DateTime.Now.Month <= 3)
                    {
                        varsayilanYil -= 1;
                        varsayilanDonem = 12;
                    }
                    else if (DateTime.Now.Month <= 6)
                        varsayilanDonem = 3;
                    else if (DateTime.Now.Month <= 9)
                        varsayilanDonem = 6;
                    else if (DateTime.Now.Month <= 12)
                        varsayilanDonem = 9;
                }

            }
            else
                donemListe.Add(new { KOD = 12, ADI = 12 });

            strDonem.DataSource = donemListe;
            strDonem.DataBind();

            if (string.IsNullOrWhiteSpace(harcamaBirimiEski))
            {
                if (string.IsNullOrEmpty(Request.QueryString["yil"] + ""))
                    pgFiltre.UpdateProperty("prpYil", varsayilanYil);
                if (string.IsNullOrEmpty(Request.QueryString["donem"] + ""))
                    pgFiltre.UpdateProperty("prpDonem", varsayilanDonem);
            }
        }

        private AmortismanIslemForm KriterOku()
        {
            AmortismanIslemForm form = new AmortismanIslemForm
            {
                yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpYil"].Value.Trim(), 0),
                donem = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpDonem"].Value.Trim(), 0),
                muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim(),
                harcamaKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim(),
                ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim(),
            };

            return form;
        }

        protected void btnOku_Click(object sender, DirectEventArgs e)
        {
            Listele(KriterOku());
        }

        private void Listele(AmortismanIslemForm kriter)
        {
            ObjectArray formlar = servisTMM.AmortismanIslemFisiListele(kullanan, kriter);

            List<object> liste = new List<object>();

            if (formlar.sonuc.islemSonuc)
            {
                foreach (AmortismanIslemForm item in formlar.objeler)
                {
                    string durum = "";
                    string onayDurum = "Onaysýz";

                    if (item.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIB)
                        onayDurum = "B Onayýna Gönderildi";
                    else if (item.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIA)
                        onayDurum = "A Onayýna Gönderildi";
                    else if (item.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI)
                        onayDurum = "Onaylandý";
                    else if (item.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GERIGONDERILDI)
                        onayDurum = "Ýptal";

                    if (item.durum == (int)ENUMAmortismanFormDurumu.ONAYLI)
                        durum = "Onaylý";
                    else if (item.durum == (int)ENUMAmortismanFormDurumu.ONAYKALDIR)
                        durum = "Onay Kaldýr";
                    else if (item.durum == (int)ENUMAmortismanFormDurumu.IPTAL)
                        durum = "Ýptal";
                    else if (item.durum == (int)ENUMAmortismanFormDurumu.DEGISTIRILDI)
                        durum = "Deðiþtirildi";
                    else if (item.durum == (int)ENUMAmortismanFormDurumu.TANIMSIZ)
                        durum = "Tanýmsýz";
                    else if (item.durum == (int)ENUMAmortismanFormDurumu.YENI)
                        durum = "Yeni";

                    liste.Add(new
                    {
                        kod = item.kod,
                        yil = item.yil,
                        donem = item.donem,
                        muhasebeKod = item.muhasebeKod,
                        harcamaKod = item.harcamaKod,
                        fisNo = item.fisNo,
                        fisTarih = item.fisTarih.ToString(),
                        ambarKod = item.ambarKod,
                        durum = durum,
                        islemTarih = item.islemTarih,
                        islemYapan = item.islemYapan,
                        onayDurum = onayDurum,
                        muhasebeAd = item.muhasebeAd,
                        harcamaAd = item.harcamaAd,
                        ambarAd = item.ambarAd,
                        hesapKodu = item.hesapKodu,
                        mernis = item.mernis,
                        tip = item.tip,
                        onayDurumKod = item.onayDurum
                    });
                }

                stoAmortisman.DataSource = liste;
                stoAmortisman.DataBind();
            }
            else
                GenelIslemler.MesajKutusu("Bilgi", formlar.sonuc.hataStr != "" ? formlar.sonuc.hataStr : Resources.TasinirMal.Amortisman002);
        }

        protected void btnOnayla_Click(object sender, DirectEventArgs e)
        {
            Sonuc sonuc = new Sonuc();
            string json = e.ExtraParams["json"];

            foreach (JContainer jc in (JArray)JSON.Deserialize(json))
            {
                AmortismanIslemForm af = new AmortismanIslemForm();

                af.kod = jc.Value<string>("kod");
                af.onayDurum = OrtakFonksiyonlar.ConvertToInt(jc.Value<string>("onayDurumKod"), 0);

                ENUMTasinirIslemFormOnayDurumu yeniDurum = ENUMTasinirIslemFormOnayDurumu.TANIMSIZ;
                yeniDurum = (af.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIB ? ENUMTasinirIslemFormOnayDurumu.GONDERILDIA : ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI);

                sonuc = servisTMM.AmortismanIslemFisiOnayDurumDegistir(kullanan, af, yeniDurum);
            }

            if (json == "")
                GenelIslemler.MesajKutusu("Uyarý", "Lütfen Listeden Kayýt Seçiniz.");

            if (sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        protected void btnOnayaGonder_Click(Object sender, DirectEventArgs e)
        {
            Sonuc sonuc = new Sonuc();
            string json = e.ExtraParams["json"];
            string bilgi = "";

            foreach (JContainer jc in (JArray)JSON.Deserialize(json))
            {
                AmortismanIslemForm af = new AmortismanIslemForm();

                af.kod = jc.Value<string>("kod");
                af.onayDurum = OrtakFonksiyonlar.ConvertToInt(jc.Value<string>("onayDurumKod"), 0);

                sonuc = servisTMM.AmortismanIslemFisiOnayDurumDegistir(kullanan, af, ENUMTasinirIslemFormOnayDurumu.GONDERILDIB);

                if (sonuc.islemSonuc)
                {
                    if (bilgi != "") bilgi += "</br>";
                    bilgi += sonuc.anahtar + " belgenin onay durumu deðiþtirildi.";
                }
            }

            if (((JArray)JSON.Deserialize(json)).Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Lütfen Listeden Kayýt Seçiniz.");
                return;
            }

            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi);
                btnOku_Click(null, null);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        protected void btnIptal_Click(object sender, DirectEventArgs e)
        {
            Sonuc sonuc = new Sonuc();
            string json = e.ExtraParams["json"];
            string bilgi = "";

            foreach (JContainer jc in (JArray)JSON.Deserialize(json))
            {
                AmortismanIslemForm af = new AmortismanIslemForm();

                af.kod = jc.Value<string>("kod");
                af.onayDurum = OrtakFonksiyonlar.ConvertToInt(jc.Value<string>("onayDurumKod"), 0);

                sonuc = servisTMM.AmortismanIslemFisiDurumDegistir(kullanan, af, ENUMAmortismanFormDurumu.IPTAL);

                if (sonuc.islemSonuc)
                {
                    if (bilgi != "") bilgi += "</br>";
                    bilgi += sonuc.anahtar + " belge iptal edildi.";
                }
            }

            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi);
                btnOku_Click(null, null);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
            pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());
        }

    }
}
