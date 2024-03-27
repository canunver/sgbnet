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
    /// Enflasyon D�zeltmesi tan�m bilgilerinin kay�t, silme ve listeleme i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class TanimEnflasyonDuzeltmesi : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = "Enflasyon D�zeltmesi Tan�mlama";
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giri� izni varm�?
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
        /// Kaydet tu�una bas�l�nca �al��an olay metodu
        /// �l�e tan�m bilgileri kaydedilmek �zere sunucuya g�nderilir,
        /// gelen sonuca g�re hata veya bilgi mesaj� g�r�nt�lenir.
        /// Son olarak g�ncel bilgilerin g�r�nmesi i�in listeleme i�lemi yap�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
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
        /// Bul tu�una bas�l�nca �al��an olay metodu
        /// Sunucudan olcubirim tan�m bilgileri al�n�r ve listelenir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Sil tu�una bas�l�nca �al��an olay metodu
        /// Se�ili olan olcubirim silinmek �zere sunucuya g�nderilir,
        /// gelen sonuca g�re hata veya bilgi mesaj� g�r�nt�lenir.
        /// Son olarak g�ncel bilgilerin g�r�nmesi i�in listeleme i�lemi yap�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            EnflasyonDuzeltmesi d = new EnflasyonDuzeltmesi();
            d.kod = hdnSeciliKod.Value.ToString();

            if (d.kod == "")
            {
                GenelIslemler.MesajKutusu("Uyar�", Resources.TasinirMal.FRMTIL008);
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
        /// Parametre olarak verilen olcubirim nesnesi sunucuya g�nderilir ve olcubirim
        /// tan�m bilgileri al�n�r. Hata varsa ekrana hata bilgisi yaz�l�r, yoksa
        /// gelen olcubirim tan�m bilgileri gvolcubirimlar GridView kontrol�ne doldurulur.
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
        /// Sayfadaki ilgili kontrollerden olcubirim kriter bilgilerini toplayan ve d�nd�ren yordam
        /// </summary>
        /// <returns>olcubirim kriter bilgileri d�nd�r�l�r.</returns>
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