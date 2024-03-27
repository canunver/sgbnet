<%@ Page Language="C#" CodeBehind="CetvellerEski.aspx.cs" Inherits="TasinirMal.CetvellerEski" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <link href="../App_themes/TakvimYeni.css" type="text/css" rel="stylesheet" />
    <script language="javascript" type="text/javascript" src="../Script/TakvimYeni.js?mc=03022015"></script>
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
                <%= Resources.TasinirMal.FRMCTV053 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 100px">
                <asp:DropDownList ID="ddlYil" CssClass="veriAlanDDL" runat="server">
                </asp:DropDownList>
            </div>
        </div>
        <div class="row" id="divBolge" runat="server" style="display: none">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMCTV054 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtBolge" runat="server" CssClass="veriAlan" MaxLength="5" Width="30px"></asp:TextBox>
            </div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMCTV055 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeBolge.aspx','txtBolge','','lblBolgeAd');return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" />
            </div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblBolgeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
            </div>
        </div>
        <div class="row" id="divIl" runat="server" style="display: none">
            <div id="divIlYaziAlan" runat="server" class="yaziAlan">
                <%= Resources.TasinirMal.FRMCTV056 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 250px">
                <asp:DropDownList ID="ddlIl" runat="server" CssClass="veriAlanDDL" Width="250px"
                    AutoPostBack="True" OnSelectedIndexChanged="ddlIl_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
        </div>
        <div class="row" id="divIlce" runat="server" style="display: none">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMCTV057 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 250px">
                <asp:DropDownList ID="ddlIlce" CssClass="veriAlanDDL" runat="server" Width="250px">
                </asp:DropDownList>
            </div>
        </div>
        <div id="divMHA0" runat="server">
            <div class="row" id="divMuhasebe" runat="server" style="display: none">
                <div id="divMuhasebeYaziAlan" runat="server" class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMCTV058 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="150px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMCTV059 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtMuhasebe','','lblMuhasebeAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row" id="divHarcama" runat="server" style="display: none">
                <div id="divHarcamaYaziAlan" runat="server" class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMCTV060 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" Width="150px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" runat="server" id="imgHarcama" alt="<%= Resources.TasinirMal.FRMCTV061 %>"
                        style="height: 14px; width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimi','txtMuhasebe','lblHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
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
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMCTV062 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtHesapPlanKod" runat="server" CssClass="veriAlan" MaxLength="40"
                    Width="150px"></asp:TextBox></div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMCTV063 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:HesapPlaniGoster();return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" /></div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblHesapPlanAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMCTV064 %></div>
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
                <%= Resources.TasinirMal.FRMCTV065 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <input type="text" runat="server" name="txtTarih2" class="veriAlan" id="txtTarih2"
                    maxlength="10" size="10" />
                <img src="../App_themes/Images/takvim.gif" id="imgTarih2" alt="" style="cursor: pointer;"
                    onclick="JavaScript:displayDatePicker('txtTarih2', false, 'dmy', '.');" />
            </div>
        </div>
        <div class="row" id="divSeviye" runat="server" style="display: none">
            <div class="yaziAlan">
                Seviye</div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 100px">
                <asp:DropDownList ID="ddlSeviye" CssClass="veriAlanDDL" runat="server">
                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                <asp:ListItem Text="2" Value="2" Selected="True"></asp:ListItem>
                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="row" id="divMuhasebeRapor" runat="server" style="display: none">
            <div class="yaziAlanK">
            </div>
            <div class="ikiNokta">
            </div>
            <div class="veriAlan">
                <asp:CheckBox ID="chkMuhasebeRapor" runat="server" Text="<%$ Resources:TasinirMal, FRMCTV066 %>" /></div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <asp:Button ID="btnYazdir" TabIndex="99" runat="server" Width="100" CssClass="dugme"
        Text="<%$ Resources:TasinirMal, FRMCTV067 %>" OnClick="btnYazdir_Click"></asp:Button>
    <asp:CheckBox ID="chkCSV" runat="server" Text="Sayıştay CSV Formatında çıktı üret"
        Visible="false" />
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
