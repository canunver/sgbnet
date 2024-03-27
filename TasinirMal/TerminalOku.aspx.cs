using System;
using System.Data;
using System.IO;
using OrtakClass;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Sayým tutanaðý hazýrlanýrken malzeme sayým iþlemleri el terminali ile
    /// yapýlmýþsa, terminaldeki bilgilerin aktarým iþleminin yapýldýðý sayfa
    /// </summary>
    public partial class TerminalOku : istemciUzayi.GenelSayfa
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servis = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Sayfa ilk defa çaðýrýlýyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);
            GenelIslemlerIstemci.JSResourceEkle(Resources.TasinirMal.ResourceManager, this, "TerminalOku", "FRMJSC010");

            //Bu form login olmadan giren TasinirIstek.aspx gibi formlardan çaðrýldýðýnda sorun olmasýn
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
        /// Dosya Oku tuþuna basýlýnca çalýþan olay metodu
        /// Seçilen terminal dosyasýndan sayým bilgilerini okuyup
        /// gvDosyadanGelenler GridView kontrolüne doldurur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
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
        /// Parametre olarak verilen dizgiyi parçalayýp sayým bilgileri dizisi döndüren yordam
        /// </summary>
        /// <param name="satir">Terminal dosyasýndan okunmuþ bir satýra ait dizgi</param>
        /// <returns>Parçalanmýþ sayým bilgileri döndürülür.</returns>
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