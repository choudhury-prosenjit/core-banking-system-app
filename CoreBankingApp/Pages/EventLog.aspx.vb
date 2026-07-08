Imports System
Namespace CoreBankingApp
Public Class Pages_EventLog
Inherits UI.Page
Private ReadOnly s As New SetupService()
Protected Sub Page_Load(sender As Object,e As EventArgs) Handles Me.Load
If Not IsPostBack Then gv.DataSource=s.GetEventLog():gv.DataBind()
End Sub
End Class
End Namespace