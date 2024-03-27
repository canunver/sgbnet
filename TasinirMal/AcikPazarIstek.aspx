<%@ Page Language="C#" CodeBehind="AcikPazarIstek.aspx.cs" Inherits="TasinirMal.AcikPazarIstek" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=2"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/AcikPazarIstek.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Defterler.js?v=1"></script>
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
            <div class="yaziAlan" style="width: 99%">
                <%= Resources.TasinirMal.FRMAPI004 %></div>
        </div>
        <div id="divMHA0" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMAPI005 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtIYMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMAPI006 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtIYMuhasebe','','lblIYMuhasebeAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblIYMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMAPI007 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtIYHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMAPI008 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtIYHarcamaBirimi','txtIYMuhasebe','lblIYHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblIYHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
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
                                        <Click Handler="#{ddlBirim}.setValue(node.id, node.text, false);#{ddlBirim}.collapse();OrgBirimBul(node, 'MuhasebeHarcamaAmbarBul(\'txtIYMuhasebe\', \'txtIYHarcamaBirimi\', null);');" />
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
                <%= Resources.TasinirMal.FRMAPI009 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtHesapPlanKod" runat="server" CssClass="veriAlan" MaxLength="40"
                    Width="150px"></asp:TextBox></div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMAPI010 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:HesapPlaniGoster();return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" /></div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblHesapPlanAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMAPI011 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:DropDownList ID="ddlIl" runat="server" CssClass="veriAlanDDL" Width="150px"
                    AutoPostBack="True" OnSelectedIndexChanged="ddlIl_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMAPI012 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:DropDownList ID="ddlIlce" CssClass="veriAlanDDL" runat="server" Width="150px">
                </asp:DropDownList>
            </div>
        </div>
        <div id="divMHA1" runat="server">
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMAPI005 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMAPI006 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtMuhasebe','','lblMuhasebeAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMAPI007 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMAPI008 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimi','txtMuhasebe','lblHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
        </div>
        <div class="row" id="divOrg1" runat="server" style="display: none">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMORG001 %></div>
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
                                        <Click Handler="#{ddlBirim2}.setValue(node.id, node.text, false);#{ddlBirim2}.collapse();OrgBirimBul(node, 'MuhasebeHarcamaAmbarBul(\'txtMuhasebe\', \'txtHarcamaBirimi\', null);');" />
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
    <center>
        <asp:Button runat="server" ID="btnListele" OnClick="btnListele_Click" CssClass="dugme"
            Text="<%$ Resources:TasinirMal, FRMAPI013 %>" Width="90" />
        <asp:Button runat="server" ID="btnKaydet" OnClick="btnKaydet_Click" CssClass="dugme"
            Text="<%$ Resources:TasinirMal, FRMAPI014 %>" Width="90" />
        <%--                        <asp:Button runat="server" ID="btnYazdir" CssClass="dugme" Text="Yazdır" Width="90" />
        --%>
    </center>
    <div class="row">
        <table width="100%" cellpadding="2" cellspacing="2">
            <tr>
                <td align="center">
                    <asp:Label ID="lblKacKayit" runat="server" CssClass="veriAlan" />
                </td>
            </tr>
            <tr>
                <td align="center">
                    <div style="overflow: auto; height: 250px;">
                        <asp:DataGrid ID="dgListe" runat="server" AllowCustomPaging="false" AllowSorting="false"
                            AutoGenerateColumns="False" BorderWidth="0" CellPadding="3" Font-Bold="False"
                            Font-Italic="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="8pt"
                            Font-Strikeout="False" Font-Underline="False" ForeColor="#333333" PageSize="1"
                            Width="98%">
                            <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                            <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Left" />
                            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" Font-Italic="False" Font-Names="Tahoma"
                                Font-Overline="False" Font-Size="8pt" Font-Strikeout="False" Font-Underline="False"
                                ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle" />
                            <Columns>
                                <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="<input type='checkbox' onclick='javascript:ListeSec();' />">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSecim" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="prSicilNo" Visible="false"></asp:BoundColumn>
                                <asp:BoundColumn DataField="sicilNo" HeaderText="<%$ Resources:TasinirMal, FRMAPI015 %>">
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="hesapAd" HeaderText="<%$ Resources:TasinirMal, FRMAPI016 %>">
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="aciklama" HeaderText="<%$ Resources:TasinirMal, FRMAPI017 %>">
                                </asp:BoundColumn>
                                <asp:BoundColumn ItemStyle-HorizontalAlign="Center" DataField="eklenisTarihi" HeaderText="<%$ Resources:TasinirMal, FRMAPI018 %>">
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="muhasebeKod" HeaderText="<%$ Resources:TasinirMal, FRMAPI005 %>">
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="muhasebeAd" HeaderText="<%$ Resources:TasinirMal, FRMAPI019 %>">
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="harcamaKod" HeaderText="<%$ Resources:TasinirMal, FRMAPI007 %>">
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="harcamaAd" HeaderText="<%$ Resources:TasinirMal, FRMAPI020 %>">
                                </asp:BoundColumn>
                            </Columns>
                        </asp:DataGrid>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div id="divBekle" class="bekleKutusu">
        <br />
        <b>
            <%= Resources.TasinirMal.FRMAPI021 %></b>
        <br />
        <br />
        <img alt="<%= Resources.TasinirMal.FRMAPI022 %>" src="../App_themes/images/loading.gif" />
        <br />
        <br />
    </div>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
