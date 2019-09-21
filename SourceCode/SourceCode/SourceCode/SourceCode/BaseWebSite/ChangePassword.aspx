<%@ Page Title="Şifre Değişikliği" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="ChangePassword.aspx.cs" Inherits="BaseWebSite.ChangePassword" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
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
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
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
         if ($.trim($('#CurrentPassword').val()) == "" || $.trim($('#NewPassword').val()) == "" || $.trim($('#ConfirmNewPassword').val()) == "" || $('#has_error').val() == "1") {
             $('#div_error_message').show();
         }
         else {
             $('#div_error_message').hide();
         }

     }
    </script>
    <div class="buySurveyForm">
        <div class="boxCenterHeaders">
            Change Password</div>
        <div class="boxCenterContent">
            <div class="ui-widget" id="div_error_message" runat="server" clientidmode="Static">
                <div class="ui-state-error ui-corner-all" style="padding: 15px;">
                    <p>
                        <asp:Literal ID="FailureText" runat="server"></asp:Literal>
                        <asp:ValidationSummary ID="ChangeUserPasswordValidationSummary" runat="server" CssClass="failureNotification"
                            ValidationGroup="ChangeUserPasswordValidationGroup" />
                    </p>
                </div>
            </div>
        
            <p>
                <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword">Old Password:</asp:Label>
                <asp:TextBox ID="CurrentPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword"
                    CssClass="failureNotification" ErrorMessage="Please Fill Old Password Info."
                    ToolTip="Please Fill Old Password Info." ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:RequiredFieldValidator>
            </p>
            <p>
                <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword">New Password:</asp:Label>
                <asp:TextBox ID="NewPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
                    CssClass="failureNotification" ErrorMessage="Please Fill New Password Info."
                    ToolTip="Please Fill New Password Info." ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="regexPassword" ControlToValidate="NewPassword"
                    runat="server" ValidationExpression="^[a-zA-Z0-9'@&#.\s]{7,100}$" CssClass="failureNotification"
                    ErrorMessage="Please Fill Mininum 7 Character Length." ValidationGroup="ChangeUserPasswordValidationGroup"></asp:RegularExpressionValidator>
            </p>
            <p>
                <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword">Password Repeat:</asp:Label>
                <asp:TextBox ID="ConfirmNewPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword"
                    CssClass="failureNotification" Display="Dynamic" ErrorMessage="Please Fill Mininum 7 Character Length."
                    ToolTip="Please Fill Mininum 7 Character Length." ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword"
                    ControlToValidate="ConfirmNewPassword" CssClass="failureNotification" Display="Dynamic"
                    ErrorMessage="Password Info Does Not Match." ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:CompareValidator>
            </p>
          
                       
        
        <p class="submitButton">
            <asp:Button ID="ChangePasswordPushButton" runat="server" CommandName="ChangePassword"
                Text="Change Password" ValidationGroup="ChangeUserPasswordValidationGroup" OnClick="ChangePasswordButton_Click" />
        </p>
    </div>
    </div>
    <input type="hidden" id="has_error" runat="server" clientidmode="Static" />
    
</asp:Content>
