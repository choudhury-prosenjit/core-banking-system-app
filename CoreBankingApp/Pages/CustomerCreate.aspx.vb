Imports System
Namespace CoreBankingApp
Public Class Pages_CustomerCreate
Inherits UI.Page
Private ReadOnly svc As New CustomerService()
Protected Sub btnCreate_Click(sender As Object,e As EventArgs) Handles btnCreate.Click
If Not Page.IsValid Then Return
Dim c As New Customer With {.CustomerType=ddlType.SelectedValue,.LegalName1=txtName.Text,.BirthOrIncorporationDate=DateTime.Parse(txtDob.Text),.Gender="U",.TaxIdentifierType="SSN",.TaxId=txtTax.Text,.Citizenship="US",.DomicileCountry="US",.KycRiskRating=ddlRisk.SelectedValue,.PepStatus=chkPep.Checked,.BackupWithholding=False,.AddressType="Physical",.StreetLine1=txtStreet.Text,.City=txtCity.Text,.StateCode=txtState.Text,.ZipCode=txtZip.Text,.PrimaryPhone=txtPhone.Text,.Email=txtEmail.Text,.EconomicSector="Retail",.IndustryCode="44",.RelationshipManagerId="RM01",.StrategicTier="Standard",.LifecycleStatus=ddlLife.SelectedValue,.CipDocType=ddlDoc.SelectedValue,.DocumentReferenceNo=txtDoc.Text,.IssuingAuthority="System Entry",.IssueDate=DateTime.Today.AddYears(-1),.ExpiryDate=DateTime.Parse(txtExpiry.Text),.LegacyCrossReferenceId=""}
Dim r=svc.CreateCustomer(c)
lbl.CssClass=If(r.Success,"text-success","text-danger")
lbl.Text=r.Message
End Sub
End Class
End Namespace