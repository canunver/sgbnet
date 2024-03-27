using System;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.TMM;
using System.Collections.Generic;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// Ambarlarý yýl bazýnda iþleme kapatma iþleminin yapýldýðý sayfa
    /// </summary>
    public partial class YilKapat : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa ilk defa çaðýrýlýyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMYLK001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                if (Request.QueryString["menuYok"] == "1")
                {
                    tabPanelAna.Margins = "0 0 0 0";
                    tabPanelAna.StyleSpec += "padding:5px";
                    tabPanelAna.Border = true;
                }
                else
                    hdnSecKapat.Value = 1;
            }
        }

        /// <summary>
        /// Ýþleme Kapat tuþuna basýlýnca çalýþan olay metodu
        /// Sayfadaki kontrollerden ambar listeleme kriterlerini toplar ve sunucudan
        /// kriterlere uygun olan ambarlarý alýr. Daha sonra stratek kullanýcýsýnýn
        /// deðiþken listesine sunucudan gelen ambarlarýn kapalý olduðu bilgisini saklar.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKapat_Click(object sender, DirectEventArgs e)
        {
            int yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);

            Ambar ambar = new Ambar();
            ambar.muhasebeKod = txtMuhasebe.Text.Trim();
            ambar.harcamaBirimKod = txtHarcamaBirimi.Text.Trim();
            ambar.kod = txtAmbar.Text.Trim();

            ObjectArray liste = servisTMM.AmbarListele(kullanan, ambar);

            TNS.KYM.Kullanici kapatKul = new TNS.KYM.Kullanici("stratek", "", "", OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul(), "stratek", "ltd", new TNSDateTime(), "", "", new TNSDateTime(), ENUMAktiflik.AKTIF, "", "", "", new TNSDateTime(), "", "");

            string degisken = "";

            foreach (Ambar a in liste.objeler)
            {
                degisken = "KAPAT" + "_" + yil + "_" + a.muhasebeKod + "_" + a.harcamaBirimKod.Replace(".", "") + "_" + a.kod;
                GenelIslemler.KullaniciDegiskenSakla(kapatKul, degisken, "1");
            }

            GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMYLK002);
        }

        /// <summary>
        /// Ýþleme Aç tuþuna basýlýnca çalýþan olay metodu
        /// Sayfadaki kontrollerden ambar listeleme kriterlerini toplar ve sunucudan
        /// kriterlere uygun olan ambarlarý alýr. Daha sonra stratek kullanýcýsýnýn
        /// deðiþken listesine sunucudan gelen ambarlarýn açýk olduðu bilgisini saklar.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAc_Click(object sender, DirectEventArgs e)
        {
            int yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);

            Ambar ambar = new Ambar();
            ambar.muhasebeKod = txtMuhasebe.Text.Trim();
            ambar.harcamaBirimKod = txtHarcamaBirimi.Text.Trim();
            ambar.kod = txtAmbar.Text.Trim();

            ObjectArray liste = servisTMM.AmbarListele(kullanan, ambar);

            TNS.KYM.Kullanici kapatKul = new TNS.KYM.Kullanici("stratek", "", "", OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul(), "stratek", "ltd", new TNSDateTime(), "", "", new TNSDateTime(), ENUMAktiflik.AKTIF, "", "", "", new TNSDateTime(), "", "");

            kapatKul.kullaniciKodu = "stratek";
            string degisken = "";

            double yilKapali = TNS.UZY.Arac.Tanimla().UzayDegeriDbl(kullanan, "TASAMBARYILKAPALI", yil.ToString(), true);
            if (yilKapali == 1)
            {
                GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMYLK019);
                return;
            }

            foreach (Ambar a in liste.objeler)
            {
                degisken = "KAPAT" + "_" + yil + "_" + a.muhasebeKod + "_" + a.harcamaBirimKod.Replace(".", "") + "_" + a.kod;
                GenelIslemler.KullaniciDegiskenSakla(kapatKul, degisken, "");
            }

            GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMYLK003);
        }
    }
}