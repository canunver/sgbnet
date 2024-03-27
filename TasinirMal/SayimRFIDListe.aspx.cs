using System;
using System.Data;
using System.Web.UI;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Web.UI.WebControls;

namespace TasinirMal
{
    /// <summary>
    /// RFID entegrasyonu olan kurumlarda RFID kullanılarak sayım işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class SayimRFIDListe : TMMSayfa
    {
        /// <summary>
        /// Taşınır mal servisine ulaşmak için kullanılan değişken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayı:
        ///     Kullanıcı session'dan okunur.
        ///     Yetki kontrolü yapılır.
        ///     Sayfa ilk defa çağırılıyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMSRF001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.btnKaydet.Attributes.Add("onclick", "return OnayAl('Kaydet','btnKaydet');");
            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");
            this.txtPersonel.Attributes.Add("onblur", "kodAdGetir('36','lblPersonelAd',true,new Array('txtPersonel'),'KONTROLDENOKU');");
            this.txtOda.Attributes.Add("onblur", "kodAdGetir('35','lblOdaAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtOda'),'KONTROLDENOKU');");

            Page.ClientScript.GetPostBackEventReference(this, String.Empty);

            if (!IsPostBack)
            {
                GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
                DurumDoldur();
                txtSayimTarih.Text = DateTime.Now.ToShortDateString();

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
            }

            if (txtMuhasebe.Text.Trim() != "")
                lblMuhasebeAd.Text = GenelIslemler.KodAd(31, txtMuhasebe.Text.Trim(), true);
            else
                lblMuhasebeAd.Text = "";

            if (txtHarcamaBirimi.Text.Trim() != "")
                lblHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim(), true);
            else
                lblHarcamaBirimiAd.Text = "";

            if (txtAmbar.Text.Trim() != "")
                lblAmbarAd.Text = GenelIslemler.KodAd(33, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim() + "-" + txtAmbar.Text.Trim(), true);
            else
                lblAmbarAd.Text = "";

            if (txtPersonel.Text.Trim() != "")
                lblPersonelAd.Text = GenelIslemler.KodAd(36, txtPersonel.Text.Trim(), true);
            else
                lblPersonelAd.Text = "";

            if (txtOda.Text.Trim() != "")
                lblOdaAd.Text = GenelIslemler.KodAd(35, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim() + "-" + txtOda.Text.Trim(), true);
            else
                lblOdaAd.Text = "";
        }

        private void DurumDoldur()
        {
            ddlDurum.Items.Clear();
            ddlDurum.Items.Add(new ListItem(Resources.TasinirMal.FRMSRF027, System.Enum.GetName(typeof(ENUMSayimRFIDDurum), ENUMSayimRFIDDurum.TANIMSIZ)));
            ddlDurum.Items.Add(new ListItem(Resources.TasinirMal.FRMSRF028, System.Enum.GetName(typeof(ENUMSayimRFIDDurum), ENUMSayimRFIDDurum.DEVAMEDIYOR)));
            ddlDurum.Items.Add(new ListItem(Resources.TasinirMal.FRMSRF029, System.Enum.GetName(typeof(ENUMSayimRFIDDurum), ENUMSayimRFIDDurum.BITTI)));
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden RFID sayım kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <param name="detay">Seçili sayıma ilişkin kriterler de alınsın mı bilgisi</param>
        /// <returns>RFID sayım kriter bilgileri döndürülür.</returns>
        private SayimRFID KriterTopla(bool detay)
        {
            SayimRFID sf = new SayimRFID();
            sf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);

            if (detay)
                sf.sayimNo = hdnSayimNo.Value;
            else
            {
                sf.sayimTarih.Yaz(DateTime.Now);
                sf.durum = (ENUMSayimRFIDDurum)System.Enum.Parse(typeof(ENUMSayimRFIDDurum), ddlDurum.SelectedValue.Trim());
                sf.muhasebeKod = txtMuhasebe.Text.Trim();
                sf.harcamaKod = txtHarcamaBirimi.Text.Trim();
                sf.ambarKod = txtAmbar.Text.Trim();
                sf.sayimAdi = txtSayimAd.Text.Trim();
                sf.tcKimlikNo = txtPersonel.Text.Trim();
                sf.odaKod = txtOda.Text.Trim();
            }

            return sf;
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            Sonuc sonuc = servisTMM.RFIDSayimBaslat(kullanan, KriterTopla(false));
            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
                hdnSayimNo.Value = sonuc.anahtar;
            }
            else
                GenelIslemler.MesajKutusu("Uyarı", sonuc.hataStr);
        }

        /// <summary>
        /// Listeleme kriterleri RFID sayım nesnesinde parametre olarak alınır,
        /// sunucuya gönderilir ve RFID sayım bilgileri sunucudan alınır.
        /// Hata varsa ekrana hata bilgisi yazılır, yoksa gelen bilgiler ekrana
        /// veya excel dosyasına yazılmak üzere ilgili yordama yönlendirilir.
        /// Detay istenmişse excel dosyasına, istenmemişse ekrana listelenir.
        /// </summary>
        /// <param name="sf">RFID sayım listeleme kriterlerini tutan nesne</param>
        /// <param name="detay">Listeleme işlemi sayım detay bilgilerini de getirsin mi bilgisi</param>
        private void Listele(SayimRFID sf, bool detay)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            ObjectArray bilgi = servisTMM.RFIDSayimListele(kullanan, sf, detay);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                dgListe.Visible = false;
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                dgListe.Visible = false;
                return;
            }

            if (detay)
                ExceleYaz((SayimRFID)bilgi.objeler[0]);
            else
                EkranaYaz(bilgi.objeler);
        }

        /// <summary>
        /// Parametre olarak verilen RFID sayım bilgilerini dgListe DataGrid kontrolüne dolduran yordam
        /// </summary>
        /// <param name="bilgi">RFID sayım bilgilerini tutan nesne</param>
        private void EkranaYaz(TNSCollection bilgi)
        {
            DataTable tablo = new DataTable();
            tablo.Columns.Add("Sayim");
            tablo.Columns.Add("Yil");
            tablo.Columns.Add("Muhasebe");
            tablo.Columns.Add("Harcama");
            tablo.Columns.Add("Ambar");
            tablo.Columns.Add("Personel");
            tablo.Columns.Add("Oda");
            tablo.Columns.Add("Durum");
            tablo.Columns.Add("Tarih");

            foreach (SayimRFID srfid in bilgi)
            {
                string durum = Resources.TasinirMal.FRMSRF033;
                if (srfid.durum == ENUMSayimRFIDDurum.BITTI)
                    durum = Resources.TasinirMal.FRMSRF034;

                string link = "<a href=\"javascript:Yazdir('" + srfid.sayimNo + "')\">" + srfid.sayimNo + " - " + srfid.sayimAdi + "</a>";
                tablo.Rows.Add(link, srfid.yil, srfid.muhasebeKod + " - " + srfid.muhasebeAd, srfid.harcamaKod + " - " + srfid.harcamaAd,
                    srfid.ambarKod + " - " + srfid.ambarAd, srfid.tcKimlikNo + " - " + srfid.kisiAd, srfid.odaKod + " - " + srfid.odaAd,
                    durum, srfid.sayimTarih);
            }

            dgListe.DataSource = tablo;
            dgListe.DataBind();
            dgListe.Visible = true;
        }

        /// <summary>
        /// Listele tuşuna basılınca çalışan olay metodu
        /// Sunucudan RFID sayım bilgileri alınır ve listelenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, EventArgs e)
        {
            Listele(KriterTopla(false), false);
        }

        /// <summary>
        /// Listedeki RFID sayım linklerinden herhangi birine basılınca çalışan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çağırılır ve toplanan
        /// kriterler Listele yordamına gönderilir ve rapor hazırlanmış olur.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            Listele(KriterTopla(true), true);
        }

        /// <summary>
        /// Parametre olarak verilen RFID sayım bilgileri excel dosyasına yazılıp kullanıcıya gönderilir.
        /// </summary>
        /// <param name="sf">Yazdırılacak RFID sayım bilgilerini tutan nesne</param>
        private void ExceleYaz(SayimRFID sf)
        {
            Tablo XLS = GenelIslemler.NewTablo();

            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "RFIDSAYIMTUTANAK.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("Muhasebe", sf.muhasebeKod + " - " + sf.muhasebeAd);
            XLS.HucreAdBulYaz("Harcama", sf.harcamaKod + " - " + sf.harcamaAd);
            XLS.HucreAdBulYaz("Ambar", sf.ambarKod + " - " + sf.ambarAd);
            XLS.HucreAdBulYaz("Personel", sf.tcKimlikNo + " - " + sf.kisiAd);
            XLS.HucreAdBulYaz("Oda", sf.odaKod + " - " + sf.odaAd);
            XLS.HucreAdBulYaz("SayimNoAd", sf.sayimNo + " - " + sf.sayimAdi);
            XLS.HucreAdBulYaz("Durum", sf.durum == ENUMSayimRFIDDurum.BITTI ? Resources.TasinirMal.FRMSRF034 : Resources.TasinirMal.FRMSRF033);
            XLS.HucreAdBulYaz("SayimTarih", sf.sayimTarih.ToString());

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            satir = kaynakSatir;
            foreach (SayimRFIDDetay sfd in sf.detay)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, sfd.sicilNo);
                XLS.HucreDegerYaz(satir, sutun + 1, sfd.sicilAd);
                XLS.HucreDegerYaz(satir, sutun + 2, sfd.bulunanKisiKod + " - " + sfd.bulunanKisiAd);
                XLS.HucreDegerYaz(satir, sutun + 3, sfd.beklenenKisiKod + " - " + sfd.beklenenKisiAd);

                XLS.HucreDegerYaz(satir, sutun + 4, sfd.bulunanOdaKod + " - " + sfd.bulunanOdaAd);
                XLS.HucreDegerYaz(satir, sutun + 5, sfd.beklenenOdaKod + " - " + sfd.beklenenOdaAd);
                XLS.HucreDegerYaz(satir, sutun + 6, sfd.beklenenMuhasebeKod + " - " + sfd.beklenenMuhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 7, sfd.beklenenHarcamaBirimKod + " - " + sfd.beklenenHarcamaBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 8, sfd.beklenenAmbarKod + " - " + sfd.beklenenAmbarAd);
                XLS.HucreDegerYaz(satir, sutun + 9, sfd.bulunanTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 11, sfd.terminalKod);

                if (sfd.durum == (int)ENUMSayimRFIDDemirbasDurum.SAYIMDABULUNAMAYAN)
                {
                    XLS.HucreDegerYaz(satir, sutun + 10, Resources.TasinirMal.FRMSRF002);
                    XLS.ArkaPlanRenk(satir, sutun + 10, OrtakClass.TabloRenk.RED);
                }
                else if (sfd.durum == ENUMSayimRFIDDemirbasDurum.SAYIMDABULUNAN)
                    XLS.HucreDegerYaz(satir, sutun + 10, Resources.TasinirMal.FRMSRF003);
                else if (sfd.durum == ENUMSayimRFIDDemirbasDurum.SAYIMFAZLASI)
                {
                    XLS.HucreDegerYaz(satir, sutun + 10, Resources.TasinirMal.FRMSRF004);
                    XLS.ArkaPlanRenk(satir, sutun + 10, OrtakClass.TabloRenk.RED);
                }
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(440));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
    }
}