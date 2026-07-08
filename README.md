# Core Banking App (ASP.NET Web Forms, VB.NET, .NET Framework 4.8)

## Project
- Solution: `/home/runner/work/core-banking-system-app/core-banking-system-app/CoreBankingApp/CoreBankingApp.sln`
- Web project: `CoreBankingApp`
- All data is static in-memory via `App_Code/DataStore/StaticDataStore.vb`.
- No database is used.

## How to run
1. Open `CoreBankingApp.sln` in Visual Studio 2022+ on Windows with .NET Framework 4.8 developer pack.
2. Build solution.
3. Run with IIS Express.
4. `Global.asax` calls `StaticDataStore.Initialize()` at startup to seed demo data.

## Structure
- `App_Code/Models` - Entity models for system setup, CIF, product, account, transaction, GL, and event log.
- `App_Code/Services` - Business logic and validation (customer onboarding, account open lifecycle, posting, EOD).
- `App_Code/DataStore/StaticDataStore.vb` - Thread-safe counters and seeded `List(Of T)` data.
- `Pages` - Web Forms UI pages.
- `Site.Master` - Left navigation grouped by modules.

## Screen map
- Dashboard: `Default.aspx`
- System Setup: `CompanyParameter.aspx`, `SystemDates.aspx`, `SystemParameters.aspx`, `CurrencyMaster.aspx`, `ExchangeRates.aspx`, `HolidayCalendar.aspx`, `TransactionCodes.aspx`, `GLAccounts.aspx`, `OverrideMatrix.aspx`
- Customer (CIF): `CustomerList.aspx`, `CustomerCreate.aspx`, `CustomerDetail.aspx`
- Product Catalog: `CategoryList.aspx`, `CategoryCreate.aspx`
- Account Management: `AccountList.aspx`, `AccountOpen.aspx`, `AccountDetail.aspx`
- DDA Operations: `TransactionPost.aspx`, `GLJournal.aspx`
- Batch/EOD: `EODProcess.aspx`, `EventLog.aspx`

## Seeded demo data
- 5 customers
- 3 categories
- 4 accounts (New-Unfunded, Active, Dormant, Frozen)
- 15 transaction history records
- System setup masters for currencies, exchange rates, holidays, transaction codes, GL, and override matrix
