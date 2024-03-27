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
    /// Firma tan�m bilgilerinin kay�t, silme ve listeleme i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class TanimFirma : TMMSayfaV2
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
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMTFR001;
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

                FirmaBilgisi sozluk = new FirmaBilgisi();
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

            if (txtAd.Text.Trim() == "")
            {
                if (hata != "") hata += "<br>";
                hata += Resources.TasinirMal.FRMTFR003;
            }

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyar�", hata);
                return;
            }

            ObjectArray firmalar = new ObjectArray();
            FirmaBilgisi firma = new FirmaBilgisi();

            firma.kod = OrtakFonksiyonlar.ConvertToInt(hdnKod.Value, 0);
            firma.ad = txtAd.Text.Trim();
            firma.vno = txtVD.Text.Trim();
            firma.vd = txtVNo.Text.Trim();
            firma.banka = txtBanka.Text.Trim();
            firma.hesapNo = txtHesapNo.Text.Trim();
            firma.tel = txtTel.Text.Trim();
            firma.fax = txtFax.Text.Trim();
            firma.kullaniciKodu = kullanan.kullaniciKodu;

            firmalar.objeler.Add(firma);

            Sonuc sonuc = servisTMM.FirmaKaydet(firmalar);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                btnTemizle_Click(null, null);
                btnAra_Click(null, null);
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTFR004);
            }
        }

        /// <summary>
        /// Bul tu�una bas�l�nca �al��an olay metodu
        /// Sunucudan firmabilgisi tan�m bilgileri al�n�r ve listelenir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Sil tu�una bas�l�nca �al��an olay metodu
        /// Se�ili olan firmabilgisi silinmek �zere sunucuya g�nderilir,
        /// gelen sonuca g�re hata veya bilgi mesaj� g�r�nt�lenir.
        /// Son olarak g�ncel bilgilerin g�r�nmesi i�in listeleme i�lemi yap�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            ObjectArray firmalar = new ObjectArray();
            FirmaBilgisi firma = new FirmaBilgisi();

            firma.kod = OrtakFonksiyonlar.ConvertToInt(hdnKod.Value, 0);
            firma.kullaniciKodu = kullanan.kullaniciKodu;
            firmalar.objeler.Add(firma);

            if (firma.kod == 0)
            {
                GenelIslemler.MesajKutusu("Uyar�", Resources.TasinirMal.FRMTFR006);
                return;
            }

            Sonuc sonuc = servisTMM.FirmaSil(firmalar);

            btnAra_Click(null, null);
            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                btnTemizle_Click(null, null);
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTFR005);
            }
        }

        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            hdnKod.Value = "";
            txtAd.Text = "";
            txtTel.Text = "";
            txtFax.Text = "";
            txtVNo.Text = "";
            txtVD.Text = "";
            txtBanka.Text = "";
            txtHesapNo.Text = "";
        }

        protected void SatirSecildi(object sender, DirectEventArgs e)
        {
            string secilisatir = e.ExtraParams["GRIDPARAM"].ToString();
            XmlNode xml = JSON.DeserializeXmlNode("{records:{record:" + secilisatir + "}}");
            foreach (XmlNode row in xml.SelectNodes("records/record"))
            {
                FirmaBilgisi kriter = new FirmaBilgisi();
                kriter.kod = OrtakFonksiyonlar.ConvertToInt(row.SelectSingleNode("KOD").InnerXml, 0);
                ObjectArray dler = servisTMM.FirmaListele(kriter);
                foreach (FirmaBilgisi dd in dler.objeler)
                {
                    hdnKod.Value = dd.kod;
                    txtAd.Text = dd.ad;
                    txtTel.Text = dd.tel;
                    txtFax.Text = dd.fax;
                    txtVNo.Text = dd.vno;
                    txtVD.Text = dd.vd;
                    txtBanka.Text = dd.banka;
                    txtHesapNo.Text = dd.hesapNo;
                }

                //if (hdnSecKapat.Text == "1")
                //{
                //    X.AddScript("try { parent.kepAdresiEkle('" + hdnSeciliAdres.Text + "'); } catch (e) { }");
                //    return;
                //}
            }
        }

        /// <summary>
        /// Parametre olarak verilen firmabilgisi nesnesi sunucuya g�nderilir ve firmabilgisi
        /// tan�m bilgileri al�n�r. Hata varsa ekrana hata bilgisi yaz�l�r, yoksa
        /// gelen firmabilgisi tan�m bilgileri gvfirmabilgisilar GridView kontrol�ne doldurulur.
        /// </summary>
        /// <param name="kriter">firmabilgisi kriter bilgilerini tutan nesne</param>
        public void Listele(FirmaBilgisi kriter)
        {
            ObjectArray dler = servisTMM.FirmaListele(kriter);

            if (!dler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyar�", dler.sonuc.hataStr);
                return;
            }

            List<object> liste = new List<object>();
            foreach (FirmaBilgisi item in dler.objeler)
            {
                liste.Add(new
                {
                    KOD = item.kod,
                    KULLANICIKODU = item.kullaniciKodu,
                    ADI = item.ad,
                    VNO = item.vno,
                    VD = item.vd,
                    BANKA = item.banka,
                    HESAPNO = item.hesapNo,
                    TEL = item.tel,
                    FAX = item.fax
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden firmabilgisi kriter bilgilerini toplayan ve d�nd�ren yordam
        /// </summary>
        /// <returns>firmabilgisi kriter bilgileri d�nd�r�l�r.</returns>
        public FirmaBilgisi KriterTopla()
        {
            FirmaBilgisi d = new FirmaBilgisi();
            d.kod = OrtakFonksiyonlar.ConvertToInt(hdnKod.Value, 0);
            d.ad = txtAd.Text.Trim();
            d.tel = txtTel.Text.Trim();
            d.fax = txtFax.Text.Trim();
            d.vno = txtVNo.Text.Trim();
            d.vd = txtVD.Text.Trim();
            d.banka = txtBanka.Text.Trim();
            d.hesapNo = txtHesapNo.Text.Trim();

            return d;
        }
    }
}