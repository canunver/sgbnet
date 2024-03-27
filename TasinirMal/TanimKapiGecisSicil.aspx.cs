using System;
using System.Data;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// RFID entegrasyonu olan kurumlarda kapılardan çıkışına izin verilen
    /// malzemelerin kayıt, listeleme ve raporlama işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class TanimKapiGecisSicil : TMMSayfa
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

            formAdi = Resources.TasinirMal.FRMTKS001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");
            this.txtHesapPlanKod.Attributes.Add("onblur", "kodAdGetir('30','lblHesapPlanAd',true,new Array('txtHesapPlanKod'),'KONTROLDENOKU');");

            if (!IsPostBack)
            {
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
            }
            else
                DegiskenSakla();

            if (!string.IsNullOrEmpty(txtMuhasebe.Text.Trim()))
                lblMuhasebeAd.Text = GenelIslemler.KodAd(31, txtMuhasebe.Text.Trim(), true);
            else
                lblMuhasebeAd.Text = string.Empty;

            if (!string.IsNullOrEmpty(txtHarcamaBirimi.Text.Trim()))
                lblHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim(), true);
            else
                lblHarcamaBirimiAd.Text = string.Empty;

            if (!string.IsNullOrEmpty(txtAmbar.Text.Trim()))
                lblAmbarAd.Text = GenelIslemler.KodAd(33, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim() + "-" + txtAmbar.Text.Trim(), true);
            else
                lblAmbarAd.Text = string.Empty;

            if (!string.IsNullOrEmpty(txtHesapPlanKod.Text.Trim()))
                lblHesapPlanAd.Text = GenelIslemler.KodAd(30, txtHesapPlanKod.Text.Trim(), true);
            else
                lblHesapPlanAd.Text = string.Empty;
        }

        /// <summary>
        /// Ekrandan seçilmiş olan muhasebe birimi, harcama birimi ve ambar
        /// bilgileri işlem yapan kullanıcının değişken listesine saklanır.
        /// </summary>
        private void DegiskenSakla()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
        }

        /// <summary>
        /// Kaydet tuşuna basılınca çalışan olay metodu
        /// Ekrandan seçilmiş olan demirbaşlar toplanır ve kapıdan geçişe izinli olarak kaydedilmek
        /// üzere sunucuya gönderilir, gelen sonuca göre hata veya bilgi mesajı görüntülenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnGecisYetkiKaydet_Click(object sender, EventArgs e)
        {
            Sonuc sonuc = servisTMM.KapiGecisSicilKaydet(kullanan, KriterToplaDetay(true));
            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTKS002);
                Listele(KriterTopla());
            }
            else
                GenelIslemler.MesajKutusu("Uyarı", sonuc.hataStr);
        }

        /// <summary>
        /// Sil tuşuna basılınca çalışan olay metodu
        /// Ekrandan seçilmiş olan demirbaşlar toplanır ve kapıdan geçiş izinleri silinmek
        /// üzere sunucuya gönderilir, gelen sonuca göre hata veya bilgi mesajı görüntülenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnGecisYetkiSil_Click(object sender, EventArgs e)
        {
            Sonuc sonuc = servisTMM.KapiGecisSicilSil(kullanan, KriterToplaDetay(false));
            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTKI003);
                Listele(KriterTopla());
            }
            else
                GenelIslemler.MesajKutusu("Uyarı", sonuc.hataStr);
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden demirbaş listeleme kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Demirbaş listeleme kriter bilgileri döndürülür.</returns>
        private TNS.TMM.SicilNoHareket KriterTopla()
        {
            TNS.TMM.SicilNoHareket kriter = new TNS.TMM.SicilNoHareket();
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaBirimKod = txtHarcamaBirimi.Text.Trim();
            kriter.ambarKod = txtAmbar.Text.Trim();
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim();
            kriter.sicilNo = txtSicilNo.Text.Trim();

            if (chkKGS.Checked)
            {
                kriter.kapiGecis= true;
            }
            return kriter;
        }

        /// <summary>
        /// Ekrandan seçilmiş olan demirbaşları toplayan ve döndüren yordam
        ///  type =  true : EKLEME işlemi,   false : Silme işlemi
        /// </summary>
        /// <returns>Seçilmiş demirbaşların listesi döndürülür.</returns>
        private TNSCollection KriterToplaDetay(bool type)
        {
            TNSCollection kriterler = new TNSCollection();

            for (int i = 0; i < dgListe.Items.Count; i++)
            {
                CheckBox chk = (CheckBox)dgListe.Items[i].FindControl("chkSecim");
                if (!chk.Checked)
                    continue;
                if ((type) && (dgListe.Items[i].Cells[1].Text.Trim().Length > 1))
                    continue;
                TNS.TMM.SicilNoHareket detay = new TNS.TMM.SicilNoHareket();
                detay.muhasebeKod = dgListe.Items[i].Cells[6].Text.Trim();
                detay.harcamaBirimKod = dgListe.Items[i].Cells[8].Text.Trim();
                detay.ambarKod = dgListe.Items[i].Cells[10].Text.Trim();
                detay.prSicilNo = OrtakFonksiyonlar.ConvertToInt(dgListe.Items[i].Cells[2].Text.Trim(), 0);

                kriterler.Add(detay);
            }

            return kriterler;
        }

        /// <summary>
        /// Listele tuşuna basılınca çalışan olay metodu
        /// Listeleme kriterlerini toplayan KriterTopla yordamı çağırılır ve gelen listeleme kriterleri
        /// kapıdan geçişine izin verilmiş ve verilmemiş demirbaşları listeleyen Listele yordamına gönderilir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, EventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Parametre olarak verilen demirbaş listeleme kriterleri sunucuya gönderilir ve kapıdan
        /// geçişine izin verilmiş ve verilmemiş demirbaşlar alınır. Hata varsa ekrana hata bilgisi
        /// yazılır, yoksa gelen demirbaş bilgileri dgListe DataGrid kontrolüne doldurulur.
        /// </summary>
        /// <param name="kriter">Demirbaş listeleme kriter bilgilerini tutan nesne</param>
        private void Listele(TNS.TMM.SicilNoHareket kriter)
        {
            ObjectArray objeler = servisTMM.KapiGecisSicilListele(kullanan, kriter);

            if (!objeler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", objeler.sonuc.hataStr);
                dgListe.DataBind();
                lblKacKayit.Visible = false;
                return;
            }

            if (objeler.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", objeler.sonuc.bilgiStr);
                dgListe.DataBind();
                lblKacKayit.Visible = false;
                return;
            }

            DataTable table = new DataTable();
            table.Columns.Add("yetki");
            table.Columns.Add("prSicilNo");
            table.Columns.Add("sicilNo");
            table.Columns.Add("hesapAd");
            table.Columns.Add("fiyat");
            table.Columns.Add("muhasebeKod");
            table.Columns.Add("muhasebeAd");
            table.Columns.Add("harcamaKod");
            table.Columns.Add("harcamaAd");
            table.Columns.Add("ambarKod");
            table.Columns.Add("ambarAd");
            table.Columns.Add("odaKodAd");
            table.Columns.Add("ozelllik");

            foreach (TNS.TMM.SicilNoHareket demirbas in objeler.objeler)
            {
                if (TasinirGenel.rfIdVarMi)
                {
                    table.Rows.Add((demirbas.kapiGecis ? "VAR" : "-"),
                                   demirbas.prSicilNo, demirbas.sicilNo + " - "+demirbas.rfIdNo.ToString(), demirbas.hesapPlanAd, demirbas.fiyat.ToString("#,###.0000"),
                                   demirbas.muhasebeKod , demirbas.muhasebeAd,
                                   demirbas.harcamaBirimKod , demirbas.harcamaBirimAd,
                                   demirbas.ambarKod , demirbas.ambarAd,
                                   demirbas.odaKod + " - " + demirbas.odaAd,
                                   (demirbas.ozellik == null) ? "-" : (string.IsNullOrEmpty(demirbas.ozellik.adi) ? "-" : demirbas.ozellik.adi));
                }
                else
                {
                    table.Rows.Add((demirbas.kapiGecis ? "VAR" : "-"), 
                                   demirbas.prSicilNo, demirbas.sicilNo, demirbas.hesapPlanAd, demirbas.fiyat.ToString("#,###.0000"),
                                   demirbas.muhasebeKod , demirbas.muhasebeAd,
                                   demirbas.harcamaBirimKod , demirbas.harcamaBirimAd,
                                   demirbas.ambarKod , demirbas.ambarAd,
                                   demirbas.odaKod,
                                   (demirbas.ozellik == null) ? "-" : (string.IsNullOrEmpty(demirbas.ozellik.adi) ? "-" : demirbas.ozellik.adi));
                }
            }

            dgListe.DataSource = table;
            dgListe.DataBind();

            //for (int i = 0; i < dgListe.Items.Count; i++)
            //{
            //    CheckBox chk = (CheckBox)dgListe.Items[i].FindControl("chkSecim");
            //    chk.Checked = ((TNS.TMM.SicilNoHareket)objeler.objeler[i]).kapiGecis;
            //}

            lblKacKayit.Visible = true;
            lblKacKayit.Text = string.Format(Resources.TasinirMal.FRMTKS003, objeler.objeler.Count.ToString());
        }

        /// <summary>
        /// Log Yazdır tuşuna basılınca çalışan olay metodu
        /// Sunucudan verilen kriterlere uygun olan demirbaş bilgileri
        /// alınır ve excel dosyasına yazılıp kullanıcıya gönderilir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnLogYaz_Click(object sender, EventArgs e)
        {
            TNS.TMM.KapiGecisLog kgl = new TNS.TMM.KapiGecisLog();
            kgl.snh = KriterTopla();
            ObjectArray bilgi = servisTMM.KapiGecisLogListele(kullanan, kgl, new TNSDateTime(txtTarih1.Value), new TNSDateTime(txtTarih2.Value));

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                //dgListe.Visible = false;
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                //dgListe.Visible = false;
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();

            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "KGSLog.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            foreach (TNS.TMM.KapiGecisLog l in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 9, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, l.snh.sicilNo);
                XLS.HucreDegerYaz(satir, sutun + 1, l.snh.rfIdNo);
                XLS.HucreDegerYaz(satir, sutun + 2, l.snh.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 3, l.snh.personelKod + " - " + l.snh.kisiAd);
                XLS.HucreDegerYaz(satir, sutun + 4, l.snh.odaKod + " - " + l.snh.odaAd);
                XLS.HucreDegerYaz(satir, sutun + 5, l.tcKimlikNo + " - " + l.personelAd);
                XLS.HucreDegerYaz(satir, sutun + 6, l.tarih.ToLongString());
                XLS.HucreDegerYaz(satir, sutun + 7, l.snh.muhasebeKod + " - " + l.snh.muhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 8, l.snh.harcamaBirimKod + " - " + l.snh.harcamaBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 9, l.snh.ambarKod + " - " + l.snh.ambarAd);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(500));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Çıkış Formu Yazdır tuşuna basılınca çalışan olay metodu
        /// Seçilen demirbaş bilgileri
        /// alınır ve excel dosyasına yazılıp kullanıcıya gönderilir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnCikisFormYaz_Click(object sender, EventArgs e)
        {


            Tablo XLS = GenelIslemler.NewTablo();

            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "ZIFCIKIS.XLT";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            for (int i = 0; i < dgListe.Items.Count; i++)
            {
                CheckBox chk = (CheckBox)dgListe.Items[i].FindControl("chkSecim");
                if (!chk.Checked)
                    continue;

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 9, satir, sutun);

                XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 2);
                XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 4);

                XLS.HucreDegerYaz(satir, sutun, satir - kaynakSatir);
                XLS.HucreDegerYaz(satir, sutun + 1, dgListe.Items[i].Cells[2].Text.Trim());
                XLS.HucreDegerYaz(satir, sutun + 3, dgListe.Items[i].Cells[3].Text.Trim());
                XLS.HucreDegerYaz(satir, sutun + 5, dgListe.Items[i].Cells[12].Text.Trim());
                XLS.HucreDegerYaz(satir, sutun + 6, (string.IsNullOrEmpty(dgListe.Items[i].Cells[11].Text) ? "-" : dgListe.Items[i].Cells[11].Text.Trim()));
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(500));
            XLS.DosyaSaklaTamYol(); 
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
    }
}