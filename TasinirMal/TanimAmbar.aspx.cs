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
    /// Ambar tan�m bilgilerinin kay�t, silme, listeleme ve raporlama i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class TanimAmbar : TMMSayfaV2
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
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMTAM001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giri� izni varm�?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                //***Kullan�c� birimi �eklinde �al���yor ise devir ambar�n� g�sterme*****************************
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
        /// Kaydet tu�una bas�l�nca �al��an olay metodu
        /// �l�e tan�m bilgileri kaydedilmek �zere sunucuya g�nderilir,
        /// gelen sonuca g�re hata veya bilgi mesaj� g�r�nt�lenir.
        /// Son olarak g�ncel bilgilerin g�r�nmesi i�in listeleme i�lemi yap�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
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
                GenelIslemler.MesajKutusu("Uyar�", hata);
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
        /// Bul tu�una bas�l�nca �al��an olay metodu
        /// Sunucudan Ambar tan�m bilgileri al�n�r ve listelenir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Sil tu�una bas�l�nca �al��an olay metodu
        /// Se�ili olan Ambar silinmek �zere sunucuya g�nderilir,
        /// gelen sonuca g�re hata veya bilgi mesaj� g�r�nt�lenir.
        /// Son olarak g�ncel bilgilerin g�r�nmesi i�in listeleme i�lemi yap�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            TNS.TMM.Ambar d = new TNS.TMM.Ambar();
            d.muhasebeKod = txtMuhasebe.Text;
            d.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            d.kod = txtKod.Text;

            if (d.kod == "")
            {
                GenelIslemler.MesajKutusu("Uyar�", Resources.TasinirMal.FRMTMA007);
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
        /// Parametre olarak verilen Ambar nesnesi sunucuya g�nderilir ve Ambar
        /// tan�m bilgileri al�n�r. Hata varsa ekrana hata bilgisi yaz�l�r, yoksa
        /// gelen Ambar tan�m bilgileri gvAmbarlar GridView kontrol�ne doldurulur.
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
        /// Sayfadaki ilgili kontrollerden Ambar kriter bilgilerini toplayan ve d�nd�ren yordam
        /// </summary>
        /// <returns>Ambar kriter bilgileri d�nd�r�l�r.</returns>
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
        /// Yazd�r tu�una bas�l�nca �al��an olay metodu
        /// Sunucudan verilen kriterlere uygun olan ambar tan�m bilgileri
        /// al�n�r ve excel dosyas�na yaz�l�p kullan�c�ya g�nderilir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
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