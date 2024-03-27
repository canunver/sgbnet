using System;
using System.Net;

namespace TasinirMal
{
    public partial class TanimDemirbasRFIDBarkodYazdir : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string yaziciURL = Request.QueryString["yaziciURL"] + "";
            string dataRequest = Request.QueryString["data"] + "";

            //yaziciURL = "rfidprinter.manas.edu.kg:7890";
            //dataRequest = "eyAiam9iTmFtZSI6ICIxIG1hbHplbWUgacOnaW4gZXRpa2V0IHlhemTEsXLEsWzEsXlvci4uLiIsICJ0ZW1wbGF0ZSI6ICIiLCAicGFnZUNvdW50IjogMSwgImNvbW1vbiI6IHt9LCAicGFnZXMiOlt7ICJyZmlkIjogIjEzOTkwMDAwMDAwMDAwMDAwMDA1MDkyNCIsICJ1c2FnZVR5cGUiOiIiLCAiZXBjIjoiMTM5OTAwMDAwMDAwMDAwMDAwMDUwOTI0IiwgInNlcmlhbE51bWJlciI6IiIsICJkZXNjcmlwdGlvbiI6IkJldG9uaXllciIsICJicmFuZCI6IiIsICJraWxsIjoiNzc3Nzc3NzciLCAiYWNjZXNzIjoiNzc3Nzc3NzciLCAidW5pdCI6IiIsICJiYXJjb2RlIjogIjI1MzAyMDIwMTEwMDExNjAwMDAwMSIgfV19";

            if (string.IsNullOrWhiteSpace(yaziciURL)) return;
            if (string.IsNullOrWhiteSpace(dataRequest)) return;

            string resourceAddress = "http://" + yaziciURL + "/" + dataRequest;
            WebRequest request = WebRequest.Create(resourceAddress);
            request.Method = "GET";
            request.ContentType = "application/json";
            var response = (HttpWebResponse)request.GetResponse();



        }
    }
}
