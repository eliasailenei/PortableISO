<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DiskSelect
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(41, 131)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(252, 13)
        Me.Label5.TabIndex = 23
        Me.Label5.Text = "If you don't see anything, you have to wait 1 minute."
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(40, 20)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(474, 20)
        Me.Label1.TabIndex = 22
        Me.Label1.Text = "Choose a drive for the Windows version and then click OK."
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(304, 381)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(218, 50)
        Me.Button1.TabIndex = 21
        Me.Button1.Text = "---->"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(208, 328)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(457, 13)
        Me.Label4.TabIndex = 20
        Me.Label4.Text = "Enter disk number e.g. 0 ( it is critical that you ONLY put the correct drive num" &
    "ber (0 is default)! )"
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(353, 344)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(111, 20)
        Me.TextBox1.TabIndex = 19
        '
        'ListBox1
        '
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.Location = New System.Drawing.Point(25, 156)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(751, 134)
        Me.ListBox1.TabIndex = 18
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(41, 99)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(423, 13)
        Me.Label3.TabIndex = 17
        Me.Label3.Text = "We will also need to use 10GB of your disk space download and extract nessesary f" &
    "iles.."
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(41, 65)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(646, 18)
        Me.Label2.TabIndex = 16
        Me.Label2.Text = "Before you begin, make sure there is no data left on the drive as the drive will " &
    "permanently deleted!"
        '
        'DiskSelect
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.ControlBox = False
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.ListBox1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Name = "DiskSelect"
        Me.Text = "Form2"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label5 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents ListBox1 As ListBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
End Class
