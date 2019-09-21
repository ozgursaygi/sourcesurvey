<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RepeaterPaging.ascx.cs"
    Inherits="BaseWebSite.BaseControls.RepeaterPaging" %>
<div id="paging" class="paging" runat="server">
    <ul>
        <li>
            <asp:LinkButton ID="Lnkfirst" runat="server" OnClick="Lnkfirst_Click" Width="100%" Height="100%">First</asp:LinkButton></li>
        <li>
            <asp:LinkButton ID="LnkPrev" runat="server" OnClick="LnkPrev_Click" Width="100%" Height="100%">Back</asp:LinkButton></li>
        
        <asp:Repeater ID="rptPages" runat="server" OnItemCommand="rptPages_ItemCommand" OnItemDataBound="rptPages_ItemDataBound">
            <ItemTemplate>
               <li > <asp:LinkButton ID="lnkbtnPaging" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "PageIndex")%>'
                    CommandName="lnkbtnPaging" Text='<%#DataBinder.Eval(Container.DataItem, "PageText")%>' Width="100%" Height="100%"></asp:LinkButton></li>
            </ItemTemplate>
        </asp:Repeater>
        <li>
            <asp:LinkButton ID="lnknext" runat="server" OnClick="lnknext_Click" Width="100%" Height="100%">Next</asp:LinkButton></li>
        <li>
            <asp:LinkButton ID="LnkLast" runat="server" OnClick="LnkLast_Click" Width="100%" Height="100%">Last</asp:LinkButton></li>
    </ul>
    <div class="numberofRecords">
        <asp:Label ID="LblDurum" runat="server"></asp:Label>
    </div>
</div>
