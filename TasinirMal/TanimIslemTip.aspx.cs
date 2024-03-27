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
    /// ��lem tipi tan�m bilgilerinin kay�t, silme ve listeleme i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class TanimIslemTip : TMMSayfaV2
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
                formAdi = Resources.TasinirMal.FRMTIT001;
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

                TNS.TMM.IslemTip sozluk = new TNS.TMM.IslemTip();
                IslemTipDoldur();
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
        /// Bul tu�una bas�l�nca �al��an olay metodu
        /// Sunucudan IslemTip tan�m bilgileri al�n�r ve listelenir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Sil tu�una bas�l�nca �al��an olay metodu
        /// Se�ili olan IslemTip silinmek �zere sunucuya g�nderilir,
        /// gelen sonuca g�re hata veya bilgi mesaj� g�r�nt�lenir.
        /// Son olarak g�ncel bilgilerin g�r�nmesi i�in listeleme i�lemi yap�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            TNS.TMM.IslemTip d = new TNS.TMM.IslemTip();
            d.kod = OrtakFonksiyonlar.ConvertToInt(txtKod.Text, 0);

            if (d.kod == 0)
            {
                GenelIslemler.MesajKutusu("Uyar�", Resources.TasinirMal.FRMTMA007);
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
        /// Parametre olarak verilen IslemTip nesnesi sunucuya g�nderilir ve IslemTip
        /// tan�m bilgileri al�n�r. Hata varsa ekrana hata bilgisi yaz�l�r, yoksa
        /// gelen IslemTip tan�m bilgileri gvIslemTiplar GridView kontrol�ne doldurulur.
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
        /// Sayfadaki ilgili kontrollerden IslemTip kriter bilgilerini toplayan ve d�nd�ren yordam
        /// </summary>
        /// <returns>IslemTip kriter bilgileri d�nd�r�l�r.</returns>
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
                                                                       
                                                                       
                                                                       
                                                                       