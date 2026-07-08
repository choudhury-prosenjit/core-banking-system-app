Imports System
Imports System.Collections.Generic

Namespace CoreBankingApp
    Public Class SiteMaster
        Inherits UI.MasterPage

        Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
            If Not IsPostBack Then
                rptNav.DataSource = GetNav()
                rptNav.DataBind()
            End If
        End Sub

        Protected Sub rptNav_ItemDataBound(sender As Object, e As Web.UI.WebControls.RepeaterItemEventArgs) Handles rptNav.ItemDataBound
            Dim child = TryCast(e.Item.FindControl("rptLinks"), Web.UI.WebControls.Repeater)
            If child Is Nothing Then Return
            Dim group = CType(e.Item.DataItem, NavGroup)
            child.DataSource = group.Links
            child.ItemTemplate = New LinkTemplate()
            child.DataBind()
        End Sub

        Private Function GetNav() As List(Of NavGroup)
            Return New List(Of NavGroup) From {
                New NavGroup("Dashboard", New List(Of NavLink) From {New NavLink("/Default.aspx", "Home")}),
                New NavGroup("System Setup", New List(Of NavLink) From {New NavLink("/Pages/CompanyParameter.aspx", "Company Parameter"), New NavLink("/Pages/SystemDates.aspx", "System Dates"), New NavLink("/Pages/SystemParameters.aspx", "System Parameters"), New NavLink("/Pages/CurrencyMaster.aspx", "Currency Master"), New NavLink("/Pages/ExchangeRates.aspx", "Exchange Rates"), New NavLink("/Pages/HolidayCalendar.aspx", "Holiday Calendar"), New NavLink("/Pages/TransactionCodes.aspx", "Transaction Codes"), New NavLink("/Pages/GLAccounts.aspx", "GL Accounts"), New NavLink("/Pages/OverrideMatrix.aspx", "Override Matrix")}),
                New NavGroup("Customer (CIF)", New List(Of NavLink) From {New NavLink("/Pages/CustomerList.aspx", "Customer List"), New NavLink("/Pages/CustomerCreate.aspx", "Customer Create")}),
                New NavGroup("Product Catalog", New List(Of NavLink) From {New NavLink("/Pages/CategoryList.aspx", "Category List"), New NavLink("/Pages/CategoryCreate.aspx", "Category Create")}),
                New NavGroup("Account Management", New List(Of NavLink) From {New NavLink("/Pages/AccountList.aspx", "Account List"), New NavLink("/Pages/AccountOpen.aspx", "Account Open"), New NavLink("/Pages/AccountDetail.aspx", "Account Detail")}),
                New NavGroup("DDA Operations", New List(Of NavLink) From {New NavLink("/Pages/TransactionPost.aspx", "Transaction Post"), New NavLink("/Pages/GLJournal.aspx", "GL Journal")}),
                New NavGroup("Batch/EOD", New List(Of NavLink) From {New NavLink("/Pages/EODProcess.aspx", "EOD Process"), New NavLink("/Pages/EventLog.aspx", "Event Log")})}
        End Function

        Public Class NavGroup
            Public Sub New(name As String, links As List(Of NavLink)) : GroupName = name : Me.Links = links : End Sub
            Public Property GroupName As String
            Public Property Links As List(Of NavLink)
        End Class

        Public Class NavLink
            Public Sub New(url As String, title As String) : Me.Url = url : Me.Title = title : End Sub
            Public Property Url As String
            Public Property Title As String
        End Class

        Private Class LinkTemplate
            Implements Web.UI.ITemplate
            Public Sub InstantiateIn(container As Web.UI.Control) Implements Web.UI.ITemplate.InstantiateIn
                Dim link As New Web.UI.WebControls.HyperLink With {.CssClass = "d-block px-2 py-1 text-decoration-none"}
                AddHandler link.DataBinding, Sub(sender, args)
                                                 Dim h = CType(sender, Web.UI.WebControls.HyperLink)
                                                 Dim item = CType(h.NamingContainer, Web.UI.WebControls.RepeaterItem)
                                                 Dim n = CType(item.DataItem, NavLink)
                                                 h.Text = n.Title
                                                 h.NavigateUrl = n.Url
                                             End Sub
                container.Controls.Add(link)
            End Sub
        End Class
    End Class
End Namespace
