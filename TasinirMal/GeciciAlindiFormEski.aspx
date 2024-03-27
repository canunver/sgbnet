<%@ Page Language="C#" CodeBehind="GeciciAlindiFormEski.aspx.cs" Inherits="TasinirMal.GeciciAlindiFormEski" %>

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
    <script language="JavaScript" type="text/javascript" src="../script/paraFormat.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/GeciciAlindiForm.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Defterler.js?v=1"></script>
</head>
<body onunload="HesapPlaniKapat();">
    <form id="form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server">
    </ext:ResourceManager>
    <input type="hidden" id="hdnFirmaHarcamadanAlma" runat="server" />
    <div class="form" style="margin-top: -2px;">
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMGAG021 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 100px">
                <asp:DropDownList ID="ddlYil" runat="server">
                </asp:DropDownList>
            </div>
            <div class="kolonArasi">
            </div>
            <div class="yaziAlanK" style="width: 100px">
                <%= Resources.TasinirMal.FRMGAG022 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 100px">
                <asp:TextBox ID="txtBelgeTarih" Columns="10" runat="server" MaxLength="10" CssClass="veriAlan"></asp:TextBox>
                <img src="../App_themes/Images/takvim.gif" id="imgBelgeTarihi" alt="" style="cursor: pointer;"
                    onclick="JavaScript:displayDatePicker('txtBelgeTarih', false, 'dmy', '.');" />
            </div>
            <div class="yaziAlanK" style="width: 100px">
                <%= Resources.TasinirMal.FRMGAG023 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtBelgeNo" Columns="7" runat="server" MaxLength="6" CssClass="veriAlan"></asp:TextBox>
                &nbsp;
                <asp:ImageButton ID="btnListele" TabIndex="99" runat="server" Width="16" ImageUrl="../App_themes/images/bul.gif"
                    Height="16" AlternateText="<%$ Resources:TasinirMal, FRMGAG024 %>" OnClick="btnListele_Click">
                </asp:ImageButton>
            </div>
        </div>
        <div id="divMHA0" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMGAG025 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox>
                </div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMGAG026 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript: ListeAc('ListeMuhasebe.aspx', 'txtMuhasebe', '', 'lblMuhasebeAd'); return false;"
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
                    <%= Resources.TasinirMal.FRMGAG027 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox>
                </div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMGAG028 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript: ListeAc('ListeHarcamaBirimi.aspx', 'txtHarcamaBirimi', 'txtMuhasebe', 'lblHarcamaBirimiAd'); return false;"
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
                    <%= Resources.TasinirMal.FRMGAG029 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtAmbar" runat="server" CssClass="veriAlan" MaxLength="3" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMGAG030 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript: ListeAc('ListeAmbar.aspx', 'txtAmbar', 'txtHarcamaBirimi', 'lblAmbarAd'); return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblAmbarAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                </div>
            </div>
        </div>
       
        <div id="divNeredenGeldi" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMGAG031 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtNeredenGeldi" runat="server" CssClass="veriAlan" MaxLength="255"
                        Columns="55"></asp:TextBox>
                    <input type="image" alt="<%= Resources.TasinirMal.FRMGAG032 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript: FirmaGoster(); return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />&nbsp;
                </div>
                <div class="kolonArasi">
                </div>
            </div>
        </div>
        <div id="divFatura" runat="server">
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMGAG033 %></div>
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
                    <%= Resources.TasinirMal.FRMGAG034 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 100px">
                    <asp:TextBox ID="txtFaturaNo" runat="server" CssClass="veriAlan" MaxLength="10" Columns="10"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <table width="100%">
        <tr>
            <td style="width: 100%">
                <table>
                    <tr>
                        <td style="width: 90%">
                            <asp:Button ID="btnKaydet" TabIndex="99" runat="server" Width="100" CssClass="dugme"
                                Text="<%$ Resources:TasinirMal, FRMGAG035 %>" OnClick="btnKaydet_Click"></asp:Button>&nbsp;
                            <asp:Button ID="btnGeciciAlindi" TabIndex="99" runat="server" Width="120" CssClass="dugme"
                                Text="<%$ Resources:TasinirMal, FRMGAG036 %>" OnClick="btnGeciciAlindi_Click">
                            </asp:Button>&nbsp;
                            <asp:Button runat="server" ID="btnTIF" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMGAG037 %>"
                                Width="100" OnClick="btnTIF_Click" />
                            <asp:Button ID="btnTemizle" TabIndex="99" runat="server" Width="100" CssClass="dugme"
                                Text="<%$ Resources:TasinirMal, FRMGAG038 %>" OnClick="btnTemizle_Click"></asp:Button>&nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <farpoint:FpSpread ID="fpL" runat="server" Width="100%" Height="450" OnSaveOrLoadSheetState="fpL_SaveOrLoadSheetState"
                    Style="position: relative;">
                </farpoint:FpSpread>
            </td>
        </tr>
    </table>
    <iframe id="frmBelgeYazdir" frameborder="0" scrolling="no" width="1" height="1">
    </iframe>
    <div id="divBekle" class="bekleKutusu">
        <br />
        <b>
            <%= Resources.TasinirMal.FRMGAG039 %></b>
        <br />
        <br />
        <img alt="<%= Resources.TasinirMal.FRMGAG040 %>" src="../App_themes/images/loading.gif" />
        <br />
        <br />
    </div>
    </form>
</body>
</html>
