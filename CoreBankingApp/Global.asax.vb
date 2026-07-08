Imports System
Imports System.Web

Namespace CoreBankingApp
    Public Class Global_asax
        Inherits HttpApplication

        Protected Sub Application_Start(sender As Object, e As EventArgs)
            StaticDataStore.Initialize()
        End Sub
    End Class
End Namespace
