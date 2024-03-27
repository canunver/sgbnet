<%@ Page Language="C#" CodeBehind="TasinirOdemeEmri.aspx.cs" Inherits="TasinirMal.TasinirOdemeEmri" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<%@ Register TagPrefix="farpoint" Namespace="FarPoint.Web.Spread" Assembly="FarPoint.Web.Spread" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=3"></script>
    <script language="JavaScript" type="text/javascript">
        function grdOEKomut(komut, rec, kolon) {
            if (komut == "TaahhutSec") {
                document.getElementById('hdnTaahhutKod').value = rec.data.KOD;
                lblTaahhut.setValue(rec.data.PROJENO);

                wndTaahhut.hide();
            }
        }

        function TaahhutPencersiniAc() {
            var ilgili = document.getElementById('txtIlgiliNo').value;

            if (ilgili == "") {
                Ext1.Msg.alert("Uyarı", "Lütfen İlgili bilgisini girin");
                return;
            }

            Ext1.net.DirectMethods.TaahhutDosyasiDoldur();

            wndTaahhut.show();
        }

        function MahsupGosterGoster() {
            var hesapKod = "";
            var altHesapKod = "";
            var altHesapAdi = "";

            hesapKod = document.getElementById('txtHesapKod').value;
            altHesapKod = document.getElementById('txtAltHesapKod').value;

            var yil = document.getElementById('hdnYil').value;
            var muhasebe = document.getElementById("hdnMuhasebe").value;
            var birim = document.getElementById("hdnBirim").value;
            var ilgiliNo = altHesapKod; // document.getElementById("txtIlgiliNo").value;
            var ilgiliAd = ""; //document.getElementById("txtIlgiliAd").value;

            showPopWin("../ButceMuhasebe/ListeMahsupBelge.aspx?menuYok=1&yil=" + yil + "&ilgiliAd=" + ilgiliAd + "&ilgiliNo=" + ilgiliNo + "&muhasebe=" + muhasebe + "&birim=" + birim + "&hesapKod=" + hesapKod, 780, 510, true, null);
        }

        function MahsupSecKapatBasildi(mahsupBelgeNo, tutar) {

            document.getElementById('txtMahsupBelgeNo').value = mahsupBelgeNo.belgeNo;

            hidePopWin();
        }
    </script>
</head>
<body>
    <form id="aspform" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <%--<Listeners>
                <DocumentReady Handler="ddlBirim.onTriggerClick();Panel1.hide();" />
            </Listeners>--%>
        </ext:ResourceManager>
        <input type="hidden" id="hdnYil" runat="server" />
        <input type="hidden" id="hdnBelgeNo" runat="server" />
        <input type="hidden" id="hdnMuhasebe" runat="server" />
        <input type="hidden" id="hdnBirim" runat="server" />
        <input type="hidden" id="hdnKur" runat="server" />
        <input type="hidden" id="hdnBelgeTarih" runat="server" />
        <input type="hidden" id="hdnToplamBorc" runat="server" />
        <input type="hidden" id="hdnToplamAlacak" runat="server" />
        <input type="hidden" id="hdnTaahhutKod" runat="server" />
        <div class="form">
            <div id="divAnaBilgi" runat="server">
                <div class="row">
                    <div class="yaziAlanK">
                        <%= Resources.TasinirMal.FRMTOE030 %>
                    </div>
                    <div class="ikiNokta">
                        :
                    </div>
                    <div class="veriAlan" style="width: 100px">
                        <asp:DropDownList ID="ddlYil" runat="server">
                        </asp:DropDownList>
                    </div>
                </div>
                <div id="divMHA0" runat="server">
                    <div class="row">
                        <div class="yaziAlanK">
                            <%= Resources.TasinirMal.FRMTOE031 %>
                        </div>
                        <div class="ikiNokta">
                            :
                        </div>
                        <div class="veriAlan">
                            <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox>
                        </div>
                        <div class="veriAlan">
                            <input type="image" alt="<%= Resources.TasinirMal.FRMTOE032 %>" style="height: 14px; width: 16px;"
                                class="veriAlan" onclick="javascript: ListeAc('ListeMuhasebe.aspx', 'txtMuhasebe', '', 'lblMuhasebeAd'); return false;"
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
                            <%= Resources.TasinirMal.FRMTOE033 %>
                        </div>
                        <div class="ikiNokta">
                            :
                        </div>
                        <div class="veriAlan">
                            <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                                Width="100px"></asp:TextBox>
                        </div>
                        <div class="veriAlan">
                            <input type="image" alt="<%= Resources.TasinirMal.FRMTOE034 %>" style="height: 14px; width: 16px;"
                                class="veriAlan" onclick="javascript: ListeAc('ListeHarcamaBirimi.aspx', 'txtHarcamaBirimi', 'txtMuhasebe', 'lblHarcamaBirimiAd'); return false;"
                                tabindex="100" src="../App_themes/images/bul1.gif" />
                        </div>
                        <div class="kolonArasi">
                        </div>
                        <div class="veriAlan">
                            <asp:Label ID="lblHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                        </div>
                    </div>
                </div>
                <div class="row" id="divOrg0" runat="server" style="display: none">
                    <div class="yaziAlanK">
                        <%= Resources.TasinirMal.FRMORG001 %>
                    </div>
                    <div class="ikiNokta">
                        :
                    </div>
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
                    <div class="yaziAlanK">
                        <%= Resources.TasinirMal.FRMTOE035 %>
                    </div>
                    <div class="ikiNokta">
                        :
                    </div>
                    <div class="veriAlan">
                        <asp:TextBox ID="txtBelgeNo" runat="server" CssClass="veriAlan" MaxLength="6" Columns="7"></asp:TextBox>&nbsp;
                    <asp:ImageButton ID="btnListele" TabIndex="99" runat="server" Width="16" ImageUrl="../App_themes/images/bul.gif"
                        Height="16" AlternateText="<%$ Resources:TasinirMal, FRMTOE036 %>" OnClick="btnListele_Click"></asp:ImageButton>
                    </div>
                </div>
            </div>
            <div id="divEkBilgi" runat="server">
                <div class="row" style="height: 30px;">
                    <div class="veriAlan">
                        <asp:Label ID="lblMIFNo" runat="server" Width="100%"></asp:Label>
                    </div>
                </div>
                <div class="row">
                    <div class="yaziAlan">
                        <asp:Label ID="lblHesapKod" runat="server" Text="<%$ Resources:TasinirMal, FRMTOE037 %>"></asp:Label>
                    </div>
                    <div class="ikiNokta">
                        :
                    </div>
                    <div class="veriAlan">
                        <asp:TextBox ID="txtHesapKod" runat="server" CssClass="veriAlan" Columns="30"></asp:TextBox>&nbsp;
                    </div>
                    <div class="veriAlan">
                        <input type="image" alt="<%= Resources.TasinirMal.FRMTOE037 %>" style="height: 14px; width: 16px;"
                            class="veriAlan" onclick="OdemeHesapKodListele(); return false;"
                            tabindex="100" src="../App_themes/images/bul1.gif" />
                    </div>
                    <div class="kolonArasi">
                    </div>
                    <div class="veriAlan">
                        <asp:Label ID="lblHesapPlanAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                    </div>
                </div>
                <div class="row">
                    <div class="yaziAlan">
                        <asp:Label ID="Label1" runat="server" Text="Alt Hesap Kodu"></asp:Label>
                    </div>
                    <div class="ikiNokta">
                        :
                    </div>
                    <div class="veriAlan">
                        <asp:TextBox ID="txtAltHesapKod" runat="server" CssClass="veriAlan" Columns="30"></asp:TextBox>&nbsp;
                    </div>
                    <div class="veriAlan">
                        <input type="image" alt="" style="height: 14px; width: 16px;" class="veriAlan" onclick="AltHesapGosterGoster('txtAltHesapKod'); return false;"
                            tabindex="100" src="../App_themes/images/bul1.gif" />
                    </div>
                    <div class="kolonArasi">
                    </div>
                    <div class="veriAlan">
                        <asp:Label ID="Label2" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                    </div>
                </div>
                <div class="row">
                    <div class="yaziAlan">
                        <asp:Label ID="Label3" runat="server" Text="Ödeme Hesabı Tertip"></asp:Label>
                    </div>
                    <div class="ikiNokta">
                        :
                    </div>
                    <div class="veriAlan">
                        <asp:TextBox ID="txtOdemeTertip" runat="server" CssClass="veriAlan" Columns="30"></asp:TextBox>&nbsp;
                    </div>
                    <div class="veriAlan">
                        <input type="image" alt="" style="height: 14px; width: 16px;" class="veriAlan" onclick="TertipGosterToText(); return false;"
                            tabindex="100" src="../App_themes/images/bul1.gif" />
                    </div>
                    <div class="kolonArasi">
                    </div>
                    <div class="veriAlan">
                        <asp:Label ID="Label4" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                    </div>
                </div>
                <div id="divBilgiler" runat="server">
                    <div class="row">
                        <div class="yaziAlan">
                            <asp:Label ID="Label5" runat="server" Text="Mahsup Belge No"></asp:Label>
                        </div>
                        <div class="ikiNokta">
                            :
                        </div>
                        <div class="veriAlan">
                            <asp:TextBox ID="txtMahsupBelgeNo" runat="server" CssClass="veriAlan" Columns="30"></asp:TextBox>&nbsp;
                        </div>
                        <div class="veriAlan">
                            <input type="image" alt="" style="height: 14px; width: 16px;" class="veriAlan" onclick="MahsupGosterGoster(); return false;"
                                tabindex="100" src="../App_themes/images/bul1.gif" />
                        </div>
                        <div class="kolonArasi">
                        </div>
                        <div class="veriAlan">
                            <asp:Label ID="Label6" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="yaziAlan">
                            <%= Resources.TasinirMal.FRMTOE038 %>
                        </div>
                        <div class="ikiNokta">
                            :
                        </div>
                        <div class="veriAlan">
                            <asp:TextBox ID="txtFaturaTarih" runat="server" MaxLength="255" Columns="30" CssClass="veriAlan"></asp:TextBox>
                        </div>
                        <div class="yaziAlan">
                            <%= Resources.TasinirMal.FRMTOE039 %>
                        </div>
                        <div class="ikiNokta">
                            :
                        </div>
                        <div class="veriAlan">
                            <asp:TextBox ID="txtFaturaNo" runat="server" MaxLength="255" Columns="30" CssClass="veriAlan"></asp:TextBox>
                        </div>
                    </div>
                    <div id="divTabILG" style="width: 100%; height: 130px;">
                        <div class="row">
                            <div class="yaziAlan">
                                <%= Resources.TasinirMal.FRMTOE040 %>
                            </div>
                            <div class="ikiNokta">
                                :
                            </div>
                            <div class="veriAlan">
                                <asp:TextBox ID="txtIlgiliAd" runat="server" MaxLength="255" Columns="90" CssClass="veriAlan"></asp:TextBox>
                                <input type="image" alt="<%= Resources.TasinirMal.FRMTOE041 %>" style="height: 14px; width: 16px;"
                                    class="veriAlan" onclick="javascript: AltHesapGosterGoster(''); return false;"
                                    tabindex="100" src="../App_themes/images/bul1.gif" />&nbsp;
                            </div>
                        </div>
                        <div class="row">
                            <div class="yaziAlan">
                                <%= Resources.TasinirMal.FRMTOE042 %>
                            </div>
                            <div class="ikiNokta">
                                :
                            </div>
                            <div class="veriAlan">
                                <asp:TextBox ID="txtIlgiliNo" runat="server" MaxLength="255" Columns="30" CssClass="veriAlan"></asp:TextBox>
                            </div>
                            <div class="yaziAlan">
                                <%= Resources.TasinirMal.FRMTOE043 %>
                            </div>
                            <div class="ikiNokta">
                                :
                            </div>
                            <div class="veriAlan">
                                <asp:TextBox ID="txtIlgiliVD" runat="server" MaxLength="255" Columns="30" CssClass="veriAlan"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="yaziAlan">
                                <%= Resources.TasinirMal.FRMTOE044 %>
                            </div>
                            <div class="ikiNokta">
                                :
                            </div>
                            <div class="veriAlan">
                                <asp:TextBox ID="txtIlgiliBankaAd" runat="server" MaxLength="255" Columns="30" CssClass="veriAlan"></asp:TextBox>
                            </div>
                            <div class="yaziAlan">
                                <%= Resources.TasinirMal.FRMTOE045 %>
                            </div>
                            <div class="ikiNokta">
                                :
                            </div>
                            <div class="veriAlan">
                                <asp:TextBox ID="txtIlgiliBankaNo" runat="server" MaxLength="255" Columns="30" CssClass="veriAlan"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="yaziAlan">
                                <%= Resources.TasinirMal.FRMTOE046 %>
                            </div>
                            <div class="ikiNokta">
                                :
                            </div>
                            <div class="veriAlan" style="height: 60px">
                                <asp:TextBox ID="txtAciklama" runat="server" MaxLength="255" Columns="70" TextMode="MultiLine"
                                    Rows="3"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="yaziAlan">
                                Tevkifat Hesap Kodu
                            </div>
                            <div class="ikiNokta">
                                :
                            </div>
                            <div class="veriAlan">
                                <asp:DropDownList ID="ddlTevkifat" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="yaziAlan">
                                Tevkifat Tutarı
                            </div>
                            <div class="ikiNokta">
                                :
                            </div>
                            <div class="veriAlan">
                                <asp:TextBox ID="txtTevkifat" runat="server" MaxLength="255" Columns="20" CssClass="veriAlan"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="yaziAlan">
                            <%= Resources.TasinirMal.FRMTOE057 %>
                        </div>
                        <div class="ikiNokta">
                            :
                        </div>
                        <div class="veriAlan">
                            <asp:TextBox ID="txtKurFarkiHesap" runat="server" MaxLength="255" Columns="30" CssClass="veriAlan"></asp:TextBox>
                        </div>
                        <div class="yaziAlan">
                            <%= Resources.TasinirMal.FRMTOE058 %>
                        </div>
                        <div class="ikiNokta">
                            :
                        </div>
                        <div class="veriAlan">
                            <asp:TextBox ID="txtKurFarkiTutar" runat="server" MaxLength="255" Columns="30" CssClass="veriAlan"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="yaziAlan">
                        </div>
                        <div class="ikiNokta">
                        </div>
                        <div class="veriAlan" style="height: 30px">
                            <ext:Button ID="btnTaahhut" runat="server" Text="Taahhüt Dosyası seçimi" Icon="BookAdd"
                                OnClientClick="TaahhutPencersiniAc();">
                            </ext:Button>
                        </div>
                        <div class="veriAlan" style="width: 300px">
                            <ext:TextField ID="lblTaahhut" runat="server" Width="100px" ReadOnly="true" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="yaziAlan">
                        </div>
                        <div class="ikiNokta">
                        </div>
                        <div class="veriAlan">
                            <asp:CheckBox ID="chkDamga" runat="server" Text="<%$ Resources:TasinirMal, FRMTOE047 %>" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="footer">
                &nbsp;
            </div>
        </div>
        <table width="100%">
            <tr>
                <td style="width: 100%" align="center">
                    <asp:Button ID="btnMIFListele" TabIndex="99" runat="server" Width="140" CssClass="dugme"
                        Text="<%$ Resources:TasinirMal, FRMTOE048 %>" OnClick="btnMIFListele_Click"></asp:Button>&nbsp;
                <asp:Button ID="btnTemizle" TabIndex="99" runat="server" Width="140" CssClass="dugme"
                    Text="<%$ Resources:TasinirMal, FRMTOE049 %>" OnClick="btnTemizle_Click"></asp:Button>&nbsp;
                <asp:Button ID="btnOlustur" TabIndex="99" runat="server" Width="140" CssClass="dugme"
                    Text="<%$ Resources:TasinirMal, FRMTOE050 %>" OnClick="btnOlustur_Click"></asp:Button>&nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <farpoint:FpSpread ID="fpL" runat="server" Width="99%" Height="400" OnSaveOrLoadSheetState="fpL_SaveOrLoadSheetState"
                        Style="position: relative;">
                    </farpoint:FpSpread>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divListe" runat="server" style="overflow: auto; height: 370px; width: 700px">
                        <asp:GridView ID="gvListe" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                            Width="96%" AutoGenerateColumns="false">
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
                            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                            <Columns>
                                <asp:BoundField DataField="yil" HeaderText="<%$ Resources:TasinirMal, FRMTOE030 %>"
                                    ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="muhasebe" HeaderText="<%$ Resources:TasinirMal, FRMTOE051 %>"
                                    ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="harcamaBirim" HeaderText="<%$ Resources:TasinirMal, FRMTOE052 %>"
                                    ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="fisNo" HeaderText="<%$ Resources:TasinirMal, FRMTOE053 %>"
                                    ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="belgeNo" HeaderText="<%$ Resources:TasinirMal, FRMTOE054 %>"
                                    ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </td>
            </tr>
        </table>
        <div id="divBekle" class="bekleKutusu">
            <br />
            <b>
                <%= Resources.TasinirMal.FRMTOE055 %></b>
            <br />
            <br />
            <img alt="<%= Resources.TasinirMal.FRMTOE056 %>" src="../App_themes/images/loading.gif" />
            <br />
            <br />
        </div>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
        <ext:Window ID="wndTaahhut" runat="server" Width="570" Height="250" Layout="FitLayout"
            Title="Taahhüt Dosyası Seçimi" Modal="true" Hidden="true">
            <Items>
                <ext:GridPanel runat="server">
                    <TopBar>
                        <ext:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <ext:Button runat="server" ID="btnWndKapat" Text="Kapat" Icon="Decline">
                                    <Listeners>
                                        <Click Handler="wndTaahhut.hide();" />
                                    </Listeners>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <ColumnModel>
                        <Columns>
                            <ext:RowNumbererColumn />
                            <ext:ImageCommandColumn ColumnID="clmOnayli" Width="30">
                                <Commands>
                                    <ext:ImageCommand CommandName="TaahhutSec" Icon="Accept">
                                        <ToolTip Text="Bu taahhüt dosyasını kullan" />
                                    </ext:ImageCommand>
                                </Commands>
                            </ext:ImageCommandColumn>
                            <ext:Column DataIndex="PROJENO" ColumnID="PROJENO" Header="Proje No" Width="80">
                            </ext:Column>
                            <ext:Column DataIndex="SOZLESMETARIH" ColumnID="SOZLESMETARIH" Header="Söz. Tarih"
                                Width="80">
                            </ext:Column>
                            <ext:Column DataIndex="KONU" ColumnID="KONU" Header="Konu" Width="210">
                            </ext:Column>
                            <ext:Column DataIndex="IHALETUTAR" ColumnID="IHALETUTAR" Header="İhale Bedeli" Width="100"
                                Align="Right">
                            </ext:Column>
                        </Columns>
                    </ColumnModel>
                    <Listeners>
                        <Command Fn="grdOEKomut" />
                    </Listeners>
                    <Store>
                        <ext:Store ID="storeTaahhut" runat="server">
                            <Reader>
                                <ext:JsonReader>
                                    <Fields>
                                        <ext:RecordField Name="KOD" />
                                        <ext:RecordField Name="PROJENO" />
                                        <ext:RecordField Name="KONU" />
                                        <ext:RecordField Name="SOZLESMETARIH" />
                                        <ext:RecordField Name="IHALETUTAR" />
                                    </Fields>
                                </ext:JsonReader>
                            </Reader>
                        </ext:Store>
                    </Store>
                </ext:GridPanel>
            </Items>
        </ext:Window>
    </form>
</body>
</html>
