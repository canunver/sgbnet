<%@ Page Language="C#" CodeBehind="TuketimUretimSorgu.aspx.cs" Inherits="TasinirMal.TuketimUretimSorgu" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TuketimUretimSorgu.js?mc=03022015&v=2012_01_05"></script>
</head>
<body>
    <form id="form" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server">
        <Listeners>
            <DocumentReady Handler="ddlBirim.onTriggerClick();Panel1.hide();" />
        </Listeners>
    </ext:ResourceManager>
    <div class="form" style="margin-top: -2px;">
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTUL001 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 250px;">
                <asp:DropDownList ID="ddlYil" runat="server" CssClass="veriAlanDDL">
                </asp:DropDownList>
            </div>
        </div>
        <div id="divMHA0" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTUL002 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTUL003 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtMuhasebe','','lblMuhasebeAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTUL004 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTUL005 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimi','txtMuhasebe','lblHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTUL016 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtAmbar" runat="server" CssClass="veriAlan" MaxLength="15" Width="100px"></asp:TextBox>
                </div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTUL017 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeAmbar.aspx','txtAmbar','txtHarcamaBirimi','lblAmbarAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
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
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTUL015 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:DropDownList ID="ddlTur" runat="server">
                </asp:DropDownList>
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <table width="100%" cellpadding="2" cellspacing="2">
        <tr>
            <td align="center">
                <asp:Button runat="server" ID="btnListele" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMTUL006 %>"
                    Width="80" OnClick="btnListele_Click" />
            </td>
        </tr>
    </table>
    <div class="row" style="padding-left: 10px; text-align: center;">
        <div style="overflow: auto; width: 80%; height: 400px; border: solid 1px gray;">
            <asp:GridView ID="gvBelgeler" runat="server" CellPadding="1" ForeColor="#333333"
                GridLines="None" Width="100%" AutoGenerateColumns="false">
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                    Height="30px" VerticalAlign="Middle" Wrap="true" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" />
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:BoundField DataField="belgeNo" Visible="false"></asp:BoundField>
                    <asp:TemplateField HeaderText="<%$ Resources:TasinirMal, FRMTUL007 %>">
                        <ItemStyle BorderColor="LightGray" />
                        <ItemTemplate>
                            <a href="javascript:BelgeAc('<%# DataBinder.Eval(Container.DataItem, "yil")%>','<%# DataBinder.Eval(Container.DataItem, "muhasebe")%>','<%# DataBinder.Eval(Container.DataItem, "harcamaBirimi")%>','<%# DataBinder.Eval(Container.DataItem, "belgeNo")%>');">
                                <%# DataBinder.Eval(Container.DataItem, "belgeNo")%></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="yil" HeaderText="<%$ Resources:TasinirMal, FRMTUL001 %>" />
                    <asp:BoundField DataField="belgeTarih" HeaderText="<%$ Resources:TasinirMal, FRMTUL008 %>" />
                    <asp:BoundField DataField="muhasebe" HeaderText="<%$ Resources:TasinirMal, FRMTUL002 %>" />
                    <asp:BoundField DataField="muhasebeAd" HeaderText="<%$ Resources:TasinirMal, FRMTUL009 %>">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="harcamaBirimi" HeaderText="<%$ Resources:TasinirMal, FRMTUL004 %>" />
                    <asp:BoundField DataField="harcamaBirimiAd" HeaderText="<%$ Resources:TasinirMal, FRMTUL010 %>">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ambar" HeaderText="<%$ Resources:TasinirMal, FRMTUL011 %>">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="belgeTur" HeaderText="<%$ Resources:TasinirMal, FRMTUL015 %>" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    </form>
</body>
</html>
