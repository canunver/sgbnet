<%@ Page Language="C#" CodeBehind="TasinirIstekGirisEski.aspx.cs" Inherits="TasinirMal.TasinirIstekGirisEski" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<%@ Register TagPrefix="FarPoint" Namespace="FarPoint.Web.Spread" Assembly="FarPoint.Web.Spread" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <link href="../App_themes/TakvimYeni.css" type="text/css" rel="stylesheet" />
    <script language="javascript" type="text/javascript" src="../Script/TakvimYeni.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="../script/paraFormat.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=2"></script>
    <script language="javascript" type="text/javascript" src="ModulScripts/TasinirIstekGiris.js?mc=03022015&v=2013"></script>
</head>
<body onunload="HesapPlaniKapat();">
    <form id="form" runat="server">
    <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
    <ext:ResourceManager ID="ResourceManager1" runat="server">
        <Listeners>
            <DocumentReady Handler="ddlBirim.onTriggerClick();Panel1.hide();" />
        </Listeners>
    </ext:ResourceManager>
    <input type="hidden" id="hdnBelgeNo" runat="server" />
    <table width="30%" cellpadding="2" cellspacing="2">
        <tr>
            <td id="kolon1" runat="server">
                <font size="1"><a href="#" runat="server" id="link1">
                    <img src="..\app_themes\images\homekutuphane.gif" alt="<%= Resources.TasinirMal.FRMIGF023 %>" />&nbsp;<%= Resources.TasinirMal.FRMIGF023 %></a></font>
            </td>
            <td id="kolon2" runat="server">
                <font size="1"><a runat="server" id="link2" onclick="javascript:void(0);">
                    <img src="..\app_themes\images\favoriKutuphane.gif" alt="<%= Resources.TasinirMal.FRMIGF024 %>" />&nbsp;<%= Resources.TasinirMal.FRMIGF024 %></a></font>
            </td>
        </tr>
    </table>
    <div class="form">
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMIGF025 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:DropDownList ID="ddlYil" runat="server" CssClass="veriAlanDDL">
                </asp:DropDownList>
            </div>
        </div>
        <div id="divMHA0" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMIGF026 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMIGF027 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtMuhasebe','','lblMuhasebeAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMIGF028 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMIGF029 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimi','txtMuhasebe','lblHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMIGF030 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtAmbar" runat="server" CssClass="veriAlan" MaxLength="3" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMIGF031 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeAmbar.aspx','txtAmbar','txtHarcamaBirimi','lblAmbarAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblAmbarAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
        </div>
        <div class="row" id="divOrg0" runat="server" style="display: none">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMORG001 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <ext:DropDownField ID="ddlBirim" runat="server" Width="300" TriggerIcon="Ellipsis"
                    Mode="ValueText">
                    <Component>
                        <ext:Panel ID="Panel1" runat="server" Width="400">
                            <Items>
                                <ext:TreePanel ID="TreePanel1" runat="server" Height="250" AutoWidth="true" AutoScroll="true"
                                    Border="false" RootVisible="false">
                                    <Root>
                                        <ext:AsyncTreeNode NodeID="0" Expanded="true" Icon="Note" />
                                    </Root>
                                    <Listeners>
                                        <BeforeLoad Fn="OrgBirimDoldur" />
                                        <Click Handler="#{ddlBirim}.setValue(node.id, node.text, false);#{ddlBirim}.collapse();OrgBirimBul(node, 'MuhasebeHarcamaAmbarBul(\'txtMuhasebe\', \'txtHarcamaBirimi\', \'txtAmbar\');');" />
                                    </Listeners>
                                </ext:TreePanel>
                            </Items>
                            <Listeners>
                                <Show Handler="try { TreePanel1.getRootNode().findChild('id', ddlBirim.getValue(), true).select(); } catch (e) { }" />
                            </Listeners>
                        </ext:Panel>
                    </Component>
                </ext:DropDownField>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMIGF032 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtPersonel" runat="server" CssClass="veriAlan" MaxLength="11" Width="100px"></asp:TextBox></div>
            <div class="veriAlan" id="divPersonelSecim" runat="server">
                <input type="image" alt="<%= Resources.TasinirMal.FRMIGF033 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListePersonel.aspx','txtPersonel','txtHarcamaBirimi','lblPersonelAd');return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" /></div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblPersonelAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMIGF034 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <input type="text" runat="server" name="txtBelgeTarihi" class="veriAlan" id="txtBelgeTarihi"
                    maxlength="10" size="10" />
                <img src="../App_themes/Images/takvim.gif" id="imgBelgeTarihi" alt="" style="cursor: pointer;"
                    onclick="JavaScript:displayDatePicker('txtBelgeTarihi', false, 'dmy', '.');" />
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <table width="100%" cellpadding="2" cellspacing="2">
        <tr>
            <td align="center">
                <asp:Button runat="server" ID="btnKaydet" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMIGF036 %>"
                    Width="80" OnClick="btnKaydet_Click" />
                <asp:Button runat="server" ID="btnYazdir" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMIGF037 %>"
                    Enabled="false" Width="80" OnClick="btnYazdir_Click" />
                <asp:Button runat="server" ID="btnTemizle" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMIGF038 %>"
                    Width="80" OnClick="btnTemizle_Click" />
                <asp:Button runat="server" ID="btnListele" CssClass="dugme" Text="Listele"
                    Width="80" OnClick="btnListele_Click" />
            </td>
        </tr>
    </table>
    <FarPoint:FpSpread ID="fpL" runat="server" Width="100%" Height="270" OnSaveOrLoadSheetState="fpL_SaveOrLoadSheetState"
        Style="position: relative; overflow: scroll;">
    </FarPoint:FpSpread>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
