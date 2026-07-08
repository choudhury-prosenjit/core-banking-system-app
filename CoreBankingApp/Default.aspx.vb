Imports System

Namespace CoreBankingApp
    Public Class _Default
        Inherits UI.Page
        Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
            If Not IsPostBack Then
                Dim s = (New DashboardService()).GetSnapshot()
                litDate.Text = s.BusinessDate.ToString("yyyy-MM-dd")
                litCustomers.Text = s.TotalCustomers.ToString()
                litAccounts.Text = s.TotalAccounts.ToString()
                litDeposits.Text = s.TotalDeposits.ToString("N2")
                litTx.Text = s.TodayTransactions.ToString()
                litEod.Text = s.LastEodResult
            End If
        End Sub
    End Class
End Namespace
