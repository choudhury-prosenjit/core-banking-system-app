Imports System
Imports System.Collections.Generic
Imports System.Linq

''' <summary>Category service for product catalog operations.</summary>
Public Class CategoryService
    Public Function GetAll() As List(Of Category)
        Return StaticDataStore.Categories.OrderBy(Function(x) x.CategoryCode).ToList()
    End Function
    Public Function GetByCode(code As String) As Category
        Return StaticDataStore.Categories.FirstOrDefault(Function(x) x.CategoryCode = code)
    End Function
    Public Function CreateCategory(c As Category) As String
        If String.IsNullOrWhiteSpace(c.CategoryCode) Then Return "Category code is required."
        If StaticDataStore.Categories.Any(Function(x) x.CategoryCode = c.CategoryCode) Then Return "Category code already exists."
        If String.IsNullOrWhiteSpace(c.ShortDescription) OrElse c.ShortDescription.Length > 35 Then Return "Short description is required and <= 35 chars."
        StaticDataStore.Categories.Add(c)
        Return "Category created."
    End Function
End Class

''' <summary>Account service supporting account opening lifecycle.</summary>
Public Class AccountService
    Private ReadOnly _dateSvc As New DateService()
    Private ReadOnly _custSvc As New CustomerService()
    Private ReadOnly _catSvc As New CategoryService()

    Public Function GetAll() As List(Of Account)
        Return StaticDataStore.Accounts.OrderBy(Function(x) x.AccountNumber).ToList()
    End Function
    Public Function GetByAccountNumber(number As String) As Account
        Return StaticDataStore.Accounts.FirstOrDefault(Function(a) a.AccountNumber = number)
    End Function
    Public Function GetTransactionHistory(accountNumber As String) As List(Of TransactionHistory)
        Return StaticDataStore.Transactions.Where(Function(t) t.AccountNumber = accountNumber).OrderByDescending(Function(t) t.TxnDate).ToList()
    End Function
    Public Function GetGlJournal(accountNumber As String) As List(Of GLJournalEntry)
        Return StaticDataStore.GLJournal.Where(Function(g) g.AccountNumber = accountNumber).OrderByDescending(Function(g) g.PostedAt).ToList()
    End Function
    Public Function GetAllGLJournal() As List(Of GLJournalEntry)
        Return StaticDataStore.GLJournal.OrderByDescending(Function(g) g.PostedAt).ToList()
    End Function

    Public Function IntakeValidation(customerId As String, categoryCode As String, ownershipType As String) As String
        Dim c = _custSvc.GetById(customerId)
        If c Is Nothing Then Return "Customer not found."
        Dim p = _catSvc.GetByCode(categoryCode)
        If p Is Nothing Then Return "Category not found."
        If Not p.AllowedCustomerTypes.Contains(c.CustomerType) Then Return "Customer type not allowed by category."
        If c.CustomerType = "Individual" Then
            Dim age = _dateSvc.GetBusinessDate().Year - c.BirthOrIncorporationDate.Year
            If c.BirthOrIncorporationDate > _dateSvc.GetBusinessDate().AddYears(-age) Then age -= 1
            If age < p.MinAge OrElse age > p.MaxAge Then Return "Customer age does not meet category limits."
        End If
        If String.IsNullOrWhiteSpace(ownershipType) Then Return "Ownership type is required."
        Return String.Empty
    End Function

    Public Function ComplianceCheck(customerId As String, overrideApproved As Boolean) As ComplianceResult
        Dim c = _custSvc.GetById(customerId)
        If c Is Nothing Then Return New ComplianceResult(False, 0, "Customer not found.", False)
        If c.KycStatus <> "Passed" OrElse c.LifecycleStatus <> "Active" Then Return New ComplianceResult(False, 0, "KYC must be Passed and lifecycle Active.", False)
        Dim score = 400 + (Math.Abs(customerId.GetHashCode()) Mod 401)
        If score < 550 AndAlso Not overrideApproved Then Return New ComplianceResult(False, score, "ChexSystems score below 550; override required.", True)
        Return New ComplianceResult(True, score, "Compliance gate passed.", score < 550)
    End Function

    Public Function ProvisionAccount(customerId As String, categoryCode As String, ownershipType As String, title As String, currency As String) As Account
        Dim category = _catSvc.GetByCode(categoryCode)
        Dim a = New Account With {.ApplicationReferenceNumber = StaticDataStore.NextApplicationRef(), .AccountNumber = StaticDataStore.NextAccountNumber(), .PrimaryCustomerId = customerId, .OwnershipType = ownershipType, .JointHolderIds = New List(Of String), .CategoryCode = categoryCode, .AccountTitle = If(String.IsNullOrWhiteSpace(title), "New Account", title), .CurrencyCode = currency.ToUpperInvariant(), .LedgerBalance = 0D, .ClearedBalance = 0D, .BlockedHeldAmount = 0D, .AccruedInterest = 0D, .AccountStatus = "New-Unfunded", .PostingRestrictionCode = category.DefaultPostingRestriction, .OverdraftAllowed = category.AllowNegativeBalance, .OverdraftOptIn = False, .OverdraftLimit = 0D, .DateOpened = _dateSvc.GetBusinessDate(), .LastTransactionDate = _dateSvc.GetBusinessDate(), .InterestPlan = category.TieringMethod, .VarianceRatePct = 0D, .TaxWithholdingCode = "NONE", .StatementCycle = category.StatementCycleTemplate, .FeeSchedule = category.ChargeScheduleCode, .GlClassCategory = category.AssetLiabilityCode, .CostCenterBranchId = StaticDataStore.Company.HeadOfficeBranchId, .AlternativeAccountNumber = "", .ExternalCoreCrossReferenceId = "", .ShadowStatus = False, .Holds = New List(Of AccountHold)}
        StaticDataStore.Accounts.Add(a)
        Return a
    End Function

    Public Function ApplyInitialFunding(accountNumber As String, amount As Decimal, channel As String) As String
        Dim a = GetByAccountNumber(accountNumber)
        If a Is Nothing Then Return "Account not found."
        If amount <= 0D Then Return "Funding amount must be positive."
        a.LedgerBalance += amount
        If channel = "Check" Then
            a.BlockedHeldAmount += amount
            a.Holds.Add(New AccountHold With {.Amount = amount, .ReleaseDate = _dateSvc.AddBusinessDays(_dateSvc.GetBusinessDate(), 2), .Reason = "Reg CC", .Released = False})
        Else
            a.ClearedBalance += amount
        End If
        a.LastTransactionDate = _dateSvc.GetBusinessDate()
        Dim cat = _catSvc.GetByCode(a.CategoryCode)
        If a.LedgerBalance - a.BlockedHeldAmount >= cat.OpeningMinimum Then a.AccountStatus = "Active"
        Return "Funding applied via " & channel & "."
    End Function

    Public Class ComplianceResult
        Public Sub New(allowed As Boolean, score As Integer, message As String, overrideRequired As Boolean)
            Me.Allowed = allowed : Me.Score = score : Me.Message = message : Me.OverrideRequired = overrideRequired
        End Sub
        Public Property Allowed As Boolean
        Public Property Score As Integer
        Public Property Message As String
        Public Property OverrideRequired As Boolean
    End Class
End Class

''' <summary>Posting service implementing teller transaction rules and memo-posting.</summary>
Public Class PostingService
    Public Function PostTransaction(accountNumber As String, txnCode As Integer, amount As Decimal, narrative As String, overrideApproved As Boolean, Optional isCardAtmDebit As Boolean = False) As PostResult
        If amount <= 0D Then Return New PostResult(False, "Amount must be positive.", False)
        Dim a = StaticDataStore.Accounts.FirstOrDefault(Function(x) x.AccountNumber = accountNumber)
        If a Is Nothing Then Return New PostResult(False, "Account not found.", False)
        Dim code = StaticDataStore.TransactionCodes.FirstOrDefault(Function(x) x.TxnCode = txnCode)
        If code Is Nothing Then Return New PostResult(False, "Transaction code not found.", False)
        If a.AccountStatus = "Frozen" OrElse a.AccountStatus = "Closed" Then Return New PostResult(False, "Account status blocks posting.", False)
        If amount > 10000D AndAlso Not overrideApproved Then Return New PostResult(False, "Override required for amount > 10,000.", True)

        Dim isDebit = code.IsDebitToAccount
        If a.PostingRestrictionCode = "Total Freeze" Then Return New PostResult(False, "Posting blocked by total freeze.", False)
        If a.PostingRestrictionCode = "No Debits" AndAlso isDebit Then Return New PostResult(False, "Posting blocked by No Debits.", False)
        If a.PostingRestrictionCode = "No Credits" AndAlso Not isDebit Then Return New PostResult(False, "Posting blocked by No Credits.", False)

        If isDebit Then
            Dim projected = a.LedgerBalance - amount
            If amount > a.AvailableBalance AndAlso (Not a.OverdraftAllowed OrElse projected < (0D - a.OverdraftLimit)) Then Return New PostResult(False, "Insufficient available balance.", False)
            If isCardAtmDebit AndAlso Not a.OverdraftOptIn AndAlso projected < 0D Then Return New PostResult(False, "Overdraft opt-in required for card/ATM debit.", False)
            a.LedgerBalance -= amount
        Else
            a.LedgerBalance += amount
            a.ClearedBalance += amount
        End If

        a.LastTransactionDate = StaticDataStore.Dates.CurrentBusinessDate
        StaticDataStore.Transactions.Add(New TransactionHistory With {.TransactionId = StaticDataStore.NextTransactionId(), .TxnDate = StaticDataStore.Dates.CurrentBusinessDate, .AccountNumber = a.AccountNumber, .TxnCode = txnCode, .Narrative = If(String.IsNullOrWhiteSpace(narrative), code.Narrative, narrative), .Amount = amount, .RunningBalance = a.LedgerBalance, .IsMemo = True})
        StaticDataStore.GLJournal.Add(New GLJournalEntry With {.JournalId = StaticDataStore.NextJournalId(), .PostedAt = DateTime.Now, .AccountNumber = a.AccountNumber, .TxnCode = txnCode, .DebitGL = code.DebitGlLink, .CreditGL = code.CreditGlLink, .Amount = amount, .IsMemo = True})
        Return New PostResult(True, "Memo-post successful; finalization occurs during EOD.", False)
    End Function

    Public Class PostResult
        Public Sub New(success As Boolean, message As String, overrideRequired As Boolean)
            Me.Success = success : Me.Message = message : Me.OverrideRequired = overrideRequired
        End Sub
        Public Property Success As Boolean
        Public Property Message As String
        Public Property OverrideRequired As Boolean
    End Class
End Class

''' <summary>EOD batch service implementing step-based end-of-day processing.</summary>
Public Class EODService
    Private ReadOnly _dateSvc As New DateService()
    Private ReadOnly _catSvc As New CategoryService()

    Public Function RunEod() As List(Of String)
        Dim logs As New List(Of String)
        Dim oldDate = StaticDataStore.Dates.CurrentBusinessDate
        StaticDataStore.Dates.EodPhaseStatus = "Running" : logs.Add("1. EOD phase set to Running")
        For Each t In StaticDataStore.Transactions.Where(Function(x) x.IsMemo) : t.IsMemo = False : Next
        For Each j In StaticDataStore.GLJournal.Where(Function(x) x.IsMemo) : j.IsMemo = False : Next
        logs.Add("2. Memo-posts converted to final")
        For Each a In StaticDataStore.Accounts
            For Each h In a.Holds.Where(Function(x) Not x.Released AndAlso x.ReleaseDate <= oldDate)
                h.Released = True : a.BlockedHeldAmount -= h.Amount : a.ClearedBalance += h.Amount
            Next
        Next
        logs.Add("3. Released matured Reg CC holds")

        For Each a In StaticDataStore.Accounts
            Dim c = _catSvc.GetByCode(a.CategoryCode)
            If c IsNot Nothing AndAlso c.InterestEligible Then
                Dim tier = c.RateTiers.FirstOrDefault(Function(t) a.LedgerBalance >= t.TierFrom AndAlso a.LedgerBalance <= t.TierTo)
                Dim annual = If(tier Is Nothing, 0D, tier.AnnualRatePct) + a.VarianceRatePct
                Dim basis = If(c.DefaultInterestBasis = "Actual/360", 360D, 365D)
                a.AccruedInterest += Decimal.Round(a.LedgerBalance * (annual / 100D) / basis, 6)
            End If
        Next
        logs.Add("4. Computed daily interest accrual")

        If oldDate.AddDays(1).Month <> oldDate.Month Then
            For Each a In StaticDataStore.Accounts.Where(Function(x) x.AccruedInterest > 0D)
                Dim amt = a.AccruedInterest : a.AccruedInterest = 0D : a.LedgerBalance += amt : a.ClearedBalance += amt
                StaticDataStore.Transactions.Add(New TransactionHistory With {.TransactionId = StaticDataStore.NextTransactionId(), .TxnDate = oldDate, .AccountNumber = a.AccountNumber, .TxnCode = 601, .Narrative = "Interest capitalization", .Amount = amt, .RunningBalance = a.LedgerBalance, .IsMemo = False})
            Next
            logs.Add("5. Capitalized accrued interest")
        Else
            logs.Add("5. Capitalization skipped (not cycle)")
        End If

        For Each a In StaticDataStore.Accounts
            Dim c = _catSvc.GetByCode(a.CategoryCode)
            If c IsNot Nothing AndAlso oldDate.Day = Math.Min(c.StatementCycleDay, DateTime.DaysInMonth(oldDate.Year, oldDate.Month)) AndAlso c.FlatMonthlyFee > 0D Then
                a.LedgerBalance -= c.FlatMonthlyFee
                StaticDataStore.Transactions.Add(New TransactionHistory With {.TransactionId = StaticDataStore.NextTransactionId(), .TxnDate = oldDate, .AccountNumber = a.AccountNumber, .TxnCode = 301, .Narrative = "Monthly fee", .Amount = c.FlatMonthlyFee, .RunningBalance = a.LedgerBalance, .IsMemo = False})
            End If
        Next
        logs.Add("6. Applied statement-cycle fees")

        For Each a In StaticDataStore.Accounts
            Dim c = _catSvc.GetByCode(a.CategoryCode)
            If c IsNot Nothing AndAlso a.AccountStatus = "Active" AndAlso (oldDate - a.LastTransactionDate).TotalDays > c.DormancyPeriodDays Then a.AccountStatus = "Dormant"
        Next
        logs.Add("7. Dormancy sweep complete")

        Dim sumLedgers = StaticDataStore.Accounts.Sum(Function(a) a.LedgerBalance)
        Dim glControl = StaticDataStore.Accounts.Sum(Function(a) a.LedgerBalance)
        logs.Add("8. Reconciliation: " & If(sumLedgers = glControl, "Balanced", "Out-of-balance"))

        Dim newDate = _dateSvc.NextWorkingDay(oldDate)
        StaticDataStore.Dates.LastWorkingDay = oldDate
        StaticDataStore.Dates.CurrentBusinessDate = newDate
        StaticDataStore.Dates.NextWorkingDay = _dateSvc.NextWorkingDay(newDate)
        StaticDataStore.Dates.EodPhaseStatus = "Complete"
        logs.Add("9. Advanced business date to next working day")

        Dim payload = String.Format("{{"event":"SYSTEM.DATE.ADVANCED","oldDate":"{0:yyyy-MM-dd}","newDate":"{1:yyyy-MM-dd}","timestamp":"{2:o}"}}", oldDate, newDate, DateTime.UtcNow)
        StaticDataStore.Events.Add(New EventLogEntry With {.EventName = "SYSTEM.DATE.ADVANCED", .Payload = payload, .Timestamp = DateTime.UtcNow})
        logs.Add("10. Added SYSTEM.DATE.ADVANCED event")

        Return logs
    End Function
End Class

''' <summary>Dashboard aggregation service.</summary>
Public Class DashboardService
    Public Function GetSnapshot() As DashboardSnapshot
        Return New DashboardSnapshot With {.BusinessDate = StaticDataStore.Dates.CurrentBusinessDate, .TotalCustomers = StaticDataStore.Customers.Count, .TotalAccounts = StaticDataStore.Accounts.Count, .ActiveAccounts = StaticDataStore.Accounts.Count(Function(a) a.AccountStatus = "Active"), .DormantAccounts = StaticDataStore.Accounts.Count(Function(a) a.AccountStatus = "Dormant"), .FrozenAccounts = StaticDataStore.Accounts.Count(Function(a) a.AccountStatus = "Frozen"), .TotalDeposits = StaticDataStore.Accounts.Sum(Function(a) a.LedgerBalance), .TodayTransactions = StaticDataStore.Transactions.Count(Function(t) t.TxnDate = StaticDataStore.Dates.CurrentBusinessDate), .LastEodResult = "Phase: " & StaticDataStore.Dates.EodPhaseStatus}
    End Function

    Public Class DashboardSnapshot
        Public Property BusinessDate As DateTime
        Public Property TotalCustomers As Integer
        Public Property TotalAccounts As Integer
        Public Property ActiveAccounts As Integer
        Public Property DormantAccounts As Integer
        Public Property FrozenAccounts As Integer
        Public Property TotalDeposits As Decimal
        Public Property TodayTransactions As Integer
        Public Property LastEodResult As String
    End Class
End Class
