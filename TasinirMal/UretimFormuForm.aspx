<%@ Page Language="C#" CodeBehind="UretimFormuForm.aspx.cs" Inherits="TasinirMal.UretimFormuForm" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function BelgeAc(kod) {
            hdnKod.setValue("");
            txtNumara.setValue(kod);
            txtNumara.fireEvent("TriggerClick");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
        </ext:ResourceManager>
        <ext:Hidden ID="hdnKod" runat="server" />
        <ext:Hidden ID="hdnGirenFisNo" runat="server" />
        <ext:Hidden ID="hdnCikanFisNo" runat="server" />
        <ext:Store ID="strUretilecekMalzeme" runat="server">
            <Reader>
                <ext:JsonReader runat="server" IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:FormPanel ID="pnlTanim" runat="server" Region="Center" Margins="5 5 0 5" Padding="10" Split="true" CollapseMode="Mini" Collapsible="true" Header="false" LabelWidth="150" Height="500">
                    <TopBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Button ID="btnKaydet" runat="server" Text="<%$Resources:TasinirMal,FRMKDG029%>" Icon="Disk">
                                    <DirectEvents>
                                        <Click OnEvent="btnKaydet_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt işlemi yapılacak. Onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator />
                                <ext:Button ID="btnTemizle" runat="server" Text="<%$Resources:TasinirMal,FRMGAG038%>" Icon="PageWhite">
                                    <DirectEvents>
                                        <Click OnEvent="btnTemizle_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Formda görülen bilgiler temizlenecektir. Onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator />
                                <ext:Button ID="btnYazdir" runat="server" Text="Raporla" Icon="PageExcel">
                                    <DirectEvents>
                                        <Click OnEvent="btnYazdir_Click" IsUpload="true">
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnImza" runat="server" Text="İmzalar" Icon="TextSignature">
                                    <Listeners>
                                        <Click Handler="wndImza.show();" />
                                    </Listeners>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Items>
                        <ext:CompositeField runat="server" FieldLabel="Belge Tarihi, Numara">
                            <Items>
                                <ext:DateField ID="txtTarih" runat="server" Width="90" />
                                <ext:TriggerField ID="txtNumara" runat="server" Width="100" EmptyText="Numara">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <DirectEvents>
                                        <TriggerClick OnEvent="btnListele_Click">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </TriggerClick>
                                        <SpecialKey Before="return e.getKey() == Ext1.EventObject.ENTER;" OnEvent="btnListele_Click">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </SpecialKey>
                                    </DirectEvents>
                                </ext:TriggerField>
                                <ext:Label ID="lblDurum" runat="server" />
                            </Items>
                        </ext:CompositeField>

                        <ext:Label runat="server" Html="<b>Üretilecek Malzeme</b>" HideLabel="true"></ext:Label>
                        <ext:ComboBox ID="ddlAnaHesap" runat="server" FieldLabel="Ürün Adı" Editable="true"
                            Mode="Local" TriggerAction="All" ForceSelection="true" AllowBlank="false" SelectOnFocus="true"
                            StoreID="strUretilecekMalzeme" DisplayField="ADI" ValueField="KOD" Width="220">
                        </ext:ComboBox>
                        <ext:TextField ID="txtMiktar" runat="server" FieldLabel="Miktar" EmptyText="Üretilecek Miktar" Width="100" />
                        <ext:Label runat="server" Html="<b>Kullanılacak Malzemelerin Alınacağı Birim</b>" HideLabel="true"></ext:Label>
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtCikanMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMKDG021 %>" Width="120">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Fn="TriggerClick" />
                                        <Change Fn="TriggerChange" />
                                    </Listeners>
                                </ext:TriggerField>
                                <ext:Label ID="lblCikanMuhasebeAd" runat="server" />
                            </Items>
                        </ext:CompositeField>
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtCikanHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMKDG023 %>" Width="120">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Fn="TriggerClick" />
                                        <Change Fn="TriggerChange" />
                                    </Listeners>
                                </ext:TriggerField>
                                <ext:Label ID="lblCikanHarcamaBirimiAd" runat="server" />
                            </Items>
                        </ext:CompositeField>
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtCikanAmbar" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMKDG025 %>" Width="120">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Fn="TriggerClick" />
                                        <Change Fn="TriggerChange" />
                                    </Listeners>
                                </ext:TriggerField>
                                <ext:Label ID="lblCikanAmbarAd" runat="server" />
                            </Items>
                        </ext:CompositeField>

                        <ext:Label runat="server" Html="<b>Üretilen Malzemenin Konulacağı Birim</b>" HideLabel="true"></ext:Label>
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtGirenMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMKDG021 %>" Width="120">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Fn="TriggerClick" />
                                        <Change Fn="TriggerChange" />
                                    </Listeners>
                                </ext:TriggerField>
                                <ext:Label ID="lblGirenMuhasebeAd" runat="server" />
                            </Items>
                        </ext:CompositeField>
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtGirenHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMKDG023 %>" Width="120">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Fn="TriggerClick" />
                                        <Change Fn="TriggerChange" />
                                    </Listeners>
                                </ext:TriggerField>
                                <ext:Label ID="lblGirenHarcamaBirimiAd" runat="server" />
                            </Items>
                        </ext:CompositeField>
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtGirenAmbar" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMKDG025 %>" Width="120">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Fn="TriggerClick" />
                                        <Change Fn="TriggerChange" />
                                    </Listeners>
                                </ext:TriggerField>
                                <ext:Label ID="lblGirenAmbarAd" runat="server" />
                            </Items>
                        </ext:CompositeField>
                        <ext:Label runat="server" Html="<b>Hazırlanan TİF Fişleri</b>" HideLabel="true"></ext:Label>
                        <ext:Label ID="lblGirisFisi" runat="server" FieldLabel="Giriş TİF Fişi"></ext:Label>
                        <ext:Label ID="lblCikisFisi" runat="server" FieldLabel="Çıkış TİF Fişi"></ext:Label>
                    </Items>
                </ext:FormPanel>
            </Items>
        </ext:Viewport>

        <ext:Window runat="server" ID="wndImza" Width="550" Height="360" Layout="FormLayout"
            Hidden="true" Modal="true" Closable="true" Title="İmza Bilgileri" Padding="10">
            <Items>
                <ext:CompositeField runat="server" FieldLabel="Hazırlayan">
                    <Items>
                        <ext:TextField ID="txtImzaAdi1" runat="server" Note="Adı Soyadı" Width="200"></ext:TextField>
                        <ext:TextField ID="txtImzaGorev1" runat="server" Note="Görevi" Width="200"></ext:TextField>
                    </Items>
                </ext:CompositeField>

                <ext:CompositeField runat="server" FieldLabel="Görevli">
                    <Items>
                        <ext:TextField ID="txtImzaAdi2" runat="server" Note="Adı Soyadı" Width="200"></ext:TextField>
                        <ext:TextField ID="txtImzaGorev2" runat="server" Note="Görevi" Width="200"></ext:TextField>
                    </Items>
                </ext:CompositeField>
            </Items>
            <Buttons>
                <ext:Button ID="btnImzaKaydet" runat="server" Text="Kaydet">
                    <DirectEvents>
                        <Click OnEvent="btnImzaKaydet_Click">
                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                        </Click>
                    </DirectEvents>
                </ext:Button>
            </Buttons>
        </ext:Window>
    </form>
</body>
</html>
