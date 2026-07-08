Imports System
Namespace CoreBankingApp
Public Class Pages_CompanyParameter
Inherits UI.Page
Private ReadOnly s As New SetupService()
Protected Sub Page_Load(sender As Object,e As EventArgs) Handles Me.Load
If Not IsPostBack Then Dim v=s.GetCompany():txtBank.Text=v.BankCodeOrLei:txtName.Text=v.LegalEntityName:txtCur.Text=v.BaseCurrency:txtBranch.Text=v.HeadOfficeBranchId:txtTax.Text=v.TaxIdOrEin:txtCountry.Text=v.SystemCountryCode
End Sub
Protected Sub btnSave_Click(sender As Object,e As EventArgs) Handles btnSave.Click
s.SaveCompany(New CompanyParameter With {.BankCodeOrLei=txtBank.Text,.LegalEntityName=txtName.Text,.BaseCurrency=txtCur.Text,.HeadOfficeBranchId=txtBranch.Text,.TaxIdOrEin=txtTax.Text,.SystemCountryCode=txtCountry.Text})
lbl.Text="Saved"
End Sub
End Class
End Namespace