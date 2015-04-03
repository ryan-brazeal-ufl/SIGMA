<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class geoidBYNfileReader
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.datumLabel = New System.Windows.Forms.Label()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.ewSpacingLabel = New System.Windows.Forms.Label()
        Me.nsSpacingLabel = New System.Windows.Forms.Label()
        Me.wLimitLabel = New System.Windows.Forms.Label()
        Me.eLimitLabel = New System.Windows.Forms.Label()
        Me.sLimitLabel = New System.Windows.Forms.Label()
        Me.nLimitLabel = New System.Windows.Forms.Label()
        Me.nameLabel = New System.Windows.Forms.Label()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.exportButton = New System.Windows.Forms.Button()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(4, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(179, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Select a Canadian Geoid Model File:"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(182, 3)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(100, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "Select .byn file..."
        Me.Button1.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.datumLabel)
        Me.GroupBox1.Controls.Add(Me.ProgressBar1)
        Me.GroupBox1.Controls.Add(Me.ewSpacingLabel)
        Me.GroupBox1.Controls.Add(Me.nsSpacingLabel)
        Me.GroupBox1.Controls.Add(Me.wLimitLabel)
        Me.GroupBox1.Controls.Add(Me.eLimitLabel)
        Me.GroupBox1.Controls.Add(Me.sLimitLabel)
        Me.GroupBox1.Controls.Add(Me.nLimitLabel)
        Me.GroupBox1.Controls.Add(Me.nameLabel)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(7, 32)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(275, 190)
        Me.GroupBox1.TabIndex = 2
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Geoid File Details"
        '
        'datumLabel
        '
        Me.datumLabel.AutoSize = True
        Me.datumLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.datumLabel.Location = New System.Drawing.Point(7, 164)
        Me.datumLabel.Name = "datumLabel"
        Me.datumLabel.Size = New System.Drawing.Size(70, 13)
        Me.datumLabel.TabIndex = 8
        Me.datumLabel.Text = "Datum = N/A"
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(114, 0)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(155, 13)
        Me.ProgressBar1.Step = 100
        Me.ProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.ProgressBar1.TabIndex = 7
        Me.ProgressBar1.Visible = False
        '
        'ewSpacingLabel
        '
        Me.ewSpacingLabel.AutoSize = True
        Me.ewSpacingLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ewSpacingLabel.Location = New System.Drawing.Point(7, 142)
        Me.ewSpacingLabel.Name = "ewSpacingLabel"
        Me.ewSpacingLabel.Size = New System.Drawing.Size(154, 13)
        Me.ewSpacingLabel.TabIndex = 6
        Me.ewSpacingLabel.Text = "East/West Grid Spacing = N/A"
        '
        'nsSpacingLabel
        '
        Me.nsSpacingLabel.AutoSize = True
        Me.nsSpacingLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.nsSpacingLabel.Location = New System.Drawing.Point(7, 121)
        Me.nsSpacingLabel.Name = "nsSpacingLabel"
        Me.nsSpacingLabel.Size = New System.Drawing.Size(162, 13)
        Me.nsSpacingLabel.TabIndex = 5
        Me.nsSpacingLabel.Text = "North/South Grid Spacing = N/A"
        '
        'wLimitLabel
        '
        Me.wLimitLabel.AutoSize = True
        Me.wLimitLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.wLimitLabel.Location = New System.Drawing.Point(7, 100)
        Me.wLimitLabel.Name = "wLimitLabel"
        Me.wLimitLabel.Size = New System.Drawing.Size(88, 13)
        Me.wLimitLabel.TabIndex = 4
        Me.wLimitLabel.Text = "West Limit = N/A"
        '
        'eLimitLabel
        '
        Me.eLimitLabel.AutoSize = True
        Me.eLimitLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.eLimitLabel.Location = New System.Drawing.Point(7, 79)
        Me.eLimitLabel.Name = "eLimitLabel"
        Me.eLimitLabel.Size = New System.Drawing.Size(84, 13)
        Me.eLimitLabel.TabIndex = 3
        Me.eLimitLabel.Text = "East Limit = N/A"
        '
        'sLimitLabel
        '
        Me.sLimitLabel.AutoSize = True
        Me.sLimitLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.sLimitLabel.Location = New System.Drawing.Point(7, 58)
        Me.sLimitLabel.Name = "sLimitLabel"
        Me.sLimitLabel.Size = New System.Drawing.Size(91, 13)
        Me.sLimitLabel.TabIndex = 2
        Me.sLimitLabel.Text = "South Limit = N/A"
        '
        'nLimitLabel
        '
        Me.nLimitLabel.AutoSize = True
        Me.nLimitLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.nLimitLabel.Location = New System.Drawing.Point(7, 37)
        Me.nLimitLabel.Name = "nLimitLabel"
        Me.nLimitLabel.Size = New System.Drawing.Size(89, 13)
        Me.nLimitLabel.TabIndex = 1
        Me.nLimitLabel.Text = "North Limit = N/A"
        '
        'nameLabel
        '
        Me.nameLabel.AutoSize = True
        Me.nameLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.nameLabel.Location = New System.Drawing.Point(7, 16)
        Me.nameLabel.Name = "nameLabel"
        Me.nameLabel.Size = New System.Drawing.Size(235, 13)
        Me.nameLabel.TabIndex = 0
        Me.nameLabel.Text = "File Name = NO GEOID HAS BEEN SELECTED"
        '
        'Button2
        '
        Me.Button2.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button2.Location = New System.Drawing.Point(7, 228)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 3
        Me.Button2.Text = "Close"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.DefaultExt = "byn"
        Me.OpenFileDialog1.Filter = "Canadian Geoid Model | *byn"
        Me.OpenFileDialog1.Title = "Select a Canadian Geoid Model File"
        '
        'exportButton
        '
        Me.exportButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.exportButton.Enabled = False
        Me.exportButton.Location = New System.Drawing.Point(207, 228)
        Me.exportButton.Name = "exportButton"
        Me.exportButton.Size = New System.Drawing.Size(75, 23)
        Me.exportButton.TabIndex = 4
        Me.exportButton.Text = "Export..."
        Me.exportButton.UseVisualStyleBackColor = True
        '
        'SaveFileDialog1
        '
        Me.SaveFileDialog1.Filter = "Text File|*.txt"
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(108, 228)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(75, 23)
        Me.Button3.TabIndex = 5
        Me.Button3.Text = "Button3"
        Me.Button3.UseVisualStyleBackColor = True
        Me.Button3.Visible = False
        '
        'geoidBYNfileReader
        '
        Me.AcceptButton = Me.Button1
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Button2
        Me.ClientSize = New System.Drawing.Size(289, 254)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.exportButton)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MinimizeBox = False
        Me.Name = "geoidBYNfileReader"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Geoid Model Manager"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents nameLabel As System.Windows.Forms.Label
    Friend WithEvents ewSpacingLabel As System.Windows.Forms.Label
    Friend WithEvents nsSpacingLabel As System.Windows.Forms.Label
    Friend WithEvents wLimitLabel As System.Windows.Forms.Label
    Friend WithEvents eLimitLabel As System.Windows.Forms.Label
    Friend WithEvents sLimitLabel As System.Windows.Forms.Label
    Friend WithEvents nLimitLabel As System.Windows.Forms.Label
    Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents datumLabel As System.Windows.Forms.Label
    Friend WithEvents exportButton As System.Windows.Forms.Button
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents Button3 As System.Windows.Forms.Button
End Class
