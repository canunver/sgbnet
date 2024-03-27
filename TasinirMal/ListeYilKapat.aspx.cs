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
    /// ��leme a��k/kapal� ambarlar�n listelendi�i sayfa
    /// </summary>
    public partial class ListeYilKapat : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur, yoksa giri� ekran�na y�nlendirilir varsa sayfa y�klenir.
        ///     Sayfa adresinde yil girdi dizgisi yoksa hata mesaj� verilir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                //Sayfaya giri� izni varm�?
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
        /// Listele tu�una bas�l�nca �al��an olay metodu
        /// Ekrandaki se�ime g�re i�leme a��k veya kapal� ambarlar� listeler.
        /// Bu i�lem yap�l�rken ambarlar�n i�leme kapal� olup olmad��� bilgisi kullan�c�n�n de�i�kenlerinden okunur.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
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