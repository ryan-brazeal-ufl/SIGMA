'''''SIGMA'''''
'By: Ryan Brazeal
'Date: April 2015
'GNU GPL V2 License
'www.rgbi.ca
'www.jrpi.ca

Imports System.IO

Public Class geoidBYNfileReader
    Friend GeoidHasBeenRead As Boolean = False
    Dim undefinedData As Integer
    Dim Factor As Double
    Dim SouthBdy As Integer
    Dim NorthBdy As Integer
    Dim WestBdy As Integer
    Dim EastBdy As Integer
    Dim nsSpacing As Short
    Dim ewSpacing As Short
    Dim Data(,) As Integer


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim yesNoResult As DialogResult = OpenFileDialog1.ShowDialog

        If yesNoResult <> Windows.Forms.DialogResult.Cancel Then
            Try
                exportButton.Enabled = False
                Dim geoidReader As New FileStream(OpenFileDialog1.FileName, FileMode.Open, FileAccess.Read)

                Dim count As Integer = 80
                Dim Header(count - 1) As Byte

                Dim GlobalModel, SizeOfData, StdDev, Datum, Ellipsoid, BytesOrder, ScaleForBdys, DataType As Short
                Dim FactorStdDev As Double

                'start reading .byn geoid file header
                geoidReader.Read(Header, 0, count)
                SouthBdy = BitConverter.ToInt32(Header, 0)
                NorthBdy = BitConverter.ToInt32(Header, 4)
                WestBdy = BitConverter.ToInt32(Header, 8)
                EastBdy = BitConverter.ToInt32(Header, 12)
                nsSpacing = BitConverter.ToInt16(Header, 16)
                ewSpacing = BitConverter.ToInt16(Header, 18)
                GlobalModel = BitConverter.ToInt16(Header, 20)
                DataType = BitConverter.ToInt16(Header, 22)
                Factor = BitConverter.ToDouble(Header, 24)
                SizeOfData = BitConverter.ToInt16(Header, 32)
                StdDev = BitConverter.ToInt16(Header, 34)
                FactorStdDev = BitConverter.ToDouble(Header, 36)
                Datum = BitConverter.ToInt16(Header, 44)
                Ellipsoid = BitConverter.ToInt16(Header, 46)
                BytesOrder = BitConverter.ToInt16(Header, 48)
                ScaleForBdys = BitConverter.ToInt16(Header, 50)
                'finish reading .byn geoid file header

                Dim Ntest, Stest, Etest, Wtest As Boolean

                If NorthBdy < 0 Then
                    Ntest = True
                    NorthBdy *= -1D
                Else
                    Ntest = False
                End If

                If SouthBdy < 0 Then
                    Stest = True
                    SouthBdy *= -1D
                Else
                    Stest = False
                End If

                If EastBdy < 0 Then
                    Etest = True
                    EastBdy *= -1D
                Else
                    Etest = False
                End If

                If WestBdy < 0 Then
                    Wtest = True
                    WestBdy *= -1D
                Else
                    Wtest = False
                End If

                Dim NbdyD, NbdyM, NbdyS, SbdyD, SbdyM, SbdyS, WbdyD, WbdyM, WbdyS, EbdyD, EbdyM, EbdyS, NSspaceD, NSspaceM, NSspaceS, EWspaceD, EWspaceM, EWspaceS As Decimal

                NbdyD = Math.Truncate(Convert.ToDecimal(NorthBdy) / 3600D)
                NbdyM = Math.Truncate(((Convert.ToDecimal(NorthBdy) / 3600D) - NbdyD) * 60D)
                NbdyS = Decimal.Round((((((Convert.ToDecimal(NorthBdy) / 3600D) - NbdyD) * 60D) - NbdyM) * 60D), 0)

                SbdyD = Math.Truncate(Convert.ToDecimal(SouthBdy) / 3600D)
                SbdyM = Math.Truncate(((Convert.ToDecimal(SouthBdy) / 3600D) - SbdyD) * 60D)
                SbdyS = Decimal.Round((((((Convert.ToDecimal(SouthBdy) / 3600D) - SbdyD) * 60D) - SbdyM) * 60D), 0)

                EbdyD = Math.Truncate(Convert.ToDecimal(EastBdy) / 3600D)
                EbdyM = Math.Truncate(((Convert.ToDecimal(EastBdy) / 3600D) - EbdyD) * 60D)
                EbdyS = Decimal.Round((((((Convert.ToDecimal(EastBdy) / 3600D) - EbdyD) * 60D) - EbdyM) * 60D), 0)

                WbdyD = Math.Truncate(Convert.ToDecimal(WestBdy) / 3600D)
                WbdyM = Math.Truncate(((Convert.ToDecimal(WestBdy) / 3600D) - WbdyD) * 60D)
                WbdyS = Decimal.Round((((((Convert.ToDecimal(WestBdy) / 3600D) - WbdyD) * 60D) - WbdyM) * 60D), 0)

                NSspaceD = Math.Truncate(Convert.ToDecimal(nsSpacing) / 3600D)
                NSspaceM = Math.Truncate(((Convert.ToDecimal(nsSpacing) / 3600D) - NSspaceD) * 60D)
                NSspaceS = Decimal.Round((((((Convert.ToDecimal(nsSpacing) / 3600D) - NSspaceD) * 60D) - NSspaceM) * 60D), 0)

                EWspaceD = Math.Truncate(Convert.ToDecimal(ewSpacing) / 3600D)
                EWspaceM = Math.Truncate(((Convert.ToDecimal(ewSpacing) / 3600D) - EWspaceD) * 60D)
                EWspaceS = Decimal.Round((((((Convert.ToDecimal(ewSpacing) / 3600D) - EWspaceD) * 60D) - EWspaceM) * 60D), 0)

                Dim NbdyString As String = checkfor60(NbdyD, NbdyM, NbdyS, True)
                Dim SbdyString As String = checkfor60(SbdyD, SbdyM, SbdyS, True)
                Dim EbdyString As String = checkfor60(EbdyD, EbdyM, EbdyS, True)
                Dim WbdyString As String = checkfor60(WbdyD, WbdyM, WbdyS, True)
                Dim NSspacingString As String = checkfor60(NSspaceD, NSspaceM, NSspaceS, True)
                Dim EWspacingString As String = checkfor60(EWspaceD, EWspaceM, EWspaceS, True)

                If Ntest = True Then
                    NbdyString = NbdyString & " S"
                    NorthBdy *= -1D
                Else
                    NbdyString = NbdyString & " N"
                End If

                If Stest = True Then
                    SbdyString = SbdyString & " S"
                    SouthBdy *= -1D
                Else
                    SbdyString = SbdyString & " N"
                End If

                If Etest = True Then
                    EbdyString = EbdyString & " W"
                    EastBdy *= -1D
                Else
                    EbdyString = EbdyString & " E"
                End If

                If Wtest = True Then
                    WbdyString = WbdyString & " W"
                    WestBdy *= -1D
                Else
                    WbdyString = WbdyString & " E"
                End If

                'starting reading actual Geoid Heights from the file and storing to the 2D Data Integer array
                Dim rows As Integer = (NorthBdy - SouthBdy) / nsSpacing + 1
                Dim columns As Integer = (EastBdy - WestBdy) / ewSpacing + 1
                Dim stepValue As Integer = Convert.ToInt32(Convert.ToDecimal(rows) * Convert.ToDecimal(columns) / 10D)
                ProgressBar1.Maximum = (rows * columns)
                ProgressBar1.Step = stepValue

                If DataType = 1 Then
                    ProgressBar1.Value = ProgressBar1.Minimum
                    Data = Nothing
                    ProgressBar1.Visible = True
                    If SizeOfData = 2 Then
                        Data = ReDim2DInteger(rows - 1, columns - 1)
                        undefinedData = 32767

                        Dim indivData(1) As Byte
                        Dim i, j As Integer
                        For i = 0 To rows - 1
                            For j = 0 To columns - 1
                                geoidReader.Read(indivData, 0, 2)
                                Data(i, j) = Convert.ToInt32(BitConverter.ToInt16(indivData, 0))
                                If ((i * j) + j) Mod stepValue = 0 Then
                                    ProgressBar1.PerformStep()
                                End If
                            Next
                        Next
                    Else    'SizeOfData = 4
                        Data = ReDim2DInteger(rows - 1, columns - 1)
                        undefinedData = 9999 * Factor

                        Dim indivData(3) As Byte
                        Dim i, j As Integer
                        For i = 0 To rows - 1
                            For j = 0 To columns - 1
                                geoidReader.Read(indivData, 0, 4)
                                Data(i, j) = BitConverter.ToInt32(indivData, 0)
                                If ((i * j) + j) Mod stepValue = 0 Then
                                    ProgressBar1.PerformStep()
                                End If
                            Next
                        Next
                    End If
                    GeoidHasBeenRead = True
                    ProgressBar1.Visible = False
                    Me.Refresh()
                    mainForm.applyGeoidCheckBox.Checked = True
                    mainForm.applyGeoidCheckBox.Enabled = True
                Else
                    GeoidHasBeenRead = False
                    mainForm.applyGeoidCheckBox.Checked = False
                    mainForm.applyGeoidCheckBox.Enabled = False
                    resetGeoidFileDetails()
                    MessageBox.Show("The geoid model selected does not contain Geoid Height information", "NO GEOID HEIGHTS IN FILE", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

                Dim fileNameString() As String
                Dim seperator() As Char = "\"

                fileNameString = OpenFileDialog1.FileName.Split(seperator)
                nameLabel.Text = "File Name = " & fileNameString(fileNameString.Length - 1)
                nLimitLabel.Text = "North Limit = " & NbdyString
                sLimitLabel.Text = "South Limit = " & SbdyString
                eLimitLabel.Text = "East Limit = " & EbdyString
                wLimitLabel.Text = "West Limit = " & WbdyString
                nsSpacingLabel.Text = "North/South Grid Spacing = " & NSspacingString
                ewSpacingLabel.Text = "East/West Grid Spacing = " & EWspacingString

                If Datum = 0 Then
                    datumLabel.Text = "Datum = ITRF"
                ElseIf Datum = 1 Then
                    datumLabel.Text = "Datum = NAD83(CSRS)"
                Else
                    datumLabel.Text = "Datum = N/A"
                End If

                geoidReader.Close()
                exportButton.Enabled = True

            Catch ex As Exception
                exportButton.Enabled = False
                GeoidHasBeenRead = False
                ProgressBar1.Visible = False
                mainForm.applyGeoidCheckBox.Checked = False
                mainForm.applyGeoidCheckBox.Enabled = False
                resetGeoidFileDetails()
                MessageBox.Show("The geoid model selected is not a valid .byn geoid file", "UNABLE TO READ GEOID DATA", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Function ReDim2DInteger(ByVal rowIndex As Integer, ByVal colIndex As Integer) As Integer(,)
        Dim temp(rowIndex, colIndex) As Integer
        Return temp
    End Function

    Private Sub resetGeoidFileDetails()
        nameLabel.Text = "File Name = NO GEOID HAS BEEN SELECTED"
        nLimitLabel.Text = "North Limit = N/A"
        sLimitLabel.Text = "South Limit = N/A"
        eLimitLabel.Text = "East Limit = N/A"
        wLimitLabel.Text = "West Limit = N/A"
        nsSpacingLabel.Text = "North/South Grid Spacing = N/A"
        ewSpacingLabel.Text = "East/West Grid Spacing = N/A"
    End Sub

    Friend Function checkfor60(ByVal deg As Decimal, ByVal min As Decimal, ByVal sec As Decimal, ByVal roundSeconds As Boolean) As String
        If sec = 60D Then
            sec = 0D
            min += 1D
        End If

        If min = 60D Then
            min = 0D
            deg += 1D
        End If

        If roundSeconds = True Then
            Return deg.ToString & Chr(176) & " " & min.ToString("00") & "' " & sec.ToString("00") & """"
        Else
            Return deg.ToString & Chr(176) & " " & min.ToString("00") & "' " & sec.ToString("00.00000") & """"
        End If

    End Function

    Friend Function getInterpolatedNfromGeoidModelData(ByVal Lat As Decimal, ByVal Lon As Decimal, ByVal bLat As Decimal, ByVal bLon As Decimal, ByVal scale As Decimal, ByVal projType As Integer) As Decimal
        Dim N As Decimal
        Dim latSecs As Decimal = Lat * 3600D
        Dim longSecs As Decimal = Lon * 3600D
        Dim closestRow, closestColumn As Integer
        Dim closestLat, closestLong As Integer
        Dim rows As Integer = (NorthBdy - SouthBdy) / nsSpacing + 1
        Dim columns As Integer = (EastBdy - WestBdy) / ewSpacing + 1

        If latSecs >= SouthBdy And latSecs <= NorthBdy And longSecs >= WestBdy And longSecs <= EastBdy Then
            Dim deltaLat As Decimal = NorthBdy - latSecs
            Dim deltaLong As Decimal = longSecs - WestBdy
            Dim exactRow As Decimal = deltaLat / Convert.ToDecimal(nsSpacing)
            Dim exactColumn As Decimal = deltaLong / Convert.ToDecimal(ewSpacing)
            Dim lowRow As Integer = Convert.ToInt32(Decimal.Truncate(exactRow))
            Dim highRow As Integer = lowRow + 1
            Dim lowColumn As Integer = Convert.ToInt32(Decimal.Truncate(exactColumn))
            Dim highColumn As Integer = lowColumn + 1

            If exactRow - Convert.ToDecimal(lowRow) <= Convert.ToDecimal(highRow) - exactRow Then
                closestRow = lowRow
            Else
                closestRow = highRow
            End If
            closestLat = NorthBdy - nsSpacing * closestRow

            If exactColumn - Convert.ToDecimal(lowColumn) <= Convert.ToDecimal(highColumn) - exactColumn Then
                closestColumn = lowColumn
            Else
                closestColumn = highColumn
            End If
            closestLong = WestBdy + ewSpacing * closestColumn

            Dim loopTest As Boolean = True
            Dim cornerCounter As Integer = 1

            'search loop to test all 4 closest points to find valid geoid model data
            Do While loopTest = True
                If Data(closestRow, closestColumn) <> undefinedData Then
                    loopTest = False
                    N = Convert.ToDecimal(Data(closestRow, closestColumn)) / Convert.ToDecimal(Factor)
                Else
                    If cornerCounter = 5 Then
                        loopTest = False
                        N = -9999D
                    ElseIf cornerCounter Mod 4 = 1 Then
                        closestRow = lowRow
                        closestColumn = lowColumn
                    ElseIf cornerCounter Mod 4 = 2 Then
                        closestRow = lowRow
                        closestColumn = highColumn
                    ElseIf cornerCounter Mod 4 = 3 Then
                        closestRow = highRow
                        closestColumn = highColumn
                    Else
                        closestRow = highRow
                        closestColumn = lowColumn
                    End If
                    cornerCounter += 1
                End If
            Loop
        Else
            N = -9999D
        End If

        Dim YesNo2Interpolate As Boolean = True

        'interpolate to exact position using the closest point and the eight points surrounding it
        If N <> -9999D And YesNo2Interpolate = True Then
            Dim surroundingCorners(8) As GeoidSample
            Dim startRow As Integer = closestRow - 1
            Dim startColumn As Integer = closestColumn - 1
            Dim startLat As Integer = closestLat + nsSpacing
            Dim startLong As Integer = closestLong - ewSpacing

            If closestRow = 0 Then
                startRow = 0
                startLat = NorthBdy
            End If
            If closestColumn = 0 Then
                startColumn = 0
                startLong = WestBdy
            End If
            If closestRow = rows Then
                startRow = rows - 2
                startLat = SouthBdy + 2 * nsSpacing
            End If
            If closestColumn = columns Then
                startColumn = columns - 2
                startLong = EastBdy - 2 * ewSpacing
            End If

            'For i As Integer = 0 To 2
            '    surroundingCorners(i).row = startRow
            '    surroundingCorners(i + 3).row = startRow + 1
            '    surroundingCorners(i + 6).row = startRow + 2
            '    surroundingCorners(i).Latitude = startLat - (startLat)
            '    surroundingCorners(i + 3).Latitude = startLat - (startLat - nsSpacing)
            '    surroundingCorners(i + 6).Latitude = startLat - (startLat - 2 * nsSpacing)
            '    surroundingCorners(i).column = startColumn + i
            '    surroundingCorners(i + 3).column = startColumn + i
            '    surroundingCorners(i + 6).column = startColumn + i
            '    surroundingCorners(i).Longitude = (startLong + i * ewSpacing) - startLong
            '    surroundingCorners(i + 3).Longitude = (startLong + i * ewSpacing) - startLong
            '    surroundingCorners(i + 6).Longitude = (startLong + i * ewSpacing) - startLong
            'Next

            For i As Integer = 0 To 2
                surroundingCorners(i).row = startRow
                surroundingCorners(i + 3).row = startRow + 1
                surroundingCorners(i + 6).row = startRow + 2
                surroundingCorners(i).Latitude = (startLat) / 3600D
                surroundingCorners(i + 3).Latitude = (startLat - nsSpacing) / 3600D
                surroundingCorners(i + 6).Latitude = (startLat - 2 * nsSpacing) / 3600D
                surroundingCorners(i).column = startColumn + i
                surroundingCorners(i + 3).column = startColumn + i
                surroundingCorners(i + 6).column = startColumn + i
                surroundingCorners(i).Longitude = (startLong + i * ewSpacing) / 3600D
                surroundingCorners(i + 3).Longitude = (startLong + i * ewSpacing) / 3600D
                surroundingCorners(i + 6).Longitude = (startLong + i * ewSpacing) / 3600D
            Next

            Dim LocalCoords(1) As Decimal
            For k As Integer = 0 To 8
                If Data(surroundingCorners(k).row, surroundingCorners(k).column) <> undefinedData Then
                    surroundingCorners(k).N = Convert.ToDecimal(Data(surroundingCorners(k).row, surroundingCorners(k).column)) / Convert.ToDecimal(Factor)
                Else
                    surroundingCorners(k).N = N     'copy closest sample point N value if undefined data is read
                End If
                If projType = 1I Then
                    LocalCoords = mainForm.calcLocalOrtho(surroundingCorners(k).Latitude, surroundingCorners(k).Longitude, 0D)
                ElseIf projType = 2I Then
                    LocalCoords = mainForm.calcLocalStereo(scale, bLat, bLon, surroundingCorners(k).Latitude, surroundingCorners(k).Longitude)
                ElseIf projType = 3I Then
                    LocalCoords = mainForm.calcLocalTransMer(scale, bLat, bLon, surroundingCorners(k).Latitude, surroundingCorners(k).Longitude)
                End If
                surroundingCorners(k).x = LocalCoords(1)
                surroundingCorners(k).y = LocalCoords(0)
            Next

            Try
                Dim Ns As New Matrix(9, 1)
                Dim A As New Matrix(9, 9)

                'divide by 1000 to reduce magnitude of x and y coordinates to make the Matrix inverse less risky to reach really large numbers
                For i As Integer = 0 To 8
                    Ns.data(i + 1, 1) = surroundingCorners(i).N
                    A.data(i + 1, 1) = 1
                    A.data(i + 1, 2) = surroundingCorners(i).x / 1000D
                    A.data(i + 1, 3) = surroundingCorners(i).y / 1000D
                    A.data(i + 1, 4) = (surroundingCorners(i).x / 1000D) ^ 2
                    A.data(i + 1, 5) = (surroundingCorners(i).y / 1000D) ^ 2
                    A.data(i + 1, 6) = (surroundingCorners(i).x / 1000D) * (surroundingCorners(i).y / 1000D)
                    A.data(i + 1, 7) = ((surroundingCorners(i).x / 1000D) ^ 2) * (surroundingCorners(i).y / 1000D)
                    A.data(i + 1, 8) = (surroundingCorners(i).x / 1000D) * ((surroundingCorners(i).y / 1000D) ^ 2)
                    A.data(i + 1, 9) = ((surroundingCorners(i).x / 1000D) ^ 2) * ((surroundingCorners(i).y / 1000D) ^ 2)
                Next

                Dim x As Matrix = A.Inverse * Ns

                If projType = 1I Then
                    LocalCoords = mainForm.calcLocalOrtho(Lat, Lon, 0D)
                ElseIf projType = 2I Then
                    LocalCoords = mainForm.calcLocalStereo(scale, bLat, bLon, Lat, Lon)
                ElseIf projType = 3I Then
                    LocalCoords = mainForm.calcLocalTransMer(scale, bLat, bLon, Lat, Lon)
                End If

                'point of interest X and Y projected coordinates
                Dim ptX As Decimal = LocalCoords(1)
                Dim ptY As Decimal = LocalCoords(0)

                Dim Au As New Matrix(1, 9)

                Au.data(1, 1) = 1
                Au.data(1, 2) = ptX / 1000D
                Au.data(1, 3) = ptY / 1000D
                Au.data(1, 4) = (ptX / 1000D) ^ 2
                Au.data(1, 5) = (ptY / 1000D) ^ 2
                Au.data(1, 6) = ptX / 1000D * ptY / 1000D
                Au.data(1, 7) = (ptX / 1000D) ^ 2 * ptY / 1000D
                Au.data(1, 8) = ptX / 1000D * (ptY / 1000D) ^ 2
                Au.data(1, 9) = (ptX / 1000D) ^ 2 * (ptY / 1000D) ^ 2

                Dim Nu As Matrix = Au * x

                N = Nu.toScalar

            Catch ex As Exception

            End Try
        End If
        Return N
    End Function

    Private Structure GeoidSample
        Dim row As Integer
        Dim column As Integer
        Dim N As Decimal
        Dim Latitude As Decimal
        Dim Longitude As Decimal
        Dim x As Decimal
        Dim y As Decimal
    End Structure

    Private Function Sec2Metres(ByVal value As Decimal) As Decimal
        Sec2Metres = value / 3600D * Math.PI / 180D * 6378
    End Function

    Private Sub exportButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exportButton.Click
        Dim yesNoResult As DialogResult = SaveFileDialog1.ShowDialog

        If yesNoResult <> Windows.Forms.DialogResult.Cancel Then
            Dim fileOutput As StreamWriter
            Try
                fileOutput = New StreamWriter(SaveFileDialog1.FileName)

                Dim min As Integer = 10000000

                For i As Integer = 0 To Data.GetLength(0) - 1
                    For j As Integer = 0 To Data.GetLength(1) - 1
                        If Data(i, j) < min Then
                            min = Data(i, j)
                        End If
                    Next
                Next

                fileOutput.WriteLine("Longitude,Latitude,Geoidal Undulation")
                Dim minCorrection As Integer = -1 * min
                For i As Integer = 0 To Data.GetLength(0) - 1 Step 1
                    For j As Integer = 0 To Data.GetLength(1) - 1 Step 1
                        If Data(i, j) <> undefinedData Then
                            Dim limit1 As Double = ((WestBdy + (j * ewSpacing)) / 3600.0R)
                            Dim limit2 As Double = ((NorthBdy - (i * nsSpacing)) / 3600.0R)
                            If limit2 >= 50.34 And limit2 <= 50.76 And limit1 >= -104.99 And limit1 <= -104.31 Then
                                fileOutput.WriteLine(((WestBdy + (j * ewSpacing)) / 3600.0R).ToString & "," & ((NorthBdy - (i * nsSpacing)) / 3600.0R).ToString & "," & (Data(i, j) / 1000.0R).ToString)
                                'uncomment this line for wide area geoid model export
                                'fileOutput.WriteLine(((WestBdy + (j * ewSpacing)) / 3600.0R).ToString & " " & ((NorthBdy - (i * nsSpacing)) / 3600.0R).ToString & " " & (Data(i, j) / 5000.0R).ToString & " " & (Data(i, j) + minCorrection).ToString)
                            End If
                        End If
                    Next
                Next
                MessageBox.Show("Export Complete", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information)
                fileOutput.Close()
            Catch ex As Exception
                MessageBox.Show("Problem exporting geoid model data to a text file, your exported dataset may be incomplete", "EXPORT ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Try
                    fileOutput.Close()
                Catch ex1 As Exception
                    MessageBox.Show("Error interpolating geoidal undulation", "GEOID INTERPOLATION ERROR")
                End Try
            End Try
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim overlayImport As New StreamReader("C:/file3.csv")
        Dim overlayExport As New StreamWriter("C:/lines3.txt")

        Dim line As String
        Dim parts() As String
        Dim sep(0) As Char
        sep(0) = ","
        Dim lat, longi As Decimal
        Dim N As Decimal
        While overlayImport.Peek <> -1
            line = overlayImport.ReadLine
            If line <> String.Empty Then
                parts = line.Split(sep)
                If IsNumeric(parts(0)) And IsNumeric(parts(1)) Then
                    longi = Decimal.Parse(parts(0))
                    lat = Decimal.Parse(parts(1))
                    If (longi) >= -111 And (longi) <= -100 And (lat) >= 48 And (lat) <= 61 Then
                        N = getInterpolatedNfromGeoidModelData(lat, longi, lat, longi, 1, 1)
                        If N <> -9999D Then
                            overlayExport.WriteLine(longi.ToString & " " & lat.ToString & " " & (N / 5.0R).ToString & " 0")
                        End If
                    End If
                End If
            End If
        End While
        MessageBox.Show("Overlay export complete")
        overlayImport.Close()
        overlayExport.Close()
    End Sub
End Class