using System;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Zimmet fi�lerinin onaylama, onay kald�rma, iptal etme ve raporlama i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class ZimmetFormSorguIslem : istemciUzayi.GenelSayfa
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servis = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur, yoksa giri� ekran�na y�nlendirilir
        ///     varsa sayfa y�klenir ve istenen i�lem yap�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (txtIslem.Value == String.Empty)
                return;

            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            Islem();
        }

        /// <summary>
        /// Belge durum de�i�tirme ve belge yazd�rma i�lemlerini yapan yordam
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
                    GenelIslemler.MesajKutusuV3("Uyar�", Resources.TasinirMal.FRMZSI001);
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