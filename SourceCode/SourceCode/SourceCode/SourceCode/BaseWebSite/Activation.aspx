<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Activation.aspx.cs" Inherits="BaseWebSite.Activation" %>


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
   <%-- <script src="js/cufon-yui.js" type="text/javascript"></script>--%>
    <script src="js/Diavlo.font.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        $(function () {
            $("input, textarea, select, button").uniform();
        });

        $(document).ready(function () {
            $('#div_error_message').hide();
            $('#div_error_message2').hide();
            if ($('#has_error').val() == "1") {
                $('#div_error_message').show();
            }

            if ($('#has_error2').val() == "1") {
                $('#div_error_message2').show();
            }
        });

        function Kontrol() {
            if ($.trim($('#UserName').val()) == "" || $.trim($('#Password').val()) == "" || $('#has_error2').val() == "1") {
                $('#div_error_message2').show();
            }
            else {
                $('#div_error_message2').hide();
            }

        }
    </script>
    <div class="buySurveyForm">
        <div class="boxCenterHeaders">
            User Activation
        </div>
        <div class="boxCenterContent">
            <div class="ui-widget" id="div_error_message" runat="server" clientidmode="Static">
                <div class="ui-state-error ui-corner-all" style="padding: 15px;">
                    <p>
                        <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                    </p>
                </div>
            </div>
        </div>
        <asp:Login ID="Login1" runat="server" OnAuthenticate="Login1_Authenticate" OnLoginError="Login1_LoginError"
            EnableViewState="false" RenderOuterTable="false" FailureText="Error Occured.Please try again.">
            <TextBoxStyle Width="150px" Font-Size="0.8em" />
            <LayoutTemplate>
                <div class="ui-widget" id="div_error_message2" runat="server" clientidmode="Static">
                    <div class="ui-state-error ui-corner-all" style="padding: 15px;">
                        <p>
                            <asp:Literal ID="FailureText" runat="server"></asp:Literal>
                            <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification"
                                ValidationGroup="LoginUserValidationGroup" />
                        </p>
                    </div>
                </div>
                <p>
                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">E-mail:</asp:Label></p>
                <p>
                    <asp:TextBox ID="UserName" runat="server" CssClass="textEntry" ClientIDMode="Static"
                        Width="320px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                        CssClass="failureNotification" ErrorMessage="Please fill your E-mail."
                        ToolTip="Please fill your E-mail." ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                </p>
                <p>
                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label></p>
                <p>
                    <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry" TextMode="Password"
                        ClientIDMode="Static" Width="320px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                        CssClass="failureNotification" ErrorMessage="Please input your Password." ToolTip="Please input your Password."
                        ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                </p>
               
                <p class="submitButton">
                    <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Login" ValidationGroup="LoginUserValidationGroup"  OnClientClick="Kontrol();" />
                </p>
            </LayoutTemplate>
            <InstructionTextStyle Font-Italic="True" ForeColor="Black" />
            <TitleTextStyle BackColor="#1C5E55" Font-Bold="True" Font-Size="0.9em" ForeColor="White" />
        </asp:Login>
    </div>
    <input type="hidden" id="has_error" runat="server" clientidmode="Static" />
    <input type="hidden" id="has_error2" runat="server" clientidmode="Static" />
</asp:Content>
