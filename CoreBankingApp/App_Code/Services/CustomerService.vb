Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions

''' <summary>Customer service for CIF create/list/detail behavior.</summary>
Public Class CustomerService
    Private ReadOnly _dateSvc As New DateService()

    Public Function GetAll() As List(Of Customer)
        Return StaticDataStore.Customers.OrderBy(Function(x) x.CustomerId).ToList()
    End Function

    Public Function GetById(id As String) As Customer
        Return StaticDataStore.Customers.FirstOrDefault(Function(x) x.CustomerId = id)
    End Function

    Public Function GetActiveCustomers() As List(Of Customer)
        Return StaticDataStore.Customers.Where(Function(c) c.LifecycleStatus = "Active" AndAlso c.KycStatus = "Passed").OrderBy(Function(c) c.CustomerId).ToList()
    End Function

    Public Function CreateCustomer(input As Customer) As ValidationResult
        Dim errs As New List(Of String)
        If String.IsNullOrWhiteSpace(input.LegalName1) Then errs.Add("Legal Name is required.")
        If String.IsNullOrWhiteSpace(input.CustomerType) Then errs.Add("Customer Type is required.")
        If input.BirthOrIncorporationDate = DateTime.MinValue Then errs.Add("DOB/Incorporation date is required.")
        If String.IsNullOrWhiteSpace(input.TaxId) Then errs.Add("Tax ID is required.")
        If String.IsNullOrWhiteSpace(input.StreetLine1) Then errs.Add("One address is required.")
        If String.IsNullOrWhiteSpace(input.KycRiskRating) Then errs.Add("KYC rating is required.")
        If Not String.IsNullOrWhiteSpace(input.PrimaryPhone) AndAlso Not Regex.IsMatch(input.PrimaryPhone, "^\+[1-9]\d{1,14}$") Then errs.Add("Primary phone must be E.164.")

        If input.CustomerType = "Individual" Then
            Dim age = _dateSvc.GetBusinessDate().Year - input.BirthOrIncorporationDate.Year
            If input.BirthOrIncorporationDate > _dateSvc.GetBusinessDate().AddYears(-age) Then age -= 1
            If age < 18 AndAlso input.LifecycleStatus = "Active" Then errs.Add("Under-18 individual cannot be Active.")
        End If

        If errs.Any() Then Return New ValidationResult(False, String.Join("<br/>", errs), Nothing)

        input.CustomerId = StaticDataStore.NextCustomerId()
        input.KycStatus = If(input.PepStatus OrElse input.KycRiskRating = "High", "Pending Review", "Passed")
        If input.KycStatus = "Pending Review" Then input.LifecycleStatus = "Pending Review"
        StaticDataStore.Customers.Add(input)
        Return New ValidationResult(True, "Customer created. KYC outcome: " & input.KycStatus, input)
    End Function

    Public Function IsCipExpiringWithin90Days(c As Customer) As Boolean
        Return c.ExpiryDate <= _dateSvc.GetBusinessDate().AddDays(90)
    End Function

    Public Class ValidationResult
        Public Sub New(success As Boolean, message As String, customer As Customer)
            Me.Success = success : Me.Message = message : Me.Customer = customer
        End Sub
        Public Property Success As Boolean
        Public Property Message As String
        Public Property Customer As Customer
    End Class
End Class
