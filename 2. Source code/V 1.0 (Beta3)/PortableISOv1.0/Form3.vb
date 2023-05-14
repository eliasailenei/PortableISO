Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions
Public Class Form3

    Dim install As Boolean
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If versel = 1 Then
            isover = 8
        ElseIf versel = 2 Then
            isover = 10
        ElseIf versel = 3 Then
            isover = 11
        ElseIf versel = 0 Then
            isover = 7
        End If
        ServicePointManager.Expect100Continue = True
        ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)
        install = True
        ListBox1.Items.Add("Install started")
        Button1.Enabled = False
        Button1.Visible = False
        Call drivemake()
        Call makeunattend()
        Call getiso()
        Dim client As New WebClient()
        AddHandler client.DownloadProgressChanged, AddressOf DownloadProgressChanged
        AddHandler client.DownloadFileCompleted, AddressOf DownloadFileCompleted
        client.DownloadFileAsync(New Uri(isourl), "F:\W" & isover & ".iso")
    End Sub

    Private Sub DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs)
        If e.Error Is Nothing Then
            installwindows()
            Me.Close()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If install = False Then
            ListBox1.Items.Add("Install halted")
            Button1.Enabled = False
            Button1.Visible = False
            Process.Start("wpeutil.exe", "reboot")
        End If
        If install = True Then
            ListBox1.Items.Add("User requested cancellation, denied as it can brick your PC... Continuing...")
            Button2.Enabled = False
            Button2.Visible = False
            MessageBox.Show("Too late! Don't shut down your PC as your can brick you PC! You can render your hard drive useless!")
        End If
    End Sub
    Private Sub drivemake()
        ListBox1.Items.Add("Creating Disk...")
        Dim filePath As String = "X:\clear.txt"
        Dim scriptContent As String = "select disk " & letter & vbCrLf &
                                      "clean" & vbCrLf &
                                      "create partition primary" & vbCrLf &
                                      "format fs=ntfs label=System quick" & vbCrLf &
                                      "assign letter=C" & vbCrLf &
                                      "shrink desired=20000 " & vbCrLf &
                                      "create partition primary size=20000" & vbCrLf &
                                      "format fs=ntfs label=Temp quick" & vbCrLf &
                                      "assign letter=F" & vbCrLf
        File.WriteAllText(filePath, scriptContent)

        Dim com As New ProcessStartInfo()
        com.FileName = "cmd.exe"
        com.Arguments = "/C diskpart /s X:\clear.txt"
        com.CreateNoWindow = True
        com.UseShellExecute = False
        com.RedirectStandardInput = True
        com.RedirectStandardOutput = True

        Dim p As Process = Process.Start(com)


        p.StandardInput.WriteLine("exit")


        p.WaitForExit()


        Dim output As String = p.StandardOutput.ReadToEnd()
    End Sub
    Sub makeunattend()
        ListBox1.Items.Add("Creating script...")
        Dim filePath As String = "F:\autounattend.xml"
        Dim scriptContent As String = "<unattend xmlns=""urn:schemas-microsoft-com:unattend"">" & vbCrLf &
                              "  <settings pass=""windowsPE"">" & vbCrLf &
                              "    <component name=""Microsoft-Windows-International-Core-WinPE"" processorArchitecture=""amd64"" publicKeyToken=""31bf3856ad364e35"" language=""neutral"" versionScope=""nonSxS"" xmlns:wcm=""http://schemas.microsoft.com/WMIConfig/2002/State"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">" & vbCrLf &
                              "      <SetupUILanguage>" & vbCrLf &
                              "        <UILanguage>en-US</UILanguage>" & vbCrLf &
                              "      </SetupUILanguage>" & vbCrLf &
                              "      <InputLocale>1033:00000409</InputLocale>" & vbCrLf &
                              "      <SystemLocale>en-US</SystemLocale>" & vbCrLf &
                              "      <UILanguage>en-US</UILanguage>" & vbCrLf &
                              "      <UserLocale>en-US</UserLocale>" & vbCrLf &
                              "    </component>" & vbCrLf &
                              "    <component name=""Microsoft-Windows-Setup"" processorArchitecture=""amd64"" publicKeyToken=""31bf3856ad364e35"" language=""neutral"" versionScope=""nonSxS"" xmlns:wcm=""http://schemas.microsoft.com/WMIConfig/2002/State"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">" & vbCrLf &
                              "      <EnableFirewall>true</EnableFirewall>" & vbCrLf &
                              "      <EnableNetwork>true</EnableNetwork>" & vbCrLf &
                              "      <ImageInstall>" & vbCrLf &
                              "        <OSImage>" & vbCrLf &
                              "          <InstallTo>" & vbCrLf &
                              "            <DiskID>" & letter & "</DiskID>" & vbCrLf &
                              "            <PartitionID>1</PartitionID>" & vbCrLf &
                              "          </InstallTo>" & vbCrLf &
                              "        </OSImage>" & vbCrLf &
                              "      </ImageInstall>" & vbCrLf &
                              "      <UserData>" & vbCrLf &
                              "        <AcceptEula>true</AcceptEula>" & vbCrLf &
                              "        <ProductKey>" & vbCrLf &
                              "          <WillShowUI>Never</WillShowUI>" & vbCrLf &
                              "          <Key>W269N-WFGWX-YVC9B-4J6C9-T83GX</Key>" & vbCrLf &
                              "        </ProductKey>" & vbCrLf &
                              "      </UserData>" & vbCrLf &
                              "    </component>" & vbCrLf &
                              "  </settings>" & vbCrLf &
                              "</unattend>"
        File.WriteAllText(filePath, scriptContent)
    End Sub
    Sub getiso()

        ListBox1.Items.Add("Downloading ISO...")
        If versel = 0 Then
            isourl = "https://archive.org/download/win-7-ult-sp-1-english-x-64/Win7_Ult_SP1_English_x64.iso"
        Else
            Dim powerShell As Diagnostics.Process = New Diagnostics.Process()
            powerShell.StartInfo.FileName = "powershell.exe"
            powerShell.StartInfo.Arguments = "X:\Windows\System32\Fido.ps1 -Win " & isover & " -Lang En -GetUrl"
            powerShell.StartInfo.RedirectStandardOutput = True
            powerShell.StartInfo.UseShellExecute = False
            powerShell.StartInfo.CreateNoWindow = True
            powerShell.Start()
            Dim output As String = powerShell.StandardOutput.ReadToEnd()
            powerShell.WaitForExit()
            If Not output.StartsWith("https") Then
                MessageBox.Show("Error: You have been banned from the Microsoft servers for too many requests. Try again in 24 hours...")
            Else
                isourl = output
            End If
        End If
    End Sub
    Private Sub DownloadProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)
        Dim progress As Integer = CInt((e.BytesReceived / e.TotalBytesToReceive) * 100)
        ProgressBar1.Value = progress
        Label5.Text = progress & "%"
    End Sub
    Sub installwindows()
        ListBox1.Items.Add("Extracting ISO...")
        ProgressBar1.Value = 0
        Dim filePath As String = "X:\extract.bat"
        Dim scriptContent As String = "@echo off " & letter & vbCrLf &
                                      "cd /d X:\Windows\System32\" & vbCrLf &
                                      "cls" & vbCrLf &
                                      "7z x " & "F:\W" & isover & ".iso -oF:\ -y" & vbCrLf &
                                      "cls" & vbCrLf &
                                      "F:\setup.exe" & vbCrLf &
                                      "exit" & vbCrLf
        File.WriteAllText(filePath, scriptContent)
        Process.Start("cmd.exe", "/C start X:\extract.bat")
        Me.Close()
    End Sub

End Class