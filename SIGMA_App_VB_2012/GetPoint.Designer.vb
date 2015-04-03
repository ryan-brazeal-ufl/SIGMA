<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GetPoint
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.cancelButton = New System.Windows.Forms.Button
        Me.okButton = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'ComboBox1
        '
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Location = New System.Drawing.Point(6, 20)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(217, 21)
        Me.ComboBox1.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 4)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Available Points:"
        '
        'cancelButton
        '
        Me.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cancelButton.Location = New System.Drawing.Point(73, 47)
        Me.cancelButton.Name = "cancelButton"
        Me.cancelButton.Size = New System.Drawing.Size(75, 23)
        Me.cancelButton.TabIndex = 2
        Me.cancelButton.Text = "Cancel"
        Me.cancelButton.UseVisualStyleBackColor = True
        '
        'okButton
        '
        Me.okButton.Location = New System.Drawing.Point(148, 47)
        Me.okButton.Name = "okButton"
        Me.okButton.Size = New System.Drawing.Size(75, 23)
        Me.okButton.TabIndex = 1
        Me.okButton.Text = "OK"
        Me.okButton.UseVisualStyleBackColor = True
        '
        'GetPoint
        '
        Me.AcceptButton = Me.okButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.cancelButton = Me.cancelButton
        Me.ClientSize = New System.Drawing.Size(228, 75)
        Me.Controls.Add(Me.okButton)
        Me.Controls.Add(Me.cancelButton)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ComboBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.Name = "GetPoint"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Select Geodetic Point"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cancelButton As System.Windows.Forms.Button
    Friend WithEvents okButton As System.Windows.Forms.Button
End Class
