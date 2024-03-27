using Ext1.Net;
using System;
using System.Collections.Generic;
using TNS;
using TNS.TMM;
using TNS.UZY;

namespace TasinirMal
{
    public partial class TanimDemirbasResimGoster : istemciUzayi.GenelSayfa
    {
        IUZYServis servisUzy = TNS.UZY.Arac.Tanimla();
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ResimGetirGoster();
            }
        }

        void ResimGetirGoster()
        {
            string resimID = Request.QueryString["resimID"].ToString();
            List<object> tipler = new List<object>();
            ObjectArray liste = servisTMM.TasinirResimGetir(string.Empty, resimID);

            if (liste.sonuc.islemSonuc)
            {
                foreach (TasResim resim in liste.objeler)
                {
                    imgResim.ImageUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])resim.resim);
                }
            }
        }

        protected void btnSagDondur_Click(object sender, DirectEventArgs e)
        {
            ResimDondur(System.Drawing.RotateFlipType.Rotate90FlipNone);
        }

        void ResimDondur(System.Drawing.RotateFlipType yon)
        {
            string resimID = Request.QueryString["resimID"].ToString();
            List<object> tipler = new List<object>();
            ObjectArray liste = servisTMM.TasinirResimGetir(string.Empty, resimID);

            if (liste.sonuc.islemSonuc)
            {
                TasResim r = new TasResim();
                foreach (TasResim resim in liste.objeler)
                {
                    r = resim;
                    break;
                }

                TasResim r2 = TanimDemirbasResim.ResimBoyutlandir(r.resim, yon, 1280, 1280);
                r2.prSicilNo = r.prSicilNo;
                r2.resimID = r.resimID;
                r2.siraNo = r.siraNo;
                servisTMM.TasinirResimKaydet(r2);
                ResimGetirGoster();
            }
        }

    }
}