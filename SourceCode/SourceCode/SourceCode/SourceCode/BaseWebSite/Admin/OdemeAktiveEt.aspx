<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OdemeAktiveEt.aspx.cs"
    Inherits="BaseWebSite.Admin.OdemeAktiveEt" %>

<%@ Register Src="~/BaseControls/RepeaterPaging.ascx" TagName="RepeaterPaging" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Ödeme Aktive Et</title>
    <link rel="stylesheet" type="text/css" href="../css/custom-theme/jquery-ui-1.8.20.custom.css" />
    <link rel="stylesheet" href="../css/uniform.default.css" type="text/css" media="screen" />
    <link rel="stylesheet" type="text/css" href="../css/ddsmoothmenu.css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.20.custom.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.numeric.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.maskedinput-1.2.2.js" type="text/javascript"></script>
    <script src="../Scripts/JQ/jquery.ui.datepicker-tr.js" type="text/javascript"></script>
    <script src="../js/ddsmoothmenu.js" type="text/javascript"></script>
    <script src="../js/jquery.uniform.js" type="text/javascript" charset="utf-8"></script>
    <link rel="stylesheet" type="text/css" href="../css/reset.css"/>
	<link rel="stylesheet" type="text/css" href="../css/style.css"/>
    <script type="text/javascript">
        $(function () {
            $(".sortable").sortable({
                revert: true
            });
            $("ul, li").disableSelection();

        });

        
    </script>
    <script type="text/javascript">
        function AktiveEt(id) {
            if (window.confirm('İlgili Üyeliği Aktive Etmek istiyor musunuz?')) {
                __doPostBack('AktiveEt', id);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 100%; margin: auto;">
      <asp:LinkButton ID="LinkButtonHeader" Font-Size="23px" ForeColor="White" Font-Underline="false"
                                                    runat="server"></asp:LinkButton>
        <div id="mainCenter" style="height: 100%">
            <div class="boxCenter">
                <div class="boxCenterContent">
                    <asp:Repeater ID="rptSurveyler" runat="server">
                        <HeaderTemplate>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="surveyListCenter">
                                <div class="surveyListLeft">
                                    <div class="surveyName">
                                        <a class="mediumUnderlineLink" href="#">
                                            <%#DataBinder.Eval(Container.DataItem, "ad")%>
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "soyad")%>
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "grup_adi")%>
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "anket_sayisi")%> Survey
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "odeme_tipi")%>
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "paket_fiyati")%>
                                            -
                                            İşlem no : <%#DataBinder.Eval(Container.DataItem, "islem_id")%>
                                            </a>
                                    </div>
                                    <div class="surveyType">
                                        <%#DataBinder.Eval(Container.DataItem, "telefonu")%>
                                        -
                                        <%#DataBinder.Eval(Container.DataItem, "cep_telefonu")%>
                                        -
                                        <%#DataBinder.Eval(Container.DataItem, "adres")%>
                                        <br />
                                        <%#DataBinder.Eval(Container.DataItem, "sirket_adi")%>
                                        -
                                        <%#DataBinder.Eval(Container.DataItem, "vergi_dairesi")%>
                                        -
                                        <%#DataBinder.Eval(Container.DataItem, "vergi_no")%>
                                        -
                                        <%#DataBinder.Eval(Container.DataItem, "odeme_sekli")%>
                                    </div>
                                    <div class="surveyListButtons">
                                        <a class="aktiveEt" href="#" onclick="AktiveEt('<%#DataBinder.Eval(Container.DataItem, "id")%>');">
                                            Aktive Et</a>
                                    </div>
                                </div>
                                <div class="surveyListRight">
                                    <div class="surveyDate">
                                        <%#DataBinder.Eval(Container.DataItem, "paket_alim_tarihi")%>
                                        -
                                        <%#DataBinder.Eval(Container.DataItem, "uyelik_bitis_tarihi")%>
                                    </div>
                                    <div class="surveyStatus">
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <div class="surveyListCenter lightBg">
                                <div class="surveyListLeft">
                                    <div class="surveyName">
                                        <a class="mediumUnderlineLink" href="#">
                                            <%#DataBinder.Eval(Container.DataItem, "ad")%>
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "soyad")%>
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "grup_adi")%>
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "anket_sayisi")%> Survey
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "odeme_tipi")%>
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "paket_fiyati")%>
                                             -
                                            İşlem no : <%#DataBinder.Eval(Container.DataItem, "islem_id")%>
                                            </a>
                                    </div>
                                    <div class="surveyType">
                                        <%#DataBinder.Eval(Container.DataItem, "telefonu")%>
                                        -
                                        <%#DataBinder.Eval(Container.DataItem, "cep_telefonu")%>
                                        -
                                        <%#DataBinder.Eval(Container.DataItem, "adres")%>
                                        <br />
                                        <%#DataBinder.Eval(Container.DataItem, "sirket_adi")%>
                                        -
                                        <%#DataBinder.Eval(Container.DataItem, "vergi_dairesi")%>
                                        -
                                        <%#DataBinder.Eval(Container.DataItem, "vergi_no")%>
                                        -
                                        <%#DataBinder.Eval(Container.DataItem, "odeme_sekli")%>
                                    </div>
                                    <div class="surveyListButtons">
                                        <a class="aktiveEt" href="#" onclick="AktiveEt('<%#DataBinder.Eval(Container.DataItem, "id")%>');">
                                            Aktive Et</a>
                                    </div>
                                </div>
                                <div class="surveyListRight">
                                    <div class="surveyDate">
                                        <%#DataBinder.Eval(Container.DataItem, "paket_alim_tarihi")%>
                                        -
                                        <%#DataBinder.Eval(Container.DataItem, "uyelik_bitis_tarihi")%>
                                    </div>
                                    <div class="surveyStatus">
                                    </div>
                                </div>
                            </div>
                        </AlternatingItemTemplate>
                    </asp:Repeater>
                </div>
                <uc1:RepeaterPaging ID="RepeaterPaging1" runat="server" />
            </div>
        </div>
    </div>
    </form>
</body>
</html>
