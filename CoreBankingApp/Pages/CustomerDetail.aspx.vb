Imports System
Namespace CoreBankingApp
Public Class Pages_CustomerDetail
Inherits UI.Page
Private ReadOnly svc As New CustomerService()
Protected Sub Page_Load(sender As Object,e As EventArgs) Handles Me.Load
If Not IsPostBack Then
Dim id=Request.QueryString("id")
Dim c=svc.GetById(id)
If c Is Nothing Then lblWarn.CssClass="text-danger":lblWarn.Text="Customer not found":Return
litId.Text=c.CustomerId:litName.Text=c.LegalName1:litType.Text=c.CustomerType:litTax.Text=c.MaskedTaxId:litKyc.Text=c.KycStatus:litExpiry.Text=c.ExpiryDate.ToString("yyyy-MM-dd")
If svc.IsCipExpiringWithin90Days(c) Then lblWarn.Text="⚠ CIP expiry within 90 days of current business date."
End If
End Sub
End Class
End Namespace