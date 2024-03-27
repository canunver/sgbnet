<%@ Page Language="C#" CodeBehind="MuhasebatKontrolEski.aspx.cs" Inherits="TasinirMal.MuhasebatKontrolEski" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=2"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Defterler.js?mc=03022015&v=2012_01_05"></script>
</head>
<body onunload="HesapPlaniKapat();">
    <form id="form1" runat="server">
    <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
    <ext:ResourceManager ID="ResourceManager1" runat="server">
        <Listeners>
            <DocumentReady Handler="ddlBirim.onTriggerClick();Panel1.hide();" />
        </Listeners>
    </ext:ResourceManager>
    <div class="form">
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMMHK006 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 100px">
                <asp:DropDownList ID="ddlYil" CssClass="veriAlanDDL" runat="server">
                </asp:DropDownList>
            </div>
        </div>
        <div id="divMHA0" runat="server">
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMMHK007 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMMHK008 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtMuhasebe','','lblMuhasebeAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMMHK009 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMMHK010 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimi','txtMuhasebe','lblHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
        </div>
        <div class="row" id="divOrg0" runat="server" style="display: none">
            <div class="yaziAlan">
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
                                        <Click Handler="#{ddlBirim}.setValue(node.id, node.text, false);#{ddlBirim}.collapse();OrgBirimBul(node, 'MuhasebeHarcamaAmbarBul(\'txtMuhasebe\', \'txtHarcamaBirimi\', null);');" />
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
                <%= Resources.TasinirMal.FRMMHK011 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtHesapPlanKod" runat="server" CssClass="veriAlan" MaxLength="40"
                    Width="150px"></asp:TextBox></div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMMHK012 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:HesapPlaniGoster();return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" />
            </div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblHesapPlanAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMMHK013 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 100px">
                <asp:DropDownList ID="ddlDuzey" CssClass="veriAlanDDL" runat="server">
                </asp:DropDownList>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMMHK014 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:RadioButtonList ID="rblRaporTur" RepeatDirection="Horizontal" runat="server">
                </asp:RadioButtonList>
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <asp:Button ID="btnYazdir" TabIndex="99" runat="server" OnClick="btnYazdir_Click"
        Width="100" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMMHK015 %>"></asp:Button>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
