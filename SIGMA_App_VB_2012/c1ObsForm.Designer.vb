<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class c1ObsForm
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
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.c1ObsDataGridView = New System.Windows.Forms.DataGridView()
        Me.PRN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Pseudorange = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.c1ObsDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'c1ObsDataGridView
        '
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.c1ObsDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.c1ObsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.c1ObsDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.PRN, Me.Pseudorange})
        Me.c1ObsDataGridView.Location = New System.Drawing.Point(12, 12)
        Me.c1ObsDataGridView.Name = "c1ObsDataGridView"
        Me.c1ObsDataGridView.ReadOnly = True
        Me.c1ObsDataGridView.Size = New System.Drawing.Size(233, 313)
        Me.c1ObsDataGridView.TabIndex = 0
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
        'Pseudorange
        '
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Pseudorange.DefaultCellStyle = DataGridViewCellStyle3
        Me.Pseudorange.HeaderText = "Pseudorange (m)"
        Me.Pseudorange.Name = "Pseudorange"
        Me.Pseudorange.ReadOnly = True
        Me.Pseudorange.Width = 125
        '
        'c1ObsForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(257, 337)
        Me.Controls.Add(Me.c1ObsDataGridView)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "c1ObsForm"
        Me.Text = "C/A Pseudorange Observations"
        CType(Me.c1ObsDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents c1ObsDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents PRN As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Pseudorange As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
