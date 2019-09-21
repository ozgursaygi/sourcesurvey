<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RepeaterSurveySayfalama.ascx.cs" Inherits="BaseWebSite.Survey.Controls.RepeaterSurveySayfalama" %>
<div id="paging" class="nav" runat="server">
    <asp:ImageButton ID="LnkPrev" runat="server" OnClick="LnkPrev_Click" CssClass="previousNav" AlternateText="" ></asp:ImageButton>
    <asp:ImageButton ID="lnknext" runat="server" OnClick="lnknext_Click" CssClass="nextNav" AlternateText="" ></asp:ImageButton>
</div>
<div class="pageNumber">
    <asp:Label ID="LblDurum" runat="server"></asp:Label></div>
     <div class="totalQuestionNumber"><asp:Label ID="lblSoruSayisi" runat="server"></asp:Label></div>

