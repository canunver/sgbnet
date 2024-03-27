using System;
using System.Data;
using System.IO;
using OrtakClass;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Say�m tutana�� haz�rlan�rken malzeme say�m i�lemleri el terminali ile
    /// yap�lm��sa, terminaldeki bilgilerin aktar�m i�leminin yap�ld��� sayfa
    /// </summary>
    public partial class TerminalOku : istemciUzayi.GenelSayfa
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servis = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Sayfa ilk defa �a��r�l�yorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlan�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);
            GenelIslemlerIstemci.JSResourceEkle(Resources.TasinirMal.ResourceManager, this, "TerminalOku", "FRMJSC010");

            //Bu form login olmadan giren TasinirIstek.aspx gibi formlardan �a�r�ld���nda sorun olmas�n
            //kullanan = OturumBilgisiIslem.KullaniciBilgiOku(false);

            if (!IsPostBack)
            {
                if (Request.QueryString["tur"] != null)
                    hdnCagiran.Value = Request.QueryString["tur"];

                rdKolonAmbar.Checked = true;
            }

            if (hdnCagiran.Value == "sayim")
                divKolonSecim.Style["display"] = "block";
            else
                divKolonSecim.Style["display"] = "none";
        }

        /// <summary>
        /// Dosya Oku tu�una bas�l�nca �al��an olay metodu
        /// Se�ilen terminal dosyas�ndan say�m bilgilerini okuyup
        /// gvDosyadanGelenler GridView kontrol�ne doldurur.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnDosyaOku_Click(object sender, EventArgs e)
        {
            if (fuDosya.HasFile)
            {
                StreamReader sr = new StreamReader(fuDosya.FileContent);

                System.Collections.ArrayList alSatirlar = new System.Collections.ArrayList();
                System.Collections.ArrayList al = new System.Collections.ArrayList();

                DataTable dt = new DataTable();
                dt.Columns.Add("yer");
                dt.Columns.Add("hesapplankod");
                //dt.Columns.Add("sicilno");
                dt.Columns.Add("miktar");

                while (!sr.EndOfStream)
                {
                    alSatirlar.Add(sr.ReadLine());
                }
                alSatirlar.Sort();

                bool ekle = true;
                int miktar;
                string[] satirParca;
                string[] satirSorgula;
                for (int i = 0; i < alSatirlar.Count; i++)
                {
                    satirParca = SatirParcala(alSatirlar[i].ToString());

                    for (int j = 0; j < al.Count; j++)
                    {
                        satirSorgula = (string[])(al[j]);
                        miktar = Convert.ToInt32(satirSorgula[2]);
                        if (satirParca[3] == satirSorgula[3])
                        {
                            miktar++;
                            ekle = false;
                        }
                        satirSorgula[2] = miktar.ToString();
                    }

                    if (ekle)
                        al.Add(satirParca);                       
                    ekle = true;
                }

                foreach (string[] item in al)
                {
                    dt.Rows.Add(item[1], item[3], item[2]);
                }

                gvDosyadanGelenler.DataSource = dt;
                gvDosyadanGelenler.DataBind();
            }
        }

        /// <summary>
        /// Parametre olarak verilen dizgiyi par�alay�p say�m bilgileri dizisi d�nd�ren yordam
        /// </summary>
        /// <param name="satir">Terminal dosyas�ndan okunmu� bir sat�ra ait dizgi</param>
        /// <returns>Par�alanm�� say�m bilgileri d�nd�r�l�r.</returns>
        private string[] SatirParcala(string satir)
        {
            string[] satirParca = new string[4];

            //"255020101010107000174\t01\t1"

            int tabIlk = satir.IndexOf("\t");
            int tabSon = satir.LastIndexOf("\t");

            satirParca[0] = satir.Substring(0, tabIlk); //sicilno
            satirParca[1] = satir.Substring(tabIlk + 1, tabSon - tabIlk - 1);
            satirParca[2] = satir.Substring(tabSon + 1);
            satirParca[3] = satirParca[0].Substring(0, satirParca[0].Length - 8);

            return satirParca;
        }
    }
}