using System;
using System.Data;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext1.Net;
using System.Xml;
using System.Collections.Generic;
using TNS.MUH;

namespace TasinirMal
{
    /// <summary>
    /// Enflasyon Düzeltmesi taným bilgilerinin kayýt, silme ve listeleme iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TanimEnflasyonDuzeltmesi : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = "Enflasyon Düzeltmesi Tanýmlama";
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

                txtYil.Text = DateTime.Now.Year.ToString();
                txtAy.Text = DateTime.Now.Month.ToString();

                Listele(new EnflasyonDuzeltmesi());
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
            EnflasyonDuzeltmesi d = KriterTopla();

            Sonuc sonuc = servisTMM.EnflasyonDuzeltmesiKaydet(kullanan, d);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                btnTemizle_Click(null, null);
                btnAra_Click(null, null);
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTOB002);
            }
        }

        /// <summary>
        /// Bul tuþuna basýlýnca çalýþan olay metodu
        /// Sunucudan olcubirim taným bilgileri alýnýr ve listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Sil tuþuna basýlýnca çalýþan olay metodu
        /// Seçili olan olcubirim silinmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata veya bilgi mesajý görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme iþlemi yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            EnflasyonDuzeltmesi d = new EnflasyonDuzeltmesi();
            d.kod = hdnSeciliKod.Value.ToString();

            if (d.kod == "")
            {
                GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTIL008);
                return;
            }

            Sonuc sonuc = servisTMM.EnflasyonDuzeltmesiSil(kullanan, d);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                btnTemizle_Click(null, null);
                btnAra_Click(null, null);
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTIC007);
            }
        }

        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            hdnSeciliKod.Value = "";
            txtAy.Text = "0";
            txtYiUfe.Text = "0";
            txtYiUfe.RawText = "0";
            txtSinir.Text = "0";
            txtSinir.RawText = "0";
        }

        protected void SatirSecildi(object sender, DirectEventArgs e)
        {
            string secilisatir = e.ExtraParams["GRIDPARAM"].ToString();
            XmlNode xml = JSON.DeserializeXmlNode("{records:{record:" + secilisatir + "}}");
            foreach (XmlNode row in xml.SelectNodes("records/record"))
            {
                EnflasyonDuzeltmesi kriter = new EnflasyonDuzeltmesi();
                kriter.kod = row.SelectSingleNode("KOD").InnerXml;
                ObjectArray dler = servisTMM.EnflasyonDuzeltmesiListele(kullanan, kriter);
                foreach (EnflasyonDuzeltmesi dd in dler.objeler)
                {
                    hdnSeciliKod.Value = dd.kod.ToString();
                    txtYil.Text = dd.yil.ToString();
                    txtAy.Text = dd.ay.ToString();
                    txtYiUfe.Text = dd.yiUfe.ToString();
                    txtSinir.Text = dd.sinir.ToString();
                }
            }
        }

        /// <summary>
        /// Parametre olarak verilen olcubirim nesnesi sunucuya gönderilir ve olcubirim
        /// taným bilgileri alýnýr. Hata varsa ekrana hata bilgisi yazýlýr, yoksa
        /// gelen olcubirim taným bilgileri gvolcubirimlar GridView kontrolüne doldurulur.
        /// </summary>
        /// <param name="kriter">olcubirim kriter bilgilerini tutan nesne</param>
        public void Listele(EnflasyonDuzeltmesi kriter)
        {
            ObjectArray dler = servisTMM.EnflasyonDuzeltmesiListele(kullanan, kriter);

            if (!dler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", dler.sonuc.hataStr);
                return;
            }

            List<object> liste = new List<object>();
            foreach (EnflasyonDuzeltmesi item in dler.objeler)
            {
                liste.Add(new
                {
                    KOD = item.kod,
                    YIL = item.yil,
                    AY = item.ay,
                    YIUFE = item.yiUfe,
                    SINIR = item.sinir,
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden olcubirim kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>olcubirim kriter bilgileri döndürülür.</returns>
        public EnflasyonDuzeltmesi KriterTopla()
        {
            EnflasyonDuzeltmesi d = new EnflasyonDuzeltmesi();
            d.kod = hdnSeciliKod.Value.ToString();
            d.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            d.ay = OrtakFonksiyonlar.ConvertToInt(txtAy.Text, 0);
            d.yiUfe = OrtakFonksiyonlar.ConvertToDouble(txtYiUfe.RawText, 0);
            d.sinir = OrtakFonksiyonlar.ConvertToDouble(txtSinir.RawText, 0);

            return d;
        }
    }
}