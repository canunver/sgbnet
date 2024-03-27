<%@ Page Language="C#" CodeBehind="TasinirIslemMalzemeTarihceRaporuEski.aspx.cs" Inherits="TasinirMal.TasinirIslemMalzemeTarihceRaporuEski" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <link href="../App_themes/TakvimYeni.css" type="text/css" rel="stylesheet" />
    <script language="javascript" type="text/javascript" src="../Script/TakvimYeni.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="../script/hideShow.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=2"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirIslemForm.js?mc=03022015&v=2012_01_05"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirIslemSorgu.js?mc=03022015&v=2012_01_05"></script>
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
    <input type="hidden" id="hdnFirmaHarcamadanAlma" runat="server" />
    <div class="form">
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTMT004 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 100px">
                <asp:DropDownList ID="ddlYil" CssClass="veriAlanDDL" runat="server">
                </asp:DropDownList>
            </div>
        </div>
        <div id="divMHA0" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTMT005 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTMT006 %>" style="height: 14px;
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
                    <%= Resources.TasinirMal.FRMTMT007 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTMT008 %>" style="height: 14px;
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
                    <%= Resources.TasinirMal.FRMTMT009 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtAmbar" runat="server" CssClass="veriAlan" MaxLength="3" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTMT010 %>" style="height: 14px;
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
                <%= Resources.TasinirMal.FRMTMT011 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:DropDownList ID="ddlIslemTipi" runat="server" CssClass="veriAlanDDL" Width="150px">
                </asp:DropDownList>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTMT012 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 250px">
                <input type="text" runat="server" name="txtTarih1" class="veriAlan" id="txtTarih1"
                    maxlength="10" size="10" />
                <img src="../App_themes/Images/takvim.gif" id="imgTarih1" alt="" style="cursor: pointer;"
                    onclick="JavaScript:displayDatePicker('txtTarih1', false, 'dmy', '.');" />
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTMT013 %></div>
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
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTMT014 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtHesapPlanKod" runat="server" CssClass="veriAlan" MaxLength="40"
                    Width="150px"></asp:TextBox></div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMTMT015 %>" style="height: 14px;
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
                <%= Resources.TasinirMal.FRMTMT016 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:CheckBox ID="chk2Duzey" runat="server" CssClass="veriAlan"></asp:CheckBox></div>
        </div>
        <div id="divEkSorgu">
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTMT017 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtGonMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTMT006 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:GonMuhasebeListeAc('ListeMuhasebe.aspx','txtGonMuhasebe','lblGonMuhasebeAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblGonMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTMT018 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtGonHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTMT008 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtGonHarcamaBirimi','txtGonMuhasebe','lblGonHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblGonHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTMT019 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtGonAmbar" runat="server" CssClass="veriAlan" MaxLength="3" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTMT010 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeAmbar.aspx','txtGonAmbar','txtGonHarcamaBirimi','lblGonAmbarAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblGonAmbarAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTMT020 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtKimeGitti" runat="server" CssClass="veriAlan" MaxLength="11"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTMT021 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListePersonel.aspx','txtKimeGitti','txtHarcamaBirimi','lblKimeGittiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblKimeGittiAd" Width="25%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTMT022 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtNeredenGeldi" runat="server" CssClass="veriAlan" MaxLength="255"
                        Width="150px"></asp:TextBox>
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTMT023 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:FirmaGoster();return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTMT024 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtNereyeGitti" runat="server" CssClass="veriAlan" MaxLength="255"
                        Width="150px"></asp:TextBox></div>
            </div>
        </div>
        <div class="row">
            <a onclick="EkSorguSaklaGoster();" href="#">
                <img src="..\app_themes\images\gelismisarama.gif" alt="" />&nbsp;<b><span id="btnEkSorgu"></span></b></a><script
                    language="javascript" type="text/javascript">                                                                                                                             EkSorguSaklaGoster();</script>
        </div>
    </div>
    <asp:Button ID="btnYazdir" TabIndex="99" OnClick="btnYazdir_Click" runat="server"
        Width="100" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMTMT025 %>"></asp:Button>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
