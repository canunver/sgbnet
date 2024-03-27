using System;
using System.Data;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Ta��n�r demirba� listesinin verilen kriterlere g�re d�nd�r�l�p listelendi�i sayfa
    /// </summary>
    public partial class ListeSicilNo : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur, yoksa giri� ekran�na y�nlendirilir varsa sayfa y�klenir.
        ///     Sayfa ilk defa �a��r�l�yorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlan�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);
            GenelIslemlerIstemci.JSResourceEkle(Resources.TasinirMal.ResourceManager, this, "ListeSicilNo", "FRMJSC010");

            //Giri� s�ras�nda kullan�c�n�n varl���n� kontrol et yoksa sayfaya girme
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);

            this.txtMarka.Attributes.Add("onblur", "kodAdGetir('37','lblMarkaAd',true,new Array('txtMarka'),'KONTROLDENOKU');");
            this.txtModel.Attributes.Add("onblur", "kodAdGetir('38','lblModelAd',true,new Array('txtMarka','txtModel'),'KONTROLDENOKU');");
            this.txtHesapPlaniKodu.Attributes.Add("onblur", "kodAdGetir('30','lblHesapPlanAd',true,new Array('txtHesapPlaniKodu'),'KONTROLDENOKU');");
            this.btnListele.Attributes.Add("onclick", "SicilNolariAl();");

            if (!IsPostBack)
            {
                GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);

                if (Request.QueryString["hpk"] != null) //hpk -> Hesap Plan� Kodu
                    txtHesapPlaniKodu.Text = Request.QueryString["hpk"].Trim();

                hdnCagiran.Value = Request.QueryString["cagiran"] + "";
                hdnCagiranLabel.Value = Request.QueryString["cagiranLabel"] + "";

                string kayitSayisi = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "SICILLISTEKAYITSAYISI");
                if (OrtakFonksiyonlar.ConvertToInt(kayitSayisi, 0) > 0)
                    txtKayitSay.Text = kayitSayisi;
                else
                    txtKayitSay.Text = "15"; //Default 15 kay�t getirsin

                if (!string.IsNullOrEmpty(Request.QueryString["TIF"]))
                    btnSecilenleriAktar2.Visible = true;
                else
                    btnSecilenAktar.Style.Remove("display");
            }

            if (txtMarka.Text.Trim() != "")
                lblMarkaAd.Text = GenelIslemler.KodAd(37, txtMarka.Text.Trim(), true);
            if (txtModel.Text.Trim() != "")
                lblModelAd.Text = GenelIslemler.KodAd(38, txtMarka.Text.Trim() + "-" + txtModel.Text.Trim(), true);
        }

        /// <summary>
        /// Sayfa adresinde gelen girdi dizgilerindeki ve sayfadaki kontrollerdeki kriterler toplan�r ve
        /// kriterlere uygun olan demirba�lar listelenmek �zere SicilNumarasiDoldur yordam� �a��r�l�r.
        /// </summary>
        public void KriterTopla()
        {
            SicilNoHareket shBilgi = new SicilNoHareket();

            int zimmetVermeDusme = 0;

            if (Request.QueryString["mb"] != null) //mb -> Muhasebe Birimi Kodu
                shBilgi.muhasebeKod = Request.QueryString["mb"].Trim();

            if (Request.QueryString["hbk"] != null) //hbk -> Harcama Birimi Kodu
                shBilgi.harcamaBirimKod = Request.QueryString["hbk"].Trim().Replace(".", "");

            //Erdal commenti a�t�.
            if (Request.QueryString["yil"] != null) //Yoruma gerek var m�
                shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"], 0);

            if (Request.QueryString["ak"] != null) //ak -> Ambar Kodu
                shBilgi.ambarKod = Request.QueryString["ak"].Trim();

            if (Request.QueryString["kisi"] != null) //personel kodu
                shBilgi.kimeGitti = Request.QueryString["kisi"].Trim();

            if (Request.QueryString["oda"] != null) //oda kodu
                shBilgi.nereyeGitti = Request.QueryString["oda"].Trim();

            if (Request.QueryString["vermeDusme"] != null) //zimmet tipi verme/d��me
                zimmetVermeDusme = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["vermeDusme"].Trim().Replace(".", ""), 0);

            if (Request.QueryString["belgeTur"] != null) //zimmet ki�i/zimmet ortak alan
            {
                int belgeTur = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["belgeTur"].Trim(), 0);
                if (belgeTur == (int)ENUMZimmetBelgeTur.ZIMMETFISI)
                    shBilgi.islemTurKod = (int)ENUMIslemTipi.ZFVERME;
                else if (belgeTur == (int)ENUMZimmetBelgeTur.DAYANIKLITL)
                    shBilgi.islemTurKod = (int)ENUMIslemTipi.DTLVERME;
            }

            if (txtMarka.Text != "")
                shBilgi.ozellik.markaKod = OrtakFonksiyonlar.ConvertToInt(txtMarka.Text.Trim(), 0);

            if (txtModel.Text != "")
                shBilgi.ozellik.modelKod = OrtakFonksiyonlar.ConvertToInt(txtModel.Text.Trim(), 0);

            if (txtPlaka.Text != "")
                shBilgi.ozellik.plaka = txtPlaka.Text.Trim();

            if (txtEserAdi.Text != "")
                shBilgi.ozellik.adi = txtEserAdi.Text.Trim();

            if (txtHesapPlaniKodu.Text != "")
                shBilgi.hesapPlanKod = txtHesapPlaniKodu.Text.Trim();

            if (txtHesapPlaniAdi.Text != "")
                shBilgi.hesapPlanAd = txtHesapPlaniAdi.Text.Trim();

            if (txtBelgeNo.Text != "")
            {
                shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
                shBilgi.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');
            }

            if (txtEskiSicil.Text != "")
                shBilgi.ozellik.disSicilNo = txtEskiSicil.Text;

            if (string.IsNullOrEmpty(txtSeriNo.Text) && fupSeriNo.PostedFile != null && fupSeriNo.PostedFile.ContentLength > 0)
            {
                shBilgi.ozellik.saseNo = ExceldenSeriNoOku();
                if (string.IsNullOrEmpty(shBilgi.ozellik.saseNo))
                    return;
            }
            else
                shBilgi.ozellik.saseNo = txtSeriNo.Text.Trim();

            if (!string.IsNullOrEmpty(txtSicilNo.Text))
                shBilgi.sicilNo = txtSicilNo.Text.Trim();

            SicilNumarasiDoldur(shBilgi, zimmetVermeDusme);
        }

        private string ExceldenSeriNoOku()
        {
            try
            {
                byte[] myData = new byte[fupSeriNo.PostedFile.ContentLength];
                fupSeriNo.PostedFile.InputStream.Read(myData, 0, fupSeriNo.PostedFile.ContentLength);

                string dosyaAd = System.IO.Path.GetTempFileName();
                if (string.IsNullOrEmpty(dosyaAd))
                {
                    GenelIslemler.MesajKutusu("Uyar�", Resources.TasinirMal.FRMTIG077);
                    return string.Empty;
                }

                System.IO.FileStream newFile = new System.IO.FileStream(dosyaAd, System.IO.FileMode.Create);
                newFile.Write(myData, 0, myData.Length);
                newFile.Close();

                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaAc(dosyaAd);
                int satir = 0;
                string seriNolar = string.Empty;
                while (true)
                {
                    string hucreBilgi = XLS.HucreDegerAl(satir, 0);
                    if (string.IsNullOrEmpty(hucreBilgi))
                        break;
                    if (!string.IsNullOrEmpty(seriNolar))
                        seriNolar += ";";
                    seriNolar += hucreBilgi;
                    satir++;
                }

                XLS.DosyaKapat();
                System.IO.File.Delete(dosyaAd);
                return seriNolar;
            }
            catch (Exception ex)
            {
                GenelIslemler.MesajKutusu("Uyar�", ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Parametre olarak verilen kriterlere uyan demirba�lar� sayfadaki GridView kontrol�ne dolduran yordam
        /// zimmetVermeDusme parametresine bakarak sunucunun hangi yordam�n� �a��raca��na karar verir.
        /// zimmetVermeDusme de�eri 1 ise SicilNoListele, 2 ise ZimmetliSicilNoListele, -1 ise
        /// ButunSicilNoListele ve bunlar�n d���nda bir de�erse SicilNoListele sunucu yordam� �a��r�l�r.
        /// </summary>
        /// <param name="shBilgi">Demirba� listeleme kriter bilgilerini tutan nesne</param>
        /// <param name="zimmetVermeDusme">Hangi demirba�lar�n istendi�i bilgisi</param>
        private void SicilNumarasiDoldur(SicilNoHareket shBilgi, int zimmetVermeDusme)
        {
            int sayfaNo = OrtakFonksiyonlar.ConvertToInt(txtSayfaNo.Text, 0);
            sayfaNo++;
            if (sayfaNo <= 0)
                sayfaNo = 1;
            int kayitSay = OrtakFonksiyonlar.ConvertToInt(txtKayitSay.Text, 0);
            int ilkKayit = (sayfaNo - 1) * kayitSay;

            if (kayitSay > 0)
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "SICILLISTEKAYITSAYISI", txtKayitSay.Text);

            ObjectArray bilgi = new ObjectArray();

            if (zimmetVermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME)
                bilgi = servisTMM.SicilNoListele(kullanan, shBilgi, ilkKayit, kayitSay);
            else if (zimmetVermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME)
                bilgi = servisTMM.ZimmetliSicilNoListele(kullanan, shBilgi);
            else if (zimmetVermeDusme == -1)//Erdal --- Dayan�kl� ta��n�r�n tarih�esinde zimmetliler de gelsin diye eklendi.
                bilgi = servisTMM.ButunSicilNoListele(kullanan, shBilgi, ilkKayit, kayitSay);
            else
                bilgi = servisTMM.SicilNoListele(kullanan, shBilgi, ilkKayit, kayitSay);

            if (bilgi.sonuc.islemSonuc)
            {
                if (bilgi.objeler.Count > 0)
                {
                    txtSayfaNo.Text = (sayfaNo).ToString();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("sicilno");
                    dt.Columns.Add("kod");
                    dt.Columns.Add("ad");
                    dt.Columns.Add("kdvoran");
                    dt.Columns.Add("birimfiyat");
                    dt.Columns.Add("rfid");
                    if (TasinirGenel.rfIdVarMi)
                        gvSicilNo.Columns[gvSicilNo.Columns.Count - 1].Visible = true;

                    string[] griddekiSicilNolar = hdnSicilNolar.Value.Split(';');
                    bool sKontrolFlag;

                    foreach (SicilNoHareket sh in bilgi.objeler)
                    {
                        sKontrolFlag = false;

                        for (int i = 0; i < griddekiSicilNolar.Length; i++)
                        {
                            if (griddekiSicilNolar[i] == sh.sicilNo)
                                sKontrolFlag = true;
                        }

                        if (sKontrolFlag == true)
                            continue;

                        string ozellik = "";

                        if (!String.IsNullOrEmpty(sh.ozellik.markaAd))
                            ozellik = sh.ozellik.markaAd;
                        if (!String.IsNullOrEmpty(sh.ozellik.modelAd))
                        {
                            if (ozellik != "") ozellik += "-";
                            ozellik += sh.ozellik.modelAd;
                        }
                        if (!String.IsNullOrEmpty(sh.ozellik.adi))
                        {
                            if (ozellik != "") ozellik += "-";
                            ozellik += sh.ozellik.adi;
                        }
                        if (!String.IsNullOrEmpty(sh.ozellik.yazarAdi))
                        {
                            if (ozellik != "") ozellik += "-";
                            ozellik += sh.ozellik.yazarAdi;
                        }
                        if (!String.IsNullOrEmpty(sh.ozellik.plaka))
                        {
                            if (ozellik != "") ozellik += "-";
                            ozellik += sh.ozellik.plaka;
                        }
                        if (!String.IsNullOrEmpty(sh.ozellik.disSicilNo))
                        {
                            if (ozellik != "") ozellik += "-";
                            ozellik += sh.ozellik.disSicilNo;
                        }
                        if (!String.IsNullOrEmpty(sh.ozellik.saseNo))
                        {
                            if (ozellik != "") ozellik += "-";
                            ozellik += sh.ozellik.saseNo;
                        }
                        if (!String.IsNullOrEmpty(sh.ozellik.yeriKonusu))
                        {
                            if (ozellik != "") ozellik += "-";
                            ozellik += sh.ozellik.yeriKonusu;
                        }
                        if (ozellik != "") ozellik = " (" + ozellik + ")";

                        dt.Rows.Add(sh.sicilNo, sh.hesapPlanKod, sh.hesapPlanAd + ozellik, sh.kdvOran, sh.fiyat.ToString(), sh.rfIdNo);

                        if (kayitSay > 0)
                        {
                            if (dt.Rows.Count >= kayitSay)
                                break;
                        }
                    }

                    gvSicilNo.DataSource = dt;
                }
                else
                    GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLSC002);

                gvSicilNo.DataBind();
            }
            else
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);
        }

        /// <summary>
        /// Listele tu�una bas�l�nca �al��an olay metodu
        /// Ta��n�r demirba�lar� listelensin diye KriterTopla yordam�n� �a��r�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, EventArgs e)
        {
            btnListele.Visible = false;
            btnSonrakiSayfa.Visible = true;
            btnYeniSorgu.Visible = true;
            KriterTopla();
        }

        /// <summary>
        /// Yeni sorgu tu�una bas�l�nca �al��an olay metodu
        /// Ekran� temizler ve yeni sorgu ba�lat�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYeniSorgu_Click(object sender, EventArgs e)
        {
            BaslangicAyarlarinaDon();         
        }

        /// <summary>
        /// Ekran� temizler ve yeni sorgu ba�lat�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        public void BaslangicAyarlarinaDon()
        {
            btnYeniSorgu.Visible = false;
            btnListele.Visible = true;
            btnSonrakiSayfa.Visible = false;

            gvSicilNo.DataSource = null;
            gvSicilNo.DataBind();
            txtSayfaNo.Text = "";
        }

        /// <summary>
        /// Handles the Click event of the btnSecilenleriAktar2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnSecilenleriAktar2_Click(object sender, EventArgs e)
        {
            ObjectArray o = new ObjectArray();

            for (int i = 0; i < gvSicilNo.Rows.Count; i++)
            {
                TableCell c = gvSicilNo.Rows[i].Cells[0];
                CheckBox chk = (CheckBox)c.FindControl("chkSecim");
                if (chk.Checked)
                {
                    TasinirIslemDetay ts = new TasinirIslemDetay();
                    ts.gorSicilNo = gvSicilNo.Rows[i].Cells[1].Text;
                    ts.hesapPlanKod = gvSicilNo.Rows[i].Cells[2].Text;
                    ts.hesapPlanAd = gvSicilNo.Rows[i].Cells[3].Text;
                    ts.kdvOran = OrtakFonksiyonlar.ConvertToInt(gvSicilNo.Rows[i].Cells[4].Text, 0);
                    ts.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(gvSicilNo.Rows[i].Cells[5].Text, 0);
                    ts.miktar = 1;
                    o.objeler.Add(ts);
                }
            }

            OturumBilgisiIslem.BilgiYazDegisken("DevirSicilNolar", o);
            ClientScript.RegisterStartupScript(this.GetType(), "yukle", "TasinirFormunaSicilNoYukle();", true);
        }
    }
}