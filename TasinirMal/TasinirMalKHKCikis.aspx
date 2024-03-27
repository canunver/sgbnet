<%@ Page Language="C#" CodeBehind="TasinirMalKHKCikis.aspx.cs" Inherits="TasinirMal.TasinirMalKHKCikis" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="../script/paraFormat.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=3"></script>
    <script type="text/javascript">
        var tur = 1;
        function SecKapatDeger(deger, aciklama) {
            if (tur == 1) {
                txtMuhasebeKod.setValue(deger)
                lblMuhasebeAd.setText(aciklama)
            }
            else if (tur == 2) {
                txtHarcamaKod.setValue(deger)
                lblHarcamaAd.setText(aciklama)
            }
            else if (tur == 3) {
                txtAmbarKod.setValue(deger)
                lblAmbarAd.setText(aciklama)
            }
        }

        function MuhasebeAc() {
            tur = 1;
            var mb = txtMuhasebeKod.getValue();
            showPopWin('ListeMuhasebe.aspx?menuYok=1&cagiran=ext&mb=' + mb, 500, 420, true, null);
        }

        function BirimAc() {
            tur = 2;
            var mb = txtMuhasebeKod.getValue();
            showPopWin('ListeHarcamaBirimi.aspx?menuYok=1&cagiran=ext&mb=' + mb, 500, 420, true, null);
        }

        function MuhasebeDegisti() {
            kodAdGetir('31', 'lblMuhasebeAd', true, new Array('txtMuhasebeKod'), 'KONTROLDENOKU');
        }

        function HarcamaDegisti() {
            kodAdGetir('32', 'lblHarcamaAd', true, new Array('txtMuhasebeKod', 'txtHarcamaKod'), 'KONTROLDENOKU');
        }

        function GridKomut(komut, record, satir, sutun) {
            Ext1.net.DirectMethods.DegerDegistir(spnYil.getValue(), record.data.prSicilNo, record.data.amortismanTur, record.data.cariToplamAmortismanTutar, record.data.saymanlikKod, record.data.harcamaBirimiKod, record.data.ambarKod)
        }

        function BelgeYazdirGoster() {
            try { document.getElementById('lblHata').innerHTML = ""; }
            catch (e) { }

            var yil = document.getElementById('spnYil').value;
            var muhasebe = document.getElementById('txtMuhasebeKod').value;
            var harcama = document.getElementById('txtHarcamaKod').value;
            var fisNo = document.getElementById('hdnYeniBelgeNo').value;
            var tifTur = ""; // document.getElementById('txtBelgeTur').value;

            document.getElementById("frmBelgeYazdir").src = "TasinirIslemFormYazdir.aspx?yil=" + yil + "&harcama=" + harcama + "&fisNo=" + fisNo + "&muhasebe=" + muhasebe + "&tifTur=" + tifTur;
        }

    </script>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server">
        </ext:ResourceManager>
        <ext:Hidden ID="hdnYeniBelgeNo" runat="server" />
        <ext:Hidden ID="hdnAmbarKod" runat="server" />
        <ext:Viewport ID="Viewport1" runat="server" Layout="BorderLayout" StyleSpec="background-color: transparent;">
            <Items>
                <ext:Panel ID="Panel1" runat="server" Region="Center" Layout="RowLayout" Margins="104 20 10 20">
                    <Items>
                        <ext:FormPanel ID="FormPanel1" runat="server" Height="260" Padding="10" ButtonAlign="Center"
                            AutoScroll="true">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <ext:Button ID="btnListele" runat="server" Text="Listele" Icon="TableGo">
                                            <DirectEvents>
                                                <Click OnEvent="btnListele_Click" Timeout="240000">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnKaydet" runat="server" Text="Kaydet" Icon="Disk">
                                            <DirectEvents>
                                                <Click OnEvent="btnKaydet_Click" Timeout="3600000">
                                                    <ExtraParams>
                                                        <ext:Parameter Name="ListeBilgileri" Value="Ext1.util.JSON.encode(#{grdListe}.getRowsValues())"
                                                            Mode="Raw" />
                                                    </ExtraParams>
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt işlemi yapılacak. Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnBelgeYazdir" runat="server" Text="Belge Yazdır" Icon="PageExcel" Hidden="true">
                                            <Listeners>
                                                <Click Handler="BelgeYazdirGoster();" />
                                            </Listeners>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:SpinnerField runat="server" ID="spnYil" TabIndex="1" FieldLabel="<%$ Resources:TasinirMal, Amortisman006%>">
                                </ext:SpinnerField>
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TextField runat="server" ID="txtMuhasebeKod" TabIndex="2" FieldLabel="<%$ Resources:TasinirMal, Amortisman008%>">
                                            <Listeners>
                                                <Blur Handler="MuhasebeDegisti()" />
                                            </Listeners>
                                        </ext:TextField>
                                        <ext:Button runat="server" Icon="Magnifier">
                                            <Listeners>
                                                <Click Fn="MuhasebeAc" />
                                            </Listeners>
                                        </ext:Button>
                                        <ext:Label runat="server" ID="lblMuhasebeAd">
                                        </ext:Label>
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TextField runat="server" ID="txtHarcamaKod" TabIndex="3" FieldLabel="<%$ Resources:TasinirMal, Amortisman010%>">
                                            <Listeners>
                                                <Blur Handler="HarcamaDegisti()" />
                                            </Listeners>
                                        </ext:TextField>
                                        <ext:Button runat="server" Icon="Magnifier">
                                            <Listeners>
                                                <Click Fn="BirimAc" />
                                            </Listeners>
                                        </ext:Button>
                                        <ext:Label runat="server" ID="lblHarcamaAd">
                                        </ext:Label>
                                    </Items>
                                </ext:CompositeField>
                                <ext:TextField runat="server" ID="txtBelgeNo" FieldLabel="Giriş Fiş No">
                                </ext:TextField>
                                <ext:TextField runat="server" ID="txtNereye" FieldLabel="Nereye Verildi" Width="300">
                                </ext:TextField>
                                <ext:CompositeField runat="server" FieldLabel="Dayanak Belge">
                                    <Items>
                                        <ext:DateField runat="server" ID="txtDayanakTarih" Note="Tarihi" Width="100">
                                        </ext:DateField>
                                        <ext:TextField runat="server" ID="txtDayanakNo" Note="Numarası" Width="195">
                                        </ext:TextField>
                                    </Items>
                                </ext:CompositeField>
                                <ext:Label ID="lblYeniBelgeNo" runat="server">
                                </ext:Label>
                            </Items>
                        </ext:FormPanel>
                        <ext:GridPanel ID="grdListe" runat="server" ClicksToEdit="1" StripeRows="true" ColumnLines="true"
                            RowHeight="1">
                            <LoadMask ShowMask="true" Msg="Lütfen bekleyin..." />
                            <ColumnModel ID="ColumnModel1" runat="server">
                                <Columns>
                                    <ext:RowNumbererColumn />
                                    <ext:Column DataIndex="hesapKod" Header="Hesap Kodu" Width="160" Sortable="false"
                                        Hideable="false" />
                                    <ext:Column DataIndex="hesapAdi" Header="Hesap Adı" Width="190" Sortable="false"
                                        Hideable="false" />
                                    <ext:Column DataIndex="sicilNo" Header="Sicil No" Width="190" Sortable="false" Hideable="false" />
                                    <ext:Column DataIndex="miktar" Header="Miktar" Width="80" Align="Right" Sortable="false"
                                        Hideable="false" />
                                    <ext:Column DataIndex="olcuBirimAdi" Header="Ölçü Birimi" Width="90" Sortable="false"
                                        Hideable="false" />
                                    <ext:Column DataIndex="kdv" Header="KDV" Width="50" Align="Right" Sortable="false"
                                        Hideable="false" />
                                    <ext:Column DataIndex="birimFiyat" Header="Birim Fiyat" Width="120" Align="Right"
                                        Sortable="false" Hideable="false">
                                        <Renderer Handler="return Ext1.util.Format.number(value, '0.000,00/i');" />
                                    </ext:Column>
                                </Columns>
                            </ColumnModel>
                            <Store>
                                <ext:Store ID="StoreListe" runat="server">
                                    <Reader>
                                        <ext:ArrayReader IDProperty="kod">
                                            <Fields>
                                                <ext:RecordField Name="hesapKod" />
                                                <ext:RecordField Name="hesapAdi" />
                                                <ext:RecordField Name="sicilNo" />
                                                <ext:RecordField Name="miktar" />
                                                <ext:RecordField Name="olcuBirimAdi" />
                                                <ext:RecordField Name="kdv" />
                                                <ext:RecordField Name="birimFiyat" />
                                            </Fields>
                                        </ext:ArrayReader>
                                    </Reader>
                                </ext:Store>
                            </Store>
                        </ext:GridPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
        <iframe id="frmBelgeYazdir" frameborder="0" scrolling="no" width="1" height="1"></iframe>
        <ext:Window ID="wndBilgi" runat="server" PageX="600" PageY="120" Width="260" Height="150"
            Padding="10" Title="Bilgi" BodyStyle="background-color:white;">
            <Items>
                <ext:Label ID="lblInfo" runat="server" Icon="Information" Html="Bu ekrandan sadece KHK kapsamında girişi yapılmış malzemelerin çıkış işlemleri yapabilirsiniz. Daha önce giriş olarak kayıt edilmiş TİF belgesini listeleyip, çıkışı yapılmak istenen mazlemeleri listeden seçin sonra <b>Kaydet</b> işlemi yaparak yeni Çıkış TİF belgesi üretebilirsiniz.">
                </ext:Label>
            </Items>
        </ext:Window>
    </form>
</body>
</html>
