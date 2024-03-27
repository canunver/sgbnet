<%@ Page Language="C#" CodeBehind="TasinirMal.aspx.cs" Inherits="TasinirMal.TasinirMal" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <ext:Store runat="server" ID="storeTasinir">
        <Reader>
            <ext:JsonReader>
                <Fields>
                    <ext:RecordField Name="TasinirKodu" />
                    <ext:RecordField Name="TasinirAdi" />
                    <ext:RecordField Name="OlcuBirimi" />
                    <ext:RecordField Name="GirenMiktar" />
                    <ext:RecordField Name="GirenTutar" />
                    <ext:RecordField Name="CikanMiktar" />
                    <ext:RecordField Name="CikanTutar" />
                    <ext:RecordField Name="ZimmetMiktar" />
                    <ext:RecordField Name="ZimmetTutar" />
                    <ext:RecordField Name="KalanMiktar" />
                    <ext:RecordField Name="KalanTutar" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Viewport ID="Viewport1" runat="server" StyleSpec="background-color: transparent;" Layout="FitLayout">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center>
                    <ext:Panel ID="panelGenel" runat="server" StyleSpec="padding:10px 10px 10px 10px;"
                        Title="<%$ Resources:TasinirMal, FRMTML013 %>" Layout="FitLayout">
                        <Items>
                            <ext:GridPanel ID="grdTasinir" runat="server" StoreID="storeTasinir"
                                StripeRows="true">
                                <ColumnModel>
                                    <Columns>
                                        <ext:RowNumbererColumn />
                                        <ext:Column Header="<%$ Resources:TasinirMal, FRMTML001 %>" DataIndex="TasinirKodu"
                                            MenuDisabled="true" Align="Left">
                                        </ext:Column>
                                        <ext:Column Header="<%$ Resources:TasinirMal, FRMTML002 %>" DataIndex="TasinirAdi"
                                            MenuDisabled="true" Align="Left">
                                        </ext:Column>
                                        <ext:Column Header="<%$ Resources:TasinirMal, FRMTML003 %>" DataIndex="OlcuBirimi"
                                            MenuDisabled="true" Align="Left">
                                        </ext:Column>
                                        <ext:Column Header="<%$ Resources:TasinirMal, FRMTML004 %>" DataIndex="GirenMiktar"
                                            MenuDisabled="true" Align="Right">
                                        </ext:Column>
                                        <ext:Column Header="<%$ Resources:TasinirMal, FRMTML005 %>" DataIndex="GirenTutar"
                                            MenuDisabled="true" Align="Right">
                                        </ext:Column>
                                        <ext:Column Header="<%$ Resources:TasinirMal, FRMTML006 %>" DataIndex="CikanMiktar"
                                            MenuDisabled="true" Align="Right">
                                        </ext:Column>
                                        <ext:Column Header="<%$ Resources:TasinirMal, FRMTML007 %>" DataIndex="CikanTutar"
                                            MenuDisabled="true" Align="Right">
                                        </ext:Column>
                                        <ext:Column Header="<%$ Resources:TasinirMal, FRMTML008 %>" DataIndex="ZimmetMiktar"
                                            MenuDisabled="true" Align="Right">
                                        </ext:Column>
                                        <ext:Column Header="<%$ Resources:TasinirMal, FRMTML009 %>" DataIndex="ZimmetTutar"
                                            MenuDisabled="true" Align="Right">
                                        </ext:Column>
                                        <ext:Column Header="<%$ Resources:TasinirMal, FRMTML010 %>" DataIndex="KalanMiktar"
                                            MenuDisabled="true" Align="Right">
                                        </ext:Column>
                                        <ext:Column Header="<%$ Resources:TasinirMal, FRMTML011 %>" DataIndex="KalanTutar"
                                            MenuDisabled="true" Align="Right">
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <View>
                                    <ext:GridView ID="GridView1" ForceFit="true" runat="server">
                                    </ext:GridView>
                                </View>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server">
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
    </form>
</body>
</html>
