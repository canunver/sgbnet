using System;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Ta��n�r i�lem fi�lerinin onaylama, onay kald�rma, iptal etme ve raporlama i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class TasinirIslemSorguIslem : istemciUzayi.GenelSayfa
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

                    if (sonuc.hataStr.StartsWith("Belgenin onay�n�n kald�r�labilmesi i�in"))
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
                //bu mesaj sorgu ekran�ndaki bir iframe i�erisinde sakl� kal�yor ve kullan�c� g�remiyor.
                //bir �st framede g�r�necek �ekilde d�zenlenmeli
                GenelIslemler.MesajKutusu(this, bilgiStr);
            }
        }
    }
}