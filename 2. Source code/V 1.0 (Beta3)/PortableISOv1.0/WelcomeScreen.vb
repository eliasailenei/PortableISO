Public Class WelcomeScreen
    Private Sub WelcomeScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For i = 0 To 3
            ListBox1.Items.Add(ver(i))
        Next
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        For k = 0 To 3
            If ListBox1.SelectedItem = ver(k) Then
                versel = k
                TextBox1.Text = ver(k)
            End If
        Next

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If versel = 1 Then
            MessageBox.Show("You are about to install " & ver(versel) & ". By doing so, you agree to the terms and conditions... You can choose your keyboard layout later")

            DiskSelect.Show()
            Me.Close()
        Else
            MessageBox.Show("You are about to install " & ver(versel) & ". By doing so, you agree to the terms and conditions... You can choose your keyboard layout later")
            DiskSelect.Show()
            Me.Close()
        End If
    End Sub
End Class