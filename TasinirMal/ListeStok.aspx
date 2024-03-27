<%@ Page Language="C#" CodeBehind="ListeStok.aspx.cs" Inherits="TasinirMal.ListeStok" EnableEventValidation="false" %>
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%= Resources.TasinirMal.FRMLST001 %></title>
    <meta http-equiv="Pragma" content="no-cache" />
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=3"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/ListeStok.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Defterler.js?v=1"></script>
    <script language="JavaScript" type="text/javascript">
        function VeriGoster(hesap) {
            document.getElementById('txtHesapPlaniKodu').value = hesap;
        }
    </script>
</head>
<body onunload="HesapPlaniKapat();">
    <form id="form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <div class="form">
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMLST008 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 160px;">
                <asp:TextBox ID="txtHesapPlaniKodu" runat="server" CssClass="veriAlan" Width="150px"></asp:TextBox></div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMLST009 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:HesapPlaniGoster();return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" /></div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblHesapPlanAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMLST010 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 150px;">
                <asp:TextBox ID="txtHesapPlaniAdi" runat="server" CssClass="veriAlan" MaxLength="40"
                    Width="150px"></asp:TextBox></div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMLST011 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 150px;">
                <asp:TextBox ID="txtMiktar" runat="server" CssClass="veriAlan" MaxLength="10" Width="100px"></asp:TextBox>
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <center>
        <div style="height: 3px">
        </div>
        <asp:Button ID="btnListele" runat="server" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMLST012 %>"
            Width="120" OnClick="btnListele_Click" />
        <input type="button" id="btnMiktarAktar" class="dugme" value="<%= Resources.TasinirMal.FRMLST013 %>"
            style="width: 120px;" onclick="javascript:Aktar('miktar');return false;" />
        <input type="button" id="btnHepsiniAktar" class="dugme" value="<%= Resources.TasinirMal.FRMLST014 %>"
            style="width: 120px; color: Red;" onclick="javascript:Aktar('hepsi');return false;" />
        <div style="height: 3px">
        </div>
    </center>
    <div style="overflow: auto; height: 340px; width: 100%">
        <asp:GridView ID="gvTuketimler" runat="server" CellPadding="4" ForeColor="#333333"
            GridLines="None" Width="96%" AutoGenerateColumns="true">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
    </div>
    <input type="hidden" id="hdnCagiran" runat="server" />
    <input type="hidden" id="hdnCagiranLabel" runat="server" />
    </form>
</body>
</html>
