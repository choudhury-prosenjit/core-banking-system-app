Imports System
Namespace CoreBankingApp
Public Class Pages_SystemParameters
Inherits UI.Page
Private ReadOnly s As New SetupService()
Protected Sub Page_Load(sender As Object,e As EventArgs) Handles Me.Load
If Not IsPostBack Then Dim v=s.GetSystemParameters():txtTimeout.Text=v.SessionTimeoutMinutes:txtLang.Text=v.DefaultLanguage:txtRetention.Text=v.AuditRetentionDays:chkMaint.Checked=v.MaintenanceWindowEnabled
End Sub
Protected Sub btnSave_Click(sender As Object,e As EventArgs) Handles btnSave.Click
Dim timeout As Integer
Dim retention As Integer
If Not Integer.TryParse(txtTimeout.Text, timeout) OrElse Not Integer.TryParse(txtRetention.Text, retention) Then
lbl.CssClass="text-danger"
lbl.Text="Timeout and retention must be valid whole numbers."
Return
End If
s.SaveSystemParameters(New SystemParameter With {.SessionTimeoutMinutes=timeout,.DefaultLanguage=txtLang.Text,.AuditRetentionDays=retention,.MaintenanceWindowEnabled=chkMaint.Checked})
lbl.CssClass="text-success"
lbl.Text="Saved"
End Sub
End Class
End Namespace