Imports System
Namespace CoreBankingApp
Public Class Pages_SystemDates
Inherits UI.Page
Private ReadOnly s As New SetupService()
Protected Sub Page_Load(sender As Object,e As EventArgs) Handles Me.Load
If Not IsPostBack Then BindData()
End Sub
Private Sub BindData()
Dim v=s.GetSystemDates():txtCurrent.Text=v.CurrentBusinessDate.ToString("yyyy-MM-dd"):txtLast.Text=v.LastWorkingDay.ToString("yyyy-MM-dd"):txtNext.Text=v.NextWorkingDay.ToString("yyyy-MM-dd"):txtPhase.Text=v.EodPhaseStatus:txtStatement.Text=v.NextStatementDate.ToString("yyyy-MM-dd")
End Sub
Protected Sub btnSave_Click(sender As Object,e As EventArgs) Handles btnSave.Click
Dim lastDate As DateTime
Dim nextDate As DateTime
Dim statementDate As DateTime
If Not DateTime.TryParse(txtLast.Text, lastDate) OrElse Not DateTime.TryParse(txtNext.Text, nextDate) OrElse Not DateTime.TryParse(txtStatement.Text, statementDate) Then
lbl.CssClass="text-danger"
lbl.Text="Please enter valid dates for Last/Next working day and Next statement date."
Return
End If
Dim v=s.GetSystemDates()
v.LastWorkingDay=lastDate
v.NextWorkingDay=nextDate
v.EodPhaseStatus=txtPhase.Text
v.NextStatementDate=statementDate
s.SaveSystemDates(v)
lbl.CssClass="text-success"
lbl.Text="Business date is EOD-controlled; other fields saved."
End Sub
End Class
End Namespace