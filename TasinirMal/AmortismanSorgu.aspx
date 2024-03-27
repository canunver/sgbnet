<%@ Page Language="C#" CodeBehind="AmortismanSorgu.aspx.cs" Inherits="TasinirMal.AmortismanSorgu" %>

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
            if (b == 'prpHarcamaBirimi' && d != "" && c != d)
                Ext1.net.DirectMethods.DonemListele(d, c, { eventMask: { showMask: false, msg: "Lütfen Bekleyin..." } });
        }
    </script>
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
                <ext:Panel ID="pnlAna" runat="server" Region="Center" Layout="BorderLayout" Margins="104 20 10 20">
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
                                        <ext:ComboBox runat="server" TriggerAction="All" ForceSelection="false"
                                            Editable="false" StoreID="strDonem" DisplayField="ADI" ValueField="KOD">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Clear" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="TriggerClick" />
                                            </Listeners>
                                        </ext:ComboBox>
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
                                <ext:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <ext:ToolbarFill />
                                        <%-- <ext:Button ID="btnBelgeOnayla" runat="server" Text="<%$Resources:TasinirMal,FRMZFS046%>"
                                            Icon="Tick">
                                            <DirectEvents>
                                                <Click OnEvent="btnOnayla_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge onaylanacaktır. Bu işlemi onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                    <ExtraParams>
                                                        <ext:Parameter Name="json" Value="Ext1.encode(#{grdAmortisman}.getRowsValues({selectedOnly:true}))"
                                                            Mode="Raw" />
                                                        <ext:Parameter Name="islem" Value="Onay" Mode="Value" />
                                                    </ExtraParams>
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>--%>
                                        <ext:Button ID="btnOnayaGonder" runat="server" Text="Onaya Gönder" Icon="Tick">
                                            <DirectEvents>
                                                <Click OnEvent="btnOnayaGonder_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Amortisman kaydı onaya gönderilecektir. Bu işlemi onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                    <ExtraParams>
                                                        <ext:Parameter Name="json" Value="Ext1.encode(#{grdAmortisman}.getRowsValues({selectedOnly:true}))"
                                                            Mode="Raw" />
                                                    </ExtraParams>
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator runat="server" />
                                        <ext:Button ID="btnBelgeIptal" runat="server" Text="Onay Kaldır"
                                            Icon="Cross">
                                            <DirectEvents>
                                                <Click OnEvent="btnIptal_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Amortisman kaydının onayı kaldırılacaktır. Bu işlemi onaylıyor musunuz??" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                    <ExtraParams>
                                                        <ext:Parameter Name="json" Value="Ext1.encode(#{grdAmortisman}.getRowsValues({selectedOnly:true}))"
                                                            Mode="Raw" />
                                                    </ExtraParams>
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <SelectionModel>
                                <ext:CheckboxSelectionModel runat="server" />
                            </SelectionModel>
                            <Store>
                                <ext:Store runat="server" ID="stoAmortisman">
                                    <Reader>
                                        <ext:JsonReader>
                                            <Fields>
                                                <ext:RecordField Name="kod" />
                                                <ext:RecordField Name="yil" />
                                                <ext:RecordField Name="donem" />
                                                <ext:RecordField Name="muhasebeKod" />
                                                <ext:RecordField Name="harcamaKod" />
                                                <ext:RecordField Name="fisNo" />
                                                <ext:RecordField Name="fisTarih" />
                                                <ext:RecordField Name="ambarKod" />
                                                <ext:RecordField Name="durum" />
                                                <ext:RecordField Name="islemTarih" />
                                                <ext:RecordField Name="islemYapan" />
                                                <ext:RecordField Name="onayDurumKod" />
                                                <ext:RecordField Name="onayDurum" />
                                                <ext:RecordField Name="muhasebeAd" />
                                                <ext:RecordField Name="harcamaAd" />
                                                <ext:RecordField Name="ambarAd" />
                                                <ext:RecordField Name="hesapKodu" />
                                                <ext:RecordField Name="mernis" />
                                                <ext:RecordField Name="tip" />
                                            </Fields>
                                        </ext:JsonReader>
                                    </Reader>
                                </ext:Store>
                            </Store>
                            <ColumnModel>
                                <Columns>
                                    <ext:RowNumbererColumn Width="30" />
                                    <ext:Column DataIndex="fisNo" ColumnID="fisNo" Header="Belge No" Width="60">
                                    </ext:Column> 
                                    <ext:Column DataIndex="yil" ColumnID="yil" Header="Yıl" Width="40">
                                    </ext:Column>
                                    <ext:Column DataIndex="donem" ColumnID="donem" Header="Dönem" Width="50">
                                    </ext:Column>
                                    <ext:Column DataIndex="harcamaAd" ColumnID="harcamaAd" Header="Harcama Birimi" Width="120">
                                    </ext:Column>
                                    <ext:Column DataIndex="ambarAd" ColumnID="ambarAd" Header="Ambar">
                                    </ext:Column>
                                    <ext:Column DataIndex="fisTarih" ColumnID="fisTarih" Header="Belge Tarihi" Width="65">
                                    </ext:Column>
                                    <ext:Column DataIndex="durum" ColumnID="durum" Header="Durum" Width="70">
                                    </ext:Column>
                                    <ext:Column DataIndex="onayDurum" ColumnID="onayDurum" Header="B/A Onay Durum" Width="120">
                                    </ext:Column>
                                    <ext:Column DataIndex="islemYapan" ColumnID="islemYapan" Header="En Son İşlem Yapan" Width="110">
                                    </ext:Column>
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
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
