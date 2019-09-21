<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RaporPage.aspx.cs" Inherits="BaseWebSite.Survey.RaporPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
 <link href="../Styles/cupertino/jquery-ui-1.8.17.custom.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui-1.8.17.custom.min.js" type="text/javascript"></script>
    <style type="text/css">
        #toolbar
        {
            padding: 10px 4px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <script type="text/javascript">
     $(function () {
         $("#accordion").accordion({ fillSpace: true });
     });
     $(function () {
         $("#ana_sayfa").button({

         }).click(function () {
             __doPostBack('AnaSayfa', 'AnaSayfa');
             return false;
         });
         
     });

     $(document).ready(function () {
         
     });


     function pageLoad() {
         
     }

     
    </script>
    <div style="width: 100%; margin: auto;">
   
        <div style="padding: 5px;">
            <div class="toolbar">
                <div id="toolbar_resizer" style="padding: 10px; width: 93%; height: 23px;" class="ui-widget-content">
                    <span id="toolbar" class="ui-dialog-content ui-corner-all">
                          <button id="ana_sayfa">
                            My Surveys</button>
                    </span>
                </div>
            </div>
        </div>
        <div style="padding: 0 5px; width: 20%; height: 100%; margin-top: 1px; float: left;">
            <div id="accordionResizer" style="padding: 10px; width: 100%; height: 500px;" class="ui-widget-content">
                <div id="accordion">

                    <h3>
                        <a href="#">Katılımcı Gönderimli Survey Raporları</a></h3>
                    <div> <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="10px">
                                </td>
                                <td>
                                    <asp:LinkButton ID="kullanici_bazli_anket_raporu"  CssClass="anket" runat="server" OnClick="kullanici_bazli_anket_raporu_Click">Katılımcı Bazlı Survey Raporu</asp:LinkButton>
                                </td>
                            </tr>
                        </table> <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <div class="separator">
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="10px">
                                </td>
                                <td>
                                    <asp:LinkButton ID="anket_giris_raporu"  CssClass="anket" runat="server" OnClick="anket_giris_raporu_Click">Survey Giriş Raporu</asp:LinkButton>
                                </td>
                            </tr>
                        </table> <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <div class="separator">
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="10px">
                                </td>
                                <td>
                                    <asp:LinkButton ID="anket_cevaplanma_raporu"  CssClass="anket" runat="server" OnClick="anket_cevaplanma_raporu_Click">Survey Cevaplanma Raporu</asp:LinkButton>
                                </td>
                            </tr>
                        </table> <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <div class="separator">
                                    </div>
                                </td>
                            </tr>
                        </table>
                         <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="10px">
                                </td>
                                <td>
                                    <asp:LinkButton ID="anket_bitirme_raporu"  CssClass="anket" runat="server" OnClick="anket_bitirme_raporu_Click">Survey Bitirme Raporu</asp:LinkButton>
                                </td>
                            </tr>
                        </table> <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <div class="separator">
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="10px">
                                </td>
                                <td>
                                    <asp:LinkButton ID="anket_cevap_raporu"  CssClass="anket" runat="server" OnClick="anket_cevap_raporu_Click">Survey Cevap Raporu</asp:LinkButton>
                                </td>
                            </tr>
                        </table> <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <div class="separator">
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="10px">
                                </td>
                                <td>
                                    <asp:LinkButton ID="anket_cevap_raporu_tumu"  CssClass="anket" runat="server" OnClick="anket_cevap_raporu_tumu_Click">Survey Answer Report(All)</asp:LinkButton>
                                </td>
                            </tr>
                        </table> <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <div class="separator">
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <h3>
                        <a href="#">Açık Survey Raporları</a></h3>
                    <div> <%--<table cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="10px">
                                </td>
                                <td>
                                    <asp:LinkButton ID="LinkButton1"  CssClass="anket" runat="server" OnClick="kullanici_bazli_anket_raporu_Click">Kullanıcı Bazlı Survey Raporu</asp:LinkButton>
                                </td>
                            </tr>
                        </table> <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <div class="separator">
                                    </div>
                                </td>
                            </tr>
                        </table>--%>
                    </div>
                </div>
            </div>
        </div>
        <div style="padding: 0px 5px 0px 20px; width: 70%; height: 100%; margin: 1px; float: left;">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="kullanici_bazli_anket_raporu" />
                </Triggers>
                <ContentTemplate>
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                        <ProgressTemplate>
                            <div class="CenterPB" style="height: 40px; width: 40px;">
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/ajax-loader.gif" Height="35px"
                                    Width="35px" />
                                Yükleniyor...
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <div id="Div1" style="padding: 10px 10px; width: 100%; height: 500px; border: 1px solid #dddddd;
                        background-color: #f2f5f7;">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr style="background-color: #5CB9E8;">
                                <td width="20px">
                                    <img src="../Images/_spacer.gif" height="40px" width="20px" alt="" />
                                </td>
                                <td>
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:LinkButton ID="LinkButtonHeader" Font-Size="23px" ForeColor="White" Font-Underline="false"
                                                    runat="server"></asp:LinkButton>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td>
                                <iframe id="iframeMap" runat="server" frameborder="0" height="450px" width="100%"></iframe>
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div style="padding: 5px; clear: both;">
            
        </div>
    </div>
</asp:Content>
