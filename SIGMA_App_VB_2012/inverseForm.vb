'''''SIGMA'''''
'By: Ryan Brazeal
'Date: April 2015
'GNU GPL V2 License
'www.rgbi.ca
'www.jrpi.ca

Public Class inverseForm

    Private Sub closeButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles closeButton.Click
        Me.Close()
    End Sub

    Private Sub inverseForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        fromComboBox.Items.Clear()
        toComboBox.Items.Clear()
        geoAziLabel.Text = "Geodetic Azimuth = "
        ellDistLabel.Text = "Ellipsoidal Distance = "
        gridAziLabel.Text = "Grid Azimuth = "
        horDistLabel.Text = "Horizontal Distance = "

        Try
            For i As Integer = 0 To mainForm.GPSLocalPts.Length - 1
                toComboBox.Items.Add("Pt #: " & mainForm.PointList.Rows(i).Cells(0).Value.ToString & " Desc: " & mainForm.PointList.Rows(i).Cells(1).Value.ToString)
                fromComboBox.Items.Add("Pt #: " & mainForm.PointList.Rows(i).Cells(0).Value.ToString & " Desc: " & mainForm.PointList.Rows(i).Cells(1).Value.ToString)
            Next
        Catch ex As Exception
            'empty catch, bad I know but it works
        End Try


        If toComboBox.Items.Count > 0 Then
            toComboBox.SelectedIndex = 0
        End If

        If fromComboBox.Items.Count > 0 Then
            fromComboBox.SelectedIndex = 0
        End If
    End Sub

    Private Sub calcButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles calcButton.Click
        If fromComboBox.SelectedIndex <> -1 And toComboBox.SelectedIndex <> -1 And fromComboBox.SelectedIndex <> toComboBox.SelectedIndex Then
            Dim fromLat As Decimal = mainForm.GPSLocalPts(fromComboBox.SelectedIndex).latitude
            Dim fromLong As Decimal = mainForm.GPSLocalPts(fromComboBox.SelectedIndex).longitude
            Dim toLat As Decimal = mainForm.GPSLocalPts(toComboBox.SelectedIndex).latitude
            Dim toLong As Decimal = mainForm.GPSLocalPts(toComboBox.SelectedIndex).longitude
            Dim fromNorthing As Decimal = mainForm.GPSLocalPts(fromComboBox.SelectedIndex).northing
            Dim fromEasting As Decimal = mainForm.GPSLocalPts(fromComboBox.SelectedIndex).easting
            Dim toNorthing As Decimal = mainForm.GPSLocalPts(toComboBox.SelectedIndex).northing
            Dim toEasting As Decimal = mainForm.GPSLocalPts(toComboBox.SelectedIndex).easting

            If mainForm.GPSLocalBeenCalcd = True Then
                Dim deltaN, deltaE As Decimal
                deltaN = toNorthing - fromNorthing
                deltaE = toEasting - fromEasting
                Dim gridAzimuth As Decimal = Math.Atan2(deltaE, deltaN)
                If gridAzimuth < 0 Then
                    gridAzimuth = 2D * Math.PI + gridAzimuth
                End If
                gridAzimuth = (gridAzimuth * 180D / Math.PI)

                gridAziLabel.Text = "Grid Azimuth = " & DEC2DMS(gridAzimuth, True)

                Dim hDist As Decimal = Math.Sqrt((deltaN) ^ 2 + (deltaE) ^ 2)
                horDistLabel.Text = "Horizontal Distance = " & (Decimal.Round(hDist, 3)).ToString("f3") & " m"
            Else
                MessageBox.Show("You must calculate the Local Coordinates" & vbNewLine & "before a Local Inverse can be computed", "LOCAL COORDINATES NOT CALC'D", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            'geodetic inverse calc (Vincenty)
            fromLat *= Math.PI / 180D
            fromLong *= Math.PI / 180D
            toLat *= Math.PI / 180D
            toLong *= Math.PI / 180D

            Dim B1 As Decimal = Math.Atan((1 - mainForm.WGS84_f) * Math.Tan(fromLat))
            Dim B2 As Decimal = Math.Atan((1 - mainForm.WGS84_f) * Math.Tan(toLat))
            Dim Lo As Decimal = toLong - fromLong
            Dim L As Decimal = Lo
            Dim Ld As Decimal = L + 1

            Dim sinS, cosS, S, sinA, cos2A, cos2SM, C, D As Decimal
            Dim stopper As Boolean = False
            While stopper = False
                sinS = Math.Sqrt((Math.Cos(B2) * Math.Sin(L)) ^ 2 + (Math.Cos(B1) * Math.Sin(B2) - Math.Sin(B1) * Math.Cos(B2) * Math.Cos(L)) ^ 2)
                cosS = Math.Sin(B1) * Math.Sin(B2) + Math.Cos(B1) * Math.Cos(B2) * Math.Cos(L)
                S = Math.Atan2(sinS, cosS)
                sinA = (Math.Cos(B1) * Math.Cos(B2) * Math.Sin(L)) / Math.Sin(S)
                cos2A = 1 - sinA ^ 2
                cos2SM = cosS - ((2 * Math.Sin(B1) * Math.Sin(B2)) / cos2A)
                C = (mainForm.WGS84_f / 16) * cos2A * (4 + mainForm.WGS84_f * (4 - 3 * cos2A))
                D = 2 * cos2SM ^ 2 - 1
                Ld = Lo + (1 - C) * mainForm.WGS84_f * sinA * (S + C * sinS * (cos2SM + C * cosS * D))
                If Math.Abs(Ld - L) < 0.0000000001 Then
                    stopper = True
                Else
                    L = Ld
                End If
            End While

            Dim b As Decimal = mainForm.WGS84_a * (1 - mainForm.WGS84_f)
            Dim u2 As Decimal = cos2A * ((mainForm.WGS84_a ^ 2 - b ^ 2) / b ^ 2)
            Dim AA As Decimal = 1 + u2 / 16384 * (4096 + u2 * (u2 * (320 - 175 * u2) - 768))
            Dim BB As Decimal = u2 / 1024 * (256 + u2 * (u2 * (74 - 47 * u2) - 128))
            Dim EE As Decimal = 4 * sinS ^ 2 - 3
            Dim FF As Decimal = 4 * cos2SM ^ 2 - 3
            Dim dS As Decimal = BB * sinS * (cos2SM + (BB / 4) * (cosS * D - (BB / 6) * cos2SM * EE * FF))
            Dim ellDist As Decimal = b * AA * (S - dS)
            Dim geoAzi As Decimal = Math.Atan2(Math.Cos(B2) * Math.Sin(Ld), Math.Cos(B1) * Math.Sin(B2) - Math.Sin(B1) * Math.Cos(B2) * Math.Cos(Ld))

            geoAzi *= 180D / Math.PI
            geoAziLabel.Text = "Geodetic Azimuth = " & DEC2DMS(geoAzi, False)

            ellDistLabel.Text = "Ellipsoidal Distance = " & (Decimal.Round(ellDist, 3)).ToString("f3") & " m"

        Else
            MessageBox.Show("You must specify a 'from' point and a 'to' point" & vbNewLine & "and they must be different points", "INCORRECT INPUT", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Function DEC2DMS(ByVal arg1 As Decimal, ByVal wholeSeconds As Boolean) As String
        Dim var1, var2, var3, var4, var5, var6, arg2 As Decimal

        If arg1 < 0 Then
            arg2 = Math.Abs(arg1)
        Else
            arg2 = arg1
        End If

        var1 = Decimal.Truncate(arg2)
        var2 = arg2 - var1
        var3 = var2 * 60D
        var4 = Decimal.Truncate(var3)
        var5 = var3 - var4
        var6 = var5 * 60D
        var6 = Decimal.Round(var6, 5)
        'var6 *= 100000D

        DEC2DMS = geoidBYNfileReader.checkfor60(var1, var4, var6, wholeSeconds)

    End Function

End Class