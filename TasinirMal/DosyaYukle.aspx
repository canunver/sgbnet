<%@ Page Language="C#" CodeBehind="DosyaYukle.aspx.cs" Inherits="TasinirMal.DosyaYukle" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <ext:ResourceManager Locale="Client" ID="ResourceManager1" runat="server" />
    <ext:Viewport ID="Viewport1" runat="server" StyleSpec="background-color: transparent;"
        Layout="FitLayout">
        <Items>
            <ext:Panel ID="pnlDosyaYukle" runat="server" Region="Center" Border="false" Layout="FitLayout">
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:FileUploadField runat="server" ButtonText="Yüklenecek Dosya Seçiniz" ID="fuDosya"
                                ButtonOnly="true" Icon="Attach" ValidateRequestMode="Enabled" ViewStateMode="Enabled">
                                <DirectEvents>
                                    <FileSelected OnEvent="fuDosya_Secildi">
                                        <EventMask ShowMask="true" />
                                    </FileSelected>
                                </DirectEvents>
                            </ext:FileUploadField>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
            </ext:Panel>
        </Items>
    </ext:Viewport>
    </form>
</body>
</html>
