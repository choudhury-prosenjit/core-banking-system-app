Imports System
Namespace CoreBankingApp
Public Class Pages_CurrencyMaster
Inherits UI.Page
Protected Sub Page_Load(sender As Object,e As EventArgs) Handles Me.Load
If Not IsPostBack Then gv.DataSource=(New SetupService()).GetCurrencies():gv.DataBind()
End Sub
End Class
End Namespace