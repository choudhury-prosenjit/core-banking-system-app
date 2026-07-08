Imports System

''' <summary>Customer CIF entity.</summary>
Public Class Customer
    Public Property CustomerId As String
    Public Property CustomerType As String
    Public Property LegalName1 As String
    Public Property LegalName2 As String
    Public Property PreferredTradeName As String
    Public Property BirthOrIncorporationDate As DateTime
    Public Property Gender As String
    Public Property TaxIdentifierType As String
    Public Property TaxId As String
    Public Property Citizenship As String
    Public Property DomicileCountry As String
    Public Property KycRiskRating As String
    Public Property PepStatus As Boolean
    Public Property BackupWithholding As Boolean
    Public Property AddressType As String
    Public Property StreetLine1 As String
    Public Property StreetLine2 As String
    Public Property StreetLine3 As String
    Public Property City As String
    Public Property StateCode As String
    Public Property ZipCode As String
    Public Property PrimaryPhone As String
    Public Property Email As String
    Public Property EconomicSector As String
    Public Property IndustryCode As String
    Public Property RelationshipManagerId As String
    Public Property StrategicTier As String
    Public Property LifecycleStatus As String
    Public Property CipDocType As String
    Public Property DocumentReferenceNo As String
    Public Property IssuingAuthority As String
    Public Property IssueDate As DateTime
    Public Property ExpiryDate As DateTime
    Public Property LegacyCrossReferenceId As String
    Public Property KycStatus As String

    Public ReadOnly Property MaskedTaxId As String
        Get
            If String.IsNullOrWhiteSpace(TaxId) OrElse TaxId.Length < 4 Then Return "****"
            Return "***-**-" & TaxId.Substring(TaxId.Length - 4)
        End Get
    End Property
End Class
