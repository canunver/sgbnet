using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Xml;
using System.Collections.Generic;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// İl tanım bilgilerinin kayıt, silme ve listeleme işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class TanimIl : TMMSayfaV2
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
                formAdi = Resources.TasinirMal.FRMTIL001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                if (Request.QueryString["menuYok"] == "1")
                {
                    tabPanelAna.Margins = "0 0 0 0";
                    tabPanelAna.StyleSpec += "padding:5px";
                    //tabPanelAna.Border = true;
                    grdListe.Width = 200;
                }
                else
                    hdnSecKapat.Value = 1;

                Il sozluk = new Il();
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

            if (txtKod.Text.Trim() == "")
            {
                if (hata != "") hata += "<br>";
                hata += Resources.TasinirMal.FRMTIL003;
            }

            if (txtAd.Text.Trim() == "")
            {
                if (hata != "") hata += "<br>";
                hata += Resources.TasinirMal.FRMTIL004;
            }

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarı", hata);
                return;
            }

            Il d = new Il();
            d.kod = txtKod.Text.Trim();
            d.ad = txtAd.Text;

            Sonuc sonuc = servisTMM.IlKaydet(kullanan, d);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                btnTemizle_Click(null, null);
                btnAra_Click(null, null);
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTIL006);
            }
        }

        /// <summary>
        /// Bul tuşuna basılınca çalışan olay metodu
        /// Sunucudan il tanım bilgileri alınır ve listelenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Sil tuşuna basılınca çalışan olay metodu
        /// Seçili olan il silinmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata veya bilgi mesajı görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme işlemi yapılır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            Il d = new Il();
            d.kod = txtKod.Text.Trim();

            if (string.IsNullOrEmpty(d.kod))
            {
                GenelIslemler.MesajKutusu("Hata", Resources.TasinirMal.FRMTIL008);
                return;
            }

            Sonuc sonuc = servisTMM.IlSil(kullanan, d);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                btnTemizle_Click(null, null);
                btnAra_Click(null, null);
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTIL007);
            }
        }

        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            txtKod.Text = "";
            txtAd.Text = "";
        }

        protected void SatirSecildi(object sender, DirectEventArgs e)
        {
            string secilisatir = e.ExtraParams["GRIDPARAM"].ToString();
            XmlNode xml = JSON.DeserializeXmlNode("{records:{record:" + secilisatir + "}}");
            foreach (XmlNode row in xml.SelectNodes("records/record"))
            {
                Il kriter = new Il();
                kriter.kod = row.SelectSingleNode("KOD").InnerXml;
                ObjectArray dler = servisTMM.IlListele(kullanan, kriter);
                foreach (Il dd in dler.objeler)
                {
                    txtKod.Text = dd.kod;
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
        /// Parametre olarak verilen il nesnesi sunucuya gönderilir ve il
        /// tanım bilgileri alınır. Hata varsa ekrana hata bilgisi yazılır, yoksa
        /// gelen il tanım bilgileri gvIller GridView kontrolüne doldurulur.
        /// </summary>
        /// <param name="kriter">İl kriter bilgilerini tutan nesne</param>
        public void Listele(Il kriter)
        {
            ObjectArray dler = servisTMM.IlListele(kullanan, kriter);

            if (!dler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", dler.sonuc.hataStr);
                return;
            }

            List<object> liste = new List<object>();
            foreach (Il item in dler.objeler)
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
        /// Sayfadaki ilgili kontrollerden il kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>İlçe kriter bilgileri döndürülür.</returns>
        public Il KriterTopla()
        {
            Il d = new Il();
            d.kod = txtKod.Text.Trim();
            d.ad = txtAd.Text.Trim();

            return d;
        }
    }
}