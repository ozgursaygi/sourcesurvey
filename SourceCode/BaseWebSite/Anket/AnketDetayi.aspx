<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AnketDetayi.aspx.cs" Inherits="BaseWebSite.Survey.AnketDetayi" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
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

            $("input, textarea  ").uniform();
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

        function SurveyiKapat() {
            $("#dialogSurveyKapat").dialog({
                height: 250,
                width: 600,
                modal: true,
                title: 'Close Survey',
                buttons: {
                    "Save": function () {
                        var aciklama = $("#txtaciklama").val();
                        __doPostBack('Kapat', '<%=anket_uid %>' + '^#^' + aciklama);
                    },
                    "Cancel": function () {
                        $(this).dialog("close");

                    }
                }

            })
        }

        function ArsiveGonder() {
            $("#dialogArsiveGonder").dialog({
                height: 250,
                width: 600,
                modal: true,
                title: 'Send To Archive',
                buttons: {
                    "Save": function () {
                        var aciklama = $("#txtArsivAciklamasi").val();
                        __doPostBack('ArsiveGonder', '<%=anket_uid %>' + '^#^' + aciklama);
                    },
                    "Cancel": function () {
                        $(this).dialog("close");

                    }
                }

            });
        }

        function ArsivdenCikart() {
            if (window.confirm("Do you want to remove from Archive?")) {
                __doPostBack('ArsivdenCikart', '<%=anket_uid %>');
            }
        }

        function OnIzleme() {
            window.open('Anket.aspx?anket_uid=<%=anket_uid %>&Preview=1', 'Survey', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=1200,height=850');
        }

        function SoruEkleme() {
            SoruEkle(1);

        }

        function SoruSirala() {
            SurveySoruSirala();
        }


        function SablondanGetir() {
            if (document.getElementById('hdn_ankete_cevap_verildimi').value != null && document.getElementById('hdn_ankete_cevap_verildimi').value != undefined && document.getElementById('hdn_ankete_cevap_verildimi').value == "1") {
                alert('If Answer Survey You can not add New Quenstion.');
                return;
            }

            $("#dialogSablondanGetir").dialog({
                height: 300,
                width: 650,
                modal: true,
                title: 'Şablonlar',
                buttons: {
                    "Save": function () {
                        var sablon_uid = $("#ddlSablonlar").val();
                        __doPostBack('SablondanGetir', sablon_uid);
                    },
                    "Cancel": function () {
                        $(this).dialog("close");

                    }
                }

            })
        }

        function SurveytenKopyala() {
            if (document.getElementById('hdn_ankete_cevap_verildimi').value != null && document.getElementById('hdn_ankete_cevap_verildimi').value != undefined && document.getElementById('hdn_ankete_cevap_verildimi').value == "1") {
                alert('Because This Survey is Answered You can not add New Quenstion.');
                return;
            }

            $("#dialogSurveytenGetir").dialog({
                height: 300,
                width: 650,
                modal: true,
                title: 'Surveyler',
                buttons: {
                    "Save": function () {
                        var sablon_uid = $("#ddlSurveyler").val();
                        __doPostBack('SurveytenGetir', sablon_uid);
                    },
                    "Cancel": function () {
                        $(this).dialog("close");

                    }
                }

            })
        }

        function TestiGonder() {
            
            if ('<%=anket_yayinda%>' == '4')
            {
                alert('This Survey is Online.');
            }

            $("#dialogTestiGonder").dialog({
                height: 350,
                width: 760,
                modal: true,
                title: 'Yayın Öncesi Görüş Al',
                buttons: {
                    "Save": function () {
                        var mailler = $("#txttest_mailler").val();
                        __doPostBack('TestiGonder', mailler);
                    },
                    "Cancel": function () {
                        $(this).dialog("close");

                    }
                }

            })
        }

        function TestSonuclari() {
            window.open('SurveyTestSonuclari.aspx?anket_uid=<%=anket_uid%>', 'SurveyTest', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=1200,height=768');
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

            if (document.getElementById('hdn_ankete_cevap_verildimi').value != null && document.getElementById('hdn_ankete_cevap_verildimi').value != undefined && document.getElementById('hdn_ankete_cevap_verildimi').value == "1") {
                alert('Because answered this Survey You can not add New Question.');
                return;
            }

//            if (document.getElementById('hdn_is_grup_admin').value != "1") {
//                alert('Surveye Soru ekleme yetkiniz bulunmamaktadır.Soru Ekleme Yetkisi Ekip Yöneticilerine verilmiştir.');
//                return;
//            }

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
                title: 'Add Question',
                buttons: {
                    "Save": function () {
                        var soru_tipi = $("#ddlsoru_tipi").val(),
			                 soru = $("#txtsoru").val(),
			                 soru_secenekleri = $("#txtsoru_secenekleri").val(),
			                 cevap_kolonları = $("#txtcevap_kolonları").val();

                        soru_text_satir = "false";

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
                        if (soru == "") message += "Please Fill Question Input...\n\r"
                        if (soru_tipi == "1" || soru_tipi == "2") {
                            if (soru_secenekleri == "") message += "Please Fill 'Question Choose' Input...\n\r"
                        }

                        if (soru_tipi == "7") {
                            if (soru_secenekleri == "") message += "Please Fill 'Text Header' Input...\n\r"
                        }
                        else if (soru_tipi == "3") {
                            if (soru_secenekleri == "") message += "Please Fill 'Question Choose' Input...\n\r"
                            if (cevap_kolonları == "") message += "Please Fill 'Answer Column' Input...\n\r"
                        }

                        if (message != "") {
                            alert(message);
                        }
                        else {
                            __doPostBack('SoruOlustur', soru_tipi + '^#^' + soru + "^#^" + soru_secenekleri + "^#^" + cevap_kolonları + "^#^" + soru_zorunlu + "^#^" + soru_sayisal_ondalik + "^#^" + soru_text_satir);
                        }
                    },
                    "Cancel": function () {
                        $(this).dialog("close");

                    }
                }

            });
        }

        function SoruDuzenle(soru_uid) {

            if (document.getElementById('hdn_ankete_cevap_verildimi').value != null && document.getElementById('hdn_ankete_cevap_verildimi').value != undefined && document.getElementById('hdn_ankete_cevap_verildimi').value == "1") {
                alert('Because answered this Survey You can not add New Question.');
                return;
            }

//            if (document.getElementById('hdn_is_grup_admin').value != "1") {
//                alert('Surveye Soru Ekleme yetkiniz bulunmamaktadır.Soru Ekleme Yetkisi Ekip Yöneticilerine verilmiştir.');
//                return;
//            }


            soru_tipi_change($("#ddlSoruTipi_Duzenle").val());

            $("#dialogSoruDuzenle").dialog({
                height: 570,
                width: 750,
                modal: true,
                title: 'Edit Answer',
                buttons: {
                    "Save": function () {
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
                            __doPostBack('SoruUpdate', soru_uid + '^#^' + soru_tipi + '^#^' + soru + "^#^" + soru_secenekleri + "^#^" + cevap_kolonları + "^#^" + soru_zorunlu + "^#^" + soru_sayisal_ondalik + "^#^" + soru_text_satir);
                        }
                    },
                    "Cancel": function () {
                        $(this).dialog("close");

                    }
                }

            });
        }


        function ddlsoru_tipi_changed() {
            soru_tipi_change($("#ddlsoru_tipi").val());

        }

        function ddlkategori_changed() {
            $.getJSON('AnketAshx/SablonGetir.ashx', { kategori_id: $("#ddlKategori").val() }, function (data) {

                $('#ddlSablonlar >option').remove();
                $.each(data, function (index, array) {
                    
                    $("<option value=\"" + array['id'] + "\">" + array['value'] + "</option>").appendTo("#ddlSablonlar");

                });

            });
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
                title: 'Edit Sort',
                buttons: {
                    "Save": function () {
                        __doPostBack('SoruSirala', getItemOrders());
                    },
                    "Cancel": function () {
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
            if (document.getElementById('hdn_ankete_cevap_verildimi').value != null && document.getElementById('hdn_ankete_cevap_verildimi').value != undefined && document.getElementById('hdn_ankete_cevap_verildimi').value == "1") {
                alert('Because answered this Survey You can not edit Question..');
                return;
            }

            __doPostBack('Duzenle', soru_uid);
        }

        function Sil(soru_uid) {
            if (document.getElementById('hdn_ankete_cevap_verildimi').value != null && document.getElementById('hdn_ankete_cevap_verildimi').value != undefined && document.getElementById('hdn_ankete_cevap_verildimi').value == "1") {
                alert('Because answered this Survey You can not delete Question..');
                return;
            }

            if (window.confirm('Do you want to delete this Question ?')) {
                __doPostBack('Sil', soru_uid);
            }
        }

        function SablonGoster() {
            if ($("#ddlSablonlar").val() == undefined || $("#ddlSablonlar").val() == null || $("#ddlSablonlar").val() == "") {
                return false;
            }
            else {
                window.open('../Admin/SablonGoster.aspx?sablon_uid=' + $("#ddlSablonlar").val() + '&Preview=1', 'Survey', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=1200,height=768');
                return false;
            }

        }

        function SurveyGoster() {
            if ($("#ddlSurveyler").val() == undefined || $("#ddlSurveyler").val() == null || $("#ddlSurveyler").val() == "") {
                return false;
            }
            else {
                window.open('AnketiGoster.aspx?anket_uid=' + $("#ddlSurveyler").val() + '&Preview=1', 'Survey', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=1200,height=768');
                return false;
            }
        }

    </script>
    <div style="width: 100%; margin: auto;">
        <div id="dialogSurveyKapat" title="Soru Ekle" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="580px">
                <tr>
                    <td colspan="2">
                        When Survey Closed If this Survey is online it is removed from Online Surveys  .
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <img src="../Images/_spacer.gif" width="20px" alt="" />
                    </td>
                </tr>
                <tr>
                    <td width="180px">
                        <asp:Label ID="lblaciklama" runat="server" Text="Closed Description :"></asp:Label>
                    </td>
                    <td width="400px">
                        <asp:TextBox ID="txtaciklama" runat="server" TextMode="MultiLine" Rows="4" Width="400px"
                            ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dialogArsiveGonder" title="Soru Ekle" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="580px">
                <tr>
                    <td colspan="2">
                        When Survey Closed If this Survey is online it is removed from Online Surveys .
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <img src="../Images/_spacer.gif" width="20px" alt="" />
                    </td>
                </tr>
                <tr>
                    <td width="180px">
                        <asp:Label ID="Label5" runat="server" Text="Description :"></asp:Label>
                    </td>
                    <td width="400px">
                        <asp:TextBox ID="txtArsivAciklamasi" runat="server" TextMode="MultiLine" Rows="4"
                            Width="400px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dialogSoruEkle" title="Soru Ekle" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="720px" >
                <tr >
                    <td width="240px" >
                        <asp:Label ID="lblsoru_tipi" runat="server" Text="Select Question Type :"></asp:Label>
                    </td>
                    <td width="480px">
                        <div class="pollSection">
                            <asp:DropDownList ID="ddlsoru_tipi" runat="server" ClientIDMode="Static">
                            </asp:DropDownList>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td width="240px">
                        <asp:Label ID="lblsoru" runat="server" Text="Please Input Question :"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:TextBox ID="txtsoru" runat="server" TextMode="MultiLine" Rows="4" Width="430px"
                            ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr id="div_text_satir">
                    <td width="240px">
                        <asp:Label ID="Label19" runat="server"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:CheckBox runat="server" ClientIDMode="Static" ID="chkTekSatir" Text="Only Onee Row" Checked="true" />
                    </td>
                </tr>
                <tr id="div_soru_secenekleri">
                    <td width="240px">
                        <asp:Label ID="Label3" runat="server" Text="Write Question Chooses line by line:"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:TextBox ID="txtsoru_secenekleri" runat="server" TextMode="MultiLine" Rows="10"
                            Width="430px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr id="div_cevap_kolonlari">
                    <td width="240px">
                        <asp:Label ID="Label4" runat="server" Text="Write Answer Columns line by line:"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:TextBox ID="txtcevap_kolonları" runat="server" TextMode="MultiLine" Rows="10"
                            Width="430px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr id="div_zorunlu">
                    <td width="240px">
                        <asp:Label ID="Label14" runat="server"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:CheckBox runat="server" ClientIDMode="Static" ID="chkcZorunlu" Text="Mandatory Question" />
                    </td>
                </tr>
                <tr>
                    <td width="240px">
                    </td>
                    <td width="480px">
                        <img src="../Images/_spacer.gif" width="5px" height="5px" alt="" />
                    </td>
                </tr>
                <tr id="div_sayisal">
                    <td width="240px">
                        <asp:Label ID="Label17" runat="server"></asp:Label>
                    </td>
                    <td width="480px">
                        <table>
                            <tr>
                                <td>
                                    <asp:RadioButton runat="server" ClientIDMode="Static" ID="rdOndalik" Text="Decimal Number"
                                        GroupName="sayisal"  />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:RadioButton runat="server" ClientIDMode="Static" ID="rdTamSayi" Text="Integer Number"
                                        GroupName="sayisal" Checked="true" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dialogSoruDuzenle" title="Edit Answer" style="display: none; font-size: 10pt">
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
                    <td width="240px">
                        <asp:Label ID="Label7" runat="server" Text="Please Input Question :"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:TextBox ID="txtsoru_duzenle" runat="server" TextMode="MultiLine" Rows="4" Width="430px"
                            ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr id="div_text_satir_duzenle">
                    <td width="240px">
                        <asp:Label ID="Label20" runat="server"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:CheckBox runat="server" ClientIDMode="Static" ID="chkTekSatir_duzenle" Text="Tek Satır" />
                    </td>
                </tr>
                <tr id="div_soru_secenekleri_duzenle">
                    <td width="240px">
                        <asp:Label ID="Label8" runat="server" Text="Write Question Chooses line by line:"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:TextBox ID="txtsoru_secenekleri_duzenle" runat="server" TextMode="MultiLine"
                            Rows="10" Width="430px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr id="div_cevap_kolonlari_duzenle">
                    <td width="240px">
                        <asp:Label ID="Label9" runat="server" Text="Write Answer Columns line by line:"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:TextBox ID="txtcevap_kolonları_duzenle" runat="server" TextMode="MultiLine"
                            Rows="10" Width="430px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr id="div_zorunlu_duzenle">
                    <td width="240px">
                        <asp:Label ID="Label16" runat="server"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:CheckBox runat="server" ClientIDMode="Static" ID="chkZorunlu_duzenle" Text="Mandatory Question" />
                    </td>
                </tr>
                <tr id="div_sayisal_duzenle">
                    <td width="240px">
                        <asp:Label ID="Label18" runat="server"></asp:Label>
                    </td>
                    <td width="480px">
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
            <div>Drag and Drop Questions and Click Save Button.</div>
        </div>
        <div id="dialogSablondanGetir" title="Şablonlar" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="630px"> 
                <tr>
                    <td width="150px">
                        <asp:Label ID="Label1" runat="server" Text="Select Category :"></asp:Label>
                    </td>
                    <td width="480px">
                        <div class="pollSection">
                            <asp:DropDownList ID="ddlKategori" runat="server" Width="350px" ClientIDMode="Static" >
                            </asp:DropDownList>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <asp:Label ID="Label10" runat="server" Text="Select Template :"></asp:Label>
                    </td>
                    <td width="480px">
                        <div class="pollSection">
                            <asp:DropDownList ID="ddlSablonlar" runat="server" Width="350px" ClientIDMode="Static">
                            </asp:DropDownList>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <img src="../Images/_spacer.gif" height="10px" alt="" />
                    </td>
                    <td width="480px">
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <asp:Label ID="Label11" runat="server"></asp:Label>
                    </td>
                    <td width="480px">
                        <a href="#" onclick="SablonGoster();">Show Template</a>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dialogSurveytenGetir" title="Surveyler" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="630px">
                <tr>
                    <td width="150px">
                        <asp:Label ID="Label12" runat="server" Text="Select Survey :"></asp:Label>
                    </td>
                    <td width="480px">
                        <div class="pollSection">
                            <asp:DropDownList ID="ddlSurveyler" runat="server" Width="350px" ClientIDMode="Static">
                            </asp:DropDownList>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <img src="../Images/_spacer.gif" height="10px" alt="" />
                    </td>
                    <td width="480px">
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <asp:Label ID="Label13" runat="server"></asp:Label>
                    </td>
                    <td width="480px">
                        <a href="#" onclick="SurveyGoster();">Show Survey</a>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dialogTestiGonder" title="Send Test Survey" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="740px">
                <tr>
                    <td width="260px">
                        <asp:Label ID="Label15" runat="server" Text="Gönderilecek E-Postaları Alt Alta Giriniz :"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:TextBox ID="txttest_mailler" runat="server" TextMode="MultiLine" Rows="8" Width="430px"
                            ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div id="smoothmenu1" class="ddsmoothmenu">
            <asp:Literal ID="ltlMenu" runat="server"></asp:Literal>
        </div>
        <div id="mainLeft" style="height: 100%">
            <div class="boxLeft">
                <div class="boxLeftHeaders">
                    Question Types
                </div>
                <div class="explanation" style="padding-left:10px">Drag and Drop Questions Types to Right Panel Top Header. Or Click "Add Answer" from Menu .</div>
                <div class="boxLeftContent">
                    <ul class="questionsDefinitions sortable">
                         <li>
                            <div id="Div1">
                                <a href="#" ></a></div>
                        </li>
                        <li>
                            <div id="draggable_coklu_secenek">
                                <a href="#" class="coklu">Radio Button Select</a></div>
                        </li>
                        <li>
                            <div id="draggable_coklu_secenek_coklu_secim">
                                <a href="#" class="coklu">Check Box Multi Select</a></div>
                        </li>
                        <li>
                            <div id="draggable_matris">
                                <a href="#" class="matris">Matrix</a></div>
                        </li>
                        <li>
                            <div id="draggable_text">
                                <a href="#" class="text">Text</a></div>
                        </li>
                        <li>
                            <div id="draggable_coklu_text">
                                <a href="#" class="cokluText">Multiple Text</a></div>
                        </li>
                        <li>
                            <div id="draggable_tarih">
                                <a href="#" class="tarih">Date</a></div>
                        </li>
                        <li>
                            <div id="draggable_sayisal">
                                <a href="#" class="sayisal">Numerical</a></div>
                        </li>
                        <li>
                            <div id="draggable_telefon">
                                <a href="#" class="telefon">Phone</a></div>
                        </li>
                        <li>
                            <div id="draggable_eposta">
                                <a href="#" class="eposta">E-Mail</a></div>
                        </li>
                        
                    </ul>
                </div>
                <br />
                <br />
                <div class="explanation" style="padding-left:10px">Drag and Drop Questions Types to Right Panel Top Header. Or Click "Add Answer" from Menu.</div>
            </div>
        </div>
         <div id="mainRight" style="height: 100%">
            <div class="boxRight">
                <div class="boxRightHeaders">
                    <asp:LinkButton ID="LinkButtonHeader" Font-Size="23px" ForeColor="White" Font-Underline="false"
                        runat="server"></asp:LinkButton>
                </div>
                <div class="boxRightContent">
                    <asp:Repeater ID="rptSurveySorulari" runat="server">
                        <HeaderTemplate>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="makeQuestion">
                                <div class="questionNumber">
                                    Q
                                     <%#DataBinder.Eval(Container.DataItem, "RowNumber")%>
                                    :</div>
                                <div class="questionName">
                                    <%#DataBinder.Eval(Container.DataItem, "soru")%></div>
                                <div class="questionContent">
                                    <%# SorulariOlustur(DataBinder.Eval(Container.DataItem, "soru_uid"))%>
                                </div>
                                <div >
                                    <div><img src="../Images/_spacer.gif" width="10px" alt="" /></div>
                                    <a class="duzenle linkButtonStyle"  href="#" onclick="Duzenle('<%#DataBinder.Eval(Container.DataItem, "soru_uid")%>');">
                                        Edit</a> <a class="sil linkButtonStyle"  href="#" onclick="Sil('<%#DataBinder.Eval(Container.DataItem, "soru_uid")%>');">
                                            Delete</a>
                                    <div><img src="../Images/_spacer.gif" width="10px" alt="" /></div>
                                </div>
                            </div>
                        </ItemTemplate>
                        <SeparatorTemplate>
                        </SeparatorTemplate>
                        <AlternatingItemTemplate>
                            <div class="makeQuestion lightBg">
                                <div class="questionNumber">
                                    Q
                                    <%#DataBinder.Eval(Container.DataItem, "RowNumber")%>
                                    :</div>
                                <div class="questionName">
                                    <%#DataBinder.Eval(Container.DataItem, "soru")%></div>
                                <div class="questionContent">
                                    <%# SorulariOlustur(DataBinder.Eval(Container.DataItem, "soru_uid"))%>
                                </div>
                                <div >
                                     <div><img src="../Images/_spacer.gif" width="10px" alt="" /></div>
                                    <a class="duzenle linkButtonStyle" href="#" onclick="Duzenle('<%#DataBinder.Eval(Container.DataItem, "soru_uid")%>');">
                                        Edit</a> <a class="sil linkButtonStyle" href="#" onclick="Sil('<%#DataBinder.Eval(Container.DataItem, "soru_uid")%>');">
                                            Delete</a>
                                     <div><img src="../Images/_spacer.gif" width="10px" alt="" /></div>
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
    <input type="hidden" id="hdn_ankete_cevap_verildimi" runat="server" clientidmode="Static" />
    <input type="hidden" id="hdn_is_grup_admin" runat="server" clientidmode="Static" />
</asp:Content>
