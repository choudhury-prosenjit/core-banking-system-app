Imports System
Namespace CoreBankingApp
Public Class Pages_AccountList
Inherits UI.Page
Protected Sub Page_Load(sender As Object,e As EventArgs) Handles Me.Load
If Not IsPostBack Then gv.DataSource=(New AccountService()).GetAll():gv.DataBind()
End Sub
End Class
End Namespace