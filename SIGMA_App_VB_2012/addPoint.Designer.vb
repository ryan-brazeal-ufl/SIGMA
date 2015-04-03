<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class addPoint
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
        Me.cancelButton = New System.Windows.Forms.Button
        Me.addButton = New System.Windows.Forms.Button
        Me.GroupBox5 = New System.Windows.Forms.GroupBox
        Me.CheckBox2 = New System.Windows.Forms.CheckBox
        Me.CheckBox1 = New System.Windows.Forms.CheckBox
        Me.LongTextBox = New System.Windows.Forms.MaskedTextBox
        Me.LatTextBox = New System.Windows.Forms.MaskedTextBox
        Me.HeightTextBox = New System.Windows.Forms.TextBox
        Me.Label39 = New System.Windows.Forms.Label
        Me.Label40 = New System.Windows.Forms.Label
        Me.Label41 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.DescTextBox = New System.Windows.Forms.TextBox
        Me.ptNumTextBox = New System.Windows.Forms.TextBox
        Me.GroupBox5.SuspendLayout()
        Me.SuspendLayout()
        '
        'cancelButton
        '
        Me.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cancelButton.Location = New System.Drawing.Point(59, 148)
        Me.cancelButton.Name = "cancelButton"
        Me.cancelButton.Size = New System.Drawing.Size(75, 23)
        Me.cancelButton.TabIndex = 2
        Me.cancelButton.Text = "Cancel"
        Me.cancelButton.UseVisualStyleBackColor = True
        '
        'addButton
        '
        Me.addButton.Location = New System.Drawing.Point(140, 148)
        Me.addButton.Name = "addButton"
        Me.addButton.Size = New System.Drawing.Size(75, 23)
        Me.addButton.TabIndex = 1
        Me.addButton.Text = "ADD"
        Me.addButton.UseVisualStyleBackColor = True
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.ptNumTextBox)
        Me.GroupBox5.Controls.Add(Me.DescTextBox)
        Me.GroupBox5.Controls.Add(Me.Label2)
        Me.GroupBox5.Controls.Add(Me.Label1)
        Me.GroupBox5.Controls.Add(Me.CheckBox2)
        Me.GroupBox5.Controls.Add(Me.CheckBox1)
        Me.GroupBox5.Controls.Add(Me.LongTextBox)
        Me.GroupBox5.Controls.Add(Me.LatTextBox)
        Me.GroupBox5.Controls.Add(Me.HeightTextBox)
        Me.GroupBox5.Controls.Add(Me.Label39)
        Me.GroupBox5.Controls.Add(Me.Label40)
        Me.GroupBox5.Controls.Add(Me.Label41)
        Me.GroupBox5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox5.Location = New System.Drawing.Point(5, 3)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(210, 141)
        Me.GroupBox5.TabIndex = 0
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Geodetic Point Info (WGS84) "
        '
        'CheckBox2
        '
        Me.CheckBox2.AutoSize = True
        Me.CheckBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CheckBox2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.CheckBox2.Location = New System.Drawing.Point(175, 93)
        Me.CheckBox2.Name = "CheckBox2"
        Me.CheckBox2.Size = New System.Drawing.Size(33, 17)
        Me.CheckBox2.TabIndex = 5
        Me.CheckBox2.Text = "E"
        Me.CheckBox2.UseVisualStyleBackColor = True
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CheckBox1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.CheckBox1.Location = New System.Drawing.Point(175, 70)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(33, 17)
        Me.CheckBox1.TabIndex = 3
        Me.CheckBox1.Text = "S"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'LongTextBox
        '
        Me.LongTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LongTextBox.ForeColor = System.Drawing.SystemColors.ControlText
        Me.LongTextBox.Location = New System.Drawing.Point(68, 91)
        Me.LongTextBox.Name = "LongTextBox"
        Me.LongTextBox.Size = New System.Drawing.Size(100, 20)
        Me.LongTextBox.TabIndex = 4
        '
        'LatTextBox
        '
        Me.LatTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LatTextBox.ForeColor = System.Drawing.SystemColors.ControlText
        Me.LatTextBox.Location = New System.Drawing.Point(68, 67)
        Me.LatTextBox.Name = "LatTextBox"
        Me.LatTextBox.Size = New System.Drawing.Size(100, 20)
        Me.LatTextBox.TabIndex = 2
        '
        'HeightTextBox
        '
        Me.HeightTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HeightTextBox.ForeColor = System.Drawing.SystemColors.ControlText
        Me.HeightTextBox.Location = New System.Drawing.Point(68, 115)
        Me.HeightTextBox.Name = "HeightTextBox"
        Me.HeightTextBox.Size = New System.Drawing.Size(61, 20)
        Me.HeightTextBox.TabIndex = 6
        '
        'Label39
        '
        Me.Label39.AutoSize = True
        Me.Label39.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label39.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label39.Location = New System.Drawing.Point(3, 118)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(58, 13)
        Me.Label39.TabIndex = 2
        Me.Label39.Text = "Height (m):"
        '
        'Label40
        '
        Me.Label40.AutoSize = True
        Me.Label40.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label40.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label40.Location = New System.Drawing.Point(3, 94)
        Me.Label40.Name = "Label40"
        Me.Label40.Size = New System.Drawing.Size(54, 13)
        Me.Label40.TabIndex = 1
        Me.Label40.Text = "Long (W):"
        '
        'Label41
        '
        Me.Label41.AutoSize = True
        Me.Label41.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label41.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label41.Location = New System.Drawing.Point(3, 70)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(42, 13)
        Me.Label41.TabIndex = 0
        Me.Label41.Text = "Lat (N):"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label1.Location = New System.Drawing.Point(3, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(30, 13)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "Pt #:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label2.Location = New System.Drawing.Point(3, 46)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(63, 13)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Description:"
        '
        'DescTextBox
        '
        Me.DescTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DescTextBox.ForeColor = System.Drawing.SystemColors.ControlText
        Me.DescTextBox.Location = New System.Drawing.Point(68, 43)
        Me.DescTextBox.Name = "DescTextBox"
        Me.DescTextBox.Size = New System.Drawing.Size(134, 20)
        Me.DescTextBox.TabIndex = 1
        '
        'ptNumTextBox
        '
        Me.ptNumTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ptNumTextBox.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ptNumTextBox.Location = New System.Drawing.Point(68, 19)
        Me.ptNumTextBox.Name = "ptNumTextBox"
        Me.ptNumTextBox.Size = New System.Drawing.Size(61, 20)
        Me.ptNumTextBox.TabIndex = 0
        '
        'addPoint
        '
        Me.AcceptButton = Me.addButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(219, 175)
        Me.Controls.Add(Me.GroupBox5)
        Me.Controls.Add(Me.addButton)
        Me.Controls.Add(Me.cancelButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.Name = "addPoint"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Add Geodetic Point"
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents cancelButton As System.Windows.Forms.Button
    Friend WithEvents addButton As System.Windows.Forms.Button
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents CheckBox2 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents LongTextBox As System.Windows.Forms.MaskedTextBox
    Friend WithEvents LatTextBox As System.Windows.Forms.MaskedTextBox
    Friend WithEvents HeightTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Label39 As System.Windows.Forms.Label
    Friend WithEvents Label40 As System.Windows.Forms.Label
    Friend WithEvents Label41 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ptNumTextBox As System.Windows.Forms.TextBox
    Friend WithEvents DescTextBox As System.Windows.Forms.TextBox
End Class
