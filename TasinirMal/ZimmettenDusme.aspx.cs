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
    /// Ambarlarý yýl bazýnda iþleme kapatma iþleminin yapýldýðý sayfa
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
                formAdi = "Toplu Zimmetten Düþme Ekraný";
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
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
        ///// <param name="sender">Olayý tetikleyen nesne</param>
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

        //        Progress1.UpdateProgress(0, "Ýþlem yapýlacak ambarlar belirleniyor.");
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
        //        Progress1.UpdateProgress(1, "Ýþlem tamamlandý.");
        //        islemGorenAmbar = 999999;
        //        btnCikis.Hidden = false;
        //        btnGiris.Hidden = false;
        //        btnIptal.Hidden = true;
        //        islemGorecekAmbar = -1;
        //        iptal = -1;
        //        olusanFis = 0;

        //        ResourceManager1.AddScript("alert('{0}');", "Yýl devri iþlemi baþarýyla sonuçlandý.");
        //    }
        //}

        protected void btnZimmettenDus_Click(object sender, DirectEventArgs e)
        {         
            string hataStr = "";           

            //Bütün harcama birimlerini ve ambarlarý listele
            Ambar ambar = new Ambar();
            ambar.muhasebeKod = txtMuhasebe.Text;
            ambar.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            ambar.kod = txtAmbar.Text;
            ambar.aktarim = true;

            ObjectArray bilgi = servisTMM.AmbarListele(kullanan, ambar);
            //foreach ile ambarlarý dolaþ
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
                //Kurum Devri çýkýþ listeyi çaðýr
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
            //                //Kayýt edilen fiþ onaylanýyor
            //                z.fisNo = sonuc.anahtar;
            //                //OrtakFonksiyonlar.HataDosyaYaz("ZimmettenDüsme.txt", "Muhasebe Birimi: "+ z.muhasebeKod+" Harcama Birimi: "+ z.harcamaBirimKod+ " Kiþi-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " FisNo: " + z.fisNo + " baþarýyla oluþtu ---->>> <br>", false);
            //                hataStr = "Muhasebe Birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Kiþi-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " FisNo: " + z.fisNo + " baþarýyla oluþtu ---->>> <br>";
            //                Sonuc onaySonuc = servisTMM.ZimmetFisiDurumDegistir(kullanan, z, "Onay");
            //                if (!onaySonuc.islemSonuc)
            //                {
            //                    hataStr += "Muhasebe Birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Kiþi-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " FisNo: " + z.fisNo + " fiþinin onayý sýrasýnda hata oluþtu ---->>> <br>";
            //                    //OrtakFonksiyonlar.HataDosyaYaz("ZimmettenDüsme.txt", "Muhasebe Birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Kiþi-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " FisNo: " + z.fisNo + " fiþinin onayý sýrasýnda hata oluþtu ---->>> <br>", false);
            //                }
            //                else
            //                {
            //                    //OrtakFonksiyonlar.HataDosyaYaz("ZimmettenDüsme.txt", "Muhasebe Birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Kiþi-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " FisNo: " + z.fisNo + " fiþinin onayý baþarýlý ---->>> <br>", false);
            //                    hataStr += "Muhasebe Birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Kiþi-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " FisNo: " + z.fisNo + " fiþinin onayý baþarýlý ---->>> <br>";
            //                }
            //            }
            //            else
            //            {
            //                hataStr += "Muhasebe Birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Kiþi-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " kiþi veya ortak alana ait fiþi oluþtururken hata oluþtu---->>> " + sonuc.hataStr + "<br>";
            //                //OrtakFonksiyonlar.HataDosyaYaz("ZimmettenDüsme.txt", "Muhasebe birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Kiþi-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " kiþi veya ortak alana ait fiþi oluþtururken hata oluþtu---->>> " + sonuc.hataStr + "<br>", false);
            //            }
            //        }
            //        else
            //        {
            //            hataStr += "Muhasebe Birimi: " + z.muhasebeKod + " Harcama Birimi: " + z.harcamaBirimKod + " Kiþi-Ortak Alan:" + z.kimeGitti + " " + z.nereyeGitti + " kiþi veya ortak alana ait malzemeleri getirirken hata oluþtu---->>>" + malzemeler.sonuc.hataStr + " < br > ";
            //            //OrtakFonksiyonlar.HataDosyaYaz("ZimmettenDüsme.txt", "Muhasebe birimi: " + z.muhasebeKod + " Harcama Birimi" + z.harcamaBirimKod + " Kiþi-Ortak Alan: " + z.kimeGitti + " " + z.nereyeGitti + " kiþi veya ortak alana ait malzemeleri getirirken hata oluþtu---->>> " + malzemeler.sonuc.hataStr + "<br>", false);
            //        }
            //    }
            //}
            //else
            //{
            //    hataStr += "Muhasebe Birimi: " + zf.muhasebeKod + " Harcama Birimi: " + zf.harcamaBirimKod+ "Kiþi veya Ortak Alan getirirken hata oluþtu---->>> " + zimmetKisiler.sonuc.hataStr + "<br>";
            //    //OrtakFonksiyonlar.HataDosyaYaz("ZimmettenDüsme.txt", "Muhasebe Birimi: " + zf.muhasebeKod + " Harcama Birimi: " + zf.harcamaBirimKod + "Kiþi veya Ortak Alan getirirken hata oluþtu---->>> " + zimmetKisiler.sonuc.hataStr + "<br>", false);
            //}

            return hataStr;
        }
    }
}
