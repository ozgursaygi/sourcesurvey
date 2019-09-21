<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="RegisterationInfoPage.aspx.cs" Inherits="BaseWebSite.RegisterationInfoPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="css/custom-theme/jquery-ui-1.8.20.custom.css" />
    <link rel="stylesheet" href="css/uniform.default.css" type="text/css" media="screen" />
    <link rel="stylesheet" type="text/css" href="css/ddsmoothmenu.css" />
    <script src="js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-1.8.20.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery.numeric.js" type="text/javascript"></script>
    <script src="Scripts/jquery.maskedinput-1.2.2.js" type="text/javascript"></script>
    <script src="js/ddsmoothmenu.js" type="text/javascript"></script>
    <script src="js/jquery.uniform.js" type="text/javascript" charset="utf-8"></script>
    <script src="js/jquery.ae.image.resize.min.js" type="text/javascript"></script>
    <script src="Scripts/JQ/jquery.ui.datepicker-tr.js" type="text/javascript"></script>
    <%--<script src="js/cufon-yui.js" type="text/javascript"></script>--%>
    <script src="js/Diavlo.font.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="buySurveyForm">
        <div class="boxCenterHeaders">
            İşlem Sonucu</div>
        <div class="boxCenterContent">
            <div class="ui-widget" id="div_error_message" runat="server" clientidmode="Static">
                <div class="ui-state-error ui-corner-all" style="padding: 15px;">
                    <p>
                        <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                    </p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
