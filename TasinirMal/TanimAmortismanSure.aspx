<%@ Page Language="C#" CodeBehind="TanimAmortismanSure.aspx.cs" Inherits="TasinirMal.TanimAmortismanSure" %>
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=3"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Defterler.js?v=1"></script>
    <script language="javascript" type="text/javascript">
        function VeriGoster(yil, hesapKod, hesapAd, sure) {
            document.getElementById('ddlYil').value = yil;
            document.getElementById('txtHesapPlanKod').value = hesapKod;
            document.getElementById('lblHesapPlanAd').innerHTML = hesapAd;
            document.getElementById('txtSure').value = sure;
        }
    </script>
</head>
<body onunload="HesapPlaniKapat();">
    <form id="form" runat="server">
    <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <div class="form">
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTTS004 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:DropDownList ID="ddlYil" runat="server" CssClass="veriAlanDDL">
                </asp:DropDownList>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTTS005 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtHesapPlanKod" runat="server" CssClass="veriAlan" MaxLength="9"
                    Columns="9"></asp:TextBox></div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMTTS006 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:HesapPlaniGoster();return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" /></div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblHesapPlanAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTTS007 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtSure" runat="server" CssClass="veriAlan" Columns="4"></asp:TextBox></div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <center>
        <asp:Button runat="server" ID="btnKaydet" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMTTS008 %>"
            OnClick="btnKaydet_Click" Width="70" />
        <asp:Button runat="server" ID="btnSil" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMTTS009 %>"
            OnClick="btnSil_Click" Width="70" />
        <asp:Button runat="server" ID="btnListele" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMTTS010 %>"
            OnClick="btnListele_Click" Width="70" />
    </center>
    <div style="overflow: auto; height: 300px; width: 100%">
        <asp:GridView ID="dgListe" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
            Width="98%" AutoGenerateColumns="false">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:BoundField DataField="yil" HeaderText="<%$ Resources:TasinirMal, FRMTTS004 %>">
                </asp:BoundField>
                <asp:BoundField DataField="hesapKod" HeaderText="<%$ Resources:TasinirMal, FRMTTS005 %>">
                </asp:BoundField>
                <asp:BoundField DataField="hesapAd" HeaderText="<%$ Resources:TasinirMal, FRMTTS011 %>"
                    HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left"></asp:BoundField>
                <asp:BoundField DataField="sure" HeaderText="<%$ Resources:TasinirMal, FRMTTS007 %>">
                </asp:BoundField>
            </Columns>
        </asp:GridView>
    </div>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
