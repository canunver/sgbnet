using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Web.UI;
using TNS;
using TNS.KYM;
using TNS.TMM;
using OrtakClass;
using Ext1.Net;
using System.Collections.Generic;
using TNS.DEG;
using System.Xml;

namespace TasinirMal
{
    /// <summary>
    /// Web formu ile ilgili olayları (event) ve fonksiyonları tutan sınıf
    /// </summary>
    public partial class Test : TMMSayfaV2
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
            if (!X.IsAjaxRequest)
            {
                formAdi = "Test İşlemleri";
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                Listele();
            }
        }

        public void Listele()
        {
            TNS.TMM.Arac.MerkezBankasiKullaniyor(); //DEGServis initialize

            List<object> liste = new List<object>();

            liste.Add(new { KOD = 1, AD = "E-Posta Gönderme İşlemi", });
            //liste.Add(new { KOD = 2, AD = "Kontrol", });

            strListe.DataSource = liste;
            strListe.DataBind();
        }

        protected void SatirSecildi(object sender, DirectEventArgs e)
        {
            string secilisatir = e.ExtraParams["GRIDPARAM"].ToString();
            XmlNode xml = JSON.DeserializeXmlNode("{records:{record:" + secilisatir + "}}");
            foreach (XmlNode row in xml.SelectNodes("records/record"))
            {
                string kod = row.SelectSingleNode("KOD").InnerXml;

                pnlMailGonder.Hide();
                pnlKontrol.Hide();

                if (kod == "1")
                    pnlMailGonder.Show();
                else if (kod == "2")
                    pnlKontrol.Show();
            }
        }

        protected void btnMailGonder_Click(object sender, DirectEventArgs e)
        {
            string hata = "";

            if (txtMailAdres.Text.Trim() == "")
                hata = "E-Posta adersi boş olamaz." + "<br>";
            else if (!OrtakFonksiyonlar.MailAdresiDogrumu(txtMailAdres.Text.Trim()))
                hata += "E-Posta adresi hatalı" + "<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarı", hata);
                return;
            }

            Sonuc sonuc = new Sonuc("Bilinmeyen bir hata oluştu.");
            if (e.ExtraParams["islem"] == "istemci")
            {
                try
                {
                    string yazilimMailAdresi = OrtakFonksiyonlar.WebConfigOku("yazilimMailAdresi", "");
                    string mailAdres = OrtakFonksiyonlar.ConvertToStr(txtMailAdres.Text.Trim());

                    string mail = OrtakFonksiyonlar.MailAt(yazilimMailAdresi, mailAdres, "E-Posta Testi (İstemci)", "Test", true, false, null);

                    if (mail != "" && mail != "1")
                        sonuc.hataStr = mail;
                    else
                    {
                        sonuc.islemSonuc = true;
                        sonuc.bilgiStr = mailAdres + "<br /> adresine mail gönderilmiştir.";
                    }
                }
                catch (Exception ex)
                {
                    sonuc.hataStr = "E-Posta Gönderilemedi.";
                    OrtakFonksiyonlar.HataStrYaz(ex.Message);
                }

            }
            else
            {
                sonuc = servisTMM.TestYap(kullanan, 1, txtMailAdres.Text.Trim());
            }

            if (sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);

        }

        protected void btnKontrol_Click(object sender, DirectEventArgs e)
        {

        }
    }
}
