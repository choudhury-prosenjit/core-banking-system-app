Imports System
Namespace CoreBankingApp
Public Class Pages_AccountOpen
Inherits UI.Page
Private ReadOnly s As New AccountService()
Protected Sub Page_Load(sender As Object,e As EventArgs) Handles Me.Load
If Not IsPostBack Then
ddlCustomer.DataSource=(New CustomerService()).GetActiveCustomers():ddlCustomer.DataTextField="LegalName1":ddlCustomer.DataValueField="CustomerId":ddlCustomer.DataBind()
ddlCategory.DataSource=(New CategoryService()).GetAll():ddlCategory.DataTextField="ShortDescription":ddlCategory.DataValueField="CategoryCode":ddlCategory.DataBind()
End If
End Sub
Protected Sub btnIntake_Click(sender As Object,e As EventArgs) Handles btnIntake.Click
Dim m=s.IntakeValidation(ddlCustomer.SelectedValue,ddlCategory.SelectedValue,ddlOwner.SelectedValue)
lbl.CssClass=If(String.IsNullOrWhiteSpace(m),"text-success","text-danger"):lbl.Text=If(String.IsNullOrWhiteSpace(m),"Intake passed",m)
End Sub
Protected Sub btnCompliance_Click(sender As Object,e As EventArgs) Handles btnCompliance.Click
Dim r=s.ComplianceCheck(ddlCustomer.SelectedValue,chkOverride.Checked)
lbl.CssClass=If(r.Allowed,"text-success","text-danger"):lbl.Text=r.Message & " (Score: " & r.Score & ")"
End Sub
Protected Sub btnProvision_Click(sender As Object,e As EventArgs) Handles btnProvision.Click
Dim a=s.ProvisionAccount(ddlCustomer.SelectedValue,ddlCategory.SelectedValue,ddlOwner.SelectedValue,txtTitle.Text,txtCurrency.Text)
hfAccount.Value=a.AccountNumber:lbl.CssClass="text-success":lbl.Text="Provisioned " & a.AccountNumber
End Sub
Protected Sub btnFunding_Click(sender As Object,e As EventArgs) Handles btnFunding.Click
If String.IsNullOrWhiteSpace(hfAccount.Value) Then lbl.CssClass="text-danger":lbl.Text="Provision first":Return
Dim m=s.ApplyInitialFunding(hfAccount.Value,Decimal.Parse(txtAmount.Text),ddlChannel.SelectedValue)
lbl.CssClass=If(m.Contains("applied"),"text-success","text-danger"):lbl.Text=m
End Sub
End Class
End Namespace