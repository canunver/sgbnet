using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.DirectoryServices; 
using System.Linq;
using System.Text;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Kiþiye verilmiþ zimmetli demirbaþ bilgilerinin listeme ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class ZimmetKisiListeleEski : System.Web.UI.Page
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Sayfa adresinde gelen TCKimlik girdi dizgisi dolu ise zimmet listeleme yapýlýr, yoksa sayfa boþ açýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["TCKimlik"]))
                {
                    txtTCKimlik.Text = Request.QueryString["TCKimlik"];
                    ZimmetKisiGrideYaz(KriterTopla());
                    btnSorgula.Visible = false;
                    btnRapor.Visible = false;
                    txtTCKimlik.Visible = false;
                    lblBilgi.Text = "";
                }
            }
            TNS.DEG.IDEGServis degServis = TNS.DEG.Arac.Tanimla();

            if (TasinirGenel.tasinirZimmeteOnay)
            {
                LDAPdogrulama.Visible = true;
                txtTCKimlik.Enabled = false;
                lblBilgi.Visible = false;
                //txtLDAPUser.Text = degServis.DegiskenDegerBul(0, "LDAPKullanici");
                //txtLDAPPassword.Text = degServis.DegiskenDegerBul(0, "LDAPSifre");
            }
            else
            {
                LDAPdogrulama.Visible = false;
                txtTCKimlik.Enabled = true;
                lblBilgi.Visible = true;
            }

            if (degServis.DegiskenDegerBul(0, "LDAPZorunlu") == "1")
            {
                txtTCKimlik.Enabled = false;
            }
            else
            {
                txtTCKimlik.Enabled = true;
            }
        }

        /// <summary>
        /// LDAP tuþuna basýlýnca çalýþan olay metodu
        /// LDAP sunucudan kiþi bilgilerinin doðrulanmasýný yapar
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnLDAP_Click(object sender, EventArgs e)
        {
            TNS.DEG.IDEGServis degServis = TNS.DEG.Arac.Tanimla();
            string bilgi = string.Empty;
            string getLDAPAttributeName = string.Empty;
            string getLDAPAttributeValue = string.Empty;
            string ldapAdres = degServis.DegiskenDegerBul(0, "LDAP");
            string ldapDebug = degServis.DegiskenDegerBul(0, "LDAPDebug");
            string ldapFormat = degServis.DegiskenDegerBul(0, "LDAPFormat");
            string ldapAttributeName = degServis.DegiskenDegerBul(0, "LDAPAttribute");

            if (string.IsNullOrWhiteSpace(ldapAdres))
            {
                GenelIslemler.HataYaz(this, "LDAP sunucusu bulunamadý !");
                return;
            }

            //int sicilno = OrtakFonksiyonlar.ConvertToInt(txtTCKimlik.Text.Replace(".",""),0);
            //if (sicilno > 0)
            //{
            //    GenelIslemler.BilgiYaz(this, String.Format(ldapFormat, sicilno));
            //}
            //return;



            try
            {
                System.DirectoryServices.DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry(ldapAdres, txtLDAPUser.Text, txtLDAPPassword.Text);
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(&(objectClass=user)(sAMAccountName=" + txtLDAPUser.Text + "))";
                search.CacheResults = false;

                SearchResultCollection allResults = search.FindAll();
                StringBuilder sb = new StringBuilder();
                string sicilNo = string.Empty;

                foreach (SearchResult searchResult in allResults)
                {
                    foreach (string propName in searchResult.Properties.PropertyNames)
                    {
                        ResultPropertyValueCollection valueCollection = searchResult.Properties[propName];
                        foreach (Object propertyValue in valueCollection)
                        {
                            sb.AppendLine(string.Format("property:{0}, value{1}", propName, propertyValue));
                            if (propName == ldapAttributeName)
                            {
                                txtTCKimlik.Text = propertyValue.ToString();
                                lblLDAPDurum.Text = Resources.TasinirMal.FRMZMO006;
                            }
                        }
                    }
                }
                int sicilno = OrtakFonksiyonlar.ConvertToInt(txtTCKimlik.Text.Replace(".", ""), 0);
                if (sicilno > 0)
                {
                    txtTCKimlik.Text = String.Format(ldapFormat, sicilno);
                }
            }
            catch (Exception ex)
            {
                GenelIslemler.HataYaz(this, ex.ToString() + ".....HATALI");
            }
        }

        /// <summary>
        /// Sorgula tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çaðýrýlýr ve toplanan kriterler kiþiye
        /// verilmiþ zimmetli demirbaþlarý listeleyen ZimmetKisiGrideYaz yordamýna gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSorgula_Click(object sender, EventArgs e)
        {
            TNS.DEG.IDEGServis degServis = TNS.DEG.Arac.Tanimla();
            int sicilno = OrtakFonksiyonlar.ConvertToInt(txtTCKimlik.Text.Replace(".", ""), 0);
            string ldapFormat = degServis.DegiskenDegerBul(0, "LDAPFormat");
            if (sicilno > 0)
            {
                txtTCKimlik.Text = String.Format(ldapFormat, sicilno);
            }
            ZimmetKisiGrideYaz(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki kontrollerden zimmet listeleme kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Zimmet listeleme kriter bilgileri döndürülür.</returns>
        private TNS.TMM.ZimmetOrtakAlanVeKisi KriterTopla()
        {
            TNS.TMM.ZimmetOrtakAlanVeKisi kriter = new TNS.TMM.ZimmetOrtakAlanVeKisi();
            kriter.tcKimlikNo = txtTCKimlik.Text.Trim();
            return kriter;
        }

        /// <summary>
        /// Parametre olarak verilen zimmet listeleme kriterlerini sunucudaki ZimmetKisi yordamýna
        /// gönderir, sunucudan gelen bilgi kümesini sayfadaki gvZimmet GridView kontrolüne doldurur.
        /// </summary>
        /// <param name="kriter">Zimmet kriter bilgilerini tutan nesne</param>
        private void ZimmetKisiGrideYaz(TNS.TMM.ZimmetOrtakAlanVeKisi kriter)
        {
            ObjectArray bilgi = servisTMM.ZimmetKisi(new Kullanici(), kriter, false);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                gvZimmet.Visible = false;
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                gvZimmet.Visible = false;
                return;
            }

            DataTable tablo = new DataTable();
            tablo.Columns.Add(Resources.TasinirMal.FRMZKL001);
            tablo.Columns.Add(Resources.TasinirMal.FRMZKL002);
            if (TasinirGenel.rfIdVarMi)
                tablo.Columns.Add(Resources.TasinirMal.FRMZKL012);
            else
                tablo.Columns.Add(Resources.TasinirMal.FRMZKL003);
            tablo.Columns.Add(Resources.TasinirMal.FRMZKL004);

            if (TasinirGenel.rfIdVarMi)
                tablo.Columns.Add(Resources.TasinirMal.FRMZKL005);

            tablo.Columns.Add(Resources.TasinirMal.FRMZKL006);
            tablo.Columns.Add(Resources.TasinirMal.FRMZKL007);

            TNS.TMM.ZimmetOrtakAlanVeKisi zoa = (TNS.TMM.ZimmetOrtakAlanVeKisi)bilgi.objeler[0];
            for (int i = 0; i < zoa.detay.Count; i++)
            {
                TNS.TMM.ZimmetOrtakAlanVeKisiDetay detay = (TNS.TMM.ZimmetOrtakAlanVeKisiDetay)zoa.detay[i];
                if (TasinirGenel.rfIdVarMi)
                    tablo.Rows.Add(detay.muhasebeKod + " - " + detay.muhasebeAd, detay.harcamaKod + " - " + detay.harcamaAd, detay.ambarKod + " - " + detay.ambarAd, detay.gorSicilNo, detay.rfIdNo, detay.sicilAd, detay.fisTarih);
                else
                    tablo.Rows.Add(detay.muhasebeKod + " - " + detay.muhasebeAd, detay.harcamaKod + " - " + detay.harcamaAd, detay.ambarKod + " - " + detay.ambarAd, detay.gorSicilNo, detay.sicilAd, detay.fisTarih);
            }

            gvZimmet.DataSource = tablo;
            gvZimmet.DataBind();
            gvZimmet.Visible = true;
        }

        /// <summary>
        /// Taþýma Formu Dök tuþuna basýlýnca çalýþan olay metodu
        /// Sayfadaki gvZimmet GridView kontrolündeki zimmet bilgilerini
        /// taþýma formu excel raporuna yazar ve kullanýcýya gönderir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnRapor_Click(object sender, EventArgs e)
        {
            Tablo XLS = GenelIslemler.NewTablo();

            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "ZIFTASIMA.XLT";

            XLS.DosyaAc(new istemciUzayi.GenelSayfa().raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            for (int i = 0; i < gvZimmet.Rows.Count; i++)
            {
                GridViewRow row = gvZimmet.Rows[i];
                if (!((CheckBox)row.FindControl("chkSecim")).Checked)
                    continue;

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);

                XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 2);
                XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 5);
                XLS.HucreBirlestir(satir, sutun + 6, satir, sutun + 7);
                
                XLS.HucreDegerYaz(satir, sutun, satir - kaynakSatir);
                XLS.HucreDegerYaz(satir, sutun + 1, Server.HtmlDecode(row.Cells[4].Text) + " (" + Server.HtmlDecode(row.Cells[5].Text) + ")");
                XLS.HucreDegerYaz(satir, sutun + 3, Server.HtmlDecode(row.Cells[6].Text));
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
    }
}