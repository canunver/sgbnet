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
    /// Marka tanım bilgilerinin kayıt, silme ve listeleme işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class TanimMarka : TMMSayfaV2
    {
        /// <summary>
        /// Taşınır mal servisine ulaşmak için kullanılan değişken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayı:
        ///     Kullanıcı session'dan okunur.
        ///     Yetki kontrolü yapılır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMTMA001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
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

                TNS.TMM.Marka sozluk = new TNS.TMM.Marka();
                Listele(sozluk);
            }
        }

        /// <summary>
        /// Kaydet tuşuna basılınca çalışan olay metodu
        /// İlçe tanım bilgileri kaydedilmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata veya bilgi mesajı görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme işlemi yapılır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            string hata = "";

            if (txtAd.Text.Trim() == "")
            {
                if (hata != "") hata += "<br>";
                hata += Resources.TasinirMal.FRMTMA003;
            }

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarı", hata);
                return;
            }

            TNS.TMM.Marka d = new TNS.TMM.Marka();
            d.kod = OrtakFonksiyonlar.ConvertToInt(hdnKod.Value, 0);
            d.ad = txtAd.Text;

            Sonuc sonuc = servisTMM.MarkaKaydet(kullanan, d);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                btnTemizle_Click(null, null);
                btnAra_Click(null, null);
                GenelIslemler.ExtNotification(Resources.TasinirMal.FRMTMA005, "Bilgi", Icon.Lightbulb);
            }
        }

        /// <summary>
        /// Bul tuşuna basılınca çalışan olay metodu
        /// Sunucudan marka tanım bilgileri alınır ve listelenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Sil tuşuna basılınca çalışan olay metodu
        /// Seçili olan marka silinmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata veya bilgi mesajı görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme işlemi yapılır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            TNS.TMM.Marka d = new TNS.TMM.Marka();
            d.kod = OrtakFonksiyonlar.ConvertToInt(hdnKod.Value, 0);

            if (d.kod == 0)
            {
                GenelIslemler.MesajKutusu("Uyarı", Resources.TasinirMal.FRMTMA007);
                return;
            }

            Sonuc sonuc = servisTMM.MarkaSil(kullanan, d);

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
            hdnKod.Value = "";
            txtAd.Text = "";
        }

        protected void SatirSecildi(object sender, DirectEventArgs e)
        {
            string secilisatir = e.ExtraParams["GRIDPARAM"].ToString();
            XmlNode xml = JSON.DeserializeXmlNode("{records:{record:" + secilisatir + "}}");
            foreach (XmlNode row in xml.SelectNodes("records/record"))
            {
                Marka kriter = new Marka();
                kriter.kod = OrtakFonksiyonlar.ConvertToInt(row.SelectSingleNode("KOD").InnerXml, 0);
                ObjectArray dler = servisTMM.MarkaListele(kullanan, kriter);
                foreach (Marka dd in dler.objeler)
                {
                    hdnKod.Value = dd.kod;
                    txtAd.Text = dd.ad;
                }

                //if (hdnSecKapat.Text == "1")
                //{
                //    X.AddScript("try { parent.kepAdresiEkle('" + hdnSeciliAdres.Text + "'); } catch (e) { }");
                //    return;
                //}
            }
        }

        /// <summary>
        /// Parametre olarak verilen marka nesnesi sunucuya gönderilir ve marka
        /// tanım bilgileri alınır. Hata varsa ekrana hata bilgisi yazılır, yoksa
        /// gelen marka tanım bilgileri gvmarkalar GridView kontrolüne doldurulur.
        /// </summary>
        /// <param name="kriter">marka kriter bilgilerini tutan nesne</param>
        public void Listele(Marka kriter)
        {
            ObjectArray dler = servisTMM.MarkaListele(kullanan, kriter);

            if (!dler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", dler.sonuc.hataStr);
                return;
            }

            List<object> liste = new List<object>();
            foreach (TNS.TMM.Marka item in dler.objeler)
            {
                liste.Add(new
                {
                    KOD = item.kod,
                    ADI = item.ad,
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden marka kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>marka kriter bilgileri döndürülür.</returns>
        public Marka KriterTopla()
        {
            TNS.TMM.Marka d = new TNS.TMM.Marka();
            d.kod = OrtakFonksiyonlar.ConvertToInt(hdnKod.Value, 0);
            d.ad = txtAd.Text.Trim();

            return d;
        }
    }
}