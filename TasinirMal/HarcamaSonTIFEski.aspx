<%@ Page Language="C#" CodeBehind="HarcamaSonTIFEski.aspx.cs" Inherits="TasinirMal.HarcamaSonTIFEski" %>
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <link href="../App_themes/TakvimYeni.css" type="text/css" rel="stylesheet" />
    <script language="javascript" type="text/javascript" src="../Script/TakvimYeni.js?mc=03022015"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <div class="form">
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMHST002 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 100px">
                <asp:DropDownList ID="ddlYil" CssClass="veriAlanDDL" runat="server">
                </asp:DropDownList>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMHST004 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <input type="text" runat="server" name="txtTarihIlk" class="veriAlan" id="txtTarihIlk"
                    maxlength="10" size="10" />
                <img src="../App_themes/Images/takvim.gif" id="imgTarihIlk" alt="" style="cursor: pointer;"
                    onclick="JavaScript:displayDatePicker('txtTarihIlk', false, 'dmy', '.');" />
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMHST005 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <input type="text" runat="server" name="txtTarihSon" class="veriAlan" id="txtTarihSon"
                    maxlength="10" size="10" />
                <img src="../App_themes/Images/takvim.gif" id="imgTarihSon" alt="" style="cursor: pointer;"
                    onclick="JavaScript:displayDatePicker('txtTarihSon', false, 'dmy', '.');" />
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <asp:Button ID="btnYazdir" TabIndex="99" runat="server" Width="100" CssClass="dugme"
        Text="<%$ Resources:TasinirMal, FRMHST003 %>" OnClick="btnYazdir_Click"></asp:Button>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
