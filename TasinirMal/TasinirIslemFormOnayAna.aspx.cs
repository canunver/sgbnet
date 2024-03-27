using OrtakClass;
using System;

namespace TasinirMal
{
    public partial class TasinirIslemFormOnayAna : TMMSayfaV2
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            formAdi = "B/A Onayý";
            SayfaUstAltBolumYaz(this);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

            //Sayfaya giriþ izni varmý?
            //GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            for (int i = 0; i < kullanan.kullaniciTipi.Length; i++)
            {
                if (string.IsNullOrEmpty(kullanan.yetkiTipiDegerleri["TasinirIslemOnay"]) && kullanan.kullaniciTipi[i] == TNS.KYM.ENUMKullaniciTipi.TASINIRMALKAYDI)
                {
                    panelIslem.Hidden = true;
                }
            }

            panelIslem.AutoLoad.Url = "TasinirIslemFormOnay.aspx?menuYok=1";
            panelSorgu.AutoLoad.Url = "TasinirIslemFormOnaySorguMB.aspx?menuYok=1";
        }
    }
}