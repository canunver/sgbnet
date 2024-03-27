using System;
using System.Data;
using System.Drawing;
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
    public partial class ZimmetePostaOnayKisiListele : TMMSayfa
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
            //if (!IsPostBack)
            //{
            //    if (!string.IsNullOrEmpty(Request.QueryString["TCKimlik"]))
            //    {
            //        txtTCKimlik.Text = Request.QueryString["TCKimlik"];
            //        ZimmetKisiEOnayGrideYaz(KriterTopla());
            //        btnSorgula.Visible = false;
            //        btnZFRapor.Visible = false;
            //        txtTCKimlik.Visible = false;
            //        lblBilgi.Text = "";

            //    }
            //}

            //if (TasinirGenel.tasinirZimmeteOnay) 
            //{
            //    LDAPdogrulama.Visible = true;
            //    txtTCKimlik.Enabled = false;
            //    lblBilgi.Visible = false;
            //    //txtLDAPUser.Text = TNS.TMM.Arac.DegiskenDegerBul(0, "LDAPKullanici");
            //    //txtLDAPPassword.Text = TNS.TMM.Arac.DegiskenDegerBul(0, "LDAPSifre");
            //}
            //else
            //{
            //    LDAPdogrulama.Visible = false;
            //    txtTCKimlik.Enabled = true;
            //    lblBilgi.Visible = true;
            //}

            //if (TNS.TMM.Arac.DegiskenDegerBul(0, "LDAPZorunlu") == "1")
            //{
            //    txtTCKimlik.Enabled = false;
            //}
            //else
            //{
            //    txtTCKimlik.Enabled = true;
            //}
        }

        /// <summary>
        /// LDAP tuþuna basýlýnca çalýþan olay metodu
        /// LDAP sunucudan kiþi bilgilerinin doðrulanmasýný yapar
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        //protected void btnLDAP1_Click(object sender, EventArgs e)
        //{
        //    string bilgi = string.Empty;
        //    string getLDAPAttributeName = string.Empty;
        //    string getLDAPAttributeValue = string.Empty;
        //    string ldapAdres = TNS.TMM.Arac.DegiskenDegerBul(0, "LDAP");
        //    string ldapDebug = TNS.TMM.Arac.DegiskenDegerBul(0, "LDAPDebug");
        //    string ldapFormat = TNS.TMM.Arac.DegiskenDegerBul(0, "LDAPFormat");
        //    if (string.IsNullOrWhiteSpace(ldapAdres))
        //    {
        //        GenelIslemler.MesajKutusu("Uyarý", "LDAP sunucusu bulunamadý !");
        //        return;
        //    }

        //    //int sicilno = OrtakFonksiyonlar.ConvertToInt(txtTCKimlik.Text.Replace(".",""),0);
        //    //if (sicilno > 0)
        //    //{
        //    //    GenelIslemler.MesajKutusu("Bilgi", String.Format(ldapFormat, sicilno));
        //    //}
        //    //return;

        //    lblLDAPDurum.Text = Resources.TasinirMal.FRMZMO006;



        //    System.DirectoryServices.DirectoryEntry de = new System.DirectoryServices.DirectoryEntry(ldapAdres, txtLDAPUser.Text, txtLDAPPassword.Text);
        //    System.DirectoryServices.DirectorySearcher dsearch = new System.DirectoryServices.DirectorySearcher(de);
        //    try
        //    {
        //        bilgi += "LDAP sunucusuna baðlandý." + "<br />";
        //        getLDAPAttributeName = TNS.TMM.Arac.DegiskenDegerBul(0, "LDAPAttribute");
        //        if (!string.IsNullOrEmpty(getLDAPAttributeName))
        //        {
        //            bilgi += "LDAP deðiþkeni adý alýndý : " + getLDAPAttributeName + "<br />";
        //        }

        //        dsearch.Filter = string.Format("(objectClass={0})", '*'); 

        //        dsearch.PropertiesToLoad.Add(getLDAPAttributeName);
        //        bilgi += "LDAP deðiþken PropertiesToLoad SET edildi." + "<br />";

        //        dsearch.Filter = "(cn=" + txtLDAPUser.Text + ")";

        //        System.DirectoryServices.SearchResult sr = dsearch.FindOne();
        //        if (sr != null)
        //        {
        //            //bilgi += "LDAP sunucusu FindOne çalýþtý." + "<br />";

        //            //getLDAPAttributeValue = sr.Properties[getLDAPAttributeName][0].ToString();

        //            //if (!string.IsNullOrEmpty(getLDAPAttributeValue))
        //            //{
        //            //    txtTCKimlik.Text = getLDAPAttributeValue;
        //            //    int sicilno = OrtakFonksiyonlar.ConvertToInt(txtTCKimlik.Text.Replace(".", ""), 0);
        //            //    if (sicilno > 0)
        //            //    {
        //            //        txtTCKimlik.Text = String.Format(ldapFormat, sicilno);
        //            //    }
        //            //    bilgi += "LDAP deðiþken DEÐERÝ alýndý." + "<br />";
        //            //}
        //            //else
        //            //{
        //            //    bilgi += "LDAP deðiþken DEÐERÝ ALINAMADI." + "<br />";
        //            //}

        //            de = sr.GetDirectoryEntry();
        //            GenelIslemler.MesajKutusu("Bilgi", de.Properties["getLDAPAttributeName"][0].ToString());
        //            return;

        //            ResultPropertyCollection fields = sr.Properties;

        //            foreach (String ldapField in fields.PropertyNames)
        //            {
        //                // cycle through objects in each field e.g. group membership  
        //                // (for many fields there will only be one object such as name)  

        //                foreach (Object myCollection in fields[ldapField])
        //                    bilgi += (String.Format("{0,-20} : {1}", ldapField, myCollection.ToString())) + "<br />";
        //            }
        //            GenelIslemler.MesajKutusu("Bilgi", bilgi + "OK");
        //        }
        //        if (ldapDebug == "1")
        //            GenelIslemler.MesajKutusu("Uyarý", bilgi + "HATALI");

        //    }
        //    catch (Exception ex)
        //    {
        //        GenelIslemler.MesajKutusu("Uyarý", bilgi +  ex.ToString() +  ".....HATALI");
        //    }
        //}

        //protected void btnLDAP2_Click(object sender, EventArgs e)
        //{
        //    string bilgi = string.Empty;
        //    string getLDAPAttributeName = string.Empty;
        //    string getLDAPAttributeValue = string.Empty;
        //    string ldapAdres = TNS.TMM.Arac.DegiskenDegerBul(0, "LDAP");
        //    string ldapDebug = TNS.TMM.Arac.DegiskenDegerBul(0, "LDAPDebug");
        //    string ldapFormat = TNS.TMM.Arac.DegiskenDegerBul(0, "LDAPFormat");
        //    if (string.IsNullOrWhiteSpace(ldapAdres))
        //    {
        //        GenelIslemler.MesajKutusu("Uyarý", "LDAP sunucusu bulunamadý !");
        //        return;
        //    }

        //    //int sicilno = OrtakFonksiyonlar.ConvertToInt(txtTCKimlik.Text.Replace(".",""),0);
        //    //if (sicilno > 0)
        //    //{
        //    //    GenelIslemler.MesajKutusu("Bilgi", String.Format(ldapFormat, sicilno));
        //    //}
        //    //return;

        //    lblLDAPDurum.Text = Resources.TasinirMal.FRMZMO006;



        //    System.DirectoryServices.DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry(ldapAdres, txtLDAPUser.Text, txtLDAPPassword.Text);
        //    System.DirectoryServices.DirectorySearcher dsearch = new System.DirectoryServices.DirectorySearcher(entry);

        //    // Use the FindOne method to find the object, which in this case, is the user
        //    // indicated by User Name, and assign it to a SearchResult.
        //    SearchResult searchResult = dsearch.FindOne();

        //    // Create a ResultPropertyValueCollection object to get the values for the 
        //    // memberOf attribute for this user.
        //    //string propertyName = "employeeID";
        //    ResultPropertyValueCollection valueCollection = searchResult.Properties[txtKontrol.Text];

        //    try
        //    {
        //        // Write the value contained in index position 5 in the memberOf attribute.
        //        GenelIslemler.MesajKutusu("Bilgi", valueCollection[0].ToString());
        //    }
        //    catch (ArgumentOutOfRangeException)
        //    {
        //        // The property contains no value in position 5.
        //        GenelIslemler.MesajKutusu("Uyarý", "The " + txtKontrol.Text + " property contains no value at the specified index.");
        //    }
        //}

        protected void btnLDAP_Click(object sender, EventArgs e)
        {
            //string bilgi = string.Empty;
            //string getLDAPAttributeName = string.Empty;
            //string getLDAPAttributeValue = string.Empty;
            //string ldapAdres = TNS.TMM.Arac.DegiskenDegerBul(0, "LDAP");
            //string ldapDebug = TNS.TMM.Arac.DegiskenDegerBul(0, "LDAPDebug");
            //string ldapFormat = TNS.TMM.Arac.DegiskenDegerBul(0, "LDAPFormat");
            //string ldapAttributeName = TNS.TMM.Arac.DegiskenDegerBul(0, "LDAPAttribute");

            //if (string.IsNullOrWhiteSpace(ldapAdres))
            //{
            //    GenelIslemler.MesajKutusu("Uyarý", "LDAP sunucusu bulunamadý !");
            //    return;
            //}

            //int sicilno = OrtakFonksiyonlar.ConvertToInt(txtTCKimlik.Text.Replace(".",""),0);
            //if (sicilno > 0)
            //{
            //    GenelIslemler.MesajKutusu("Bilgi", String.Format(ldapFormat, sicilno));
            //}
            //return;

            //lblLDAPDurum.Text = Resources.TasinirMal.FRMZMO006;


            //try
            //{
            //    System.DirectoryServices.DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry(ldapAdres, txtLDAPUser.Text, txtLDAPPassword.Text);
            //    DirectorySearcher search = new DirectorySearcher(entry);
            //    search.Filter = "(&(objectClass=user)(sAMAccountName=" + txtLDAPUser.Text + "))";
            //    search.CacheResults = false;

            //    SearchResultCollection allResults = search.FindAll();
            //    StringBuilder sb = new StringBuilder();
            //    string sicilNo = string.Empty;

            //    foreach (SearchResult searchResult in allResults)
            //    {
            //        foreach (string propName in searchResult.Properties.PropertyNames)
            //        {
            //            ResultPropertyValueCollection valueCollection = searchResult.Properties[propName];
            //            foreach (Object propertyValue in valueCollection)
            //            {
            //                sb.AppendLine(string.Format("property:{0}, value{1}", propName, propertyValue));
            //                if (propName == ldapAttributeName)
            //                {
            //                    txtTCKimlik.Text = propertyValue.ToString();
            //                    lblLDAPDurum.Text = Resources.TasinirMal.FRMZMO006;
            //                }
            //            }
            //        }
            //    }
            //    int sicilno = OrtakFonksiyonlar.ConvertToInt(txtTCKimlik.Text.Replace(".", ""), 0);
            //    if (sicilno > 0)
            //    {
            //        txtTCKimlik.Text = String.Format(ldapFormat, sicilno);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    GenelIslemler.MesajKutusu("Uyarý", ex.ToString() + ".....HATALI");
            //}
            //GenelIslemler.MesajKutusu("Bilgi",sb.ToString());
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
            int sicilno = OrtakFonksiyonlar.ConvertToInt(txtTCKimlik.Text.Replace(".", ""), 0);
            string ldapFormat = TNS.TMM.Arac.DegiskenDegerBul(0, "LDAPFormat");
            if (sicilno > 0)
            {
                txtTCKimlik.Text = String.Format(ldapFormat, sicilno);
            }
            ZimmetKisiEOnayGrideYaz(KriterTopla());

        }

        /// <summary>
        /// Sayfadaki kontrollerden zimmet eonay listeleme kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>ZimmetFormEOnay listeleme kriter bilgileri döndürülür.</returns>
        private TNS.TMM.ZimmetFormEOnayKriter KriterTopla()
        {
            TNS.TMM.ZimmetFormEOnayKriter kriter = new TNS.TMM.ZimmetFormEOnayKriter();
            //kriter.emailKime = txtTCKimlik.Text.Trim();
            //kriter.durum = 0;
            //kriter.eonayGondermeTarihBasla = new TNSDateTime();
            //kriter.eonayGondermeTarihBitis = new TNSDateTime();
            //kriter.eonayCevapTarihBasla = new TNSDateTime();
            //kriter.eonayCevapTarihBitis = new TNSDateTime();
            return kriter;
        }

        /// <summary>
        /// Parametre olarak verilen zimmet listeleme kriterlerini sunucudaki ZimmetKisi yordamýna
        /// gönderir, sunucudan gelen bilgi kümesini sayfadaki gvZimmet GridView kontrolüne doldurur.
        /// </summary>
        /// <param name="kriter">Zimmet kriter bilgilerini tutan nesne</param>
        private void ZimmetKisiEOnayGrideYaz(TNS.TMM.ZimmetFormEOnayKriter kriter)
        {

            //ObjectArray bilgi = servisTMM.ZimmetFisiListeleKisiOrtakEOnayBilgisiyle(kriter);

            //if (!bilgi.sonuc.islemSonuc)
            //{
            //    GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
            //    gvZimmet.Visible = false;
            //    return;
            //}

            //if (bilgi.objeler.Count <= 0)
            //{
            //    GenelIslemler.MesajKutusu("Bilgi", TasinirMalResI.TMM029);
            //    gvZimmet.Visible = false;
            //    return;
            //}

            //DataTable tablo = new DataTable();
            //tablo.Columns.Add(Resources.TasinirMal.FRMZMO019);
            //tablo.Columns.Add(Resources.TasinirMal.FRMZMO010);
            //tablo.Columns.Add(Resources.TasinirMal.FRMZMO011);
            ////tablo.Columns.Add(Resources.TasinirMal.FRMZMO012);
            //tablo.Columns.Add(Resources.TasinirMal.FRMZMO013);
            //tablo.Columns.Add(Resources.TasinirMal.FRMZMO014);
            //tablo.Columns.Add(Resources.TasinirMal.FRMZMO015);
            //tablo.Columns.Add(Resources.TasinirMal.FRMZMO016);
            //tablo.Columns.Add(Resources.TasinirMal.FRMZMO017);
            //tablo.Columns.Add(Resources.TasinirMal.FRMZMO018);

            //string durumAciklama = "";

            //for (int i = 0; i < bilgi.objeler.Count; i++)
            //{
            //    TNS.TMM.ZimmetFormEOnay zfBelge = (TNS.TMM.ZimmetFormEOnay)bilgi.objeler[i];
            //    durumAciklama = "";
            //    if (zfBelge.durum == (int)ENUMEOnayDurumu.GONDERILMEDI)
            //        durumAciklama = Resources.TasinirMal.FRMZFG063;
            //    else if (zfBelge.durum == (int)ENUMEOnayDurumu.GONDERILDI)
            //        durumAciklama = Resources.TasinirMal.FRMZFG059;
            //    else if (zfBelge.durum == (int)ENUMEOnayDurumu.CEVAPLADIKABUL)
            //        durumAciklama = Resources.TasinirMal.FRMZFG060;
            //    else if (zfBelge.durum == (int)ENUMEOnayDurumu.CEVAPLADIRED)
            //        durumAciklama = Resources.TasinirMal.FRMZFG061;

            //    tablo.Rows.Add((zfBelge.belgeTur == 1 ? "ZIM" : "ORT"), zfBelge.fisNo, zfBelge.yil, zfBelge.muhasebeKod + " - " + zfBelge.muhasebeAd, zfBelge.harcamaBirimKod + " - " + zfBelge.harcamaBirimAd, zfBelge.emailKimdenAd, zfBelge.emailGondermeTarih, zfBelge.emailCevapTarih, durumAciklama);
            //}

            //gvZimmet.DataSource = tablo;
            //gvZimmet.DataBind();
            //gvZimmet.Visible = true;

        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    string durum = e.Row.Cells[9].Text;

            //    foreach (TableCell cell in e.Row.Cells)
            //    {
            //        if (durum == "eOnaya Cevaplandý (RED)")
            //        {
            //            e.Row.BackColor = Color.LightCoral;
            //        }
            //        if (durum == "eOnaya Cevaplandý (KABUL)")
            //        {
            //            e.Row.BackColor = Color.LightGreen;
            //        }

            //    }
            //}
        }
        /// <summary>
        /// Zimmet Formu Dök tuþuna basýlýnca çalýþan olay metodu
        /// Sayfadaki gvZimmet GridView kontrolündeki seçilen zimmet bilgilerini
        /// zimmet formu excel raporuna yazar ve kullanýcýya gönderir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnZFRapor_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < gvZimmet.Rows.Count; i++)
            //{
            //    GridViewRow row = gvZimmet.Rows[i];
            //    if (!((CheckBox)row.FindControl("chkSecim")).Checked)
            //        continue;
                
            //    int yil = OrtakFonksiyonlar.ConvertToInt(Server.HtmlDecode(row.Cells[3].Text),0);
            //    string fisNo = Server.HtmlDecode(row.Cells[2].Text);
            //    string harcamaKod = Server.HtmlDecode(row.Cells[5].Text);
            //    string muhasebeKod = Server.HtmlDecode(row.Cells[4].Text);
            //    int belgeTur = (Server.HtmlDecode(row.Cells[1].Text) == "ZIM" ? 1 : 2);

            //    muhasebeKod = Server.HtmlDecode(row.Cells[4].Text).Split('-')[0].Trim();
            //    harcamaKod = Server.HtmlDecode(row.Cells[5].Text).Split('-')[0].Trim();

            //    Kullanici kullanan = new Kullanici();
            //    kullanan.kullaniciKodu = "stratek";
            //    kullanan.kullaniciTipi = new ENUMKullaniciTipi[1];
            //    kullanan.kullaniciTipi[0] = ENUMKullaniciTipi.SISTEMYONETICISI;

            //    TNS.TMM.ZimmetForm zf = new TNS.TMM.ZimmetForm();

            //    zf.yil = yil;
            //    zf.fisNo = fisNo;
            //    zf.harcamaBirimKod = harcamaKod;
            //    zf.muhasebeKod = muhasebeKod;
            //    zf.belgeTur = belgeTur;

            //    ObjectArray bilgi = servisTMM.ZimmetFisiAc(kullanan, zf);

            //    if (!bilgi.sonuc.islemSonuc)
            //        return;

            //    zf = (TNS.TMM.ZimmetForm)bilgi[0];

            //    ZimmetDetay zdal = new ZimmetDetay();

            //    zdal.yil = zf.yil;
            //    zdal.muhasebeKod = zf.muhasebeKod;
            //    zdal.harcamaBirimKod = zf.harcamaBirimKod;
            //    zdal.fisNo = zf.fisNo;
            //    zdal.belgeTur = zf.belgeTur;

            //    ObjectArray detay = servisTMM.ZimmetFisiDetayListele(kullanan, zf);

            //    Tablo XLS = GenelIslemler.NewTablo();

            //    int satir = 0;
            //    int sutun = 0;
            //    int kaynakSatir = 0;

            //    string sonucDosyaAd = System.IO.Path.GetTempFileName();
            //    string sablonAd = "ZIFDEMIRBAS.XLT";

            //    XLS.DosyaAc(new istemciUzayi.GenelSayfa().raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            //    XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            //    satir = kaynakSatir;

            //    XLS.HucreAdBulYaz("BelgeNo", zf.fisNo);
            //    XLS.HucreAdBulYaz("BelgeTarih", zf.fisTarih.ToString());
            //    XLS.HucreAdBulYaz("IlAd", zf.ilAd + "-" + zf.ilceAd);
            //    XLS.HucreAdBulYaz("IlKod", zf.ilKod + "-" + zf.ilceKod);
            //    XLS.HucreAdBulYaz("HarcamaAd", zf.harcamaBirimAd);
            //    XLS.HucreAdBulYaz("HarcamaKod", zf.harcamaBirimKod);

            //    if (!string.IsNullOrEmpty(zf.kimeGitti))
            //    {
            //        //string ad = uzyServis.UzayDegeriStr(null, "TASPERSONEL", zf.kimeGitti, true, "");
            //        //if (string.IsNullOrEmpty(ad))
            //            XLS.HucreAdBulYaz("KimeVerildi", zf.kimeGitti);
            //        //else
            //        //    XLS.HucreAdBulYaz("KimeVerildi", zf.kimeGitti + "-" + ad);
            //    }

            //    if (!string.IsNullOrEmpty(zf.nereyeGitti))
            //    {
            //        //string oda = uzyServis.UzayDegeriStr(null, "TASODA", zf.muhasebeKod + "-" + zf.harcamaBirimKod + "-" + zf.nereyeGitti, true, "");
            //        //if (string.IsNullOrEmpty(oda))
            //            XLS.HucreAdBulYaz("NereyeVerildi", zf.nereyeGitti);
            //        //else
            //        //    XLS.HucreAdBulYaz("NereyeVerildi", zf.nereyeGitti + "-" + oda);
            //    }

            //                if (zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME)
            //{
            //    XLS.HucreAdBulYaz("KimeVerildiYazi", Resources.TasinirMal.FRMZFY007);
            //    XLS.HucreAdBulYaz("TurAciklama", Resources.TasinirMal.FRMZFY008);
            //    XLS.HucreAdBulYaz("TeslimAlVer", Resources.TasinirMal.FRMZFY002);
            //    XLS.HucreAdBulYaz("VermeDusme", Resources.TasinirMal.FRMZFY003);
            //}
            //else if (zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME)
            //{
            //    XLS.HucreAdBulYaz("KimeVerildiYazi", Resources.TasinirMal.FRMZFY009);
            //    XLS.HucreAdBulYaz("TurAciklama", Resources.TasinirMal.FRMZFY010);
            //    XLS.HucreAdBulYaz("TeslimAlVer", Resources.TasinirMal.FRMZFY005);
            //    XLS.HucreAdBulYaz("VermeDusme", Resources.TasinirMal.FRMZFY006);
            //}

            ////Satýr ekleyince hücre adresleri kaydýðý için önce yazýyorum
            //XLS.HucreAdBulYaz("Tarih1", DateTime.Today.Date.ToShortDateString());
            ////XLS.HucreAdBulYaz("Tarih2", DateTime.Today.Date.ToShortDateString());
            //if (!string.IsNullOrEmpty(zf.kimeGitti))
            //{
            //    //string ad = uzyServis.UzayDegeriStr(null, "TASPERSONEL", zf.kimeGitti, true, "");
            //    //if (string.IsNullOrEmpty(ad))
            //        XLS.HucreAdBulYaz("AdSoyad2", zf.kimeGitti);
            //    //else
            //    //    XLS.HucreAdBulYaz("AdSoyad2", ad);

            //    //string unvan = uzyServis.UzayDegeriStr(null, "TASPERSONEL", zf.kimeGitti, true, "UNVAN");
            //    //if (!string.IsNullOrEmpty(unvan))
            //    //    XLS.HucreAdBulYaz("Unvan2", unvan);
            //}

            ////ImzaEkle(XLS, zf.muhasebeKod, zf.harcamaBirimKod, zf.ambarKod);

            //kaynakSatir = 0;
            //sutun = 0;

            //XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            //int sayac = 0;
            //satir = kaynakSatir;

            //foreach (ZimmetDetay zd in detay.objeler)
            //{
            //    satir++;
            //    XLS.SatirAc(satir, 1);
            //    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);
            //    XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 2);
            //    XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 5);
            //    XLS.HucreBirlestir(satir, sutun + 6, satir, sutun + 7);

            //    XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

            //    XLS.HucreDegerYaz(satir, sutun, ++sayac);

            //    if (TasinirGenel.rfIdVarMi)
            //        XLS.HucreDegerYaz(satir, sutun + 1, zd.gorSicilNo + " - " + zd.rfIdNo);
            //    else
            //        XLS.HucreDegerYaz(satir, sutun + 1, zd.gorSicilNo);

            //    XLS.HucreDegerYaz(satir, sutun + 3, zd.hesapPlanAd);
            //    XLS.HucreDegerYaz(satir, sutun + 6, zd.ozellik);

            //}
            //    //ZimmetFormYazdir.Yazdir(kuln, yil, fisNo, harcamaKod, muhasebeKod, belgeTur, excelYazYer, false);
            //XLS.DosyaSaklaTamYol();
            //OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            //}
        }

        /// <summary>
        /// Zimmet Formu Onay KABUL tuþuna basýlýnca çalýþan olay metodu
        /// Sayfadaki gvZimmet GridView kontrolündeki seçilen zimmet bilgilerini
        /// KABUL etmek için kullanýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnZFOnayKabul_Click(object sender, EventArgs e)
        {
            //ZimmetFormEOnay zfeOnay = new ZimmetFormEOnay();
            //Sonuc s = new Sonuc();
            //for (int i = 0; i < gvZimmet.Rows.Count; i++)
            //{
            //    GridViewRow row = gvZimmet.Rows[i];
            //    if (!((CheckBox)row.FindControl("chkSecim")).Checked)
            //        continue;

            //    zfeOnay.yil = OrtakFonksiyonlar.ConvertToInt(Server.HtmlDecode(row.Cells[3].Text),0);
            //    zfeOnay.muhasebeKod = Server.HtmlDecode(row.Cells[4].Text).Split('-')[0].Trim();
            //    zfeOnay.harcamaBirimKod = Server.HtmlDecode(row.Cells[5].Text).Split('-')[0].Trim();
            //    zfeOnay.fisNo = Server.HtmlDecode(row.Cells[2].Text);
            //    zfeOnay.belgeTur =  (Server.HtmlDecode(row.Cells[1].Text) == "ZIM" ? 1 : 2);

            //    s = servisTMM.ZimmetFisiEOnayDurumDegistir(new Kullanici(), zfeOnay, "4", txtTCKimlik.Text);
            //    //kabulzf += Server.HtmlDecode(row.Cells[3].Text) + ";" + Server.HtmlDecode(row.Cells[4].Text).Split('-')[0].Trim() + ";" + Server.HtmlDecode(row.Cells[5].Text).Split('-')[0].Trim() + ";" + Server.HtmlDecode(row.Cells[2].Text) + ";" + (Server.HtmlDecode(row.Cells[1].Text) == "ZIM" ? 1 : 2) + ";" + "<br />";
            //}
            ////GenelIslemler.MesajKutusu("Bilgi", kabulzf);
            //if (s.islemSonuc)
            //{
            //    GenelIslemler.MesajKutusu("Bilgi", "Baþarý ile durumu KABUL olarak güncellendi");
            //    ZimmetKisiEOnayGrideYaz(KriterTopla());
            //}
            //else
            //{
            //    GenelIslemler.MesajKutusu("Uyarý", "Güncelleme YAPILAMADI !");
            //}
        }

        /// <summary>
        /// Zimmet Formu Onay RED tuþuna basýlýnca çalýþan olay metodu
        /// Sayfadaki gvZimmet GridView kontrolündeki seçilen zimmet bilgilerini
        /// RED etmek için kullanýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnZFOnayRed_Click(object sender, EventArgs e)
        {
            //ZimmetFormEOnay zfeOnay = new ZimmetFormEOnay();
            //Sonuc s = new Sonuc();
            //for (int i = 0; i < gvZimmet.Rows.Count; i++)
            //{
            //    GridViewRow row = gvZimmet.Rows[i];
            //    if (!((CheckBox)row.FindControl("chkSecim")).Checked)
            //        continue;

            //    zfeOnay.yil = OrtakFonksiyonlar.ConvertToInt(Server.HtmlDecode(row.Cells[3].Text), 0);
            //    zfeOnay.muhasebeKod = Server.HtmlDecode(row.Cells[4].Text).Split('-')[0].Trim();
            //    zfeOnay.harcamaBirimKod = Server.HtmlDecode(row.Cells[5].Text).Split('-')[0].Trim();
            //    zfeOnay.fisNo = Server.HtmlDecode(row.Cells[2].Text);
            //    zfeOnay.belgeTur = (Server.HtmlDecode(row.Cells[1].Text) == "ZIM" ? 1 : 2);

            //    s = servisTMM.ZimmetFisiEOnayDurumDegistir(new Kullanici(), zfeOnay, "5", txtTCKimlik.Text);
            //    //kabulzf += Server.HtmlDecode(row.Cells[3].Text) + ";" + Server.HtmlDecode(row.Cells[4].Text).Split('-')[0].Trim() + ";" + Server.HtmlDecode(row.Cells[5].Text).Split('-')[0].Trim() + ";" + Server.HtmlDecode(row.Cells[2].Text) + ";" + (Server.HtmlDecode(row.Cells[1].Text) == "ZIM" ? 1 : 2) + ";" + "<br />";
            //}
            ////GenelIslemler.MesajKutusu("Bilgi", kabulzf);
            //if (s.islemSonuc)
            //{
            //    GenelIslemler.MesajKutusu("Bilgi", "Baþarý ile durumu RED olarak güncellendi");
            //    ZimmetKisiEOnayGrideYaz(KriterTopla());
            //}
            //else
            //{
            //    GenelIslemler.MesajKutusu("Uyarý", "Güncelleme YAPILAMADI !");
            //}
        }
    }
}