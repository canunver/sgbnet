using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext.Net;
using System.Xml;
using System.Collections.Generic;

namespace TasinirMal
{
    /// <summary>
    /// Bölge tanım bilgilerinin kayıt, silme ve listeleme işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class TanimBolge : istemciUzayi.GenelSayfaV3
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
                formAdi = Resources.TasinirMal.FRMTBO001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                if (Request.QueryString["menuYok"] != "1")
                {
                    BorderLayoutPanel.MarginSpec = "104 20 10 20";
                    BorderLayoutPanel.StyleSpec += "padding:10px";
                    BorderLayoutPanel.Border = true;
                    grdListe.Width = 200;
                }

                MuhasebeDoldur();

                Bolge sozluk = new Bolge();
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
            Bolge d = new Bolge();
            d.kod = OrtakFonksiyonlar.ConvertToInt(txtKod.Text, 0);
            d.ad = txtAd.Text;

            foreach (Ext.Net.ListItem item in lstMuhasebe.SelectedItems)
            {
                TNS.TMM.Muhasebe m = new TNS.TMM.Muhasebe();
                m.kod = item.Value;
                d.muhasebe.Add(m);
            }

            Sonuc sonuc = servisTMM.BolgeKaydet(kullanan, d);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusuV3("Hata", sonuc.hataStr);
            else
            {
                btnTemizle_Click(null, null);
                btnAra_Click(null, null);
                GenelIslemler.MesajKutusuV3("Bilgi", Resources.TasinirMal.FRMTMA005);
            }
        }

        /// <summary>
        /// Bul tuşuna basılınca çalışan olay metodu
        /// Sunucudan Bolge tanım bilgileri alınır ve listelenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Sil tuşuna basılınca çalışan olay metodu
        /// Seçili olan Bolge silinmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata veya bilgi mesajı görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme işlemi yapılır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            Bolge d = new Bolge();
            d.kod = OrtakFonksiyonlar.ConvertToInt(txtKod.Text, 0);

            if (d.kod == 0)
            {
                GenelIslemler.MesajKutusuV3("Uyarı", Resources.TasinirMal.FRMTMA007);
                return;
            }

            Sonuc sonuc = servisTMM.BolgeSil(kullanan, d);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusuV3("Hata", sonuc.hataStr);
            else
            {
                btnTemizle_Click(null, null);
                btnAra_Click(null, null);
                GenelIslemler.MesajKutusuV3("Bilgi", Resources.TasinirMal.FRMTMA006);
            }
        }

        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            txtKod.Value = "";
            txtAd.Text = "";
            lstMuhasebe.SelectedItems.Clear();
            lstMuhasebe.UpdateSelectedItems();
        }

        protected void SatirSecildi(object sender, DirectEventArgs e)
        {
            string secilisatir = e.ExtraParams["GRIDPARAM"].ToString();
            XmlNode xml = JSON.DeserializeXmlNode("{records:{record:" + secilisatir + "}}");
            foreach (XmlNode row in xml.SelectNodes("records/record"))
            {
                Bolge kriter = new Bolge();
                kriter.kod = OrtakFonksiyonlar.ConvertToInt(row.SelectSingleNode("KOD").InnerXml, 0);
                ObjectArray dler = servisTMM.BolgeListele(kullanan, kriter);
                lstMuhasebe.SelectedItems.Clear();
                foreach (Bolge dd in dler.objeler)
                {
                    txtKod.Text = dd.kod.ToString();
                    txtAd.Text = dd.ad;

                    foreach (Muhasebe muh in dd.muhasebe)
                    {
                        lstMuhasebe.SelectedItems.Add(new Ext.Net.ListItem(muh.ad, muh.kod));
                    }
                }
                lstMuhasebe.UpdateSelectedItems();
            }
        }

        /// <summary>
        /// Parametre olarak verilen Bolge nesnesi sunucuya gönderilir ve Bolge
        /// tanım bilgileri alınır. Hata varsa ekrana hata bilgisi yazılır, yoksa
        /// gelen Bolge tanım bilgileri gvBolgelar GridView kontrolüne doldurulur.
        /// </summary>
        /// <param name="kriter">Bolge kriter bilgilerini tutan nesne</param>
        public void Listele(Bolge kriter)
        {
            ObjectArray dler = servisTMM.BolgeListele(kullanan, kriter);

            if (!dler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusuV3("Uyarı", dler.sonuc.hataStr);
                return;
            }

            List<object> liste = new List<object>();
            foreach (Bolge item in dler.objeler)
            {
                liste.Add(new
                {
                    KOD = item.kod,
                    AD = item.ad,
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden Bolge kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Bolge kriter bilgileri döndürülür.</returns>
        public Bolge KriterTopla()
        {
            Bolge d = new Bolge();
            d.kod = OrtakFonksiyonlar.ConvertToInt(txtKod.Text, 0);
            d.ad = txtAd.Text.Trim();

            return d;
        }

        private void MuhasebeDoldur()
        {
            Muhasebe kriter = new Muhasebe();
            ObjectArray dler = servisTMM.MuhasebeListele(kullanan, kriter);

            foreach (Muhasebe item in dler.objeler)
            {
                lstMuhasebe.Items.Add(new Ext.Net.ListItem(item.kod + "-" + item.ad, item.kod));
            }
            lstMuhasebe.UpdateSelectedItems();
        }
    }
}