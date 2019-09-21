<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SablonDetayi.aspx.cs" Inherits="BaseWebSite.Admin.SablonDetayi" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/cupertino/jquery-ui-1.8.17.custom.css" rel="stylesheet" type="text/css" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" type="text/css" href="../css/custom-theme/jquery-ui-1.8.20.custom.css" />
    <link rel="stylesheet" href="../css/uniform.default.css" type="text/css" media="screen" />
    <link rel="stylesheet" type="text/css" href="../css/ddsmoothmenu.css" />
    <style type="text/css">
        #sortable
        {
            list-style-type: none;
            margin: 0;
            padding: 0;
            width: 60%;
        }
        #sortable li
        {
            margin: 0 3px 3px 3px;
            padding: 0.4em;
            padding-left: 1.5em;
            font-size: 1.4em;
            height: 18px;
        }
        #sortable li span
        {
            position: absolute;
            margin-left: -1.3em;
        }
    </style>
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.20.custom.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.numeric.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.maskedinput-1.2.2.js" type="text/javascript"></script>
    <script src="../Scripts/JQ/jquery.ui.datepicker-tr.js" type="text/javascript"></script>
    <script src="../js/ddsmoothmenu.js" type="text/javascript"></script>
    <script src="../js/jquery.uniform.js" type="text/javascript" charset="utf-8"></script>
    <script type="text/javascript">
        $(function () {
            $(".sortable").sortable({
                revert: true
            });
            $("ul, li").disableSelection();

            $("input, textarea").uniform();
        });

        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#sortable").sortable();
            $("#sortable").disableSelection();
            $(".datepicker").datepicker({ dateFormat: 'dd-mm-yy', onSelect: function (dateText, inst) {

            }
            });
            $(".phone").mask("(999)999-9999");

            ddsmoothmenu.init({
                mainmenuid: "smoothmenu1",
                orientation: 'h',
                classname: 'ddsmoothmenu',
                contentsource: "markup"
            })
        });

        function SablonuKapat() {
            $("#dialogSablonKapat").dialog({
                height: 250,
                width: 600,
                modal: true,
                title: 'Şablonu Kapat',
                buttons: {
                    "Kaydet": function () {
                        var aciklama = $("#txtaciklama").val();
                        __doPostBack('Kapat', '<%=sablon_uid %>' + '^#^' + aciklama);
                    },
                    "Vazgeç": function () {
                        $(this).dialog("close");

                    }
                }

            })
        }

        function SablonuAc() {
            __doPostBack('SablonuAc', 'SablonuAc');
        }

        function SoruEkleme() {
            SoruEkle(1);

        }

        function SoruSirala() {
            SurveySoruSirala();
        }


        $(document).ready(function () {



            $(".numeric").numeric({ decimal: "," });
            $(".integer").numeric(false);
        });


        function pageLoad() {

        }

        $(function () {
            $(this).find("#draggable_coklu_secenek").draggable({ revert: "valid", helper: "clone", cursor: "move" });
            $(this).find("#draggable_matris").draggable({ revert: "valid", helper: "clone", cursor: "move" });
            $(this).find("#draggable_text").draggable({ revert: "valid", helper: "clone", cursor: "move" });
            $(this).find("#draggable_true_false").draggable({ revert: "valid", helper: "clone", cursor: "move" });
            $(this).find("#draggable_coklu_secenek_coklu_secim").draggable({ revert: "valid", helper: "clone", cursor: "move" });
            $(this).find("#draggable_coklu_text").draggable({ revert: "valid", helper: "clone", cursor: "move" });
            $(this).find("#draggable_tarih").draggable({ revert: "valid", helper: "clone", cursor: "move" });
            $(this).find("#draggable_sayisal").draggable({ revert: "valid", helper: "clone", cursor: "move" });
            $(this).find("#draggable_telefon").draggable({ revert: "valid", helper: "clone", cursor: "move" });
            $(this).find("#draggable_eposta").draggable({ revert: "valid", helper: "clone", cursor: "move" });
            $(this).find("#draggable_evet_hayir").draggable({ revert: "valid", helper: "clone", cursor: "move" });

            $(this).find("#mainRight").droppable({
                drop: function (event, ui) {
                    if (ui.draggable.attr('id') == "draggable_coklu_secenek") {
                        SoruEkle(1);
                    }
                    else if (ui.draggable.attr('id') == "draggable_coklu_secenek_coklu_secim") {
                        SoruEkle(2);
                    }
                    else if (ui.draggable.attr('id') == "draggable_matris") {
                        SoruEkle(3);
                    }
                    else if (ui.draggable.attr('id') == "draggable_text") {
                        SoruEkle(4);
                    }
                    else if (ui.draggable.attr('id') == "draggable_true_false") {
                        SoruEkle(5);
                    }
                    else if (ui.draggable.attr('id') == "draggable_evet_hayir") {
                        SoruEkle(6);
                    }
                    else if (ui.draggable.attr('id') == "draggable_coklu_text") {
                        SoruEkle(7);
                    }
                    else if (ui.draggable.attr('id') == "draggable_tarih") {
                        SoruEkle(9);
                    }
                    else if (ui.draggable.attr('id') == "draggable_sayisal") {
                        SoruEkle(8);
                    }
                    else if (ui.draggable.attr('id') == "draggable_telefon") {
                        SoruEkle(10);
                    }
                    else if (ui.draggable.attr('id') == "draggable_eposta") {
                        SoruEkle(11);
                    }
                }
            });
        });

        function SoruEkle(tip) {


            $("#txtsoru_secenekleri").val("");
            $("#txtsoru").val("");
            
            if (tip == 1) {
                $("#ddlsoru_tipi").val("1");
                soru_tipi_change($("#ddlsoru_tipi").val());
            }
            else if (tip == 2) {
                $("#ddlsoru_tipi").val("2");
                soru_tipi_change($("#ddlsoru_tipi").val());
            }
            else if (tip == 3) {
                $("#ddlsoru_tipi").val("3");
                soru_tipi_change($("#ddlsoru_tipi").val());
            }
            else if (tip == 4) {
                $("#ddlsoru_tipi").val("4");
                soru_tipi_change($("#ddlsoru_tipi").val());
            }
            else if (tip == 5) {
                $("#ddlsoru_tipi").val("5");
                soru_tipi_change($("#ddlsoru_tipi").val());
            }
            else if (tip == 6) {
                $("#ddlsoru_tipi").val("6");
                soru_tipi_change($("#ddlsoru_tipi").val());
            }
            else if (tip == 7) {
                $("#ddlsoru_tipi").val("7");
                soru_tipi_change($("#ddlsoru_tipi").val());
            }
            else if (tip == 8) {
                $("#ddlsoru_tipi").val("8");
                soru_tipi_change($("#ddlsoru_tipi").val());
            }
            else if (tip == 9) {
                $("#ddlsoru_tipi").val("9");
                soru_tipi_change($("#ddlsoru_tipi").val());
            }
            else if (tip == 10) {
                $("#ddlsoru_tipi").val("10");
                soru_tipi_change($("#ddlsoru_tipi").val());
            }
            else if (tip == 11) {
                $("#ddlsoru_tipi").val("11");
                soru_tipi_change($("#ddlsoru_tipi").val());
            }
            
            $("#dialogSoruEkle").dialog({
                height: 570,
                width: 750,
                modal: true,
                title: 'Soru Ekle',
                buttons: {
                    "Kaydet": function () {
                        var soru_tipi = $("#ddlsoru_tipi").val(),
			                 soru = $("#txtsoru").val(),
			                 soru_secenekleri = $("#txtsoru_secenekleri").val(),
			                 cevap_kolonları = $("#txtcevap_kolonları").val();

                        var soru_text_satir = "false";

                        if ($("#chkTekSatir").attr('checked') == "checked") {
                            soru_text_satir = "true";
                        }

                        var soru_zorunlu = "false";

                        if ($("#chkcZorunlu").attr('checked') == "checked") {
                            soru_zorunlu = "true";
                        }

                        var soru_sayisal_ondalik = "false";

                        if ($("#rdOndalik").attr('checked') == "checked") {

                            soru_sayisal_ondalik = "true";
                        }


                        var message = "";
                        if (soru == "") message += "Soru alanını boş geçmeyiniz...\n\r"
                        if (soru_tipi == "1" || soru_tipi == "2") {
                            if (soru_secenekleri == "") message += "Soru Optionsi alanını boş geçmeyiniz...\n\r"
                        }

                        if (soru_tipi == "7") {
                            if (soru_secenekleri == "") message += "Text Başlıkları alanını boş geçmeyiniz...\n\r"
                        }
                        else if (soru_tipi == "3") {
                            if (soru_secenekleri == "") message += "Soru Optionsi alanını boş geçmeyiniz...\n\r"
                            if (cevap_kolonları == "") message += "Cevap Kolonları alanını boş geçmeyiniz...\n\r"
                        }

                        if (message != "") {
                            alert(message);
                        }
                        else {
                            __doPostBack('SoruOlustur', soru_tipi + '^#^' + soru + "^#^" + soru_secenekleri + "^#^" + cevap_kolonları + "^#^" + soru_sayisal_ondalik + "^#^" + soru_text_satir);
                        }
                    },
                    "Vazgeç": function () {
                        $(this).dialog("close");

                    }
                }

            });
        }

        function SoruDuzenle(soru_uid) {



            soru_tipi_change($("#ddlSoruTipi_Duzenle").val());

            $("#dialogSoruDuzenle").dialog({
                height: 570,
                width: 750,
                modal: true,
                title: 'Edit Answer',
                buttons: {
                    "Kaydet": function () {
                        var soru_tipi = $("#ddlSoruTipi_Duzenle").val(),
			                 soru = $("#txtsoru_duzenle").val(),
			                 soru_secenekleri = $("#txtsoru_secenekleri_duzenle").val(),
                             cevap_kolonları = $("#txtcevap_kolonları_duzenle").val();
                        var soru_zorunlu = "false";

                        if ($("#chkZorunlu_duzenle").attr('checked') == "checked") {
                            soru_zorunlu = "true";
                        }

                        var soru_text_satir = "false";

                        if ($("#chkTekSatir_duzenle").attr('checked') == "checked") {
                            soru_text_satir = "true";
                        }

                        var soru_sayisal_ondalik = "false";

                        if ($("#rdOndalik_duzenle").attr('checked') == "checked") {
                            soru_sayisal_ondalik = "true";
                        }

                        var message = "";
                        if (soru == "") message += "Soru alanını boş geçmeyiniz...\n\r"
                        if (soru_tipi == "1" || soru_tipi == "2") {
                            if (soru_secenekleri == "") message += "Soru Optionsi alanını boş geçmeyiniz...\n\r"
                        }

                        if (soru_tipi == "7") {
                            if (soru_secenekleri == "") message += "Text Başlıkları alanını boş geçmeyiniz...\n\r"
                        }

                        else if (soru_tipi == "3") {
                            if (soru_secenekleri == "") message += "Soru Optionsi alanını boş geçmeyiniz...\n\r"
                            if (cevap_kolonları == "") message += "Cevap Kolonları alanını boş geçmeyiniz...\n\r"
                        }

                        if (message != "") {
                            alert(message);
                        }
                        else {
                            __doPostBack('SoruUpdate', soru_uid + '^#^' + soru_tipi + '^#^' + soru + "^#^" + soru_secenekleri + "^#^" + cevap_kolonları + "^#^" + soru_sayisal_ondalik + "^#^" + soru_text_satir);
                        }
                    },
                    "Vazgeç": function () {
                        $(this).dialog("close");

                    }
                }

            });
        }


        function ddlsoru_tipi_changed() {
            soru_tipi_change($("#ddlsoru_tipi").val());

        }

        function soru_tipi_change(soru_tipi) {
            $("#div_text_satir").hide();
            $("#div_text_satir_duzenle").hide();
            if (soru_tipi == 1) {
                $("#div_soru_secenekleri").show();
                $("#div_cevap_kolonlari").hide();
                $("#div_soru_secenekleri_duzenle").show();
                $("#div_cevap_kolonlari_duzenle").hide();
                $("#div_sayisal").hide();
                $("#div_sayisal_duzenle").hide();
            }
            else if (soru_tipi == 2) {
                $("#div_soru_secenekleri").show();
                $("#div_cevap_kolonlari").hide();
                $("#div_soru_secenekleri_duzenle").show();
                $("#div_cevap_kolonlari_duzenle").hide();
                $("#div_sayisal").hide();
                $("#div_sayisal_duzenle").hide();
            }
            else if (soru_tipi == 3) {
                $("#div_cevap_kolonlari").show();
                $("#div_soru_secenekleri").show();
                $("#div_soru_secenekleri_duzenle").show();
                $("#div_cevap_kolonlari_duzenle").show();
                $("#div_sayisal").hide();
                $("#div_sayisal_duzenle").hide();
            }
            else if (soru_tipi == 4) {
                $("#div_cevap_kolonlari").hide();
                $("#div_soru_secenekleri").hide();
                $("#div_soru_secenekleri_duzenle").hide();
                $("#div_cevap_kolonlari_duzenle").hide();
                $("#div_sayisal").hide();
                $("#div_sayisal_duzenle").hide();
                $("#div_text_satir").show();
                $("#div_text_satir_duzenle").show();
            }
            else if (soru_tipi == 5) {
                $("#div_cevap_kolonlari").hide();
                $("#div_soru_secenekleri").hide();
                $("#div_soru_secenekleri_duzenle").hide();
                $("#div_cevap_kolonlari_duzenle").hide();
                $("#div_sayisal").hide();
                $("#div_sayisal_duzenle").hide();
            }
            else if (soru_tipi == 6) {
                $("#div_cevap_kolonlari").hide();
                $("#div_soru_secenekleri").hide();
                $("#div_soru_secenekleri_duzenle").hide();
                $("#div_cevap_kolonlari_duzenle").hide();
                $("#div_sayisal").hide();
                $("#div_sayisal_duzenle").hide();
            }
            else if (soru_tipi == 7) {
                $("#div_cevap_kolonlari").hide();
                $("#div_soru_secenekleri").show();
                $("#div_soru_secenekleri_duzenle").show();
                $("#div_cevap_kolonlari_duzenle").hide();
                $("#div_sayisal").hide();
                $("#div_sayisal_duzenle").hide();
            }
            else if (soru_tipi == 8) {
                $("#div_cevap_kolonlari").hide();
                $("#div_soru_secenekleri").hide();
                $("#div_sayisal").show();
                $("#div_sayisal_duzenle").show();
                $("#div_soru_secenekleri_duzenle").hide();
                $("#div_cevap_kolonlari_duzenle").hide();

            }
            else if (soru_tipi == 9) {
                $("#div_cevap_kolonlari").hide();
                $("#div_soru_secenekleri").hide();
                $("#div_sayisal").hide();
                $("#div_sayisal_duzenle").hide();
                $("#div_soru_secenekleri_duzenle").hide();
                $("#div_cevap_kolonlari_duzenle").hide();

            }
            else if (soru_tipi == 10) {
                $("#div_cevap_kolonlari").hide();
                $("#div_soru_secenekleri").hide();
                $("#div_sayisal").hide();
                $("#div_sayisal_duzenle").hide();
                $("#div_soru_secenekleri_duzenle").hide();
                $("#div_cevap_kolonlari_duzenle").hide();

            }
            else if (soru_tipi == 11) {
                $("#div_cevap_kolonlari").hide();
                $("#div_soru_secenekleri").hide();
                $("#div_sayisal").hide();
                $("#div_sayisal_duzenle").hide();
                $("#div_soru_secenekleri_duzenle").hide();
                $("#div_cevap_kolonlari_duzenle").hide();
            }

        }



        function SurveySoruSirala() {
            $("#dialogSoruSirala").dialog({
                height: 600,
                width: 800,
                modal: true,
                title: 'Sıralamayı Düzenle',
                buttons: {
                    "Kaydet": function () {
                        __doPostBack('SoruSirala', getItemOrders());
                    },
                    "Vazgeç": function () {
                        $(this).dialog("close");

                    }
                }

            });
        }


        function getItemOrders() {
            var oUl = document.getElementById("sortable");
            var orderOfItems = "";
            var x;

            var listItems = $("#sortable > li");
            for (var i = 0; i < listItems.length; i++) {
                if (orderOfItems.length > 0)
                    orderOfItems += ",";
                x = listItems[i];
                ind = (i + 1);
                orderOfItems += x.id + "=" + ind.toString();
            }
            return orderOfItems;
        }



        function Duzenle(soru_uid) {
            __doPostBack('Duzenle', soru_uid);
        }

        function Sil(soru_uid) {


            if (window.confirm('İlgili soruyu silmek istiyor musunuz?')) {
                __doPostBack('Sil', soru_uid);
            }
        }




    </script>
    <div style="width: 100%; margin: auto;">
        <div id="dialogSablonKapat" title="Soru Ekle" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="580px">
                <tr>
                    <td colspan="2">
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <img src="../Images/_spacer.gif" width="20px" alt="" />
                    </td>
                </tr>
                <tr>
                    <td width="180px">
                        <asp:Label ID="lblaciklama" runat="server" Text="Kapatma Açıklaması :"></asp:Label>
                    </td>
                    <td width="400px">
                        <asp:TextBox ID="txtaciklama" runat="server" TextMode="MultiLine" Rows="4" Width="400px"
                            ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dialogSoruEkle" title="Soru Ekle" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="720px">
                <tr>
                    <td width="240px">
                        <asp:Label ID="lblsoru_tipi" runat="server" Text="Soru Tipi Seçiniz:"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:DropDownList ID="ddlsoru_tipi" runat="server" Width="200px" ClientIDMode="Static">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblsoru" runat="server" Text="Sorunuzu Giriniz :"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtsoru" runat="server" TextMode="MultiLine" Rows="4" Width="430px"
                            ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr id="div_text_satir">
                    <td>
                        <asp:Label ID="Label19" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ClientIDMode="Static" ID="chkTekSatir" Text="Tek Satır" Checked="true" />
                    </td>
                </tr>
                <tr id="div_soru_secenekleri">
                    <td>
                        <asp:Label ID="Label3" runat="server" Text="Write Question Chooses line by line:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtsoru_secenekleri" runat="server" TextMode="MultiLine" Rows="10"
                            Width="430px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr id="div_cevap_kolonlari">
                    <td>
                        <asp:Label ID="Label4" runat="server" Text="Write Answer Columns line by line:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtcevap_kolonları" runat="server" TextMode="MultiLine" Rows="10"
                            Width="430px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr id="div_sayisal">
                    <td>
                        <asp:Label ID="Label17" runat="server"></asp:Label>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <asp:RadioButton runat="server" ClientIDMode="Static" ID="rdOndalik" Text="Ondalık Sayı"
                                        GroupName="sayisal" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:RadioButton runat="server" ClientIDMode="Static" ID="rdTamSayi" Text="Tam Sayı"
                                        GroupName="sayisal" Checked="true" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dialogSoruDuzenle" title="Soruyu Düzenle" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="720px">
                <tr>
                    <td width="240px">
                        <asp:Label ID="Label6" runat="server" Text="Select Question Type :"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:DropDownList ID="ddlSoruTipi_Duzenle" runat="server" Width="200px" ClientIDMode="Static"
                            Enabled="false">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label7" runat="server" Text="Sorunuzu Giriniz :"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtsoru_duzenle" runat="server" TextMode="MultiLine" Rows="4" Width="430px"
                            ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr id="div_text_satir_duzenle">
                    <td>
                        <asp:Label ID="Label20" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ClientIDMode="Static" ID="chkTekSatir_duzenle" Text="Tek Satır" />
                    </td>
                </tr>
                <tr id="div_soru_secenekleri_duzenle">
                    <td>
                        <asp:Label ID="Label8" runat="server" Text="Write Question Chooses line by line:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtsoru_secenekleri_duzenle" runat="server" TextMode="MultiLine"
                            Rows="10" Width="430px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr id="div_cevap_kolonlari_duzenle">
                    <td>
                        <asp:Label ID="Label9" runat="server" Text="Write Answer Columns line by line:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtcevap_kolonları_duzenle" runat="server" TextMode="MultiLine"
                            Rows="10" Width="430px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr id="div_sayisal_duzenle">
                    <td>
                        <asp:Label ID="Label18" runat="server"></asp:Label>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <asp:RadioButton runat="server" ClientIDMode="Static" ID="rdOndalik_duzenle" Text="Ondalık Sayı"
                                        GroupName="sayisal" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:RadioButton runat="server" ClientIDMode="Static" ID="rdTamSayi_duzenle" Text="Tam Sayı"
                                        GroupName="sayisal" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dialogSoruSirala" title="Survey Sırala" style="display: none; font-size: 10pt">
            <div style="border: solid 1px black; margin: 5px 5px 5px 5px; padding: 2px 2px 2px 2px;
                background-color: White">
                <ul id="sortable">
                    <asp:Repeater ID="rptSira" runat="server">
                        <ItemTemplate>
                            <li class="ui-state-default" id="<%# DataBinder.Eval(Container.DataItem, "soru_uid") %>"
                                style="width: 700px"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>Sıra
                                No :
                                <%# DataBinder.Eval(Container.DataItem, "RowNumber")%>
                                -
                                <%# DataBinder.Eval(Container.DataItem, "soru_kisa")%></li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
             <div>Soruları sürükleyip bırakarak yerlerini değiştirin ve Kaydet butonuna basınız.</div>
        </div>
        <div id="smoothmenu1" class="ddsmoothmenu">
            <asp:Literal ID="ltlMenu" runat="server"></asp:Literal>
        </div>
        <div id="mainLeft">
            <div class="boxLeft">
                <div class="boxLeftHeaders">
                    Soru Tipi Optionsi
                </div>
                <div class="boxLeftContent">
                    <ul class="questionsDefinitions sortable">
                        <li>
                            <div id="draggable_coklu_secenek">
                                <a href="#" class="coklu">Çoklu Seçenek</a></div>
                        </li>
                        <li>
                            <div id="draggable_coklu_secenek_coklu_secim">
                                <a href="#" class="coklu">Çoklu Seçenek(Çoklu)</a></div>
                        </li>
                        <li>
                            <div id="draggable_matris">
                                <a href="#" class="matris">Matris</a></div>
                        </li>
                        <li>
                            <div id="draggable_text">
                                <a href="#" class="text">Text</a></div>
                        </li>
                        <li>
                            <div id="draggable_coklu_text">
                                <a href="#" class="cokluText">Çoklu Text</a></div>
                        </li>
                        <li>
                            <div id="draggable_tarih">
                                <a href="#" class="tarih">Tarih</a></div>
                        </li>
                        <li>
                            <div id="draggable_sayisal">
                                <a href="#" class="sayisal">Sayısal</a></div>
                        </li>
                        <li>
                            <div id="draggable_telefon">
                                <a href="#" class="telefon">Telefon</a></div>
                        </li>
                        <li>
                            <div id="draggable_eposta">
                                <a href="#" class="eposta">E-Posta</a></div>
                        </li>
                        <li>
                            <div id="draggable_true_false">
                                <a href="#" class="dogruYanlis">Doğru / Yanlış</a></div>
                        </li>
                        <li>
                            <div id="draggable_evet_hayir">
                                <a href="#" class="evetHayir">Evet / Hayır</a></div>
                        </li>
                    </ul>
                </div>
                <br />
                <div class="explanation" style="padding-left:10px">Sağ Panel Başlık Alanına Sürükleyip Bırakınız.</div>
            </div>
        </div>
        <div id="mainRight" style="height: 100%">
            <div class="boxRight">
                <div class="boxRightHeaders">
                    <asp:LinkButton ID="LinkButtonHeader" Font-Size="23px" ForeColor="White" Font-Underline="false"
                        runat="server"></asp:LinkButton>
                </div>
                <div class="boxRightContent">
                    <asp:Repeater ID="rptSablonSorulari" runat="server">
                        <HeaderTemplate>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="makeQuestion">
                                <div class="questionNumber">
                                    Soru
                                    <%#DataBinder.Eval(Container.DataItem, "RowNumber")%>
                                    :</div>
                                <div class="questionName">
                                    <%#DataBinder.Eval(Container.DataItem, "soru")%></div>
                                <div class="questionContent">
                                    <p>
                                        <%# SorulariOlustur(DataBinder.Eval(Container.DataItem, "soru_uid"))%>
                                    </p>
                                </div>
                                 <div class="surveyListButtons">
                                    <a class="duzenle linkButtonStyle" href="#" onclick="Duzenle('<%#DataBinder.Eval(Container.DataItem, "soru_uid")%>');">
                                        Düzenle</a> <a class="sil linkButtonStyle" href="#" onclick="Sil('<%#DataBinder.Eval(Container.DataItem, "soru_uid")%>');">
                                            Sil</a>
                                </div>
                            </div>
                        </ItemTemplate>
                        <SeparatorTemplate>
                        </SeparatorTemplate>
                        <AlternatingItemTemplate>
                            <div class="makeQuestion lightBg">
                                <div class="questionNumber">
                                    Soru
                                    <%#DataBinder.Eval(Container.DataItem, "RowNumber")%>
                                    :</div>
                                <div class="questionName">
                                    <%#DataBinder.Eval(Container.DataItem, "soru")%></div>
                                <div class="questionContent">
                                    <p>
                                        <%# SorulariOlustur(DataBinder.Eval(Container.DataItem, "soru_uid"))%>
                                    </p>
                                </div>
                                 <div class="surveyListButtons">
                                    <a class="duzenle linkButtonStyle" href="#" onclick="Duzenle('<%#DataBinder.Eval(Container.DataItem, "soru_uid")%>');">
                                        Düzenle</a> <a class="sil linkButtonStyle" href="#" onclick="Sil('<%#DataBinder.Eval(Container.DataItem, "soru_uid")%>');">
                                            Sil</a>
                                </div>
                            </div>
                        </AlternatingItemTemplate>
                        <FooterTemplate>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
