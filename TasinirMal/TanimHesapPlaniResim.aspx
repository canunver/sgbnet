<%@ Page Language="C#" Inherits="TasinirMal.TanimHesapPlaniResim" CodeBehind="TanimHesapPlaniResim.aspx.cs" %>
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Defterler.js?mc=03022015&v=2012_01_05"></script>
    <script language="javascript" type="text/javascript">
        function VeriGoster(hesap, aciklama, resim) {
            document.getElementById('txtHesapPlanKod').value = hesap;
            document.getElementById('lblHesapPlanAd').innerHTML = aciklama;
            document.getElementById('hdnDosya').value = resim;
            document.getElementById('lblResim').innerHTML = resim;
        }
    </script>
</head>
<body onunload="HesapPlaniKapat();">
    <form id="form" runat="server">
    <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <input type="hidden" id="hdnDosya" runat="server" />
    <div class="form">
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTHR003 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtHesapPlanKod" runat="server" CssClass="veriAlan" MaxLength="40"
                    Width="150px"></asp:TextBox></div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMTHR004 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:HesapPlaniGoster();return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" /></div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblHesapPlanAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTHR005 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:FileUpload ID="fupDosya" runat="server" Width="60%" /></div>
        </div>
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTHR005 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:Label ID="lblResim" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <asp:Button runat="server" ID="btnKaydet" OnClick="btnKaydet_Click" CssClass="dugme"
        Text="<%$ Resources:TasinirMal, FRMTHR006 %>" Width="70" />
    <asp:Button runat="server" ID="btnListele" OnClick="btnListele_Click" CssClass="dugme"
        Text="<%$ Resources:TasinirMal, FRMTHR007 %>" Width="70" />
    <input type="button" id="btnTemizle" class="dugme" style="width: 70px;" value="<%= Resources.TasinirMal.FRMTHR008 %>"
        onclick="VeriGoster('','','');" />
    <asp:DataGrid ID="dgListe" runat="server" AllowCustomPaging="false" AllowSorting="false"
        AutoGenerateColumns="False" BorderWidth="0" CellPadding="3" Font-Bold="False"
        Font-Italic="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="8pt"
        Font-Strikeout="False" Font-Underline="False" ForeColor="#333333" PageSize="1"
        Width="70%">
        <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
        <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Left" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" Font-Italic="False" Font-Names="Tahoma"
            Font-Overline="False" Font-Size="8pt" Font-Strikeout="False" Font-Underline="False"
            ForeColor="White" HorizontalAlign="Left" VerticalAlign="Middle" />
        <Columns>
            <asp:BoundColumn DataField="hesapKod" HeaderText="<%$ Resources:TasinirMal, FRMTHR003 %>" />
            <asp:BoundColumn DataField="hesapAd" HeaderText="<%$ Resources:TasinirMal, FRMTHR009 %>" />
            <asp:BoundColumn DataField="resim" HeaderText="<%$ Resources:TasinirMal, FRMTHR010 %>" />
        </Columns>
    </asp:DataGrid>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
