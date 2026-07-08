Imports System
Imports System.Linq
Namespace CoreBankingApp
Public Class Pages_CategoryCreate
Inherits UI.Page
Private ReadOnly svc As New CategoryService()
Protected Sub btnCreate_Click(sender As Object,e As EventArgs) Handles btnCreate.Click
Dim c As New Category With {.CategoryCode=txtCode.Text,.ShortDescription=txtShort.Text,.LongDescription=txtLong.Text,.AccountClassType=ddlClass.SelectedValue,.PermittedCurrencies=txtCurr.Text.Split(","c).Select(Function(x)x.Trim()).Where(Function(x)x<>"").ToList(),.AssetLiabilityCode="Liability",.PrimaryGlLine="200100",.InterestGlLine="500100",.FeeIncomeGlLine="400100",.AllowedCustomerTypes=New Collections.Generic.List(Of String) From {"Individual","Sole Proprietorship","Corporation","Trust"},.MinAge=Integer.Parse(txtMinAge.Text),.MaxAge=Integer.Parse(txtMaxAge.Text),.DormancyPeriodDays=365,.EscheatmentMonths=36,.AllowNegativeBalance=False,.DefaultPostingRestriction="None",.InterestEligible=True,.DefaultInterestBasis="Actual/365",.AccrualFrequency="Daily",.CapitalizationFrequency="Monthly",.TieringMethod="Whole Balance",.RateTiers=New Collections.Generic.List(Of CategoryRateTier) From {New CategoryRateTier With {.TierFrom=0,.TierTo=999999,.AnnualRatePct=0.5D}},.ChargeScheduleCode="CUSTOM",.FlatMonthlyFee=Decimal.Parse(txtFee.Text),.StatementCycleTemplate="MonthEnd",.StatementCycleDay=31,.TaxReportingCategory="1099-INT",.SweepingAllowed=False,.LegacyProductCode="",.EpcId="",.OpeningMinimum=Decimal.Parse(txtOpenMin.Text)}
Dim m=svc.CreateCategory(c):lbl.Text=m:lbl.CssClass=If(m.Contains("created"),"text-success","text-danger")
End Sub
End Class
End Namespace