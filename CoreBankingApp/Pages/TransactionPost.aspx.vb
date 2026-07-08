Imports System
Namespace CoreBankingApp
Public Class Pages_TransactionPost
Inherits UI.Page
Private ReadOnly p As New PostingService()
Private ReadOnly accountSvc As New AccountService()
Private ReadOnly setupSvc As New SetupService()
Protected Sub Page_Load(sender As Object,e As EventArgs) Handles Me.Load
If Not IsPostBack Then
ddlAccount.DataSource=accountSvc.GetAll()
ddlAccount.DataTextField="AccountNumber"
ddlAccount.DataValueField="AccountNumber"
ddlAccount.DataBind()
ddlCode.DataSource=setupSvc.GetTransactionCodes()
ddlCode.DataTextField="Narrative"
ddlCode.DataValueField="TxnCode"
ddlCode.DataBind()
End If
End Sub
Protected Sub btnPost_Click(sender As Object,e As EventArgs) Handles btnPost.Click
Dim txnCode As Integer
Dim amount As Decimal
If Not Integer.TryParse(ddlCode.SelectedValue, txnCode) OrElse Not Decimal.TryParse(txtAmount.Text, amount) Then
lbl.CssClass="text-danger"
lbl.Text="Please enter a valid transaction code and amount."
Return
End If
Dim r=p.PostTransaction(ddlAccount.SelectedValue,txnCode,amount,txtNarrative.Text,chkOverride.Checked,chkCard.Checked)
lbl.CssClass=If(r.Success,"text-success","text-danger"):lbl.Text=r.Message
End Sub
End Class
End Namespace