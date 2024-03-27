<%@ Page Language="C#" CodeBehind="TanimKapiGecisSms.aspx.cs" Inherits="TasinirMal.TanimKapiGecisSms" %>
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=3"></script>
    <script language="javascript" type="text/javascript">
        function VeriGoster(kod, ad, cep, eposta, cepKullanim, epostaKullanim) {
            document.getElementById('txtTCKimlikNo').value = kod;
            document.getElementById('lblKisiAd').innerHTML = ad;
            document.getElementById('txtCepTel').value = cep;
            document.getElementById('txtEposta').value = eposta;

            if (cepKullanim == "True")
                document.getElementById('chkKullanim_0').checked = true;
            else
                document.getElementById('chkKullanim_0').checked = false;

            if (epostaKullanim == "True")
                document.getElementById('chkKullanim_1').checked = true;
            else
                document.getElementById('chkKullanim_1').checked = false;
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
    <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <div class="form">
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTKI006 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtTCKimlikNo" runat="server" CssClass="veriAlan" MaxLength="11"></asp:TextBox></div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMTKI007 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:AdresAc('ListePersonel.aspx?cagiran=txtTCKimlikNo&cagiranLabel=lblKisiAd');return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" /></div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblKisiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTKI008 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtCepTel" runat="server" CssClass="veriAlan" MaxLength="13"></asp:TextBox></div>
            <div class="ikiNokta">
                &nbsp;<%= Resources.TasinirMal.FRMTKI009 %></div>
        </div>
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTKI010 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtEposta" runat="server" CssClass="veriAlan"></asp:TextBox></div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTKI011 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="height: 50px">
                <asp:CheckBoxList ID="chkKullanim" runat="server" CssClass="veriAlan">
                    <asp:ListItem Text="<%$ Resources:TasinirMal, FRMTKI012 %>" Value="1"></asp:ListItem>
                    <asp:ListItem Text="<%$ Resources:TasinirMal, FRMTKI013 %>" Value="2"></asp:ListItem>
                </asp:CheckBoxList>
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <center>
        <asp:Button runat="server" ID="btnKaydet" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMTKI014 %>"
            OnClick="btnKaydet_Click" Width="70" />
        <asp:Button runat="server" ID="btnSil" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMTKI015 %>"
            OnClick="btnSil_Click" Width="70" />
        <asp:Button runat="server" ID="btnAra" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMTKI016 %>"
            OnClick="btnAra_Click" Width="70" />
        <input type="button" id="btnTemizle" class="dugme" style="width: 70px;" value="<%= Resources.TasinirMal.FRMTKI017 %>"
            onclick="VeriGoster('','','','','','');" />
    </center>
    <asp:DataGrid ID="dgListe" runat="server" AllowCustomPaging="false" AllowSorting="false"
        AutoGenerateColumns="False" BorderWidth="0" CellPadding="3" Font-Bold="False"
        Font-Italic="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="8pt"
        Font-Strikeout="False" Font-Underline="False" ForeColor="#333333" PageSize="1"
        Width="60%">
        <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
        <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Left" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" Font-Italic="False" Font-Names="Tahoma"
            Font-Overline="False" Font-Size="8pt" Font-Strikeout="False" Font-Underline="False"
            ForeColor="White" HorizontalAlign="Left" VerticalAlign="Middle" />
        <Columns>
            <asp:BoundColumn DataField="kod" HeaderText="<%$ Resources:TasinirMal, FRMTKI006 %>" />
            <asp:BoundColumn DataField="ad" HeaderText="<%$ Resources:TasinirMal, FRMTKI018 %>" />
            <asp:BoundColumn DataField="cepTel" HeaderText="<%$ Resources:TasinirMal, FRMTKI012 %>" />
            <asp:BoundColumn DataField="eposta" HeaderText="<%$ Resources:TasinirMal, FRMTKI013 %>" />
            <asp:BoundColumn DataField="cepTelKullanim" HeaderText="<%$ Resources:TasinirMal, FRMTKI019 %>" />
            <asp:BoundColumn DataField="epostaKullanim" HeaderText="<%$ Resources:TasinirMal, FRMTKI020 %>" />
        </Columns>
    </asp:DataGrid>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
