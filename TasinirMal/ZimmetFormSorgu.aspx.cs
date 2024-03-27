using Ext1.Net;
using OrtakClass;
using System;
using System.Collections.Generic;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Zimmet fiþi bilgilerinin sorgulama, yazdýrma ve durum deðiþtirme iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class ZimmetFormSorgu : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa adresinde belgeTur girdi dizgisi dolu deðilse hata verir
        ///     ve sayfayý yüklemez, dolu ise sayfada bazý ayarlamalar yapýlýr.
        ///     Ýlk yükleniþte, sayfadaki kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                string belgeTuru = string.Empty;
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                if (Request.QueryString["belgeTur"] != null)
                {
                    belgeTuru = Request.QueryString["belgeTur"] + "";
                }
                else
                {
                    GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMZFS001);
                    return;
                }
                FormAyarla(belgeTuru);

                hdnBelgeTur.Value = belgeTuru == "10" ? ((int)ENUMZimmetBelgeTur.ZIMMETFISI).ToString() : belgeTuru == "20" ? ((int)ENUMZimmetBelgeTur.DAYANIKLITL).ToString() : ((int)ENUMZimmetBelgeTur.BELIRSIZ).ToString();

                pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());
                pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));
                pgFiltre.UpdateProperty("prpAmbar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR"));
                //pgFiltre.UpdateProperty("prpDurum", OrtakFonksiyonlar.ConvertToInt(GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ZIFDURUM"), 0));
                pgFiltre.UpdateProperty("prpDurum", 0);
                pgFiltre.UpdateProperty("prpBelgeTuru", 0);
                pgFiltre.UpdateProperty("prpBelgeTipi", 0);

                DurumDoldur();
                BelgeTuruDoldur();
                BelgeTipiDoldur();

                //if (TasinirGenel.tasinirZimmeteOnay)
                //{
                //    btnEOnayYazdir.Visible = true;
                //    eOnayAlan.Visible = true;

                //    ddleOnayDurum.Items.Add(new ListItem(Resources.TasinirMal.FRMZFG063, "0"));
                //    ddleOnayDurum.Items.Add(new ListItem(Resources.TasinirMal.FRMZFG059, "1"));
                //    ddleOnayDurum.Items.Add(new ListItem(Resources.TasinirMal.FRMZFG060, "4"));
                //    ddleOnayDurum.Items.Add(new ListItem(Resources.TasinirMal.FRMZFG061, "5"));
                //    ddleOnayDurum.Items.Add(new ListItem(Resources.TasinirMal.FRMZFS007, ""));
                //    ddleOnayDurum.SelectedIndex = 4;
                //}
                //else
                //{
                //    btnEOnayYazdir.Visible = false;
                //    eOnayAlan.Visible = false;
                //}
            }
        }

        /// <summary>
        /// Parametre olarak verilen zimmet belge türüne bakarak sayfada bazý ayarlamalar yapar.
        /// </summary>
        /// <param name="belgeTuru">Zimmet belge türü</param>
        private void FormAyarla(string belgeTuru)
        {
            if (belgeTuru == "10")
                formAdi = Resources.TasinirMal.FRMZFS012;
            else if (belgeTuru == "20")
                formAdi = Resources.TasinirMal.FRMZFS013;
            else
            {
                GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMZFS014);
                return;
            }
        }

        /// <summary>
        /// Listele tuþuna basýlýnca çalýþan olay metodu
        /// Listeleme kriterleri ekrandaki ilgili kontrollerden toplanýr ve sunucuya gönderilir
        /// ve zimmet fiþi bilgileri sunucudan alýnýr. Hata varsa ekrana hata bilgisi yazýlýr,
        /// yoksa gelen zimmet fiþi bilgileri gvBelgeler GridView kontrolüne doldurulur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, EventArgs e)
        {
            TNS.TMM.ZimmetForm zimmet = new TNS.TMM.ZimmetForm();

            zimmet.yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpYil"].Value.Trim(), 0);
            zimmet.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            zimmet.harcamaBirimKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            zimmet.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            zimmet.tip = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpBelgeTipi"].Value, 0);
            zimmet.vermeDusme = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpBelgeTuru"].Value, 0);
            zimmet.belgeTur = OrtakFonksiyonlar.ConvertToInt(hdnBelgeTur.Value, 0);

            TasinirGenel.DegiskenleriKaydet(kullanan, zimmet.muhasebeKod, zimmet.harcamaBirimKod, zimmet.ambarKod);

            ZimmetFormKriter kriter = new ZimmetFormKriter();
            if (!string.IsNullOrWhiteSpace(pgFiltre.Source["prpBelgeNo1"].Value))
                kriter.belgeNoBasla = pgFiltre.Source["prpBelgeNo1"].Value.PadLeft(6, '0').Trim();
            if (!string.IsNullOrWhiteSpace(pgFiltre.Source["prpBelgeNo2"].Value))
                kriter.belgeNoBit = pgFiltre.Source["prpBelgeNo2"].Value.PadLeft(6, '0').Trim();
            kriter.belgeTarihBasla = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi1"].Value.Trim());
            kriter.belgeTarihBit = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi2"].Value.Trim());
            kriter.islemYapan = pgFiltre.Source["prpSonislemyapan"].Value.Trim();
            kriter.durum = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpDurum"].Value, 0);
            kriter.kime = pgFiltre.Source["prpKisiKod"].Value.Trim();
            kriter.nereye = pgFiltre.Source["prpOdaKod"].Value.Trim();
            kriter.sicilNo = pgFiltre.Source["prpHesapKod"].Value.Trim().Replace(".", "");

            GenelIslemler.KullaniciDegiskenSakla(kullanan, "ZIFDURUM", kriter.durum.ToString());

            ObjectArray bilgiler = new ObjectArray();

            if (TasinirGenel.tasinirZimmeteOnay)
            {
                //bilgiler = servisTMM.ZimmetFisiListele(kullanan, zimmet, kriter, ddleOnayDurum.SelectedValue);
            }
            else
            {
                bilgiler = servisTMM.ZimmetFisiListele(kullanan, zimmet, kriter, null);
            }

            if (!bilgiler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", bilgiler.sonuc.hataStr);
                return;
            }

            List<object> liste = new List<object>();
            foreach (TNS.TMM.ZimmetForm zf in bilgiler.objeler)
            {
                string fisTipi = zf.tip == (int)ENUMZimmetTipi.DEMIRBASCIHAZ ? Resources.TasinirMal.FRMZFS002 : zf.tip == (int)ENUMZimmetTipi.TASITMAKINE ? Resources.TasinirMal.FRMZFS003 : string.Empty;
                string tur = zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME ? Resources.TasinirMal.FRMZFS005 : zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME ? Resources.TasinirMal.FRMZFS006 : string.Empty;
                string durum = string.Empty;
                if (zf.durum == (int)ENUMBelgeDurumu.YENI)
                    durum = Resources.TasinirMal.FRMZFS015;
                else if (zf.durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
                    durum = Resources.TasinirMal.FRMZFS016;
                else if (zf.durum == (int)ENUMBelgeDurumu.ONAYLI)
                    durum = Resources.TasinirMal.FRMZFS017;
                else if (zf.durum == (int)ENUMBelgeDurumu.IPTAL)
                    durum = Resources.TasinirMal.FRMZFS018;

                liste.Add(new
                {
                    BELGENO = zf.fisNo,
                    YIL = zf.yil,
                    BELGETARIHI = zf.fisTarih.Oku(),
                    TIPI = fisTipi,
                    TURU = tur,
                    MUHASEBE = zf.muhasebeKod + " - " + zf.muhasebeAd,
                    HARCAMABIRIMI = zf.harcamaBirimKod + " - " + zf.harcamaBirimAd,
                    AMBAR = zf.ambarKod + " - " + zf.ambarAd,
                    DURUM = durum,
                    SONISLEMTARIHI = zf.islemTarih.Oku(),
                    SONISLEMYAPAN = zf.islemYapan,
                    MUHASEBEKOD = zf.muhasebeKod,
                    HARCAMABIRIMIKOD = zf.harcamaBirimKod,
                    KIMEVERILDI = zf.kisiAdi,
                    NEREYEVERILDI = zf.nereyeGitti
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Liste Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Listeleme kriterleri ekrandaki ilgili kontrollerden toplanýr, sunucuya gönderilir
        /// ve zimmet fiþi bilgileri sunucudan alýnýr. Hata varsa ekrana hata bilgisi
        /// yazýlýr, yoksa gelen zimmet fiþi bilgilerini içeren excel raporu üretilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListeYazdir_Click(object sender, EventArgs e)
        {
            TNS.TMM.ZimmetForm zimmet = new TNS.TMM.ZimmetForm();

            zimmet.yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpYil"].Value.Trim(), 0);
            zimmet.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            zimmet.harcamaBirimKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            zimmet.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            zimmet.tip = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpBelgeTipi"].Value, 0);
            zimmet.vermeDusme = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpBelgeTuru"].Value, 0);
            zimmet.belgeTur = OrtakFonksiyonlar.ConvertToInt(hdnBelgeTur.Value, 0);

            ZimmetFormKriter kriter = new ZimmetFormKriter();
            if (!string.IsNullOrWhiteSpace(pgFiltre.Source["prpBelgeNo1"].Value))
                kriter.belgeNoBasla = pgFiltre.Source["prpBelgeNo1"].Value.PadLeft(6, '0').Trim();
            if (!string.IsNullOrWhiteSpace(pgFiltre.Source["prpBelgeNo2"].Value))
                kriter.belgeNoBit = pgFiltre.Source["prpBelgeNo2"].Value.PadLeft(6, '0').Trim();
            kriter.belgeTarihBasla = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi1"].Value.Trim());
            kriter.belgeTarihBit = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi2"].Value.Trim());
            //kriter.sicilNo = pgFiltre.Source["05nereyeVerildi"].Value.Trim();//yok
            kriter.islemYapan = pgFiltre.Source["prpSonislemyapan"].Value.Trim();
            kriter.durum = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpDurum"].Value, 0);
            kriter.kime = pgFiltre.Source["prpKisiKod"].Value.Trim();
            kriter.nereye = pgFiltre.Source["prpOdaKod"].Value.Trim();

            ObjectArray bilgiler = new ObjectArray();

            if (TasinirGenel.tasinirZimmeteOnay)
            {
                //bilgiler = servisTMM.ZimmetFisiListele(kullanan, zimmet, kriter, ddleOnayDurum.SelectedValue);
            }
            else
            {
                bilgiler = servisTMM.ZimmetFisiListele(kullanan, zimmet, kriter, null);
            }

            if (!bilgiler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", bilgiler.sonuc.hataStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "ZFListe.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            for (int i = 0; i < bilgiler.objeler.Count; i++)
            {
                TNS.TMM.ZimmetForm zfBelge = (TNS.TMM.ZimmetForm)bilgiler.objeler[i];

                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 9, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, zfBelge.yil);
                XLS.HucreDegerYaz(satir, sutun + 1, zfBelge.muhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 2, zfBelge.harcamaBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 3, zfBelge.ambarAd);
                XLS.HucreDegerYaz(satir, sutun + 4, zfBelge.fisNo);

                string fisTipi = zfBelge.tip == (int)ENUMZimmetTipi.DEMIRBASCIHAZ ? Resources.TasinirMal.FRMZFS002 : zfBelge.tip == (int)ENUMZimmetTipi.TASITMAKINE ? Resources.TasinirMal.FRMZFS003 : string.Empty;
                string tur = zfBelge.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME ? Resources.TasinirMal.FRMZFS005 : zfBelge.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME ? Resources.TasinirMal.FRMZFS006 : string.Empty;
                XLS.HucreDegerYaz(satir, sutun + 5, fisTipi);
                XLS.HucreDegerYaz(satir, sutun + 6, tur);

                //string durum = "";
                //if (zfBelge.durum == (int)ENUMBelgeDurumu.YENI)
                //    durum = "Yeni";
                //else if (zfBelge.durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
                //    durum = "Deðiþtirildi";
                //else if (zfBelge.durum == (int)ENUMBelgeDurumu.ONAYLI)
                //    durum = "Onaylý";
                //else if (zfBelge.durum == (int)ENUMBelgeDurumu.IPTAL)
                //    durum = "Ýptal";
                //XLS.HucreDegerYaz(satir, sutun + 5, durum);

                XLS.HucreDegerYaz(satir, sutun + 7, zfBelge.fisTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 8, zfBelge.islemTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 9, zfBelge.islemYapan.ToString());
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// EOnay Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Listeleme kriterleri ekrandaki ilgili kontrollerden toplanýr, sunucuya gönderilir
        /// ve zimmet fiþi eonay bilgileri sunucudan alýnýr. Hata varsa ekrana hata bilgisi
        /// yazýlýr, yoksa gelen zimmet fiþi eonay bilgilerini içeren excel raporu üretilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnEOnayYazdir_Click(object sender, EventArgs e)
        {
            TNS.TMM.ZimmetForm zimmet = new TNS.TMM.ZimmetForm();
            //zimmet.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.SelectedValue, 0);
            //zimmet.muhasebeKod = txtMuhasebe.Text.Trim();
            //zimmet.harcamaBirimKod = txtHarcamaBirimi.Text.Trim();
            //zimmet.ambarKod = txtAmbar.Text.Trim();
            //zimmet.tip = OrtakFonksiyonlar.ConvertToInt(ddlZimmetFisiTipi.SelectedValue, 0);
            //zimmet.vermeDusme = OrtakFonksiyonlar.ConvertToInt(ddlzimmetVermeDusme.SelectedValue, 0);
            //zimmet.belgeTur = OrtakFonksiyonlar.ConvertToInt(hdnBelgeTur.Value, 0);

            ZimmetFormKriter kriter = new ZimmetFormKriter();
            //kriter.belgeNoBasla = txtBelgeNo1.Text == string.Empty ? string.Empty : txtBelgeNo1.Text.PadLeft(6, '0');
            //kriter.belgeNoBit = txtBelgeNo2.Text == string.Empty ? string.Empty : txtBelgeNo2.Text.PadLeft(6, '0');
            //kriter.belgeTarihBasla = new TNSDateTime(txtBelgeTarih1.Text);
            //kriter.belgeTarihBit = new TNSDateTime(txtBelgeTarih2.Text);
            //kriter.sicilNo = txtSicilNo.Text;
            //kriter.islemYapan = txtIslemYapan.Text;
            //kriter.durum = OrtakFonksiyonlar.ConvertToInt(ddlDurum.SelectedValue, 0);

            ZimmetFormEOnayKriter eonaykriter = new ZimmetFormEOnayKriter();
            //eonaykriter.durum = OrtakFonksiyonlar.ConvertToInt(ddleOnayDurum.SelectedValue, 0);
            //eonaykriter.eonayGondermeTarihBasla = new TNSDateTime();
            //eonaykriter.eonayGondermeTarihBitis = new TNSDateTime();
            //eonaykriter.eonayCevapTarihBasla = new TNSDateTime();
            //eonaykriter.eonayCevapTarihBitis = new TNSDateTime();

            ObjectArray bilgi;
            if (TasinirGenel.tasinirZimmeteOnay)
            {
                bilgi = servisTMM.ZimmetFisiListeleEOnayBilgisiyle(kullanan, zimmet, kriter, eonaykriter);
            }
            else
            {
                GenelIslemler.MesajKutusu("Uyarý", "Hata.");
                return;
            }
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
            string sablonAd = "ZFEOnayListe.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                TNS.TMM.ZimmetFormWithEonay zfBelge = (TNS.TMM.ZimmetFormWithEonay)bilgi.objeler[i];

                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 9, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, zfBelge.yil);
                XLS.HucreDegerYaz(satir, sutun + 1, zfBelge.muhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 2, zfBelge.harcamaBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 3, zfBelge.emailKimdenAd);
                XLS.HucreDegerYaz(satir, sutun + 4, zfBelge.fisNo);
                XLS.HucreDegerYaz(satir, sutun + 5, zfBelge.emailKime);
                XLS.HucreDegerYaz(satir, sutun + 6, zfBelge.emailKimeAd);

                XLS.HucreDegerYaz(satir, sutun + 7, zfBelge.emailGondermeTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 8, zfBelge.emailCevapTarih.ToString());

                string durum = "";
                if (zfBelge.emaildurum == (int)ENUMEOnayDurumu.GONDERILMEDI)
                    durum = Resources.TasinirMal.FRMZFG063;
                else if (zfBelge.emaildurum == (int)ENUMEOnayDurumu.GONDERILDI)
                    durum = Resources.TasinirMal.FRMZFG059;
                else if (zfBelge.emaildurum == (int)ENUMEOnayDurumu.CEVAPLADIKABUL)
                    durum = Resources.TasinirMal.FRMZFG060;
                else if (zfBelge.emaildurum == (int)ENUMEOnayDurumu.CEVAPLADIRED)
                    durum = Resources.TasinirMal.FRMZFG061;

                XLS.HucreDegerYaz(satir, sutun + 9, durum);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Belge durum deðiþtirme ve belge yazdýrma iþlemlerini yapan yordam
        /// </summary>
        [DirectMethod]
        public void Islem(string yil, string muhasebeKod, string harcamaBirimKod, string fisNo, string islem)
        {
            string[] yillar = yil.Split(';');
            string[] muhasebeKodlar = muhasebeKod.Split(';');
            string[] harcamaBirimKodlar = harcamaBirimKod.Split(';');
            string[] fisNolar = fisNo.Split(';');

            int[] esitMi = { yillar.Length, muhasebeKodlar.Length, harcamaBirimKodlar.Length, fisNolar.Length };

            for (int i = 0; i < esitMi.Length - 1; i++)
            {
                if (esitMi[i] != esitMi[i + 1])
                {
                    GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMZSI001);
                    return;
                }
            }

            int belgeTur = OrtakFonksiyonlar.ConvertToInt(hdnBelgeTur.Value, 0);
            if (islem == "Yazdir")
            {
                string tempFileName = System.IO.Path.GetTempFileName();
                string klasor = tempFileName + ".dir";
                System.IO.DirectoryInfo dri = System.IO.Directory.CreateDirectory(klasor);
                klasor += "\\";

                for (int i = 0; i < fisNolar.Length; i++)
                {
                    int yili = OrtakFonksiyonlar.ConvertToInt(yillar[i], 0);
                    string muhasebeKodu = muhasebeKodlar[i].Split('-')[0];
                    string harcamaKodu = harcamaBirimKodlar[i].Split('-')[0];
                    string belgeNo = fisNolar[i].Trim();

                    string tmpFile = System.IO.Path.GetTempFileName();
                    string excelYazYer = klasor + harcamaKodu.Replace(".", "") + "_" + belgeNo + "." + GenelIslemler.ExcelTur();

                    System.IO.File.Move(tmpFile, excelYazYer);
                    System.IO.File.Delete(tmpFile);

                    ZimmetFormYazdir.Yazdir(kullanan, yili, belgeNo, harcamaKodu, muhasebeKodu, belgeTur, excelYazYer, false);
                }

                string[] dosyalar = { "" };
                string sonucDosyaAd = System.IO.Path.GetTempFileName();

                OrtakClass.Zip.Ziple(dri.FullName, sonucDosyaAd, dosyalar);
                dri.Delete(true);
                System.IO.File.Delete(tempFileName);
                OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, "Belgeler.zip", true, "zip");
            }
            else
            {
                Sonuc sonuc = new Sonuc();
                string bilgiStr = string.Empty;

                for (int i = 0; i < yillar.Length; i++)
                {
                    TNS.TMM.ZimmetForm zf = new TNS.TMM.ZimmetForm();

                    zf.yil = OrtakFonksiyonlar.ConvertToInt(yillar[i], 0);
                    zf.muhasebeKod = muhasebeKodlar[i].Split('-')[0];
                    zf.harcamaBirimKod = harcamaBirimKodlar[i].Split('-')[0];
                    zf.fisNo = fisNolar[i].Trim();
                    zf.belgeTur = belgeTur;

                    sonuc = servisTMM.ZimmetFisiDurumDegistir(kullanan, zf, islem);

                    if (!sonuc.islemSonuc)
                    {
                        GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
                        return;
                    }
                    else
                    {
                        bilgiStr += sonuc.bilgiStr;
                    }
                }

                GenelIslemler.MesajKutusu("Bilgi", bilgiStr);
                btnSorguTemizle_Click(null, null);
                btnListele_Click(null, null);
            }
        }

        /// <summary>
        /// Sayfadaki ddlDurum DropDownList kontrolüne durum bilgileri doldurulur.
        /// </summary>
        private void DurumDoldur()
        {
            List<object> liste = new List<object>();

            liste.Add(new { KOD = 1, ADI = Resources.TasinirMal.FRMZFS008 });
            liste.Add(new { KOD = 5, ADI = Resources.TasinirMal.FRMZFS009 });
            liste.Add(new { KOD = 9, ADI = Resources.TasinirMal.FRMZFS010 });
            liste.Add(new { KOD = 0, ADI = Resources.TasinirMal.FRMZFS011 });

            strDurum.DataSource = liste;
            strDurum.DataBind();
        }

        /// <summary>
        /// Sayfadaki ddlBelgeTuru DropDownList kontrolüne belge türü bilgileri doldurulur.
        /// </summary>
        private void BelgeTuruDoldur()
        {
            List<object> liste = new List<object>();

            liste.Add(new { KOD = 1, ADI = Resources.TasinirMal.FRMZFS005 });
            liste.Add(new { KOD = 2, ADI = Resources.TasinirMal.FRMZFS006 });
            liste.Add(new { KOD = 0, ADI = Resources.TasinirMal.FRMZFS007 });

            strBelgeTuru.DataSource = liste;
            strBelgeTuru.DataBind();
        }

        /// <summary>
        /// Sayfadaki ddlBelgeTipi DropDownList kontrolüne belge tipi bilgileri doldurulur.
        /// </summary>
        private void BelgeTipiDoldur()
        {
            List<object> liste = new List<object>();

            liste.Add(new { KOD = 1, ADI = Resources.TasinirMal.FRMZFS002 });
            liste.Add(new { KOD = 2, ADI = Resources.TasinirMal.FRMZFS003 });
            liste.Add(new { KOD = 0, ADI = Resources.TasinirMal.FRMZFS004 });

            strBelgeTipi.DataSource = liste;
            strBelgeTipi.DataBind();
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
            pgFiltre.UpdateProperty("prpBelgeNo1", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeNo2", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeTarihi1", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeTarihi2", string.Empty);
            pgFiltre.UpdateProperty("prpHesapKod", string.Empty);
            pgFiltre.UpdateProperty("prpKisiKod", string.Empty);
            pgFiltre.UpdateProperty("prpOdaKod", string.Empty);
            pgFiltre.UpdateProperty("prpSonislemyapan", string.Empty);
        }
    }
}