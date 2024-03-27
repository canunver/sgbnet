using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext1.Net;
using System.Collections.Generic;
using TNS.KYM;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr istek belgesi bilgilerinin listeleme iþleminin yapýldýðý sayfa
    /// </summary>
    public partial class TasinirIstekListe : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Sayfa ilk defa çaðýrýlýyorsa kontrollere ilgili bilgiler
        ///     doldurulur, sayfa ayarlanýr ve taþýnýr istek belgeleri listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                if (OrtakFonksiyonlar.ConvertToInt(Request.QueryString["hediyelik"], 0) <= 0)
                    formAdi = Resources.TasinirMal.FRMILF001;
                else
                    formAdi = Resources.TasinirMal.FRMILF017;

                //if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MISAFIR))
                //{
                //    string istekUrl = System.Configuration.ConfigurationManager.AppSettings.Get("TasinirIstekURL");
                //    link1.Attributes.Add("onclick", "this.style.behavior='url(#default#homepage)';this.setHomePage('" + istekUrl + "');");
                //    link2.Attributes.Add("href", "javascript:window.external.AddFavorite('" + istekUrl + "','" + Resources.TasinirMal.FRMILF002 + "')");
                //    divPersonelSecim.Visible = false;
                //}
                //else
                //{
                //    kolon1.Visible = false;
                //    kolon2.Visible = false;
                //}

                pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());
                pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));
                pgFiltre.UpdateProperty("prpAmbar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR"));
            }
        }

        /// <summary>
        /// Listele tuþuna basýlýnca çalýþan olay metodu
        /// Sunucudan kayýttan düþme teklif ve onay tutanaðý bilgileri alýnýr ve listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, DirectEventArgs e)
        {
            IstekForm kf = new IstekForm();
            kf.yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpYil"].Value.Trim(), 0);
            kf.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            kf.harcamaKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim();
            kf.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            kf.istekYapanKod = pgFiltre.Source["prpKisiKod"].Value.Trim();
            kf.hediyelik = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["hediyelik"], 0);

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MISAFIR))
                TasinirGenel.DegiskenleriKaydet(kullanan, kf.muhasebeKod, kf.harcamaKod, kf.ambarKod);

            ObjectArray bilgi = servisTMM.IstekListele(kullanan, kf, false);
            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                return;
            }

            List<object> liste = new List<object>();
            foreach (IstekForm kForm in bilgi.objeler)
            {
                liste.Add(new
                {
                    BELGENO = kForm.belgeNo.Trim(),
                    BELGENOSECIM = OrtakFonksiyonlar.ConvertToInt(kForm.belgeNo, 0).ToString("00000"),
                    YIL = kForm.yil,
                    BELGETARIHI = kForm.belgeTarihi.Oku(),
                    MUHASEBEKOD = kForm.muhasebeKod,
                    HARCAMABIRIMIKOD = kForm.harcamaKod,
                    AMBARKOD = kForm.harcamaKod,
                    MUHASEBE = kForm.muhasebeKod + "-" + kForm.muhasebeAd,
                    HARCAMABIRIMI = kForm.harcamaKod + "-" + kForm.harcamaAd,
                    AMBAR = kForm.ambarKod + "-" + kForm.ambarAd,
                    MUHASEBEAD = kForm.muhasebeAd,
                    HARCAMABIRIMIAD = kForm.harcamaAd,
                    AMBARAD = kForm.ambarAd,
                    ISTEKYAPANADI = kForm.istekYapanAd,
                    ISTEKYAPANKOD = kForm.istekYapanKod
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
        }

        protected void btnYazdir_Click(object sender, DirectEventArgs e)
        {
            IstekForm kf = new IstekForm();
            kf.yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpYil"].Value.Trim(), 0);
            kf.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            kf.harcamaKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim();
            kf.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            kf.istekYapanKod = pgFiltre.Source["prpKisiKod"].Value.Trim();
            kf.hediyelik = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["hediyelik"], 0);

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MISAFIR))
                TasinirGenel.DegiskenleriKaydet(kullanan, kf.muhasebeKod, kf.harcamaKod, kf.ambarKod);

            ObjectArray bilgi = servisTMM.IstekListele(kullanan, kf, false);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TasinirIstek.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                TNS.TMM.IstekForm tifBelge = (TNS.TMM.IstekForm)bilgi.objeler[i];

                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, tifBelge.yil);
                XLS.HucreDegerYaz(satir, sutun + 1, tifBelge.muhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 2, tifBelge.harcamaAd);
                XLS.HucreDegerYaz(satir, sutun + 3, tifBelge.belgeNo.PadLeft(5, '0'));

                XLS.HucreDegerYaz(satir, sutun + 4, tifBelge.belgeTarihi.ToString());
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
    }
}