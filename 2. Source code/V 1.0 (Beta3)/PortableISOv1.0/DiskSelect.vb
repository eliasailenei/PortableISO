Imports System.Diagnostics
Public Class DiskSelect



    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        letter = TextBox1.Text
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        MessageBox.Show("Warning! You are about to completley format Drive " & letter & "! Please make sure you have no important data on it!")
        Form3.Show()
        Me.Close()

    End Sub

    Private Sub DiskSelect_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim com As New ProcessStartInfo()
        com.FileName = "powershell.exe"
        com.Arguments = "Get-Disk"
        com.CreateNoWindow = True
        com.UseShellExecute = False
        com.RedirectStandardOutput = True

        Dim p As Process = Process.Start(com)
        p.WaitForExit()

        Dim output As String = p.StandardOutput.ReadToEnd()

        Dim outputArray As String() = output.Split(Environment.NewLine)
        For Each out As String In outputArray
            ListBox1.Items.Add(out)
        Next
    End Sub
End Class