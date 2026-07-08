<%@ Page Title="Dashboard" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="CoreBankingApp._Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h3>Dashboard</h3>
<div class="row g-2">
<div class="col-md-4"><div class="card"><div class="card-body"><small>Business Date</small><h5><asp:Literal ID="litDate" runat="server"/></h5></div></div></div>
<div class="col-md-4"><div class="card"><div class="card-body"><small>Total Customers</small><h5><asp:Literal ID="litCustomers" runat="server"/></h5></div></div></div>
<div class="col-md-4"><div class="card"><div class="card-body"><small>Total Accounts</small><h5><asp:Literal ID="litAccounts" runat="server"/></h5></div></div></div>
<div class="col-md-4"><div class="card"><div class="card-body"><small>Total Deposits</small><h5><asp:Literal ID="litDeposits" runat="server"/></h5></div></div></div>
<div class="col-md-4"><div class="card"><div class="card-body"><small>Today's Transactions</small><h5><asp:Literal ID="litTx" runat="server"/></h5></div></div></div>
<div class="col-md-4"><div class="card"><div class="card-body"><small>Last EOD</small><h5><asp:Literal ID="litEod" runat="server"/></h5></div></div></div>
</div>
</asp:Content>