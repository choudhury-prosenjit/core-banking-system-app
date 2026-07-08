Imports System
Namespace CoreBankingApp
Public Class Pages_EODProcess
Inherits UI.Page
Private ReadOnly eod As New EODService()
Protected Sub btnRun_Click(sender As Object,e As EventArgs) Handles btnRun.Click
bl.DataSource=eod.RunEod():bl.DataBind()
End Sub
End Class
End Namespace