'''''SIGMA'''''
'By: Ryan Brazeal
'Date: April 2015
'GNU GPL V2 License
'www.rgbi.ca
'www.jrpi.ca

Public Class addPoint

    Friend Lat2Add As Decimal
    Friend Long2Add As Decimal
    Friend Height2Add As Decimal
    Friend GeodeticTest As Boolean
    Friend PtNum2Add As String
    Friend Desc2Add As String

    Private Sub addButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles addButton.Click
        Dim dlatd, dlatm, dlats, dlongd, dlongm, dlongs, dheight As Decimal
        Dim slatd As String = LatTextBox.Text.Substring(0, 2)
        Dim slatm As String = LatTextBox.Text.Substring(3, 2)
        Dim slats As String = LatTextBox.Text.Substring(6, 8)
        Dim slongd As String = LongTextBox.Text.Substring(0, 3)
        Dim slongm As String = LongTextBox.Text.Substring(4, 2)
        Dim slongs As String = LongTextBox.Text.Substring(7, 8)
        Dim GeodeticCurv(2) As Decimal
        GeodeticTest = True

        PtNum2Add = ptNumTextBox.Text
        Desc2Add = DescTextBox.Text

        If ptNumTextBox.Text = String.Empty Then
            GeodeticTest = False
        End If

        Try
            dlatd = Decimal.Parse(slatd)
        Catch ex As Exception
            dlatd = 0D
            GeodeticTest = False
        End Try

        Try
            dlatm = Decimal.Parse(slatm)
        Catch ex As Exception
            dlatm = 0D
            GeodeticTest = False
        End Try

        Try
            dlats = Decimal.Parse(slats)
        Catch ex As Exception
            dlats = 0D
            GeodeticTest = False
        End Try

        Try
            dlongd = Decimal.Parse(slongd)
        Catch ex As Exception
            dlongd = 0D
            GeodeticTest = False
        End Try

        Try
            dlongm = Decimal.Parse(slongm)
        Catch ex As Exception
            dlongm = 0D
            GeodeticTest = False
        End Try

        Try
            dlongs = Decimal.Parse(slongs)
        Catch ex As Exception
            dlongs = 0D
            GeodeticTest = False
        End Try

        Try
            dheight = Decimal.Parse(HeightTextBox.Text)
        Catch ex As Exception
            dheight = 0D
            GeodeticTest = False
        End Try

        GeodeticCurv(0) = dlatd + dlatm / 60D + dlats / 3600D
        GeodeticCurv(1) = -1 * (dlongd + dlongm / 60D + dlongs / 3600D)
        GeodeticCurv(2) = dheight

        If CheckBox1.Checked Then
            GeodeticCurv(0) *= -1
        End If

        If CheckBox2.Checked Then
            GeodeticCurv(1) *= -1
        End If

        If GeodeticTest = True Then
            Lat2Add = GeodeticCurv(0)
            Long2Add = GeodeticCurv(1)
            Height2Add = GeodeticCurv(2)
            Me.Close()
        Else
            MessageBox.Show("There is a problem with one or more of the geodetic coordinates you entered. Please review your entry and try to add the point again.", "COORDINATE ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub cancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cancelButton.Click
        Me.Close()
    End Sub

    Private Sub addPoint_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        GeodeticTest = False
    End Sub

    Private Sub addPoint_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LatTextBox.Mask = "90" & Chr(176) & "90" & "'" & "90.99999" & ControlChars.Quote
        LongTextBox.Mask = "990" & Chr(176) & "90" & "'" & "90.99999" & ControlChars.Quote
    End Sub

End Class