<%@ Page Language="C#" CodeBehind="AcikPazarForm.aspx.cs" Inherits="TasinirMal.AcikPazarForm" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<%@ Register TagPrefix="farpoint" Namespace="FarPoint.Web.Spread" Assembly="FarPoint.Web.Spread" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <link href="../App_themes/TakvimYeni.css" type="text/css" rel="stylesheet" />
    <script language="javascript" type="text/javascript" src="../Script/TakvimYeni.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?mc=03022015"></script>
    <script language="javascript" type="text/javascript" src="../script/hideShow.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=2"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/AcikPazarForm.js?mc=03022015&v=2013_11"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
    <ext:ResourceManager ID="ResourceManager1" runat="server">
        <Listeners>
            <DocumentReady Handler="ddlBirim.onTriggerClick();Panel1.hide();" />
        </Listeners>
    </ext:ResourceManager>
    <div class="form">
        <div id="divMHA0" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMAPF022 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMAPF023 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtMuhasebe','','lblMuhasebeAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMAPF024 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMAPF025 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimi','txtMuhasebe','lblHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
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
        <div id="divIY" runat="server" style="display: none">
            <div class="row">
                <div class="yaziAlan" style="width: 99%">
                    <%= Resources.TasinirMal.FRMAPF026 %></div>
            </div>
            <div id="divMHA1" runat="server">
                <div class="row">
                    <div class="yaziAlan">
                        <%= Resources.TasinirMal.FRMAPF022 %></div>
                    <div class="ikiNokta">
                        :</div>
                    <div class="veriAlan">
                        <asp:TextBox ID="txtIYMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5"
                            Width="100px"></asp:TextBox></div>
                    <div class="veriAlan">
                        <input type="image" alt="<%= Resources.TasinirMal.FRMAPF023 %>" style="height: 14px;
                            width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtIYMuhasebe','','lblIYMuhasebeAd');return false;"
                            tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                    <div class="kolonArasi">
                    </div>
                    <div class="veriAlan">
                        <asp:Label ID="lblIYMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
                </div>
                <div class="row">
                    <div class="yaziAlan">
                        <%= Resources.TasinirMal.FRMAPF024 %></div>
                    <div class="ikiNokta">
                        :</div>
                    <div class="veriAlan">
                        <asp:TextBox ID="txtIYHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                            Width="100px"></asp:TextBox></div>
                    <div class="veriAlan">
                        <input type="image" alt="<%= Resources.TasinirMal.FRMAPF025 %>" style="height: 14px;
                            width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtIYHarcamaBirimi','txtIYMuhasebe','lblIYHarcamaBirimiAd');return false;"
                            tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                    <div class="kolonArasi">
                    </div>
                    <div class="veriAlan">
                        <asp:Label ID="lblIYHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
                </div>
            </div>
            <div class="row" id="divOrg1" runat="server" style="display: none">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMORG001 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <ext:DropDownField ID="ddlIYBirim" runat="server" Width="300" TriggerIcon="Ellipsis"
                        Mode="ValueText">
                        <Component>
                            <ext:Panel ID="Panel2" runat="server" Width="400">
                                <Items>
                                    <ext:TreePanel ID="TreePanel2" runat="server" Height="250" AutoWidth="true" AutoScroll="true"
                                        Border="false" RootVisible="false">
                                        <Root>
                                            <ext:AsyncTreeNode NodeID="0" Expanded="true" Icon="Note" />
                                        </Root>
                                        <Listeners>
                                            <BeforeLoad Fn="OrgBirimDoldur" />
                                            <Click Handler="#{ddlIYBirim}.setValue(node.id, node.text, false);#{ddlIYBirim}.collapse();OrgBirimBul(node, 'MuhasebeHarcamaAmbarBul(\'txtIYMuhasebe\', \'txtIYHarcamaBirimi\', null);');" />
                                        </Listeners>
                                    </ext:TreePanel>
                                </Items>
                                <Listeners>
                                    <Show Handler="try { TreePanel2.getRootNode().findChild('id', ddlIYBirim.getValue(), true).select(); } catch (e) { }" />
                                </Listeners>
                            </ext:Panel>
                        </Component>
                    </ext:DropDownField>
                </div>
            </div>
            <div class="row" style="height: 5px">
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMAPF027 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:Button runat="server" ID="btnYazdir" OnClick="btnYazdir_Click" CssClass="dugme"
                        Text="<%$ Resources:TasinirMal, FRMAPF028 %>" Width="50" Height="20" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Button runat="server" ID="btnTIF" OnClick="btnTIF_Click" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMAPF029 %>"
                        Width="75" Height="20" /></div>
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <center>
        <asp:Button runat="server" ID="btnListele" OnClick="btnListele_Click" CssClass="dugme"
            Text="<%$ Resources:TasinirMal, FRMAPF030 %>" Width="90" />
        <asp:Button runat="server" ID="btnKaydet" OnClick="btnKaydet_Click" CssClass="dugme"
            Text="<%$ Resources:TasinirMal, FRMAPF031 %>" Width="90" />
        <input type="button" id="btnTalep" onclick="hideshow('divIY');" class="dugme" value="<%= Resources.TasinirMal.FRMAPF032 %>"
            style="width: 90px" />
        <asp:Button runat="server" ID="btnSil" OnClick="btnSil_Click" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMAPF033 %>"
            Width="90" />
        <asp:Button runat="server" ID="btnTemizle" OnClick="btnTemizle_Click" CssClass="dugme"
            Text="<%$ Resources:TasinirMal, FRMAPF034 %>" Width="90" />
    </center>
    <center>
        <farpoint:FpSpread ID="fpL" runat="server" Width="100%" Height="300" OnSaveOrLoadSheetState="fpL_SaveOrLoadSheetState"
            Style="position: relative;">
        </farpoint:FpSpread>
    </center>
    <div id="divBekle" class="bekleKutusu">
        <br />
        <b>
            <%= Resources.TasinirMal.FRMAPF035 %></b>
        <br />
        <br />
        <img alt="<%= Resources.TasinirMal.FRMAPF036 %>" src="../App_themes/images/loading.gif" />
        <br />
        <br />
    </div>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
