﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KullanicilarRaporu.aspx.cs" Inherits="BaseWebSite.Admin.Raporlar.KullanicilarRaporu" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   <title></title>
     <link href="../../Styles/Site.css" rel="stylesheet" type="text/css" />
    <meta http-equiv="Pragma" content="no-cache" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="page_report">
      
      

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

        <rsweb:ReportViewer ID="ReportViewer1" runat="server"   Font-Names="Verdana" 
            Font-Size="8pt" InteractiveDeviceInfos="(Collection)" 
            WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="100%" 
            Height="500px">
            <LocalReport ReportPath="Admin\Raporlar\KullanicilarRaporu.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="DataSet1" />
                </DataSources>
            </LocalReport>
           
        </rsweb:ReportViewer>
        
      
        
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" 
            OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" 
            TypeName="BaseWebSite.Admin.Raporlar.KullanicilarDataSetTableAdapters.gnl_kullanicilar_vTableAdapter">
        </asp:ObjectDataSource>
        
      
        
    </div>
    </form>
</body>
</html>
