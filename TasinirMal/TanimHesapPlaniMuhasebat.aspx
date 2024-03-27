<%@ Page Language="C#" Inherits="TasinirMal.HesapPlaniMuhasebat" CodeBehind="TanimHesapPlaniMuhasebat.aspx.cs" %>
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form" runat="server">
    <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <div class="form">
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTHM009 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:DropDownList ID="ddlSablon" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSablon_SelectedIndexChanged">
                    <asp:ListItem Text="<%$ Resources:TasinirMal, FRMTHM010 %>" Value="1"></asp:ListItem>
                    <asp:ListItem Text="<%$ Resources:TasinirMal, FRMTHM011 %>" Value="2"></asp:ListItem>
                    <asp:ListItem Text="<%$ Resources:TasinirMal, FRMTHM012 %>" Value="3"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTHM013 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtBaslaSatir" runat="server" Columns="5" CssClass="veriAlan"></asp:TextBox></div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTHM014 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtHesapKodKolon" runat="server" Columns="5" CssClass="veriAlan"></asp:TextBox></div>
            <div class="kolonArasi">
            </div>
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTHM015 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtHesapKodKolonSay" runat="server" Columns="5" CssClass="veriAlan"></asp:TextBox></div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTHM016 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtAciklamaKolon" runat="server" Columns="5" CssClass="veriAlan"></asp:TextBox></div>
            <div class="kolonArasi">
            </div>
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTHM017 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtOlcuBirimiKolon" runat="server" Columns="5" CssClass="veriAlan"></asp:TextBox></div>
            <div class="kolonArasi">
            </div>
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTHM018 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtKDVKolon" runat="server" Columns="5" CssClass="veriAlan"></asp:TextBox></div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTHM019 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtKullanilmiyor" runat="server" Columns="5" CssClass="veriAlan"></asp:TextBox></div>
            <div class="kolonArasi">
            </div>
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTHM020 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtGuncelleme" runat="server" Columns="5" CssClass="veriAlan"></asp:TextBox></div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTHM021 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <input class="dugme" id="fileListe" type="file" size="80" name="fileListe" runat="server" /></div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <center>
        <asp:Button runat="server" ID="btnKaydet" OnClick="btnKaydet_Click" CssClass="dugme"
            Text="<%$ Resources:TasinirMal, FRMTHM022 %>" Width="200" />
    </center>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
