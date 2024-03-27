<%@ Page Language="C#" CodeBehind="EnflasyonArtisi.aspx.cs" Inherits="TasinirMal.EnflasyonArtisi" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">

        var artisTutarTurDegisti = function () {
            var katsayi = rgArtisTutar.isVisible() && rdKaysayi.checked;
            cfArtisTutar.setFieldLabel(katsayi ? "Enflasyon Oranı % " : "Artış Tutarı")
            cfArtisTutar.label.setStyle({ "color": katsayi ? "#820d0d" : "#686868" })

            if (katsayi) {
                txtArtisTutar.setValue("");
                pnlEnflasyonAciklama.show();
            }
            else
                pnlEnflasyonAciklama.hide();

            txtArtisTutar.focus();
        }

        var islemTuruDegisti = function () {
            if (ddlTur.getValue() == "1") //Enflasyon 
            {
                rgArtisTutar.show();
                if (txtArtisTutar.getValue() != "")
                    rdTutar.setValue(true);
            }
            else {
                rgArtisTutar.hide();
            }
            artisTutarTurDegisti();
        }
      
    </script>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Store ID="strListe" runat="server" IgnoreExtraFields="false" AutoLoad="false"
            RemotePaging="true" RemoteSort="true" OnRefreshData="strListe_Refresh" RemoteGroup="true">
            <Reader>
                <ext:JsonReader IDProperty="prSicilNo">
                    <Fields>
                        <ext:RecordField Name="prSicilNo" />
                        <ext:RecordField Name="sicilno" />
                        <ext:RecordField Name="kod" />
                        <ext:RecordField Name="ad" />
                        <ext:RecordField Name="eskiSicilNo" />
                        <ext:RecordField Name="amoYuzde" />
                        <ext:RecordField Name="birimFiyat" />
                        <ext:RecordField Name="aciklama" />
                        <ext:RecordField Name="alimTarihi" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
            <BaseParams>
                <ext:Parameter Name="start" Value="0" Mode="Raw" />
                <ext:Parameter Name="limit" Value="0" Mode="Raw" />
                <ext:Parameter Name="sort" Value="" />
                <ext:Parameter Name="dir" Value="ASC" />
            </BaseParams>
            <Proxy>
                <ext:PageProxy />
            </Proxy>
        </ext:Store>
        <ext:Store ID="strAmortismanDurum" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="tabPanelAna" runat="server" Region="Center" StyleSpec="background-color:white;padding:10px"
                    Border="false" Margins="104 20 10 20" Layout="BorderLayout">
                    <Items>
                        <ext:PropertyGrid ID="pgFiltre" runat="server" Region="West" Split="true" Border="true" Width="250">
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
                                        <ext:Button ID="btnListe" runat="server" Text="<%$Resources:Evrak,FRMSRG119 %>" Icon="ApplicationGo">
                                            <DirectEvents>
                                                <Click OnEvent="btnListe_Click" Timeout="2400000">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Source>
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
                                <ext:PropertyGridParameter Name="prpSicilNo" DisplayName="<%$ Resources:TasinirMal, FRMBRK018 %>">
                                    <Editor>
                                        <ext:TriggerField runat="server">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="TriggerClickProperty('prpSicilNo',this.triggers[0]);" />
                                            </Listeners>
                                        </ext:TriggerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpEskiSicilNo" DisplayName="Eski Sicil No">
                                </ext:PropertyGridParameter>

                                <ext:PropertyGridParameter Name="prpFiyat1" DisplayName="Fiyat >=">
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpFiyat2" DisplayName="Fiyat <=">
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpAlimTarih1" DisplayName="Alım Tarihi >=">
                                    <Renderer Fn="TarihRenderer" />
                                    <Editor>
                                        <ext:DateField runat="server" Format="dd.m.Y" />
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpAlimTarih2" DisplayName="Alım Tarihi <=">
                                    <Renderer Fn="TarihRenderer" />
                                    <Editor>
                                        <ext:DateField runat="server" Format="dd.m.Y" />
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpAmoYuzde" DisplayName="Amortisman Yılı">
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpAmoDurum" DisplayName="Amortisman Durumu">
                                    <Renderer Handler="return PropertyRenderer(strAmortismanDurum,value);" />
                                    <Editor>
                                        <ext:ComboBox runat="server" TriggerAction="All" ForceSelection="true"
                                            Editable="false" StoreID="strAmortismanDurum" DisplayField="ADI" ValueField="KOD" Resizable="true"
                                            ListWidth="200" />
                                    </Editor>
                                </ext:PropertyGridParameter>
                            </Source>
                            <Listeners>
                                <Render Handler="function() { this.getStore().sortInfo = undefined; this.getColumnModel().config[0].sortable = false;this.getColumnModel().config[1].sortable = false;}" />
                                <SortChange Handler="this.getStore().sortInfo = undefined;" />
                            </Listeners>
                            <View>
                                <ext:GridView ID="GridView1" ForceFit="true" runat="server" />
                            </View>
                        </ext:PropertyGrid>
                        <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" StoreID="strListe"
                            Border="true" Cls="gridExt">
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:Button runat="server" Text="Enflasyon Düzeltme / Değer Artışı" Icon="Add">
                                            <Listeners>
                                                <Click Handler="wndEnflasyonArtisi.show();" />
                                            </Listeners>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <LoadMask ShowMask="true" Msg="Lütfen Bekleyiniz..." />
                            <ColumnModel ID="ColumnModel1" runat="server">
                                <Columns>
                                    <ext:Column ColumnID="prSicilNo" DataIndex="prSicilNo" Hidden="true" Align="Left" />
                                    <ext:Column ColumnID="sicilno" DataIndex="sicilno" Header="<%$ Resources:TasinirMal, FRMBRK037 %>" Align="Left" Width="90" />
                                    <ext:Column ColumnID="kod" DataIndex="kod" Header="<%$ Resources:TasinirMal, FRMBRK038 %>" Align="Left" Width="100" />
                                    <ext:Column ColumnID="ad" DataIndex="ad" Header="<%$ Resources:TasinirMal, FRMBRK039 %>" Align="Left" Width="225" />
                                    <ext:Column ColumnID="eskiSicilNo" DataIndex="eskiSicilNo" Header="Eski Sicil No" Align="Left" Width="70" />
                                    <ext:Column ColumnID="amoYuzde" DataIndex="amoYuzde" Header="A.%" Align="Right" Width="50">
                                        <%--<Renderer Handler="return Ext1.util.Format.number(value, '0.000,00/i');" />--%>
                                    </ext:Column>
                                    <ext:Column ColumnID="birimFiyat" DataIndex="birimFiyat" Header="Birim Fiyat" Align="Right">
                                        <%--<Renderer Handler="return Ext1.util.Format.number(value, '0.000,00/i');" />--%>
                                    </ext:Column>
                                    <ext:Column ColumnID="aciklama" DataIndex="aciklama" Header="Açıklama" Align="Left" Width="200" />
                                    <ext:Column ColumnID="alimTarihi" DataIndex="alimTarihi" Header="Alım Tarihi" Width="80" />
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:CheckboxSelectionModel runat="server" />
                            </SelectionModel>
                            <BottomBar>
                                <ext:PagingToolbar ID="PagingToolbar1" runat="server" HideRefresh="true"
                                    StoreID="strListe">
                                    <Items>
                                        <ext:Label ID="Label1" runat="server" Text="Satır sayısı:" />
                                        <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
                                        <ext:ComboBox ID="cmbPageSize" runat="server" Width="60">
                                            <Items>
                                                <ext:ListItem Text="50" />
                                                <ext:ListItem Text="100" />
                                                <ext:ListItem Text="250" />
                                                <ext:ListItem Text="500" />
                                                <ext:ListItem Text="1000" />
                                                <ext:ListItem Text="2500" />
                                                <ext:ListItem Text="5000" />
                                            </Items>
                                            <SelectedItem Value="250" />
                                            <Listeners>
                                                <Select Handler="#{PagingToolbar1}.pageSize = parseInt(this.getValue()); #{PagingToolbar1}.doLoad();" />
                                            </Listeners>
                                        </ext:ComboBox>
                                    </Items>
                                </ext:PagingToolbar>
                            </BottomBar>
                        </ext:GridPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
        <ext:Window ID="wndEnflasyonArtisi" runat="server" Title="Değer Düzeltme" Width="500" Height="320"
            Modal="true" Hidden="true" Padding="5" LabelWidth="120">
            <TopBar>
                <ext:Toolbar runat="server">
                    <Items>
                        <ext:Button ID="btnEnflasyonArtisiKaydet" runat="server" Text="Kaydet" Icon="Disk">
                            <DirectEvents>
                                <Click OnEvent="btnEnflasonArtisiKaydet_Click" Timeout="50000">
                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                    <ExtraParams>
                                        <ext:Parameter Name="BILGI" Value="Ext1.encode(#{grdListe}.getRowsValues({selectedOnly:true}))"
                                            Mode="Raw" />
                                    </ExtraParams>
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
            </TopBar>
            <Items>
                <ext:DateField ID="txtBelgeTarihi" runat="server" FieldLabel="İşlem Tarihi" Width="220" />
                <ext:ComboBox ID="ddlTur" runat="server" FieldLabel="İşlem Türü" Editable="false" TypeAhead="true"
                    Mode="Local"
                    ForceSelection="true"
                    TriggerAction="All"
                    SelectOnFocus="true" Width="300">
                    <Items>
                        <ext:ListItem Text="Enflasyon Düzeltme Farkı" Value="1" />
                        <ext:ListItem Text="Değer Artışı" Value="2" />
                        <ext:ListItem Text="Değer Azalışı" Value="3" />
                    </Items>
                    <SelectedItem Value="1" />
                    <Listeners>
                        <Select Handler="islemTuruDegisti();" />
                    </Listeners>
                </ext:ComboBox>
                <ext:CompositeField ID="cfArtisTutar" runat="server" FieldLabel="Artış Tutarı">
                    <Items>
                        <ext:TextField ID="txtArtisTutar" runat="server" Width="120" EnableKeyEvents="true">
                            <Listeners>
                                <KeyUp Handler="enflasyonHesapla();" />
                            </Listeners>
                        </ext:TextField>
                        <ext:RadioGroup ID="rgArtisTutar" runat="server" ColumnsWidths="65,65" Hidden="true">
                            <Items>
                                <ext:Radio runat="server" ID="rdKaysayi" BoxLabel="Oran" />
                                <ext:Radio runat="server" ID="rdTutar" BoxLabel="Tutar" Checked="true" />
                            </Items>
                            <Listeners>
                                <Change Handler="artisTutarTurDegisti();" />
                            </Listeners>
                        </ext:RadioGroup>
                    </Items>
                </ext:CompositeField>
                <ext:Panel ID="pnlEnflasyonAciklama" runat="server" Border="false" FieldLabel="<span style='color:red;'>Enflasyon Açıklaması</span>" Html="Artış oranını % olarak giriniz. Girilen rakam -100 den büyük olacak. Ekrana % rakamı girildiğinde son bedelden hesaplanarak ekrana yazılsın." Frame="true">
                </ext:Panel>
                <ext:TextArea ID="txtGerekce" runat="server" Width="470" FieldLabel="Gerekçe"
                    EmptyText="Gerekçe" Height="170" />
            </Items>
            <Listeners>
                <Show Handler="islemTuruDegisti();" />
            </Listeners>
        </ext:Window>

        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
