'''''SIGMA'''''
'By: Ryan Brazeal
'Date: April 2015
'GNU GPL V2 License
'www.rgbi.ca
'www.jrpi.ca

Public Class GetPoint
    Friend selectedPointIndex As Integer

    Private Sub GetPoint_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        selectedPointIndex = -9999
        ComboBox1.Items.Clear()
        For i As Integer = 0 To mainForm.GPSLocalPts.Length - 1
            ComboBox1.Items.Add("Pt #: " & mainForm.PointList.Rows(i).Cells(0).Value.ToString & " Desc: " & mainForm.PointList.Rows(i).Cells(1).Value.ToString) 'mainForm.GPSLocalPts(i).desc)
        Next
        If ComboBox1.Items.Count > 0 Then
            ComboBox1.SelectedIndex = 0
        End If
    End Sub

    Private Sub okButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles okButton.Click
        selectedPointIndex = ComboBox1.SelectedIndex
        Me.Close()
    End Sub

    Private Sub cancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cancelButton.Click
        selectedPointIndex = -9999
        Me.Close()
    End Sub
End Class