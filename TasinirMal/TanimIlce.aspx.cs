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
    /// Ýlçe taným bilgilerinin kayýt, silme ve listeleme iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TanimIlce : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa ilk defa çaðýrýlýyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMTIC001;
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

                IlDoldur();

                Ilce sozluk = new Ilce();
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
                GenelIslemler.MesajKutusu("Uyarý", hata);
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
        /// Bul tuþuna basýlýnca çalýþan olay metodu
        /// Sunucudan ilçe taným bilgileri alýnýr ve listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Sil tuþuna basýlýnca çalýþan olay metodu
        /// Seçili olan ilçe silinmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata veya bilgi mesajý görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme iþlemi yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            Ilce d = new Ilce();
            d.ilKodu = TasinirGenel.ComboDegerDondur(ddlIl);
            d.kod = txtKod.Text.Trim();

            if (string.IsNullOrEmpty(d.kod))
            {
                GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTIL008);
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
        /// Parametre olarak verilen ilçe nesnesi sunucuya gönderilir ve ilçe
        /// taným bilgileri alýnýr. Hata varsa ekrana hata bilgisi yazýlýr, yoksa
        /// gelen ilçe taným bilgileri gvIlceler GridView kontrolüne doldurulur.
        /// </summary>
        /// <param name="kriter">Ýlçe kriter bilgilerini tutan nesne</param>
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
        /// Sayfadaki ilgili kontrollerden ilçe kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Ýlçe kriter bilgileri döndürülür.</returns>
        public Ilce KriterTopla()
        {
            Ilce d = new Ilce();
            d.kod = txtKod.Text.Trim();
            d.ad = txtAd.Text.Trim();
            d.ilKodu = TasinirGenel.ComboDegerDondur(ddlIl);

            return d;
        }

        /// <summary>
        /// Ýl bilgileri sunucudan çekilir ve ddlIl DropDownList kontrolüne doldurulur.
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