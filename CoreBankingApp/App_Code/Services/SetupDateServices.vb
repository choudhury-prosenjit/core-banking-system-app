Imports System
Imports System.Collections.Generic
Imports System.Linq

''' <summary>Service for system setup master data screens.</summary>
Public Class SetupService
    Public Function GetCompany() As CompanyParameter : Return StaticDataStore.Company : End Function
    Public Sub SaveCompany(v As CompanyParameter) : StaticDataStore.Company = v : End Sub
    Public Function GetSystemDates() As SystemDateControl : Return StaticDataStore.Dates : End Function
    Public Sub SaveSystemDates(v As SystemDateControl)
        StaticDataStore.Dates.LastWorkingDay = v.LastWorkingDay
        StaticDataStore.Dates.NextWorkingDay = v.NextWorkingDay
        StaticDataStore.Dates.EodPhaseStatus = v.EodPhaseStatus
        StaticDataStore.Dates.NextStatementDate = v.NextStatementDate
    End Sub
    Public Function GetSystemParameters() As SystemParameter : Return StaticDataStore.Parameters : End Function
    Public Sub SaveSystemParameters(v As SystemParameter) : StaticDataStore.Parameters = v : End Sub
    Public Function GetCurrencies() As List(Of Currency) : Return StaticDataStore.Currencies.OrderBy(Function(x) x.IsoCode).ToList() : End Function
    Public Function GetExchangeRates() As List(Of ExchangeRate) : Return StaticDataStore.ExchangeRates.OrderBy(Function(x) x.CurrencyPair).ToList() : End Function
    Public Function GetHolidays() As List(Of HolidayCalendarEntry) : Return StaticDataStore.Holidays : End Function
    Public Function GetTransactionCodes() As List(Of TransactionCodeDefinition) : Return StaticDataStore.TransactionCodes.OrderBy(Function(x) x.TxnCode).ToList() : End Function
    Public Function GetGLAccounts() As List(Of GLAccount) : Return StaticDataStore.GLAccounts.OrderBy(Function(x) x.GlNumber).ToList() : End Function
    Public Function GetOverrideMatrix() As List(Of OverrideMatrixRule) : Return StaticDataStore.OverrideMatrix : End Function
    Public Function GetEventLog() As List(Of EventLogEntry) : Return StaticDataStore.Events.OrderByDescending(Function(x) x.Timestamp).ToList() : End Function
End Class

''' <summary>Business-date helper service using holiday setup.</summary>
Public Class DateService
    Public Function GetBusinessDate() As DateTime
        Return StaticDataStore.Dates.CurrentBusinessDate.Date
    End Function

    Public Function IsWorkingDay([date] As DateTime) As Boolean
        Dim h = StaticDataStore.Holidays.FirstOrDefault(Function(x) x.Year = [date].Year AndAlso x.CountryCode = StaticDataStore.Company.SystemCountryCode)
        If h Is Nothing Then Return [date].DayOfWeek <> DayOfWeek.Saturday AndAlso [date].DayOfWeek <> DayOfWeek.Sunday
        If h.WeeklyHolidays.Contains([date].DayOfWeek) Then Return False
        Return Not h.FloatingHolidays.Any(Function(d) d.Date = [date].Date)
    End Function

    Public Function NextWorkingDay(fromDate As DateTime) As DateTime
        Dim d = fromDate.Date.AddDays(1)
        Do While Not IsWorkingDay(d)
            d = d.AddDays(1)
        Loop
        Return d
    End Function

    Public Function AddBusinessDays(fromDate As DateTime, days As Integer) As DateTime
        Dim d = fromDate.Date
        Dim n = days
        While n > 0
            d = d.AddDays(1)
            If IsWorkingDay(d) Then n -= 1
        End While
        Return d
    End Function
End Class
