<%@ Page Language="C#" CodeBehind="Giris.aspx.cs" Inherits="OrtakSayfa.Giris" Culture="auto:tr-TR"
    UICulture="auto:tr-TR" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="App_Themes/Theme1/Theme1.css?v=20150204" type="text/css" rel="stylesheet" />

    <script language="javascript" type="text/javascript" src="script/GirisDogrula.js?v=2022_10_12"></script>

    <script language="javascript" type="text/javascript" src="script/CapsLock.js?mc=03022015&v=2011_11_28"></script>
    <script language="javascript" type="text/javascript" src="Script/jquery/jquery-latest.min.js?mc=03022017"></script>
    <script language="javascript" type="text/javascript" src="Script/jQuery/ui/jquery-ui-custom-latest.min.js?mc=03022016"></script>
    <script language="javascript" type="text/javascript" src="Script/genel.js?mc=20171207"></script>
    <script language="javascript" type="text/javascript" src="Script/jquery/dialog/modalPopUp.js?mc=03122015"></script>
    <script language="javascript" type="text/javascript">
        try {
            window.moveTo(0, 0);
            window.resizeTo(window.screen.availWidth, window.screen.availHeight);
        }
        catch (e) { }

        try {
            if (window.top && window.top.location.href != window.location.href)
                window.top.location.href = window.location.href;
        } catch (e) { }

        function SifremiUnuttum() {
            showPopWin("OrtakSayfa/SifremiUnuttum.aspx?menuYok=1", 450, 200, false, null);
        }

        function sf() {
            try {
                if (document.getElementById('txtKKod').value == "")
                    document.getElementById('txtKKod').focus();
                else
                    document.getElementById('txtParola').focus();
            }
            catch (e) { }
        }

        function HataGoster() {
            var obj = document.getElementById('divHata');
            obj.style.visibility = 'visible';
        }

        function DosyaGoster(param) {
            var dosyaPath = "Dosyalar/AnaSayfa/";
            var dosyaAd = "";
            if (param == "b1") { dosyaAd = "2010_PBOdenekCetveliRaporlari.pdf" }
            else if (param == "b2") { dosyaAd = "2009_PBOdenekCetveliRaporlari.pdf" }
            else if (param == "b3") { dosyaAd = "2008_PBOdenekCetveliRaporlari.pdf" }
            else if (param == "f1") { dosyaAd = "2009_YiliFaaliyetRaporu.pdf" }
            else if (param == "f2") { dosyaAd = "2008_YiliFaaliyetRaporu.pdf" }
            else if (param == "f3") { dosyaAd = "2007_YiliFaaliyetRaporu.pdf" }
            else if (param == "p1") { dosyaAd = "2010_YiliPerformansProgrami.pdf" }
            else if (param == "p2") { dosyaAd = "2009_YiliPerformansProgrami.pdf" }
            else if (param == "p3") { dosyaAd = "2008_YiliPerformansProgrami.pdf" }
            else if (param == "k1") { dosyaAd = "2009_KesinHesap.pdf" }
            else if (param == "k2") { dosyaAd = "2008_KesinHesap.pdf" }
            else if (param == "k3") { dosyaAd = "2007_KesinHesap.pdf" }

            if (dosyaAd != "") {
                window.location.href = dosyaPath + dosyaAd;
            }
            else {
                alert('Dosya bulunamadı.');
            }
        }

        function ResimYenile() {
            var rnd = Math.floor((Math.random() * 10000) + 1);
            $("#imgGuvenlikResmi").attr("src", "OrtakSayfa/GuvenlikResmi.aspx?v=" + rnd);
        }
    </script>
    <style type="text/css">
        body {
            background: none;
        }

        .bilgiWin .x-window-body {
            background-color: white;
        }
    </style>
</head>
<body onkeypress="capsLockKontrol(event);">
    <form id="Form1" runat="server">
        <asp:Button UseSubmitBehavior="false" ID="btnGirisASP" runat="server" TabIndex="1000" Text="..."
            OnClick="btnGiris_Click" Hidden="true"></asp:Button>
        <ext:Hidden runat="server" ID="hdnIkiAsamaliGiris" />
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="sf" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Window ID="winAna" runat="server" Width="800" Height="600" Layout="BorderLayout"
            Closable="false" Resizable="false" Header="false" Draggable="false">
            <Items>
                <ext:Panel ID="divUstMenu" runat="server" Height="80" Region="North" Border="true"
                    Layout="FitLayout" Margins="5 5 0 5">
                    <Content>
                        <div class='logo'>
                            <a href="#">.</a>
                        </div>
                    </Content>
                </ext:Panel>
                <ext:Panel ID="divSolMenu" runat="server" Width="250" Layout="Accordion" Split="true"
                    Region="West" Margins="5 0 5 0" Border="false">
                    <Items>
                        <ext:Panel ID="divSifresizBolum" runat="server" Title="<%$Resources:Istemci,IST024%>"
                            Icon="PageWhiteEdit" HideBorders="true">
                            <Content>
                                <div style="padding: 5px 5px 5px 5px">
                                    <p>
                                        <b>*</b>&nbsp;
                                    <%=Resources.Istemci.IST051%><a tabindex="-1" href="http://uygulama.sgb.gov.tr/SGBnettanitim.pdf">
                                        <%=Resources.Istemci.IST052%></a>
                                    </p>
                                    <p>
                                        &nbsp;
                                    </p>
                                    <p>
                                        <b>*</b>&nbsp;<%=Resources.Istemci.IST026%>
                                    </p>
                                </div>
                                <div style="width: 300px; overflow: hidden">
                                    <table cellspacing="2" cellpadding="2" border="0" width="300px">
                                        <tr>
                                            <td style="vertical-align: top;" align="left">
                                                <table>
                                                    <tr id="trEvrak" runat="server">
                                                        <td>
                                                            <img src="app_themes\images\top.gif" alt="" />
                                                        </td>
                                                        <td colspan="2">
                                                            <a href="Evrak/EvrakAra.aspx" tabindex="-1" target="_blank">
                                                                <%=Resources.Istemci.IST028%></a>
                                                        </td>
                                                    </tr>
                                                    <tr id="trZimmet" runat="server">
                                                        <td>
                                                            <img src="app_themes\images\top.gif" alt="" />
                                                        </td>
                                                        <td colspan="2">
                                                            <a href="TasinirMal/ZimmetKisiListele.aspx" tabindex="-1" target="_blank">
                                                                <%=Resources.Istemci.IST029%></a>
                                                        </td>
                                                    </tr>
                                                    <tr id="trZimmetePostaOnay" runat="server">
                                                        <td>
                                                            <img src="app_themes\images\top.gif" alt="" />
                                                        </td>
                                                        <td colspan="2">
                                                            <a href="TasinirMal/ZimmetePostaOnayKisiListele.aspx" tabindex="-1" target="_blank">
                                                                <%=Resources.Istemci.IST055%></a>
                                                        </td>
                                                    </tr>
                                                    <tr id="trTalepKarsilama" runat="server">
                                                        <td>
                                                            <img src="app_themes\images\top.gif" alt="" />
                                                        </td>
                                                        <td colspan="2">
                                                            <a href="TasinirMal/TalepKarsilamaAna.aspx" tabindex="-1" target="_blank">
                                                                <%=Resources.Istemci.IST039%></a>
                                                        </td>
                                                    </tr>
                                                    <tr id="trKutuphane" runat="server">
                                                        <td>
                                                            <img src="app_themes\images\top.gif" alt="" />
                                                        </td>
                                                        <td colspan="2">
                                                            <a href="Kutuphane/YayinArama.aspx" tabindex="-1" target="_blank">
                                                                <%=Resources.Istemci.IST030%></a>
                                                        </td>
                                                    </tr>
                                                    <tr id="trYolluk" runat="server">
                                                        <td>
                                                            <img src="app_themes\images\top.gif" alt="" />
                                                        </td>
                                                        <td colspan="2">
                                                            <a href="#" tabindex="-1">
                                                                <%=Resources.Istemci.IST031%></a>
                                                        </td>
                                                    </tr>
                                                    <tr id="trYollukYIS" runat="server">
                                                        <td>&nbsp;
                                                        </td>
                                                        <td>
                                                            <img src="app_themes\images\top.gif" alt="" />
                                                        </td>
                                                        <td>
                                                            <a href="harcamaSurecleri/PSLSurekliGorevYolluk.aspx?tur=1&menuYok=1" tabindex="-1"
                                                                target="_blank">
                                                                <%=Resources.Istemci.IST032%></a>
                                                        </td>
                                                    </tr>
                                                    <tr id="trYollukYDS" runat="server">
                                                        <td>&nbsp;
                                                        </td>
                                                        <td>
                                                            <img src="app_themes\images\top.gif" alt="" />
                                                        </td>
                                                        <td>
                                                            <a href="harcamaSurecleri/PSLSurekliGorevYolluk.aspx?tur=2&menuYok=1" tabindex="-1"
                                                                target="_blank">
                                                                <%=Resources.Istemci.IST033%></a>
                                                        </td>
                                                    </tr>
                                                    <tr id="trYollukYIG" runat="server">
                                                        <td>&nbsp;
                                                        </td>
                                                        <td>
                                                            <img src="app_themes\images\top.gif" alt="" />
                                                        </td>
                                                        <td>
                                                            <a href="harcamaSurecleri/PSLGeciciGorevYolluk.aspx?tur=1&menuYok=1" tabindex="-1"
                                                                target="_blank">
                                                                <%=Resources.Istemci.IST034%></a>
                                                        </td>
                                                    </tr>
                                                    <tr id="trYollukYDG" runat="server">
                                                        <td>&nbsp;
                                                        </td>
                                                        <td>
                                                            <img src="app_themes\images\top.gif" alt="" />
                                                        </td>
                                                        <td>
                                                            <a href="harcamaSurecleri/PSLGeciciGorevYolluk.aspx?tur=3&menuYok=1" tabindex="-1"
                                                                target="_blank">
                                                                <%=Resources.Istemci.IST035%></a>
                                                        </td>
                                                    </tr>
                                                    <tr id="trYollukDEG" runat="server">
                                                        <td>&nbsp;
                                                        </td>
                                                        <td>
                                                            <img src="app_themes\images\top.gif" alt="" />
                                                        </td>
                                                        <td>
                                                            <a href="harcamaSurecleri/PSLGeciciGorevYolluk.aspx?tur=2&menuYok=1" tabindex="-1"
                                                                target="_blank">
                                                                <%=Resources.Istemci.IST036%></a>
                                                        </td>
                                                    </tr>
                                                    <tr id="trIzin" runat="server">
                                                        <td>
                                                            <img src="app_themes\images\top.gif" alt="" />
                                                        </td>
                                                        <td colspan="2">
                                                            <a href="Personelv2/PersonelIzin.aspx" tabindex="-1" target="_blank">
                                                                <%=Resources.Istemci.IST050%></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </Content>
                        </ext:Panel>
                        <ext:Panel ID="divRaporBolum" runat="server" Title="<%$Resources:Istemci,IST027%>"
                            Icon="Report" HideBorders="true">
                            <Content>
                                <div style="width: 300px; overflow: hidden">
                                    <table cellspacing="2" cellpadding="2" border="0" width="300px">
                                        <tr>
                                            <td>Bütçe<br />
                                                <a href='#' tabindex="-1" onclick="DosyaGoster('b1');return false" target='_self'>2010
                                                Ödenek Cetvelleri</a><br />
                                                <a href='#' tabindex="-1" onclick="DosyaGoster('b2');return false" target='_self'>2009
                                                Ödenek Cetvelleri</a><br />
                                                <a href='#' tabindex="-1" onclick="DosyaGoster('b3');return false" target='_self'>2008
                                                Ödenek Cetvelleri</a>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Faaliyet Raporları<br />
                                                <a href='#' tabindex="-1" onclick="DosyaGoster('f1');return false" target='_self'>2009
                                                Yılı Faaliyet Raporları</a><br />
                                                <a href='#' tabindex="-1" onclick="DosyaGoster('f2');return false" target='_self'>2008
                                                Yılı Faaliyet Raporları</a><br />
                                                <a href='#' tabindex="-1" onclick="DosyaGoster('f3');return false" target='_self'>2007
                                                Yılı Faaliyet Raporları</a>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Performans Raporları<br />
                                                <a href='#' tabindex="-1" onclick="DosyaGoster('p1');return false" target='_self'>2010
                                                Yılı Performans Raporları</a><br />
                                                <a href='#' tabindex="-1" onclick="DosyaGoster('p2');return false" target='_self'>2009
                                                Yılı Performans Raporları</a><br />
                                                <a href='#' tabindex="-1" onclick="DosyaGoster('p3');return false" target='_self'>2008
                                                Yılı Performans Raporları</a>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Kesin Hesap<br />
                                                <a href='#' tabindex="-1" onclick="DosyaGoster('k1');return false" target='_self'>2009
                                                Yılı Kesin Hesap Cetvelleri</a><br />
                                                <a href='#' tabindex="-1" onclick="DosyaGoster('k2');return false" target='_self'>2008
                                                Yılı Kesin Hesap Cetvelleri</a><br />
                                                <a href='#' tabindex="-1" onclick="DosyaGoster('k3');return false" target='_self'>2007
                                                Yılı Kesin Hesap Cetvelleri</a>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </Content>
                        </ext:Panel>
                        <ext:Panel ID="divDokumanBolum" runat="server" Title="<%$Resources:Istemci,IST053%>"
                            Icon="PageWhite" HideBorders="true">
                            <Content>
                                <div style="width: 300px; overflow: hidden">
                                    <table cellspacing="2" cellpadding="2" border="0" width="300px">
                                        <tr>
                                            <td>&nbsp;
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <img src="app_themes\images\top.gif" alt="" />
                                            </td>
                                            <td>
                                                <a href='http://uygulama.sgb.gov.tr/Dosyalar/AnaSayfa/Sifre.xls' target='_self'>Şifre
                                                Talep Formu</a>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <img src="app_themes\images\top.gif" alt="" />
                                            </td>
                                            <td>
                                                <a href='http://uygulama.sgb.gov.tr/Dosyalar/AnaSayfa/Ambar.xls' target='_self'>Ambar
                                                Tanımlama Formu</a>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <img src="app_themes\images\top.gif" alt="" />
                                            </td>
                                            <td>
                                                <a href='http://uygulama.sgb.gov.tr/Dosyalar/AnaSayfa/KPS.doc' target='_self'>KPS Taahhütnamesi
                                                Formu</a>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <img src="app_themes\images\top.gif" alt="" />
                                            </td>
                                            <td>
                                                <a href='http://uygulama.sgb.gov.tr/Dosyalar/AnaSayfa/Vergi.doc' target='_self'>Vergi
                                                Borcu Sorgulama Taahhütnamesi Formu</a>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </Content>
                        </ext:Panel>
                    </Items>
                </ext:Panel>
                <ext:Panel ID="divSagMenu" runat="server" Region="Center" Margins="5 5 5 5" Border="true">
                    <Items>
                        <ext:Panel ID="Panel1" runat="server" Border="false" HideBorders="true" Height="220">
                            <Content>
                                <center>
                                    <div style="height: 10px">
                                    </div>
                                    <%--<ext:Image runat="server" ImageUrl="KYM/LogoGetir.aspx?tur=2" AlternateText="" /> --%>
                                    <ext:Image ID="Image1" runat="server" ImageUrl="App_themes/kurumlogo.jpg" AlternateText="" />
                                    <br />
                                    <ext:Label ID="lblKurumAdi" runat="server" StyleSpec="font-weight:bold" />
                                    <div style="height: 20px">
                                    </div>
                                    <div style="width: 60%">
                                        <%=Resources.Istemci.IST025%>
                                    </div>
                                    <br />
                                    <ext:Label ID="lblHata" runat="server" Text="<%$ Resources:Istemci,IST003%>" Icon="Exclamation"
                                        Hidden="true">
                                    </ext:Label>
                                    <br />
                                    <ext:Label ID="lblCapsHata" runat="server" Text="<%$ Resources:Istemci,IST005%>"
                                        Icon="Error" StyleSpec="visibility :hidden">
                                    </ext:Label>
                                </center>
                            </Content>
                        </ext:Panel>
                        <ext:FormPanel ID="frmGiris" runat="server" Border="false" Padding="5" Width="300"
                            Height="100" Style="margin-left: 100px">
                            <Items>
                                <ext:TextField ID="txtKKod" runat="server" MaxLength="50" Width="180" TabIndex="1"
                                    AutoFocus="true" FieldLabel="<%$ Resources:Istemci,IST004%>" AllowBlank="false">
                                    <CustomConfig>
                                        <ext:ConfigItem Name="autoCreate" Value="{tag: 'input', type: 'text', size: '20', autocomplete: 'off'}">
                                        </ext:ConfigItem>
                                    </CustomConfig>
                                </ext:TextField>
                                <ext:TextField ID="txtParola" runat="server" MaxLength="20" InputType="Password"
                                    Width="180" TabIndex="2" FieldLabel="<%$ Resources:Istemci,IST006%>" AllowBlank="false">
                                </ext:TextField>

                                <ext:Container ID="hdnGuvenlikResmi" runat="server" Layout="HBoxLayout" Hidden="true">
                                    <Items>
                                        <ext:TextField ID="txtRegisterGuvenlikKodu" runat="server" MaxLength="20"
                                            Width="180" TabIndex="2" FieldLabel="<%$ Resources:Istemci,IST056%>" AllowBlank="false">
                                        </ext:TextField>
                                        <ext:Button ID="btnRegisterRefresh" runat="server" Icon="Reload">
                                            <Listeners>
                                                <Click Handler="ResimYenile();return false;" />
                                            </Listeners>
                                        </ext:Button>

                                        <ext:Panel ID="Panel3" runat="server" Border="false" HideBorders="true" Height="220">
                                            <Content>
                                                <center>
                                                    <div style="float: left;">
                                                        &nbsp;
                                                        <asp:Image ID="imgGuvenlikResmi" runat="server" ImageUrl="OrtakSayfa/GuvenlikResmi.aspx" Width="75px"
                                                            Height="30px" />
                                                        &nbsp;
                                                    </div>
                                                    <div style="clear: both;">
                                                    </div>
                                                </center>
                                            </Content>
                                        </ext:Panel>
                                    </Items>
                                </ext:Container>
                            </Items>
                        </ext:FormPanel>
                        <ext:Panel ID="Panel2" runat="server" Border="false" HideBorders="true" Height="220">
                            <Content>
                                <center>
                                    <br />
                                    <ext:Panel ID="pnlDugme" runat="server" Width="200" Border="false" Layout="TableLayout">
                                        <Items>
                                            <ext:Button Type="Submit" ID="btnGirisKontrol" runat="server" TabIndex="3" Text="<%$ Resources:Istemci,IST001%>"
                                                Icon="Accept">
                                                <Listeners>
                                                    <Click Fn="KontrolDevam" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:DisplayField ID="lblBosluk" runat="server" Width="10">
                                            </ext:DisplayField>
                                            <ext:Checkbox TabIndex="-1" ID="chkHatirla" runat="server" BoxLabel="<%$ Resources:Istemci,IST008%>" />
                                        </Items>
                                    </ext:Panel>
                                    <br />
                                    <ext:LinkButton ID="lnkUnuttum" runat="server" Icon="UserComment" Text="<%$ Resources:Istemci,IST007%>">
                                        <Listeners>
                                            <Click Handler="SifremiUnuttum()" />
                                        </Listeners>
                                    </ext:LinkButton>
                                    <br />
                                    <br />
                                    <asp:Literal ID="ltlDil" runat="server"></asp:Literal>
                                    <br />
                                    <ext:Image ID="imgCallCenter" runat="server" ImageUrl="app_themes/images/cagrimerkezi.jpg"
                                        StyleSpec="position:absolute;bottom:10px;left:10px" Visible="false">
                                    </ext:Image>
                                    <ext:Label ID="lblSurum" runat="server" Text="" Icon="Computer" StyleSpec="position:absolute;bottom:10px;right:10px">
                                    </ext:Label>
                                </center>
                            </Content>
                        </ext:Panel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Window>
        <ext:Window ID="wndBilgi" Cls="bilgiWin" runat="server" Modal="false" Width="400" Height="400" X="10" Y="10" Closable="true" Floating="true" InitCenter="false" Title="Bilgi" Hidden="true">
            <Items>
                <ext:DisplayField ID="dspBilgi" StyleSpec="font-size: larger; font-weight: bold; padding:5px" runat="server" Html="" />
            </Items>
        </ext:Window>

        <ext:Window ID="wndIkinciSeviye" runat="server" Modal="true" Width="300" Height="200" Closable="true" Title="G&udblac;venlik &Scedil;ifresi" Hidden="true" Layout="FormLayout" LabelWidth="50" Padding="11">
            <Items>
                <ext:DisplayField runat="server" HideLabel="true" Html="<center>L&udblac;tfen telefonunuza ya da elektronik posta adresinize gelen kodu giriniz. Yenile tu&scedil;una basarak, kodun yeniden gelmesini sa&gbreve;layabilirsiniz.</center>"></ext:DisplayField>
                <ext:TextField ID="txtIniknciKod" runat="server" MaxLength="20"
                    Width="180" TabIndex="2" FieldLabel="Kod" AllowBlank="false">
                </ext:TextField>

                <ext:CompositeField runat="server">
                    <Items>
                        <ext:Button ID="Button1" runat="server" TabIndex="3" Text="<%$ Resources:Istemci,IST001%>"
                            Icon="Accept">
                            <Listeners>
                                <Click Handler="$('#btnGirisASP').click();" />
                            </Listeners>
                        </ext:Button>
                        <ext:Button ID="btnIkinciKodYenile" runat="server" TabIndex="3" Text="Yenile"
                            Icon="Reload">
                            <DirectEvents>
                                <Click OnEvent="btnIkinciKodYenile_Click" />
                            </DirectEvents>
                        </ext:Button>
                    </Items>
                </ext:CompositeField>
            </Items>
        </ext:Window>
    </form>
</body>
</html>
