<%@ Page Language="C#" CodeBehind="AktarimForm.aspx.cs" Inherits="TasinirMal.AktarimForm" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function BelgeAc(kod) {
            if (kod != "")
                hdnKod.setValue(kod);

            Ext1.net.DirectMethods.btnListele_Click({ eventMask: { showMask: true, msg: "Lütfen Bekleyin..." } });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="AlanIsimleriniYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Hidden ID="hdnKod" runat="server" />
        <ext:Store ID="strIslemTipi" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="FitLayout">
            <Items>
                <ext:FormPanel ID="pnlTanim" runat="server" Margins="5 5 0 5" Padding="10" LabelWidth="150">
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
                                <ext:Button ID="btnYazdir" runat="server" Text="Raporla" Icon="PageExcel" Hidden="true">
                                    <DirectEvents>
                                        <Click OnEvent="btnYazdir_Click" IsUpload="true">
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Items>
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMKDG021 %>" Width="120">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Fn="TriggerClick" />
                                        <Change Fn="TriggerChange" />
                                    </Listeners>
                                </ext:TriggerField>
                                <ext:Label ID="lblMuhasebeAd" runat="server" />
                            </Items>
                        </ext:CompositeField>
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMKDG023 %>" Width="120">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Fn="TriggerClick" />
                                        <Change Fn="TriggerChange" />
                                    </Listeners>
                                </ext:TriggerField>
                                <ext:Label ID="lblHarcamaBirimiAd" runat="server" />
                            </Items>
                        </ext:CompositeField>
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtAmbar" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMKDG025 %>" Width="120">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Fn="TriggerClick" />
                                        <Change Fn="TriggerChange" />
                                    </Listeners>
                                </ext:TriggerField>
                                <ext:Label ID="lblAmbarAd" runat="server" />
                            </Items>
                        </ext:CompositeField>
                        <ext:CompositeField runat="server" FieldLabel="Belge Tarihi">
                            <Items>
                                <ext:DateField ID="txtTarih" runat="server" Width="120" />
                                <ext:Label ID="lblDurum" runat="server" />
                            </Items>
                        </ext:CompositeField>
                        <ext:ComboBox ID="cmbBelgeTuru" runat="server" Width="120" FieldLabel="Fiş Türü">
                            <Items>
                                <ext:ListItem Value="1" Text="Dayanıklı" />
                                <ext:ListItem Value="2" Text="Kütüphane" />
                                <ext:ListItem Value="3" Text="Müze" />
                            </Items>
                            <SelectedItem Value="1" />
                        </ext:ComboBox>

                        <ext:ComboBox ID="ddlIslemTipi" runat="server" Editable="false" FieldLabel="<%$Resources:TasinirMal,FRMTMT011 %>" StoreID="strIslemTipi" ValueField="KOD" DisplayField="ADI" SelectOnFocus="true" Width="220" />
                        <ext:TextField ID="txtBulunduguYer" runat="server" FieldLabel="Nereden Geldi" Width="450" />
                        <ext:TextField ID="txtAciklama" runat="server" FieldLabel="Açıklama" Width="450" />
                        <ext:FileUploadField ID="btnDosyaYukle" runat="server" ButtonOnly="true" Icon="PageWhiteAdd"
                            ButtonText="Dosya Seç">
                        </ext:FileUploadField>
                        <ext:DisplayField ID="lblBilgi" runat="server" Width="500"></ext:DisplayField>

                        <ext:Panel ID="pnlTarihce" runat="server" Title="Bilgi Paneli" Width="550" Height="300" Layout="FitLayout" Hidden="false" StyleSpec="position:absolute; top:10px; right:20px">
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:Button ID="btnDevamEt" runat="server" Text="Devam Et" Icon="Accept" Hidden="true">
                                            <DirectEvents>
                                                <Click OnEvent="btnDevamEt_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Fiş oluşturulmaya devam edilecek. Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarFill />
                                        <ext:Button runat="server" Text="Yenile" Icon="ArrowRotateClockwise">
                                            <Listeners>
                                                <Click Handler="BelgeAc('');" />
                                            </Listeners>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:DisplayField ID="lblTarihce" runat="server" AutoScroll="true"></ext:DisplayField>
                            </Items>
                        </ext:Panel>
                    </Items>
                </ext:FormPanel>
            </Items>
        </ext:Viewport>
    </form>
</body>
</html>
