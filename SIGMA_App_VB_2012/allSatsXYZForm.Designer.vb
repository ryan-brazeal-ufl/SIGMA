<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class allSatsXYZForm
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.allSatsXYZDataGridView = New System.Windows.Forms.DataGridView()
        Me.Elev = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Azimuth = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.satZ = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.satY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.satX = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PRN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.allSatsXYZDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'allSatsXYZDataGridView
        '
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.allSatsXYZDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.allSatsXYZDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.allSatsXYZDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.PRN, Me.satX, Me.satY, Me.satZ, Me.Azimuth, Me.Elev})
        Me.allSatsXYZDataGridView.Location = New System.Drawing.Point(12, 12)
        Me.allSatsXYZDataGridView.Name = "allSatsXYZDataGridView"
        Me.allSatsXYZDataGridView.Size = New System.Drawing.Size(683, 352)
        Me.allSatsXYZDataGridView.TabIndex = 0
        '
        'Elev
        '
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Elev.DefaultCellStyle = DataGridViewCellStyle7
        Me.Elev.HeaderText = "Elev (deg)"
        Me.Elev.Name = "Elev"
        Me.Elev.ReadOnly = True
        '
        'Azimuth
        '
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Azimuth.DefaultCellStyle = DataGridViewCellStyle6
        Me.Azimuth.HeaderText = "Azimuth (deg)"
        Me.Azimuth.Name = "Azimuth"
        Me.Azimuth.ReadOnly = True
        '
        'satZ
        '
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.satZ.DefaultCellStyle = DataGridViewCellStyle5
        Me.satZ.HeaderText = "Z (m)"
        Me.satZ.Name = "satZ"
        Me.satZ.ReadOnly = True
        Me.satZ.Width = 125
        '
        'satY
        '
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.satY.DefaultCellStyle = DataGridViewCellStyle4
        Me.satY.HeaderText = "Y (m)"
        Me.satY.Name = "satY"
        Me.satY.ReadOnly = True
        Me.satY.Width = 125
        '
        'satX
        '
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.satX.DefaultCellStyle = DataGridViewCellStyle3
        Me.satX.HeaderText = "X (m)"
        Me.satX.Name = "satX"
        Me.satX.ReadOnly = True
        Me.satX.Width = 125
        '
        'PRN
        '
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.PRN.DefaultCellStyle = DataGridViewCellStyle2
        Me.PRN.HeaderText = "PRN #"
        Me.PRN.Name = "PRN"
        Me.PRN.ReadOnly = True
        Me.PRN.Width = 65
        '
        'allSatsXYZForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(709, 379)
        Me.Controls.Add(Me.allSatsXYZDataGridView)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Name = "allSatsXYZForm"
        Me.Text = "Satellite XYZ Coordinates for Selected First Epoch"
        CType(Me.allSatsXYZDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents allSatsXYZDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents PRN As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents satX As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents satY As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents satZ As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Azimuth As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Elev As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
