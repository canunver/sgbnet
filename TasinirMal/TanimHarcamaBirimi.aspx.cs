using System;
using System.Data;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext1.Net;
using System.Xml;
using System.Collections.Generic;

namespace TasinirMal
{
    /// <summary>
    /// Harcama birimi taným bilgilerinin kayýt, silme, listeleme ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TanimHarcamaBirimi : TMMSayfaV2
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
                formAdi = Resources.TasinirMal.FRMTHB001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                if (Request.QueryString["menuYok"] == "1")
                {
                    tabPanelAna.Margins = "0 0 0 0";
                    tabPanelAna.StyleSpec += "padding:5px";
                    tabPanelAna.Border = true;
                    grdListe.Width = 200;
                }
                else
                    hdnSecKapat.Value = 1;

                HarcamaBirimi sozluk = new HarcamaBirimi();
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
            {
                if (hata != "") hata += "<br>";
                hata = Resources.TasinirMal.FRMTHB003;
            }
            if (txtKod.Text.Trim() == "")
            {
                if (hata != "") hata += "<br>";
                hata += Resources.TasinirMal.FRMTHB004;
            }
            if (txtAd.Text.Trim() == "")
            {
                if (hata != "") hata += "<br>";
                hata += Resources.TasinirMal.FRMTHB005;
            }

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarý", hata);
                return;
            }

            HarcamaBirimi d = new HarcamaBirimi();
            d.kod = txtKod.Text.Replace(".", "");
            d.ad = txtAd.Text;
            d.muhasebeKod = txtMuhasebe.Text;
            d.kapali = OrtakFonksiyonlar.ConvertToInt(chkKapali.Checked, 0);

            Sonuc sonuc = servisTMM.HarcamaBirimiKaydet(kullanan, d);

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
        /// Sunucudan HarcamaBirimi taným bilgileri alýnýr ve listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Sil tuþuna basýlýnca çalýþan olay metodu
        /// Seçili olan HarcamaBirimi silinmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata veya bilgi mesajý görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme iþlemi yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            if (txtKod.Text.Trim() == "" || txtMuhasebe.Text.Trim() == "")
            {
                GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTMA007);
                return;
            }

            HarcamaBirimi d = new HarcamaBirimi();
            d.kod = txtKod.Text.Replace(".", "");
            d.muhasebeKod = txtMuhasebe.Text;

            Sonuc sonuc = servisTMM.HarcamaBirimiSil(kullanan, d);

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
            chkKapali.Clear();
        }

        protected void SatirSecildi(object sender, DirectEventArgs e)
        {
            string secilisatir = e.ExtraParams["GRIDPARAM"].ToString();
            XmlNode xml = JSON.DeserializeXmlNode("{records:{record:" + secilisatir + "}}");
            foreach (XmlNode row in xml.SelectNodes("records/record"))
            {
                HarcamaBirimi kriter = new HarcamaBirimi();
                kriter.kod = row.SelectSingleNode("KOD").InnerXml;
                kriter.muhasebeKod= row.SelectSingleNode("MUHASEBEKOD").InnerXml;
                ObjectArray dler = servisTMM.HarcamaBirimiListele(kullanan, kriter);
                foreach (HarcamaBirimi dd in dler.objeler)
                {
                    txtKod.Text = dd.kod;
                    txtAd.Text = dd.ad;
                    txtMuhasebe.Text = dd.muhasebeKod;
                    lblMuhasebeAd.Text = dd.muhasebeAd;
                    chkKapali.Checked = dd.kapali > 0;
                }

                //if (hdnSecKapat.Text == "1")
                //{
                //    X.AddScript("try { parent.kepAdresiEkle('" + hdnSeciliAdres.Text + "'); } catch (e) { }");
                //    return;
                //}
            }
        }

        /// <summary>
        /// Parametre olarak verilen HarcamaBirimi nesnesi sunucuya gönderilir ve HarcamaBirimi
        /// taným bilgileri alýnýr. Hata varsa ekrana hata bilgisi yazýlýr, yoksa
        /// gelen HarcamaBirimi taným bilgileri gvHarcamaBirimilar GridView kontrolüne doldurulur.
        /// </summary>
        /// <param name="kriter">HarcamaBirimi kriter bilgilerini tutan nesne</param>
        public void Listele(HarcamaBirimi kriter)
        {
            ObjectArray dler = servisTMM.HarcamaBirimiListele(kullanan, kriter);

            if (!dler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", dler.sonuc.hataStr);
                return;
            }

            List<object> liste = new List<object>();
            foreach (HarcamaBirimi item in dler.objeler)
            {
                liste.Add(new
                {
                    MUHASEBEKOD = item.muhasebeKod,
                    KOD = item.kod,
                    ADI = item.ad,
                    KAPALI = item.kapali
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden HarcamaBirimi kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>HarcamaBirimi kriter bilgileri döndürülür.</returns>
        public HarcamaBirimi KriterTopla()
        {
            HarcamaBirimi d = new HarcamaBirimi();

            d.muhasebeKod = txtMuhasebe.Text;
            d.kod = txtKod.Text.Replace(".", "");
            d.ad = txtAd.Text;
            d.kapali = OrtakFonksiyonlar.ConvertToInt(chkKapali.Checked, 0);
            return d;
        }

        /// <summary>
        /// Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Sunucudan verilen kriterlere uygun olan harcama birimi taným
        /// bilgileri alýnýr ve excel dosyasýna yazýlýp kullanýcýya gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "HarcamaBirimiListesi.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

            kaynakSatir = satir;
            int siraNo = 0;

            ObjectArray bilgi = servisTMM.HarcamaBirimiListele(kullanan, KriterTopla());

            foreach (HarcamaBirimi hb in bilgi.objeler)
            {
                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);

                siraNo++;

                XLS.HucreDegerYaz(satir, 0, siraNo);
                XLS.HucreDegerYaz(satir, 1, hb.il);
                XLS.HucreDegerYaz(satir, 2, hb.ilce);
                XLS.HucreDegerYaz(satir, 3, hb.muhasebeKod);
                XLS.HucreDegerYaz(satir, 4, hb.muhasebeAd);
                XLS.HucreDegerYaz(satir, 5, hb.kod);
                XLS.HucreDegerYaz(satir, 6, hb.ad);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
    }
}