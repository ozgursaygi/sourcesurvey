<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ArkadasimaOner.aspx.cs" Inherits="BaseWebSite.ArkadasimaOner" %>
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
<script type="text/javascript">
    $(function () {
        $("input, textarea, select, button").uniform();
    });

    $(document).ready(function () {

        $('#div_error_message').hide();
        if ($('#has_error').val() == "1") {
            $('#div_error_message').show();
        }


    });

    function Kontrol() {
        if ($.trim($('#Email').val()) == "" || $('#has_error').val() == "1") {
            $('#div_error_message').show();
        }
        else {
            $('#div_error_message').hide();
        }

    }

    </script>
         <div class="buySurveyForm">
         <div class="boxCenterHeaders">
             Arkadaşıma Öner</div>
         <div class="boxCenterContent">
             <div class="ui-widget" id="div_error_message" runat="server" clientidmode="Static">
                 <div class="ui-state-error ui-corner-all" style="padding: 15px;">
                     <p>
                         <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                         <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="failureNotification"
                             ValidationGroup="RegisterUserValidationGroup" />
                 </div>
             </div>
         </div>
            <p>
                <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">E-posta:</asp:Label>
                <asp:TextBox ID="Email" runat="server" CssClass="textEntry"></asp:TextBox>
                <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                    CssClass="failureNotification" ErrorMessage="Lütfen E-posta bilgisini doldurunuz."
                    ToolTip="Lütfen E-posta bilgisini doldurunuz." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="regexEmailValid" runat="server" CssClass="failureNotification"
                    ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ControlToValidate="Email"
                    ErrorMessage="Lütfen geçerli bir e-posta adresi giriniz." ValidationGroup="RegisterUserValidationGroup">*</asp:RegularExpressionValidator>
            </p>
    <p>
                <asp:Label ID="MesajLabel" runat="server" AssociatedControlID="Mesaj">Mesaj :</asp:Label>
                <asp:TextBox ID="Mesaj" runat="server" CssClass="textEntry" TextMode="MultiLine" Rows="8"></asp:TextBox>
            </p>        
        
        <p class="submitButton">
            <asp:Button ID="Gonder" runat="server" CommandName="MoveNext" Text="Gönder"
                ValidationGroup="RegisterUserValidationGroup" OnClientClick="Kontrol();" 
                onclick="Gonder_Click"  />
        </p>
    </div>
    <input type="hidden" id="has_error" runat="server" clientidmode="Static" />
</asp:Content>
