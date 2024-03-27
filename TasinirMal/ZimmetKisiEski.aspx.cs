using System;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Ki�iye verilmi� zimmetli demirba� bilgilerinin raporlama i�leminin yap�ld��� sayfa
    /// </summary>
    public partial class ZimmetKisiEski : TMMSayfa
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        ///     Sayfa ilk defa �a��r�l�yorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlan�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMZKS001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giri� izni varm�?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtKimeGitti.Attributes.Add("onblur", "kodAdGetir('36','lblKimeGittiAd',true,new Array('txtKimeGitti'),'KONTROLDENOKU');");
            this.txtHesapPlanKod.Attributes.Add("onblur", "kodAdGetir('30','lblHesapPlanAd',true,new Array('txtHesapPlanKod'),'KONTROLDENOKU');");

            if (!IsPostBack)
            {
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");

                YilDoldur();
            }

            if (TasinirGenel.tasinirSicilNoRFIDFarkli)
            {
                btnListeYazdir.Enabled = true;
                btnListeYazdir.Visible = true;
            }
            else
            {
                btnListeYazdir.Enabled = false;
                btnListeYazdir.Visible = false;
            }

            if (txtMuhasebe.Text.Trim() != "")
                lblMuhasebeAd.Text = GenelIslemler.KodAd(31, txtMuhasebe.Text.Trim(), true);
            else
                lblMuhasebeAd.Text = "";

            if (txtHarcamaBirimi.Text.Trim() != "")
                lblHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim(), true);
            else
                lblHarcamaBirimiAd.Text = "";

            if (txtKimeGitti.Text.Trim() != "")
                lblKimeGittiAd.Text = GenelIslemler.KodAd(36, txtKimeGitti.Text.Trim(), true);
            else
                lblKimeGittiAd.Text = "";
        }

        /// <summary>
        /// Sayfadaki ddlYil DropDownList kontrol�ne y�l bilgileri doldurulur.
        /// </summary>
        private void YilDoldur()
        {
            GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
        }

        /// <summary>
        /// Yazd�r tu�una bas�l�nca �al��an olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam �a��r�l�r ve toplanan kriterler
        /// ki�iye verilmi� zimmetli demirba�lar raporunu �reten ZimmetKisiYazdir yordam�na
        /// g�nderilir, b�ylece excel raporu �retilip kullan�c�ya g�nderilmi� olur.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            ZimmetKisiYazdir(KriterTopla());
        }


        protected void btnListeYazdir_Click(object sender, EventArgs e)
        {
            ZimmetKisiListeYazdir(KriterTopla());
        }


        /// <summary>
        /// Sayfadaki kontrollerden zimmet listeleme kriter bilgilerini toplayan ve d�nd�ren yordam
        /// </summary>
        /// <returns>Zimmet listeleme kriter bilgileri d�nd�r�l�r.</returns>
        private TNS.TMM.ZimmetOrtakAlanVeKisi KriterTopla()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "");

            TNS.TMM.ZimmetOrtakAlanVeKisi kriter = new TNS.TMM.ZimmetOrtakAlanVeKisi();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.tcKimlikNo = txtKimeGitti.Text.Trim();
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            return kriter;
        }

        /// <summary>
        /// Parametre olarak verilen zimmet listeleme kriterlerini sunucudaki ZimmetKisi yordam�na
        /// g�nderir, sunucudan gelen bilgi k�mesini excel dosyas�na yazar ve kullan�c�ya g�nderir.
        /// </summary>
        /// <param name="kriter">Zimmet kriter bilgilerini tutan nesne</param>
        private void ZimmetKisiYazdir(TNS.TMM.ZimmetOrtakAlanVeKisi kriter)
        {
            ObjectArray bilgi = servisTMM.ZimmetKisi(kullanan, kriter, true);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "ZimmetKisi.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            TNS.TMM.ZimmetOrtakAlanVeKisi zoa = (TNS.TMM.ZimmetOrtakAlanVeKisi)bilgi.objeler[0];
            //XLS.HucreAdBulYaz("HarcamaAd", zoa.harcamaAd);
            //XLS.HucreAdBulYaz("HarcamaKod", zoa.harcamaKod);
            XLS.HucreAdBulYaz("MuhasebeAd", zoa.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", zoa.muhasebeKod);
            XLS.HucreAdBulYaz("KisiAd", zoa.kisiAd);
            XLS.HucreAdBulYaz("KisiKod", zoa.tcKimlikNo);

            for (int i = 0; i < zoa.detay.Count; i++)
            {
                TNS.TMM.ZimmetOrtakAlanVeKisiDetay detay = (TNS.TMM.ZimmetOrtakAlanVeKisiDetay)zoa.detay[i];

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 4);
                XLS.HucreBirlestir(satir, sutun + 8, satir, sutun + 9);

                XLS.HucreDegerYaz(satir, sutun, detay.harcamaKod + " - " + detay.harcamaAd);
                XLS.HucreDegerYaz(satir, sutun + 1, detay.ambarKod + " - " + detay.ambarAd);

                if (TasinirGenel.rfIdVarMi)
                    XLS.HucreDegerYaz(satir, sutun + 2, detay.gorSicilNo + " - " + detay.rfIdNo);
                else
                    XLS.HucreDegerYaz(satir, sutun + 2, detay.gorSicilNo);

                XLS.HucreDegerYaz(satir, sutun + 3, detay.sicilAd);
                XLS.HucreDegerYaz(satir, sutun + 5, detay.odaAd);//zoa.odaKod + "-" +
                XLS.HucreDegerYaz(satir, sutun + 6, detay.fisNo);
                XLS.HucreDegerYaz(satir, sutun + 7, detay.fisTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 8, detay.aciklama);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Parametre olarak verilen zimmet listeleme kriterlerini sunucudaki ZimmetKisi yordam�na
        /// g�nderir, sunucudan gelen bilgi k�mesini excel dosyas�na yazar ve kullan�c�ya g�nderir.
        /// </summary>
        /// <param name="kriter">Zimmet kriter bilgilerini tutan nesne</param>
        private void ZimmetKisiListeYazdir(TNS.TMM.ZimmetOrtakAlanVeKisi kriter)
        {
            ObjectArray bilgi = servisTMM.ZimmetKisi(kullanan, kriter, true);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "ZimmetKisiListe.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            TNS.TMM.ZimmetOrtakAlanVeKisi zoa = (TNS.TMM.ZimmetOrtakAlanVeKisi)bilgi.objeler[0];
            //XLS.HucreAdBulYaz("HarcamaAd", zoa.harcamaAd);
            //XLS.HucreAdBulYaz("HarcamaKod", zoa.harcamaKod);
            XLS.HucreAdBulYaz("MuhasebeAd", zoa.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", zoa.muhasebeKod);
            XLS.HucreAdBulYaz("KisiAd", zoa.kisiAd);
            XLS.HucreAdBulYaz("KisiKod", zoa.tcKimlikNo);

            for (int i = 0; i < zoa.detay.Count; i++)
            {
                TNS.TMM.ZimmetOrtakAlanVeKisiDetay detay = (TNS.TMM.ZimmetOrtakAlanVeKisiDetay)zoa.detay[i];

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 10, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 2);
                XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 4);

                XLS.HucreDegerYaz(satir, sutun, detay.harcamaKod + " - " + detay.harcamaAd);
                //XLS.HucreDegerYaz(satir, sutun + 1, detay.ambarKod + " - " + detay.ambarAd);

                if (TasinirGenel.rfIdVarMi)
                    XLS.HucreDegerYaz(satir, sutun + 1, detay.gorSicilNo + " - " + detay.rfIdNo);
                else
                    XLS.HucreDegerYaz(satir, sutun + 1, detay.gorSicilNo);

                XLS.HucreDegerYaz(satir, sutun + 3, detay.sicilAd);

                ObjectArray sler = servisTMM.SicilNoOzellikListele(kullanan, detay.prSicilNo);
                if (sler.sonuc.islemSonuc && sler.objeler.Count > 0)
                {
                    TNS.TMM.SicilNoOzellik s = (TNS.TMM.SicilNoOzellik)sler.objeler[0];
                    XLS.HucreDegerYaz(satir, sutun + 5, s.markaAd);
                    XLS.HucreDegerYaz(satir, sutun + 6, s.modelAd);
                }


                XLS.HucreDegerYaz(satir, sutun + 8, detay.aciklama);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
    }
}