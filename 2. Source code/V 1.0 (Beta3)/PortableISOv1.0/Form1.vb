Imports System.IO
Imports System.Text.RegularExpressions

Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Process.Start("wpeinit.exe")
        Process.Start("powershell.exe", "Set-ExecutionPolicy Unrestricted -Force")
        Process.Start("powershell.exe", "[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12")
        Dim welcomeScreen As New WelcomeScreen()
        welcomeScreen.Show()
        Me.Close()
    End Sub
End Class

