Imports System
Imports System.Collections.Generic

''' <summary>Category rate tier entity.</summary>
Public Class CategoryRateTier
    Public Property TierFrom As Decimal
    Public Property TierTo As Decimal
    Public Property AnnualRatePct As Decimal
End Class

''' <summary>Product category entity.</summary>
Public Class Category
    Public Property CategoryCode As String
    Public Property ShortDescription As String
    Public Property LongDescription As String
    Public Property AccountClassType As String
    Public Property PermittedCurrencies As List(Of String)
    Public Property AssetLiabilityCode As String
    Public Property PrimaryGlLine As String
    Public Property InterestGlLine As String
    Public Property FeeIncomeGlLine As String
    Public Property AllowedCustomerTypes As List(Of String)
    Public Property MinAge As Integer
    Public Property MaxAge As Integer
    Public Property DormancyPeriodDays As Integer
    Public Property EscheatmentMonths As Integer
    Public Property AllowNegativeBalance As Boolean
    Public Property DefaultPostingRestriction As String
    Public Property InterestEligible As Boolean
    Public Property DefaultInterestBasis As String
    Public Property AccrualFrequency As String
    Public Property CapitalizationFrequency As String
    Public Property TieringMethod As String
    Public Property RateTiers As List(Of CategoryRateTier)
    Public Property ChargeScheduleCode As String
    Public Property FlatMonthlyFee As Decimal
    Public Property StatementCycleTemplate As String
    Public Property StatementCycleDay As Integer
    Public Property TaxReportingCategory As String
    Public Property SweepingAllowed As Boolean
    Public Property LegacyProductCode As String
    Public Property EpcId As String
    Public Property OpeningMinimum As Decimal
End Class

''' <summary>Funding hold entity.</summary>
Public Class AccountHold
    Public Property Amount As Decimal
    Public Property ReleaseDate As DateTime
    Public Property Reason As String
    Public Property Released As Boolean
End Class

''' <summary>Transaction history entity.</summary>
Public Class TransactionHistory
    Public Property TransactionId As String
    Public Property TxnDate As DateTime
    Public Property AccountNumber As String
    Public Property TxnCode As Integer
    Public Property Narrative As String
    Public Property Amount As Decimal
    Public Property RunningBalance As Decimal
    Public Property IsMemo As Boolean
End Class

''' <summary>GL journal entry entity.</summary>
Public Class GLJournalEntry
    Public Property JournalId As String
    Public Property PostedAt As DateTime
    Public Property AccountNumber As String
    Public Property TxnCode As Integer
    Public Property DebitGL As String
    Public Property CreditGL As String
    Public Property Amount As Decimal
    Public Property IsMemo As Boolean
End Class

''' <summary>DDA account entity.</summary>
Public Class Account
    Public Property ApplicationReferenceNumber As String
    Public Property AccountNumber As String
    Public Property PrimaryCustomerId As String
    Public Property OwnershipType As String
    Public Property JointHolderIds As List(Of String)
    Public Property CategoryCode As String
    Public Property AccountTitle As String
    Public Property CurrencyCode As String
    Public Property LedgerBalance As Decimal
    Public Property ClearedBalance As Decimal
    Public Property BlockedHeldAmount As Decimal
    Public Property AccruedInterest As Decimal
    Public Property AccountStatus As String
    Public Property PostingRestrictionCode As String
    Public Property OverdraftAllowed As Boolean
    Public Property OverdraftOptIn As Boolean
    Public Property OverdraftLimit As Decimal
    Public Property DateOpened As DateTime
    Public Property LastTransactionDate As DateTime
    Public Property InterestPlan As String
    Public Property VarianceRatePct As Decimal
    Public Property TaxWithholdingCode As String
    Public Property StatementCycle As String
    Public Property FeeSchedule As String
    Public Property GlClassCategory As String
    Public Property CostCenterBranchId As String
    Public Property AlternativeAccountNumber As String
    Public Property ExternalCoreCrossReferenceId As String
    Public Property ShadowStatus As Boolean
    Public Property Holds As List(Of AccountHold)

    Public ReadOnly Property AvailableBalance As Decimal
        Get
            Return LedgerBalance - BlockedHeldAmount + OverdraftLimit
        End Get
    End Property
End Class

''' <summary>Event log entity.</summary>
Public Class EventLogEntry
    Public Property EventName As String
    Public Property Payload As String
    Public Property Timestamp As DateTime
End Class
