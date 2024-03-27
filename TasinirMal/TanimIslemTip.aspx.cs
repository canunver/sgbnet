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
    /// Ýþlem tipi taným bilgilerinin kayýt, silme ve listeleme iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TanimIslemTip : TMMSayfaV2
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
                formAdi = Resources.TasinirMal.FRMTIT001;
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

                TNS.TMM.IslemTip sozluk = new TNS.TMM.IslemTip();
                IslemTipDoldur();
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
            TNS.TMM.IslemTip d = new TNS.TMM.IslemTip();
            d.kod = OrtakFonksiyonlar.ConvertToInt(txtKod.Text, 0);
            d.ad = txtAd.Text;
            d.tur = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlIslemTuru), 0);

            Sonuc sonuc = servisTMM.IslemTipKaydet(kullanan, d);

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
        /// Sunucudan IslemTip taným bilgileri alýnýr ve listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Sil tuþuna basýlýnca çalýþan olay metodu
        /// Seçili olan IslemTip silinmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata veya bilgi mesajý görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme iþlemi yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            TNS.TMM.IslemTip d = new TNS.TMM.IslemTip();
            d.kod = OrtakFonksiyonlar.ConvertToInt(txtKod.Text, 0);

            if (d.kod == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTMA007);
                return;
            }

            Sonuc sonuc = servisTMM.IslemTipSil(kullanan, d);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                btnTemizle_Click(null, null);
                btnAra_Click(null, null);
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTIT003);
            }
        }

        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            txtKod.Clear();
            txtAd.Clear();
            ddlIslemTuru.Clear();
        }

        protected void SatirSecildi(object sender, DirectEventArgs e)
        {
            string secilisatir = e.ExtraParams["GRIDPARAM"].ToString();
            XmlNode xml = JSON.DeserializeXmlNode("{records:{record:" + secilisatir + "}}");
            foreach (XmlNode row in xml.SelectNodes("records/record"))
            {
                IslemTip kriter = new IslemTip();
                kriter.kod = OrtakFonksiyonlar.ConvertToInt(row.SelectSingleNode("KOD").InnerXml, 0);
                ObjectArray dler = servisTMM.IslemTipListele(kullanan, kriter);
                foreach (IslemTip dd in dler.objeler)
                {
                    txtKod.Text = dd.kod.ToString();
                    txtAd.Text = dd.ad;
                    ddlIslemTuru.SetValueAndFireSelect(dd.tur);
                }

                //if (hdnSecKapat.Text == "1")
                //{
                //    X.AddScript("try { parent.kepAdresiEkle('" + hdnSeciliAdres.Text + "'); } catch (e) { }");
                //    return;
                //}
            }
        }

        /// <summary>
        /// Parametre olarak verilen IslemTip nesnesi sunucuya gönderilir ve IslemTip
        /// taným bilgileri alýnýr. Hata varsa ekrana hata bilgisi yazýlýr, yoksa
        /// gelen IslemTip taným bilgileri gvIslemTiplar GridView kontrolüne doldurulur.
        /// </summary>
        /// <param name="kriter">IslemTip kriter bilgilerini tutan nesne</param>
        public void Listele(IslemTip kriter)
        {
            ObjectArray dler = servisTMM.IslemTipListele(kullanan, kriter);

            if (!dler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", dler.sonuc.hataStr);
                return;
            }

            List<object> liste = new List<object>();
            foreach (TNS.TMM.IslemTip item in dler.objeler)
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
        /// Sayfadaki ilgili kontrollerden IslemTip kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>IslemTip kriter bilgileri döndürülür.</returns>
        public IslemTip KriterTopla()
        {
            TNS.TMM.IslemTip d = new TNS.TMM.IslemTip();
            d.kod = OrtakFonksiyonlar.ConvertToInt(txtKod.Text, 0);
            d.ad = txtAd.Text.Trim();

            return d;
        }

        private void IslemTipDoldur()
        {
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT017, ((int)ENUMIslemTipi.ACILIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT005, ((int)ENUMIslemTipi.SATINALMAGIRIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT009, ((int)ENUMIslemTipi.DEVIRGIRIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT037, ((int)ENUMIslemTipi.DAGITIMIADEGIRIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT006, ((int)ENUMIslemTipi.BAGISGIRIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT007, ((int)ENUMIslemTipi.SAYIMFAZLASIGIRIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT008, ((int)ENUMIslemTipi.IADEGIRIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT013, ((int)ENUMIslemTipi.BAGISCIKIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT010, ((int)ENUMIslemTipi.URETILENGIRIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT019, ((int)ENUMIslemTipi.YILDEVIRGIRIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT023, ((int)ENUMIslemTipi.YILDEVIRCIKIS).ToString()));
            //ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT025, ((int)ENUMIslemTipi.ZFDUSME).ToString()));
            //ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT024, ((int)ENUMIslemTipi.ZFVERME).ToString()));
            //ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT027, ((int)ENUMIslemTipi.DTLDUSME).ToString()));
            //ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT026, ((int)ENUMIslemTipi.DTLVERME).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT021, ((int)ENUMIslemTipi.DEVIRCIKISKURUM).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT014, ((int)ENUMIslemTipi.SATISCIKIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT018, ((int)ENUMIslemTipi.DEGERARTTIR).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT022, ((int)ENUMIslemTipi.SAYIMNOKSANICIKIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT011, ((int)ENUMIslemTipi.TUKETIMCIKIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT015, ((int)ENUMIslemTipi.KULLANILMAZCIKIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT012, ((int)ENUMIslemTipi.DEVIRCIKIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT016, ((int)ENUMIslemTipi.HURDACIKIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT020, ((int)ENUMIslemTipi.DEVIRGIRISKURUM).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT036, ((int)ENUMIslemTipi.DAGITIMIADECIKIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT038, ((int)ENUMIslemTipi.URETIMCIKIS).ToString()));
            ddlIslemTuru.Items.Add(new ListItem(Resources.TasinirMal.FRMTIT039, ((int)ENUMIslemTipi.ENFLASYONARTISI).ToString()));
        }
    }                                                                  
}                                                                      
                                                                       
                                                                       
                                                                       
                                                                       