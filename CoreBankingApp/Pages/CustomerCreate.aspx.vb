Imports System
Namespace CoreBankingApp
Public Class Pages_CustomerCreate
Inherits UI.Page
Private ReadOnly svc As New CustomerService()
Protected Sub btnCreate_Click(sender As Object,e As EventArgs) Handles btnCreate.Click
If Not Page.IsValid Then Return
Dim dob As DateTime
Dim expiry As DateTime
If Not DateTime.TryParse(txtDob.Text, dob) OrElse Not DateTime.TryParse(txtExpiry.Text, expiry) Then
lbl.CssClass="text-danger"
lbl.Text="DOB/Incorporation and document expiry must be valid dates."
Return
End If
Dim c As New Customer With {.CustomerType=ddlType.SelectedValue,.LegalName1=txtName.Text,.BirthOrIncorporationDate=dob,.Gender="U",.TaxIdentifierType="SSN",.TaxId=txtTax.Text,.Citizenship="US",.DomicileCountry="US",.KycRiskRating=ddlRisk.SelectedValue,.PepStatus=chkPep.Checked,.BackupWithholding=False,.AddressType="Physical",.StreetLine1=txtStreet.Text,.City=txtCity.Text,.StateCode=txtState.Text,.ZipCode=txtZip.Text,.PrimaryPhone=txtPhone.Text,.Email=txtEmail.Text,.EconomicSector="Retail",.IndustryCode="44",.RelationshipManagerId="RM01",.StrategicTier="Standard",.LifecycleStatus=ddlLife.SelectedValue,.CipDocType=ddlDoc.SelectedValue,.DocumentReferenceNo=txtDoc.Text,.IssuingAuthority="System Entry",.IssueDate=DateTime.Today.AddYears(-1),.ExpiryDate=expiry,.LegacyCrossReferenceId=""}
Dim r=svc.CreateCustomer(c)
lbl.CssClass=If(r.Success,"text-success","text-danger")
lbl.Text=r.Message
End Sub
End Class
End Namespace