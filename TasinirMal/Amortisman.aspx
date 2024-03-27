<%@ Page Language="C#" Inherits="TasinirMal.AmortismanForm " CodeBehind="Amortisman.aspx.cs" %>

<%@ Register TagPrefix="ext" Namespace="Ext1.Net" Assembly="Ext1.Net" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head" runat="server">
    <title>Amortisman İşlemleri</title>
    <script type="text/javascript">
        function GridKomut(komut, record, satir, sutun) {
            Ext1.net.DirectMethods.DegerDegistir(record.data.kod, pgFiltre.source["prpYil"], record.data.donem, record.data.prSicilNo, record.data.amortismanTur, record.data.cariToplamAmortismanTutar, record.data.saymanlikKod, record.data.harcamaBirimiKod, record.data.ambarKod)
        }

        var harcamaBirimiEski = "";
        var PropertyChange = function (a, b, c, d) {
            harcamaBirimiEski = d;
            if (b == 'prpHarcamaBirimi' && c != d)
                Ext1.net.DirectMethods.DonemListele(d, c, { eventMask: { showMask: false, msg: "Lütfen Bekleyin..." } });
        }

        var durdur = false;
        function Durdur() {
            durdur = true;
        }

        function Bitir(mesaj) {
            TaskManager1.stopTask('IslemGostergec');
            if (mesaj.length <= 300) {
                Ext1.Msg.show({ title: "Bilgi", msg: mesaj, icon: Ext1.Msg.INFO, buttons: Ext1.Msg.OK });
                if (!durdur)
                    wndDurum.hide();
            }
            else {
                Ext1.getCmp('pnlMesaj').update(mesaj);
                wndMesaj.show();
            }

        }

        function AmortismanAramaPenceresi() {
            var adres = "AmortismanSorgu.aspx?menuYok=1";
            adres += "&yil=" + pgFiltre.source["prpYil"];
            adres += "&donem=" + pgFiltre.source["prpDonem"];
            adres += "&muhasebeKod=" + pgFiltre.source["prpMuhasebe"];
            adres += "&harcamaKod=" + pgFiltre.source["prpHarcamaBirimi"];
            adres += "&ambarKod=" + pgFiltre.source["prpAmbar"];

            window.top.showPopWin(adres, 1024, 600, true, null);
        }

        var OnayaGonder = function () {
            Ext1.Msg.show({
                title: 'Onay',
                msg: "Amortisman kaydı B/A onayına gönderilecektir. Bu işlemi onaylıyor musunuz?<br><br><b>Açıklama:</b>",
                width: 450,
                closable: false,
                buttons: Ext1.Msg.YESNO,
                buttonText:
                {
                    yes: 'Evet',
                    no: 'Hayır'
                },
                multiline: true,
                fn: function (buttonValue, inputText, showConfig) {
                    if (buttonValue == "yes") {
                        Ext1.net.DirectMethods.OnayaGonder(inputText, { eventMask: { showMask: true } });
                    }
                },
                icon: Ext1.Msg.QUESTION
            });
        }
    </script>
    <style type="text/css">
        .x-grid3-hd-inner {
            white-space: normal !important;
        }
    </style>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server">
        </ext:ResourceManager>
        <ext:Hidden runat="server" ID="hdnKod" />
        <ext:Store ID="strDonem" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport ID="Viewport1" runat="server" Layout="BorderLayout" StyleSpec="background-color: transparent;">
            <Items>
                <ext:Panel ID="Panel1" runat="server" Region="Center" Layout="BorderLayout" Margins="104 20 10 20">
                    <Items>

                        <ext:PropertyGrid ID="pgFiltre" runat="server" Region="West" ForceFit="true" Collapsible="true"
                            Width="250" Margins="5 0 5 5" Split="true" AutoRender="false" Header="false">
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:Button ID="btnSorguTemizle" runat="server" Text="Temizle" Icon="PageWhite">
                                            <DirectEvents>
                                                <Click OnEvent="btnSorguTemizle_Click">
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarFill runat="server" />
                                        <ext:Button ID="btnListele" runat="server" Text="<%$ Resources:TasinirMal,FRMTIM033 %>"
                                            Icon="ApplicationGo">
                                            <DirectEvents>
                                                <Click OnEvent="btnOku_Click" Timeout="1200000">
                                                    <EventMask ShowMask="true" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Source>
                                <ext:PropertyGridParameter Name="prpYil" DisplayName="<%$ Resources:TasinirMal, FRMBRK005 %>">
                                    <Editor>
                                        <ext:SpinnerField runat="server">
                                        </ext:SpinnerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpDonem" DisplayName="Dönem">
                                    <Editor>
                                        <ext:ComboBox runat="server" TriggerAction="All" ForceSelection="true"
                                            Editable="false" StoreID="strDonem" DisplayField="ADI" ValueField="KOD" />
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpMuhasebe" DisplayName="<%$ Resources:TasinirMal, FRMBRK006 %>">
                                    <Editor>
                                        <ext:TriggerField runat="server">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="TriggerClickProperty('prpMuhasebe',this.triggers[0]);" />
                                            </Listeners>
                                        </ext:TriggerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpHarcamaBirimi" DisplayName="<%$ Resources:TasinirMal, FRMBRK008 %>">
                                    <Editor>
                                        <ext:TriggerField runat="server">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="TriggerClickProperty('prpHarcamaBirimi',this.triggers[0]);" />
                                            </Listeners>
                                        </ext:TriggerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpAmbar" DisplayName="<%$ Resources:TasinirMal, FRMTIS019 %>">
                                    <Editor>
                                        <ext:TriggerField runat="server">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="TriggerClickProperty('prpAmbar',this.triggers[0]);" />
                                            </Listeners>
                                        </ext:TriggerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpHesapKod" DisplayName="Hesap Kodu">
                                    <Editor>
                                        <ext:TriggerField runat="server">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="TriggerClickProperty('prpHesapKod',this.triggers[0]);" />
                                            </Listeners>
                                        </ext:TriggerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                            </Source>
                            <Listeners>
                                <Render Handler="function() { this.getStore().sortInfo = undefined; this.getColumnModel().config[0].sortable = false;this.getColumnModel().config[1].sortable = false;}" />
                                <SortChange Handler="this.getStore().sortInfo = undefined;" />
                                <PropertyChange Fn="PropertyChange" />
                            </Listeners>
                            <View>
                                <ext:GridView ID="GridView2" ForceFit="true" runat="server" />
                            </View>
                        </ext:PropertyGrid>
                        <ext:GridPanel ID="grdAmortisman" runat="server" Region="Center" StripeRows="true" Header="false"
                            TrackMouseOver="true" Border="true" Margins="5 5 5 0" Split="true"
                            Cls="gridExt">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <ext:Button ID="btnKaydet" runat="server" Text="Amortisman Kaydı Üret" TabIndex="7"
                                            Icon="DatabaseSave">
                                            <DirectEvents>
                                                <Click OnEvent="btnKaydet_Click" Timeout="1200000">
                                                    <Confirmation ConfirmRequest="true" Title="<%$ Resources:TasinirMal, Amortisman019%>"
                                                        Message="<%$ Resources:TasinirMal, Amortisman020%>" />
                                                    <EventMask ShowMask="true" Msg="<%$ Resources:TasinirMal, Amortisman021%>" />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button runat="server" Text="Raporlar" Icon="PageExcel">
                                            <Menu>
                                                <ext:Menu runat="server">
                                                    <Items>
                                                        <ext:MenuItem ID="btnMenu1" runat="server" Text="Amortisman Raporu"
                                                            TabIndex="8" Timeout="1200000">
                                                            <DirectEvents>
                                                                <Click OnEvent="btnYazdir_Click" IsUpload="true">
                                                                    <ExtraParams>
                                                                        <ext:Parameter Name="tur" Value="4" Mode="Value" />
                                                                        <ext:Parameter Name="raporSekli" Value="" Mode="Value" />
                                                                    </ExtraParams>
                                                                </Click>
                                                            </DirectEvents>
                                                        </ext:MenuItem>
                                                        <ext:MenuItem ID="btnBAOnay" runat="server" Text="BA Onay Raporu"
                                                            TabIndex="8" Timeout="1200000" Hidden="true">
                                                            <DirectEvents>
                                                                <Click OnEvent="btnYazdir_Click" IsUpload="true">
                                                                    <ExtraParams>
                                                                        <ext:Parameter Name="tur" Value="23" Mode="Value" />
                                                                        <ext:Parameter Name="raporSekli" Value="23" Mode="Value" />
                                                                    </ExtraParams>
                                                                </Click>
                                                            </DirectEvents>
                                                        </ext:MenuItem>
                                                        <ext:MenuItem ID="btnMenu2" runat="server" Text="Amortisman Raporu (Çıkan Malzemelerle)"
                                                            TabIndex="8" Timeout="1200000">
                                                            <DirectEvents>
                                                                <Click OnEvent="btnYazdir_Click" IsUpload="true">
                                                                    <ExtraParams>
                                                                        <ext:Parameter Name="tur" Value="5" Mode="Value" />
                                                                        <ext:Parameter Name="raporSekli" Value="" Mode="Value" />
                                                                    </ExtraParams>
                                                                </Click>
                                                            </DirectEvents>
                                                        </ext:MenuItem>
                                                        <ext:MenuItem ID="MenuItem1" runat="server" Text="Amortisman Raporu (TTK)"
                                                            TabIndex="8" Timeout="1200000">
                                                            <DirectEvents>
                                                                <Click OnEvent="btnYazdir_Click" IsUpload="true">
                                                                    <ExtraParams>
                                                                        <ext:Parameter Name="tur" Value="7" Mode="Value" />
                                                                        <ext:Parameter Name="raporSekli" Value="" Mode="Value" />
                                                                    </ExtraParams>
                                                                </Click>
                                                            </DirectEvents>
                                                        </ext:MenuItem>
                                                         <ext:MenuItem ID="MenuItem2" runat="server" Text="Amortisman Raporu (TTK) * İhraç Dahil"
                                                            TabIndex="8" Timeout="1200000">
                                                            <DirectEvents>
                                                                <Click OnEvent="btnYazdir_Click" IsUpload="true">
                                                                    <ExtraParams>
                                                                        <ext:Parameter Name="tur" Value="7" Mode="Value" />
                                                                        <ext:Parameter Name="ihracDahil" Value="1" Mode="Value" />
                                                                        <ext:Parameter Name="raporSekli" Value="" Mode="Value" />
                                                                    </ExtraParams>
                                                                </Click>
                                                            </DirectEvents>
                                                        </ext:MenuItem>
                                                        <ext:MenuItem ID="mnuMuhasebat" runat="server" Text="Amortisman Raporu (Muhasebat)"
                                                            TabIndex="8" Timeout="1200000">
                                                            <DirectEvents>
                                                                <Click OnEvent="btnYazdir_Click" IsUpload="true">
                                                                    <ExtraParams>
                                                                        <ext:Parameter Name="tur" Value="5" Mode="Value" />
                                                                        <ext:Parameter Name="raporSekli" Value="99" Mode="Value" />
                                                                    </ExtraParams>
                                                                </Click>
                                                            </DirectEvents>
                                                        </ext:MenuItem>
                                                        <ext:MenuItem ID="mnuMaliyetMuhasebesi" runat="server" Text="Maliyet Muhasebesi Raporu"
                                                            TabIndex="8" Timeout="1200000" Hidden="true">
                                                            <DirectEvents>
                                                                <Click OnEvent="btnYazdir_Click" IsUpload="true">
                                                                    <ExtraParams>
                                                                        <ext:Parameter Name="tur" Value="8" Mode="Value" />
                                                                        <ext:Parameter Name="raporSekli" Value="" Mode="Value" />
                                                                    </ExtraParams>
                                                                </Click>
                                                            </DirectEvents>
                                                        </ext:MenuItem>
                                                    </Items>
                                                </ext:Menu>
                                            </Menu>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnAmortismanArama" runat="server" Text="Yapılan Amortismanlar" Icon="ApplicationSideList">
                                            <Listeners>
                                                <Click Handler="AmortismanAramaPenceresi();" />
                                            </Listeners>
                                        </ext:Button>
                                        <ext:ToolbarFill />
                                        <ext:Button ID="btnBelgeOnayaGonder" runat="server" Text="Onaya Gönder" Icon="Tick">
                                            <Listeners>
                                                <Click Handler="OnayaGonder();" />
                                            </Listeners>
                                        </ext:Button>
                                        <ext:Button ID="btnBelgeOnayKaldir" runat="server" Text="Onay Kaldır"
                                            Icon="Cross" Hidden="true">
                                            <DirectEvents>
                                                <Click OnEvent="btnOnayKaldir_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Amortisman kaydının onayı kaldırılacaktır. Bu işlemi onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <SelectionModel>
                                <ext:RowSelectionModel ID="RowSelectionModel1" runat="server">
                                </ext:RowSelectionModel>
                            </SelectionModel>
                            <Store>
                                <ext:Store runat="server" ID="stoAmortisman">
                                    <Reader>
                                        <ext:JsonReader>
                                            <Fields>
                                                <ext:RecordField Name="prSicilNo" Type="Int" />
                                                <ext:RecordField Name="amortismanTur" Type="Int" />
                                                <ext:RecordField Name="gorSicilNo" Type="String" />
                                                <ext:RecordField Name="hesapPlanKod" Type="String" />
                                                <ext:RecordField Name="hesapPlanAd" Type="String" />
                                                <ext:RecordField Name="amortismanBaslamaYil" Type="Int" />
                                                <ext:RecordField Name="amortismanSuresi" Type="Int" />
                                                <ext:RecordField Name="toplamTutar" Type="Float" />

                                                <ext:RecordField Name="toplamAmortismanBirikmisTutar" Type="Float" />
                                                <ext:RecordField Name="cariToplamAmortismanTutar" Type="Float" />
                                                <ext:RecordField Name="kalanTutar" Type="Float" />

                                                <ext:RecordField Name="degerArtisKod" Type="String" />
                                                <ext:RecordField Name="degerArtisTutar" Type="Float" />
                                                <ext:RecordField Name="degerArtisOncekiAmortismanTutar" Type="Float" />
                                                <ext:RecordField Name="degerArtisAmortismanTutar" Type="Float" />
                                                <ext:RecordField Name="degerArtisKalanTutar" Type="Float" />
                                                <ext:RecordField Name="kod" Type="String" />
                                                <ext:RecordField Name="donem" Type="Int" />
                                                <ext:RecordField Name="saymanlikKod" Type="String" />
                                                <ext:RecordField Name="harcamaBirimiKod" Type="String" />
                                                <ext:RecordField Name="ambarKod" Type="String" />
                                                <%--ilk Bedel--%>
                                                <ext:RecordField Name="maliyetTutar" Type="Float" />
                                                <%--Bedel Düzeltme Farkı--%>
                                                <ext:RecordField Name="degerDuzeltmeToplamTutar" Type="Float" />
                                                <%--Son Bedel--%>
                                                <ext:RecordField Name="toplamTutar" Type="Float" />
                                                <%--Ayrılan Son Dönem Amortisman--%>
                                                <ext:RecordField Name="sonDonemAmortisman" Type="Float" />
                                                <%--Biriken Amortisman--%>
                                                <ext:RecordField Name="toplamAmortisman" Type="Float" />
                                                <ext:RecordField Name="maliyetCariAmortisman" Type="Float" />
                                                <ext:RecordField Name="maliyetBirikmisAmortisman" Type="Float" />
                                                <ext:RecordField Name="maliyetToplamAmortisman" Type="Float" />
                                                <ext:RecordField Name="maliyetKalanAmortisman" Type="Float" />
                                                <ext:RecordField Name="degerDuzeltmeCariAmortisman" Type="Float" />
                                                <ext:RecordField Name="degerDuzeltmeBirikmisAmortisman" Type="Float" />
                                                <ext:RecordField Name="degerDuzeltmeToplamAmortisman" Type="Float" />
                                                <ext:RecordField Name="degerDuzeltmeKalanAmortisman" Type="Float" />

                                                <ext:RecordField Name="toplamCariAmortisman" Type="Float" />
                                                <ext:RecordField Name="toplamBirikmisAmortisman" Type="Float" />
                                                <ext:RecordField Name="toplamAmortisman" Type="Float" />
                                                <ext:RecordField Name="toplamKalanAmortisman" Type="Float" />
                                            </Fields>
                                        </ext:JsonReader>
                                    </Reader>
                                </ext:Store>
                            </Store>
                            <ColumnModel>
                                <Columns>
                                    <ext:RowNumbererColumn Width="30" />
                                    <ext:NumberColumn DataIndex="prSicilNo" ColumnID="prSicilNo" Header="<%$ Resources:TasinirMal, Amortisman023%>"
                                        Format="0.000,00/i" Align="Right" Hidden="true">
                                    </ext:NumberColumn>
                                    <ext:Column DataIndex="gorSicilNo" ColumnID="gorSicilNo" Header="<%$ Resources:TasinirMal, Amortisman026%>">
                                    </ext:Column>
                                    <ext:Column DataIndex="hesapPlanKod" ColumnID="hesapPlanKod" Header="<%$ Resources:TasinirMal, Amortisman014%>">
                                    </ext:Column>
                                    <ext:Column DataIndex="hesapPlanAd" ColumnID="hesapPlanAd" Header="<%$ Resources:TasinirMal, Amortisman027%>">
                                    </ext:Column>
                                    <ext:NumberColumn DataIndex="amortismanBaslamaYil" ColumnID="amortismanBaslamaYil"
                                        Header="<%$ Resources:TasinirMal, Amortisman028%>" Format="0000/i" Align="Right">
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="amortismanSuresi" ColumnID="amortismanSuresi" Header="<%$ Resources:TasinirMal, Amortisman029%>"
                                        Format="0000/i" Align="Right">
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="maliyetTutar" ColumnID="maliyetTutar" Header="İlk Bedel"
                                        Format="0.000,00/i" Align="Right" Hidden="true" />
                                    <ext:NumberColumn DataIndex="degerDuzeltmeToplamTutar" ColumnID="degerDuzeltmeToplamTutar" Header="Bedel Düzeltme Farkı"
                                        Format="0.000,00/i" Align="Right" Hidden="true" />
                                    <ext:NumberColumn DataIndex="toplamTutar" ColumnID="toplamTutar" Header="Son Bedel"
                                        Format="0.000,00/i" Align="Right" Hidden="true" />

                                    <ext:NumberColumn DataIndex="maliyetCariAmortisman" ColumnID="maliyetCariAmortisman" Header="Maliyet <br \>Cari Dönem Amortisman"
                                        Format="0.000,00/i" Align="Right" Hidden="true" />
                                    <ext:NumberColumn DataIndex="maliyetBirikmisAmortisman" ColumnID="maliyetBirikmisAmortisman" Header="Maliyet <br \>Geçmiş Dönem Amortisman"
                                        Format="0.000,00/i" Align="Right" Hidden="true" />
                                    <ext:NumberColumn DataIndex="maliyetToplamAmortisman" ColumnID="maliyetToplamAmortisman" Header="Maliyet <br \>Toplam Amortisman"
                                        Format="0.000,00/i" Align="Right" Hidden="true" />
                                    <ext:NumberColumn DataIndex="maliyetKalanAmortisman" ColumnID="maliyetKalanAmortisman" Header="Maliyet <br \>Kalan Amortisman"
                                        Format="0.000,00/i" Align="Right" Hidden="true" />

                                    <ext:NumberColumn DataIndex="degerDuzeltmeCariAmortisman" ColumnID="degerDuzeltmeCariAmortisman" Header="Bedel Düzeltme <br \>Cari Dönem Amortisman"
                                        Format="0.000,00/i" Align="Right" Hidden="true" />
                                    <ext:NumberColumn DataIndex="degerDuzeltmeBirikmisAmortisman" ColumnID="degerDuzeltmeBirikmisAmortisman" Header="Bedel Düzeltme <br \>Geçmiş Dönem Amortisman"
                                        Format="0.000,00/i" Align="Right" Hidden="true" />
                                    <ext:NumberColumn DataIndex="degerDuzeltmeToplamAmortisman" ColumnID="degerDuzeltmeToplamAmortisman" Header="Bedel Düzeltme <br \>Toplam Amortisman"
                                        Format="0.000,00/i" Align="Right" Hidden="true" />
                                    <ext:NumberColumn DataIndex="degerDuzeltmeKalanAmortisman" ColumnID="degerDuzeltmeKalanAmortisman" Header="Bedel Düzeltme <br \>Kalan Amortisman"
                                        Format="0.000,00/i" Align="Right" Hidden="true" />

                                    <ext:NumberColumn DataIndex="toplamCariAmortisman" ColumnID="toplamCariAmortisman" Header="Cari Dönem Amortisman"
                                        Format="0.000,00/i" Align="Right" Hidden="true" />
                                    <ext:NumberColumn DataIndex="toplamBirikmisAmortisman" ColumnID="toplamBirikmisAmortisman" Header="Geçmiş Dönem Amortisman"
                                        Format="0.000,00/i" Align="Right" Hidden="true" />
                                    <ext:NumberColumn DataIndex="toplamAmortisman" ColumnID="degerDuzeltmeToplamAmortisman" Header="Toplam Amortisman"
                                        Format="0.000,00/i" Align="Right" Hidden="true" />
                                    <ext:NumberColumn DataIndex="toplamKalanAmortisman" ColumnID="degerDuzeltmeKalanAmortisman" Header="Kalan Amortisman"
                                        Format="0.000,00/i" Align="Right" Hidden="true" />

                                    <ext:NumberColumn DataIndex="amortismanTur" ColumnID="amortismanTur" Header="<%$ Resources:TasinirMal, Amortisman024%>"
                                        Format="0.000,00/i" Align="Right" Hidden="true">
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="toplamTutar" ColumnID="toplamTutar" Header="Giriş Tutarı"
                                        Format="0.000,00/i" Align="Right">
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="toplamAmortismanBirikmisTutar" ColumnID="toplamAmortismanBirikmisTutar"
                                        Header="<%$ Resources:TasinirMal, Amortisman031%>" Format="0.000,00/i" Align="Right">
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="cariToplamAmortismanTutar" ColumnID="cariToplamAmortismanTutar"
                                        Header="<%$ Resources:TasinirMal, Amortisman025%>" Format="0.000,00/i" Align="Right">
                                        <Editor>
                                            <ext:NumberField runat="server" SelectOnFocus="true">
                                            </ext:NumberField>
                                        </Editor>
                                    </ext:NumberColumn>
                                    <ext:ImageCommandColumn Width="30">
                                        <Commands>
                                            <ext:ImageCommand CommandName="Yaz" Icon="Disk"></ext:ImageCommand>
                                        </Commands>
                                    </ext:ImageCommandColumn>
                                    <ext:NumberColumn DataIndex="kalanTutar" ColumnID="kalanTutar" Header="<%$ Resources:TasinirMal, Amortisman032%>"
                                        Format="0.000,00/i" Align="Right">
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="degerArtisTutar" ColumnID="degerArtisTutar" Header="Değer Artış Tutarı"
                                        Format="0.000,00/i" Align="Right" Width="125">
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="degerArtisOncekiAmortismanTutar" ColumnID="degerArtisOncekiAmortismanTutar"
                                        Header="Değer Artış Önceki Amortisman" Format="0.000,00/i" Align="Right" Width="120">
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="cariDegerlemeAmortismanTutar" ColumnID="degerArtisAmortismanTutar"
                                        Header="Değer Artış Amortisman" Format="0.000,00/i" Align="Right">
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="degerArtisKalanTutar" ColumnID="degerArtisKalanTutar" Header="Değer Artış Kalan Amortisman"
                                        Format="0.000,00/i" Align="Right" Width="125">
                                    </ext:NumberColumn>
                                </Columns>
                            </ColumnModel>
                            <BottomBar>
                                <ext:PagingToolbar ID="PagingToolbar1" runat="server" HideRefresh="true" PageSize="250">
                                </ext:PagingToolbar>
                            </BottomBar>
                            <Listeners>
                                <Command Fn="GridKomut" />
                            </Listeners>
                        </ext:GridPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
        <ext:TaskManager ID="TaskManager1" runat="server">
            <Tasks>
                <ext:Task TaskID="IslemGostergec" Interval="1000" AutoRun="false" OnStart="durdur=false;">
                    <DirectEvents>
                        <Update OnEvent="RefreshProgress">
                            <ExtraParams>
                                <ext:Parameter Name="durdur" Value="durdur" Mode="Raw" />
                            </ExtraParams>
                        </Update>
                    </DirectEvents>
                </ext:Task>
            </Tasks>
        </ext:TaskManager>
        <ext:Window runat="server" ID="wndDurum" Width="500" Height="240" Layout="FormLayout"
            Hidden="true" Modal="true" Closable="true" LabelWidth="300" LabelAlign="Top" Padding="10">
            <Items>
                <ext:Label ID="lblIslemYapilanBirim" runat="server" FieldLabel="İşlem Yapılan Birim" LabelAlign="Top" LabelStyle="color:#B40404;" AnchorHorizontal="99%" Text="-" />
                <ext:Label ID="lblIslemYapilanAmbar" runat="server" FieldLabel="İşlem Yapılan Ambar" LabelAlign="Top" LabelStyle="color:#B40404;" AnchorHorizontal="99%" Text="-" />
                <ext:Label ID="lblToplamAmbar" runat="server" FieldLabel="İşlem görecek ambar sayısı"
                    AnchorHorizontal="99%" Hidden="true" />
                <ext:Label ID="lblAmbarSayac" runat="server" FieldLabel="İşlem gören ambar sayısı"
                    AnchorHorizontal="99%" Hidden="true" />
                <ext:Label ID="lblKalanAmbar" runat="server" FieldLabel="Kalan ambar sayısı" AnchorHorizontal="99%" Hidden="true" />
                <ext:Label ID="lblBilgi" runat="server" Html="-" AnchorHorizontal="99%" StyleSpec="text-align:center;" />
                <ext:ProgressBar ID="Progress1" runat="server" AnchorHorizontal="99%" />
            </Items>
            <Buttons>
                <ext:Button runat="server" Text="Durdur" ID="btnDurdur">
                    <Listeners>
                        <Click Fn="Durdur"></Click>
                    </Listeners>
                </ext:Button>
            </Buttons>
        </ext:Window>

        <ext:Window runat="server" ID="wndMesaj" Title="Hata Detay" Width="600" Height="400" Layout="FitLayout"
            Hidden="false" Modal="true" Closable="true">
            <Items>
                <ext:Panel runat="server" ID="pnlMesaj" Border="false" Layout="FitLayout" Padding="20" AutoScroll="true" />
            </Items>
            <Listeners>
                <Hide Handler="Ext1.getCmp('pnlMesaj').update(''); if(!durdur){wndDurum.hide();}" />
            </Listeners>
        </ext:Window>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
