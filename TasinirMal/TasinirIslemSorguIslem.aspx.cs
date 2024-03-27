using System;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr iþlem fiþlerinin onaylama, onay kaldýrma, iptal etme ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TasinirIslemSorguIslem : istemciUzayi.GenelSayfa
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servis = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur, yoksa giriþ ekranýna yönlendirilir
        ///     varsa sayfa yüklenir ve istenen iþlem yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {

            if (txtIslem.Value == String.Empty)
                return;


            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            Islem();
        }

        /// <summary>
        /// Belge durum deðiþtirme ve belge yazdýrma iþlemlerini yapan yordam
        /// </summary>
        private void Islem()
        {
            string[] yillar = txtYil.Value.Split(';');
            string[] muhasebeKodlar = txtMuhasebeKod.Value.Split(';');
            string[] harcamaBirimKodlar = txtHarcamaBirimKod.Value.Split(';');
            string[] fisNolar = txtFisNo.Value.Split(';');

            int[] esitMi = { yillar.Length, muhasebeKodlar.Length, harcamaBirimKodlar.Length, fisNolar.Length };

            for (int i = 0; i < esitMi.Length - 1; i++)
            {
                if (esitMi[i] != esitMi[i + 1])
                {
                    GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMTSI001);
                    return;
                }
            }

            if (txtIslem.Value == "Yazdir")
            {
                string tempFileName = System.IO.Path.GetTempFileName();
                string klasor = tempFileName + ".dir";
                System.IO.DirectoryInfo dri = System.IO.Directory.CreateDirectory(klasor);
                klasor += "\\";

                for (int i = 0; i < fisNolar.Length; i++)
                {
                    string tmpFile = System.IO.Path.GetTempFileName();
                    string excelYazYer = klasor + fisNolar[i] + "." + GenelIslemler.ExcelTur();

                    System.IO.File.Move(tmpFile, excelYazYer);
                    System.IO.File.Delete(tmpFile);

                    int yil = OrtakFonksiyonlar.ConvertToInt(yillar[i], 0);
                    string muhasebeKod = muhasebeKodlar[i].Split('-')[0];
                    string harcamaKod = harcamaBirimKodlar[i].Split('-')[0];
                    string fisNo = fisNolar[i].Trim();

                    string tifTur = string.Empty;
                    if (Request.QueryString["kutuphane"] == "1")
                        tifTur = "kutuphane";
                    else if (Request.QueryString["muze"] == "1")
                        tifTur = "muze";
                    TasinirIslemFormYazdir.Yazdir(kullanan, yil, fisNo, harcamaKod, muhasebeKod, excelYazYer, tifTur);
                }

                string[] dosyalar = { "" };
                string sonucDosyaAd = System.IO.Path.GetTempFileName();

                OrtakClass.Zip.Ziple(dri.FullName, sonucDosyaAd, dosyalar);
                dri.Delete(true);
                System.IO.File.Delete(tempFileName);
                OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, "Belgeler.zip", true, "zip");
            }
            else
            {
                Sonuc sonuc = new Sonuc();
                string bilgiStr = string.Empty;

                for (int i = 0; i < yillar.Length; i++)
                {
                    TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

                    tf.yil = OrtakFonksiyonlar.ConvertToInt(yillar[i], 0);
                    tf.muhasebeKod = muhasebeKodlar[i].Split('-')[0];
                    tf.harcamaKod = harcamaBirimKodlar[i].Split('-')[0];
                    tf.fisNo = fisNolar[i].Trim();

                    sonuc = servis.TasinirIslemFisiDurumDegistir(kullanan, tf, txtIslem.Value);

                    if (sonuc.hataStr.StartsWith("Belgenin onayýnýn kaldýrýlabilmesi için"))
                    {
                        bilgiStr += sonuc.hataStr;
                        sonuc.islemSonuc = true;
                    }

                    if (!sonuc.islemSonuc)
                    {
                        GenelIslemler.MesajKutusu(this, sonuc.hataStr);
                        return;
                    }
                    else
                        bilgiStr += sonuc.bilgiStr;
                }

                //ClientScript.RegisterStartupScript(this.GetType(), "mesaj", "alert('" + bilgiStr + "')", false);
                //bu mesaj sorgu ekranýndaki bir iframe içerisinde saklý kalýyor ve kullanýcý göremiyor.
                //bir üst framede görünecek þekilde düzenlenmeli
                GenelIslemler.MesajKutusu(this, bilgiStr);
            }
        }
    }
}