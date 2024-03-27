<%@ Page Language="C#" CodeBehind="SayimRFIDListe.aspx.cs" Inherits="TasinirMal.SayimRFIDListe" %>

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
    <script language="javascript" type="text/javascript">
        function Yazdir(sayimNo) {
            document.getElementById("hdnSayimNo").value = sayimNo;
            __doPostBack('btnYazdir', '');
        }
    </script>
</head>
<body>
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
                <%= Resources.TasinirMal.FRMSRF005 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 100px">
                <asp:DropDownList ID="ddlYil" runat="server" CssClass="veriAlanDDL">
                </asp:DropDownList>
            </div>
            <div class="kolonArasi">
            </div>
            <div class="yaziAlanK" style="width: 100px">
                <%= Resources.TasinirMal.FRMSRF020 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 150px">
                <asp:TextBox ID="txtSayimTarih" Columns="10" runat="server" MaxLength="10" CssClass="veriAlan"></asp:TextBox>
                <img src="../App_themes/Images/takvim.gif" id="imgBelgeTarihi" alt="" style="cursor: pointer;"
                    onclick="JavaScript:displayDatePicker('txtSayimTarih', false, 'dmy', '.');" />
            </div>
            <div class="kolonArasi">
            </div>
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMSRF025 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:DropDownList ID="ddlDurum" runat="server" CssClass="veriAlanDDL">
                </asp:DropDownList>
            </div>
        </div>
        <div id="divMHA0" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMSRF006 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMSRF007 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtMuhasebe','','lblMuhasebeAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMSRF008 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMSRF009 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimi','txtMuhasebe','lblHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMSRF010 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtAmbar" runat="server" CssClass="veriAlan" MaxLength="3" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMSRF011 %>" style="height: 14px;
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
                <%= Resources.TasinirMal.FRMSRF019 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtSayimAd" runat="server" CssClass="veriAlan" Width="300"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMSRF021 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtPersonel" runat="server" CssClass="veriAlan" MaxLength="11" Width="100px"></asp:TextBox></div>
            <div class="veriAlan" id="divPersonelSecim" runat="server">
                <input type="image" alt="<%= Resources.TasinirMal.FRMSRF023 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListePersonel.aspx','txtPersonel','txtHarcamaBirimi','lblPersonelAd');return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" /></div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblPersonelAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMSRF022 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtOda" runat="server" CssClass="veriAlan" MaxLength="10"
                    Width="100px"></asp:TextBox></div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMSRF024 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeOda.aspx','txtOda','txtHarcamaBirimi','lblOdaAd');return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" /></div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblOdaAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <asp:Button runat="server" ID="btnListele" OnClick="btnListele_Click" CssClass="dugme"
        Text="<%$ Resources:TasinirMal, FRMSRF012 %>" Width="90" />
    <asp:Button runat="server" ID="btnKaydet" OnClick="btnKaydet_Click" CssClass="dugme"
        Text="<%$ Resources:TasinirMal, FRMSRF026 %>" Width="120" />
    <div style="visibility: hidden">
        <asp:Button runat="server" ID="btnYazdir" OnClick="btnYazdir_Click" CssClass="dugme"
            Text="<%$ Resources:TasinirMal, FRMSRF013 %>" Width="90" /></div>
    <div style="overflow: auto; width: 99%; height: 450px; border: solid 1px gray;">
        <asp:DataGrid ID="dgListe" runat="server" AutoGenerateColumns="False" CellPadding="2"
            Font-Bold="False" Font-Italic="False" Font-Names="Tahoma" Font-Overline="False"
            Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" ForeColor="#333333"
            PageSize="1" Width="100%">
            <ItemStyle BorderColor="LightGray" HorizontalAlign="Left" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                VerticalAlign="Middle" />
            <Columns>
                <asp:BoundColumn DataField="Sayim" HeaderText="<%$ Resources:TasinirMal, FRMSRF017 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="Yil" HeaderText="<%$ Resources:TasinirMal, FRMSRF005 %>" ItemStyle-HorizontalAlign="Center">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="Muhasebe" HeaderText="<%$ Resources:TasinirMal, FRMSRF014 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="Harcama" HeaderText="<%$ Resources:TasinirMal, FRMSRF015 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="Ambar" HeaderText="<%$ Resources:TasinirMal, FRMSRF016 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="Personel" HeaderText="<%$ Resources:TasinirMal, FRMSRF030 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="Oda" HeaderText="<%$ Resources:TasinirMal, FRMSRF031 %>">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="Durum" HeaderText="<%$ Resources:TasinirMal, FRMSRF032 %>" ItemStyle-HorizontalAlign="Center">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="Tarih" HeaderText="<%$ Resources:TasinirMal, FRMSRF018 %>" ItemStyle-HorizontalAlign="Center">
                </asp:BoundColumn>
            </Columns>
        </asp:DataGrid>
    </div>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
