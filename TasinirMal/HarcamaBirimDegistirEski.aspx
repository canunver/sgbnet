<%@ Page Language="C#" CodeBehind="HarcamaBirimDegistirEski.aspx.cs" Inherits="TasinirMal.HarcamaBirimDegistirEski" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <link href="../App_themes/TakvimYeni.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="../script/TakvimYeni.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=3"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/ZimmetForm.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?v=1"></script>
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
                    <%= Resources.TasinirMal.FRMHBD003 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebeEski" runat="server" CssClass="veriAlan" MaxLength="5"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMHBD004 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtMuhasebeEski','','lblMuhasebeAdEski');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblMuhasebeAdEski" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMHBD005 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimiEski" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMHBD006 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimiEski','txtMuhasebeEski','lblHarcamaBirimiAdEski');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblHarcamaBirimiAdEski" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
        </div>
        <div class="row" id="divOrg0" runat="server" style="display: none">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMORG002 %></div>
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
                                        <Click Handler="#{ddlBirim}.setValue(node.id, node.text, false);#{ddlBirim}.collapse();OrgBirimBul(node, 'MuhasebeHarcamaAmbarBul(\'txtMuhasebeEski\', \'txtHarcamaBirimiEski\', null);');" />
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
        <div id="divMHA1" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMHBD007 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebeYeni" runat="server" CssClass="veriAlan" MaxLength="5"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMHBD008 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtMuhasebeYeni','','lblMuhasebeAdYeni');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblMuhasebeAdYeni" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMHBD009 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimiYeni" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMHBD010 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimiYeni','txtMuhasebeYeni','lblHarcamaBirimiAdYeni');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblHarcamaBirimiAdYeni" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
        </div>
        <div class="row" id="divOrg1" runat="server" style="display: none">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMORG003 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <ext:DropDownField ID="ddlBirim2" runat="server" Width="300" TriggerIcon="Ellipsis"
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
                                        <Click Handler="#{ddlBirim2}.setValue(node.id, node.text, false);#{ddlBirim2}.collapse();OrgBirimBul(node, 'MuhasebeHarcamaAmbarBul(\'txtMuhasebeYeni\', \'txtHarcamaBirimiYeni\', null);');" />
                                    </Listeners>
                                </ext:TreePanel>
                            </Items>
                            <Listeners>
                                <Show Handler="try { TreePanel2.getRootNode().findChild('id', ddlBirim2.getValue(), true).select(); } catch (e) { }" />
                            </Listeners>
                        </ext:Panel>
                    </Component>
                </ext:DropDownField>
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <asp:Button ID="btnDegistir" TabIndex="99" runat="server" Width="100" CssClass="dugme"
        Text="<%$ Resources:TasinirMal, FRMHBD011 %>" OnClick="btnDegistir_Click"></asp:Button>&nbsp;
    <div id="divBekle" class="bekleKutusu">
        <br />
        <b>
            <%= Resources.TasinirMal.FRMHBD012 %></b>
        <br />
        <br />
        <img alt="<%= Resources.TasinirMal.FRMHBD013 %>" src="../App_themes/images/loading.gif" />
        <br />
        <br />
    </div>
    <div class="bilgiKutusu">
        <li>
            <%= Resources.TasinirMal.FRMHBD014 %></li>
        <li>
            <%= Resources.TasinirMal.FRMHBD015 %></li>
        <li>
            <%= Resources.TasinirMal.FRMHBD016 %></li>
        <li>
            <%= Resources.TasinirMal.FRMHBD017 %></li>
    </div>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
