# Core Banking App (ASP.NET Web Forms, VB.NET, .NET Framework 4.8)

A browser-based core banking simulation built on ASP.NET Web Forms and VB.NET targeting .NET Framework 4.8. It models the essential back-office processes of a retail/commercial bank — customer onboarding, account opening, teller posting, interest accrual, and end-of-day batch — entirely in-memory with no external database.

## Table of Contents
- [Project overview](#project-overview)
- [Functional details](#functional-details)
- [How to set up and run in Visual Studio](#how-to-set-up-and-run-in-visual-studio)
- [How to set up and run in VS Code](#how-to-set-up-and-run-in-vs-code)
- [Project structure](#project-structure)
- [Screen map](#screen-map)
- [Seeded demo data](#seeded-demo-data)

---

## Project overview
- **Solution**: `CoreBankingApp/CoreBankingApp.sln`
- **Web project**: `CoreBankingApp`
- **Runtime**: .NET Framework 4.8, ASP.NET Web Forms
- **Data storage**: All data lives in-memory via `App_Code/DataStore/StaticDataStore.vb` — no database, no file I/O.
- **Startup hook**: `Global.asax` calls `StaticDataStore.Initialize()` on application start to seed demo data.

---

## Functional details

### System Setup
Administrators configure bank-wide parameters before any customer or account work:

| Page | Purpose |
|---|---|
| **Company Parameters** | Bank identity: legal name, LEI/bank code, base currency, head-office branch, tax ID (EIN), country code. |
| **System Dates** | Current business date, last working day, next working day, EOD phase status, and next statement date. Drives all date-sensitive business rules. |
| **System Parameters** | Session timeout, default language, audit log retention days, and maintenance-window flag. |
| **Currency Master** | ISO currency codes, decimal places, minor-unit names, and rounding rules. |
| **Exchange Rates** | Spot / retail rates per currency pair with variance tolerance percentages. |
| **Holiday Calendar** | Per-year, per-country weekly holiday pattern and floating (fixed-date) holidays; used by the date service to skip non-working days. |
| **Transaction Codes** | Defines every posting code: narrative, debit/credit GL link, and whether the code is a debit or credit to the account ledger. |
| **GL Accounts** | Chart of accounts: GL number, asset/liability/income/expense classification, consolidation node, and currency restriction. |
| **Override Matrix** | Approval rules: maps a trigger condition (e.g., large-amount override, KYC exception) to a severity level and the minimum user class that may approve it. |

### Customer Information File (CIF)
Full customer lifecycle management:
- **Customer types**: Individual and Corporate.
- **Fields captured**: legal names, preferred trade name, date of birth/incorporation, gender, tax identifier (SSN/EIN with masked display), citizenship, domicile country, KYC risk rating, PEP flag, backup-withholding flag, address (street/city/state/ZIP), primary phone, email, economic sector, industry code, relationship manager, strategic tier, and lifecycle status.
- **CIP / identity documents**: document type, reference number, issuing authority, issue/expiry dates.
- **KYC status**: drives eligibility for account opening (must be `Passed` with lifecycle `Active`).
- **Lifecycle status**: `Active`, `Dormant`, `Closed`, etc.

### Product Catalog (Categories)
A *Category* is the product template every account inherits:
- **Account class**: asset/liability side, permitted currencies, allowed customer types, age limits.
- **GL wiring**: primary GL line, interest GL line, fee income GL line.
- **Interest**: tiered rate schedules (from/to balance bands with annual rate %), interest basis (Actual/360 or Actual/365), accrual and capitalisation frequencies, tiering method.
- **Fees**: charge schedule code, flat monthly fee.
- **Account rules**: dormancy period (days), escheatment months, allow-negative-balance flag, default posting restriction, opening minimum balance.
- **Statement**: cycle template and cycle day.
- **Miscellaneous**: tax reporting category, sweeping allowed, legacy product code, EPC ID.

### Account Opening Lifecycle
Account creation follows a three-stage workflow:

1. **Intake validation** — customer exists; category exists; customer type is permitted by category; individual age is within category min/max age limits; ownership type is provided.
2. **Compliance gate** — KYC status must be `Passed` and lifecycle must be `Active`; a synthetic ChexSystems score (400–800) is computed deterministically from the customer ID; scores below 550 require an authorised override to proceed.
3. **Provision** — account record is created with status `New-Unfunded`; defaults are copied from the category (posting restriction, overdraft flag, interest plan, statement cycle, fee schedule, GL category, branch).
4. **Initial funding** — a positive funding amount moves the account to `Active` once the net cleared balance meets or exceeds the category's opening minimum. Check deposits are held under Regulation CC (2-business-day hold) and block the available balance until released by EOD.

**Available balance** = Ledger Balance − Blocked/Held Amount + Overdraft Limit.

### DDA Teller Posting
The posting service enforces real-time rules on every transaction:
- Amount must be positive.
- Account must exist and not be `Frozen` or `Closed`.
- Transaction code must exist in the master list.
- Amounts exceeding $10,000 require supervisor override.
- Posting restriction codes are honoured: `Total Freeze` blocks all postings; `No Debits` blocks debits; `No Credits` blocks credits.
- Debit transactions check available balance; overdraft is only permitted if `OverdraftAllowed` is true and the post does not breach the overdraft limit.
- Card/ATM debits additionally require `OverdraftOptIn` when the transaction would take the balance negative.
- Every accepted transaction creates a **memo-post** `TransactionHistory` record and a corresponding **GL journal entry** (both marked `IsMemo = True` until end-of-day finalisation).

### GL Journal
Mirrors every posting at the general-ledger level. Each journal entry records: journal ID, timestamp, account number, transaction code, debit GL line, credit GL line, amount, and memo flag. The GL journal view aggregates across all accounts.

### End-of-Day (EOD) Batch
The EOD process runs ten sequential steps and advances the business date:

| Step | Action |
|---|---|
| 1 | Set EOD phase status to `Running`. |
| 2 | Finalise memo-posts: flip `IsMemo` to `False` on all transactions and GL entries. |
| 3 | Release matured Reg CC holds: any hold with `ReleaseDate ≤ current business date` is marked released; blocked amount decreases and cleared balance increases. |
| 4 | Compute daily interest accrual: for each interest-eligible account, determine the applicable rate tier, add variance rate, apply day-count basis (Actual/360 or Actual/365), and accumulate to `AccruedInterest`. |
| 5 | Month-end capitalisation: if the next calendar day is in a new month, sweep `AccruedInterest` to `LedgerBalance` and `ClearedBalance` and post a transaction (TxnCode 601). |
| 6 | Apply statement-cycle fees: on the configured statement-cycle day, deduct the flat monthly fee and post a transaction (TxnCode 301). |
| 7 | Dormancy sweep: any `Active` account with no transaction activity for more than the category's dormancy period is moved to `Dormant`. |
| 8 | Reconciliation: verify that the sum of account ledger balances equals the total GL control balance. |
| 9 | Advance dates: set `LastWorkingDay`, compute `NextWorkingDay` (skipping weekends and holidays), update `CurrentBusinessDate`. |
| 10 | Emit `SYSTEM.DATE.ADVANCED` event to the event log. |

### Event Log
Append-only log of system-level events emitted by EOD and other batch processes. Each entry records event name, JSON payload, and UTC timestamp.

### Dashboard
Real-time operational snapshot: current business date, total customers, total/active/dormant/frozen account counts, total deposits (sum of all ledger balances), today's transaction count, and last EOD phase status.

---

## How to set up and run in Visual Studio

> **Requirements**: Windows OS, Visual Studio 2022 or later, .NET Framework 4.8 Developer Pack.

1. Clone the repository.
2. Open `CoreBankingApp/CoreBankingApp.sln` in Visual Studio.
3. Restore packages if prompted (right-click solution → *Restore NuGet Packages*).
4. Build the solution (**Build → Build Solution** or `Ctrl+Shift+B`).
5. Press **F5** (or click *IIS Express* in the toolbar) to start with debugging.
6. The browser opens automatically. `Global.asax` seeds all demo data on first request.

---

## How to set up and run in VS Code

> **Important**: This is a **classic ASP.NET Web Forms / .NET Framework 4.8** application. It cannot be run with `dotnet run` (which targets .NET Core / .NET 5+). Running the web server still requires **Visual Studio on Windows** or a manual MSBuild + IIS Express workflow (see below). VS Code is best used here for **browsing and editing code**.

### Prerequisites (Windows only)
- [Visual Studio Build Tools 2022](https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2022) with the **ASP.NET and web development** workload (installs MSBuild and IIS Express).
- [.NET Framework 4.8 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48).
- [VS Code](https://code.visualstudio.com/).

### Recommended VS Code extensions
Install these from the Extensions panel (`Ctrl+Shift+X`):

| Extension | Purpose |
|---|---|
| **VB.NET** (`mikhail-arkhipov.vbnet`) | Syntax highlighting and basic IntelliSense for VB.NET. |
| **C# Dev Kit** (`ms-dotnettools.csdevkit`) | General .NET tooling support. |
| **ASP.NET Core Snippets** | Snippet library useful for Web Forms markup patterns. |
| **XML** (`redhat.vscode-xml`) | Highlights `.aspx`, `.master`, and `Web.config` XML markup. |
| **GitLens** (`eamodio.gitlens`) | Enhanced Git integration. |

### Opening the project
1. Clone the repository.
   ```bash
   git clone https://github.com/choudhury-prosenjit/core-banking-system-app.git
   cd core-banking-system-app
   ```
2. Open the `CoreBankingApp` folder in VS Code:
   ```bash
   code CoreBankingApp
   ```
   You can now browse and edit all `.vb`, `.aspx`, `.master`, and config files.

### Building from the VS Code terminal (Windows)
Open the integrated terminal (`Ctrl+`` ` ``) and run MSBuild:

```powershell
# Adjust path if Visual Studio Build Tools are installed elsewhere
$msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
& $msbuild CoreBankingApp.sln /p:Configuration=Debug
```

A successful build produces the compiled assemblies in `CoreBankingApp\bin\`.

### Running with IIS Express from the terminal (Windows)
```powershell
# Adjust path to your IIS Express installation
$iisexpress = "C:\Program Files (x86)\IIS Express\iisexpress.exe"
& $iisexpress /path:"$PWD\CoreBankingApp" /port:44300
```

Then open `http://localhost:44300` in your browser.

> **Tip**: For the best debugging and run experience (breakpoints, hot reload, IntelliSense for `.aspx` code-behind) open the `.sln` in **Visual Studio 2022** instead of VS Code.

---

## Project structure

```
CoreBankingApp/
├── App_Code/
│   ├── DataStore/
│   │   └── StaticDataStore.vb      # Thread-safe in-memory store; seed data
│   ├── Models/
│   │   ├── ConfigModels.vb         # CompanyParameter, SystemDateControl, Currency,
│   │   │                           #   ExchangeRate, HolidayCalendar, TransactionCode,
│   │   │                           #   GLAccount, OverrideMatrixRule, SystemParameter
│   │   ├── CustomerModels.vb       # Customer (CIF) entity
│   │   └── AccountModels.vb        # Category, Account, TransactionHistory,
│   │                               #   GLJournalEntry, AccountHold, EventLogEntry
│   └── Services/
│       ├── CustomerService.vb      # CIF CRUD and lookup
│       ├── AccountCategoryPostingServices.vb
│       │                           # CategoryService, AccountService (lifecycle),
│       │                           #   PostingService (teller rules),
│       │                           #   EODService (batch), DashboardService
│       └── SetupDateServices.vb    # DateService (business-day calculation)
├── Pages/                          # All .aspx Web Forms pages (see Screen map)
├── Default.aspx                    # Dashboard
├── Site.Master                     # Shell layout with left navigation
├── Global.asax                     # Application lifecycle; calls StaticDataStore.Initialize()
├── Web.config                      # ASP.NET configuration
└── CoreBankingApp.sln
```

---

## Screen map

| Module | Pages |
|---|---|
| **Dashboard** | `Default.aspx` |
| **System Setup** | `CompanyParameter.aspx`, `SystemDates.aspx`, `SystemParameters.aspx`, `CurrencyMaster.aspx`, `ExchangeRates.aspx`, `HolidayCalendar.aspx`, `TransactionCodes.aspx`, `GLAccounts.aspx`, `OverrideMatrix.aspx` |
| **Customer (CIF)** | `CustomerList.aspx`, `CustomerCreate.aspx`, `CustomerDetail.aspx` |
| **Product Catalog** | `CategoryList.aspx`, `CategoryCreate.aspx` |
| **Account Management** | `AccountList.aspx`, `AccountOpen.aspx`, `AccountDetail.aspx` |
| **DDA Operations** | `TransactionPost.aspx`, `GLJournal.aspx` |
| **Batch / EOD** | `EODProcess.aspx`, `EventLog.aspx` |

---

## Seeded demo data

| Entity | Count / Details |
|---|---|
| Customers | 5 (mix of Individual and Corporate) |
| Product categories | 3 |
| Accounts | 4 — one each of `New-Unfunded`, `Active`, `Dormant`, `Frozen` |
| Transaction history | 15 records |
| System setup masters | Currencies, exchange rates, holiday calendar, transaction codes, GL accounts, override matrix rules |
