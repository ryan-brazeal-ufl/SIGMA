<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CalcSatXYZForm
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.gpsWS = New System.Windows.Forms.TextBox()
        Me.gpsW = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.gpsS = New System.Windows.Forms.NumericUpDown()
        Me.gpsM = New System.Windows.Forms.NumericUpDown()
        Me.gpsH = New System.Windows.Forms.NumericUpDown()
        Me.calendarDateTimePicker = New System.Windows.Forms.DateTimePicker()
        Me.gpsTimeRadioButton = New System.Windows.Forms.RadioButton()
        Me.calendarRadioButton = New System.Windows.Forms.RadioButton()
        Me.SatandEphLabel = New System.Windows.Forms.Label()
        Me.optionsGroupBox = New System.Windows.Forms.GroupBox()
        Me.nominalDTCheckBox = New System.Windows.Forms.CheckBox()
        Me.c1TextBox = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TransmitTimeRadioButton = New System.Windows.Forms.RadioButton()
        Me.ReceptionTimeRadioButton = New System.Windows.Forms.RadioButton()
        Me.solveXYZButton = New System.Windows.Forms.Button()
        Me.GroupBox = New System.Windows.Forms.GroupBox()
        Me.SatelliteClockCheckBox = New System.Windows.Forms.CheckBox()
        Me.earthRotationCheckBox = New System.Windows.Forms.CheckBox()
        Me.outputParametersCheckBox = New System.Windows.Forms.CheckBox()
        Me.GroupBox1.SuspendLayout()
        CType(Me.gpsS, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gpsM, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gpsH, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.optionsGroupBox.SuspendLayout()
        Me.GroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.gpsWS)
        Me.GroupBox1.Controls.Add(Me.gpsW)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.gpsS)
        Me.GroupBox1.Controls.Add(Me.gpsM)
        Me.GroupBox1.Controls.Add(Me.gpsH)
        Me.GroupBox1.Controls.Add(Me.calendarDateTimePicker)
        Me.GroupBox1.Controls.Add(Me.gpsTimeRadioButton)
        Me.GroupBox1.Controls.Add(Me.calendarRadioButton)
        Me.GroupBox1.Location = New System.Drawing.Point(5, 25)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(510, 72)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Reception Time"
        '
        'gpsWS
        '
        Me.gpsWS.Location = New System.Drawing.Point(326, 43)
        Me.gpsWS.Name = "gpsWS"
        Me.gpsWS.Size = New System.Drawing.Size(82, 20)
        Me.gpsWS.TabIndex = 14
        '
        'gpsW
        '
        Me.gpsW.Location = New System.Drawing.Point(204, 43)
        Me.gpsW.Name = "gpsW"
        Me.gpsW.Size = New System.Drawing.Size(71, 20)
        Me.gpsW.TabIndex = 13
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(410, 47)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(49, 13)
        Me.Label5.TabIndex = 12
        Me.Label5.Text = "Seconds"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(277, 47)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(41, 13)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "Weeks"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(492, 23)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(14, 13)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "S"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(441, 23)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(16, 13)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "M"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(391, 23)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(15, 13)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "H"
        '
        'gpsS
        '
        Me.gpsS.Location = New System.Drawing.Point(457, 19)
        Me.gpsS.Maximum = New Decimal(New Integer() {59, 0, 0, 0})
        Me.gpsS.Name = "gpsS"
        Me.gpsS.Size = New System.Drawing.Size(35, 20)
        Me.gpsS.TabIndex = 5
        '
        'gpsM
        '
        Me.gpsM.Location = New System.Drawing.Point(406, 19)
        Me.gpsM.Maximum = New Decimal(New Integer() {59, 0, 0, 0})
        Me.gpsM.Name = "gpsM"
        Me.gpsM.Size = New System.Drawing.Size(35, 20)
        Me.gpsM.TabIndex = 4
        '
        'gpsH
        '
        Me.gpsH.Location = New System.Drawing.Point(356, 19)
        Me.gpsH.Maximum = New Decimal(New Integer() {23, 0, 0, 0})
        Me.gpsH.Name = "gpsH"
        Me.gpsH.Size = New System.Drawing.Size(35, 20)
        Me.gpsH.TabIndex = 3
        '
        'calendarDateTimePicker
        '
        Me.calendarDateTimePicker.CustomFormat = "M/dd/yyyy"
        Me.calendarDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.calendarDateTimePicker.Location = New System.Drawing.Point(255, 19)
        Me.calendarDateTimePicker.Name = "calendarDateTimePicker"
        Me.calendarDateTimePicker.Size = New System.Drawing.Size(95, 20)
        Me.calendarDateTimePicker.TabIndex = 2
        Me.calendarDateTimePicker.Value = New Date(2008, 1, 1, 0, 0, 0, 0)
        '
        'gpsTimeRadioButton
        '
        Me.gpsTimeRadioButton.AutoSize = True
        Me.gpsTimeRadioButton.Location = New System.Drawing.Point(6, 44)
        Me.gpsTimeRadioButton.Name = "gpsTimeRadioButton"
        Me.gpsTimeRadioButton.Size = New System.Drawing.Size(192, 17)
        Me.gpsTimeRadioButton.TabIndex = 1
        Me.gpsTimeRadioButton.TabStop = True
        Me.gpsTimeRadioButton.Text = "Use GPS Week and GPS Seconds"
        Me.gpsTimeRadioButton.UseVisualStyleBackColor = True
        '
        'calendarRadioButton
        '
        Me.calendarRadioButton.AutoSize = True
        Me.calendarRadioButton.Location = New System.Drawing.Point(7, 19)
        Me.calendarRadioButton.Name = "calendarRadioButton"
        Me.calendarRadioButton.Size = New System.Drawing.Size(242, 17)
        Me.calendarRadioButton.TabIndex = 0
        Me.calendarRadioButton.TabStop = True
        Me.calendarRadioButton.Text = "Use Calendar and GPS Hour, Minute, Second"
        Me.calendarRadioButton.UseVisualStyleBackColor = True
        '
        'SatandEphLabel
        '
        Me.SatandEphLabel.AutoSize = True
        Me.SatandEphLabel.Location = New System.Drawing.Point(8, 6)
        Me.SatandEphLabel.Name = "SatandEphLabel"
        Me.SatandEphLabel.Size = New System.Drawing.Size(39, 13)
        Me.SatandEphLabel.TabIndex = 1
        Me.SatandEphLabel.Text = "Label6"
        '
        'optionsGroupBox
        '
        Me.optionsGroupBox.Controls.Add(Me.nominalDTCheckBox)
        Me.optionsGroupBox.Controls.Add(Me.c1TextBox)
        Me.optionsGroupBox.Controls.Add(Me.Label6)
        Me.optionsGroupBox.Controls.Add(Me.TransmitTimeRadioButton)
        Me.optionsGroupBox.Controls.Add(Me.ReceptionTimeRadioButton)
        Me.optionsGroupBox.Location = New System.Drawing.Point(5, 98)
        Me.optionsGroupBox.Name = "optionsGroupBox"
        Me.optionsGroupBox.Size = New System.Drawing.Size(510, 63)
        Me.optionsGroupBox.TabIndex = 2
        Me.optionsGroupBox.TabStop = False
        Me.optionsGroupBox.Text = "Calculation Time"
        '
        'nominalDTCheckBox
        '
        Me.nominalDTCheckBox.AutoSize = True
        Me.nominalDTCheckBox.Location = New System.Drawing.Point(240, 40)
        Me.nominalDTCheckBox.Name = "nominalDTCheckBox"
        Me.nominalDTCheckBox.Size = New System.Drawing.Size(189, 17)
        Me.nominalDTCheckBox.TabIndex = 4
        Me.nominalDTCheckBox.Text = "Use nominal Transit Time (0.074 s)"
        Me.nominalDTCheckBox.UseVisualStyleBackColor = True
        '
        'c1TextBox
        '
        Me.c1TextBox.Location = New System.Drawing.Point(374, 17)
        Me.c1TextBox.Name = "c1TextBox"
        Me.c1TextBox.Size = New System.Drawing.Size(129, 20)
        Me.c1TextBox.TabIndex = 3
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(236, 21)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(139, 13)
        Me.Label6.TabIndex = 2
        Me.Label6.Text = "Observed Pseudorange (m):"
        '
        'TransmitTimeRadioButton
        '
        Me.TransmitTimeRadioButton.AutoSize = True
        Me.TransmitTimeRadioButton.Location = New System.Drawing.Point(126, 19)
        Me.TransmitTimeRadioButton.Name = "TransmitTimeRadioButton"
        Me.TransmitTimeRadioButton.Size = New System.Drawing.Size(104, 17)
        Me.TransmitTimeRadioButton.TabIndex = 1
        Me.TransmitTimeRadioButton.TabStop = True
        Me.TransmitTimeRadioButton.Text = "At Transmit Time"
        Me.TransmitTimeRadioButton.UseVisualStyleBackColor = True
        '
        'ReceptionTimeRadioButton
        '
        Me.ReceptionTimeRadioButton.AutoSize = True
        Me.ReceptionTimeRadioButton.Checked = True
        Me.ReceptionTimeRadioButton.Location = New System.Drawing.Point(7, 19)
        Me.ReceptionTimeRadioButton.Name = "ReceptionTimeRadioButton"
        Me.ReceptionTimeRadioButton.Size = New System.Drawing.Size(113, 17)
        Me.ReceptionTimeRadioButton.TabIndex = 0
        Me.ReceptionTimeRadioButton.TabStop = True
        Me.ReceptionTimeRadioButton.Text = "At Reception Time"
        Me.ReceptionTimeRadioButton.UseVisualStyleBackColor = True
        '
        'solveXYZButton
        '
        Me.solveXYZButton.Location = New System.Drawing.Point(220, 167)
        Me.solveXYZButton.Name = "solveXYZButton"
        Me.solveXYZButton.Size = New System.Drawing.Size(143, 41)
        Me.solveXYZButton.TabIndex = 3
        Me.solveXYZButton.Text = "Solve for ECEF X,Y,Z Satellite Coordinates"
        Me.solveXYZButton.UseVisualStyleBackColor = True
        '
        'GroupBox
        '
        Me.GroupBox.Controls.Add(Me.SatelliteClockCheckBox)
        Me.GroupBox.Controls.Add(Me.earthRotationCheckBox)
        Me.GroupBox.Location = New System.Drawing.Point(5, 162)
        Me.GroupBox.Name = "GroupBox"
        Me.GroupBox.Size = New System.Drawing.Size(209, 46)
        Me.GroupBox.TabIndex = 4
        Me.GroupBox.TabStop = False
        Me.GroupBox.Text = "Corrections to Apply"
        '
        'SatelliteClockCheckBox
        '
        Me.SatelliteClockCheckBox.AutoSize = True
        Me.SatelliteClockCheckBox.Location = New System.Drawing.Point(111, 20)
        Me.SatelliteClockCheckBox.Name = "SatelliteClockCheckBox"
        Me.SatelliteClockCheckBox.Size = New System.Drawing.Size(93, 17)
        Me.SatelliteClockCheckBox.TabIndex = 1
        Me.SatelliteClockCheckBox.Text = "Satellite Clock"
        Me.SatelliteClockCheckBox.UseVisualStyleBackColor = True
        '
        'earthRotationCheckBox
        '
        Me.earthRotationCheckBox.AutoSize = True
        Me.earthRotationCheckBox.Enabled = False
        Me.earthRotationCheckBox.Location = New System.Drawing.Point(8, 20)
        Me.earthRotationCheckBox.Name = "earthRotationCheckBox"
        Me.earthRotationCheckBox.Size = New System.Drawing.Size(97, 17)
        Me.earthRotationCheckBox.TabIndex = 0
        Me.earthRotationCheckBox.Text = "Earth Rotation "
        Me.earthRotationCheckBox.UseVisualStyleBackColor = True
        '
        'outputParametersCheckBox
        '
        Me.outputParametersCheckBox.AutoSize = True
        Me.outputParametersCheckBox.Location = New System.Drawing.Point(366, 179)
        Me.outputParametersCheckBox.Name = "outputParametersCheckBox"
        Me.outputParametersCheckBox.Size = New System.Drawing.Size(152, 17)
        Me.outputParametersCheckBox.TabIndex = 5
        Me.outputParametersCheckBox.Text = "Display Solved Parameters"
        Me.outputParametersCheckBox.UseVisualStyleBackColor = True
        '
        'CalcSatXYZForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(520, 214)
        Me.ControlBox = False
        Me.Controls.Add(Me.outputParametersCheckBox)
        Me.Controls.Add(Me.GroupBox)
        Me.Controls.Add(Me.solveXYZButton)
        Me.Controls.Add(Me.optionsGroupBox)
        Me.Controls.Add(Me.SatandEphLabel)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.Name = "CalcSatXYZForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Calculate Satellite ECEF X,Y,Z Position"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.gpsS, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gpsM, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gpsH, System.ComponentModel.ISupportInitialize).EndInit()
        Me.optionsGroupBox.ResumeLayout(False)
        Me.optionsGroupBox.PerformLayout()
        Me.GroupBox.ResumeLayout(False)
        Me.GroupBox.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents gpsTimeRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents calendarRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents calendarDateTimePicker As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents gpsS As System.Windows.Forms.NumericUpDown
    Friend WithEvents gpsM As System.Windows.Forms.NumericUpDown
    Friend WithEvents gpsH As System.Windows.Forms.NumericUpDown
    Friend WithEvents gpsW As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents gpsWS As System.Windows.Forms.TextBox
    Friend WithEvents SatandEphLabel As System.Windows.Forms.Label
    Friend WithEvents optionsGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents TransmitTimeRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents ReceptionTimeRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents c1TextBox As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents solveXYZButton As System.Windows.Forms.Button
    Friend WithEvents GroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents SatelliteClockCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents earthRotationCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents nominalDTCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents outputParametersCheckBox As System.Windows.Forms.CheckBox
End Class
