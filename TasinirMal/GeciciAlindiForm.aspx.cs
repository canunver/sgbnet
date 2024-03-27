using Ext1.Net;
using OrtakClass;
using System;
using System.Collections;
using System.Collections.Generic;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Geçici alýndý fiþi bilgilerinin kayýt ve listeleme iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class GeciciAlindiForm : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Ýlk yükleniþte, sayfadaki kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMGAG001;
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtBelgeTarihi.Value = DateTime.Now.Date;
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                txtYil.Value = DateTime.Now.Year;
                ListeTemizle();
                hdnFirmaHarcamadanAlma.Value = TasinirGenel.tasinirFirmaBilgisiniHarcamadanAlma;

                if (Arac.MerkezBankasiKullaniyor())
                {
                    btnBelgeOnayla.Hidden = false;
                    btnBelgeOnayKaldir.Hidden = false;
                }
            }
        }

        /// <summary>
        /// Kaydet tuþuna basýlýnca çalýþan olay metodu
        /// Geçici alýndý fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp kaydedilmek
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(Object sender, DirectEventArgs e)
        {
            Newtonsoft.Json.Linq.JArray satirlar = (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(e.ExtraParams["SATIRLAR"]);

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.fisTarih = new TNSDateTime(txtBelgeTarihi.RawText);
            tf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tf.ambarKod = txtAmbar.Text.Replace(".", "");
            tf.neredenGeldi = txtNeredenGeldi.Text;
            tf.faturaNo = txtNumara.Text;
            tf.faturaTarih = new TNSDateTime(txtFaturaTarihi.RawText);
            tf.islemTarih = new TNSDateTime(DateTime.Now);
            tf.islemYapan = kullanan.kullaniciKodu;

            int siraNo = 1;
            foreach (Newtonsoft.Json.Linq.JObject item in satirlar)
            {
                TasinirIslemDetay td = new TasinirIslemDetay();

                td.yil = tf.yil;
                td.muhasebeKod = tf.muhasebeKod;
                td.harcamaKod = tf.harcamaKod;
                td.ambarKod = tf.ambarKod;
                td.hesapPlanKod = TasinirGenel.DegerAlStr(item, "TASINIRHESAPKOD");
                td.miktar = OrtakFonksiyonlar.ConvertToDecimal(TasinirGenel.DegerAlDbl(item, "MIKTAR"));
                td.kdvOran = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.DegerAlInt(item, "KDVORANI"), 0);
                td.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(TasinirGenel.DegerAlDbl(item, "BIRIMFIYATI"));
                //Serverda hesaplanýyor (kdv mükellefi olan muhasebelerden dolayý commentlendi)
                //td.birimFiyatKDVLi = (1 + (OrtakFonksiyonlar.ConvertToDecimal(td.kdvOran) / 100)) * td.birimFiyat;

                if (!string.IsNullOrEmpty(td.hesapPlanKod))
                {
                    td.siraNo = siraNo++;
                    tf.detay.Ekle(td);
                }
            }

            Sonuc sonuc = servisTMM.GeciciAlindiFisiKaydet(kullanan, tf);
            if (sonuc.islemSonuc)
            {
                DurumAdDegistir(txtBelgeNo.Text == "" ? (int)ENUMBelgeDurumu.YENI : (int)ENUMBelgeDurumu.DEGISTIRILDI);

                txtBelgeNo.Text = sonuc.anahtar;
                TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
                GenelIslemler.ExtNotification(Resources.TasinirMal.FRMSYG017, "Bilgi", Icon.Information);
            }
            else
                GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
        }

        /// <summary>
        /// Listele resmine basýlýnca çalýþan olay metodu
        /// Sunucudan geçici alýndý fiþinin bilgileri alýnýr ve listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, DirectEventArgs e)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            TNS.TMM.TasinirIslemForm tff = new TNS.TMM.TasinirIslemForm();
            tff.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tff.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tff.muhasebeKod = txtMuhasebe.Text;
            tff.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');

            if (OrtakFonksiyonlar.ConvertToInt(tff.fisNo, 0) == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Fiþ Numarasý boþ býrakýlamaz.");
                return;
            }

            ObjectArray bilgi = servisTMM.GeciciAlindiFisiAc(kullanan, tff);

            if (!bilgi.sonuc.islemSonuc || bilgi.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Listelenecek kayýt bulunamadý." + bilgi.sonuc.hataStr);
                ListeTemizle();
                return;
            }

            TNS.TMM.TasinirIslemForm tf = (TNS.TMM.TasinirIslemForm)bilgi[0];
            txtBelgeNo.Text = tf.fisNo;
            txtBelgeTarihi.Text = tf.fisTarih.ToString();
            txtNumara.Text = tf.faturaNo;
            txtFaturaTarihi.Text = tf.faturaTarih.ToString();
            txtNeredenGeldi.Text = tf.neredenGeldi;
            hdnTIFFisNo.Value = tf.tifFisNo;
            if (tf.tifFisNo != "")
                lblTIF.Text = "TÝF Numarasý: " + "<a href = '../TasinirMal/TasinirislemFormAna.aspx?yil=" + tf.yil + "&muhasebe=" + tf.muhasebeKod + "&harcamaBirimi=" + tf.harcamaKod + "&belgeNo=" + tf.tifFisNo + "' target = '_blank' > " + tf.yil + "/" + tf.tifFisNo + " </ a > ";
            else
                lblTIF.Clear();

            txtHarcamaBirimi.Text = tf.harcamaKod;
            lblHarcamaBirimiAd.Text = tf.harcamaAd;

            txtAmbar.Text = tf.ambarKod;
            lblAmbarAd.Text = tf.ambarAd;

            DurumAdDegistir(tf.durum);

            List<object> liste = new List<object>();
            foreach (TasinirIslemDetay td in tf.detay.objeler)
            {
                liste.Add(new
                {
                    TASINIRHESAPKOD = td.hesapPlanKod,
                    TASINIRHESAPADI = td.hesapPlanAd,
                    MIKTAR = OrtakFonksiyonlar.ConvertToDbl(td.miktar),
                    OLCUBIRIMIKOD = td.olcuBirimAd,
                    KDVORANI = td.kdvOran,
                    BIRIMFIYATI = OrtakFonksiyonlar.ConvertToDbl(td.birimFiyat),
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Belge Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Geçici alýndý fiþinin bilgilerini sunucudan alýr ve excel dosyasýna yazýp kullanýcýya gönderir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnBelgeYazdir_Click(object sender, DirectEventArgs e)
        {
            FisAc(true, false);
        }

        /// <summary>
        /// Temizle tuþuna basýlýnca çalýþan olay metodu
        /// Kullanýcý tarafýndan sayfadaki kontrollere yazýlmýþ bilgiler temizlenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            ListeTemizle();
            txtYil.Value = DateTime.Now.Year;
            txtBelgeTarihi.Value = DateTime.Now.Date;
            txtBelgeNo.Clear();
            hdnTIFFisNo.Clear();
            txtNeredenGeldi.Clear();
            txtFaturaTarihi.Clear();
            txtNumara.Clear();
            lblFormDurum.Text = "";
            lblTIF.Text = "";
            if (txtMuhasebe.Text == "") lblMuhasebeAd.Text = "";
            if (txtHarcamaBirimi.Text == "") lblHarcamaBirimiAd.Text = "";
            if (txtAmbar.Text == "") lblAmbarAd.Text = "";
        }

        private void ListeTemizle()
        {
            List<object> liste = new List<object>();
            for (int i = 0; i < 10; i++)
            {
                liste.Add(new
                {
                    KOD = ""
                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Geçici alýndý fiþinin bilgilerini sunucudaki ilgili yordamdan ister,
        /// hata varsa hatayý görüntüler, yoksa sunucudan gelen bilgileri verilen
        /// parametrelere bakarak ExceleYaz yordamýna ya da TifOlustur yordamýna yönlendirir.
        /// </summary>
        /// <param name="excelMi">Geçici alýndý fiþinin excel raporu mu isteniyor bilgisi</param>
        /// <param name="tifMi">Geçici alýndý fiþinin taþýnýr iþlem fiþi mi oluþturulacak bilgisi</param>
        private void FisAc(bool excelMi, bool tifMi)
        {
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.fisNo = txtBelgeNo.Text.Trim();
            tf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            tf.muhasebeKod = txtMuhasebe.Text.Trim();
            tf.ambarKod = txtAmbar.Text.Trim();

            ObjectArray bilgi = servisTMM.GeciciAlindiFisiAc(kullanan, tf);

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

            tf = (TNS.TMM.TasinirIslemForm)bilgi[0];

            if (excelMi)
                ExceleYaz(tf);
            else if (tifMi)
                TifOlustur(tf);
        }

        /// <summary>
        /// TÝF Oluþtur tuþuna basýlýnca çalýþan olay metodu
        /// Geçici alýndý fiþinin bilgilerini sunucudan alýr, sessiona yazar ve taþýnýr iþlem fiþi ekranýna yönlendirir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTIF_Click(object sender, DirectEventArgs e)
        {
            FisAc(false, true);
        }

        /// <summary>
        /// Parametre olarak verilen geçici alýndý fiþini sessiona yazar ve taþýnýr iþlem fiþi ekranýna yönlendirme yapar.
        /// </summary>
        /// <param name="form">Geçici alýndý fiþi bilgilerini tutan nesne</param>
        private void TifOlustur(TNS.TMM.TasinirIslemForm form)
        {
            int aktifYil = DateTime.Now.Year;
            TNSDateTime tarih = new TNSDateTime(DateTime.Now.ToShortDateString());
            if (form.yil < aktifYil)
            {
                tarih = new TNSDateTime("31.12." + (aktifYil - 1).ToString());
            }

            form.fisNo = hdnTIFFisNo.Value.ToString();
            form.fisTarih = tarih;
            form.islemTipKod = (int)ENUMIslemTipi.SATINALMAGIRIS;

            Sonuc sonuc = servisTMM.TasinirIslemFisiKaydet(kullanan, form);

            if (sonuc.islemSonuc)
            {
                string belgeNo = sonuc.anahtar.Split('-')[0];
                string mesaj = "Taþýnýr Ýþlem Fiþi baþarýyla oluþturuldu.<br><br>Belge Numarasý: <a href='../TasinirMal/TasinirislemFormAna.aspx?yil=" + form.yil + "&muhasebe=" + form.muhasebeKod + "&harcamaBirimi=" + form.harcamaKod + "&belgeNo=" + belgeNo + "' target='_blank'>" + belgeNo + "</a>";

                form.fisNo = txtBelgeNo.Text.Trim();
                form.tifFisNo = belgeNo;
                hdnTIFFisNo.Value = belgeNo;

                sonuc = servisTMM.GeciciAlindiFisiTifFisNoKaydet(kullanan, form);
                if (!sonuc.islemSonuc)
                    mesaj += "<br><br>Hata: " + sonuc.hataStr;

                GenelIslemler.MesajKutusu("Bilgi", mesaj);
            }
            else
            {
                GenelIslemler.MesajKutusu("Hata", "Taþýnýr Ýþlem Fiþi oluþturma sýrasýnda hata oluþtu.<br>Hata: " + sonuc.hataStr);
            }
        }

        /// <summary>
        /// Parametre olarak verilen geçici alýndý fiþinin excel raporunu oluþturur.
        /// </summary>
        /// <param name="tf">Geçici alýndý fiþi bilgilerini tutan nesne</param>
        private void ExceleYaz(TNS.TMM.TasinirIslemForm tf)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TasinirGeciciAlindi.XLT";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("IlAd", tf.ilAd + "-" + tf.ilceAd);
            XLS.HucreAdBulYaz("IlKod", tf.ilKod + "-" + tf.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", tf.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", tf.harcamaKod);
            XLS.HucreAdBulYaz("AmbarAd", tf.ambarAd);
            XLS.HucreAdBulYaz("AmbarKod", tf.ambarKod);
            XLS.HucreAdBulYaz("MuhasebeAd", tf.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", tf.muhasebeKod);
            XLS.HucreAdBulYaz("FaturaTarih", tf.faturaTarih.ToString());
            XLS.HucreAdBulYaz("FaturaSayi", tf.faturaNo);
            XLS.HucreAdBulYaz("NeredenGeldi", tf.neredenGeldi);
            XLS.HucreAdBulYaz("BelgeTarih", tf.fisTarih.ToString());
            XLS.HucreAdBulYaz("BelgeNo", tf.fisNo);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            satir = kaynakSatir;

            int siraNo = 0;
            foreach (TasinirIslemDetay dt in tf.detay.objeler)
            {
                siraNo++;
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 3);

                XLS.HucreDegerYaz(satir, sutun, siraNo.ToString());
                XLS.HucreDegerYaz(satir, sutun + 1, dt.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 2, dt.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 4, dt.olcuBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(dt.miktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(dt.birimFiyat.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble((dt.miktar * dt.birimFiyat).ToString(), (double)0));
            }

            ImzaEkle(XLS, ref satir, sutun, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Geçici alýndý fiþi excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="satir">Ýmza bilgilerinin yazýlmaya baþlanacaðý satýr numarasý</param>
        /// <param name="sutun">Ýmza bilgilerinin yazýlmaya baþlanacaðý sütun numarasý</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <param name="ambar">Ambar kodu</param>
        private void ImzaEkle(Tablo XLS, ref int satir, int sutun, string muhasebe, string harcama, string ambar)
        {
            satir += 2;

            ObjectArray imza = servisTMM.ImzaListele(kullanan, muhasebe, harcama, ambar, (int)ENUMImzaYer.TASINIRKAYITYETKILISI);

            ImzaBilgisi iBilgi = new ImzaBilgisi();
            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
                iBilgi = (ImzaBilgisi)imza[0];

            XLS.SatirAc(satir, 9);
            XLS.HucreKopyala(0, sutun, 8, sutun + 7, satir, sutun);

            XLS.HucreDegerYaz(satir, sutun + 1, Resources.TasinirMal.FRMGAG002);
            XLS.HucreDegerYaz(satir + 2, sutun + 6, string.Format(Resources.TasinirMal.FRMGAG003, DateTime.Today.Date.ToShortDateString()));
            XLS.HucreDegerYaz(satir + 3, sutun + 4, Resources.TasinirMal.FRMGAG004);
            XLS.HucreDegerYaz(satir + 4, sutun + 4, string.Format(Resources.TasinirMal.FRMGAG005, iBilgi.adSoyad));
            XLS.HucreDegerYaz(satir + 5, sutun + 4, string.Format(Resources.TasinirMal.FRMGAG006, iBilgi.unvan));
            XLS.HucreDegerYaz(satir + 6, sutun + 4, Resources.TasinirMal.FRMGAG007);

            for (int i = satir; i <= satir + 6; i++)
            {
                if (i == satir)
                    XLS.HucreBirlestir(i, sutun + 1, i, sutun + 6);
                else if (i != satir + 2)
                    XLS.HucreBirlestir(i, sutun + 4, i, sutun + 6);

                if (i == satir + 3)
                    XLS.KoyuYap(i, sutun + 4, true);

                if (i == satir)
                {
                    XLS.DuseyHizala(i, sutun + 1, 0);
                    XLS.YatayHizala(i, sutun + 1, 2);
                }
                else if (i == satir + 3)
                {
                    XLS.DuseyHizala(i, sutun + 4, 2);
                    XLS.YatayHizala(i, sutun + 4, 2);
                }
                else
                {
                    XLS.DuseyHizala(i, sutun + 4, 0);
                    XLS.YatayHizala(i, sutun + 4, 2);
                }
            }

            XLS.YatayCizgiCiz(satir + 8, sutun, sutun + 7, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            XLS.DuseyCizgiCiz(satir - 1, satir + 7, sutun, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir - 1, satir + 7, sutun + 8, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            satir += 7;
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
                    modelKod = detay.modelKod
                });
            }
            return liste;
        }

        protected void btnOnaylaIslem_Click(object sender, DirectEventArgs e)
        {
            string islem = e.ExtraParams["ISLEM"];

            TNS.TMM.TasinirIslemForm form = new TNS.TMM.TasinirIslemForm();

            form.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            form.fisNo = txtBelgeNo.Text.Trim();
            form.harcamaKod = txtHarcamaBirimi.Text.Trim();
            form.muhasebeKod = txtMuhasebe.Text.Trim();
            form.ambarKod = txtAmbar.Text.Trim();

            Sonuc sonuc = servisTMM.GeciciAlindiFisiSicilOlustur(kullanan, form, islem);
            if (sonuc.islemSonuc)
            {
                DurumAdDegistir(islem == "Onay" ? (int)ENUMBelgeDurumu.ONAYLI : (int)ENUMBelgeDurumu.ONAYKALDIR);
                GenelIslemler.ExtNotification(sonuc.bilgiStr, "Bilgi", Icon.Information);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        private void DurumAdDegistir(int durum)
        {
            string durumAd = "";
            if (Arac.MerkezBankasiKullaniyor())
            {
                if (durum == (int)ENUMBelgeDurumu.YENI || durum == (int)ENUMBelgeDurumu.DEGISTIRILDI || durum == (int)ENUMBelgeDurumu.ONAYKALDIR)
                    durumAd = Resources.TasinirMal.FRMTIG055;
                else if (durum == (int)ENUMBelgeDurumu.ONAYLI)
                    durumAd = Resources.TasinirMal.FRMTIG056;
                else if (durum == (int)ENUMBelgeDurumu.IPTAL)
                    durumAd = Resources.TasinirMal.FRMTIG057;
                lblFormDurum.Text = durumAd;
            }
        }
    }
}