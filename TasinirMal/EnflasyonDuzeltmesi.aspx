<%@ Page Language="C#" CodeBehind="EnflasyonDuzeltmesi.aspx.cs" Inherits="TasinirMal.EnflasyonDuzeltmesiSayfa" %>

<%@ Register TagPrefix="ext" Namespace="Ext1.Net" Assembly="Ext1.Net" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head" runat="server">
    <title>Enflasyon Düzeltmesi Sayfası</title>
    <script type="text/javascript">
        var harcamaBirimiEski = "";
        var PropertyChange = function (a, b, c, d) {
            //    harcamaBirimiEski = d;
            //    if (b == 'prpHarcamaBirimi' && c != d)
            //        Ext1.net.DirectMethods.DonemListele(d, c, { eventMask: { showMask: false, msg: "Lütfen Bekleyin..." } });
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

    </script>
    <style type="text/css">
        .x-grid3-hd-inner {
            white-space: normal !important;
        }

        .x-grid3-col-edTutarEnflasyonDuzeltmesi, .x-grid3-col-edAmortismanEnflasyonDuzeltmesi, .x-grid3-hd-edAmortismanEnflasyonDuzeltmesi, .x-grid3-hd-edTutarEnflasyonDuzeltmesi {
            color: red;
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
                                <%-- <ext:PropertyGridParameter Name="prpYil" DisplayName="<%$ Resources:TasinirMal, FRMBRK005 %>">
                                    <Editor>
                                        <ext:SpinnerField runat="server">
                                        </ext:SpinnerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpDonem" DisplayName="Ay">
                                    <Editor>
                                        <ext:ComboBox runat="server" TriggerAction="All" ForceSelection="true"
                                            Editable="false" StoreID="strDonem" DisplayField="ADI" ValueField="KOD" />
                                    </Editor>
                                </ext:PropertyGridParameter>--%>
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
<%--                                <ext:PropertyGridParameter Name="prpHarcamaBirimi" DisplayName="<%$ Resources:TasinirMal, FRMBRK008 %>">
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
                                </ext:PropertyGridParameter>--%>
                                <%--                                <ext:PropertyGridParameter Name="prpAmbar" DisplayName="<%$ Resources:TasinirMal, FRMTIS019 %>">
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
                                </ext:PropertyGridParameter>--%>
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
                                        <ext:Button ID="btnKaydet" runat="server" Text="Enflasyon Düzeltmesi Kaydı Üret" TabIndex="7"
                                            Icon="DatabaseSave">
                                            <DirectEvents>
                                                <Click OnEvent="btnKaydet_Click" Timeout="1200000">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Kaydetmek İstiyor musunuz?" />
                                                    <EventMask ShowMask="true" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnRapor" runat="server" Text="Enflasyon Düzeltmesi Raporu" TabIndex="8"
                                            Icon="PageExcel">
                                            <DirectEvents>
                                                <Click OnEvent="btnYazdir_Click" Timeout="1200000" IsUpload="true">
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnKaydetTif" runat="server" Text="TIF Kaydı Üret" TabIndex="9"
                                            Icon="DatabaseSave">
                                            <DirectEvents>
                                                <Click OnEvent="btnKaydetTif_Click" Timeout="1200000">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Kaydetmek İstiyor musunuz?" />
                                                    <EventMask ShowMask="true" Msg="Lütfen bekleyin..." />
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
                                                <ext:RecordField Name="maliyetCariAmortisman" Type="Float" />
                                                <ext:RecordField Name="maliyetBirikmisAmortisman" Type="Float" />
                                                <ext:RecordField Name="maliyetToplamAmortisman" Type="Float" />
                                                <ext:RecordField Name="maliyetKalanAmortisman" Type="Float" />
                                                <ext:RecordField Name="degerDuzeltmeCariAmortisman" Type="Float" />
                                                <ext:RecordField Name="degerDuzeltmeBirikmisAmortisman" Type="Float" />
                                                <ext:RecordField Name="degerDuzeltmeToplamAmortisman" Type="Float" />
                                                <ext:RecordField Name="degerDuzeltmeKalanAmortisman" Type="Float" />

                                                <ext:RecordField Name="girisTarih" Type="String" />

                                                <ext:RecordField Name="toplamCariAmortisman" Type="Float" />
                                                <ext:RecordField Name="toplamBirikmisAmortisman" Type="Float" />
                                                <ext:RecordField Name="toplamAmortisman" Type="Float" />
                                                <ext:RecordField Name="toplamKalanAmortisman" Type="Float" />

                                                <ext:RecordField Name="edTutar" Type="Float" />
                                                <ext:RecordField Name="edAmortisman" Type="Float" />
                                                <ext:RecordField Name="edTutarEnflasyonDuzeltmesi" Type="Float" />
                                                <ext:RecordField Name="edAmortismanEnflasyonDuzeltmesi" Type="Float" />
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
                                    <ext:Column DataIndex="harcamaBirimiKod" ColumnID="harcamaBirimiKod" Header="Harcama Birimi">
                                    </ext:Column>
                                    <ext:Column DataIndex="gorSicilNo" ColumnID="gorSicilNo" Header="<%$ Resources:TasinirMal, Amortisman026%>">
                                    </ext:Column>
                                    <ext:Column DataIndex="hesapPlanKod" ColumnID="hesapPlanKod" Header="<%$ Resources:TasinirMal, Amortisman014%>">
                                    </ext:Column>
                                    <ext:Column DataIndex="hesapPlanAd" ColumnID="hesapPlanAd" Header="<%$ Resources:TasinirMal, Amortisman027%>">
                                    </ext:Column>
                                    <ext:NumberColumn DataIndex="amortismanBaslamaYil" ColumnID="amortismanBaslamaYil"
                                        Header="<%$ Resources:TasinirMal, Amortisman028%>" Format="0000/i" Align="Right">
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="edTutar" ColumnID="edTutar" Header="Toplam Tutar"
                                        Format="0.000,00/i" Align="Right" Width="125" />
                                    <ext:NumberColumn DataIndex="edTutarEnflasyonDuzeltmesi" ColumnID="edTutarEnflasyonDuzeltmesi" Header="Enflasyon Düzeltme Tutarı"
                                        Format="0.000,00/i" Align="Right" Width="135" />
                                    <ext:NumberColumn DataIndex="edAmortisman" ColumnID="edAmortisman" Header="Toplam Amortisman"
                                        Format="0.000,00/i" Align="Right" Width="125" />
                                    <ext:NumberColumn DataIndex="edAmortismanEnflasyonDuzeltmesi" ColumnID="edAmortismanEnflasyonDuzeltmesi" Header="Amortisman Enflasyon Düzeltme Tutarı "
                                        Format="0.000,00/i" Align="Right" Width="135" />
                                </Columns>
                            </ColumnModel>
                            <BottomBar>
                                <ext:PagingToolbar ID="PagingToolbar1" runat="server" HideRefresh="true" PageSize="250">
                                </ext:PagingToolbar>
                            </BottomBar>
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
