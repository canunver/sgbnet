<%@ Page Language="C#" CodeBehind="ZimmetFormEski.aspx.cs" Inherits="TasinirMal.ZimmetFormEski" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<%@ Register TagPrefix="farpoint" Namespace="FarPoint.Web.Spread" Assembly="FarPoint.Web.Spread" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <link href="../App_themes/TakvimYeni.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="../script/TakvimYeni.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=2"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/ZimmetForm.js?mc=03022015&v=2014_08"></script>
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?mc=03022015"></script>
</head>
<body onunload="HesapPlaniKapat();">
    <form id="form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server">
        <Listeners>
            <DocumentReady Handler="ddlBirim.onTriggerClick();Panel1.hide();" />
        </Listeners>
    </ext:ResourceManager>
    <asp:HiddenField ID="hdnBelgeTur" runat="server" />
    <div class="form" style="margin-top: -2px;">
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMZFG033 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 100px">
                <asp:DropDownList ID="ddlYil" runat="server">
                </asp:DropDownList>
            </div>
            <div class="kolonArasi">
            </div>
            <div class="yaziAlanK" style="width: 100px">
                <%= Resources.TasinirMal.FRMZFG034 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 100px">
                <asp:TextBox ID="txtBelgeTarih" Columns="10" runat="server" MaxLength="10" CssClass="veriAlan"></asp:TextBox>
                <img src="../App_themes/Images/takvim.gif" id="imgBelgeTarihi" alt="" style="cursor: pointer;"
                    onclick="JavaScript:displayDatePicker('txtBelgeTarih', false, 'dmy', '.');" />
            </div>
            <div class="yaziAlanK" style="width: 100px">
                <%= Resources.TasinirMal.FRMZFG035 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtBelgeNo" Columns="7" runat="server" MaxLength="6" CssClass="veriAlan"></asp:TextBox>
                &nbsp;
                <asp:ImageButton ID="btnListele" TabIndex="99" runat="server" Width="16" ImageUrl="../App_themes/images/bul.gif"
                    Height="16" AlternateText="<%$ Resources:TasinirMal, FRMZFG036 %>" OnClick="btnListele_Click">
                </asp:ImageButton>
            </div>
            <div class="veriAlan">
                &nbsp;<asp:Label ID="lblFormDurum" runat="server"></asp:Label></div> 
            <br />
            <div class="bilgiAlan">
                &nbsp;<asp:Label ID="lbleOnayDurum" runat="server"></asp:Label></div>
        </div>
        <div id="divMHA0" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMZFG037 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMZFG038 %>" style="height: 14px;
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
                    <%= Resources.TasinirMal.FRMZFG039 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMZFG040 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimi','txtMuhasebe','lblHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMZFG041 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtAmbar" runat="server" CssClass="veriAlan" MaxLength="3" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMZFG042 %>" style="height: 14px;
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
                <%= Resources.TasinirMal.FRMZFG043 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 200px">
                <asp:DropDownList ID="ddlZimmetVermeDusme" runat="server" CssClass="veriAlanDDL"
                    Width="200px" AutoPostBack="true" OnSelectedIndexChanged="ddlZimmetVermeDusme_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
        </div>
        <div class="row" id="divZimmetFisiTipi" runat="server">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMZFG044 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 200px">
                <asp:DropDownList ID="ddlZimmetFisiTipi" runat="server" CssClass="veriAlanDDL" Width="200px">
                </asp:DropDownList>
            </div>
            <div class="kolonArasi">
            </div>
        </div>
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMZFG045 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtKimeGitti" runat="server" CssClass="veriAlan" MaxLength="11"
                    Width="100px"></asp:TextBox></div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMZFG046 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListePersonel.aspx','txtKimeGitti','txtHarcamaBirimi','lblKimeGittiAd');return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" /></div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblKimeGittiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="row" id="divNereyeVerildi" runat="server">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMZFG047 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtNereyeGitti" runat="server" CssClass="veriAlan" MaxLength="10"
                    Width="100px"></asp:TextBox></div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMZFG048 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeOda.aspx','txtNereyeGitti','txtHarcamaBirimi','lblNereyeGittiAd');return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" /></div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblNereyeGittiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <table width="100%">
        <tr>
            <td style="width: 100%">
                <table width="430">
                    <tr>
                        <td style="vertical-align: top">
                            <asp:Button ID="btnKaydet" TabIndex="99" runat="server" Width="100" CssClass="dugme"
                                Text="<%$ Resources:TasinirMal, FRMZFG049 %>" OnClick="btnKaydet_Click"></asp:Button>&nbsp;
                            <br />
                            <%--                            <asp:CheckBox ID="chkeOnay" runat="server" Text="ePosta Gönder" Checked="true" />--%>
                        </td>
                        <td style="vertical-align: top">
                            <button class="dugme" id="btnYazdir" style="width: 100px" onclick="BelgeYazdirGoster();return false;"
                                tabindex="99" type="button">
                                <%= Resources.TasinirMal.FRMZFG050 %></button>&nbsp;
                            <br />
                            <asp:CheckBox ID="chkResim" runat="server" Text="Resim yazdır" />
                        </td>
                        <td style="vertical-align: top">
                            <asp:Button ID="btnTemizle" TabIndex="99" runat="server" Width="100" CssClass="dugme"
                                Text="<%$ Resources:TasinirMal, FRMZFG051 %>" OnClick="btnTemizle_Click"></asp:Button>&nbsp;
                        </td>
                        <td style="vertical-align: top">
                            <asp:Button ID="btnBarkodYazdir" TabIndex="99" runat="server" Width="90" CssClass="dugme"
                                Text="<%$ Resources:TasinirMal, FRMZFG052 %>" OnClientClick="BarkodEkraniAc();return false;" />
                        </td>
                        <td style="vertical-align: top">
                            <asp:Button ID="btnEOnayGonder" TabIndex="99" runat="server" Width="90" CssClass="dugme"
                                Text="<%$ Resources:TasinirMal, FRMZFG057 %>" OnClick="btnEOnayGonder_Click" />
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 12%; vertical-align: top">
                <asp:Button ID="btnOnayla" TabIndex="99" runat="server" Width="100" CssClass="dugme"
                    Text="<%$ Resources:TasinirMal, FRMZFG053 %>" OnClick="btnOnayla_Click"></asp:Button>&nbsp;
            </td>
            <td style="width: 12%; vertical-align: top">
                <asp:Button ID="btnOnayKaldir" TabIndex="99" runat="server" Width="110" CssClass="dugme"
                    Text="<%$ Resources:TasinirMal, FRMZFG054 %>" OnClick="btnOnayKaldir_Click">
                </asp:Button>&nbsp;
            </td>
        </tr>
    </table>
    <farpoint:FpSpread ID="fpL" runat="server" Width="100%" Height="450" OnSaveOrLoadSheetState="fpL_SaveOrLoadSheetState"
        Style="position: relative;">
    </farpoint:FpSpread>
    <div id="divBekle" class="bekleKutusu">
        <br />
        <b>
            <%= Resources.TasinirMal.FRMZFG055 %></b>
        <br />
        <br />
        <img alt="<%= Resources.TasinirMal.FRMZFG056 %>" src="../App_themes/images/loading.gif" />
        <br />
        <br />
    </div>
    <iframe id="frmBelgeYazdir" frameborder="0" scrolling="no" width="1" height="1">
    </iframe>
    </form>
</body>
</html>
