<%@ Page Language="C#" CodeBehind="KayittanDusmeGirisEski.aspx.cs" Inherits="TasinirMal.KayittanDusmeGirisEski" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<%@ Register TagPrefix="FarPoint" Namespace="FarPoint.Web.Spread" Assembly="FarPoint.Web.Spread" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <link href="../App_themes/TakvimYeni.css" type="text/css" rel="stylesheet" />
    <script language="javascript" type="text/javascript" src="../Script/TakvimYeni.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="../script/paraFormat.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=2"></script>
    <script language="javascript" type="text/javascript" src="ModulScripts/KayittanDusmeGiris.js?mc=03022015&v=2013_11"></script>
</head>
<body onunload="HesapPlaniKapat();">
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server">
        </ext:ResourceManager>
        <input type="hidden" id="hdnTutanakNo" runat="server" />
        <div class="form">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMKDG020 %>
                </div>
                <div class="ikiNokta">
                    :
                </div>
                <div class="veriAlan">
                    <asp:DropDownList ID="ddlYil" runat="server" CssClass="veriAlanDDL">
                    </asp:DropDownList>
                </div>
            </div>
            <div id="divMHA0" runat="server">
                <div class="row">
                    <div class="yaziAlanK">
                        <%= Resources.TasinirMal.FRMKDG021 %>
                    </div>
                    <div class="ikiNokta">
                        :
                    </div>
                    <div class="veriAlan">
                        <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox>
                    </div>
                    <div class="veriAlan">
                        <input type="image" alt="<%= Resources.TasinirMal.FRMKDG022 %>" style="height: 14px; width: 16px;"
                            class="veriAlan" onclick="javascript: ListeAc('ListeMuhasebe.aspx', 'txtMuhasebe', '', 'lblMuhasebeAd'); return false;"
                            tabindex="100" src="../App_themes/images/bul1.gif" />
                    </div>
                    <div class="kolonArasi">
                    </div>
                    <div class="veriAlan">
                        <asp:Label ID="lblMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                    </div>
                </div>
                <div class="row">
                    <div class="yaziAlanK">
                        <%= Resources.TasinirMal.FRMKDG023 %>
                    </div>
                    <div class="ikiNokta">
                        :
                    </div>
                    <div class="veriAlan">
                        <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                            Width="100px"></asp:TextBox>
                    </div>
                    <div class="veriAlan">
                        <input type="image" alt="<%= Resources.TasinirMal.FRMKDG024 %>" style="height: 14px; width: 16px;"
                            class="veriAlan" onclick="javascript: ListeAc('ListeHarcamaBirimi.aspx', 'txtHarcamaBirimi', 'txtMuhasebe', 'lblHarcamaBirimiAd'); return false;"
                            tabindex="100" src="../App_themes/images/bul1.gif" />
                    </div>
                    <div class="kolonArasi">
                    </div>
                    <div class="veriAlan">
                        <asp:Label ID="lblHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                    </div>
                </div>
                <div class="row">
                    <div class="yaziAlanK">
                        <%= Resources.TasinirMal.FRMKDG025 %>
                    </div>
                    <div class="ikiNokta">
                        :
                    </div>
                    <div class="veriAlan">
                        <asp:TextBox ID="txtAmbar" runat="server" CssClass="veriAlan" MaxLength="3" Width="100px"></asp:TextBox>
                    </div>
                    <div class="veriAlan">
                        <input type="image" alt="<%= Resources.TasinirMal.FRMKDG026 %>" style="height: 14px; width: 16px;"
                            class="veriAlan" onclick="javascript: ListeAc('ListeAmbar.aspx', 'txtAmbar', 'txtHarcamaBirimi', 'lblAmbarAd'); return false;"
                            tabindex="100" src="../App_themes/images/bul1.gif" />
                    </div>
                    <div class="kolonArasi">
                    </div>
                    <div class="veriAlan">
                        <asp:Label ID="lblAmbarAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMKDG027 %>
                </div>
                <div class="ikiNokta">
                    :
                </div>
                <div class="veriAlan">
                    <input type="text" runat="server" name="txtTutanakTarihi" class="veriAlan" id="txtTutanakTarihi"
                        maxlength="10" size="10" />
                    <img src="../App_themes/Images/takvim.gif" id="imgTutanakTarihi" alt="" style="cursor: pointer;"
                        onclick="JavaScript:displayDatePicker('txtTutanakTarihi', false, 'dmy', '.');" />
                </div>
            </div>
            <div class="footer">
                &nbsp;
            </div>
        </div>
        <table width="100%" cellpadding="2" cellspacing="2">
            <tr>
                <td align="center">
                    <asp:Button runat="server" ID="btnKaydet" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMKDG029 %>"
                        Width="80" OnClick="btnKaydet_Click" />&nbsp;
                <asp:Button runat="server" ID="btnSil" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMKDG030 %>"
                    Width="80" Enabled="false" OnClick="btnSil_Click" />
                    <asp:Button runat="server" ID="btnYazdir" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMKDG031 %>"
                        Width="80" Enabled="false" OnClick="btnYazdir_Click" />
                    <asp:Button runat="server" ID="btnTIF" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMKDG032 %>"
                        Width="80" Enabled="false" OnClick="btnTIF_Click" />
                    <asp:Button runat="server" ID="btnTemizle" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMKDG033 %>"
                        Width="80" OnClick="btnTemizle_Click" />
                    <asp:Button runat="server" ID="btnListele" CssClass="dugme" Text="Listele"
                        Width="80" OnClick="btnListele_Click" />
                </td>
            </tr>
        </table>
        <FarPoint:FpSpread ID="fpL" runat="server" Width="100%" Height="350" OnSaveOrLoadSheetState="fpL_SaveOrLoadSheetState"
            Style="position: relative;">
        </FarPoint:FpSpread>
        <div id="divBekle" class="bekleKutusu">
            <br />
            <b>
                <%= Resources.TasinirMal.FRMKDG034 %></b>
            <br />
            <br />
            <img alt="<%= Resources.TasinirMal.FRMKDG035 %>" src="../App_themes/images/loading.gif" />
            <br />
            <br />
        </div>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
