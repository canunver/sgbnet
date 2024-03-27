using System;
using OrtakClass;
using TNS.KYM;
using System.Collections.Generic;
using TNS;
using TNS.TMM;
using Ext1.Net;
using System.Collections;

namespace TasinirMal
{
    public partial class UretimRecetesiSorgu : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                kullanan = OturumBilgisiIslem.KullaniciBilgiOku(false);
                SayfaUstAltBolumYaz(this);
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);
                Listele();
            }
        }

        protected void btnListele_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void Listele()
        {
            UretimRecetesi filtre = new UretimRecetesi();

            ObjectArray bilgi = servisTMM.UretimRecetesiListele(kullanan, filtre);

            List<object> liste = new List<object>();
            Hashtable htListe = new Hashtable();
            foreach (UretimRecetesi item in bilgi.objeler)
            {
                if (TasinirGenel.HashtableDegerVerDbl(htListe, item.anaUrun.hesapKod) > 0) continue;

                liste.Add(new
                {
                    ANAURUNADI = item.anaUrun.aciklama,
                    ANAURUNKOD = item.anaUrun.hesapKod
                });

                htListe[item.anaUrun.hesapKod] = 1;

            }
            strListe.DataSource = liste;
            strListe.DataBind();

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);
                return;
            }
        }


    }
}