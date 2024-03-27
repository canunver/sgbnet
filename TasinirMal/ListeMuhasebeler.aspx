<%@ Page Language="C#" CodeBehind="ListeMuhasebeler.aspx.cs" Inherits="TasinirMal.ListeMuhasebeler" %>
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="Pragma" content="no-cache" />
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="../script/add_remove.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="../script/comboSelect.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=1"></script>
    <script language="javascript" type="text/javascript">
        function SozlukAc(sayfa) {
            PopupWin(sayfa + "?menuYok=1", 780, 540, 'sozluk');
        }
    </script>
</head>
<body>
    <script language="Javascript" type="text/javascript">        window.focus();</script>
    <form id="aspnetForm" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <input type="hidden" id="hdngirisListeAd" runat="server" />
    <table cellspacing="2" cellpadding="2" border="0" class="form">
        <tr>
            <td>
                <b>
                    <%= Resources.TasinirMal.FRMLML003 %></b>&nbsp;<asp:TextBox ID="txtArama" runat="server"
                        Width="150" CssClass="veriAlanTD"></asp:TextBox>&nbsp;<asp:Button ID="btnAra" Width="30"
                            Text="<%$ Resources:TasinirMal, FRMLML004 %>" runat="server" CssClass="dugme"
                            OnClick="btnAra_Click" />
            </td>
            <td>
            </td>
            <td>
                <asp:Label ID="lblYeniGiris" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <b>
                    <%= Resources.TasinirMal.FRMLML005 %></b>
            </td>
            <td>
            </td>
            <td>
                <b>
                    <%= Resources.TasinirMal.FRMLML006 %></b>
            </td>
        </tr>
        <tr>
            <td width="49%">
                <select id="lstSrc" name="lstSrc" multiple="true" runat="server" style="width: 100%;
                    height: 150px" ondblclick="listeye_ekle('lstSrc','lstDest',document.getElementById('txtSecimSayisi').value)"
                    onkeypress="return shiftHighlight(event.keyCode, this);">
                </select>
            </td>
            <td align="center" width="2%">
                <p>
                    <a href="Javascript:listeye_ekle('lstSrc','lstDest',document.getElementById('txtSecimSayisi').value);">
                        <img alt="<%= Resources.TasinirMal.FRMLML007 %>" src="../App_themes/images/scroll_right.gif"
                            border="0" /></a>
                    <br />
                    <br />
                    <a href="Javascript:listeden_cikart('lstSrc','lstDest');">
                        <img alt="<%= Resources.TasinirMal.FRMLML008 %>" src="../App_themes/images/scroll_left.gif"
                            border="0" /></a>
                </p>
            </td>
            <td width="49%">
                <select id="lstDest" name="lstDest" enableviewstate="false" multiple="true" runat="server"
                    style="width: 100%; height: 150px" ondblclick="listeden_cikart('lstSrc','lstDest')"
                    onkeypress="return shiftHighlight(event.keyCode, this);">
                </select>
            </td>
        </tr>
    </table>
    <div class="row">
        <table width="100%" cellspacing="2" cellpadding="2" border="0">
            <tr>
                <td align="center">
                    <asp:Button ID="btnTamam" runat="server" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMLML009 %>"
                        Width="100px"></asp:Button>&nbsp;
                    <input type="button" id="btnKapat" class="dugme" value="<%= Resources.TasinirMal.FRMLML010 %>"
                        style="width: 100px" onclick="javascript:self.close();" />
                </td>
            </tr>
        </table>
    </div>
    <div class="bilgiKutusu">
        <%= Resources.TasinirMal.FRMLML011 %></div>
    <input type="hidden" id="txtSecimSayisi" runat="server" name="txtSecimSayisi" />
    </form>
</body>
</html>
