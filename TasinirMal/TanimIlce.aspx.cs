using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections.Generic;
using Ext1.Net;
using System.Xml;

namespace TasinirMal
{
    /// <summary>
    /// �l�e tan�m bilgilerinin kay�t, silme ve listeleme i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class TanimIlce : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        ///     Sayfa ilk defa �a��r�l�yorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlan�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMTIC001;
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

                IlDoldur();

                Ilce sozluk = new Ilce();
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

            if (TasinirGenel.ComboDegerDondur(ddlIl) == "")
                hata += Resources.TasinirMal.FRMTIC003;

            if (txtKod.Text.Trim() == "")
            {
                if (hata != "") hata += "<br>";
                hata += Resources.TasinirMal.FRMTIC004;
            }

            if (txtAd.Text.Trim() == "")
            {
                if (hata != "") hata += "<br>";
                hata += Resources.TasinirMal.FRMTIC005;
            }

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyar�", hata);
                return;
            }

            Ilce d = new Ilce();
            d.ilKodu = TasinirGenel.ComboDegerDondur(ddlIl);
            d.kod = txtKod.Text.Trim();
            d.ad = txtAd.Text;

            Sonuc sonuc = servisTMM.IlceKaydet(kullanan, d);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                btnTemizle_Click(null, null);
                btnAra_Click(null, null);
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTIC006);
            }
        }

        /// <summary>
        /// Bul tu�una bas�l�nca �al��an olay metodu
        /// Sunucudan il�e tan�m bilgileri al�n�r ve listelenir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Sil tu�una bas�l�nca �al��an olay metodu
        /// Se�ili olan il�e silinmek �zere sunucuya g�nderilir,
        /// gelen sonuca g�re hata veya bilgi mesaj� g�r�nt�lenir.
        /// Son olarak g�ncel bilgilerin g�r�nmesi i�in listeleme i�lemi yap�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            Ilce d = new Ilce();
            d.ilKodu = TasinirGenel.ComboDegerDondur(ddlIl);
            d.kod = txtKod.Text.Trim();

            if (string.IsNullOrEmpty(d.kod))
            {
                GenelIslemler.MesajKutusu("Uyar�", Resources.TasinirMal.FRMTIL008);
                return;
            }

            Sonuc sonuc = servisTMM.IlceSil(kullanan, d);

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
            txtKod.Text = "";
            txtAd.Text = "";
            ddlIl.Value = "";
        }

        protected void SatirSecildi(object sender, DirectEventArgs e)
        {
            string secilisatir = e.ExtraParams["GRIDPARAM"].ToString();
            XmlNode xml = JSON.DeserializeXmlNode("{records:{record:" + secilisatir + "}}");
            foreach (XmlNode row in xml.SelectNodes("records/record"))
            {
                Ilce kriter = new Ilce();
                kriter.kod = row.SelectSingleNode("KOD").InnerXml;
                kriter.ilKodu = row.SelectSingleNode("ILKOD").InnerXml;
                ObjectArray dler = servisTMM.IlceListele(kullanan, kriter);
                foreach (Ilce dd in dler.objeler)
                {
                    ddlIl.SetValueAndFireSelect(dd.ilKodu);
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
        /// Parametre olarak verilen il�e nesnesi sunucuya g�nderilir ve il�e
        /// tan�m bilgileri al�n�r. Hata varsa ekrana hata bilgisi yaz�l�r, yoksa
        /// gelen il�e tan�m bilgileri gvIlceler GridView kontrol�ne doldurulur.
        /// </summary>
        /// <param name="kriter">�l�e kriter bilgilerini tutan nesne</param>
        public void Listele(Ilce kriter)
        {
            ObjectArray dler = servisTMM.IlceListele(kullanan, kriter);

            if (!dler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", dler.sonuc.hataStr);
                return;
            }

            List<object> liste = new List<object>();
            foreach (Ilce item in dler.objeler)
            {
                liste.Add(new
                {
                    ILKOD = item.ilKodu,
                    ILAD = item.ilAd,
                    KOD = item.kod,
                    ADI = item.ad,
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden il�e kriter bilgilerini toplayan ve d�nd�ren yordam
        /// </summary>
        /// <returns>�l�e kriter bilgileri d�nd�r�l�r.</returns>
        public Ilce KriterTopla()
        {
            Ilce d = new Ilce();
            d.kod = txtKod.Text.Trim();
            d.ad = txtAd.Text.Trim();
            d.ilKodu = TasinirGenel.ComboDegerDondur(ddlIl);

            return d;
        }

        /// <summary>
        /// �l bilgileri sunucudan �ekilir ve ddlIl DropDownList kontrol�ne doldurulur.
        /// </summary>
        private void IlDoldur()
        {
            ObjectArray dler = new ObjectArray();
            dler = servisTMM.IlListele(kullanan, new Il());

            if (dler.sonuc.islemSonuc)
            {
                List<object> liste = new List<object>();
                foreach (Il d in dler.objeler)
                {
                    liste.Add(new
                    {
                        KOD = d.kod,
                        ADI = d.ad
                    });
                }
                strIl.DataSource = liste;
                strIl.DataBind();
            }
        }

    }
}