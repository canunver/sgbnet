<%@ Page Language="C#" CodeBehind="TasinirIslemForm.aspx.cs" Inherits="TasinirMal.TasinirIslemForm"
    EnableEventValidation="false" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<%@ Register TagPrefix="farpoint" Namespace="FarPoint.Web.Spread" Assembly="FarPoint.Web.Spread" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <link href="../App_themes/TakvimYeni.css" type="text/css" rel="stylesheet" />
    <script language="javascript" type="text/javascript" src="../Script/TakvimYeni.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="../script/paraFormat.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=3"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirIslemForm.js?v=1"></script>
</head>
<body onunload="HesapPlaniKapat();">
    <form id="form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server">
        <Listeners>
            <DocumentReady Handler="ddlBirim.onTriggerClick();Panel1.hide();" />
        </Listeners>
    </ext:ResourceManager>
    <input type="hidden" id="txtBelgeTur" runat="server" />
    <input type="hidden" id="hdnFirmaHarcamadanAlma" runat="server" />
    <div class="form" style="margin-top: -2px;">
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTIG087 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 100px">
                <asp:DropDownList ID="ddlYil" runat="server">
                </asp:DropDownList>
            </div>
            <div class="kolonArasi">
            </div>
            <div class="yaziAlanK" style="width: 100px">
                <%= Resources.TasinirMal.FRMTIG088 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 100px">
                <asp:TextBox ID="txtBelgeTarih" Columns="10" runat="server" MaxLength="10" CssClass="veriAlan"></asp:TextBox>
                <img src="../App_themes/Images/takvim.gif" id="imgBelgeTarihi" alt="" style="cursor: pointer;"
                    onclick="JavaScript:displayDatePicker('txtBelgeTarih', false, 'dmy', '.');" />
            </div>
            <div class="yaziAlanK" style="width: 100px">
                <%= Resources.TasinirMal.FRMTIG089 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtBelgeNo" Columns="7" runat="server" MaxLength="7" CssClass="veriAlan"></asp:TextBox>
                &nbsp;
                <asp:ImageButton ID="btnListele" TabIndex="99" runat="server" Width="16" ImageUrl="../App_themes/images/bul.gif"
                    Height="16" AlternateText="<%$ Resources:TasinirMal, FRMTIG090 %>" OnClick="btnListele_Click">
                </asp:ImageButton>
            </div>
            <div class="veriAlan">
                &nbsp;<asp:Label ID="lblFormDurum" runat="server"></asp:Label></div>
        </div>
        <div id="divMHA0" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTIG091 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox>
                </div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTIG092 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtMuhasebe','','lblMuhasebeAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                </div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTIG093 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox>
                </div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTIG094 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimi','txtMuhasebe','lblHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                </div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTIG095 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtAmbar" runat="server" CssClass="veriAlan" MaxLength="3" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTIG096 %>" style="height: 14px;
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
        <div class="row" id="divIslemTipi" runat="server">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTIG097 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 350px">
                <asp:DropDownList ID="ddlIslemTipi" runat="server" CssClass="veriAlanDDL" Width="345px"
                    AutoPostBack="True" OnSelectedIndexChanged="ddlIslemTipi_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
            <div class="kolonArasi">
            </div>
        </div>
        <div class="row" id="divNeredenGeldi" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTIG098 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtNeredenGeldi" runat="server" CssClass="veriAlan" MaxLength="255"
                        Columns="55"></asp:TextBox>
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTIG099 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:FirmaGoster();return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />&nbsp;
                </div>
                <div class="kolonArasi">
                </div>
            </div>
        </div>
        <div class="row" id="divDoviz" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTIG127%></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:DropDownList ID="ddlDoviz" runat="server">
                    </asp:DropDownList>
                </div>
                <div class="kolonArasi">
                </div>
            </div>
        </div>
        <div class="row" id="divDayanakBelge" runat="server">
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTIG100 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 100px">
                    <asp:TextBox ID="txtDayanakTarih" runat="server" CssClass="veriAlan" MaxLength="10"
                        Columns="10"></asp:TextBox>
                    <img src="../App_themes/Images/takvim.gif" id="imgDayanakTarih" alt="" style="cursor: pointer;"
                        onclick="JavaScript:displayDatePicker('txtDayanakTarih', false, 'dmy', '.');" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="yaziAlan" style="width: 100px">
                    <%= Resources.TasinirMal.FRMTIG101 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 100px">
                    <asp:TextBox ID="txtDayanakNo" runat="server" CssClass="veriAlan" MaxLength="20"
                        Columns="10"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="row" id="divFatura" runat="server">
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTIG102 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 100px">
                    <asp:TextBox ID="txtFaturaTarih" runat="server" CssClass="veriAlan" MaxLength="10"
                        Columns="10"></asp:TextBox>
                    <img src="../App_themes/Images/takvim.gif" id="imgFaturaTarih" alt="" style="cursor: pointer;"
                        onclick="JavaScript:displayDatePicker('txtFaturaTarih', false, 'dmy', '.');" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="yaziAlan" style="width: 100px">
                    <%= Resources.TasinirMal.FRMTIG101 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 100px">
                    <asp:TextBox ID="txtFaturaNo" runat="server" CssClass="veriAlan" MaxLength="20" Columns="10"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="row" id="divKomisyon" runat="server">
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTIG103 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 100px">
                    <asp:TextBox ID="txtMuayeneTarih" runat="server" CssClass="veriAlan" MaxLength="10"
                        Columns="10"></asp:TextBox>
                    <img src="../App_themes/Images/takvim.gif" id="imgMuayeneTarih" alt="" style="cursor: pointer;"
                        onclick="JavaScript:displayDatePicker('txtMuayeneTarih', false, 'dmy', '.');" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="yaziAlan" style="width: 100px">
                    <%= Resources.TasinirMal.FRMTIG104 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 100px">
                    <asp:TextBox ID="txtMuayeneNo" runat="server" CssClass="veriAlan" MaxLength="20"
                        Columns="10"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="row" id="divNereyeGitti" style="display: none" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTIG105 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtNereyeGitti" runat="server" CssClass="veriAlan" MaxLength="255"
                        Columns="55"></asp:TextBox></div>
            </div>
        </div>
        <div class="row" id="divKimeGitti" style="display: none" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTIG106 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtKimeGitti" runat="server" CssClass="veriAlan" MaxLength="11"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTIG107 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListePersonel.aspx','txtKimeGitti','txtHarcamaBirimi','lblKimeGittiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblKimeGittiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
        </div>
        <div class="row" id="divGonderilenBirim" style="display: none" runat="server">
            <div class="row">
                <div class="yaziAlan" style="width: 99%">
                    <%= Resources.TasinirMal.FRMTIG108 %>&nbsp;<img id="imgDevir" src="../app_themes/images/aramainfo.gif"
                        alt="<%$ Resources:TasinirMal,FRMTIG109 %>" runat="server" onclick="javascript:DevirListesiAc();"
                        style="cursor: hand;" /></div>
            </div>
            <div id="divMHA1" runat="server">
                <div id="divGonderilenMuhasebe" class="row" runat="server">
                    <div class="yaziAlanK">
                        <%= Resources.TasinirMal.FRMTIG091 %></div>
                    <div class="ikiNokta">
                        :</div>
                    <div class="veriAlan">
                        <asp:TextBox ID="txtGonMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5"
                            Width="100px"></asp:TextBox>
                    </div>
                    <div class="veriAlan">
                        <input type="image" alt="<%= Resources.TasinirMal.FRMTIG092 %>" style="height: 14px;
                            width: 16px;" class="veriAlan" onclick="javascript:GonMuhasebeListeAc('ListeMuhasebe.aspx','txtGonMuhasebe','lblGonMuhasebeAd');return false;"
                            tabindex="100" src="../App_themes/images/bul1.gif" />
                    </div>
                    <div class="kolonArasi">
                    </div>
                    <div class="veriAlan">
                        <asp:Label ID="lblGonMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                    </div>
                </div>
                <div id="divGonderilenHarcamaBirimi" class="row" runat="server">
                    <div class="yaziAlanK">
                        <%= Resources.TasinirMal.FRMTIG093 %></div>
                    <div class="ikiNokta">
                        :</div>
                    <div class="veriAlan">
                        <asp:TextBox ID="txtGonHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                            Width="100px"></asp:TextBox>
                    </div>
                    <div class="veriAlan">
                        <input type="image" alt="<%= Resources.TasinirMal.FRMTIG094 %>" style="height: 14px;
                            width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtGonHarcamaBirimi','txtGonMuhasebe','lblGonHarcamaBirimiAd');return false;"
                            tabindex="100" src="../App_themes/images/bul1.gif" />
                    </div>
                    <div class="kolonArasi">
                    </div>
                    <div class="veriAlan">
                        <asp:Label ID="lblGonHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                    </div>
                </div>
                <div id="divGonderilenAmbar" class="row" runat="server">
                    <div class="yaziAlanK">
                        <%= Resources.TasinirMal.FRMTIG095 %></div>
                    <div class="ikiNokta">
                        :</div>
                    <div class="veriAlan">
                        <asp:TextBox ID="txtGonAmbar" runat="server" CssClass="veriAlan" MaxLength="3" Width="100px"></asp:TextBox>
                    </div>
                    <div class="veriAlan">
                        <input type="image" alt="<%= Resources.TasinirMal.FRMTIG096 %>" style="height: 14px;
                            width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeAmbar.aspx','txtGonAmbar','txtGonHarcamaBirimi','lblGonAmbarAd');return false;"
                            tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                    <div class="kolonArasi">
                    </div>
                    <div class="veriAlan">
                        <asp:Label ID="lblGonAmbarAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="row" id="divOrg1" runat="server" style="display: none">
                <div class="yaziAlanK">
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
                                            <Click Handler="#{ddlBirim2}.setValue(node.id, node.text, false);#{ddlBirim2}.collapse();OrgBirimBul(node, 'MuhasebeHarcamaAmbarBul(\'txtGonMuhasebe\', \'txtGonHarcamaBirimi\', \'txtGonAmbar\');');" />
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
        </div>
        <div class="row" id="divGonderilenBelge" style="display: none" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTIG110 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 100px">
                    <asp:DropDownList ID="ddlGonYil" runat="server">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTIG111 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtGonBelgeNo" runat="server" CssClass="veriAlan" MaxLength="7"
                        Width="100px"></asp:TextBox>
                    &nbsp;
                    <asp:ImageButton ID="btnGonListele" TabIndex="99" runat="server" Width="16" ImageUrl="../App_themes/images/bul.gif"
                        Height="16" AlternateText="<%$ Resources:TasinirMal, FRMTIG090 %>" OnClick="btnGonListele_Click">
                    </asp:ImageButton>
                </div>
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <table width="100%">
        <tr>
            <td style="width: 100%; height: 43px;">
                <button class="dugme" id="btnYazdir" style="width: 85px" onclick="BelgeYazdirGoster();return false;"
                    tabindex="99" type="button">
                    <%= Resources.TasinirMal.FRMTIG112 %></button>&nbsp;
                <asp:Button ID="btnKaydet" TabIndex="99" runat="server" Width="85px" CssClass="dugme"
                    Text="<%$ Resources:TasinirMal, FRMTIG113 %>" OnClick="btnKaydet_Click"></asp:Button>&nbsp;
                <asp:Button ID="btnTemizle" TabIndex="99" runat="server" Width="60px" CssClass="dugme"
                    Text="<%$ Resources:TasinirMal, FRMTIG114 %>" OnClick="btnTemizle_Click"></asp:Button>&nbsp;
                <button class="dugme" id="btnKomisyon" runat="server" style="width: 120px" onclick="KomisyonGoster();return false;"
                    tabindex="99" type="button">
                    <%= Resources.TasinirMal.FRMTIG115 %></button>&nbsp;
                <button class="dugme" id="btnOzellik" runat="server" style="width: 120px" onclick="BelgeOzellikGoster();return false;"
                    tabindex="99" type="button">
                    <%= Resources.TasinirMal.FRMTIG116 %></button>&nbsp;
                <button class="dugme" id="btnBarkodYazdir" runat="server" style="width: 120px" onclick="BarkodEkraniAc();return false;"
                    tabindex="99" type="button">
                    <%= Resources.TasinirMal.FRMTIG117 %></button>&nbsp;
                <button class="dugme" id="btnSicilYazdir" runat="server" style="width: 75px" onclick="TIFSicilYazdir();return false;"
                    tabindex="99" type="button">
                    <%= Resources.TasinirMal.FRMTIG118 %></button>
            </td>
            <td style="width: 10%;">
                <asp:Button ID="btnOnayla" TabIndex="99" runat="server" Width="60px" CssClass="dugme"
                    Text="<%$ Resources:TasinirMal, FRMTIG119 %>" OnClick="btnOnayla_Click"></asp:Button>&nbsp;
                <asp:Button ID="btnOnayKaldir" TabIndex="99" runat="server" Width="75px" CssClass="dugme"
                    Text="<%$ Resources:TasinirMal, FRMTIG120 %>" OnClick="btnOnayKaldir_Click">
                </asp:Button>
            </td>
        </tr>
    </table>
    <farpoint:FpSpread ID="fpL" runat="server" Width="99%" Height="350" OnSaveOrLoadSheetState="fpL_SaveOrLoadSheetState"
        Style="position: relative;">
    </farpoint:FpSpread>
    <br />
    <div id="pnlDosyaYukle" runat="server">
        <table width="600" cellspacing="2" cellpadding="2" border="0" class="tabloCerceve">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td class="yaziAlanK" style="width: 110px;">
                                <%= Resources.TasinirMal.FRMTIG121 %>
                            </td>
                            <td class="yaziAlanK" style="width: 5px;">
                                :
                            </td>
                            <td class="veriAlan" style="width: 150px;">
                                <input class="dugme" id="fileListe" type="file" size="30" name="fileListe" runat="server" />
                            </td>
                            <td>
                                <asp:Button ID="btnDosyaYukle" runat="server" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMTIG122 %>"
                                    OnClick="btnDosyaYukle_Click"></asp:Button>&nbsp;
                                <asp:Button ID="btnDosyaSakla" runat="server" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMTIG123 %>"
                                    OnClick="btnDosyaSakla_Click"></asp:Button>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <asp:Button ID="btnSicilNoYukle" runat="server" OnClick="btnSicilNoYukle_Click" Style="display: none">
    </asp:Button>
    <div id="divBekle" class="bekleKutusu">
        <br />
        <b>
            <%= Resources.TasinirMal.FRMTIG124 %></b>
        <br />
        <br />
        <img alt="<%= Resources.TasinirMal.FRMTIG125 %>" src="../App_themes/images/loading.gif" />
        <br />
        <br />
    </div>
    <iframe id="frmBelgeYazdir" frameborder="0" scrolling="no" width="1" height="1">
    </iframe>
    </form>
</body>
</html>
