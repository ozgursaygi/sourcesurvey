<%@ Page Title="Survey - Easy and Fast Solution" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="BaseWebSite._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
 <link rel="stylesheet" type="text/css" href="css/custom-theme/jquery-ui-1.8.20.custom.css" />
    <link rel="stylesheet" href="css/uniform.default.css" type="text/css" media="screen" />
    <link href="css/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="css/ddsmoothmenu.css" />
    <link rel="image_src" href="img/sliderImage.png" style="width:100px" />
    <script src="js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-1.8.20.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery.numeric.js" type="text/javascript"></script>
    <script src="Scripts/jquery.maskedinput-1.2.2.js" type="text/javascript"></script>
    <script src="js/ddsmoothmenu.js" type="text/javascript"></script>
    <script src="js/jquery.uniform.js" type="text/javascript" charset="utf-8"></script>
     <%-- <script src="js/cufon-yui.js" type="text/javascript"></script>--%>
    <script src="js/Diavlo.font.js" type="text/javascript"></script>
    <script src="js/jquery.fancybox.pack.js" type="text/javascript"></script>
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
            //Cufon.replace('.slideContent h1');
            //Cufon.replace('.slideContent h2');
            //Cufon.replace('.slideContent h3');
            //Cufon.replace('.homePageBox h2');
//            $(".fancybox").fancybox();
        });
        </script>
          <div class="slider">
            <div class="slide" > <img src="img/2cd43b_7a6b9e4e84044e6492beea817c670bb6_mv2.png" alt="" width="209px" height="209px"/>
              <div class="slideContent">
                <h1>Online Survey and Form <br />Creations</h1>
                <h2>Simple and Fast Solution.</h2>
                <h3>User Friendly.</h3>
                </div>
            </div>
          </div>
          <%--<div class="homePageBox"><a class="fancybox" rel="group" href="img/anketOlustur.png"><img src="img/anketOlustur.png" alt=""  /></a>--%>
          <div class="homePageBox"><img src="img/anketOlustur2.png" alt=""  />
            <h2>Creation Survey and Forms</h2>
            <p>You can prepare your questions , surveys in very short time.Also copy from previous Surveys.</p>
            </div>
          <div class="homePageBox"> <img src="img/sonuclariTopla2.png" alt=""  />
            <h2>Monitoring Surveys and Forms</h2>
            <p>You can always track the responses of your surveys and forms. You can download the results on your computer.</p>
            </div>
          <div class="homePageBox lastBox"> <img src="img/raporOlustur2.png" alt=""  />
            <h2>Reporting Surveys and Forms</h2>
            <p>You can analyze the graphical and numerical reports to make your healthier and more efficient decisions and results-oriented work more efficient.</p>
            </div>
             
</asp:Content>
