<%@ Page Title="Survey - Kaydol" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs" Inherits="BaseWebSite.Register" %>
    
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
   <%-- <script src="js/cufon-yui.js" type="text/javascript"></script>--%>
    <script src="js/Diavlo.font.js" type="text/javascript"></script>
     <script type="text/javascript">

         var _gaq = _gaq || [];
         _gaq.push(['_setAccount', 'UA-44470048-1']);
         _gaq.push(['_trackPageview']);

         (function () {
             var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
             ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
             var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
         })();

    </script>
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
        if ($.trim($('#Name').val()) == "" || $.trim($('#Surname').val()) == "" || $.trim($('#Ekip').val()) == "" || $.trim($('#Email').val()) == "" || $.trim($('#Password').val()) == "" || $.trim($('#ConfirmPassword').val()) == "" || $('#has_error').val() == "1") {
            $('#div_error_message').show();
        }
        else {
            $('#div_error_message').hide();
        }

    }

    function GosterHtml(tip) {
        if (tip == 1) {
            window.open('Anket/ShowHtmlTemplate.aspx?tip=1', 'Üyelik', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=900,height=500');
        }
        else if (tip == 2) {
            window.open('Anket/ShowHtmlTemplate.aspx?tip=2', 'Üyelik', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=900,height=500');
        }
    }
    </script>
     <div class="buySurveyForm">
         <div class="boxCenterHeaders">
             Register</div>
         <div class="boxCenterContent">
             <div class="ui-widget" id="div_error_message" runat="server" clientidmode="Static">
                 <div class="ui-state-error ui-corner-all" style="padding: 15px;">
                     <p>
                         <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                         <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="failureNotification"
                             ValidationGroup="RegisterUserValidationGroup" />
                             </p>
                 </div>
             </div>
         
          <asp:Literal ID="ltl_ucretsiz_uyelik" runat="server"></asp:Literal>
    <p>
                <asp:Label ID="NameLabel" runat="server" AssociatedControlID="Name">Name:</asp:Label>
                <asp:TextBox ID="Name" runat="server" CssClass="textEntry"></asp:TextBox>
                <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name"
                    CssClass="failureNotification" ErrorMessage="Please Fill Name Field."
                    ToolTip="Please Fill Name Field." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
            </p>
            <p>
                <asp:Label ID="SurnameLabel" runat="server" AssociatedControlID="Surname">Surname:</asp:Label>
                <asp:TextBox ID="Surname" runat="server" CssClass="textEntry"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Surname"
                    CssClass="failureNotification" ErrorMessage="Please Fill Surname Field."
                    ToolTip="Please Fill Surname Field." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
            </p>
             <p>
                <asp:Label ID="EkipLabel" runat="server" AssociatedControlID="Ekip">Team Name:</asp:Label>
                <asp:TextBox ID="Ekip" runat="server" CssClass="textEntry" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="Ekip"
                    CssClass="failureNotification" ErrorMessage="Please Fill Team Name Field."
                    ToolTip="Please Fill Team Name Field." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
            </p>
            <p>
                <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">E-mail:</asp:Label>
                <asp:TextBox ID="Email" runat="server" CssClass="textEntry"></asp:TextBox>
                <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                    CssClass="failureNotification" ErrorMessage="Please Fill E-Name Field."
                    ToolTip="Please Fill Name Field." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="regexEmailValid" runat="server" CssClass="failureNotification"
                    ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ControlToValidate="Email"
                    ErrorMessage="Please Fill Valid E-Name Field." ValidationGroup="RegisterUserValidationGroup">*</asp:RegularExpressionValidator>
            </p>
            <p>
                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                    CssClass="failureNotification" ErrorMessage="Please Fill Password Field."
                    ToolTip="Please Fill Password Field." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="regexPassword" ControlToValidate="Password" runat="server"
                    ValidationExpression="^[a-zA-Z0-9'@&#.\s]{7,100}$" CssClass="failureNotification"
                    ErrorMessage="Password Length Sholud Be 7 Charachter." ValidationGroup="RegisterUserValidationGroup">*</asp:RegularExpressionValidator>
            </p>
            <p>
                <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword">Repeat Password:</asp:Label>
                <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="ConfirmPassword" CssClass="failureNotification"
                    Display="Dynamic" ErrorMessage="Please Fill Repeat Password Field." ID="ConfirmPasswordRequired"
                    runat="server" ToolTip="Please Fill Repeat Password Field." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
                    ControlToValidate="ConfirmPassword" CssClass="failureNotification" Display="Dynamic"
                    ErrorMessage="Password Information must match each other." ValidationGroup="RegisterUserValidationGroup">*</asp:CompareValidator>
            </p>
            <div><p><asp:CheckBox runat="server" ID="chkKabul" /><span class="okudum">I Read / I Agree</span> <a href="#" class="okudum_link" class="okudum_link" onclick="GosterHtml(1);return false;">User Agreement</a> , <a href="#" class="okudum_link" onclick="GosterHtml(2);return false;">Privacy Polic</a></p></div>
            
        
        <p class="submitButton">
            <asp:Button ID="CreateUserButton" runat="server" CommandName="MoveNext" Text="Register"
                ValidationGroup="RegisterUserValidationGroup" OnClick="CreateUserButton_Click" OnClientClick="Kontrol();"  />
        </p>
    </div>
    </div>
    <input type="hidden" id="has_error" runat="server" clientidmode="Static" />
</asp:Content>
