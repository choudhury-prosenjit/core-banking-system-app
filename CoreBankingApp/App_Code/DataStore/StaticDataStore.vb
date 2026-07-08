Imports System
Imports System.Collections.Generic
Imports System.Linq

''' <summary>Static in-memory datastore with seed records.</summary>
Public Module StaticDataStore
    Public Company As CompanyParameter
    Public Dates As SystemDateControl
    Public Parameters As SystemParameter
    Public Currencies As New List(Of Currency)
    Public ExchangeRates As New List(Of ExchangeRate)
    Public Holidays As New List(Of HolidayCalendarEntry)
    Public TransactionCodes As New List(Of TransactionCodeDefinition)
    Public GLAccounts As New List(Of GLAccount)
    Public OverrideMatrix As New List(Of OverrideMatrixRule)
    Public Customers As New List(Of Customer)
    Public Categories As New List(Of Category)
    Public Accounts As New List(Of Account)
    Public Transactions As New List(Of TransactionHistory)
    Public GLJournal As New List(Of GLJournalEntry)
    Public Events As New List(Of EventLogEntry)

    Private _initialized As Boolean
    Private ReadOnly _syncRoot As New Object()
    Private _customerCounter As Integer = 5
    Private _accountCounter As Integer = 4
    Private _applicationCounter As Integer = 4
    Private _transactionCounter As Integer = 15
    Private _journalCounter As Integer = 15

    Public Sub Initialize()
        SyncLock _syncRoot
            If _initialized Then Return

            Company = New CompanyParameter With {.BankCodeOrLei = "5493001KJTIIGC8Y1R12", .LegalEntityName = "Contoso Community Bank", .BaseCurrency = "USD", .HeadOfficeBranchId = "BR001", .TaxIdOrEin = "12-3456789", .SystemCountryCode = "US"}
            Dates = New SystemDateControl With {.CurrentBusinessDate = New DateTime(2026, 7, 8), .LastWorkingDay = New DateTime(2026, 7, 7), .NextWorkingDay = New DateTime(2026, 7, 9), .EodPhaseStatus = "Complete", .NextStatementDate = New DateTime(2026, 7, 31)}
            Parameters = New SystemParameter With {.SessionTimeoutMinutes = 20, .DefaultLanguage = "en-US", .AuditRetentionDays = 365, .MaintenanceWindowEnabled = False}

            Currencies = New List(Of Currency) From {
                New Currency With {.IsoCode = "USD", .DecimalPlaces = 2, .MinorUnitName = "Cent", .RoundingRule = "Round Half Up"},
                New Currency With {.IsoCode = "EUR", .DecimalPlaces = 2, .MinorUnitName = "Cent", .RoundingRule = "Round Half Up"},
                New Currency With {.IsoCode = "JPY", .DecimalPlaces = 0, .MinorUnitName = "Sen", .RoundingRule = "Truncate"},
                New Currency With {.IsoCode = "CAD", .DecimalPlaces = 2, .MinorUnitName = "Cent", .RoundingRule = "Round Half Up"}}

            ExchangeRates = New List(Of ExchangeRate) From {
                New ExchangeRate With {.MarketType = "Mid", .CurrencyPair = "USD/EUR", .Rate = 0.93D, .VarianceTolerancePct = 1.2D},
                New ExchangeRate With {.MarketType = "Buy", .CurrencyPair = "USD/CAD", .Rate = 1.35D, .VarianceTolerancePct = 1.0D},
                New ExchangeRate With {.MarketType = "Sell", .CurrencyPair = "USD/JPY", .Rate = 154.1D, .VarianceTolerancePct = 1.5D}}

            Holidays = New List(Of HolidayCalendarEntry) From {
                New HolidayCalendarEntry With {.Year = 2026, .CountryCode = "US", .WeeklyHolidays = New List(Of DayOfWeek) From {DayOfWeek.Saturday, DayOfWeek.Sunday}, .FloatingHolidays = New List(Of DateTime) From {New DateTime(2026, 1, 1), New DateTime(2026, 7, 4), New DateTime(2026, 12, 25)}}}

            TransactionCodes = New List(Of TransactionCodeDefinition) From {
                New TransactionCodeDefinition With {.TxnCode = 101, .Narrative = "Cash Deposit", .DebitGlLink = "100100", .CreditGlLink = "200100", .IsDebitToAccount = False},
                New TransactionCodeDefinition With {.TxnCode = 102, .Narrative = "Cash Withdrawal", .DebitGlLink = "200100", .CreditGlLink = "100100", .IsDebitToAccount = True},
                New TransactionCodeDefinition With {.TxnCode = 201, .Narrative = "ACH Credit", .DebitGlLink = "100200", .CreditGlLink = "200100", .IsDebitToAccount = False},
                New TransactionCodeDefinition With {.TxnCode = 202, .Narrative = "ACH Debit", .DebitGlLink = "200100", .CreditGlLink = "100200", .IsDebitToAccount = True},
                New TransactionCodeDefinition With {.TxnCode = 301, .Narrative = "Fee Charge", .DebitGlLink = "200100", .CreditGlLink = "400100", .IsDebitToAccount = True},
                New TransactionCodeDefinition With {.TxnCode = 601, .Narrative = "Interest Capitalization", .DebitGlLink = "500100", .CreditGlLink = "200100", .IsDebitToAccount = False}}

            GLAccounts = New List(Of GLAccount) From {
                New GLAccount With {.GlNumber = "100100", .Classification = "Asset", .ConsolidationNode = "Cash", .CurrencyRestricted = False},
                New GLAccount With {.GlNumber = "100200", .Classification = "Asset", .ConsolidationNode = "Clearing", .CurrencyRestricted = False},
                New GLAccount With {.GlNumber = "200100", .Classification = "Liability", .ConsolidationNode = "Deposits", .CurrencyRestricted = False},
                New GLAccount With {.GlNumber = "400100", .Classification = "Income", .ConsolidationNode = "Fee Income", .CurrencyRestricted = False},
                New GLAccount With {.GlNumber = "500100", .Classification = "Expense", .ConsolidationNode = "Interest Expense", .CurrencyRestricted = False}}

            OverrideMatrix = New List(Of OverrideMatrixRule) From {
                New OverrideMatrixRule With {.TriggerCondition = "Amount > 10,000", .Severity = "Override Needed", .MinimumUserClass = "Supervisor"},
                New OverrideMatrixRule With {.TriggerCondition = "ChexScore < 550", .Severity = "Override Needed", .MinimumUserClass = "Supervisor"},
                New OverrideMatrixRule With {.TriggerCondition = "KYC Pending", .Severity = "Fatal", .MinimumUserClass = "Compliance"}}

            Customers = New List(Of Customer) From {
                New Customer With {.CustomerId = "CIF00001", .CustomerType = "Individual", .LegalName1 = "John Carter", .BirthOrIncorporationDate = New DateTime(1987, 3, 2), .Gender = "M", .TaxIdentifierType = "SSN", .TaxId = "111223333", .Citizenship = "US", .DomicileCountry = "US", .KycRiskRating = "Low", .PepStatus = False, .AddressType = "Physical", .StreetLine1 = "10 Main", .City = "Austin", .StateCode = "TX", .ZipCode = "78701", .PrimaryPhone = "+15125550101", .Email = "john@example.com", .EconomicSector = "Retail", .IndustryCode = "44", .RelationshipManagerId = "RM01", .StrategicTier = "Silver", .LifecycleStatus = "Active", .CipDocType = "Driver's License", .DocumentReferenceNo = "DL10001", .IssuingAuthority = "DMV", .IssueDate = New DateTime(2021, 1, 1), .ExpiryDate = New DateTime(2027, 1, 1), .LegacyCrossReferenceId = "LGC1", .KycStatus = "Passed"},
                New Customer With {.CustomerId = "CIF00002", .CustomerType = "Corporation", .LegalName1 = "Bluewave Logistics Inc.", .BirthOrIncorporationDate = New DateTime(2010, 9, 21), .Gender = "U", .TaxIdentifierType = "EIN", .TaxId = "993456789", .Citizenship = "US", .DomicileCountry = "US", .KycRiskRating = "Medium", .PepStatus = False, .AddressType = "Registered", .StreetLine1 = "200 Harbor", .City = "Seattle", .StateCode = "WA", .ZipCode = "98101", .PrimaryPhone = "+12065550102", .Email = "treasury@bluewave.example.com", .EconomicSector = "Corporate", .IndustryCode = "48", .RelationshipManagerId = "RM02", .StrategicTier = "Gold", .LifecycleStatus = "Active", .CipDocType = "Articles of Incorporation", .DocumentReferenceNo = "AOI2002", .IssuingAuthority = "WA SOS", .IssueDate = New DateTime(2010, 9, 21), .ExpiryDate = New DateTime(2099, 12, 31), .LegacyCrossReferenceId = "LGC2", .KycStatus = "Passed"},
                New Customer With {.CustomerId = "CIF00003", .CustomerType = "Individual", .LegalName1 = "Ava Stone", .BirthOrIncorporationDate = New DateTime(2008, 5, 5), .Gender = "F", .TaxIdentifierType = "SSN", .TaxId = "112229999", .Citizenship = "US", .DomicileCountry = "US", .KycRiskRating = "Low", .PepStatus = False, .AddressType = "Mailing", .StreetLine1 = "55 Pine", .City = "Denver", .StateCode = "CO", .ZipCode = "80202", .PrimaryPhone = "+13035550103", .Email = "ava@example.com", .EconomicSector = "Retail", .IndustryCode = "81", .RelationshipManagerId = "RM01", .StrategicTier = "Bronze", .LifecycleStatus = "Prospect", .CipDocType = "Passport", .DocumentReferenceNo = "P3003", .IssuingAuthority = "US DOS", .IssueDate = New DateTime(2020, 6, 20), .ExpiryDate = New DateTime(2026, 8, 15), .LegacyCrossReferenceId = "LGC3", .KycStatus = "Passed"},
                New Customer With {.CustomerId = "CIF00004", .CustomerType = "Sole Proprietorship", .LegalName1 = "Green Farm Supplies", .BirthOrIncorporationDate = New DateTime(2016, 2, 1), .Gender = "U", .TaxIdentifierType = "EIN", .TaxId = "881234567", .Citizenship = "US", .DomicileCountry = "US", .KycRiskRating = "High", .PepStatus = False, .AddressType = "Billing", .StreetLine1 = "89 Country", .City = "Boise", .StateCode = "ID", .ZipCode = "83702", .PrimaryPhone = "+12085550104", .Email = "owner@greenfarm.example.com", .EconomicSector = "SME", .IndustryCode = "11", .RelationshipManagerId = "RM03", .StrategicTier = "Silver", .LifecycleStatus = "Pending Review", .CipDocType = "Driver's License", .DocumentReferenceNo = "DL4004", .IssuingAuthority = "ID DMV", .IssueDate = New DateTime(2021, 4, 5), .ExpiryDate = New DateTime(2026, 9, 1), .LegacyCrossReferenceId = "LGC4", .KycStatus = "Pending Review"},
                New Customer With {.CustomerId = "CIF00005", .CustomerType = "Trust", .LegalName1 = "Harper Family Trust", .BirthOrIncorporationDate = New DateTime(2018, 7, 1), .Gender = "U", .TaxIdentifierType = "EIN", .TaxId = "776543210", .Citizenship = "US", .DomicileCountry = "US", .KycRiskRating = "Medium", .PepStatus = True, .AddressType = "Registered", .StreetLine1 = "8 Oak", .City = "Miami", .StateCode = "FL", .ZipCode = "33101", .PrimaryPhone = "+13055550105", .Email = "admin@harpertrust.example.com", .EconomicSector = "Institutional", .IndustryCode = "52", .RelationshipManagerId = "RM04", .StrategicTier = "Gold", .LifecycleStatus = "Pending Review", .CipDocType = "Articles", .DocumentReferenceNo = "TR5005", .IssuingAuthority = "FL Court", .IssueDate = New DateTime(2018, 7, 1), .ExpiryDate = New DateTime(2030, 7, 1), .LegacyCrossReferenceId = "LGC5", .KycStatus = "Pending Review"}}

            Categories = New List(Of Category) From {
                New Category With {.CategoryCode = "1010", .ShortDescription = "Retail Interest Checking", .LongDescription = "Retail interest bearing", .AccountClassType = "Checking", .PermittedCurrencies = New List(Of String) From {"USD", "CAD"}, .AssetLiabilityCode = "Liability", .PrimaryGlLine = "200100", .InterestGlLine = "500100", .FeeIncomeGlLine = "400100", .AllowedCustomerTypes = New List(Of String) From {"Individual", "Sole Proprietorship"}, .MinAge = 18, .MaxAge = 120, .DormancyPeriodDays = 365, .EscheatmentMonths = 36, .AllowNegativeBalance = True, .DefaultPostingRestriction = "None", .InterestEligible = True, .DefaultInterestBasis = "Actual/365", .AccrualFrequency = "Daily", .CapitalizationFrequency = "Monthly", .TieringMethod = "Whole Balance", .RateTiers = New List(Of CategoryRateTier) From {New CategoryRateTier With {.TierFrom = 0D, .TierTo = 9999.99D, .AnnualRatePct = 0.5D}, New CategoryRateTier With {.TierFrom = 10000D, .TierTo = 999999D, .AnnualRatePct = 1.1D}}, .ChargeScheduleCode = "CHK-STD", .FlatMonthlyFee = 7.5D, .StatementCycleTemplate = "MonthEnd", .StatementCycleDay = 31, .TaxReportingCategory = "1099-INT", .SweepingAllowed = False, .LegacyProductCode = "LP1010", .EpcId = "EPC-CHK-INT", .OpeningMinimum = 100D},
                New Category With {.CategoryCode = "1000", .ShortDescription = "Basic Non-Interest Checking", .LongDescription = "Basic checking", .AccountClassType = "Checking", .PermittedCurrencies = New List(Of String) From {"USD"}, .AssetLiabilityCode = "Liability", .PrimaryGlLine = "200100", .InterestGlLine = "500100", .FeeIncomeGlLine = "400100", .AllowedCustomerTypes = New List(Of String) From {"Individual", "Sole Proprietorship", "Corporation", "Trust"}, .MinAge = 16, .MaxAge = 120, .DormancyPeriodDays = 180, .EscheatmentMonths = 36, .AllowNegativeBalance = False, .DefaultPostingRestriction = "None", .InterestEligible = False, .DefaultInterestBasis = "Actual/365", .AccrualFrequency = "Daily", .CapitalizationFrequency = "Monthly", .TieringMethod = "None", .RateTiers = New List(Of CategoryRateTier), .ChargeScheduleCode = "CHK-BASIC", .FlatMonthlyFee = 3D, .StatementCycleTemplate = "MonthEnd", .StatementCycleDay = 31, .TaxReportingCategory = "1042-S", .SweepingAllowed = False, .LegacyProductCode = "LP1000", .EpcId = "EPC-CHK-BASIC", .OpeningMinimum = 25D},
                New Category With {.CategoryCode = "2100", .ShortDescription = "Commercial Money Market", .LongDescription = "Commercial MM", .AccountClassType = "Savings", .PermittedCurrencies = New List(Of String) From {"USD", "EUR"}, .AssetLiabilityCode = "Liability", .PrimaryGlLine = "200100", .InterestGlLine = "500100", .FeeIncomeGlLine = "400100", .AllowedCustomerTypes = New List(Of String) From {"Corporation", "Trust", "Institutional"}, .MinAge = 0, .MaxAge = 200, .DormancyPeriodDays = 365, .EscheatmentMonths = 24, .AllowNegativeBalance = False, .DefaultPostingRestriction = "None", .InterestEligible = True, .DefaultInterestBasis = "Actual/360", .AccrualFrequency = "Daily", .CapitalizationFrequency = "Monthly", .TieringMethod = "Graduated", .RateTiers = New List(Of CategoryRateTier) From {New CategoryRateTier With {.TierFrom = 0D, .TierTo = 24999D, .AnnualRatePct = 0.7D}, New CategoryRateTier With {.TierFrom = 25000D, .TierTo = 999999D, .AnnualRatePct = 1.45D}}, .ChargeScheduleCode = "MM-COMM", .FlatMonthlyFee = 12D, .StatementCycleTemplate = "MonthEnd", .StatementCycleDay = 31, .TaxReportingCategory = "1099-INT", .SweepingAllowed = True, .LegacyProductCode = "LP2100", .EpcId = "EPC-MM-COMM", .OpeningMinimum = 5000D}}

            Accounts = New List(Of Account) From {
                New Account With {.ApplicationReferenceNumber = "APP000001", .AccountNumber = "DDA000001", .PrimaryCustomerId = "CIF00001", .OwnershipType = "Sole", .JointHolderIds = New List(Of String), .CategoryCode = "1000", .AccountTitle = "John Carter Basic", .CurrencyCode = "USD", .LedgerBalance = 80D, .ClearedBalance = 80D, .BlockedHeldAmount = 0D, .AccruedInterest = 0D, .AccountStatus = "New-Unfunded", .PostingRestrictionCode = "None", .OverdraftAllowed = False, .OverdraftOptIn = False, .OverdraftLimit = 0D, .DateOpened = New DateTime(2026, 7, 7), .LastTransactionDate = New DateTime(2026, 7, 7), .InterestPlan = "None", .VarianceRatePct = 0D, .TaxWithholdingCode = "NONE", .StatementCycle = "MonthEnd", .FeeSchedule = "CHK-BASIC", .GlClassCategory = "Liability", .CostCenterBranchId = "BR001", .AlternativeAccountNumber = "ALT-0001", .ExternalCoreCrossReferenceId = "EXT-0001", .ShadowStatus = False, .Holds = New List(Of AccountHold)},
                New Account With {.ApplicationReferenceNumber = "APP000002", .AccountNumber = "DDA000002", .PrimaryCustomerId = "CIF00002", .OwnershipType = "Sole", .JointHolderIds = New List(Of String), .CategoryCode = "2100", .AccountTitle = "Bluewave Operating", .CurrencyCode = "USD", .LedgerBalance = 25600D, .ClearedBalance = 25600D, .BlockedHeldAmount = 0D, .AccruedInterest = 42D, .AccountStatus = "Active", .PostingRestrictionCode = "None", .OverdraftAllowed = True, .OverdraftOptIn = True, .OverdraftLimit = 10000D, .DateOpened = New DateTime(2025, 12, 1), .LastTransactionDate = New DateTime(2026, 7, 8), .InterestPlan = "Graduated", .VarianceRatePct = 0.1D, .TaxWithholdingCode = "NONE", .StatementCycle = "MonthEnd", .FeeSchedule = "MM-COMM", .GlClassCategory = "Liability", .CostCenterBranchId = "BR001", .AlternativeAccountNumber = "ALT-0002", .ExternalCoreCrossReferenceId = "EXT-0002", .ShadowStatus = False, .Holds = New List(Of AccountHold)},
                New Account With {.ApplicationReferenceNumber = "APP000003", .AccountNumber = "DDA000003", .PrimaryCustomerId = "CIF00003", .OwnershipType = "Custodian", .JointHolderIds = New List(Of String) From {"CIF00001"}, .CategoryCode = "1000", .AccountTitle = "Ava Stone Student", .CurrencyCode = "USD", .LedgerBalance = 220D, .ClearedBalance = 220D, .BlockedHeldAmount = 0D, .AccruedInterest = 0D, .AccountStatus = "Dormant", .PostingRestrictionCode = "No Debits", .OverdraftAllowed = False, .OverdraftOptIn = False, .OverdraftLimit = 0D, .DateOpened = New DateTime(2024, 2, 1), .LastTransactionDate = New DateTime(2025, 12, 15), .InterestPlan = "None", .VarianceRatePct = 0D, .TaxWithholdingCode = "NONE", .StatementCycle = "MonthEnd", .FeeSchedule = "CHK-BASIC", .GlClassCategory = "Liability", .CostCenterBranchId = "BR001", .AlternativeAccountNumber = "ALT-0003", .ExternalCoreCrossReferenceId = "EXT-0003", .ShadowStatus = False, .Holds = New List(Of AccountHold)},
                New Account With {.ApplicationReferenceNumber = "APP000004", .AccountNumber = "DDA000004", .PrimaryCustomerId = "CIF00004", .OwnershipType = "Sole", .JointHolderIds = New List(Of String), .CategoryCode = "1010", .AccountTitle = "Green Farm Checking", .CurrencyCode = "USD", .LedgerBalance = 640D, .ClearedBalance = 640D, .BlockedHeldAmount = 125D, .AccruedInterest = 3.2D, .AccountStatus = "Frozen", .PostingRestrictionCode = "Total Freeze", .OverdraftAllowed = True, .OverdraftOptIn = False, .OverdraftLimit = 500D, .DateOpened = New DateTime(2026, 2, 14), .LastTransactionDate = New DateTime(2026, 6, 30), .InterestPlan = "Whole Balance", .VarianceRatePct = 0D, .TaxWithholdingCode = "BWH", .StatementCycle = "MonthEnd", .FeeSchedule = "CHK-STD", .GlClassCategory = "Liability", .CostCenterBranchId = "BR001", .AlternativeAccountNumber = "ALT-0004", .ExternalCoreCrossReferenceId = "EXT-0004", .ShadowStatus = True, .Holds = New List(Of AccountHold) From {New AccountHold With {.Amount = 125D, .ReleaseDate = New DateTime(2026, 7, 9), .Reason = "Reg CC", .Released = False}}}}

            Transactions = New List(Of TransactionHistory)
            GLJournal = New List(Of GLJournalEntry)
            Dim rows = New List(Of Tuple(Of String, Integer, Decimal, Decimal, Boolean)) From {
                Tuple.Create("DDA000002", 101, 5000D, 25000D, False), Tuple.Create("DDA000002", 201, 900D, 25900D, False), Tuple.Create("DDA000002", 301, 12D, 25888D, False), Tuple.Create("DDA000002", 102, 288D, 25600D, False),
                Tuple.Create("DDA000004", 101, 500D, 500D, False), Tuple.Create("DDA000004", 301, 10D, 490D, False), Tuple.Create("DDA000004", 201, 150D, 640D, False),
                Tuple.Create("DDA000003", 101, 100D, 100D, False), Tuple.Create("DDA000003", 101, 150D, 250D, False), Tuple.Create("DDA000003", 301, 15D, 235D, False), Tuple.Create("DDA000003", 102, 15D, 220D, False),
                Tuple.Create("DDA000001", 101, 80D, 80D, True), Tuple.Create("DDA000002", 601, 42D, 25642D, False), Tuple.Create("DDA000002", 301, 42D, 25600D, False), Tuple.Create("DDA000004", 202, 50D, 590D, True)}
            For i = 0 To rows.Count - 1
                Dim r = rows(i)
                Transactions.Add(New TransactionHistory With {.TransactionId = "TXN" & (i + 1).ToString("000000"), .TxnDate = Dates.CurrentBusinessDate, .AccountNumber = r.Item1, .TxnCode = r.Item2, .Narrative = "Seed txn", .Amount = r.Item3, .RunningBalance = r.Item4, .IsMemo = r.Item5})
                Dim c = TransactionCodes.First(Function(x) x.TxnCode = r.Item2)
                GLJournal.Add(New GLJournalEntry With {.JournalId = "JRN" & (i + 1).ToString("000000"), .PostedAt = Dates.CurrentBusinessDate, .AccountNumber = r.Item1, .TxnCode = r.Item2, .DebitGL = c.DebitGlLink, .CreditGL = c.CreditGlLink, .Amount = r.Item3, .IsMemo = r.Item5})
            Next

            Events = New List(Of EventLogEntry) From {New EventLogEntry With {.EventName = "SYSTEM.STARTUP", .Timestamp = DateTime.UtcNow, .Payload = "{"event":"SYSTEM.STARTUP"}"}}
            _initialized = True
        End SyncLock
    End Sub

    Public Function NextCustomerId() As String
        SyncLock _syncRoot
            _customerCounter += 1 : Return "CIF" & _customerCounter.ToString("00000")
        End SyncLock
    End Function
    Public Function NextAccountNumber() As String
        SyncLock _syncRoot
            _accountCounter += 1 : Return "DDA" & _accountCounter.ToString("000000")
        End SyncLock
    End Function
    Public Function NextApplicationRef() As String
        SyncLock _syncRoot
            _applicationCounter += 1 : Return "APP" & _applicationCounter.ToString("000000")
        End SyncLock
    End Function
    Public Function NextTransactionId() As String
        SyncLock _syncRoot
            _transactionCounter += 1 : Return "TXN" & _transactionCounter.ToString("000000")
        End SyncLock
    End Function
    Public Function NextJournalId() As String
        SyncLock _syncRoot
            _journalCounter += 1 : Return "JRN" & _journalCounter.ToString("000000")
        End SyncLock
    End Function
End Module
