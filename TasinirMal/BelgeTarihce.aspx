<%@ Page Language="C#" CodeBehind="BelgeTarihce.aspx.cs" Inherits="TasinirMal.BelgeTarihce" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="aspnetForm" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <ext:Viewport ID="Viewport1" runat="server" StyleSpec="background-color: transparent;">
        <Items>
            <ext:BorderLayout ID="BorderLayout2" runat="server">
                <North MarginsSummary="10">
                    <ext:Panel ID="Panel2" runat="server" BodyBorder="false" Border="false" Height="50">
                        <Items>
                            <ext:Container ID="Container3" runat="server" Layout="Column" Height="155">
                                <Items>
                                    <ext:Container ID="Container1" runat="server" Layout="Form" ColumnWidth=".5">
                                        <Items>
                                            <ext:DisplayField ID="lblYil" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMBTR006 %>"
                                                LabelStyle="font-weight: bold;" />
                                            <ext:DisplayField ID="lblBelgeNo" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMBTR007 %>"
                                                LabelStyle="font-weight: bold;" />
                                        </Items>
                                    </ext:Container>
                                    <ext:Container ID="Container2" runat="server" Layout="Form" ColumnWidth=".5">
                                        <Items>
                                            <ext:DisplayField ID="lblMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMBTR008 %>"
                                                LabelStyle="font-weight: bold;" />
                                            <ext:DisplayField ID="lblHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMBTR009 %>"
                                                LabelStyle="font-weight: bold;" />
                                        </Items>
                                    </ext:Container>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Panel>
                </North>
                <Center>
                    <ext:GridPanel ID="gridBelgeTarihce" runat="server" Layout="FitLayout" StripeRows="true"
                        TrackMouseOver="true" Border="false" StyleSpec="border-top:1px solid #99bbe8;"
                        ButtonAlign="Center">
                        <Store>
                            <ext:Store ID="tarihceStore" runat="server">
                                <Reader>
                                    <ext:JsonReader>
                                        <Fields>
                                            <ext:RecordField Name="islemTarihi" />
                                            <ext:RecordField Name="islem" />
                                            <ext:RecordField Name="islemYapan" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel>
                            <Columns>
                                <ext:RowNumbererColumn />
                                <ext:Column ColumnID="islemTarihi" DataIndex="islemTarihi" Header="İşlem Tarihi" Width="125" Fixed="true" />
                                <ext:Column ColumnID="islem" DataIndex="islem" Header="İşlem" />
                                <ext:Column ColumnID="islemYapan" DataIndex="islemYapan" Header="İşlemi Yapan" Width="140" Fixed="true"/>
                            </Columns>
                        </ColumnModel>
                        <View>
                            <ext:GridView ID="GridView1" runat="server" ForceFit="true" />
                        </View>
                        <SelectionModel>
                            <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" />
                        </SelectionModel>
                        <Buttons>
                            <ext:Button ID="btnKapat" Text="Kapat" runat="server" Icon="Decline" OnClientClick="parent.hidePopWin();return false;" />
                        </Buttons>
                    </ext:GridPanel>
                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
    </form>
</body>
</html>
