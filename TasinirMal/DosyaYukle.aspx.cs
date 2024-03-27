using Ext1.Net;
using Newtonsoft.Json.Linq;
using OrtakClass;
using System;
using System.Collections.Generic;
using System.Data;
using TNS;
using TNS.KYM;
using TNS.TMM;
using TNS.UZY;
using System.IO;

namespace TasinirMal
{
    public partial class DosyaYukle : TMMSayfaV2
    {

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

            }
        }

        protected void fuDosya_Secildi(object sender, DirectEventArgs e)
        {
            string hata = "";
            try
            {
                string dosyaAdi = System.IO.Path.GetFileName(fuDosya.PostedFile.FileName);
                byte[] ekliDosyaIcerik = new byte[fuDosya.PostedFile.ContentLength];
                fuDosya.PostedFile.InputStream.Read(ekliDosyaIcerik, 0, fuDosya.PostedFile.ContentLength);

                string tmp = Path.GetTempFileName();
                File.WriteAllBytes(tmp, ekliDosyaIcerik);

                X.AddScript("parent.DosyaYukle('" + dosyaAdi + "','" + Path.GetFileName(tmp) + "'); parent.hidePopWin();");
            }
            catch (Exception ex)
            {
                hata = "Dosya kaydedilemedi:" + ex.Message;
            }
            fuDosya.Reset();

            if (hata != "")
                GenelIslemler.MesajKutusu("Hata", hata);
        }
    }
}