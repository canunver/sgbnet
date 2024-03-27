using System;
using System.Data;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections.Generic;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// Ýþleme açýk/kapalý ambarlarýn listelendiði sayfa
    /// </summary>
    public partial class ListeYilKapat : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur, yoksa giriþ ekranýna yönlendirilir varsa sayfa yüklenir.
        ///     Sayfa adresinde yil girdi dizgisi yoksa hata mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                TNS.TMM.TasinirIslemForm kriter = new TNS.TMM.TasinirIslemForm();
                int yil = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"] + "", 0);

                if (yil <= 0)
                {
                    GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLYK002);
                    X.AddScript("parent.hidePopWin();");
                    return;
                }
            }
        }

        /// <summary>
        /// Listele tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandaki seçime göre iþleme açýk veya kapalý ambarlarý listeler.
        /// Bu iþlem yapýlýrken ambarlarýn iþleme kapalý olup olmadýðý bilgisi kullanýcýnýn deðiþkenlerinden okunur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListe_Click(object sender, EventArgs e)
        {
            int yil = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"] + "", 0);

            bool acik = true;
            string degisken = "";
            Ambar ambar = new Ambar();
            ObjectArray bilgiler = servisTMM.AmbarListele(kullanan, ambar);

            List<object> liste = new List<object>();
            foreach (Ambar a in bilgiler.objeler)
            {
                acik = true;
                degisken = "KAPAT" + "_" + yil + "_" + a.muhasebeKod + "_" + a.harcamaBirimKod.Replace(".", "") + "_" + a.kod;
                TNS.TNSCollection kDeg = GenelIslemler.KullaniciDegiskenleriGetir("stratek", degisken);

                if (kDeg != null && kDeg.Count > 0)
                    if (kDeg[0].ToString() == "1")
                        acik = false;

                if ((acik && rdAcik.Checked) || (!acik && rdKapali.Checked))
                {
                    liste.Add(new
                    {
                        MUHASEBE = a.muhasebeKod + "-" + a.muhasebeAd,
                        HARCAMABIRIMI = a.harcamaBirimKod + "-" + a.harcamaBirimAd,
                        AMBAR = a.kod + "-" + a.ad
                    });
                }
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }
    }
}