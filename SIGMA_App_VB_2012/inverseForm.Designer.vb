<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class inverseForm
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
        Me.closeButton = New System.Windows.Forms.Button
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.calcButton = New System.Windows.Forms.Button
        Me.toComboBox = New System.Windows.Forms.ComboBox
        Me.fromComboBox = New System.Windows.Forms.ComboBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.ellDistLabel = New System.Windows.Forms.Label
        Me.geoAziLabel = New System.Windows.Forms.Label
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.horDistLabel = New System.Windows.Forms.Label
        Me.gridAziLabel = New System.Windows.Forms.Label
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'closeButton
        '
        Me.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.closeButton.Location = New System.Drawing.Point(105, 214)
        Me.closeButton.Name = "closeButton"
        Me.closeButton.Size = New System.Drawing.Size(75, 23)
        Me.closeButton.TabIndex = 3
        Me.closeButton.Text = "Close"
        Me.closeButton.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.calcButton)
        Me.GroupBox1.Controls.Add(Me.toComboBox)
        Me.GroupBox1.Controls.Add(Me.fromComboBox)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(6, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(274, 74)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Points in List"
        '
        'calcButton
        '
        Me.calcButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.calcButton.Location = New System.Drawing.Point(203, 16)
        Me.calcButton.Name = "calcButton"
        Me.calcButton.Size = New System.Drawing.Size(65, 48)
        Me.calcButton.TabIndex = 2
        Me.calcButton.Text = "Calculate"
        Me.calcButton.UseVisualStyleBackColor = True
        '
        'toComboBox
        '
        Me.toComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.toComboBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.toComboBox.FormattingEnabled = True
        Me.toComboBox.Location = New System.Drawing.Point(45, 43)
        Me.toComboBox.Name = "toComboBox"
        Me.toComboBox.Size = New System.Drawing.Size(152, 21)
        Me.toComboBox.TabIndex = 1
        '
        'fromComboBox
        '
        Me.fromComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.fromComboBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.fromComboBox.FormattingEnabled = True
        Me.fromComboBox.Location = New System.Drawing.Point(45, 16)
        Me.fromComboBox.Name = "fromComboBox"
        Me.fromComboBox.Size = New System.Drawing.Size(152, 21)
        Me.fromComboBox.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(16, 46)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(23, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "To:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(6, 19)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(33, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "From:"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.ellDistLabel)
        Me.GroupBox3.Controls.Add(Me.geoAziLabel)
        Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.Location = New System.Drawing.Point(6, 83)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(274, 60)
        Me.GroupBox3.TabIndex = 1
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Geodetic Coordinates Inverse Results"
        '
        'ellDistLabel
        '
        Me.ellDistLabel.AutoSize = True
        Me.ellDistLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ellDistLabel.Location = New System.Drawing.Point(6, 38)
        Me.ellDistLabel.Name = "ellDistLabel"
        Me.ellDistLabel.Size = New System.Drawing.Size(110, 13)
        Me.ellDistLabel.TabIndex = 1
        Me.ellDistLabel.Text = "Ellipsoidal Distance = "
        '
        'geoAziLabel
        '
        Me.geoAziLabel.AutoSize = True
        Me.geoAziLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.geoAziLabel.Location = New System.Drawing.Point(6, 16)
        Me.geoAziLabel.Name = "geoAziLabel"
        Me.geoAziLabel.Size = New System.Drawing.Size(102, 13)
        Me.geoAziLabel.TabIndex = 0
        Me.geoAziLabel.Text = "Geodetic Azimuth = "
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.horDistLabel)
        Me.GroupBox2.Controls.Add(Me.gridAziLabel)
        Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox2.Location = New System.Drawing.Point(6, 149)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(274, 60)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Local Coordinates Inverse Results"
        '
        'horDistLabel
        '
        Me.horDistLabel.AutoSize = True
        Me.horDistLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.horDistLabel.Location = New System.Drawing.Point(6, 38)
        Me.horDistLabel.Name = "horDistLabel"
        Me.horDistLabel.Size = New System.Drawing.Size(111, 13)
        Me.horDistLabel.TabIndex = 1
        Me.horDistLabel.Text = "Horizontal Distance = "
        '
        'gridAziLabel
        '
        Me.gridAziLabel.AutoSize = True
        Me.gridAziLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gridAziLabel.Location = New System.Drawing.Point(6, 16)
        Me.gridAziLabel.Name = "gridAziLabel"
        Me.gridAziLabel.Size = New System.Drawing.Size(78, 13)
        Me.gridAziLabel.TabIndex = 0
        Me.gridAziLabel.Text = "Grid Azimuth = "
        '
        'inverseForm
        '
        Me.AcceptButton = Me.calcButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.closeButton
        Me.ClientSize = New System.Drawing.Size(284, 240)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.closeButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "inverseForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Inverse Between Points"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents closeButton As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents fromComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents toComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents calcButton As System.Windows.Forms.Button
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents ellDistLabel As System.Windows.Forms.Label
    Friend WithEvents geoAziLabel As System.Windows.Forms.Label
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents horDistLabel As System.Windows.Forms.Label
    Friend WithEvents gridAziLabel As System.Windows.Forms.Label
End Class
