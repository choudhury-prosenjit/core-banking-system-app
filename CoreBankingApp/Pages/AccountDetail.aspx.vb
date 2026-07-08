Imports System
Imports System.Linq
Namespace CoreBankingApp
Public Class Pages_AccountDetail
Inherits UI.Page
Private ReadOnly s As New AccountService()
Protected Sub Page_Load(sender As Object,e As EventArgs) Handles Me.Load
If Not IsPostBack Then
Dim account=Request.QueryString("account")
If String.IsNullOrWhiteSpace(account) Then account=s.GetAll().First().AccountNumber
Dim a=s.GetByAccountNumber(account)
If a Is Nothing Then lbl.Text="Account not found":Return
litAccount.Text=a.AccountNumber:litStatus.Text=a.AccountStatus:litLedger.Text=a.LedgerBalance.ToString("N2"):litAvail.Text=a.AvailableBalance.ToString("N2"):litHeld.Text=a.BlockedHeldAmount.ToString("N2"):litAccrued.Text=a.AccruedInterest.ToString("N2")
gvTx.DataSource=s.GetTransactionHistory(a.AccountNumber):gvTx.DataBind()
gvGl.DataSource=s.GetGlJournal(a.AccountNumber):gvGl.DataBind()
End If
End Sub
End Class
End Namespace