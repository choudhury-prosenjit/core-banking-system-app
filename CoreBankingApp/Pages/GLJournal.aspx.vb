Imports System
Namespace CoreBankingApp
Public Class Pages_GLJournal
Inherits UI.Page
Private ReadOnly s As New AccountService()
Protected Sub Page_Load(sender As Object,e As EventArgs) Handles Me.Load
If Not IsPostBack Then gv.DataSource=s.GetAllGLJournal():gv.DataBind()
End Sub
End Class
End Namespace