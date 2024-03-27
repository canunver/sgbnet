<%@ Page Language="C#" CodeBehind="SicilNoOzellikEski.aspx.cs" Inherits="TasinirMal.SicilNoOzellikEski" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<%@ Register TagPrefix="farpoint" Namespace="FarPoint.Web.Spread" Assembly="FarPoint.Web.Spread" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%= Resources.TasinirMal.FRMSCO001 %></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=3"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/SicilNoOzellik.js?v=1"></script>
</head>
<body>
    <form id="form" runat="server">
    <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
    <ext:ResourceManager ID="ResourceManager1" runat="server">
        <Listeners>
            <DocumentReady Handler="ddlBirim.onTriggerClick();Panel1.hide();" />
        </Listeners>
    </ext:ResourceManager>
    <input type="hidden" id="txtBelgeTur" runat="server" />
    <div class="form">
        <div id="divMHA0" runat="server">
            <div class="row" id="divMuhasebe" runat="server">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMSCO046 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox>
                </div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMSCO047 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtMuhasebe','','lblMuhasebeAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                </div>
            </div>
            <div class="row" id="divHarcama" runat="server">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMSCO048 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox>
                </div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMSCO049 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimi','txtMuhasebe','lblHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                </div>
            </div>
            <div class="row" id="divAmbar" runat="server">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMSCO050 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtAmbar" runat="server" CssClass="veriAlan" MaxLength="3" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMSCO051 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeAmbar.aspx','txtAmbar','txtHarcamaBirimi','lblAmbarAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblAmbarAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                </div>
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
                <%= Resources.TasinirMal.FRMSCO052 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtSicilNo" runat="server" CssClass="veriAlan" MaxLength="40" Columns="30"></asp:TextBox>
            </div>
            <div class="kolonArasi">
            </div>
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMSCO053 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 60px;">
                <asp:DropDownList ID="ddlYil" runat="server">
                </asp:DropDownList>
                &nbsp;
            </div>
            <div class="veriAlan">
                <asp:TextBox ID="txtBelgeNo" runat="server" CssClass="veriAlan" MaxLength="30" Columns="10"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMSCO054 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:RadioButton ID="optTasinir" runat="server" GroupName="grpSecim" Checked="true"
                    Text="<%$ Resources:TasinirMal, FRMSCO055 %>" />
                <asp:RadioButton ID="optKutuphane" runat="server" GroupName="grpSecim" Text="<%$ Resources:TasinirMal, FRMSCO056 %>" />
                <asp:RadioButton ID="optMuze" runat="server" GroupName="grpSecim" Text="<%$ Resources:TasinirMal, FRMSCO057 %>" />
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMSCO064 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:FileUpload runat="server" ID="fupSeriNo" />
            </div>
        </div>
        <div class="row">
            <asp:CheckBox ID="chkKaydet" runat="server" Text="<%$ Resources:TasinirMal, FRMSCO058 %>" />
        </div>
    </div>
    <table width="100%" cellpadding="2" cellspacing="2">
        <tr>
            <td align="left">
                <asp:Button runat="server" ID="btnAra" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMSCO059 %>"
                    OnClick="btnAra_Click" Width="100" />
                <asp:Button runat="server" ID="btnKaydet" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMSCO060 %>"
                    OnClick="btnKaydet_Click" Width="100" />
                <asp:Button runat="server" ID="btnSeriNoYukle" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMSCO065 %>"
                    OnClick="btnSeriNoYukle_Click" Width="100" />
                <asp:Button runat="server" ID="btnSil" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMSCO061 %>"
                    Width="100" OnClick="btnSil_Click" />&nbsp;&nbsp;
            </td>
            <td align="right">
                <a href="#" onclick="javascript:PopupWin('TanimMarka.aspx?menuYok=1',620,500,'MarkaGiris');">
                    <%= Resources.TasinirMal.FRMSCO062 %></a>&nbsp;&nbsp; <a href="#" onclick="javascript:PopupWin('TanimModel.aspx?menuYok=1',620,500,'ModelGiris');">
                        <%= Resources.TasinirMal.FRMSCO063 %></a>
            </td>
        </tr>
    </table>
    <farpoint:FpSpread ID="fpL" runat="server" Width="100%" Height="270" OnSaveOrLoadSheetState="fpL_SaveOrLoadSheetState"
        Style="position: relative;">
    </farpoint:FpSpread>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
