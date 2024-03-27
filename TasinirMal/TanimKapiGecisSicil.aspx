<%@ Page Language="C#" CodeBehind="TanimKapiGecisSicil.aspx.cs" Inherits="TasinirMal.TanimKapiGecisSicil" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <link href="../App_themes/TakvimYeni.css" type="text/css" rel="stylesheet" />
    <script language="javascript" type="text/javascript" src="../Script/TakvimYeni.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=3"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Amortisman.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Defterler.js?v=1"></script>
</head>
<body onunload="HesapPlaniKapat();">
    <form id="form" runat="server">
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
                    <%= Resources.TasinirMal.FRMTKS004 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTKS005 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtMuhasebe','','lblMuhasebeAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTKS006 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTKS007 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimi','txtMuhasebe','lblHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTKS008 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtAmbar" runat="server" CssClass="veriAlan" MaxLength="3" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTKS009 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeAmbar.aspx','txtAmbar','txtHarcamaBirimi','lblAmbarAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
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
                <%= Resources.TasinirMal.FRMTKS010 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtHesapPlanKod" runat="server" CssClass="veriAlan" MaxLength="40"
                    Width="170px"></asp:TextBox></div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMTKS011 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:HesapPlaniGoster();return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" />
            </div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblHesapPlanAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTKS012 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtSicilNo" runat="server" CssClass="veriAlan" MaxLength="40" Width="170px"></asp:TextBox></div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMTKS013 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:SicilNoListesiAc();return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" />
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTKS014 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <input type="text" runat="server" name="txtTarih1" class="veriAlan" id="txtTarih1"
                    maxlength="10" size="10" />
                <img src="../App_themes/Images/takvim.gif" id="imgTarih1" alt="" style="cursor: pointer;"
                    onclick="JavaScript:displayDatePicker('txtTarih1', false, 'dmy', '.');" />
            </div>
        </div>

        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTKS015 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <input type="text" runat="server" name="txtTarih2" class="veriAlan" id="txtTarih2"
                    maxlength="10" size="10" />
                <img src="../App_themes/Images/takvim.gif" id="imgTarih2" alt="" style="cursor: pointer;"
                    onclick="JavaScript:displayDatePicker('txtTarih2', false, 'dmy', '.');" />
            </div>
        </div>
        
        <div class="row">
            <asp:CheckBox ID="chkKGS" runat="server" Text="<%$ Resources:TasinirMal, FRMTKS028 %>" />
        </div>
        <div class="row">
            <br />
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <asp:Button runat="server" ID="btnListele" OnClick="btnListele_Click" CssClass="dugme"
        Text="<%$ Resources:TasinirMal, FRMTKS016 %>" Width="90" />
    <asp:Button runat="server" ID="btnGecisYetkiKaydet" OnClick="btnGecisYetkiKaydet_Click" CssClass="dugme"
        Text="<%$ Resources:TasinirMal, FRMTKS026 %>" Width="120" />
    <asp:Button runat="server" ID="btnGecisYetkiSil" OnClick="btnGecisYetkiSil_Click" CssClass="dugme"
        Text="<%$ Resources:TasinirMal, FRMTKS027 %>" Width="120" />
    <asp:Button runat="server" ID="btnCikisFormYazdir" OnClick="btnCikisFormYaz_Click" CssClass="dugme"
        Text="<%$ Resources:TasinirMal, FRMTKS025 %>" Width="120" />
    <asp:Button runat="server" ID="btnLogYaz" OnClick="btnLogYaz_Click" CssClass="dugme"
        Text="<%$ Resources:TasinirMal, FRMTKS018 %>" Width="90" />
    <asp:Label ID="lblKacKayit" runat="server" CssClass="veriAlan" />
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
                ForeColor="White" HorizontalAlign="Left" VerticalAlign="Middle" />
            <Columns>
                <asp:TemplateColumn HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                    HeaderText="<input type='checkbox' onclick='javascript:ListeSec();' />">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSecim" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="yetki" HeaderText="<%$ Resources:TasinirMal, FRMTKS029 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="prSicilNo" Visible="false"></asp:BoundColumn>
                <asp:BoundColumn DataField="sicilNo" HeaderText="<%$ Resources:TasinirMal, FRMTKS019 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="hesapAd" HeaderText="<%$ Resources:TasinirMal, FRMTKS020 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="fiyat" HeaderText="<%$ Resources:TasinirMal, FRMTKS021 %>"
                    HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                <asp:BoundColumn DataField="muhasebeKod" HeaderText="<%$ Resources:TasinirMal, FRMZFS051 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="muhasebeAd" HeaderText="<%$ Resources:TasinirMal, FRMZFS051 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="harcamaKod" HeaderText="<%$ Resources:TasinirMal, FRMZFS052 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="harcamaAd" HeaderText="<%$ Resources:TasinirMal, FRMZFS052 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="ambarKod" HeaderText="<%$ Resources:TasinirMal, FRMZFS053 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="ambarAd" HeaderText="<%$ Resources:TasinirMal, FRMZFS053 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="odaKodAd" HeaderText="<%$ Resources:TasinirMal, FRMZKL012 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="ozelllik" HeaderText="<%$ Resources:TasinirMal, FRMRFE008 %>">
                </asp:BoundColumn>
            </Columns>
        </asp:DataGrid>
    </div>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
