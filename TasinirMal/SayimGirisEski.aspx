<%@ Page Language="C#" CodeBehind="SayimGirisEski.aspx.cs" Inherits="TasinirMal.SayimGirisEski" %>

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
    <script language="javascript" type="text/javascript" src="ModulScripts/SayimGiris.js?mc=03022015&v=2013_11"></script>
</head>
<body onunload="HesapPlaniKapat();">
    <form id="form" runat="server">
    <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
    <ext:ResourceManager ID="ResourceManager1" runat="server">
        <Listeners>
            <DocumentReady Handler="ddlBirim.onTriggerClick();Panel1.hide();" />
        </Listeners>
    </ext:ResourceManager>
    <input type="hidden" id="hdnSayimNo" runat="server" />
    <div class="form">
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMSYG030 %></div>
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
                    <%= Resources.TasinirMal.FRMSYG031 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMSYG032 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtMuhasebe','','lblMuhasebeAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMSYG033 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMSYG034 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimi','txtMuhasebe','lblHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMSYG035 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtAmbar" runat="server" CssClass="veriAlan" MaxLength="3" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMSYG036 %>" style="height: 14px;
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
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMSYG037 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <input type="text" runat="server" name="txtSayimTarihi" class="veriAlan" id="txtSayimTarihi"
                    maxlength="10" size="10" />
                <img src="../App_themes/Images/takvim.gif" id="imgSayimTarihi" alt="" style="cursor: pointer;"
                    onclick="JavaScript:displayDatePicker('txtSayimTarihi', false, 'dmy', '.');" />
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <div class="row">
        <table width="100%" cellpadding="2" cellspacing="2">
            <tr>
                <td align="center">
                    <asp:Button runat="server" ID="btnKaydet" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMSYG039 %>"
                        Width="60" OnClick="btnKaydet_Click" />
                    <asp:Button runat="server" ID="btnSil" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMSYG040 %>"
                        Width="54" Enabled="false" OnClick="btnSil_Click" />
                    <asp:Button runat="server" ID="btnSayimTutanak" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMSYG041 %>"
                        Enabled="false" Width="100" OnClick="btnSayimTutanak_Click" />
                    <asp:Button runat="server" ID="btnAmbarDevirTutanak" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMSYG042 %>"
                        Enabled="false" Width="200" OnClick="btnAmbarDevirTutanak_Click" />
                    <asp:Button runat="server" ID="btnTemizle" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMSYG043 %>"
                        Width="60" OnClick="btnTemizle_Click" />
                    <asp:Button runat="server" ID="btnListele" CssClass="dugme" Text="Listele"
                        Width="80" OnClick="btnListele_Click" />
                </td>
                <td align="center">
                    <asp:Button runat="server" ID="btnAmbarAktar" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMSYG044 %>"
                        Width="160" OnClick="btnAmbarAktar_Click" />
                    <asp:Button runat="server" ID="btnAmbarAktarKaydet" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMSYG051 %>"
                        Width="180" OnClick="btnAmbarAktarKaydet_Click" />
                    <asp:Button runat="server" ID="btnTIFNoksan" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMSYG045 %>"
                        Width="160" Enabled="false" OnClick="btnTIFNoksan_Click" />
                    <asp:Button runat="server" ID="btnTIFFazla" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMSYG046 %>"
                        Width="160" Enabled="false" OnClick="btnTIFFazla_Click" />
                    <input type="button" onclick="javascript:ListeAcTerminal('TerminalOku.aspx');return false;"
                        value="<%= Resources.TasinirMal.FRMSYG047 %>" class="dugme" />
                </td>
        </table>
        <FarPoint:FpSpread ID="fpL" runat="server" Width="100%" Height="250" OnSaveOrLoadSheetState="fpL_SaveOrLoadSheetState"
            Style="position: relative;">
        </FarPoint:FpSpread>
    </div>
    <div id="divBekle" class="bekleKutusu">
        <br />
        <b>
            <%= Resources.TasinirMal.FRMSYG048 %></b>
        <br />
        <br />
        <img alt="<%= Resources.TasinirMal.FRMSYG049 %>" src="../App_themes/images/loading.gif" />
        <br />
        <br />
    </div>
    <br />
    <div class="bilgiKutusu">
        <li>&nbsp;&nbsp;<%= Resources.TasinirMal.FRMSYG050 %></li>
    </div>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
