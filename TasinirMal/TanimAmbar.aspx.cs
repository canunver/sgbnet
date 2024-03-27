using System;
using OrtakClass;
using TNS;
using TNS.DEG;
using TNS.TMM;
using Ext1.Net;
using System.Xml;
using System.Collections.Generic;

namespace TasinirMal
{
    /// <summary>
    /// Ambar taným bilgilerinin kayýt, silme, listeleme ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TanimAmbar : TMMSayfaV2
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
                formAdi = Resources.TasinirMal.FRMTAM001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                //***Kullanýcý birimi þeklinde çalýþýyor ise devir ambarýný gösterme*****************************
                int devirSekli = OrtakFonksiyonlar.ConvertToInt(TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRKULLANICIBIRIMI"), 0);
                if (devirSekli > 0)
                    chkKBirimi.Style["display"] = "block";
                //***********************************************************************************************

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");

                if (Request.QueryString["menuYok"] == "1")
                {
                    tabPanelAna.Margins = "0 0 0 0";
                    tabPanelAna.StyleSpec += "padding:5px";
                    tabPanelAna.Border = true;
                    grdListe.Width = 200;
                }
                else
                    hdnSecKapat.Value = 1;

                Ambar sozluk = new Ambar();
                Listele(sozluk);
            }
        }

        /// <summary>
        /// Kaydet tuþuna basýlýnca çalýþan olay metodu
        /// Ýlçe taným bilgileri kaydedilmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata veya bilgi mesajý görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme iþlemi yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            string hata = "";

            if (txtMuhasebe.Text.Trim() == "")
                hata = Resources.TasinirMal.FRMTAM003 + "<br>";

            if (txtHarcamaBirimi.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTAM004 + "<br>";

            if (txtKod.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTAM005 + "<br>";

            if (txtAd.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTAM006 + "<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarý", hata);
                return;
            }

            Ambar d = new Ambar();
            d.muhasebeKod = txtMuhasebe.Text;
            d.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            d.kod = txtKod.Text;
            d.ad = txtAd.Text.Trim();
            d.adres = txtAdres.Text.Trim();
            d.TKKYAd = txtYetkili.Text.Trim();
            d.kapali = OrtakFonksiyonlar.ConvertToInt(chkKapali.Checked, 0);
            d.kullaniciBirimi = OrtakFonksiyonlar.ConvertToInt(chkKBirimi.Checked, 0);
            d.hediye = OrtakFonksiyonlar.ConvertToInt(chkHediye.Checked, 0);

            Sonuc sonuc = servisTMM.AmbarKaydet(kullanan, d);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                btnTemizle_Click(null, null);
                btnAra_Click(null, null);
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTMA005);
            }
        }

        /// <summary>
        /// Bul tuþuna basýlýnca çalýþan olay metodu
        /// Sunucudan Ambar taným bilgileri alýnýr ve listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Sil tuþuna basýlýnca çalýþan olay metodu
        /// Seçili olan Ambar silinmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata veya bilgi mesajý görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme iþlemi yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            TNS.TMM.Ambar d = new TNS.TMM.Ambar();
            d.muhasebeKod = txtMuhasebe.Text;
            d.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            d.kod = txtKod.Text;

            if (d.kod == "")
            {
                GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTMA007);
                return;
            }

            Sonuc sonuc = servisTMM.AmbarSil(kullanan, d);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                btnTemizle_Click(null, null);
                btnAra_Click(null, null);
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTMA006);
            }
        }

        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            txtKod.Clear();
            txtAd.Clear();
            txtMuhasebe.Clear();
            lblMuhasebeAd.Text = "";
            txtHarcamaBirimi.Clear();
            lblHarcamaBirimiAd.Text = "";
            txtAdres.Clear();
            txtYetkili.Clear();
            chkKapali.Clear();
            chkKBirimi.Clear();
            chkHediye.Clear();
        }

        protected void SatirSecildi(object sender, DirectEventArgs e)
        {
            string secilisatir = e.ExtraParams["GRIDPARAM"].ToString();
            XmlNode xml = JSON.DeserializeXmlNode("{records:{record:" + secilisatir + "}}");
            foreach (XmlNode row in xml.SelectNodes("records/record"))
            {
                Ambar kriter = new Ambar();
                kriter.kod = row.SelectSingleNode("KOD").InnerXml;
                kriter.muhasebeKod = row.SelectSingleNode("MUHASEBEKOD").InnerXml;
                kriter.harcamaBirimKod = row.SelectSingleNode("HARCAMABIRIMKOD").InnerXml;
                ObjectArray dler = servisTMM.AmbarListele(kullanan, kriter);
                foreach (Ambar dd in dler.objeler)
                {
                    txtKod.Text = dd.kod;
                    txtAd.Text = dd.ad;
                    txtMuhasebe.Text = dd.muhasebeKod;
                    lblMuhasebeAd.Text = dd.muhasebeAd;
                    txtHarcamaBirimi.Text = dd.harcamaBirimKod;
                    lblHarcamaBirimiAd.Text = dd.harcamaBirimAd;
                    txtAdres.Text = dd.adres;
                    txtYetkili.Text = dd.TKKYAd;
                    chkKapali.Checked = dd.kapali > 0;
                    chkKBirimi.Checked = dd.kullaniciBirimi > 0;
                    chkHediye.Checked = dd.hediye > 0;
                }

                //if (hdnSecKapat.Text == "1")
                //{
                //    X.AddScript("try { parent.kepAdresiEkle('" + hdnSeciliAdres.Text + "'); } catch (e) { }");
                //    return;
                //}
            }
        }

        /// <summary>
        /// Parametre olarak verilen Ambar nesnesi sunucuya gönderilir ve Ambar
        /// taným bilgileri alýnýr. Hata varsa ekrana hata bilgisi yazýlýr, yoksa
        /// gelen Ambar taným bilgileri gvAmbarlar GridView kontrolüne doldurulur.
        /// </summary>
        /// <param name="kriter">Ambar kriter bilgilerini tutan nesne</param>
        public void Listele(Ambar kriter)
        {
            ObjectArray dler = servisTMM.AmbarListele(kullanan, kriter);

            if (!dler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", dler.sonuc.hataStr);
                return;
            }

            List<object> liste = new List<object>();
            foreach (TNS.TMM.Ambar item in dler.objeler)
            {
                liste.Add(new
                {
                    MUHASEBEKOD = item.muhasebeKod,
                    HARCAMABIRIMKOD = item.harcamaBirimKod,
                    KOD = item.kod,
                    ADI = item.ad,
                    TKKYADI = item.TKKYAd
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden Ambar kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Ambar kriter bilgileri döndürülür.</returns>
        public Ambar KriterTopla()
        {
            TNS.TMM.Ambar d = new TNS.TMM.Ambar();
            d.muhasebeKod = txtMuhasebe.Text;
            d.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            d.kod = txtKod.Text;
            d.ad = txtAd.Text;
            d.adres = txtAdres.Text;
            d.TKKYAd = txtYetkili.Text;
            d.kapali = OrtakFonksiyonlar.ConvertToInt(chkKapali.Checked, 0);
            d.hediye = OrtakFonksiyonlar.ConvertToInt(chkHediye.Checked, 0);
            if (chkKBirimi.Checked)
                d.kullaniciBirimi = OrtakFonksiyonlar.ConvertToInt(chkKBirimi.Checked, -1);

            return d;
        }

        /// <summary>
        /// Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Sunucudan verilen kriterlere uygun olan ambar taným bilgileri
        /// alýnýr ve excel dosyasýna yazýlýp kullanýcýya gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, DirectEventArgs e)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "AmbarListesi.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

            kaynakSatir = satir;
            int siraNo = 0;

            ObjectArray bilgi = servisTMM.AmbarListele(kullanan, KriterTopla());

            foreach (Ambar amb in bilgi.objeler)
            {
                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

                siraNo++;

                XLS.HucreDegerYaz(satir, 0, siraNo);
                XLS.HucreDegerYaz(satir, 1, amb.il);
                XLS.HucreDegerYaz(satir, 2, amb.ilce);
                XLS.HucreDegerYaz(satir, 3, amb.muhasebeKod);
                XLS.HucreDegerYaz(satir, 4, amb.muhasebeAd);
                XLS.HucreDegerYaz(satir, 5, amb.harcamaBirimKod);
                XLS.HucreDegerYaz(satir, 6, amb.harcamaBirimAd);
                XLS.HucreDegerYaz(satir, 7, amb.kod);
                XLS.HucreDegerYaz(satir, 8, amb.ad);
                XLS.HucreDegerYaz(satir, 9, amb.adres);
                XLS.HucreDegerYaz(satir, 10, amb.TKKYAd +"-"+ amb.TKKYUnvan);
                XLS.HucreDegerYaz(satir, 11, amb.TKKYAd2 + "-" + amb.TKKYUnvan2);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
    }
}