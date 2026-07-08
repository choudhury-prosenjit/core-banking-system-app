Imports System
Namespace CoreBankingApp
Public Class Pages_SystemParameters
Inherits UI.Page
Private ReadOnly s As New SetupService()
Protected Sub Page_Load(sender As Object,e As EventArgs) Handles Me.Load
If Not IsPostBack Then Dim v=s.GetSystemParameters():txtTimeout.Text=v.SessionTimeoutMinutes:txtLang.Text=v.DefaultLanguage:txtRetention.Text=v.AuditRetentionDays:chkMaint.Checked=v.MaintenanceWindowEnabled
End Sub
Protected Sub btnSave_Click(sender As Object,e As EventArgs) Handles btnSave.Click
s.SaveSystemParameters(New SystemParameter With {.SessionTimeoutMinutes=Integer.Parse(txtTimeout.Text),.DefaultLanguage=txtLang.Text,.AuditRetentionDays=Integer.Parse(txtRetention.Text),.MaintenanceWindowEnabled=chkMaint.Checked}):lbl.Text="Saved"
End Sub
End Class
End Namespace