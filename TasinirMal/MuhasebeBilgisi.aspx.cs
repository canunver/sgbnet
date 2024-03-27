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
using Newtonsoft.Json;

namespace TasinirMal
{
    /// <summary>
    /// Web formu ile ilgili olayları (event) ve fonksiyonları tutan sınıf
    /// </summary>
    public partial class MuhasebeBilgisi : TMMSayfaV2
    {
        ITMMServis servisTMM = null;

        /// <summary>
        /// Sayfa hazırlanırken, çağrılan olay (event) fonksiyon.
        /// </summary>
        /// <param name="sender">Olayı uyandıran nesne</param>
        /// <param name="e">Olayın parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            servisTMM = TNS.TMM.Arac.Tanimla();

            if (!X.IsAjaxRequest)
            {
                formAdi = "Muhasebe İşlemleri";
                yardimBag = yardimYol + "#YardimDosyasiAd";
                OrtakClass.GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);
                SayfaUstAltBolumYaz(this);
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                DurumDoldur();

                pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));
                pgFiltre.UpdateProperty("prpDurum", -1);

                pgFiltre.UpdateProperty("prpBelgeTarihi1", DateTime.Now.Date);
                pgFiltre.UpdateProperty("prpBelgeTarihi2", DateTime.Now.Date);
            }
        }

        private void DurumDoldur()
        {
            List<object> liste = new List<object>();

            liste.Add(new { KOD = (int)ENUMMuhasebeIslemiOnayDurumu.GONDERILDI, ADI = "Gönderildi" });
            liste.Add(new { KOD = (int)ENUMMuhasebeIslemiOnayDurumu.TANIMSIZ, ADI = "Servis İle Bağlanı Kurulamadı" });

            liste.Add(new { KOD = (int)ENUMMuhasebeIslemiOnayDurumu.ISLENDI, ADI = "Islendi" });
            liste.Add(new { KOD = (int)ENUMMuhasebeIslemiOnayDurumu.HATALI, ADI = "Hatali" });
            liste.Add(new { KOD = (int)ENUMMuhasebeIslemiOnayDurumu.HAZIR, ADI = "Hazir" });
            liste.Add(new { KOD = (int)ENUMMuhasebeIslemiOnayDurumu.BEKLEMEDE, ADI = "Beklemede" });
            liste.Add(new { KOD = (int)ENUMMuhasebeIslemiOnayDurumu.HENUZISLENMEDI, ADI = "HenuzIslenmedi" });
            liste.Add(new { KOD = (int)ENUMMuhasebeIslemiOnayDurumu.ISLEMBULUNAMADI, ADI = "IslemBulunamadi" });
            liste.Add(new { KOD = (int)ENUMMuhasebeIslemiOnayDurumu.BILINMEYENDURUM, ADI = "BilinmeyenDurum" });
            liste.Add(new { KOD = -1, ADI = "Hepsi" });

            strDurum.DataSource = liste;
            strDurum.DataBind();
        }

        private MuhasebeIslemi KriterOku()
        {
            MuhasebeIslemi form = new MuhasebeIslemi();
            form.kriter.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            form.kriter.harcamaKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim();
            form.durum = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpDurum"].Value.Trim(), 0);
            form.islemRefNo = pgFiltre.Source["prpIslemRefNo"].Value.Trim();
            form.islemCinsi = pgFiltre.Source["prpIslemCinsi"].Value.Trim();
            form.sorguTarih1 = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi1"].Value.Trim());
            form.sorguTarih2 = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi2"].Value.Trim());
            form.sorguTarih2.AddDays(1);

            return form;
        }

        protected void btnOku_Click(object sender, DirectEventArgs e)
        {
            MuhasebeIslemi form = KriterOku();

            List<object> listeStore = new List<object>();

            ObjectArray liste = servisTMM.MuhasebeBilgiListele(kullanan, form);
            if (liste.sonuc.islemSonuc)
            {
                foreach (MuhasebeIslemi mi in liste.objeler)
                {
                    listeStore.Add(new
                    {
                        ISLEMREFNO = mi.islemRefNo,
                        ISLEMCINSI = mi.islemCinsi,
                        SERVIS = mi.servis,
                        DURUM = mi.durum,
                        ISLEMYAPAN = mi.islemYapan,
                        ISLEMTARIH = mi.islemTarih.Oku(),
                        JSON = mi.json
                    });
                }
            }
            else
            {
                if (!liste.sonuc.islemSonuc)
                    GenelIslemler.MesajKutusu("HATA", liste.sonuc.hataStr);
            }


            strMuhasebe.DataSource = listeStore;
            strMuhasebe.DataBind();
        }

        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            string hata = "";
            List<MuhasebeIslemi> liste = new List<MuhasebeIslemi>();

            Newtonsoft.Json.Linq.JArray satirlar = (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(e.ExtraParams["SATIRLAR"]);

            foreach (Newtonsoft.Json.Linq.JObject item in satirlar)
            {
                MuhasebeIslemi form = new MuhasebeIslemi();
                form.islemRefNo = TasinirGenel.DegerAlStr(item, "ISLEMREFNO");
                form.islemCinsi = TasinirGenel.DegerAlStr(item, "ISLEMCINSI");
                form.durum = TasinirGenel.DegerAlInt(item, "DURUM");
                form.json = TasinirGenel.DegerAlStr(item, "JSON");

                if (form.durum != (int)ENUMMuhasebeIslemiOnayDurumu.TANIMSIZ)
                {
                    hata += form.islemRefNo + " referans nolu kayıt daha önce muhasebe servisine gönderilmiştir." + "<br>";
                    continue;
                }

                liste.Add(form);
            }

            if (hata != "")
                GenelIslemler.MesajKutusu("Hata", hata);
            else
            {
                Sonuc sonuc = servisTMM.MuhasebeServisineGonder(kullanan, liste.ToArray());
                if (sonuc.islemSonuc)
                {
                    if (sonuc.bilgiStr != "")
                        GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
                }
                else
                {
                    if (sonuc.hataStr != "")
                        GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
                }
            }
        }

        protected void btnDurumGuncelle_Click(object sender, DirectEventArgs e)
        {
            List<MuhasebeIslemi> liste = new List<MuhasebeIslemi>();

            Newtonsoft.Json.Linq.JArray satirlar = (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(e.ExtraParams["SATIRLAR"]);

            foreach (Newtonsoft.Json.Linq.JObject item in satirlar)
            {
                MuhasebeIslemi form = new MuhasebeIslemi();
                form.islemRefNo = TasinirGenel.DegerAlStr(item, "ISLEMREFNO");
                form.islemCinsi = TasinirGenel.DegerAlStr(item, "ISLEMCINSI");
                form.durum = TasinirGenel.DegerAlInt(item, "DURUM");

                liste.Add(form);
            }

            servisTMM = TNS.TMM.Arac.Tanimla();
            Sonuc sonuc = servisTMM.MuhasebeBilgiDurumSorgulaVeGuncelle(kullanan, liste.ToArray());
            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                btnOku_Click(null, null);
                if (sonuc.bilgiStr != "")
                    GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            }
        }


        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
            pgFiltre.UpdateProperty("prpDurum", (int)ENUMMuhasebeIslemiOnayDurumu.GONDERILDI);
            pgFiltre.UpdateProperty("prpIslemRefNo", string.Empty);
            pgFiltre.UpdateProperty("prpIslemCinsi", string.Empty);
        }

    }


}
