<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AcikAnketSoruTipileriCevapRaporu.aspx.cs" Inherits="BaseWebSite.Anket.Raporlar.AcikAnketSoruTipi1CevapRaporu1" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Raporlar</title>
    <link rel="stylesheet" type="text/css" href="../../css/custom-theme/jquery-ui-1.8.20.custom.css" />
    <link rel="stylesheet" href="../../css/uniform.default.css" type="text/css" media="screen" />
    <link rel="stylesheet" type="text/css" href="../../css/ddsmoothmenu.css" />
    <script src="../../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../js/jquery-ui-1.8.20.custom.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.numeric.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.maskedinput-1.2.2.js" type="text/javascript"></script>
    <script src="../../Scripts/JQ/jquery.ui.datepicker-tr.js" type="text/javascript"></script>
    <script src="../../js/ddsmoothmenu.js" type="text/javascript"></script>
    <script src="../../js/jquery.uniform.js" type="text/javascript" charset="utf-8"></script>
    
     <script type="text/javascript">
         $(function () {

             $("input, textarea,  select").uniform();

         });
        </script>
</head>
<body>
    <form id="form1" runat="server">
  <div class="page_report">
       
        <div id="searchFilter">
            <table>
                <tr>
                    <td>
                        Survey Name :
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlAnket" runat="server" Width="400px" OnSelectedIndexChanged="ddlAnket_SelectedIndexChanged"
                            AutoPostBack="true" />
                    </td>
                <td>
                    Team :
                </td>
                <td>
                    <asp:DropDownList ID="ddlgrup" runat="server" CssClass="textEntry" Width="200px"
                        OnSelectedIndexChanged="ddlgrup_SelectedIndexChanged" AutoPostBack="true">
                    </asp:DropDownList> 
                </td>   
                </tr>
                 <tr>
                    <td>
                        Question :
                    </td>
                    <td colspan="3">
                        <asp:DropDownList ID="ddlSoru" runat="server" Width="700px" OnSelectedIndexChanged="ddlSoru_SelectedIndexChanged"
                            AutoPostBack="true"  />
                    </td>
                
                </tr>
            </table>
        </div>
        <hr />
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" 
            Font-Size="8pt" InteractiveDeviceInfos="(Collection)" 
            WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="100%" Height="620px" 
                >
            <LocalReport ReportPath="Anket\Raporlar\AcikAnketSoruTipi1CevapRaporu.rdlc">
                
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="DataSet1" />
                </DataSources>
                
            </LocalReport>
        </rsweb:ReportViewer>
       
 
        <asp:ObjectDataSource ID="ObjectDataSource3" runat="server" 
            OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" 
            TypeName="BaseWebSite.Anket.Raporlar.AcikAnketSoruTipi3CevapRaporuTableAdapters.sbr_acik_anket_soru_tipi_3_cevap_raporu_spTableAdapter">
            <SelectParameters>
                <asp:Parameter DbType="Guid" Name="soru_uid" />
            </SelectParameters>
        </asp:ObjectDataSource>
       
 
       
        <asp:ObjectDataSource ID="ObjectDataSource7" runat="server" 
            OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" 
            TypeName="BaseWebSite.Anket.Raporlar.AcikAnketSoruTipi7CevapRaporuTableAdapters.sbr_acik_anket_soru_tipi_7_cevap_raporu_spTableAdapter">
            <SelectParameters>
                <asp:Parameter DbType="Guid" Name="soru_uid" />
            </SelectParameters>
        </asp:ObjectDataSource>
       
 
       
        <asp:ObjectDataSource ID="ObjectDataSource11" runat="server" 
            OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" 
            TypeName="BaseWebSite.Anket.Raporlar.AcikAnketSoruTipi11CevapRaporuTableAdapters.sbr_acik_anket_soru_tipi_11_cevap_raporu_spTableAdapter">
            <SelectParameters>
                <asp:Parameter DbType="Guid" Name="soru_uid" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="ObjectDataSource9" runat="server" 
            OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" 
            TypeName="BaseWebSite.Anket.Raporlar.AcikAnketSoruTipi9CevapRaporuTableAdapters.sbr_acik_anket_soru_tipi_9_cevap_raporu_spTableAdapter">
            <SelectParameters>
                <asp:Parameter DbType="Guid" Name="soru_uid" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="ObjectDataSource8" runat="server" 
            OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" 
            TypeName="BaseWebSite.Anket.Raporlar.AcikAnketSoruTipi8CevapRaporuTableAdapters.sbr_acik_anket_soru_tipi_8_cevap_raporu_spTableAdapter">
            <SelectParameters>
                <asp:Parameter DbType="Guid" Name="soru_uid" />
            </SelectParameters>
        </asp:ObjectDataSource>
       
 
       
        <asp:ObjectDataSource ID="ObjectDataSource4" runat="server" 
            OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" 
            TypeName="BaseWebSite.Anket.Raporlar.AcikAnketSoruTipi4CevapRaporuTableAdapters.sbr_acik_anket_soru_tipi_4_cevap_raporu_spTableAdapter">
            <SelectParameters>
                <asp:Parameter DbType="Guid" Name="soru_uid" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="ObjectDataSource10" runat="server" 
            OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" 
            TypeName="BaseWebSite.Anket.Raporlar.AcikAnketSoruTipi10CevapRaporuTableAdapters.sbr_acik_anket_soru_tipi_10_cevap_raporu_spTableAdapter">
            <SelectParameters>
                <asp:Parameter DbType="Guid" Name="soru_uid" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="ObjectDataSource6" runat="server" 
            OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" 
            TypeName="BaseWebSite.Anket.Raporlar.AcikAnketSoruTipi6CevapRaporuTableAdapters.sbr_acik_anket_soru_tipi_6_cevap_raporu_spTableAdapter">
            <SelectParameters>
                <asp:Parameter DbType="Guid" Name="soru_uid" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="ObjectDataSource5" runat="server" 
            OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" 
            TypeName="BaseWebSite.Anket.Raporlar.AcikAnketSoruTipi5CevapRaporuTableAdapters.sbr_acik_anket_soru_tipi_5_cevap_raporu_spTableAdapter">
            <SelectParameters>
                <asp:Parameter DbType="Guid" Name="soru_uid" />
            </SelectParameters>
        </asp:ObjectDataSource>
       
 
       
        <asp:ObjectDataSource ID="ObjectDataSource2" runat="server" 
            OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" 
            TypeName="BaseWebSite.Anket.Raporlar.AcikAnketSoruTipi2CevapRaporuTableAdapters.sbr_acik_anket_soru_tipi_2_cevap_raporu_spTableAdapter">
            <SelectParameters>
                <asp:Parameter DbType="Guid" Name="soru_uid" />
            </SelectParameters>
        </asp:ObjectDataSource>
       
 
       
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" 
            OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" 
            TypeName="BaseWebSite.Anket.Raporlar.AcikAnketSoruTipi1CevapRaporuTableAdapters.sbr_acik_anket_soru_tipi_1_cevap_raporu_spTableAdapter">
            <SelectParameters>
                <asp:Parameter DbType="Guid" Name="soru_uid" />
            </SelectParameters>
        </asp:ObjectDataSource>
       

           
       
    </div>
    </form>
</body>
</html>
