using Ext1.Net;
using OrtakClass;
using System;
using System.Threading;
using TNS;
using TNS.TMM;
using TNS.UZY;

namespace TasinirMal
{
    /// <summary>
    /// Ambarlar� y�l baz�nda i�leme kapatma i�leminin yap�ld��� sayfa
    /// </summary>
    public partial class ZimmettenDusme : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
        static int islemGorenAmbar = 0;
        static int islemGorecekAmbar = -1;
        static int iptal = -1;
        static int olusanFis = 0;

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
                formAdi = "Toplu Zimmetten D��me Ekran�";
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giri� izni varm�?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                txtYil.Value = DateTime.Now.Year;
            }
        }

        ///// <summary>
        ///// Handles the Click event of the btnIptal control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="Ext1.Net.DirectEventArgs"/> instance containing the event data.</param>
        //protected void btnIptal_Click(object sender, DirectEventArgs e)
        //{
        //    iptal = 100;
        //}

        ///// <summary>
        ///// Handles the Click event of the btnCikis control.
        ///// </summary>
        ///// <param name="sender">Olay� tetikleyen nesne</param>
        ///// <param name="e">Olay parametresi</param>
        //protected void btnCikis_Click(object sender, DirectEventArgs e)
        //{
        //    btnCikis.Hidden = true;
        //    btnGiris.Hidden = true;
        //    btnIptal.Hidden = false;

        //    ResourceManager1.AddScript("{0}.startTask('IslemGostergec');", TaskManager1.ClientID);
        //    ThreadPool.QueueUserWorkItem(CikisIsle);
        //}             


        ///// <summary>
        ///// Refreshes the progress.
        ///// </summary>
        ///// <param name="sender">The sender.</param>
        ///// <param name="e">The <see cref="Ext1.Net.DirectEventArgs"/> instance containing the event data.</param>
        //protected void RefreshProgress(object sender, DirectEventArgs e)
        //{
        //    if (islemGorecekAmbar == -1)
        //    {
        //        lblToplamAmbar.Text = "0";
        //        lblAmbarSayac.Text = "0";
        //        lblKalanAmbar.Text = "0";
        //        lblBelgeSayac.Text = "0";

        //        Progress1.UpdateProgress(0, "��lem yap�lacak ambarlar belirleniyor.");
        //    }
        //    else if (islemGorenAmbar < islemGorecekAmbar)
        //    {
        //        lblToplamAmbar.Text = islemGorecekAmbar.ToString("#,###");
        //        lblAmbarSayac.Text = islemGorenAmbar.ToString("#,###");
        //        lblKalanAmbar.Text = (islemGorecekAmbar - islemGorenAmbar).ToString("#,###");
        //        lblBelgeSayac.Text = olusanFis.ToString("#,###");
        //        Progress1.UpdateProgress(islemGorenAmbar / (float)islemGorecekAmbar, string.Format("% {0}", ((islemGorenAmbar / (float)islemGorecekAmbar) * 100).ToString("###")));
        //    }
        //    else
        //    {
        //        ResourceManager1.AddScript("{0}.stopTask('IslemGostergec');", TaskManager1.ClientID);
        //        Progress1.UpdateProgress(1, "��lem tamamland�.");
        //        islemGorenAmbar = 999999;
        //        btnCikis.Hidden = false;
        //        btnGiris.Hidden = false;
        //        btnIptal.Hidden = true;
        //        islemGorecekAmbar = -1;
        //        iptal = -1;
        //        olusanFis = 0;

        //        ResourceManager1.AddScript("alert('{0}');", "Y�l devri i�lemi ba�ar�yla sonu�land�.");
        //    }
        //}

        protected void btnZimmettenDus_Click(object sender, DirectEventArgs e)
        {         
            string hataStr = "";           

            //B�t�n harcama birimlerini ve ambarlar� listele
            Ambar ambar = new Ambar();
            ambar.muhasebeKod = txtMuhasebe.Text;
            ambar.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            ambar.kod = txtAmbar.Text;
            ambar.aktarim = true;

            ObjectArray bilgi = servisTMM.AmbarListele(kullanan, ambar);
            //foreach ile ambarlar� dola�
            islemGorecekAmbar = bilgi.objeler.Count;
            islemGorenAmbar = 0;

            foreach (Ambar a in bilgi.objeler)
            {
                if (iptal == 100)
                {
                    islemGorenAmbar = 999999;
                    break;
                }

                TNS.TMM.ZimmetForm zf = new TNS.TMM.ZimmetForm();
                zf.muhasebeKod = a.muhasebeKod;
                zf.harcamaBirimKod =  a.harcamaBirimKod;
                zf.ambarKod = a.kod;
                zf.belgeTur = OrtakFonksiyonlar.ConvertToInt(cmbBelgeTuru.Value, 0);
                //Kurum Devri ��k�� listeyi �a��r
                //if(islemGorenAmbar<5)
                    hataStr+= ZimmettenDus(zf);
                islemGorenAmbar++;
            }

            GenelIslemler.MesajKutusu("Mesaj", hataStr);
        }

        string ZimmettenDus(TNS.TMM.ZimmetForm zf)
        {
            string hataStr = "";
            //ObjectArray zimmetKisiler = new ObjectArray();
            //int belgeTuru = 0;

            //if (zf.belgeTur == 1001)
            //{
            //    belgeTuru = (int)ENUMZimmetBelgeTur.ZIMMETFISI;
            //    zimmetKisiler = servisTMM.ZimmetKisiBirimAmbarBazli(kullanan, zf);
            //}
            //else
            //{
            //    belgeTuru = (int)ENUMZimmetBelgeTur.DAYANIKLITL;
            //    zimmetKisiler = servisTMM.ZimmetOrtakAlanBirimAmbarBazli(kullanan, zf);
            //}

            //if (zimmetKisiler.sonuc.islemSonuc)
            //{
            //    foreach (TNS.TMM.ZimmetForm z in zimmetKisiler.objeler)
            //    {
            //        z.yil = System.DateTime.Now.Year;
            //        z.belgeTur = OrtakFonksiyonlar.ConvertToInt(cmbBelgeTuru.Value, 0);
            //        z.vermeDusme = (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME;
            //        z.fisTarih = new TNSDateTime(System.DateTime.Now);
            //        z.islemTarih = new TNSDateTime(System.DateTime.Now);
            //        z.dusmeTarih = new TNSDateTime(System.DateTime.Now);
            //        z.islemYapan = kullanan.kullaniciKodu;
            //        z.tip = (int)ENUMZimmetTipi.DEMIRBASCIHAZ;

            //        ObjectArray malzemeler = servisTMM.ZimmetKisideki(kullanan, z);
            //        z.belgeTur = belgeTuru;

            //        if (malzemeler.sonuc.islemSonuc)
            //        {
            //            z.aktarim = true;
            //            Sonuc sonuc = servisTMM.ZimmetFisiKaydet(kullanan, z, malzemeler);
            //            if (sonuc.islemSonuc)
            //            {
            //                //Kay�t edilen fi� onaylan�yor
            //                z.fisNo = sonuc.anahtar;
            //                //OrtakFonksiyonlar.HataDosyaYaz("ZimmettenD�sme.txt", "Muhasebe Birimi: "+ z.muhasebeKod+" Harcama Birimi: "+ z.harcamaBirimKod+ " Ki�i-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " FisNo: " + z.fisNo + " ba�ar�yla olu�tu ---->>> <br>", false);
            //                hataStr = "Muhasebe Birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Ki�i-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " FisNo: " + z.fisNo + " ba�ar�yla olu�tu ---->>> <br>";
            //                Sonuc onaySonuc = servisTMM.ZimmetFisiDurumDegistir(kullanan, z, "Onay");
            //                if (!onaySonuc.islemSonuc)
            //                {
            //                    hataStr += "Muhasebe Birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Ki�i-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " FisNo: " + z.fisNo + " fi�inin onay� s�ras�nda hata olu�tu ---->>> <br>";
            //                    //OrtakFonksiyonlar.HataDosyaYaz("ZimmettenD�sme.txt", "Muhasebe Birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Ki�i-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " FisNo: " + z.fisNo + " fi�inin onay� s�ras�nda hata olu�tu ---->>> <br>", false);
            //                }
            //                else
            //                {
            //                    //OrtakFonksiyonlar.HataDosyaYaz("ZimmettenD�sme.txt", "Muhasebe Birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Ki�i-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " FisNo: " + z.fisNo + " fi�inin onay� ba�ar�l� ---->>> <br>", false);
            //                    hataStr += "Muhasebe Birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Ki�i-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " FisNo: " + z.fisNo + " fi�inin onay� ba�ar�l� ---->>> <br>";
            //                }
            //            }
            //            else
            //            {
            //                hataStr += "Muhasebe Birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Ki�i-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " ki�i veya ortak alana ait fi�i olu�tururken hata olu�tu---->>> " + sonuc.hataStr + "<br>";
            //                //OrtakFonksiyonlar.HataDosyaYaz("ZimmettenD�sme.txt", "Muhasebe birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Ki�i-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " ki�i veya ortak alana ait fi�i olu�tururken hata olu�tu---->>> " + sonuc.hataStr + "<br>", false);
            //            }
            //        }
            //        else
            //        {
            //            hataStr += "Muhasebe Birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Ki�i-Ortak Alan:" + z.kimeGitti + " " + z.nereyeGitti + " ki�i veya ortak alana ait malzemeleri getirirken hata olu�tu---->>>" + malzemeler.sonuc.hataStr + " < br > ";
            //            //OrtakFonksiyonlar.HataDosyaYaz("ZimmettenD�sme.txt", "Muhasebe birimi: " + z.muhasebeKod + " Harcama Birimi" + z.harcamaBirimKod + " Ki�i-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " ki�i veya ortak alana ait malzemeleri getirirken hata olu�tu---->>> " + malzemeler.sonuc.hataStr + "<br>", false);
            //        }
            //    }
            //}
            //else
            //{
            //    hataStr += "Muhasebe Birimi: " + zf.muhasebeKod + " Harcama Birimi: " + zf.harcamaBirimKod+ "Ki�i veya Ortak Alan getirirken hata olu�tu---->>> " + zimmetKisiler.sonuc.hataStr + "<br>";
            //    //OrtakFonksiyonlar.HataDosyaYaz("ZimmettenD�sme.txt", "Muhasebe Birimi: " + zf.muhasebeKod + " Harcama Birimi: " + zf.harcamaBirimKod + "Ki�i veya Ortak Alan getirirken hata olu�tu---->>> " + zimmetKisiler.sonuc.hataStr + "<br>", false);
            //}

            return hataStr;
        }
    }
}
