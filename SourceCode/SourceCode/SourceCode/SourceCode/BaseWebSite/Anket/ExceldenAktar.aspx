<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExceldenAktar.aspx.cs" Inherits="BaseWebSite.Survey.ExceldenAktar" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Expires" content="-1" />
    <title>Raporlar</title>
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
    <link rel="stylesheet" type="text/css" href="../css/reset.css" />
    <link rel="stylesheet" type="text/css" href="../css/style.css" />
    <script type="text/javascript">
        $(function () {

            $("input, textarea,  select").uniform();

        });
    </script>
    <base target="_self" />
        <script type="text/javascript">
            

            function checkFileExtension(elem) {
                var filePath = elem.value;

                if (filePath.indexOf('.') == -1)
                    return false;

                var validExtensions = new Array();
                var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();
                //Add valid extentions in this array
                validExtensions[0] = 'xls';
                validExtensions[1] = 'xlsx';
                //validExtensions[1] = 'pdf';

                for (var i = 0; i < validExtensions.length; i++) {
                    if (ext == validExtensions[i])
                        return true;
                }

                alert('Lütfen .xls , .xlsx uzantılı dosyalar seçiniz.');

                return false;
            }
    </script>
</head>
<body >
    <form id="form1" runat="server">
    <div style="vertical-align:middle">
     <table cellpadding="0" cellspacing="0" width="630px">
     <tr>
                    <td width="150px">
                        <img src="../Images/_spacer.gif" height="20px" />
                    </td>
                    <td width="480px">
                     
                    </td>
                </tr>
     <tr>
                    <td width="150px">
                        <asp:Label ID="Label1" runat="server" Text="E-Posta Grubu :"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:Label ID="Label2" runat="server" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <img src="../Images/_spacer.gif" height="20px" />
                    </td>
                    <td width="480px">
                     
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <img src="../Images/_spacer.gif" height="50px" />
                    </td>
                    <td width="480px">
                     Excel Formatı : Ad , Soyad , E-Posta Adresi şeklinde Olmalıdır.
                     <br />
                     <br />
                     Örnek :
                     <img src="../Images/excel_ornek.png" />
                    </td>
                </tr>
                   <tr>
                    <td width="150px">
                        <img src="../Images/_spacer.gif" height="50px" />
                    </td>
                    <td width="480px">
                     
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <asp:Label ID="Label4" runat="server" Text="Dosya :"></asp:Label>
                    </td>
                    <td width="480px">
                       <asp:FileUpload ID="fileuploadExcel" runat="server" onchange ="return checkFileExtension(this);"   />
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <asp:Label ID="Label5" runat="server" Text="Dosya :"></asp:Label>
                    </td>
                    <td width="480px">
                       <asp:Button ID="btnSend" runat="server" Text="Excelden Aktar" OnClick="btnSend_Click" />
                    </td>
                </tr>
            </table>
    </div>
    <input type="hidden" id="hdn_id" runat="server" />
    </form>
</body>
</html>