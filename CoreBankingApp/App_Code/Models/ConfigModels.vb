Imports System
Imports System.Collections.Generic

''' <summary>Company parameter entity.</summary>
Public Class CompanyParameter
    Public Property BankCodeOrLei As String
    Public Property LegalEntityName As String
    Public Property BaseCurrency As String
    Public Property HeadOfficeBranchId As String
    Public Property TaxIdOrEin As String
    Public Property SystemCountryCode As String
End Class

''' <summary>System dates and processing control.</summary>
Public Class SystemDateControl
    Public Property CurrentBusinessDate As DateTime
    Public Property LastWorkingDay As DateTime
    Public Property NextWorkingDay As DateTime
    Public Property EodPhaseStatus As String
    Public Property NextStatementDate As DateTime
End Class

''' <summary>System parameters entity.</summary>
Public Class SystemParameter
    Public Property SessionTimeoutMinutes As Integer
    Public Property DefaultLanguage As String
    Public Property AuditRetentionDays As Integer
    Public Property MaintenanceWindowEnabled As Boolean
End Class

''' <summary>Currency master entity.</summary>
Public Class Currency
    Public Property IsoCode As String
    Public Property DecimalPlaces As Integer
    Public Property MinorUnitName As String
    Public Property RoundingRule As String
End Class

''' <summary>Exchange rate entity.</summary>
Public Class ExchangeRate
    Public Property MarketType As String
    Public Property CurrencyPair As String
    Public Property Rate As Decimal
    Public Property VarianceTolerancePct As Decimal
End Class

''' <summary>Holiday calendar entity.</summary>
Public Class HolidayCalendarEntry
    Public Property [Year] As Integer
    Public Property CountryCode As String
    Public Property WeeklyHolidays As List(Of DayOfWeek)
    Public Property FloatingHolidays As List(Of DateTime)
End Class

''' <summary>Transaction code definition entity.</summary>
Public Class TransactionCodeDefinition
    Public Property TxnCode As Integer
    Public Property Narrative As String
    Public Property DebitGlLink As String
    Public Property CreditGlLink As String
    Public Property IsDebitToAccount As Boolean
End Class

''' <summary>GL account entity.</summary>
Public Class GLAccount
    Public Property GlNumber As String
    Public Property Classification As String
    Public Property ConsolidationNode As String
    Public Property CurrencyRestricted As Boolean
End Class

''' <summary>Override matrix rule entity.</summary>
Public Class OverrideMatrixRule
    Public Property TriggerCondition As String
    Public Property Severity As String
    Public Property MinimumUserClass As String
End Class
