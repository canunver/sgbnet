using System;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Zimmet fiþlerinin onaylama, onay kaldýrma, iptal etme ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class ZimmetFormSorguIslem : istemciUzayi.GenelSayfa
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
                    GenelIslemler.MesajKutusuV3("Uyarý", Resources.TasinirMal.FRMZSI001);
                    return;
                }
            }

            int belgeTur = OrtakFonksiyonlar.ConvertToInt(txtBelgeTur.Value, 0);
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

                    ZimmetFormYazdir.Yazdir(kullanan, yil, fisNo, harcamaKod, muhasebeKod, belgeTur, excelYazYer, true);
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
                    TNS.TMM.ZimmetForm zf = new TNS.TMM.ZimmetForm();

                    zf.yil = OrtakFonksiyonlar.ConvertToInt(yillar[i], 0);
                    zf.muhasebeKod = muhasebeKodlar[i].Split('-')[0];
                    zf.harcamaBirimKod = harcamaBirimKodlar[i].Split('-')[0];
                    zf.fisNo = fisNolar[i].Trim();
                    zf.belgeTur = belgeTur;

                    sonuc = servis.ZimmetFisiDurumDegistir(kullanan, zf, txtIslem.Value);

                    if (!sonuc.islemSonuc)
                    {
                        GenelIslemler.MesajKutusuV3("Hata", sonuc.hataStr);
                        return;
                    }
                    else
                        bilgiStr += sonuc.bilgiStr;
                }

                GenelIslemler.MesajKutusuV3("Bilgi", bilgiStr);
            }
        }
    }
}