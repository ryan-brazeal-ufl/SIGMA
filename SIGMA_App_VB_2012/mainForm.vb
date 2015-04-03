'''''SIGMA'''''
'By: Ryan Brazeal
'Date: April 2015
'GNU GPL V2 License
'www.rgbi.ca
'www.jrpi.ca

Imports System.IO

Public Class mainForm
    Dim SelectedNavFile As New RINEX_Nav()
    Friend SelectedObsFile As New RINEX_Obs()
    Dim TropWarning As Boolean = False
    Dim MaxEpochs As Integer = 1000  'max sample size of epochs to process 
    Dim MaxSkyPlotEpochs As Integer = 500   'max number of epochs to plot satellite positions (SkyPlot)
    Friend NavSuccess As Boolean
    Friend ObsSuccess As Boolean
    Dim SatsAdded As New Matrix(1, 5)
    Dim DrawSats As Boolean = False
    Friend BeenProcessed As Boolean = False
    Friend ProcessedSolution As New Matrix(3, 1)
    Friend ProcessedSolutionXYZ As New Matrix(3, 1)
    Public Const EARTH_U As Decimal = 398600500000000D
    Public Const EARTH_ROT As Decimal = 0.000072921151467D
    Public Const GPS_PI As Decimal = 3.1415926535898D
    Public Const SPEED_LIGHT As Decimal = 299792458D
    Public Const RELATIVISTIC As Decimal = -0.0000000004442807633
    Public Const WGS84_a As Decimal = 6378137.0  'WGS 84 Ellipsoid Parameters
    Public Const WGS84_f As Decimal = 1D / 298.257223563
    Public Const WGS84_e2 As Decimal = 2D * WGS84_f - WGS84_f * WGS84_f
    Public Const FREQ_L1 As Decimal = 1575420000
    Public Const FREQ_L2 As Decimal = 1227600000
    Public Const LAMBDA_L1 As Decimal = (SPEED_LIGHT / FREQ_L1)
    Public Const LAMBDA_L2 As Decimal = (SPEED_LIGHT / FREQ_L2)
    Friend GPSLocalPts(0) As GPS_Local_Pt
    Friend GPSLocalBeenCalcd As Boolean
    Dim GeodeticCurv(2), GeodeticXYZ(2), LocalNEel(2) As Decimal
    Dim R1, R3 As New Matrix(3, 3)
    Dim WithEvents serialPort2 As IO.Ports.SerialPort
    Dim GPS_Connected As Boolean
    Dim timeOutErrorFlag As Boolean
    Dim ErrorFlag As Boolean
    Dim LastCommand(1) As Byte
    Dim timeOfLastWrite As Long
    Dim ProductID As UInt16
    Dim SoftwareVersion As Double
    Dim DeviceInfo As String
    Dim AlmanacDownload As Boolean
    Dim downloadedCounter As Integer
    Dim AlmanacPoints As UInt16
    Dim ProtocolArray(0) As ProtocolDataType
    Dim almanacCounter As Integer
    Dim Alman_Data_Type As Integer
    Dim command(7) As Byte
    Dim WithEvents hyperlink As New LinkLabel()
    Friend PRNs2UseBoolean(32) As Boolean
    Friend UseUnhealthySatsBoolean As Boolean
    Friend localDetailsSet As localizationDetails
    Dim CommercialSoftwareSelection As Integer

    Structure ProtocolDataType
        Dim tag As String
        Dim Data As UInt16
    End Structure

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim myDialogResult As DialogResult = FolderBrowserDialog1.ShowDialog()

        If myDialogResult <> Windows.Forms.DialogResult.Cancel Then
            NavFilesList.Items.Clear()
            ObsFilesList.Items.Clear()
            NavDetails.Enabled = False
            ObsDetails.Enabled = False
            RINEXDetails.Enabled = False
            TextBox1.Text = FolderBrowserDialog1.SelectedPath
            Dim selDirectoryInfo As New DirectoryInfo(FolderBrowserDialog1.SelectedPath)

            Dim indFileInfo As FileInfo
            Dim string2Test As String
            Dim NavFiles As Integer = 0
            Dim ObsFiles As Integer = 0

            For Each indFileInfo In selDirectoryInfo.GetFiles
                string2Test = Path.GetExtension(indFileInfo.FullName).ToUpper
                If string2Test.Length = 4 Then
                    If IsNumeric(string2Test.Substring(1, 1)) And IsNumeric(string2Test.Substring(2, 1)) And string2Test.Substring(3, 1) = "N" Then
                        NavFilesList.Items.Add(Path.GetFileName(indFileInfo.FullName).ToUpper)
                        NavFiles += 1
                    End If
                    If IsNumeric(string2Test.Substring(1, 1)) And IsNumeric(string2Test.Substring(2, 1)) And string2Test.Substring(3, 1) = "O" Then
                        ObsFilesList.Items.Add(Path.GetFileName(indFileInfo.FullName).ToUpper)
                        ObsFiles += 1
                    End If
                End If
            Next
            NavFilesFound.Text = NavFiles.ToString & " Files Found"
            ObsFilesFound.Text = ObsFiles.ToString & " Files Found"
            SelectedNavFile.Reset()
            SelectedObsFile.Reset()
            SatellitesPRNComboBox.Items.Clear()
            SatelliteEphComboBox.Items.Clear()
            ObsEpochsComboBox.Items.Clear()
            EndObsEpochsComboBox.Items.Clear()
            NavDetailsGroupBox.Enabled = False
            ObsDetailsGroupBox.Enabled = False
        End If
    End Sub

    Private Sub NavFilesList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NavFilesList.SelectedIndexChanged
        If NavFilesList.SelectedItem <> String.Empty Then
            NavDetails.Enabled = True
            Dim fileString As String = NavFilesList.Items(NavFilesList.SelectedIndex)
            fileString = (fileString.Substring(0, fileString.Length - 1) & "o").ToUpper
            Dim fileIndex As Integer = ObsFilesList.Items.IndexOf(fileString)
            If fileIndex <> -1 Then
                ObsFilesList.SelectedIndex = fileIndex
            End If
        Else
            NavDetails.Enabled = False
        End If
        NavDetailsGroupBox.Enabled = False
        BeenProcessed = False
        If NavFilesList.SelectedIndex <> -1 And ObsFilesList.SelectedIndex <> -1 Then
            RINEXDetails.Enabled = True
        End If
    End Sub

    Private Sub ObsFilesList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ObsFilesList.SelectedIndexChanged
        If ObsFilesList.SelectedItem <> String.Empty Then
            ObsDetails.Enabled = True
        Else
            ObsDetails.Enabled = False
        End If
        ObsDetailsGroupBox.Enabled = False
        BeenProcessed = False
        If NavFilesList.SelectedIndex <> -1 And ObsFilesList.SelectedIndex <> -1 Then
            RINEXDetails.Enabled = True
        End If
    End Sub


    Private Sub NavDetails_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NavDetails.Click
        NavSuccess = True
        SelectedNavFile.Reset()

        StatusLabel.Text = "Please wait, reading data from navigational file..."
        Me.Refresh()

        ProgressBar1.Visible = True
        SelectedNavFile.ReadFile(TextBox1.Text & "\" & NavFilesList.SelectedItem, ProgressBar1)
        If ProgressBar1.Value <> ProgressBar1.Maximum Then
            Dim indexUpdater As Integer
            For indexUpdater = ProgressBar1.Value To ProgressBar1.Maximum
                ProgressBar1.Value = indexUpdater
                ProgressBar1.Update()
            Next
        End If
        ProgressBar1.Visible = False
        ProgressBar1.Value = ProgressBar1.Minimum

        StatusLabel.Text = String.Empty
        Me.Refresh()

        If NavSuccess = True Then
            NavDetailsGroupBox.Enabled = True
            SatellitesPRNComboBox.Items.Clear()
            SatelliteEphComboBox.Items.Clear()
            NumEphLabel.Text = String.Empty
            ShowEphButton.Enabled = False

            Dim i As Integer = 0
            Dim testBool As Boolean = False
            For i = 0 To SelectedNavFile.EphemerisV.GetLength(1) - 1
                If SelectedNavFile.EphemerisV(0, i).PRN <> -9999I Then
                    SatellitesPRNComboBox.Items.Add(SelectedNavFile.EphemerisV(0, i).PRN)
                    testBool = True
                End If
            Next

            If testBool = True Then
                SatellitesPRNComboBox.SelectedIndex = 0
                ShowEphButton.Enabled = True
                rinexVersionLabel.Text = "RINEX Version: " & SelectedNavFile.Rinex_Version
                dateCreatedLabel.Text = "Date File Created: " & SelectedNavFile.Date_File_Created
                SatellitesGroupBox.Text = SelectedNavFile.Num_Sats_in_File & " Satellites Tracked"
            End If
            checkIonoModel()
        End If
    End Sub

    Private Sub ObsDetails_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ObsDetails.Click
        IonoFreeRadioButton.Checked = False
        IonoFreeRadioButton.Enabled = False

        ObsSuccess = True
        SelectedObsFile.Reset()

        StatusLabel.Text = "Please wait, reading data from observation file..."
        Me.Refresh()

        ProgressBar1.Visible = True
        SelectedObsFile.ReadFile(TextBox1.Text & "\" & ObsFilesList.SelectedItem, ProgressBar1)
        If ProgressBar1.Value <> ProgressBar1.Maximum Then
            Dim indexUpdater As Integer
            For indexUpdater = ProgressBar1.Value To ProgressBar1.Maximum
                ProgressBar1.Value = indexUpdater
                ProgressBar1.Update()
            Next
        End If
        ProgressBar1.Visible = False
        ProgressBar1.Value = ProgressBar1.Minimum
        ProgressBar1.Refresh()

        Dim preprocessResult As DialogResult = MessageBox.Show("Do you want to preprocess the observations in order to attempt to remove the receiver clock jumps (resets), if they are present?", "PREPROCESSING ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If preprocessResult = Windows.Forms.DialogResult.Yes Then
            StatusLabel.Text = "Preprocessing, please wait..."
            StatusLabel.Refresh()
            SelectedObsFile.preProcessObs("Single Point")
        End If

        StatusLabel.Text = String.Empty
        Me.Refresh()

        If ObsSuccess = True Then
            If SelectedObsFile.Sat_System.ToUpper = "G" Or SelectedObsFile.Sat_System.ToUpper = "M" Then
                ObsDetailsGroupBox.Enabled = True
                ObsEpochsComboBox.Items.Clear()
                EndObsEpochsComboBox.Items.Clear()
                NumSats.Text = String.Empty
                Label10.Text = "RINEX Version: " & SelectedObsFile.Rinex_Version
                Label9.Text = "Date File Created: " & SelectedObsFile.Date_File_Created
                Label12.Text = SelectedObsFile.NumEpochs
                Label14.Text = SelectedObsFile.Interval.ToString & " sec"

                Dim i As Integer
                For i = 0 To SelectedObsFile.NumEpochs - 1
                    ObsEpochsComboBox.Items.Add(SelectedObsFile.ObsData(0, 0, i) & ": " & Decimal.Round(SelectedObsFile.ObsData(0, 1, i), 2))
                    EndObsEpochsComboBox.Items.Add(SelectedObsFile.ObsData(0, 0, i) & ": " & Decimal.Round(SelectedObsFile.ObsData(0, 1, i), 2))
                Next
                ObsEpochsComboBox.SelectedIndex = 0
                checkIonoModel()

                If SelectedObsFile.Dual_Frequency Then
                    IonoFreeRadioButton.Enabled = True
                    If IonoRadioButton.Checked = False Then
                        IonoFreeRadioButton.Checked = True
                    End If
                    If IonoCheckBox.Checked = False Then
                        TGDCheckBox.Enabled = True
                    End If
                End If

            End If
        End If
    End Sub

    Private Sub RINEXDetails_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RINEXDetails.Click
        NavDetails_Click(sender, e)
        ObsDetails_Click(sender, e)
    End Sub

    Private Sub SatellitesPRNComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SatellitesPRNComboBox.SelectedIndexChanged
        EphLabel.Text = "Time of Ephemeris Data for PRN # " & SatellitesPRNComboBox.Items(SatellitesPRNComboBox.SelectedIndex)
        SatelliteEphComboBox.Items.Clear()

        Dim i, j As Integer
        Dim EphCount As Integer = 0

        For i = 0 To SelectedNavFile.EphemerisV.GetLength(1) - 1
            If SelectedNavFile.EphemerisV(0, i).PRN = SatellitesPRNComboBox.Items(SatellitesPRNComboBox.SelectedIndex) Then
                For j = 0 To SelectedNavFile.EphemerisV.GetLength(0) - 1
                    If SelectedNavFile.EphemerisV(j, i).Toe_Week <> -9999I Then
                        SatelliteEphComboBox.Items.Add("GPS Week " & Decimal.Round(SelectedNavFile.EphemerisV(j, i).Toe_Week) & ", Seconds " & Decimal.Round(SelectedNavFile.EphemerisV(j, i).Toe, 2))
                        EphCount += 1
                    End If
                Next
            End If
        Next

        If EphCount > 0 Then
            SatelliteEphComboBox.SelectedIndex = 0
            NumEphLabel.Text = EphCount.ToString
        End If

        If EphemerisForm.Created Then
            ShowEphButton_Click(sender, e)
        End If

        If CalcSatXYZForm.Created Then
            calcSatXYZButton_Click(sender, e)
        End If

    End Sub

    Private Sub SatelliteEphComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SatelliteEphComboBox.SelectedIndexChanged
        If EphemerisForm.Created Then
            ShowEphButton_Click(sender, e)
        End If

        If CalcSatXYZForm.Created Then
            calcSatXYZButton_Click(sender, e)
        End If
    End Sub

    Private Sub ObsEpochsComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ObsEpochsComboBox.SelectedIndexChanged
        Dim i As Integer
        Dim NumSatsInt As Integer = 0
        For i = 4 To SelectedObsFile.NumRows(ObsEpochsComboBox.SelectedIndex) - 1
            If SelectedObsFile.ObsData(i, 0, ObsEpochsComboBox.SelectedIndex) <> -9999D Then
                NumSatsInt += 1
            End If
        Next
        Dim currentSelection As String = EndObsEpochsComboBox.Text
        EndObsEpochsComboBox.Items.Clear()
        For i = ObsEpochsComboBox.SelectedIndex To ObsEpochsComboBox.Items.Count - 1
            EndObsEpochsComboBox.Items.Add(SelectedObsFile.ObsData(0, 0, i) & ": " & Decimal.Round(SelectedObsFile.ObsData(0, 1, i), 2))
        Next

        If currentSelection <> String.Empty Then
            EndObsEpochsComboBox.SelectedIndex = EndObsEpochsComboBox.Items.IndexOf(currentSelection)
            If EndObsEpochsComboBox.SelectedIndex < 0 Then
                EndObsEpochsComboBox.SelectedIndex = EndObsEpochsComboBox.Items.Count - 1
            End If
        Else
            EndObsEpochsComboBox.SelectedIndex = EndObsEpochsComboBox.Items.Count - 1
        End If

        NumSats.Text = NumSatsInt.ToString

        Dim month, day, year, hour, min, sec, leap_secs As Decimal
        Dim leapCorrected As String = "(GPS Time)"
        month = SelectedObsFile.ObsData(1, 0, ObsEpochsComboBox.SelectedIndex)
        day = SelectedObsFile.ObsData(1, 1, ObsEpochsComboBox.SelectedIndex)
        year = SelectedObsFile.ObsData(2, 0, ObsEpochsComboBox.SelectedIndex)
        hour = SelectedObsFile.ObsData(2, 1, ObsEpochsComboBox.SelectedIndex)
        min = SelectedObsFile.ObsData(3, 0, ObsEpochsComboBox.SelectedIndex)
        sec = SelectedObsFile.ObsData(3, 1, ObsEpochsComboBox.SelectedIndex)
        leap_secs = 0

        If SelectedNavFile.Leap_Seconds <> -9999I Or SelectedObsFile.Leap_Seconds <> -9999I Then
            leapCorrected = "(UTC Time)"
            leap_secs = Convert.ToDecimal(SelectedNavFile.Leap_Seconds)
            If leap_secs = -9999D Then
                leap_secs = Convert.ToDecimal(SelectedObsFile.Leap_Seconds)
            End If
            sec -= leap_secs

            If sec < 0 Then
                sec += 60
                min -= 1
                If min < 0 Then
                    min += 60
                    hour -= 1
                    If hour < 0 Then
                        hour += 24
                        day -= 1
                        If day < 1 Then
                            If month = 1 Or month = 2 Or month = 4 Or month = 6 Or month = 8 Or month = 9 Or month = 11 Then
                                day += 31
                            ElseIf month = 3 Then
                                Dim leapyearBool As Boolean = False
                                If year Mod 4 = 0 Then
                                    leapyearBool = True
                                    If year Mod 100 = 0 Then
                                        leapyearBool = False
                                        If year Mod 400 = 0 Then
                                            leapyearBool = True
                                        End If
                                    End If
                                End If
                                If leapyearBool Then
                                    day += 29
                                Else
                                    day += 28
                                End If
                            Else
                                day += 30
                            End If
                            month -= 1
                            If month < 1 Then
                                month += 12
                                year -= 1
                            End If
                        End If
                    End If
                End If
            End If
        End If

        Label32.Text = month.ToString & "/" & day.ToString & "/" & year.ToString & "  " & hour.ToString("00") & ":" & min.ToString("00") & ":" & sec.ToString("00")
        Label34.Text = leapCorrected
    End Sub
   
    Private Sub ShowEphButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowEphButton.Click
        Dim senderComboBox As New ComboBox
        Try
            senderComboBox = CType(sender, ComboBox)
        Catch
        End Try

        Dim tester As Boolean = True

        If senderComboBox.Name.ToUpper = "SATELLITEEPHCOMBOBOX" Or senderComboBox.Name.ToUpper = "SATELLITESPRNCOMBOBOX" Then
            tester = False
        End If

        If EphemerisForm.Created And tester Then
            EphemerisForm.Close()
            showEphRadioButton.Checked = False
        Else
            EphemerisForm.Text = "Broadcast Ephemeris Data for PRN # " & SatellitesPRNComboBox.Items(SatellitesPRNComboBox.SelectedIndex).ToString
            EphemerisForm.Label1.Text = "Time of Ephemeris = " & SatelliteEphComboBox.Items(SatelliteEphComboBox.SelectedIndex).ToString

            Dim i, j As Integer
            Dim WeekString, SecondsString As String
            Dim WeekDecimal, SecondsDecimal As Decimal
            Dim PRNInt As Integer
            Dim CurrentEphRecord As New RINEX_Eph

            WeekString = SatelliteEphComboBox.Items(SatelliteEphComboBox.SelectedIndex).ToString
            WeekString = WeekString.Substring(9, WeekString.IndexOf(",") - 9)
            SecondsString = SatelliteEphComboBox.Items(SatelliteEphComboBox.SelectedIndex).ToString
            SecondsString = SecondsString.Substring(SecondsString.IndexOf(",") + 10)
            WeekDecimal = Convert.ToDecimal(WeekString)
            SecondsDecimal = Convert.ToDecimal(SecondsString)
            PRNInt = Convert.ToInt32(SatellitesPRNComboBox.Items(SatellitesPRNComboBox.SelectedIndex).ToString)

            For i = 0 To SelectedNavFile.EphemerisV.GetLength(0) - 1
                For j = 0 To SelectedNavFile.EphemerisV.GetLength(1) - 1
                    If SelectedNavFile.EphemerisV(i, j).PRN = PRNInt And SelectedNavFile.EphemerisV(i, j).Toe_Week = WeekDecimal And SelectedNavFile.EphemerisV(i, j).Toe = SecondsDecimal Then
                        CurrentEphRecord = SelectedNavFile.EphemerisV(i, j)
                    End If
                Next
            Next

            If CurrentEphRecord.PRN <> -9999I Then
                EphemerisForm.Label2.Text = "a: " & CurrentEphRecord.a & " m"
                EphemerisForm.Label3.Text = "e: " & CurrentEphRecord.e
                EphemerisForm.Label4.Text = "OMEGA: " & CurrentEphRecord.Big_Omega & " rads"
                EphemerisForm.Label5.Text = "omega: " & CurrentEphRecord.Little_Omega & " rads"
                EphemerisForm.Label6.Text = "i0: " & CurrentEphRecord.i0 & " rads"
                EphemerisForm.Label7.Text = "M0: " & CurrentEphRecord.M0 & " rads"
                EphemerisForm.Label8.Text = "Delta n: " & CurrentEphRecord.Delta_n & " rads/s"
                EphemerisForm.Label9.Text = "I_dot: " & CurrentEphRecord.i_DOT & " rads/s"
                EphemerisForm.Label10.Text = "OMEGA_dot: " & CurrentEphRecord.Omega_DOT & " rads/s"
                EphemerisForm.Label11.Text = "Cus: " & CurrentEphRecord.Cus & " rads"
                EphemerisForm.Label12.Text = "Cuc: " & CurrentEphRecord.Cuc & " rads"
                EphemerisForm.Label13.Text = "Crs: " & CurrentEphRecord.Crs & " m"
                EphemerisForm.Label14.Text = "Crc: " & CurrentEphRecord.Crc & " m"
                EphemerisForm.Label15.Text = "Cis: " & CurrentEphRecord.Cis & " rads"
                EphemerisForm.Label16.Text = "Cic: " & CurrentEphRecord.Cic & " rads"
                EphemerisForm.Label17.Text = "Total Group Delay: " & CurrentEphRecord.TGD & " s"

                Dim monthString As String
                Select Case CurrentEphRecord.Toc_Month.ToString
                    Case 1
                        monthString = "January"
                    Case 2
                        monthString = "February"
                    Case 3
                        monthString = "March"
                    Case 4
                        monthString = "April"
                    Case 5
                        monthString = "May"
                    Case 6
                        monthString = "June"
                    Case 7
                        monthString = "July"
                    Case 8
                        monthString = "August"
                    Case 9
                        monthString = "September"
                    Case 10
                        monthString = "October"
                    Case 11
                        monthString = "November"
                    Case 12
                        monthString = "December"
                End Select

                Dim yearString As String = CurrentEphRecord.Toc_Year
                If yearString.Length = 1 Then
                    yearString = "0" & yearString
                End If

                Dim minuteString As String = CurrentEphRecord.Toc_Minute
                If minuteString.Length = 1 Then
                    minuteString = "0" & minuteString
                End If

                Dim secondString As String = CurrentEphRecord.Toc_Second
                If secondString.Length = 3 Then
                    secondString = "0" & secondString
                End If

                EphemerisForm.Label18.Text = "Time of Clock: " & monthString & "." & CurrentEphRecord.Toc_Day & " " & yearString & _
                " " & CurrentEphRecord.Toc_Hour & ":" & minuteString & ":" & secondString
                EphemerisForm.Label21.Text = "Satellite Clock Bias (af0): " & CurrentEphRecord.Clock_Bias & " s"
                EphemerisForm.Label22.Text = "Satellite Clock Drift (af1): " & CurrentEphRecord.Clock_Drift & " s/s"
                EphemerisForm.Label23.Text = "Satellite Clock Drift Rate (af2): " & CurrentEphRecord.Clock_DriftRate & " s/s2"
                EphemerisForm.Label24.Text = "Issue of Ephemeris Data # " & Decimal.Round(CurrentEphRecord.IODE)
                EphemerisForm.Label25.Text = "Issue of Clock Data # " & Decimal.Round(CurrentEphRecord.IODC)

                Dim intervalString As String
                If CurrentEphRecord.Fit_Interval = 0 Then
                    intervalString = "4 hours"
                Else
                    intervalString = "> 4 hours"
                End If

                EphemerisForm.Label26.Text = "Fit Interval: " & intervalString
                EphemerisForm.Label27.Text = "Satellite Accuracy: " & Decimal.Round(CurrentEphRecord.SV_accuracy, 3) & " m"

                Dim healthString As String
                If CurrentEphRecord.SV_health = 0D Then
                    healthString = "Healthy"
                Else
                    healthString = "Unhealthy"
                End If

                EphemerisForm.Label28.Text = "Satellite Health: " & healthString
            End If
            showEphRadioButton.Checked = True
            EphemerisForm.Show()
        End If
        
    End Sub

    Private Sub calcSatXYZButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles calcSatXYZButton.Click
        Dim senderComboBox As New ComboBox
        Try
            senderComboBox = CType(sender, ComboBox)
        Catch
        End Try

        Dim tester As Boolean = True

        If senderComboBox.Name.ToUpper = "SATELLITEEPHCOMBOBOX" Or senderComboBox.Name.ToUpper = "SATELLITESPRNCOMBOBOX" Then
            tester = False
        End If

        If CalcSatXYZForm.Created And tester Then
            CalcSatXYZForm.Close()
            calcSatRadioButton.Checked = False
        Else
            Dim i, j As Integer
            Dim WeekString, SecondsString As String
            Dim WeekDecimal, SecondsDecimal As Decimal
            Dim PRNInt As Integer
            Dim foundBool As Boolean = False

            WeekString = SatelliteEphComboBox.Items(SatelliteEphComboBox.SelectedIndex).ToString
            WeekString = WeekString.Substring(9, WeekString.IndexOf(",") - 9)
            SecondsString = SatelliteEphComboBox.Items(SatelliteEphComboBox.SelectedIndex).ToString
            SecondsString = SecondsString.Substring(SecondsString.IndexOf(",") + 10)
            WeekDecimal = Convert.ToDecimal(WeekString)
            SecondsDecimal = Convert.ToDecimal(SecondsString)
            PRNInt = Convert.ToInt32(SatellitesPRNComboBox.Items(SatellitesPRNComboBox.SelectedIndex).ToString)

            For i = 0 To SelectedNavFile.EphemerisV.GetLength(0) - 1
                For j = 0 To SelectedNavFile.EphemerisV.GetLength(1) - 1
                    If SelectedNavFile.EphemerisV(i, j).PRN = PRNInt And SelectedNavFile.EphemerisV(i, j).Toe_Week = WeekDecimal And SelectedNavFile.EphemerisV(i, j).Toe = SecondsDecimal Then
                        CalcSatXYZForm.CurrentEphRecord = SelectedNavFile.EphemerisV(i, j)
                        foundBool = True
                    End If
                Next
            Next

            If foundBool Then
                CalcSatXYZForm.SatandEphLabel.Text = "PRN # " & SatellitesPRNComboBox.Items(SatellitesPRNComboBox.SelectedIndex).ToString & " - " & SatelliteEphComboBox.Items(SatelliteEphComboBox.SelectedIndex).ToString & " (Ephemeris Reference)"
                calcSatRadioButton.Checked = True
                CalcSatXYZForm.Show()
            Else
                MessageBox.Show("Problem finding ephemeris data for chosen Satellite")
            End If
        End If

    End Sub

    Private Sub ProcessSolution(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProcessEpochButton.Click, ProcessALLButton.Click
        If NavDetailsGroupBox.Enabled = True Then
            TropWarning = False
            Dim ProcessResults(6) As Decimal
            Dim ProcessStdDevs(6) As String
            Dim DOPS(3) As Decimal
            Dim Success As Boolean = True
            Dim startAt, stopAt As Integer
            Dim clockString As String = String.Empty
            Dim H2drms As Decimal = -9999
            Dim V2drms As Decimal = -9999
            Dim H2drms_String As String = "N/A"
            Dim V2drms_String As String = "N/A"

            Dim selectedButton As Button
            selectedButton = CType(sender, Button)

            If selectedButton.Name.ToUpper = "PROCESSEPOCHBUTTON" Then
                startAt = ObsEpochsComboBox.SelectedIndex
                stopAt = startAt
                CalcPositionFromALLEpochs(startAt, stopAt, Success, ProcessResults, ProcessStdDevs, DOPS, H2drms, V2drms)

                clockString = vbNewLine & "The Receiver Clock Offset(dT) for this epoch is: " & Decimal.Round(ProcessResults(6) / SPEED_LIGHT, 10) & " s"
            Else
                startAt = ObsEpochsComboBox.SelectedIndex
                stopAt = ObsEpochsComboBox.SelectedIndex + EndObsEpochsComboBox.SelectedIndex
                'CalcPositionFromALLEpochs(startAt, stopAt, Success, ProcessResults, ProcessStdDevs, DOPS, H2drms, V2drms)
                CalcPositionSequential(startAt, stopAt, Success, ProcessResults, ProcessStdDevs, DOPS, H2drms, V2drms, True)
                If Success Then
                    BeenProcessed = True
                    ProcessedSolutionXYZ.data(1, 1) = ProcessResults(0)
                    ProcessedSolutionXYZ.data(2, 1) = ProcessResults(1)
                    ProcessedSolutionXYZ.data(3, 1) = ProcessResults(2)
                End If
            End If

            If Success Then

                If H2drms <> -9999 And V2drms <> -9999 Then
                    H2drms_String = Decimal.Round(H2drms, 3).ToString & " m"
                    V2drms_String = Decimal.Round(V2drms, 3).ToString & " m"
                End If
                MessageBox.Show("The Receiver's ECEF Cartesian Coordinates are:" & ControlChars.NewLine & ControlChars.NewLine & _
                            "     X: " & Decimal.Round(ProcessResults(0), 3) & " m" & ControlChars.Tab & ControlChars.NewLine & _
                            "     Y: " & Decimal.Round(ProcessResults(1), 3) & " m" & ControlChars.Tab & ControlChars.NewLine & _
                            "     Z: " & Decimal.Round(ProcessResults(2), 3) & " m" & ControlChars.Tab & ControlChars.NewLine & ControlChars.NewLine & _
                            "The Receiver's Geodetic Coordinates are:" & ControlChars.NewLine & ControlChars.NewLine & _
                            "     Latitude: " & DecToDMS(ProcessResults(3), 1) & ControlChars.Tab & ControlChars.NewLine & _
                            "     Longitude: " & DecToDMS(ProcessResults(4), 2) & ControlChars.Tab & ControlChars.NewLine & _
                            "     Height: " & Decimal.Round(ProcessResults(5), 3) & " m" & ControlChars.Tab & ControlChars.NewLine & ControlChars.NewLine & _
                            "     Horizontal Accuracy (95%): " & H2drms_String & _
                            vbNewLine & "     Vertical Accuracy (95%): " & V2drms_String & vbNewLine & clockString, "WGS84 Receiver Coordinates", MessageBoxButtons.OK)
            End If
        Else
            MessageBox.Show("You must specify a Navigational RINEX file to use and click the get details first!", "Missing Nav Data", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub CalcPositionFromALLEpochs(ByVal Starting As Integer, ByVal Ending As Integer, ByRef Success As Boolean, ByRef Results() As Decimal, ByRef StdDevs() As String, ByRef DOPS() As Decimal, ByRef H2drms As Decimal, ByRef V2drms As Decimal, Optional ByVal SuppressWarnings As Boolean = False)

        Dim i, j, k, m, z As Integer
        Dim SatNum As Integer
        Dim PRN As Integer
        Dim ReceptionTime(1), EphemerisTime(1) As Decimal
        Dim TimeDiffFromReference As Decimal
        Dim Eph2Use As New RINEX_Eph
        Dim pseudorange, transmitDistance As Decimal
        Dim pseudorangeIndex As Integer = -9999I
        Dim P1Index As Integer = -9999I
        Dim P2Index As Integer = -9999I
        Dim SatPositions As New Matrix(1, 9)
        Dim ECEF_XYZ_SatPosition(0 To 3) As Decimal
        Dim TempRINEX_Nav As New RINEX_Nav
        Dim newRecord As Integer = 1
        Dim Sat_Clock_Corr, Rel_Corr, TGD_Corr As Decimal
        Dim AboveMask As Boolean
        Dim NumSatsInEpoch As Integer = 0
        Dim CartPosition(2) As Decimal
        Dim CurvPosition() As Decimal

        CartPosition(0) = SelectedObsFile.Approx_X
        CartPosition(1) = SelectedObsFile.Approx_Y
        CartPosition(2) = SelectedObsFile.Approx_Z
        CurvPosition = cart2curv(CartPosition)

        'define rotation matrices to convert ECEF to Local E,N,Up
        Dim R1, R3 As New Matrix(3, 3)
        R1.data(1, 1) = 1D
        R1.data(1, 2) = 0D
        R1.data(1, 3) = 0D
        R1.data(2, 1) = 0D
        R1.data(2, 2) = Math.Cos((90 - CurvPosition(0)) * Math.PI / 180D)
        R1.data(2, 3) = Math.Sin((90 - CurvPosition(0)) * Math.PI / 180D)
        R1.data(3, 1) = 0D
        R1.data(3, 2) = -1 * Math.Sin((90 - CurvPosition(0)) * Math.PI / 180D)
        R1.data(3, 3) = Math.Cos((90 - CurvPosition(0)) * Math.PI / 180D)

        R3.data(1, 1) = Math.Cos((90 + CurvPosition(1)) * Math.PI / 180D)
        R3.data(1, 2) = Math.Sin((90 + CurvPosition(1)) * Math.PI / 180D)
        R3.data(1, 3) = 0D
        R3.data(2, 1) = -1 * Math.Sin((90 + CurvPosition(1)) * Math.PI / 180D)
        R3.data(2, 2) = Math.Cos((90 + CurvPosition(1)) * Math.PI / 180D)
        R3.data(2, 3) = 0D
        R3.data(3, 1) = 0D
        R3.data(3, 2) = 0D
        R3.data(3, 3) = 1D

        'locate the C/A code ("C1"), P1 code ("P1"), and P2 code ("P2") pseudorange observables
        For m = 0 To 17
            If SelectedObsFile.Obs_Type(m).ToUpper = "C1" Then
                pseudorangeIndex = m
            ElseIf SelectedObsFile.Obs_Type(m).ToUpper = "P1" Then
                P1Index = m
            ElseIf SelectedObsFile.Obs_Type(m).ToUpper = "P2" Then
                P2Index = m
            End If
        Next

        Dim increment As Integer = 1
        Dim Number2Process As Integer = Ending - Starting + 1
        If Number2Process > 50 Then  'or Maxepochs
            increment = Number2Process \ 50 'or MaxEpochs
        End If

        If pseudorangeIndex <> -9999I Then
            Dim epochCount As Integer = 1
            For z = Starting To Ending Step increment
                ReceptionTime(0) = SelectedObsFile.ObsData(0, 0, z)
                ReceptionTime(1) = SelectedObsFile.ObsData(0, 1, z)

                For i = 4 To SelectedObsFile.NumRows(z) - 1
                    TimeDiffFromReference = 1000000000000.0
                    pseudorange = 0
                    transmitDistance = 0
                    AboveMask = False
                    SatNum = SelectedObsFile.ObsData(i, 0, z)
                    If SatNum <> -9999I AndAlso PRNs2UseBoolean(SatNum) = True Then
                        NumSatsInEpoch += 1
                        Eph2Use = New RINEX_Eph
                        For j = 0 To SelectedNavFile.EphemerisV.GetLength(1) - 1
                            For k = 0 To SelectedNavFile.EphemerisV.GetLength(0) - 1
                                PRN = SelectedNavFile.EphemerisV(k, j).PRN
                                If SatNum = PRN Then
                                    EphemerisTime(0) = SelectedNavFile.EphemerisV(k, j).Toe_Week
                                    EphemerisTime(1) = SelectedNavFile.EphemerisV(k, j).Toe
                                    If Math.Abs((EphemerisTime(0) * 604800D + EphemerisTime(1)) - (ReceptionTime(0) * 604800D + ReceptionTime(1))) < TimeDiffFromReference Then
                                        TimeDiffFromReference = Math.Abs((EphemerisTime(0) * 604800D + EphemerisTime(1)) - (ReceptionTime(0) * 604800D + ReceptionTime(1)))
                                        Eph2Use = SelectedNavFile.EphemerisV(k, j)
                                    End If
                                End If
                            Next
                        Next

                        pseudorange = SelectedObsFile.ObsData(i, (pseudorangeIndex + 1), z)

                        If pseudorange <> -9999D And pseudorange <> 0D Then
                            If Eph2Use.PRN <> -9999I Then
                                Sat_Clock_Corr = 0

                                If receptionRadioButton.Checked Then
                                    transmitDistance = 0D
                                Else
                                    transmitDistance = pseudorange
                                End If
                                Dim delta As Decimal = 1000
                                While delta > 0.1
                                    ECEF_XYZ_SatPosition = TempRINEX_Nav.ComputeSatellitePositionFromEphemeris(Sat_Clock_Corr, Rel_Corr, TGD_Corr, Eph2Use, ReceptionTime, transmitDistance, EarthRotCheckBox.Checked, SatClockCheckBox.Checked, True, True)
                                    If ECEF_XYZ_SatPosition(0) = 0 Then 'good position returned
                                        Dim newRange As Decimal = Math.Sqrt((ECEF_XYZ_SatPosition(1) - SelectedObsFile.Approx_X) ^ 2 + (ECEF_XYZ_SatPosition(2) - SelectedObsFile.Approx_Y) ^ 2 + (ECEF_XYZ_SatPosition(3) - SelectedObsFile.Approx_Z) ^ 2)
                                        delta = Math.Abs(newRange - transmitDistance)
                                        transmitDistance = newRange
                                    Else
                                        Exit While
                                    End If
                                End While

                                If ECEF_XYZ_SatPosition(0) = 0 Then
                                    Dim E_N_Up, Temp As Matrix
                                    Dim diff As New Matrix(3, 1)
                                    diff.data(1, 1) = ECEF_XYZ_SatPosition(1) - SelectedObsFile.Approx_X
                                    diff.data(2, 1) = ECEF_XYZ_SatPosition(2) - SelectedObsFile.Approx_Y
                                    diff.data(3, 1) = ECEF_XYZ_SatPosition(3) - SelectedObsFile.Approx_Z
                                    Temp = R3 * diff
                                    E_N_Up = R1 * Temp

                                    Dim Elev, Azimuth As Decimal
                                    Elev = (Math.Atan2(E_N_Up.data(3, 1), Math.Sqrt(E_N_Up.data(2, 1) ^ 2D + E_N_Up.data(1, 1) ^ 2D))) * 180D / Math.PI
                                    Azimuth = (Math.Atan2(E_N_Up.data(1, 1), E_N_Up.data(2, 1))) * 180D / Math.PI

                                    If Azimuth < 0 Then
                                        Azimuth += 360D
                                    End If

                                    If Elev >= ElevMaskNumericUpDown.Value Then
                                        AboveMask = True
                                    End If

                                    'secondary test using AboveMask flag to check if an unhealthy sat is being used when the user does not want to use unhealthy sats in the solution
                                    If UseUnhealthySatsBoolean = False And Eph2Use.SV_health <> 0D Then
                                        AboveMask = False
                                    End If

                                    If AboveMask = True Then
                                        SatPositions = SatPositions.matrixReDim(newRecord, 9, True)
                                        SatPositions.data(newRecord, 1) = ECEF_XYZ_SatPosition(1)
                                        SatPositions.data(newRecord, 2) = ECEF_XYZ_SatPosition(2)
                                        SatPositions.data(newRecord, 3) = ECEF_XYZ_SatPosition(3)

                                        If SatClockCheckBox.Checked Then
                                            If TGDCheckBox.Checked Then
                                                Sat_Clock_Corr -= TGD_Corr
                                            End If
                                            If RelativisticCheckBox.Checked Then
                                                Sat_Clock_Corr += Rel_Corr
                                            End If
                                            Sat_Clock_Corr *= SPEED_LIGHT
                                        Else
                                            Sat_Clock_Corr = 0
                                        End If

                                        Dim d_ion As Decimal = 0
                                        If IonoCheckBox.Checked Then
                                            'broadcast ionospheric model to be used to calc ionospheric correction
                                            If IonoRadioButton.Checked Then
                                                With SelectedNavFile
                                                    d_ion = calcL1Ionospheric(.Ion_A0, .Ion_A1, .Ion_A2, .Ion_A3, .Ion_B0, .Ion_B1, .Ion_B2, .Ion_B3, CurvPosition(0), CurvPosition(1), Elev, Azimuth, ReceptionTime(1))
                                                    'pseudorange -= d_ion
                                                    'd_ion = 0
                                                End With
                                                'dual frequency data to be used to calc ionospheric correction
                                            ElseIf IonoFreeRadioButton.Checked Then
                                                Const gamma As Decimal = (77 / 60) ^ 2
                                                Dim P1range As Decimal = SelectedObsFile.ObsData(i, (P1Index + 1), z)
                                                Dim P2range As Decimal = SelectedObsFile.ObsData(i, (P2Index + 1), z)

                                                If P1range <> -9999D And P2range <> -9999D Then
                                                    'd_ion = pseudorange - (P2range - gamma * P1range) / (1 - gamma)
                                                    d_ion = ((1227600000.0) ^ 2 / (1227600000.0 ^ 2 - 1575420000.0 ^ 2)) * (P1range - P2range)
                                                    'pseudorange -= d_ion
                                                    'd_ion = 0
                                                End If
                                            End If
                                        End If

                                        Dim d_trop As Decimal = 0
                                        If TropCheckBox.Checked Then
                                            d_trop = calcTropospheric(TropTempTextBox.Text, TropPressureTextBox.Text, TropHumidityTextBox.Text, Elev)
                                            'pseudorange -= d_trop
                                            'd_trop = 0
                                        End If

                                        SatPositions.data(newRecord, 4) = pseudorange
                                        SatPositions.data(newRecord, 5) = Sat_Clock_Corr
                                        SatPositions.data(newRecord, 6) = d_ion
                                        SatPositions.data(newRecord, 7) = d_trop
                                        SatPositions.data(newRecord, 8) = epochCount
                                        SatPositions.data(newRecord, 9) = 1D / (Math.Sqrt(1D - (6371D * Math.Cos(Elev * Math.PI / 180D) / 6721D) ^ 2))
                                        newRecord += 1
                                    End If
                                Else
                                    Success = False
                                    Exit Sub
                                End If
                            Else

                            End If
                        Else

                        End If
                    End If
                Next
                epochCount += 1
            Next

            If SatPositions.nRows >= 4 Then
                Dim A As New Matrix(SatPositions.nRows, 3 + epochCount - 1)
                Dim w As New Matrix(SatPositions.nRows, 1)
                Dim L As New Matrix(SatPositions.nRows, 1)
                Dim Cl As New Matrix(SatPositions.nRows, SatPositions.nRows)
                Dim Xo As New Matrix(3 + epochCount - 1, 1)
                Dim N, u, d, X, v, NInverse As Matrix
                Xo.data(1, 1) = SelectedObsFile.Approx_X
                Xo.data(2, 1) = SelectedObsFile.Approx_Y
                Xo.data(3, 1) = SelectedObsFile.Approx_Z
                Dim stopCondition As Boolean = False
                Dim threshold As Decimal = 0.001
                Dim loopCounter As Integer = 1

                For f As Integer = 1 To SatPositions.nRows
                    Cl.data(f, f) = SatPositions.data(f, 9) ^ 2
                    'For g As Integer = 1 To SatPositions.nRows
                    '    Cl.data(f, g) = SatPositions.data(f, 9) * SatPositions.data(g, 9)
                    'Next
                Next

                Dim apriori As Decimal = 1

                If IonoCheckBox.Checked And IonoFreeRadioButton.Checked Then
                    Cl = Cl.makeIdentity
                End If

                Dim P As Matrix = Cl.Inverse
                'Dim P As Matrix = Cl.makeIdentity()
                P = apriori * P

                While Not stopCondition
                    Dim range, FXo As Decimal
                    For i = 1 To SatPositions.nRows
                        range = Math.Sqrt((SatPositions.data(i, 1) - Xo.data(1, 1)) ^ 2 + (SatPositions.data(i, 2) - Xo.data(2, 1)) ^ 2 + (SatPositions.data(i, 3) - Xo.data(3, 1)) ^ 2)
                        A.data(i, 1) = -1D * ((SatPositions.data(i, 1) - Xo.data(1, 1)) / (range))
                        A.data(i, 2) = -1D * ((SatPositions.data(i, 2) - Xo.data(2, 1)) / (range))
                        A.data(i, 3) = -1D * ((SatPositions.data(i, 3) - Xo.data(3, 1)) / (range))
                        A.data(i, SatPositions.data(i, 8) + 3) = 1D
                        FXo = range - SatPositions.data(i, 5) + SatPositions.data(i, 6) + SatPositions.data(i, 7) + Xo.data(SatPositions.data(i, 8) + 3, 1)
                        w.data(i, 1) = FXo - SatPositions.data(i, 4)
                    Next

                    'A.printAll()
                    'w.printAll()


                    N = A.Transpose * P * A
                    u = A.Transpose * P * w
                    NInverse = N.Inverse
                    d = -1 * (NInverse * u)
                    X = Xo + d
                    Xo = X

                    If Math.Abs(d.data(1, 1)) <= threshold And Math.Abs(d.data(2, 1)) <= threshold And Math.Abs(d.data(3, 1)) <= threshold Then
                        stopCondition = True
                    ElseIf loopCounter = 10 Then
                        stopCondition = True
                        Success = False
                        If SuppressWarnings = False Then
                            MessageBox.Show("Could not converge on the coordinates for the receiver's position after 20 iterations" & ControlChars.NewLine & ControlChars.NewLine & _
                            "dx: " & Decimal.Round(d.data(1, 1), 4) & ControlChars.NewLine & "dy: " & Decimal.Round(d.data(2, 1), 4) & ControlChars.NewLine & "dz: " & Decimal.Round(d.data(3, 1), 4), "Non-converging Solution", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If
                    End If
                    loopCounter += 1
                End While

                If Success Then
                    Dim ReceiverCartPosition(2) As Decimal
                    ReceiverCartPosition(0) = X.data(1, 1)
                    ReceiverCartPosition(1) = X.data(2, 1)
                    ReceiverCartPosition(2) = X.data(3, 1)
                    Dim ReceiverCurvPosition() As Decimal = cart2curv(ReceiverCartPosition)

                    Dim StdDevN, StdDevE, StdDevUp As String
                    Dim StdDevX, StdDevY, StdDevZ, StdDev_cdT As Decimal
                    StdDevX = -9999
                    StdDevY = -9999
                    StdDevZ = -9999
                    StdDevN = "N/A"
                    StdDevE = "N/A"
                    StdDevUp = "N/A"

                    If A.nRows > 4 Then
                        Dim Cx, Cenu, X_StdDevs As Matrix
                        Dim ENU_StdDevs As New Matrix(3, 1)
                        Dim apost As Decimal

                        v = A * d + w
                        apost = (v.Transpose * v).toScalar
                        apost = apost / (A.nRows - A.nCols)
                        Cx = apost * NInverse

                        X_StdDevs = Cx.getDiagonal.Sqrt

                        'law of propagation of variances to compute Std Devs. in the ENU coordinate system
                        Cenu = Cx.matrixReDim(3, 3, True)

                        'Dim R As New Matrix(3, 3)
                        'R.data(1, 1) = -1 * Math.Sin(ReceiverCurvPosition(0) * Math.PI / 180D) * Math.Cos(ReceiverCurvPosition(1) * Math.PI / 180D)
                        'R.data(1, 2) = -1 * Math.Sin(ReceiverCurvPosition(0) * Math.PI / 180D) * Math.Sin(ReceiverCurvPosition(1) * Math.PI / 180D)
                        'R.data(1, 3) = Math.Cos(ReceiverCurvPosition(0) * Math.PI / 180D)
                        'R.data(2, 1) = -1 * Math.Sin(ReceiverCurvPosition(1) * Math.PI / 180D)
                        'R.data(2, 2) = Math.Cos(ReceiverCurvPosition(1) * Math.PI / 180D)
                        'R.data(2, 3) = 0
                        'R.data(3, 1) = Math.Cos(ReceiverCurvPosition(0) * Math.PI / 180D) * Math.Cos(ReceiverCurvPosition(1) * Math.PI / 180D)
                        'R.data(3, 2) = Math.Cos(ReceiverCurvPosition(0) * Math.PI / 180D) * Math.Sin(ReceiverCurvPosition(1) * Math.PI / 180D)
                        'R.data(3, 3) = Math.Sin(ReceiverCurvPosition(0) * Math.PI / 180D)

                        'Cenu = R * Cenu * (R.Transpose)
                        Cenu = (R1 * R3) * Cenu * (R1 * R3).Transpose
                        Cenu = Cenu.getDiagonal.Sqrt
                        ENU_StdDevs.data(1, 1) = Cenu.data(1, 1)
                        ENU_StdDevs.data(2, 1) = Cenu.data(2, 1)
                        ENU_StdDevs.data(3, 1) = Cenu.data(3, 1)

                        StdDevX = X_StdDevs.data(1, 1)
                        StdDevY = X_StdDevs.data(2, 1)
                        StdDevZ = X_StdDevs.data(3, 1)
                        StdDev_cdT = X_StdDevs.data(4, 1)

                        StdDevE = Math.Abs(Decimal.Round(2 * (ENU_StdDevs.data(1, 1)), 3)).ToString & " m"
                        StdDevN = Math.Abs(Decimal.Round(2 * (ENU_StdDevs.data(2, 1)), 3)).ToString & " m"
                        StdDevUp = Math.Abs(Decimal.Round(2 * (ENU_StdDevs.data(3, 1)), 3)).ToString & " m"

                        Dim Qx, Qenu As Matrix
                        Qx = (A.Transpose * A).Inverse
                        Qx = Qx.matrixReDim(3, 3, True)

                        'Qenu = R * Qx * R.Transpose
                        Qenu = R1 * R3 * Qx * (R1 * R3).Transpose
                        Qenu = Qenu.getDiagonal

                        DOPS(1) = Math.Sqrt(Qenu.data(1, 1) + Qenu.data(2, 1) + Qenu.data(3, 1))    'PDOP
                        DOPS(2) = Math.Sqrt(Qenu.data(1, 1) + Qenu.data(2, 1))                      'HDOP
                        DOPS(3) = Math.Sqrt(Qenu.data(3, 1))                                        'VDOP

                        Dim MRSE, UERE As Decimal
                        MRSE = 2 * Math.Sqrt(StdDevX ^ 2 + StdDevY ^ 2 + StdDevZ ^ 2)
                        UERE = MRSE / DOPS(1)
                        H2drms = DOPS(2) * UERE
                        V2drms = DOPS(3) * UERE
                    End If

                    Results(0) = ReceiverCartPosition(0)
                    Results(1) = ReceiverCartPosition(1)
                    Results(2) = ReceiverCartPosition(2)
                    Results(3) = ReceiverCurvPosition(0)
                    Results(4) = ReceiverCurvPosition(1)
                    Results(5) = ReceiverCurvPosition(2)
                    Results(6) = X.data(4, 1)
                    StdDevs(0) = StdDevX
                    StdDevs(1) = StdDevY
                    StdDevs(2) = StdDevZ
                    StdDevs(3) = StdDevE
                    StdDevs(4) = StdDevN
                    StdDevs(5) = StdDevUp
                    StdDevs(6) = StdDev_cdT
                    DOPS(0) = SatPositions.nRows
                End If
            Else
                Success = False
                If SuppressWarnings = False Then
                    MessageBox.Show("Not enough observations where recorded for this epoch to solve for the receiver's position" & ControlChars.NewLine & "Or the Elevation Mask was set too high", "Low # of Observations or Too High of Mask", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        Else
            Success = False
            If SuppressWarnings = False Then
                MessageBox.Show("No pseudoranges on L1 were observed, unable to process", "Code Only Processing", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End If
    End Sub

    Private Sub CalcPositionSequential(ByVal Starting As Integer, ByVal Ending As Integer, ByRef Success As Boolean, ByRef Results() As Decimal, ByRef StdDevs() As String, ByRef DOPS() As Decimal, ByRef H2drms As Decimal, ByRef V2drms As Decimal, Optional ByVal SuppressWarnings As Boolean = False)

        Dim i, j, k, m, z As Integer
        Dim SatNum As Integer
        Dim PRN As Integer
        Dim ReceptionTime(1), EphemerisTime(1) As Decimal
        Dim TimeDiffFromReference As Decimal
        Dim Eph2Use As New RINEX_Eph
        Dim pseudorange, transmitDistance As Decimal
        Dim pseudorangeIndex As Integer = -9999I
        Dim P1Index As Integer = -9999I
        Dim P2Index As Integer = -9999I
        Dim SatPositions As New Matrix(1, 8)
        Dim ECEF_XYZ_SatPosition(0 To 3) As Decimal
        Dim TempRINEX_Nav As New RINEX_Nav
        Dim newRecord As Integer = 1
        Dim Sat_Clock_Corr, Rel_Corr, TGD_Corr As Decimal
        Dim AboveMask As Boolean
        Dim NumSatsInEpoch As Integer = 0
        Dim CartPosition(2) As Decimal
        Dim CurvPosition() As Decimal

        CartPosition(0) = SelectedObsFile.Approx_X
        CartPosition(1) = SelectedObsFile.Approx_Y
        CartPosition(2) = SelectedObsFile.Approx_Z
        CurvPosition = cart2curv(CartPosition)

        'define rotation matrices to convert ECEF to Local E,N,Up
        Dim R1, R3 As New Matrix(3, 3)
        R1.data(1, 1) = 1D
        R1.data(1, 2) = 0D
        R1.data(1, 3) = 0D
        R1.data(2, 1) = 0D
        R1.data(2, 2) = Math.Cos((90 - CurvPosition(0)) * Math.PI / 180D)
        R1.data(2, 3) = Math.Sin((90 - CurvPosition(0)) * Math.PI / 180D)
        R1.data(3, 1) = 0D
        R1.data(3, 2) = -1 * Math.Sin((90 - CurvPosition(0)) * Math.PI / 180D)
        R1.data(3, 3) = Math.Cos((90 - CurvPosition(0)) * Math.PI / 180D)

        R3.data(1, 1) = Math.Cos((90 + CurvPosition(1)) * Math.PI / 180D)
        R3.data(1, 2) = Math.Sin((90 + CurvPosition(1)) * Math.PI / 180D)
        R3.data(1, 3) = 0D
        R3.data(2, 1) = -1 * Math.Sin((90 + CurvPosition(1)) * Math.PI / 180D)
        R3.data(2, 2) = Math.Cos((90 + CurvPosition(1)) * Math.PI / 180D)
        R3.data(2, 3) = 0D
        R3.data(3, 1) = 0D
        R3.data(3, 2) = 0D
        R3.data(3, 3) = 1D

        'locate the pseudorange observable ("C1")
        For m = 0 To 17
            If SelectedObsFile.Obs_Type(m) = "C1" Then
                pseudorangeIndex = m
            ElseIf SelectedObsFile.Obs_Type(m).ToUpper = "P1" Then
                P1Index = m
            ElseIf SelectedObsFile.Obs_Type(m).ToUpper = "P2" Then
                P2Index = m
            End If
        Next

        Dim increment As Integer = 1
        Dim Number2Process As Integer = Ending - Starting + 1
        If Number2Process > MaxEpochs Then
            increment = Number2Process \ MaxEpochs
        End If
        'increment = 1

        If pseudorangeIndex <> -9999I Then
            Dim Xo As New Matrix(4, 1)
            Xo.data(1, 1) = SelectedObsFile.Approx_X
            Xo.data(2, 1) = SelectedObsFile.Approx_Y
            Xo.data(3, 1) = SelectedObsFile.Approx_Z
            Xo.data(4, 1) = 0

            Dim X_POE As New Matrix(4, 1)
            X_POE.data(1, 1) = Xo.data(1, 1)
            X_POE.data(2, 1) = Xo.data(2, 1)
            X_POE.data(3, 1) = Xo.data(3, 1)
            X_POE.data(4, 1) = Xo.data(4, 1)

            Dim Avg_X_POE As New Matrix(3, 1)
            Dim Avg_X_POE_count As Integer = 0
            Avg_X_POE.data(1, 1) = 0
            Avg_X_POE.data(2, 1) = 0
            Avg_X_POE.data(3, 1) = 0

            Dim N_total As New Matrix(3, 3)
            Dim U_total As New Matrix(3, 1)
            Dim V_total As New Matrix(1, 1)

            Dim Inv_Hrms, Inv_Vrms As Decimal
            Inv_Hrms = 0
            Inv_Vrms = 0

            Dim NumObs As Integer = 0
            Dim NumUnknowns As Integer = 3

            Dim epochCount As Integer = 0

            StatusLabel.Text = "Please wait, computing Receiver's Position..."
            Me.Refresh()
            ProgressBar1.Maximum = (Ending \ increment) - Starting
            ProgressBar1.Visible = True

            For z = Starting To Ending Step increment
                Xo.data(1, 1) = X_POE.data(1, 1)
                Xo.data(2, 1) = X_POE.data(2, 1)
                Xo.data(3, 1) = X_POE.data(3, 1)
                Xo.data(4, 1) = X_POE.data(4, 1)

                newRecord = 1
                SatPositions = SatPositions.matrixReDim(newRecord, 7)
                ReceptionTime(0) = SelectedObsFile.ObsData(0, 0, z)
                ReceptionTime(1) = SelectedObsFile.ObsData(0, 1, z)

                If ProgressBar1.Value <> ProgressBar1.Maximum Then
                    ProgressBar1.Value += 1
                    ProgressBar1.Update()
                End If

                For i = 4 To SelectedObsFile.NumRows(z) - 1
                    TimeDiffFromReference = 1000000000000.0
                    pseudorange = 0
                    transmitDistance = 0
                    AboveMask = False
                    SatNum = SelectedObsFile.ObsData(i, 0, z)
                    If SatNum <> -9999I AndAlso PRNs2UseBoolean(SatNum) = True Then
                        NumSatsInEpoch += 1
                        Eph2Use = New RINEX_Eph
                        For j = 0 To SelectedNavFile.EphemerisV.GetLength(1) - 1
                            For k = 0 To SelectedNavFile.EphemerisV.GetLength(0) - 1
                                PRN = SelectedNavFile.EphemerisV(k, j).PRN
                                If SatNum = PRN Then
                                    EphemerisTime(0) = SelectedNavFile.EphemerisV(k, j).Toe_Week
                                    EphemerisTime(1) = SelectedNavFile.EphemerisV(k, j).Toe
                                    If Math.Abs((EphemerisTime(0) * 604800D + EphemerisTime(1)) - (ReceptionTime(0) * 604800D + ReceptionTime(1))) < TimeDiffFromReference Then
                                        TimeDiffFromReference = Math.Abs((EphemerisTime(0) * 604800D + EphemerisTime(1)) - (ReceptionTime(0) * 604800D + ReceptionTime(1)))
                                        Eph2Use = SelectedNavFile.EphemerisV(k, j)
                                    End If
                                End If
                            Next
                        Next

                        pseudorange = SelectedObsFile.ObsData(i, (pseudorangeIndex + 1), z)

                        If pseudorange <> -9999D And Eph2Use.PRN <> -9999I And Decimal.Round(pseudorange, 0) <> 0 Then
                            Sat_Clock_Corr = 0

                            If receptionRadioButton.Checked Then
                                transmitDistance = 0D
                            Else
                                transmitDistance = pseudorange
                            End If
                            Dim delta As Decimal = 1000
                            While delta > 0.1
                                ECEF_XYZ_SatPosition = TempRINEX_Nav.ComputeSatellitePositionFromEphemeris(Sat_Clock_Corr, Rel_Corr, TGD_Corr, Eph2Use, ReceptionTime, transmitDistance, EarthRotCheckBox.Checked, SatClockCheckBox.Checked, True, True)
                                If ECEF_XYZ_SatPosition(0) = 0 Then 'good position returned
                                    Dim newRange As Decimal = Math.Sqrt((ECEF_XYZ_SatPosition(1) - SelectedObsFile.Approx_X) ^ 2 + (ECEF_XYZ_SatPosition(2) - SelectedObsFile.Approx_Y) ^ 2 + (ECEF_XYZ_SatPosition(3) - SelectedObsFile.Approx_Z) ^ 2)
                                    delta = Math.Abs(newRange - transmitDistance)
                                    transmitDistance = newRange
                                Else
                                    Exit While
                                End If
                            End While

                            If ECEF_XYZ_SatPosition(0) = 0 Then
                                Dim E_N_Up As Matrix
                                Dim diff As New Matrix(3, 1)
                                diff.data(1, 1) = ECEF_XYZ_SatPosition(1) - SelectedObsFile.Approx_X
                                diff.data(2, 1) = ECEF_XYZ_SatPosition(2) - SelectedObsFile.Approx_Y
                                diff.data(3, 1) = ECEF_XYZ_SatPosition(3) - SelectedObsFile.Approx_Z
                                E_N_Up = R1 * R3 * diff

                                Dim Elev, Azimuth As Decimal
                                Elev = (Math.Atan2(E_N_Up.data(3, 1), Math.Sqrt(E_N_Up.data(2, 1) ^ 2D + E_N_Up.data(1, 1) ^ 2D))) * 180D / Math.PI
                                Azimuth = (Math.Atan2(E_N_Up.data(1, 1), E_N_Up.data(2, 1))) * 180D / Math.PI

                                If Azimuth < 0 Then
                                    Azimuth += 360D
                                End If

                                If Elev >= ElevMaskNumericUpDown.Value Then
                                    AboveMask = True
                                End If

                                'secondary test using AboveMask flag to check if an unhealthy sat is being used when the user does not want to use unhealthy sats in the solution
                                If UseUnhealthySatsBoolean = False And Eph2Use.SV_health <> 0D Then
                                    AboveMask = False
                                End If

                                If AboveMask = True Then
                                    SatPositions = SatPositions.matrixReDim(newRecord, 8, True)
                                    SatPositions.data(newRecord, 1) = ECEF_XYZ_SatPosition(1)
                                    SatPositions.data(newRecord, 2) = ECEF_XYZ_SatPosition(2)
                                    SatPositions.data(newRecord, 3) = ECEF_XYZ_SatPosition(3)

                                    If SatClockCheckBox.Checked Then
                                        If TGDCheckBox.Checked Then
                                            Sat_Clock_Corr -= TGD_Corr
                                        End If
                                        If RelativisticCheckBox.Checked Then
                                            Sat_Clock_Corr += Rel_Corr
                                        End If
                                        Sat_Clock_Corr *= SPEED_LIGHT
                                    Else
                                        Sat_Clock_Corr = 0
                                    End If

                                    Dim d_ion As Decimal = 0
                                    If IonoCheckBox.Checked Then
                                        'broadcast ionospheric model to be used to calc ionospheric correction
                                        If IonoRadioButton.Checked Then
                                            With SelectedNavFile
                                                d_ion = calcL1Ionospheric(.Ion_A0, .Ion_A1, .Ion_A2, .Ion_A3, .Ion_B0, .Ion_B1, .Ion_B2, .Ion_B3, CurvPosition(0), CurvPosition(1), Elev, Azimuth, ReceptionTime(1))
                                                'pseudorange -= d_ion
                                                'd_ion = 0
                                            End With
                                            'dual frequency data to be used to calc ionospheric correction
                                        ElseIf IonoFreeRadioButton.Checked Then
                                            Const gamma As Decimal = (77 / 60) ^ 2
                                            Dim P1range As Decimal = SelectedObsFile.ObsData(i, (P1Index + 1), z)
                                            Dim P2range As Decimal = SelectedObsFile.ObsData(i, (P2Index + 1), z)

                                            If P1range <> -9999D And P2range <> -9999D Then 'And pseudorange <> 0 Then
                                                'd_ion = pseudorange - (P2range - gamma * P1range) / (1 - gamma)
                                                d_ion = ((1227600000.0) ^ 2 / (1227600000.0 ^ 2 - 1575420000.0 ^ 2)) * (P1range - P2range)
                                                'pseudorange -= d_ion
                                                'd_ion = 0
                                            End If
                                        End If
                                    End If

                                    Dim d_trop As Decimal = 0
                                    If TropCheckBox.Checked Then
                                        d_trop = calcTropospheric(TropTempTextBox.Text, TropPressureTextBox.Text, TropHumidityTextBox.Text, Elev)
                                        'pseudorange -= d_trop
                                        'd_trop = 0
                                    End If

                                    SatPositions.data(newRecord, 4) = pseudorange
                                    SatPositions.data(newRecord, 5) = Sat_Clock_Corr
                                    SatPositions.data(newRecord, 6) = d_ion
                                    SatPositions.data(newRecord, 7) = d_trop
                                    SatPositions.data(newRecord, 8) = 1D / (Math.Sqrt(1D - (6371D * Math.Cos(Elev * Math.PI / 180D) / 6721D) ^ 2))
                                    newRecord += 1
                                End If
                            Else
                                Success = False
                                Exit Sub
                            End If
                        End If
                    End If
                Next

                If SatPositions.nRows >= 4 Then
                    Dim A As New Matrix(SatPositions.nRows, 4)
                    Dim w As New Matrix(SatPositions.nRows, 1)
                    Dim L As New Matrix(SatPositions.nRows, 1)
                    Dim Cl As New Matrix(SatPositions.nRows, SatPositions.nRows)

                    For f As Integer = 1 To SatPositions.nRows
                        Cl.data(f, f) = SatPositions.data(f, 8) ^ 2
                        'For g As Integer = 1 To SatPositions.nRows
                        '    Cl.data(f, g) = SatPositions.data(f, 8) * SatPositions.data(g, 8)
                        'Next
                    Next

                    If IonoCheckBox.Checked And IonoFreeRadioButton.Checked Then
                        Cl = Cl.makeIdentity
                    End If

                    Dim apriori As Decimal = 1
                    Dim P As Matrix = Cl.Inverse
                    'Dim P As Matrix = Cl.makeIdentity
                    P = apriori * P

                    Dim N, N_epoch, NInverse, N_xyz, N_cdT, N_xyzcdT, N_cdTxyz, U, U_epoch, U_xyz, U_cdT, d, v As Matrix

                    Dim stopCondition As Boolean = False
                    Dim threshold As Decimal = 0.001
                    Dim loopCounter As Integer = 1

                    'While Not stopCondition
                    Dim range, FXo As Decimal
                    For i = 1 To SatPositions.nRows
                        range = Math.Sqrt((SatPositions.data(i, 1) - Xo.data(1, 1)) ^ 2 + (SatPositions.data(i, 2) - Xo.data(2, 1)) ^ 2 + (SatPositions.data(i, 3) - Xo.data(3, 1)) ^ 2)
                        A.data(i, 1) = -1D * ((SatPositions.data(i, 1) - Xo.data(1, 1)) / (range))
                        A.data(i, 2) = -1D * ((SatPositions.data(i, 2) - Xo.data(2, 1)) / (range))
                        A.data(i, 3) = -1D * ((SatPositions.data(i, 3) - Xo.data(3, 1)) / (range))
                        A.data(i, 4) = 1D
                        FXo = range - SatPositions.data(i, 5) + SatPositions.data(i, 6) + SatPositions.data(i, 7) + Xo.data(4, 1)
                        w.data(i, 1) = FXo - SatPositions.data(i, 4)
                    Next

                    N = A.Transpose * P * A
                    NInverse = N.Inverse
                    U = A.Transpose * P * w

                    'block wise least squares solution
                    N_xyz = N.matrixReDim(3, 3, True)
                    N_cdT = New Matrix(1, 1)
                    N_cdT.data(1, 1) = N.data(4, 4)
                    N_xyzcdT = N.getColumn(4)
                    N_xyzcdT = N_xyzcdT.matrixReDim(3, 1, True)
                    N_cdTxyz = N.getRow(4)
                    N_cdTxyz = N_cdTxyz.matrixReDim(1, 3, True)
                    U_xyz = U.matrixReDim(3, 1, True)
                    U_cdT = New Matrix(1, 1)
                    U_cdT.data(1, 1) = U.data(4, 1)

                    N_epoch = (N_xyz - N_xyzcdT * N_cdT.Inverse * N_cdTxyz)
                    U_epoch = (U_xyz - N_xyzcdT * N_cdT.Inverse * U_cdT)

                    d = -1 * NInverse * U
                    Xo = Xo + d

                    N_total = N_total + N_epoch
                    U_total = U_total + U_epoch
                    v = A * d + w
                    V_total = V_total + (v.Transpose * v)

                    'If Math.Abs(d.data(1, 1)) <= threshold And Math.Abs(d.data(2, 1)) <= threshold And Math.Abs(d.data(3, 1)) <= threshold Then
                    '    stopCondition = True
                    'ElseIf loopCounter = 10 Then
                    '    stopCondition = True
                    '    If SuppressWarnings = False Then
                    '        MessageBox.Show("Could not converge on the coordinates for the receiver's position after 20 iterations" & ControlChars.NewLine & ControlChars.NewLine & _
                    '        "dx: " & Decimal.Round(d.data(1, 1), 4) & ControlChars.NewLine & "dy: " & Decimal.Round(d.data(2, 1), 4) & ControlChars.NewLine & "dz: " & Decimal.Round(d.data(3, 1), 4), "Non-converging Solution", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    '    End If
                    'End If
                    'loopCounter += 1
                    'End While
                    epochCount += 1
                    NumObs += A.nRows
                    NumUnknowns += 1
                End If
            Next

            If ProgressBar1.Value <> ProgressBar1.Maximum Then
                Dim indexUpdater As Integer
                For indexUpdater = ProgressBar1.Value To ProgressBar1.Maximum
                    ProgressBar1.Value = indexUpdater
                    ProgressBar1.Update()
                    System.Threading.Thread.Sleep(1)    'aesthetic to have the progress bar complete before plots are shown
                Next
            End If

            ProgressBar1.Visible = False
            ProgressBar1.Value = ProgressBar1.Minimum

            StatusLabel.Text = String.Empty
            Me.Refresh()

            Dim X_hat, Cx_hat, Qx_Total, d_Total As Matrix
            X_POE = X_POE.matrixReDim(3, 1, True)

            Dim apost, MRSE_Total, UERE_Total As Decimal

            Qx_Total = N_total.Inverse
            Qx_Total = Qx_Total.matrixReDim(3, 3, True)
            Qx_Total = R1 * R3 * Qx_Total * (R1 * R3).Transpose
            Qx_Total = Qx_Total.getDiagonal

            apost = V_total.toScalar / (NumObs - NumUnknowns)

            d_Total = (-1 * N_total.Inverse * U_total)
            X_hat = X_POE + d_total
            Cx_hat = apost * N_total.Inverse

            DOPS(1) = Math.Sqrt(Qx_Total.data(1, 1) + Qx_Total.data(2, 1) + Qx_Total.data(3, 1))    'PDOP
            DOPS(2) = Math.Sqrt(Qx_Total.data(1, 1) + Qx_Total.data(2, 1))                          'HDOP
            DOPS(3) = Math.Sqrt(Qx_Total.data(3, 1))                                                'VDOP

            MRSE_Total = 2 * Math.Sqrt(Cx_hat.data(1, 1) + Cx_hat.data(2, 2) + Cx_hat.data(3, 3))
            UERE_Total = MRSE_Total / DOPS(1)

            H2drms = DOPS(2) * UERE_Total * 10
            V2drms = DOPS(3) * UERE_Total * 10

            'H2drms = 1 / (Math.Sqrt(Inv_Hrms))
            'V2drms = 1 / (Math.Sqrt(Inv_Vrms))

            Dim ReceiverCartPosition(2) As Decimal
            ReceiverCartPosition(0) = X_hat.data(1, 1)
            ReceiverCartPosition(1) = X_hat.data(2, 1)
            ReceiverCartPosition(2) = X_hat.data(3, 1)
            Dim ReceiverCurvPosition() As Decimal = cart2curv(ReceiverCartPosition)

            Dim StdDevN, StdDevE, StdDevUp As String
            Dim StdDevX, StdDevY, StdDevZ As Decimal
            StdDevX = -9999
            StdDevY = -9999
            StdDevZ = -9999
            StdDevN = "N/A"
            StdDevE = "N/A"
            StdDevUp = "N/A"

            Dim Cenu, X_StdDevs As Matrix
            Dim ENU_StdDevs As New Matrix(3, 1)

            X_StdDevs = Cx_hat.getDiagonal.Sqrt

            'law of propagation of variances to compute Std Devs. in the ENU coordinate system
            Cenu = Cx_hat.matrixReDim(3, 3, True)
            Cenu = R1 * R3 * Cenu * (R1 * R3).Transpose
            Cenu = Cenu.getDiagonal.Sqrt
            ENU_StdDevs.data(1, 1) = Cenu.data(1, 1)
            ENU_StdDevs.data(2, 1) = Cenu.data(2, 1)
            ENU_StdDevs.data(3, 1) = Cenu.data(3, 1)

            StdDevX = X_StdDevs.data(1, 1)
            StdDevY = X_StdDevs.data(2, 1)
            StdDevZ = X_StdDevs.data(3, 1)

            StdDevE = Math.Abs(Decimal.Round(3 * (ENU_StdDevs.data(1, 1)), 3)).ToString & " m"
            StdDevN = Math.Abs(Decimal.Round(3 * (ENU_StdDevs.data(2, 1)), 3)).ToString & " m"
            StdDevUp = Math.Abs(Decimal.Round(3 * (ENU_StdDevs.data(3, 1)), 3)).ToString & " m"

            Results(0) = ReceiverCartPosition(0)
            Results(1) = ReceiverCartPosition(1)
            Results(2) = ReceiverCartPosition(2)
            Results(3) = ReceiverCurvPosition(0)
            Results(4) = ReceiverCurvPosition(1)
            Results(5) = ReceiverCurvPosition(2)
            'Results(6) = X_hat.data(4, 1)
            StdDevs(0) = StdDevX
            StdDevs(1) = StdDevY
            StdDevs(2) = StdDevZ
            StdDevs(3) = StdDevE
            StdDevs(4) = StdDevN
            StdDevs(5) = StdDevUp
            DOPS(0) = SatPositions.nRows
        Else
            Success = False
            If SuppressWarnings = False Then
                MessageBox.Show("No pseudoranges on L1 were observed, unable to process", "Code Only Processing", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End If
    End Sub

    Private Sub TropCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TropCheckBox.CheckedChanged
        Label16.Enabled = TropCheckBox.Checked
        Label17.Enabled = TropCheckBox.Checked
        Label18.Enabled = TropCheckBox.Checked
        Label5.Enabled = TropCheckBox.Checked
        TropTempTextBox.Enabled = TropCheckBox.Checked
        TropPressureTextBox.Enabled = TropCheckBox.Checked
        TropHumidityTextBox.Enabled = TropCheckBox.Checked
    End Sub

    'broadcast Klobuchar Ionospheric Model 
    Private Function calcL1Ionospheric(ByVal a0 As Decimal, ByVal a1 As Decimal, ByVal a2 As Decimal, ByVal a3 As Decimal, ByVal b0 As Decimal, ByVal b1 As Decimal, ByVal b2 As Decimal, ByVal b3 As Decimal, ByVal latitude As Decimal, ByVal longitude As Decimal, ByVal elev As Decimal, ByVal azimuth As Decimal, ByVal gpstow As Decimal) As Decimal
        Dim E, x, F, t, AMP, PER, lat, lon, phi_m, lon_i, lat_i, central_angle, d_ion As Decimal

        latitude = latitude * Math.PI / 180D
        longitude = longitude * Math.PI / 180D
        azimuth = azimuth * Math.PI / 180D
        elev = elev * Math.PI / 180D

        lat = latitude / Math.PI
        lon = longitude / Math.PI
        E = elev / Math.PI
        central_angle = 0.0137D / (E + 0.11D) - 0.022D

        lat_i = lat + central_angle * Math.Cos(azimuth)
        If lat_i > 0.416D Then
            lat_i = 0.416D
        ElseIf lat_i < -0.416D Then
            lat_i = -0.416D
        End If

        lon_i = lon + central_angle * Math.Sin(azimuth) / Math.Cos(lat_i * Math.PI)

        t = 43200D * lon_i + gpstow
        While Not (t >= 0D And t <= 86400D)
            If (t < 0D) Then
                t += 86400D
            Else
                t -= 86400D
            End If
        End While

        phi_m = lat_i + 0.064D * Math.Cos((lon_i - 1.617D) * Math.PI)
        F = 1D + 16D * (0.53D - E) * (0.53D - E) * (0.53D - E)

        PER = b0 + b1 * phi_m + b2 * phi_m * phi_m + b3 * phi_m * phi_m * phi_m
        If PER < 72000D Then
            PER = 72000D
        End If

        AMP = a0 + a1 * phi_m + a2 * phi_m * phi_m + a3 * phi_m * phi_m * phi_m
        If AMP < 0D Then
            AMP = 0D
        End If

        x = 2D * Math.PI * (t - 50400D) / PER

        If x >= 1.57D Or x <= -1.57D Then
            d_ion = F * 0.000000005D
        Else
            d_ion = F * (0.000000005D + AMP * (1D - x * x / 2D + x * x * x * x / 24D))
        End If
        d_ion *= SPEED_LIGHT

        Return d_ion
    End Function

    'Hopfield Tropospheric Model
    Private Function calcTropospheric(ByVal temp As String, ByVal pressure As String, ByVal humidity As String, ByVal elev As Decimal) As Decimal
        Dim d_trop As Decimal = 0
        Dim Sd, Sw As Decimal
        Dim TempD, PressureD, HumidityD As Decimal
        Dim boolFlag As Boolean = False

        If temp <> String.Empty And pressure <> String.Empty And humidity <> String.Empty Then
            If IsNumeric(temp) And IsNumeric(pressure) And IsNumeric(humidity) Then
                TempD = Decimal.Parse(temp)
                PressureD = Decimal.Parse(pressure)
                HumidityD = Decimal.Parse(humidity)
                If HumidityD > 100 Or HumidityD < 0 Then
                    boolFlag = True
                    If TropWarning = False Then
                        MessageBox.Show("Humidity input must be between 0% and 100%" & ControlChars.NewLine & "No tropospheric effects have been modelled in this solution", "Bad Tropospheric Model Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        TropWarning = True
                    End If
                End If
                If PressureD > 110 Or PressureD < 30 Then
                    boolFlag = True
                    If TropWarning = False Then
                        MessageBox.Show("Pressure input must be between 110 kPa and 30 kPa" & ControlChars.NewLine & "No tropospheric effects have been modelled in this solution", "Bad Tropospheric Model Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        TropWarning = True
                    End If
                End If
                If TempD > 60 Or TempD < -60 Then
                    boolFlag = True
                    If TropWarning = False Then
                        MessageBox.Show("Temperature input must be between 60" & Chr(176) & "C and -60" & Chr(176) & "C" & ControlChars.NewLine & "No tropospheric effects have been modelled in this solution", "Bad Tropospheric Model Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        TropWarning = True
                    End If
                End If
            Else
                boolFlag = True
                If TropWarning = False Then
                    MessageBox.Show("Non-numeric Tropospheric Input" & ControlChars.NewLine & "No tropospheric effects have been modelled in this solution", "Bad Tropospheric Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    TropWarning = True
                End If
            End If
        Else
            boolFlag = True
            If TropWarning = False Then
                MessageBox.Show("Missing Tropospheric Input, a box(es) have been left blank" & ControlChars.NewLine & "No tropospheric effects have been modelled in this solution", "Bad Tropospheric Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TropWarning = True
            End If

        End If

        If Not boolFlag Then
            PressureD = 10D * PressureD
            TempD = TempD + 273.15
            HumidityD = 0.01D * HumidityD * Math.E ^ (-37.2465 + 0.213166 * TempD - 0.000256908 * TempD ^ 2)
            elev = elev * Math.PI / 180D
            Sd = ((0.00007764 * (PressureD / TempD)) / (5 * Math.Sin(Math.Sqrt(elev ^ 2 + (6.25 / 32400) * Math.PI ^ 2)))) * (40136 + 148.72 * (TempD - 273.15))
            Sw = (4089.8 * (HumidityD / TempD ^ 2) - 0.14256 * (HumidityD / TempD)) / (5 * Math.Sin(Math.Sqrt(elev ^ 2 + (2.25 / 32400) * Math.PI ^ 2)))
            d_trop = Sd + Sw
        End If
        Return d_trop
    End Function

    'input cartesian coordinates in metres output geodetic (curvilinear) coordinates in decimal degrees
    Private Function cart2curv(ByVal cart_Coords() As Decimal) As Decimal()
        
        Dim X, Y, Z As Decimal
        Dim results(2) As Decimal
        X = cart_Coords(0)
        Y = cart_Coords(1)
        Z = cart_Coords(2)

        Dim phi2, lambda, h, v As Decimal
        Dim phi1 As Decimal = 0D
        Dim iterations As Integer = 0

        lambda = Math.Atan2(Y, X)

        phi2 = Math.Atan2(Z, (Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2))))

        While (Math.Abs(phi2 - phi1) > Math.Pow(4.8481368, -11D)) And iterations < 100
            phi1 = phi2
            v = WGS84_a / (Math.Sqrt(1 - WGS84_e2 * Math.Pow(Math.Sin(phi1), 2)))
            phi2 = Math.Atan2((Z + WGS84_e2 * v * Math.Sin(phi1)), (Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2))))
            iterations += 1
        End While

        h = (((Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2))) / Math.Cos(phi2)) - v)
        h = Decimal.Round(h, 4)

        results(0) = phi2 * 180D / Math.PI
        results(1) = lambda * 180D / Math.PI
        results(2) = h

        Return results
    End Function

    'input geodetic (curvilinear) coordinates in decimal degrees output cartesian coordinates in metres
    Private Function curv2cart(ByVal curv_Coords() As Decimal) As Decimal()

        Dim results(2) As Decimal
        Dim phi, lambda, h, v As Decimal

        phi = curv_Coords(0) * Math.PI / 180D
        lambda = curv_Coords(1) * Math.PI / 180D
        h = curv_Coords(2)

        v = WGS84_a / (Math.Sqrt(1 - WGS84_e2 * Math.Pow(Math.Sin(phi), 2)))
        results(0) = (v + h) * Math.Cos(phi) * Math.Cos(lambda)
        results(1) = (v + h) * Math.Cos(phi) * Math.Sin(lambda)
        results(2) = ((1 - WGS84_e2) * v + h) * Math.Sin(phi)

        Return results
    End Function

    Friend Function DecToDMS(ByVal arg1 As Decimal, ByVal Dir As Integer) As String
        Dim var1, var2, var3, var4, var5, var6, arg2 As Decimal
        Dim ending As String

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

        If arg1 < 0 And Dir = 1 Then
            ending = " S"
        ElseIf arg1 > 0 And Dir = 1 Then
            ending = " N"
        ElseIf arg1 < 0 And Dir = 2 Then
            ending = " W"
        Else
            ending = " E"
        End If

        DecToDMS = var1.ToString & Chr(176) & " " & var4.ToString("00") & "' " & Decimal.Round(var6, 5).ToString("00.00000") & Chr(34) & ending
    End Function

    Private Sub checkIonoModel()
        If SelectedNavFile.Ion_A0 = -9999D Or _
        SelectedNavFile.Ion_A1 = -9999D Or _
        SelectedNavFile.Ion_A2 = -9999D Or _
        SelectedNavFile.Ion_A3 = -9999D Or _
        SelectedNavFile.Ion_B0 = -9999D Or _
        SelectedNavFile.Ion_B1 = -9999D Or _
        SelectedNavFile.Ion_B2 = -9999D Or _
        SelectedNavFile.Ion_B3 = -9999D Then
            IonoRadioButton.Checked = False
            IonoRadioButton.Enabled = False
        Else
            IonoRadioButton.Checked = True
            IonoRadioButton.Enabled = True
        End If
    End Sub

    Private Sub NavDetailsGroupBox_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles NavDetailsGroupBox.EnabledChanged
        checkIonoModel()
    End Sub

    Private Sub ObsDetailsGroupBox_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ObsDetailsGroupBox.EnabledChanged
        checkIonoModel()
    End Sub

    Private Sub SkyPlotALLButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SkyPlotALLButton.Click, SkyPlotEpochButton.Click
        If NavDetailsGroupBox.Enabled = True Then
            StatusLabel.Text = "Please wait, generating SkyPlot..."
            Me.Refresh()

            Dim selectedButton As Button
            selectedButton = CType(sender, Button)

            Dim i, j, k, q, startAt, stopAt As Integer
            Dim SatNum As Integer
            Dim PRN As Integer
            Dim ReceptionTime(1), EphemerisTime(1) As Decimal
            Dim TimeDiffFromReference As Decimal
            Dim Eph2Use As New RINEX_Eph
            Dim ECEF_XYZ_SatPosition(0 To 3) As Decimal
            Dim TempRINEX_Nav As New RINEX_Nav
            Dim Sat_Clock_Corr, Rel_Corr, TGD_Corr As Decimal
            Dim newRecord As Integer = 3

            Dim CartPosition(2) As Decimal
            Dim CurvPosition() As Decimal
            CartPosition(0) = SelectedObsFile.Approx_X
            CartPosition(1) = SelectedObsFile.Approx_Y
            CartPosition(2) = SelectedObsFile.Approx_Z
            CurvPosition = cart2curv(CartPosition)

            'define rotation matrices to convert ECEF to Local E,N,Up
            Dim R1, R3 As New Matrix(3, 3)
            R1.data(1, 1) = 1D
            R1.data(1, 2) = 0D
            R1.data(1, 3) = 0D
            R1.data(2, 1) = 0D
            R1.data(2, 2) = Math.Cos((90 - CurvPosition(0)) * Math.PI / 180D)
            R1.data(2, 3) = Math.Sin((90 - CurvPosition(0)) * Math.PI / 180D)
            R1.data(3, 1) = 0D
            R1.data(3, 2) = -1 * Math.Sin((90 - CurvPosition(0)) * Math.PI / 180D)
            R1.data(3, 3) = Math.Cos((90 - CurvPosition(0)) * Math.PI / 180D)

            R3.data(1, 1) = Math.Cos((90 + CurvPosition(1)) * Math.PI / 180D)
            R3.data(1, 2) = Math.Sin((90 + CurvPosition(1)) * Math.PI / 180D)
            R3.data(1, 3) = 0D
            R3.data(2, 1) = -1 * Math.Sin((90 + CurvPosition(1)) * Math.PI / 180D)
            R3.data(2, 2) = Math.Cos((90 + CurvPosition(1)) * Math.PI / 180D)
            R3.data(2, 3) = 0D
            R3.data(3, 1) = 0D
            R3.data(3, 2) = 0D
            R3.data(3, 3) = 1D

            SkyPlot.satData = New Matrix(2, 3)
            SkyPlot.satData.data(1, 1) = ElevMaskNumericUpDown.Value
            SkyPlot.satData.data(2, 1) = CurvPosition(0)
            SkyPlot.satData.data(2, 2) = CurvPosition(1)
            SkyPlot.satData.data(2, 3) = CurvPosition(2)

            Dim increment As Integer = 1
            If ObsEpochsComboBox.Items.Count > MaxSkyPlotEpochs Then
                increment = ObsEpochsComboBox.Items.Count \ MaxSkyPlotEpochs
            End If
           
            If selectedButton.Name.ToUpper = "SKYPLOTALLBUTTON" Then
                startAt = ObsEpochsComboBox.SelectedIndex
                stopAt = ObsEpochsComboBox.SelectedIndex + EndObsEpochsComboBox.SelectedIndex
                SkyPlot.ONEorALL = 2
                If (stopAt - 1 \ increment) - startAt < 100 Then
                    ProgressBar1.Maximum = 100
                Else
                    ProgressBar1.Maximum = (stopAt - 1 \ increment) - startAt
                End If
                If stopAt - startAt = 0 Then
                    SkyPlot.ONEorALL = 1
                End If
            ElseIf selectedButton.Name.ToUpper = "SKYPLOTEPOCHBUTTON" Then
                startAt = ObsEpochsComboBox.SelectedIndex
                stopAt = startAt
                SkyPlot.ONEorALL = 1
                ProgressBar1.Maximum = 100
            End If

            ProgressBar1.Visible = True

            For q = startAt To stopAt Step increment
                If SkyPlot.ONEorALL = 2 Then
                    If ProgressBar1.Value <> ProgressBar1.Maximum Then
                        ProgressBar1.Value += 1
                        ProgressBar1.Update()
                    End If
                End If

                ReceptionTime(0) = SelectedObsFile.ObsData(0, 0, q)
                ReceptionTime(1) = SelectedObsFile.ObsData(0, 1, q)
                For i = 4 To SelectedObsFile.NumRows(q) - 1
                    If SkyPlot.ONEorALL = 1 Then
                        If ProgressBar1.Value <> ProgressBar1.Maximum Then
                            ProgressBar1.Value += 1
                            ProgressBar1.Update()
                        End If
                    End If
                    TimeDiffFromReference = 1000000000000.0
                    SatNum = SelectedObsFile.ObsData(i, 0, q)
                    If SatNum <> -9999I Then
                        Eph2Use = New RINEX_Eph
                        For j = 0 To SelectedNavFile.EphemerisV.GetLength(1) - 1
                            For k = 0 To SelectedNavFile.EphemerisV.GetLength(0) - 1
                                PRN = SelectedNavFile.EphemerisV(k, j).PRN
                                If SatNum = PRN Then
                                    EphemerisTime(0) = SelectedNavFile.EphemerisV(k, j).Toe_Week
                                    EphemerisTime(1) = SelectedNavFile.EphemerisV(k, j).Toe
                                    If Math.Abs((EphemerisTime(0) * 604800D + EphemerisTime(1)) - (ReceptionTime(0) * 604800D + ReceptionTime(1))) < TimeDiffFromReference Then
                                        TimeDiffFromReference = Math.Abs((EphemerisTime(0) * 604800D + EphemerisTime(1)) - (ReceptionTime(0) * 604800D + ReceptionTime(1)))
                                        Eph2Use = SelectedNavFile.EphemerisV(k, j)
                                    End If
                                End If
                            Next
                        Next

                        If Eph2Use.PRN <> -9999I Then

                            ECEF_XYZ_SatPosition = TempRINEX_Nav.ComputeSatellitePositionFromEphemeris(Sat_Clock_Corr, Rel_Corr, TGD_Corr, Eph2Use, ReceptionTime, 0, False, False, False, False)

                            If ECEF_XYZ_SatPosition(0) = 0 Then
                                SkyPlot.satData = SkyPlot.satData.matrixReDim(newRecord, 3, True)

                                Dim E_N_Up As Matrix
                                Dim diff As New Matrix(3, 1)

                                diff.data(1, 1) = ECEF_XYZ_SatPosition(1) - SelectedObsFile.Approx_X
                                diff.data(2, 1) = ECEF_XYZ_SatPosition(2) - SelectedObsFile.Approx_Y
                                diff.data(3, 1) = ECEF_XYZ_SatPosition(3) - SelectedObsFile.Approx_Z
                                E_N_Up = R1 * R3 * diff

                                Dim Elev, Azimuth As Decimal
                                Elev = (Math.Atan2(E_N_Up.data(3, 1), Math.Sqrt(E_N_Up.data(2, 1) ^ 2D + E_N_Up.data(1, 1) ^ 2D))) * 180D / Math.PI
                                Azimuth = (Math.Atan2(E_N_Up.data(1, 1), E_N_Up.data(2, 1))) * 180D / Math.PI

                                If Azimuth < 0 Then
                                    Azimuth += 360D
                                End If

                                SkyPlot.satData.data(newRecord, 1) = Eph2Use.PRN
                                SkyPlot.satData.data(newRecord, 2) = Azimuth
                                SkyPlot.satData.data(newRecord, 3) = Elev
                                newRecord += 1
                            Else
                                Exit Sub
                            End If
                        End If
                    End If
                Next
            Next

            If ProgressBar1.Value <> ProgressBar1.Maximum Then
                Dim indexUpdater As Integer
                For indexUpdater = ProgressBar1.Value To ProgressBar1.Maximum
                    ProgressBar1.Value = indexUpdater
                    ProgressBar1.Update()
                    System.Threading.Thread.Sleep(1)    'aesthetic to have the progress bar complete before plots are shown
                Next
            End If
            ProgressBar1.Visible = False
            ProgressBar1.Value = ProgressBar1.Minimum

            StatusLabel.Text = String.Empty
            Me.Refresh()
            SkyPlot.ShowDialog()
        Else
            MessageBox.Show("You must specify a Navigational RINEX file to use and click the get details first!", "Missing Nav Data", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub mainForm_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            serialPort2.Dispose()
            serialPort2.Close()
        Catch
        End Try
    End Sub

    Private Sub mainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        resultsGrid.AllowUserToAddRows = False
        resultsGrid.AllowUserToDeleteRows = False
        resultsGrid.AllowUserToOrderColumns = False

        Label16.Text &= "(" & Chr(176) & "C):"
        LatTextBox.Mask = "90" & Chr(176) & "90" & "'" & "90.99999" & ControlChars.Quote
        LongTextBox.Mask = "990" & Chr(176) & "90" & "'" & "90.99999" & ControlChars.Quote
        Label32.Text = String.Empty
        Label34.Text = String.Empty
        Label51.Text = String.Empty
        Label52.Text = String.Empty
        Label55.Text = String.Empty
        Label56.Text = String.Empty
        Label57.Text = String.Empty
        'LatTextBox.Text = "020422394769"
        'LongTextBox.Text = "156152526873"
        'HeightTextBox.Text = "3062.242"

        Me.Text &= " v" & My.Application.Info.Version.Major.ToString & "." & My.Application.Info.Version.Minor.ToString

        LocalLatTextBox.Mask = "90" & Chr(176) & "90" & "'" & "90.99999" & ControlChars.Quote
        LocalLongTextBox.Mask = "990" & Chr(176) & "90" & "'" & "90.99999" & ControlChars.Quote

        'garmin downloading Form Load actions
        For i As Integer = 0 To My.Computer.Ports.SerialPortNames.Count - 1
            cbbCOMPorts.Items.Add(My.Computer.Ports.SerialPortNames(i))
        Next
        GPS_Connected = False
        
        timeOfLastWrite = -9999
        timeOutErrorFlag = False
        ErrorFlag = False
        AlmanacDownload = False
        almanacCounter = 0

        hyperlink.Text = "Click here to download the current SEM Almanac file from the Official Civilian GPS Website"
        hyperlink.Location = New Point(12, 60)
        hyperlink.Size = New Size(435, 15)
        hyperlink.TabIndex = 0
        ToolTip1.SetToolTip(hyperlink, "http://www.navcen.uscg.gov/?pageName=currentAlmanac&format=sem")
        Me.TabPage4.Controls.Add(hyperlink)
        hyperlink.BringToFront()

        For i As Integer = 1 To 32
            PRNs2UseBoolean(i) = True   'ignore the zero element in this array it is not used
        Next
        UseUnhealthySatsBoolean = False

        stereoRadioButton.Checked = True
        EGHRadioButton.Checked = True
        GPSLocalBeenCalcd = False
        initLocalizationDetails(localDetailsSet)
        CommercialSoftwareSelection = 1
        'Me.TabControl1.SelectedIndex = 3
    End Sub

    Private Sub hyperlink_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles hyperlink.LinkClicked
        System.Diagnostics.Process.Start("http://www.navcen.uscg.gov/?pageName=currentAlmanac&format=sem")
    End Sub

    Private Sub PositioningCalcs(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PlotPositionsButton.Click, SatsDOPsButton.Click, HeightButton.Click, PlotClockOffsetButton.Click
        If NavDetailsGroupBox.Enabled = True Then
            Dim selectedButton As Button
            selectedButton = CType(sender, Button)

            TropWarning = False
            Dim ProcessResults(6) As Decimal
            Dim ProcessStdDevs(6) As String
            Dim DOPS(3) As Decimal
            Dim PositionData As New Matrix(1, 11)
            Dim newRecord As Integer = 1
            Dim i As Integer
            Dim Success As Boolean
            Dim startAt As Integer = ObsEpochsComboBox.SelectedIndex
            Dim stopAt As Integer = ObsEpochsComboBox.SelectedIndex + EndObsEpochsComboBox.SelectedIndex

            Dim increment As Integer = 1
            Dim Number2Process As Integer = stopAt - startAt + 1
            If Number2Process > MaxEpochs Then
                increment = Number2Process \ MaxEpochs
            End If

            If selectedButton.Name.ToUpper = "PLOTPOSITIONSBUTTON" Then
                StatusLabel.Text = "Please wait, generating positions plot..."
            ElseIf selectedButton.Name.ToUpper = "SATSDOPSBUTTON" Then
                StatusLabel.Text = "Please wait, generating DOPs plot..."
            ElseIf selectedButton.Name.ToUpper = "HEIGHTBUTTON" Then
                StatusLabel.Text = "Please wait, generating height plot..."
            ElseIf selectedButton.Name.ToUpper = "PLOTCLOCKOFFSETBUTTON" Then
                StatusLabel.Text = "Please wait, generating clock offset plot..."
            End If
            Me.Refresh()

            ProgressBar1.Visible = True
            ProgressBar1.Maximum = (stopAt \ increment)

            Dim EpochCartPosition(2) As Decimal
            Dim EpochCurvPosition(2) As Decimal
            
            For i = startAt To stopAt Step increment

                If ProgressBar1.Value <> ProgressBar1.Maximum Then
                    ProgressBar1.Value += 1
                    ProgressBar1.Update()
                End If

                Success = True
                CalcPositionFromALLEpochs(i, i, Success, ProcessResults, ProcessStdDevs, DOPS, 1, 1, True)
                If Success Then
                    PositionData = PositionData.matrixReDim(newRecord, 11, True)
                    PositionData.data(newRecord, 1) = SelectedObsFile.ObsData(0, 1, i)  'reception time GPS seconds
                    PositionData.data(newRecord, 2) = ProcessResults(0)                 'ECEF X coordinate
                    PositionData.data(newRecord, 3) = ProcessResults(1)                 'ECEF Y coordinate
                    PositionData.data(newRecord, 4) = ProcessResults(2)                 'ECEF Z coordinate
                    PositionData.data(newRecord, 5) = ProcessResults(6)                 'Receiver Clock Offset Range Error
                    PositionData.data(newRecord, 6) = DOPS(0)                           'Number of Satellites used in Solution (Above Elev Mask)
                    PositionData.data(newRecord, 7) = DOPS(1)                           'PDOP
                    PositionData.data(newRecord, 8) = DOPS(2)                           'HDOP
                    PositionData.data(newRecord, 9) = DOPS(3)                           'VDOP
                    PositionData.data(newRecord, 10) = SelectedObsFile.ObsData(0, 0, i) 'reception time GPS Weeks

                    EpochCartPosition(0) = ProcessResults(0)
                    EpochCartPosition(1) = ProcessResults(1)
                    EpochCartPosition(2) = ProcessResults(2)

                    EpochCurvPosition = cart2curv(EpochCartPosition)

                    PositionData.data(newRecord, 11) = EpochCurvPosition(2)             'ellipsoidal height coordinate

                    newRecord += 1
                End If
            Next

            If ProgressBar1.Value <> ProgressBar1.Maximum Then
                Dim indexUpdater As Integer
                For indexUpdater = ProgressBar1.Value To ProgressBar1.Maximum
                    ProgressBar1.Value = indexUpdater
                    ProgressBar1.Update()
                    System.Threading.Thread.Sleep(1)    'aesthetic to have the progress bar complete before plots are shown
                Next
            End If
            ProgressBar1.Visible = False
            ProgressBar1.Value = ProgressBar1.Minimum

            If selectedButton.Name.ToUpper = "PLOTPOSITIONSBUTTON" Or selectedButton.Name.ToUpper = "HEIGHTBUTTON" Then

                Dim X_Values, Y_Values, Z_Values, eh_Values As Matrix
                Dim MaxMinX(), MaxMinY(), MaxMinZ(), MaxMineh() As Decimal
                Dim MidrangeX, MidrangeY, MidrangeZ, Midrangeh As Decimal
                Dim BasePt, ControlPt As New Matrix(3, 1)

                ControlPt.data(1, 1) = -9999D

                X_Values = PositionData.getColumn(2)
                Y_Values = PositionData.getColumn(3)
                Z_Values = PositionData.getColumn(4)
                eh_Values = PositionData.getColumn(11)

                If BeenProcessed = True Then
                    X_Values = X_Values.matrixReDim(X_Values.nRows + 1, X_Values.nCols, True)
                    Y_Values = Y_Values.matrixReDim(Y_Values.nRows + 1, Y_Values.nCols, True)
                    Z_Values = Z_Values.matrixReDim(Z_Values.nRows + 1, Z_Values.nCols, True)
                    eh_Values = eh_Values.matrixReDim(eh_Values.nRows + 1, eh_Values.nCols, True)

                    X_Values.data(X_Values.nRows, X_Values.nCols) = ProcessedSolutionXYZ.data(1, 1)
                    Y_Values.data(Y_Values.nRows, Y_Values.nCols) = ProcessedSolutionXYZ.data(2, 1)
                    Z_Values.data(Z_Values.nRows, Z_Values.nCols) = ProcessedSolutionXYZ.data(3, 1)

                    EpochCartPosition(0) = ProcessedSolutionXYZ.data(1, 1)
                    EpochCartPosition(1) = ProcessedSolutionXYZ.data(2, 1)
                    EpochCartPosition(2) = ProcessedSolutionXYZ.data(3, 1)

                    EpochCurvPosition = cart2curv(EpochCartPosition)

                    eh_Values.data(eh_Values.nRows, eh_Values.nCols) = EpochCurvPosition(2)
                End If

                Dim Curv(2) As Decimal

                If ControlPtCheckBox.Checked Then

                    Dim dlatd, dlatm, dlats, dlongd, dlongm, dlongs, dheight As Decimal
                    Dim slatd As String = LatTextBox.Text.Substring(0, 2)
                    Dim slatm As String = LatTextBox.Text.Substring(3, 2)
                    Dim slats As String = LatTextBox.Text.Substring(6, 8)
                    Dim slongd As String = LongTextBox.Text.Substring(0, 3)
                    Dim slongm As String = LongTextBox.Text.Substring(4, 2)
                    Dim slongs As String = LongTextBox.Text.Substring(7, 8)

                    Try
                        dlatd = Decimal.Parse(slatd)
                    Catch ex As Exception
                        dlatd = 0D
                    End Try

                    Try
                        dlatm = Decimal.Parse(slatm)
                    Catch ex As Exception
                        dlatm = 0D
                    End Try

                    Try
                        dlats = Decimal.Parse(slats)
                    Catch ex As Exception
                        dlats = 0D
                    End Try

                    Try
                        dlongd = Decimal.Parse(slongd)
                    Catch ex As Exception
                        dlongd = 0D
                    End Try

                    Try
                        dlongm = Decimal.Parse(slongm)
                    Catch ex As Exception
                        dlongm = 0D
                    End Try

                    Try
                        dlongs = Decimal.Parse(slongs)
                    Catch ex As Exception
                        dlongs = 0D
                    End Try

                    Try
                        dheight = Decimal.Parse(HeightTextBox.Text)
                    Catch ex As Exception
                        dheight = 0D
                    End Try

                    Curv(0) = dlatd + dlatm / 60D + dlats / 3600D
                    Curv(1) = dlongd + dlongm / 60D + dlongs / 3600D
                    Curv(2) = dheight

                    If LatSCheckBox.Checked Then
                        Curv(0) *= -1
                    End If

                    If LongWCheckBox.Checked Then
                        Curv(1) *= -1
                    End If

                    Dim CartPos() As Decimal = curv2cart(Curv)

                    ControlPt.data(1, 1) = CartPos(0)
                    ControlPt.data(2, 1) = CartPos(1)
                    ControlPt.data(3, 1) = CartPos(2)

                    If ControlPtForceCheckBox.Checked Then
                        X_Values = X_Values.matrixReDim(X_Values.nRows + 1, X_Values.nCols, True)
                        Y_Values = Y_Values.matrixReDim(Y_Values.nRows + 1, Y_Values.nCols, True)
                        Z_Values = Z_Values.matrixReDim(Z_Values.nRows + 1, Z_Values.nCols, True)
                        eh_Values = eh_Values.matrixReDim(eh_Values.nRows + 1, eh_Values.nCols, True)

                        X_Values.data(X_Values.nRows, X_Values.nCols) = ControlPt.data(1, 1)
                        Y_Values.data(Y_Values.nRows, Y_Values.nCols) = ControlPt.data(2, 1)
                        Z_Values.data(Z_Values.nRows, Z_Values.nCols) = ControlPt.data(3, 1)
                        eh_Values.data(eh_Values.nRows, eh_Values.nCols) = Curv(2)
                    End If
                End If

                MaxMinX = ComputeMaxMin(X_Values)
                MaxMinY = ComputeMaxMin(Y_Values)
                MaxMinZ = ComputeMaxMin(Z_Values)
                MaxMineh = ComputeMaxMin(eh_Values)

                MidrangeX = (MaxMinX(0) - MaxMinX(1)) / 2D
                MidrangeY = (MaxMinY(0) - MaxMinY(1)) / 2D
                MidrangeZ = (MaxMinZ(0) - MaxMinZ(1)) / 2D
                Midrangeh = (MaxMineh(0) - MaxMineh(1)) / 2D

                BasePt.data(1, 1) = MaxMinX(1) + MidrangeX
                BasePt.data(2, 1) = MaxMinY(1) + MidrangeY
                BasePt.data(3, 1) = MaxMinZ(1) + MidrangeZ

                If ControlPtCheckBox.Checked And ControlPtForceCheckBox.Checked = False Then
                    X_Values = X_Values.matrixReDim(X_Values.nRows + 1, X_Values.nCols, True)
                    Y_Values = Y_Values.matrixReDim(Y_Values.nRows + 1, Y_Values.nCols, True)
                    Z_Values = Z_Values.matrixReDim(Z_Values.nRows + 1, Z_Values.nCols, True)

                    X_Values.data(X_Values.nRows, X_Values.nCols) = ControlPt.data(1, 1)
                    Y_Values.data(Y_Values.nRows, Y_Values.nCols) = ControlPt.data(2, 1)
                    Z_Values.data(Z_Values.nRows, Z_Values.nCols) = ControlPt.data(3, 1)
                End If

                Dim Cart(2), CurvPosition() As Decimal
                Cart(0) = BasePt.data(1, 1)
                Cart(1) = BasePt.data(2, 1)
                Cart(2) = BasePt.data(3, 1)
                CurvPosition = cart2curv(Cart)

                Dim R1, R3 As New Matrix(3, 3)
                R1.data(1, 1) = 1D
                R1.data(1, 2) = 0D
                R1.data(1, 3) = 0D
                R1.data(2, 1) = 0D
                R1.data(2, 2) = Math.Cos((90 - CurvPosition(0)) * Math.PI / 180D)
                R1.data(2, 3) = Math.Sin((90 - CurvPosition(0)) * Math.PI / 180D)
                R1.data(3, 1) = 0D
                R1.data(3, 2) = -1 * Math.Sin((90 - CurvPosition(0)) * Math.PI / 180D)
                R1.data(3, 3) = Math.Cos((90 - CurvPosition(0)) * Math.PI / 180D)

                R3.data(1, 1) = Math.Cos((90 + CurvPosition(1)) * Math.PI / 180D)
                R3.data(1, 2) = Math.Sin((90 + CurvPosition(1)) * Math.PI / 180D)
                R3.data(1, 3) = 0D
                R3.data(2, 1) = -1 * Math.Sin((90 + CurvPosition(1)) * Math.PI / 180D)
                R3.data(2, 2) = Math.Cos((90 + CurvPosition(1)) * Math.PI / 180D)
                R3.data(2, 3) = 0D
                R3.data(3, 1) = 0D
                R3.data(3, 2) = 0D
                R3.data(3, 3) = 1D

                If ControlPtCheckBox.Checked Then
                    ControlPt = ControlPt - BasePt
                    ControlPt = R1 * R3 * ControlPt
                End If

                If BeenProcessed = True Then
                    ProcessedSolution = ProcessedSolutionXYZ - BasePt
                    ProcessedSolution = R1 * R3 * ProcessedSolution

                    EpochCartPosition(0) = ProcessedSolutionXYZ.data(1, 1)
                    EpochCartPosition(1) = ProcessedSolutionXYZ.data(2, 1)
                    EpochCartPosition(2) = ProcessedSolutionXYZ.data(3, 1)

                    EpochCurvPosition = cart2curv(EpochCartPosition)
                    ProcessedSolution.data(3, 1) = EpochCurvPosition(2)
                End If

                Dim EpochPosition As New Matrix(3, 1)
                Dim AllEpochPositions As New Matrix(X_Values.nRows + 4, 3) 'PositionData.nRows + 4, 3)
                Dim StopRow As Integer = X_Values.nRows

                If ControlPtCheckBox.Checked And ControlPtForceCheckBox.Checked = False Then
                    StopRow = X_Values.nRows - 1
                End If
                'Dim coordstring As String = String.Empty
                For i = 1 To StopRow
                    EpochPosition.data(1, 1) = X_Values.data(i, 1) 'PositionData.data(i, 2)
                    EpochPosition.data(2, 1) = Y_Values.data(i, 1) 'PositionData.data(i, 3)
                    EpochPosition.data(3, 1) = Z_Values.data(i, 1) 'PositionData.data(i, 4)

                    EpochPosition = EpochPosition - BasePt
                    EpochPosition = R1 * R3 * EpochPosition
                    'ENU coordinate differences from BasePt
                    AllEpochPositions.data(i, 1) = EpochPosition.data(1, 1)
                    AllEpochPositions.data(i, 2) = EpochPosition.data(2, 1)
                    AllEpochPositions.data(i, 3) = EpochPosition.data(3, 1)
                    'coordstring &= "N: " & EpochPosition.data(2, 1) & "E: " & EpochPosition.data(1, 1) & vbNewLine
                Next
                'MessageBox.Show(coordstring)
                Dim E_Values, N_Values, Up_Values As Matrix
                Dim MaxMinE(), MaxMinN(), MaxMinUp() As Decimal
                Dim MidrangeE, MidrangeN, MidrangeUp As Decimal

                E_Values = AllEpochPositions.getColumn(1)
                N_Values = AllEpochPositions.getColumn(2)
                Up_Values = AllEpochPositions.getColumn(3)

                MaxMinE = ComputeMaxMin(E_Values)
                MaxMinN = ComputeMaxMin(N_Values)
                MaxMinUp = ComputeMaxMin(Up_Values)

                MidrangeE = (MaxMinE(0) - MaxMinE(1)) / 2
                MidrangeN = (MaxMinN(0) - MaxMinN(1)) / 2
                MidrangeUp = (MaxMinUp(0) - MaxMinUp(1)) / 2

                BasePt.data(1, 1) = MaxMinE(1) + MidrangeE
                BasePt.data(2, 1) = MaxMinN(1) + MidrangeN
                BasePt.data(3, 1) = MaxMinUp(1) + MidrangeUp

                If ControlPtCheckBox.Checked Then
                    AllEpochPositions.data(AllEpochPositions.nRows - 3, 1) = ControlPt.data(1, 1)
                    AllEpochPositions.data(AllEpochPositions.nRows - 3, 2) = ControlPt.data(2, 1)
                    AllEpochPositions.data(AllEpochPositions.nRows - 3, 3) = ControlPt.data(3, 1)
                    If ControlPtForceCheckBox.Checked Then
                        'Optional Control Point to be drawn on plot (if coordinates known and provided)
                        AllEpochPositions.data(AllEpochPositions.nRows - 3, 1) = EpochPosition.data(1, 1)
                        AllEpochPositions.data(AllEpochPositions.nRows - 3, 2) = EpochPosition.data(2, 1)
                        AllEpochPositions.data(AllEpochPositions.nRows - 3, 3) = EpochPosition.data(3, 1)
                    End If
                Else
                    AllEpochPositions.data(AllEpochPositions.nRows - 3, 1) = -9999D
                End If

                'Max and Min Northing Values
                AllEpochPositions.data(AllEpochPositions.nRows - 2, 1) = MaxMinN(0)
                AllEpochPositions.data(AllEpochPositions.nRows - 2, 2) = MaxMinN(1)

                'Max and Min Easting Values
                AllEpochPositions.data(AllEpochPositions.nRows - 1, 1) = MaxMinE(0)
                AllEpochPositions.data(AllEpochPositions.nRows - 1, 2) = MaxMinE(1)

                'Base Point Coordinates
                AllEpochPositions.data(AllEpochPositions.nRows, 1) = BasePt.data(1, 1)
                AllEpochPositions.data(AllEpochPositions.nRows, 2) = BasePt.data(2, 1)
                AllEpochPositions.data(AllEpochPositions.nRows, 3) = BasePt.data(3, 1)

                If selectedButton.Name.ToUpper = "PLOTPOSITIONSBUTTON" Then
                    PositionsPlot.EpochPositions = AllEpochPositions
                    PositionsPlot.CurvPosition(0) = CurvPosition(0)
                    PositionsPlot.CurvPosition(1) = CurvPosition(1)
                    StatusLabel.Text = String.Empty
                    Me.Refresh()
                    PositionsPlot.ShowDialog()
                Else
                    Dim AllEpochHeights As New Matrix(PositionData.nRows, 3)

                    For i = 1 To PositionData.nRows
                        AllEpochHeights.data(i, 1) = PositionData.data(i, 1)
                        AllEpochHeights.data(i, 3) = PositionData.data(i, 10)
                        AllEpochHeights.data(i, 2) = PositionData.data(i, 11) 'AllEpochPositions.data(i, 3)
                    Next

                    If ControlPtForceCheckBox.Checked And ControlPtCheckBox.Checked Then
                        AllEpochHeights = AllEpochHeights.matrixReDim(AllEpochHeights.nRows + 1, AllEpochHeights.nCols, True)
                        AllEpochHeights.data(AllEpochHeights.nRows, 1) = 9999D   'yes flag
                        AllEpochHeights.data(AllEpochHeights.nRows, 2) = Curv(2) 'ControlPt.data(3, 1)
                    End If

                    Dim Time_Values, H_Values, Week_Values As Matrix
                    Dim MaxMinTime(,), MaxMinH() As Decimal

                    Time_Values = AllEpochHeights.getColumn(1)
                    Week_Values = AllEpochHeights.getColumn(3)
                    If ControlPtForceCheckBox.Checked And ControlPtCheckBox.Checked Then
                        Time_Values = Time_Values.matrixReDim(Time_Values.nRows - 1, 1, True)
                    End If
                    H_Values = AllEpochHeights.getColumn(2)

                    MaxMinTime = ComputeMaxMinTime(Time_Values, Week_Values)
                    MaxMinH = ComputeMaxMin(H_Values)

                    If ControlPtForceCheckBox.Checked = False And ControlPtCheckBox.Checked Then
                        AllEpochHeights = AllEpochHeights.matrixReDim(AllEpochHeights.nRows + 1, AllEpochHeights.nCols, True)
                        AllEpochHeights.data(AllEpochHeights.nRows, 1) = 9999D   'yes flag
                        AllEpochHeights.data(AllEpochHeights.nRows, 2) = Curv(2) 'ControlPt.data(3, 1)
                    ElseIf ControlPtForceCheckBox.Checked = False And ControlPtCheckBox.Checked = False Then
                        AllEpochHeights = AllEpochHeights.matrixReDim(AllEpochHeights.nRows + 1, AllEpochHeights.nCols, True)
                        AllEpochHeights.data(AllEpochHeights.nRows, 1) = -9999D   'yes flag
                        AllEpochHeights.data(AllEpochHeights.nRows, 2) = Curv(2) 'ControlPt.data(3, 1)
                    End If
                    AllEpochHeights = AllEpochHeights.matrixReDim(AllEpochHeights.nRows + 3, AllEpochHeights.nCols, True)

                    AllEpochHeights.data(AllEpochHeights.nRows - 2, 1) = MaxMinTime(1, 0)
                    AllEpochHeights.data(AllEpochHeights.nRows - 2, 2) = MaxMinTime(1, 1)

                    AllEpochHeights.data(AllEpochHeights.nRows - 1, 1) = MaxMinTime(0, 0)
                    AllEpochHeights.data(AllEpochHeights.nRows - 1, 2) = MaxMinTime(0, 1)

                    AllEpochHeights.data(AllEpochHeights.nRows, 1) = MaxMinH(0)
                    AllEpochHeights.data(AllEpochHeights.nRows, 2) = MaxMinH(1)

                    HeightPlot.Heights = AllEpochHeights
                    HeightPlot.elevAngle = ElevMaskNumericUpDown.Value
                    HeightPlot.CentralHeight = MaxMineh(1) + Midrangeh 'CurvPosition(2)
                    StatusLabel.Text = String.Empty
                    Me.Refresh()
                    HeightPlot.ShowDialog()
                End If

            ElseIf selectedButton.Name.ToUpper = "SATSDOPSBUTTON" Then

                Dim AllEpochDOPS As New Matrix(PositionData.nRows + 6, 6)

                For i = 1 To PositionData.nRows
                    AllEpochDOPS.data(i, 1) = PositionData.data(i, 1)
                    AllEpochDOPS.data(i, 2) = PositionData.data(i, 6)
                    AllEpochDOPS.data(i, 3) = PositionData.data(i, 7)
                    AllEpochDOPS.data(i, 4) = PositionData.data(i, 8)
                    AllEpochDOPS.data(i, 5) = PositionData.data(i, 9)
                    AllEpochDOPS.data(i, 6) = PositionData.data(i, 10)
                Next

                Dim Time_Values, Week_Values, Sat_Values, PDOP_Values, HDOP_Values, VDOP_Values As Matrix
                Dim MaxMinTime(,), MaxMinSat(), MaxMinPDOP(), MaxMinHDOP(), MaxMinVDOP() As Decimal

                Time_Values = PositionData.getColumn(1)
                Week_Values = PositionData.getColumn(10)
                Sat_Values = PositionData.getColumn(6)
                PDOP_Values = PositionData.getColumn(7)
                HDOP_Values = PositionData.getColumn(8)
                VDOP_Values = PositionData.getColumn(9)

                MaxMinTime = ComputeMaxMinTime(Time_Values, Week_Values)
                MaxMinSat = ComputeMaxMin(Sat_Values)
                MaxMinPDOP = ComputeMaxMin(PDOP_Values)
                MaxMinHDOP = ComputeMaxMin(HDOP_Values)
                MaxMinVDOP = ComputeMaxMin(VDOP_Values)

                AllEpochDOPS.data(PositionData.nRows + 1, 1) = MaxMinTime(1, 0)
                AllEpochDOPS.data(PositionData.nRows + 1, 2) = MaxMinTime(1, 1)

                AllEpochDOPS.data(PositionData.nRows + 2, 1) = MaxMinTime(0, 0)
                AllEpochDOPS.data(PositionData.nRows + 2, 2) = MaxMinTime(0, 1)

                AllEpochDOPS.data(PositionData.nRows + 3, 1) = MaxMinSat(0)
                AllEpochDOPS.data(PositionData.nRows + 3, 2) = MaxMinSat(1)

                AllEpochDOPS.data(PositionData.nRows + 4, 1) = MaxMinPDOP(0)
                AllEpochDOPS.data(PositionData.nRows + 4, 2) = MaxMinPDOP(1)

                AllEpochDOPS.data(PositionData.nRows + 5, 1) = MaxMinHDOP(0)
                AllEpochDOPS.data(PositionData.nRows + 5, 2) = MaxMinHDOP(1)

                AllEpochDOPS.data(PositionData.nRows + 6, 1) = MaxMinVDOP(0)
                AllEpochDOPS.data(PositionData.nRows + 6, 2) = MaxMinVDOP(1)

                DOPsPlot.DOP_Data = AllEpochDOPS
                DOPsPlot.elevAngle = ElevMaskNumericUpDown.Value

                StatusLabel.Text = String.Empty
                Me.Refresh()
                DOPsPlot.ShowDialog()

            ElseIf selectedButton.Name.ToUpper = "PLOTCLOCKOFFSETBUTTON" Then

                Dim AllEpochClockOffsets As New Matrix(PositionData.nRows + 3, 3)

                For i = 1 To PositionData.nRows
                    AllEpochClockOffsets.data(i, 1) = PositionData.data(i, 1)
                    AllEpochClockOffsets.data(i, 2) = (1000D / 299792458D) * PositionData.data(i, 5)    'millisecond units
                    AllEpochClockOffsets.data(i, 3) = PositionData.data(i, 10)
                Next

                Dim Time_Values, Week_Values, ClockOffset_Values As Matrix
                Dim MaxMinTime(,), MaxMinClockOffset() As Decimal

                Time_Values = PositionData.getColumn(1)
                Week_Values = PositionData.getColumn(10)
                ClockOffset_Values = (1000D / 299792458D) * PositionData.getColumn(5) 'millisecond units

                MaxMinTime = ComputeMaxMinTime(Time_Values, Week_Values)
                MaxMinClockOffset = ComputeMaxMin(ClockOffset_Values)

                AllEpochClockOffsets.data(PositionData.nRows + 1, 1) = MaxMinTime(1, 0)
                AllEpochClockOffsets.data(PositionData.nRows + 1, 2) = MaxMinTime(1, 1)

                AllEpochClockOffsets.data(PositionData.nRows + 2, 1) = MaxMinTime(0, 0)
                AllEpochClockOffsets.data(PositionData.nRows + 2, 2) = MaxMinTime(0, 1)

                AllEpochClockOffsets.data(PositionData.nRows + 3, 1) = MaxMinClockOffset(0)
                AllEpochClockOffsets.data(PositionData.nRows + 3, 2) = MaxMinClockOffset(1)

                ClockOffsetPlot.ClockOffsets = AllEpochClockOffsets

                StatusLabel.Text = String.Empty
                Me.Refresh()
                ClockOffsetPlot.ShowDialog()
            End If
        Else
            MessageBox.Show("You must specify a Navigational RINEX file to use and click the get details first!", "Missing Nav Data", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Function ComputeMaxMin(ByVal values As Matrix) As Decimal()
        Dim MaxMin(1) As Decimal
        Dim i, j As Integer
        MaxMin(0) = values.data(1, 1)
        MaxMin(1) = values.data(1, 1)

        For i = 1 To values.nRows
            For j = 1 To values.nCols
                If values.data(i, j) <> -999999999.999999999D Then
                    If values.data(i, j) > MaxMin(0) Then
                        MaxMin(0) = values.data(i, j)
                    End If
                    If values.data(i, j) < MaxMin(1) Then
                        MaxMin(1) = values.data(i, j)
                    End If
                End If
            Next
        Next

        If MaxMin(1) = -999999999.999999999D Then
            MaxMin(1) = MaxMin(0)
            For i = 1 To values.nRows
                For j = 1 To values.nCols
                    If values.data(i, j) <> -999999999.999999999D Then
                        If values.data(i, j) > MaxMin(0) Then
                            MaxMin(0) = values.data(i, j)
                        End If
                        If values.data(i, j) < MaxMin(1) Then
                            MaxMin(1) = values.data(i, j)
                        End If
                    End If
                Next
            Next
        End If
        Return MaxMin
    End Function

    Private Function ComputeMaxMinTime(ByVal Sec_values As Matrix, ByVal Week_values As Matrix) As Decimal(,)
        Dim MaxMin(1) As Decimal
        Dim MaxMinWeek(1) As Decimal

        Dim i, j As Integer
        MaxMin(0) = Sec_values.data(1, 1)
        MaxMin(1) = Sec_values.data(1, 1)

        MaxMinWeek(0) = Week_values.data(1, 1)
        MaxMinWeek(1) = Week_values.data(1, 1)

        Dim totalMaxSeconds As Decimal = Decimal.Round(MaxMinWeek(0) * 604800D, 0) + MaxMin(0)
        Dim totalMinSeconds As Decimal = totalMaxSeconds

        For i = 1 To Sec_values.nRows
            For j = 1 To Sec_values.nCols

                Dim totalCurrentSeconds As Decimal = Decimal.Round(Week_values.data(i, j) * 604800D, 0) + Sec_values.data(i, j)

                If totalCurrentSeconds > totalMaxSeconds Then
                    MaxMin(0) = Sec_values.data(i, j)
                    MaxMinWeek(0) = Week_values.data(i, j)
                    totalMaxSeconds = totalCurrentSeconds
                End If
                If totalCurrentSeconds < totalMinSeconds Then
                    MaxMin(1) = Sec_values.data(i, j)
                    MaxMinWeek(1) = Week_values.data(i, j)
                    totalMinSeconds = totalCurrentSeconds
                End If
            Next
        Next

        Dim returns(1, 1) As Decimal

        returns(0, 0) = MaxMin(0)
        returns(0, 1) = MaxMin(1)
        returns(1, 0) = MaxMinWeek(0)
        returns(1, 1) = MaxMinWeek(1)

        Return returns
    End Function

    Private Sub ControlPtCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ControlPtCheckBox.CheckedChanged
        Label19.Enabled = ControlPtCheckBox.Checked
        Label20.Enabled = ControlPtCheckBox.Checked
        Label21.Enabled = ControlPtCheckBox.Checked
        Label22.Enabled = ControlPtCheckBox.Checked
        datumGroupBox.Enabled = ControlPtCheckBox.Checked
        LatTextBox.Enabled = ControlPtCheckBox.Checked
        LongTextBox.Enabled = ControlPtCheckBox.Checked
        HeightTextBox.Enabled = ControlPtCheckBox.Checked
        LatNCheckBox.Enabled = ControlPtCheckBox.Checked
        LatSCheckBox.Enabled = ControlPtCheckBox.Checked
        LongECheckBox.Enabled = ControlPtCheckBox.Checked
        LongWCheckBox.Enabled = ControlPtCheckBox.Checked
        ControlPtForceCheckBox.Enabled = ControlPtCheckBox.Checked
    End Sub

    Private Sub LatSCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LatSCheckBox.CheckedChanged
        If LatNCheckBox.Checked = True And LatSCheckBox.Checked = True Then
            LatNCheckBox.Checked = False
        End If
        If LatSCheckBox.Checked = False And LatNCheckBox.Checked = False Then
            LatSCheckBox.Checked = True
        End If
    End Sub

    Private Sub LatNCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LatNCheckBox.CheckedChanged
        If LatNCheckBox.Checked = True And LatSCheckBox.Checked = True Then
            LatSCheckBox.Checked = False
        End If
        If LatSCheckBox.Checked = False And LatNCheckBox.Checked = False Then
            LatNCheckBox.Checked = True
        End If
    End Sub

    Private Sub LongECheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LongECheckBox.CheckedChanged
        If LongECheckBox.Checked = True And LongWCheckBox.Checked = True Then
            LongWCheckBox.Checked = False
        End If
        If LongWCheckBox.Checked = False And LongECheckBox.Checked = False Then
            LongECheckBox.Checked = True
        End If
    End Sub

    Private Sub LongWCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LongWCheckBox.CheckedChanged
        If LongECheckBox.Checked = True And LongWCheckBox.Checked = True Then
            LongECheckBox.Checked = False
        End If
        If LongWCheckBox.Checked = False And LongECheckBox.Checked = False Then
            LongWCheckBox.Checked = True
        End If
    End Sub

    Private Sub TabPage2_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles TabPage2.MouseClick
        Dim dist2Centre As Decimal
        Dim elev, azimuth As Decimal
        Dim X, Y As Decimal

        dist2Centre = Math.Sqrt((e.Location.X - 300) ^ 2 + (225 - e.Location.Y) ^ 2)

        If dist2Centre <= 200D Then
            X = e.Location.X - 300
            Y = 225 - e.Location.Y
            elev = 90 - (dist2Centre / 200D * 90)
            azimuth = Math.Atan2((e.Location.X - 300), (225 - e.Location.Y))
            azimuth *= 180D / Math.PI
            If azimuth < 0 Then
                azimuth += 360
            End If

            SatsAdded = SatsAdded.matrixReDim(SatsAdded.nRows + 1, 5, True)
            SatsAdded.data(SatsAdded.nRows, 1) = e.Location.X
            SatsAdded.data(SatsAdded.nRows, 2) = e.Location.Y
            SatsAdded.data(SatsAdded.nRows, 3) = Math.Cos(elev * Math.PI / 180D) * Math.Sin(azimuth * Math.PI / 180D) * -1
            SatsAdded.data(SatsAdded.nRows, 4) = Math.Cos(elev * Math.PI / 180D) * Math.Cos(azimuth * Math.PI / 180D) * -1
            SatsAdded.data(SatsAdded.nRows, 5) = Math.Sin(elev * Math.PI / 180D) * -1
            DrawSats = True
            Me.Refresh()
        End If
    End Sub

    Private Sub TabPage2_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles TabPage2.Paint
        Dim ScreenBasePt(1) As Integer
        Dim drawingAreaSize As Integer
        Dim thinBlackPen As New Pen(Color.Black, 1)
        Dim seperation As Integer
        Dim dashspacing As Integer
        Dim SatPen As New Pen(Color.Red, 15)

        Dim i As Integer

        ScreenBasePt(0) = 100
        ScreenBasePt(1) = 25
        drawingAreaSize = 400
        dashspacing = 8

        seperation = drawingAreaSize \ 18

        With e.Graphics
            For i = 0 To 8
                .DrawEllipse(thinBlackPen, ScreenBasePt(0) + i * seperation, ScreenBasePt(1) + i * seperation, drawingAreaSize - 2 * i * seperation, drawingAreaSize - 2 * i * seperation)
            Next

            For i = ScreenBasePt(0) To ScreenBasePt(0) + drawingAreaSize Step dashspacing
                .DrawEllipse(thinBlackPen, i, ScreenBasePt(1) + drawingAreaSize \ 2, 1, 1)
            Next

            For i = ScreenBasePt(1) To ScreenBasePt(1) + drawingAreaSize Step dashspacing
                .DrawEllipse(thinBlackPen, ScreenBasePt(0) + drawingAreaSize \ 2, i, 1, 1)
            Next

            If DrawSats = True Then
                Label27.Text = "# of Satellites: " & (SatsAdded.nRows - 1).ToString
                Dim A As New Matrix(SatsAdded.nRows - 1, 4)
                For i = 2 To SatsAdded.nRows
                    .DrawEllipse(SatPen, SatsAdded.data(i, 1), SatsAdded.data(i, 2), 1, 1)
                    A.data(i - 1, 1) = SatsAdded.data(i, 3)
                    A.data(i - 1, 2) = SatsAdded.data(i, 4)
                    A.data(i - 1, 3) = SatsAdded.data(i, 5)
                    A.data(i - 1, 4) = 1
                Next
                If A.nRows >= 4 Then
                    Dim Qx As Matrix = (A.Transpose * A).Inverse
                    Qx = Qx.getDiagonal
                    Dim sqrtD As Decimal
                    Dim DOPstring As String

                    sqrtD = Math.Sqrt(Qx.data(1, 1) + Qx.data(2, 1) + Qx.data(3, 1))
                    DOPstring = Decimal.Round(sqrtD, 2).ToString
                    Label28.Text = "PDOP: " & DOPstring

                    sqrtD = Math.Sqrt(Qx.data(1, 1) + Qx.data(2, 1))
                    DOPstring = Decimal.Round(sqrtD, 2).ToString
                    Label29.Text = "HDOP: " & DOPstring

                    sqrtD = Math.Sqrt(Qx.data(3, 1))
                    DOPstring = Decimal.Round(sqrtD, 2).ToString
                    Label30.Text = "VDOP: " & DOPstring
                Else
                    Label28.Text = "PDOP: N/A"
                    Label29.Text = "HDOP: N/A"
                    Label30.Text = "VDOP: N/A"
                End If

            End If
        End With
    End Sub

    Private Sub ClearButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClearButton.Click
        SatsAdded = New Matrix(1, 5)
        DrawSats = False
        Label27.Text = "# of Satellites: 0"
        Label28.Text = "PDOP: N/A"
        Label29.Text = "HDOP: N/A"
        Label30.Text = "VDOP: N/A"
        Me.Refresh()
    End Sub

    Private Sub TabPage2_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabPage2.Enter
        Label23.Text = "0" & Chr(176) & "(North)"
        Label24.Text = "90" & Chr(176)
        Label25.Text = "180" & Chr(176)
        Label26.Text = "270" & Chr(176)
    End Sub

    Private Sub RemoveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveButton.Click
        If SatsAdded.nRows <= 2 Then
            ClearButton_Click(sender, e)
        Else
            SatsAdded = SatsAdded.matrixReDim(SatsAdded.nRows - 1, 5, True)
            Me.Refresh()
        End If
    End Sub

    Private Sub SatClockCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SatClockCheckBox.CheckedChanged
        RelativisticCheckBox.Checked = SatClockCheckBox.Checked
        TGDCheckBox.Checked = SatClockCheckBox.Checked
        RelativisticCheckBox.Enabled = SatClockCheckBox.Checked
        TGDCheckBox.Enabled = SatClockCheckBox.Checked
    End Sub

    Private Sub geoInputButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles geoImportButton.Click
        PointList.DefaultCellStyle.ForeColor = Color.Black
        Dim myDialogResult As DialogResult = OpenFileDialog1.ShowDialog()

        If myDialogResult <> Windows.Forms.DialogResult.Cancel Then
            geoInputTextBox.Text = OpenFileDialog1.FileName
            'importButton.Enabled = True
            Call importPoints(OpenFileDialog1.FilterIndex)
        End If
    End Sub

    Private Sub geoAddButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles geoAddButton.Click
        PointList.DefaultCellStyle.ForeColor = Color.Black

        addPoint.ShowDialog()

        If addPoint.GeodeticTest <> False Then

            Dim numPoints As Integer = PointList.Rows.Count
            Dim GPSLocalPt As New GPS_Local_Pt
            initGPSLocalPt(GPSLocalPt)

            GPSLocalPt.ptNum = addPoint.PtNum2Add
            GPSLocalPt.desc = addPoint.Desc2Add
            GPSLocalPt.latitude = addPoint.Lat2Add
            GPSLocalPt.longitude = addPoint.Long2Add
            GPSLocalPt.height = addPoint.Height2Add

            If numPoints = 0 Then
                GPSLocalPts(0).ptNum = GPSLocalPt.ptNum
                GPSLocalPts(0).latitude = GPSLocalPt.latitude
                GPSLocalPts(0).longitude = GPSLocalPt.longitude
                GPSLocalPts(0).height = GPSLocalPt.height
                GPSLocalPts(0).desc = GPSLocalPt.desc
                numPoints = 1
            Else
                ReDim Preserve GPSLocalPts(numPoints)
                GPSLocalPts(numPoints).ptNum = GPSLocalPt.ptNum
                GPSLocalPts(numPoints).latitude = GPSLocalPt.latitude
                GPSLocalPts(numPoints).longitude = GPSLocalPt.longitude
                GPSLocalPts(numPoints).height = GPSLocalPt.height
                GPSLocalPts(numPoints).desc = GPSLocalPt.desc
                numPoints += 1
            End If

            Dim latString As String = DecToDMS(GPSLocalPt.latitude, 1)
            Dim longString As String = DecToDMS(GPSLocalPt.longitude, 2)

            Dim geoData() As String = {GPSLocalPt.ptNum.ToString, GPSLocalPt.desc, "", latString, longString, (Decimal.Round(GPSLocalPt.height, 3)).ToString}
            PointList.Rows.Add(geoData)

            If numPoints > 0 Then
                getGeodeticButton.Enabled = True
                ComputeButton.Enabled = True
                PointList.ClearSelection()
            End If

            If GPSLocalBeenCalcd = True Then
                Call computeLocalPoints()
            End If

        End If
    End Sub

    Private Sub importPoints(ByVal filterIndex As Integer)
        Dim geoPoints As New StreamReader(geoInputTextBox.Text)
        Dim numPoints As Integer = PointList.Rows.Count

        'GPSLocalBeenCalcd = False
        If AppendCheckBox.Checked = False Then
            PointList.Rows.Clear()
            numPoints = 0
        End If

        Dim GPSLocalPt As New GPS_Local_Pt
        initGPSLocalPt(GPSLocalPt)

        Dim readGeoLine As String = String.Empty
        Dim testForImportError As Boolean = False

        While geoPoints.Peek <> -1
            readGeoLine = geoPoints.ReadLine
            Try
                GPSLocalPt.ptNum = readGeoLine.Substring(0, readGeoLine.IndexOf(","))
                readGeoLine = readGeoLine.Substring(readGeoLine.IndexOf(",") + 1, readGeoLine.Length - (readGeoLine.IndexOf(",") + 1))

                If filterIndex = 2 Then
                    GPSLocalPt.latitude = ParseAndConvertDEC2DMS(readGeoLine)
                Else
                    GPSLocalPt.latitude = Convert.ToDecimal(readGeoLine.Substring(0, readGeoLine.IndexOf(",")))
                End If

                readGeoLine = readGeoLine.Substring(readGeoLine.IndexOf(",") + 1, readGeoLine.Length - (readGeoLine.IndexOf(",") + 1))

                If filterIndex = 2 Then
                    GPSLocalPt.longitude = ParseAndConvertDEC2DMS(readGeoLine)
                Else
                    GPSLocalPt.longitude = Convert.ToDecimal(readGeoLine.Substring(0, readGeoLine.IndexOf(",")))
                End If

                readGeoLine = readGeoLine.Substring(readGeoLine.IndexOf(",") + 1, readGeoLine.Length - (readGeoLine.IndexOf(",") + 1))
                GPSLocalPt.height = Convert.ToDecimal(readGeoLine.Substring(0, readGeoLine.IndexOf(",")))
                readGeoLine = readGeoLine.Substring(readGeoLine.IndexOf(",") + 1, readGeoLine.Length - (readGeoLine.IndexOf(",") + 1))
                GPSLocalPt.desc = readGeoLine.Substring(0, readGeoLine.Length)

                If numPoints = 0 Then
                    GPSLocalPts(0).ptNum = GPSLocalPt.ptNum
                    GPSLocalPts(0).latitude = GPSLocalPt.latitude
                    GPSLocalPts(0).longitude = GPSLocalPt.longitude
                    GPSLocalPts(0).height = GPSLocalPt.height
                    GPSLocalPts(0).desc = GPSLocalPt.desc
                    numPoints = 1
                Else
                    ReDim Preserve GPSLocalPts(numPoints)
                    GPSLocalPts(numPoints).ptNum = GPSLocalPt.ptNum
                    GPSLocalPts(numPoints).latitude = GPSLocalPt.latitude
                    GPSLocalPts(numPoints).longitude = GPSLocalPt.longitude
                    GPSLocalPts(numPoints).height = GPSLocalPt.height
                    GPSLocalPts(numPoints).desc = GPSLocalPt.desc
                    numPoints += 1
                End If
                Dim latString As String = DecToDMS(GPSLocalPt.latitude, 1)
                Dim longString As String = DecToDMS(GPSLocalPt.longitude, 2)

                Dim geoData() As String = {GPSLocalPt.ptNum.ToString, GPSLocalPt.desc, "", latString, longString, (Decimal.Round(GPSLocalPt.height, 3)).ToString("f3")}
                PointList.Rows.Add(geoData)

            Catch ex As Exception
                If testForImportError = False Then
                    MessageBox.Show("An error occured while importing your points from the .csv file" & vbNewLine & "All of your data may not have been imported, check your .csv file format", "IMPORT ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    testForImportError = True
                End If
            End Try

        End While
        If numPoints > 0 Then
            getGeodeticButton.Enabled = True
            ComputeButton.Enabled = True
            PointList.ClearSelection()
        End If

        If GPSLocalBeenCalcd = True Then
            Call computeLocalPoints()
        End If

        geoPoints.Close()
    End Sub

    Private Function ParseAndConvertDEC2DMS(ByVal input As String) As Decimal
        Dim tempLatString As String = input.Substring(0, input.IndexOf(","))
        Dim delims(1) As Char
        delims(0) = "'"
        delims(1) = ControlChars.Quote

        Dim tempLatArgsString(0) As String
        tempLatArgsString = tempLatString.Split(delims)

        Dim tempLatDeg As String = String.Empty
        Dim tempLatMin As String = String.Empty
        For a As Integer = 0 To tempLatArgsString(0).Length - 1
            If Microsoft.VisualBasic.AscW(tempLatArgsString(0).Substring(a, 1)) <> 65533 Then
                tempLatDeg &= tempLatArgsString(0).Substring(a, 1)
            Else
                tempLatMin = tempLatArgsString(0).Substring(a + 1, tempLatArgsString(0).Length - (a + 1))
                Exit For
            End If
        Next

        Dim tempLatDegDec As Decimal = Convert.ToDecimal(tempLatDeg)

        Dim negTest As Boolean = False
        If tempLatDegDec < 0D Then
            tempLatDegDec *= -1
            negTest = True
        End If

        Dim tempLatMinDec As Decimal = Convert.ToDecimal(tempLatMin)
        Dim tempLatSecDec As Decimal = Convert.ToDecimal(tempLatArgsString(1))
        Dim decLatDec As Decimal = tempLatDegDec + (tempLatMinDec / 60D) + (tempLatSecDec / 3600D)

        If negTest = True Then
            decLatDec *= -1
        End If

        Return decLatDec
    End Function

    Structure GPS_Local_Pt
        Dim ptNum As String
        Dim desc As String
        Dim latitude As Decimal
        Dim longitude As Decimal
        Dim height As Decimal
        Dim northing As Decimal
        Dim easting As Decimal
        Dim elevation As Decimal
    End Structure

    Sub initGPSLocalPt(ByRef GPSLocalPt As GPS_Local_Pt)
        GPSLocalPt.ptNum = -9999
        GPSLocalPt.desc = String.Empty
        GPSLocalPt.latitude = -9999
        GPSLocalPt.longitude = -9999
        GPSLocalPt.height = -9999
        GPSLocalPt.northing = -9999
        GPSLocalPt.easting = -9999
        GPSLocalPt.elevation = -9999
    End Sub

    Private Sub ClearListButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClearListButton.Click
        PointList.Rows.Clear()
        getGeodeticButton.Enabled = False
        ComputeButton.Enabled = False
        localDetailsButton.Enabled = False
        exportButton.Enabled = False
        ReDim GPSLocalPts(0)
        GPSLocalBeenCalcd = False
        initLocalizationDetails(localDetailsSet)
    End Sub

    Private Sub getGeodeticButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles getGeodeticButton.Click
        GetPoint.ShowDialog()
        If GetPoint.selectedPointIndex <> -9999 Then
            Dim Index As Integer = GetPoint.selectedPointIndex
            Dim latdeg, latmin, latsec, longdeg, longmin, longsec As String
            DEC2DMS(GPSLocalPts(Index).latitude, latdeg, latmin, latsec)
            DEC2DMS(GPSLocalPts(Index).longitude, longdeg, longmin, longsec)

            If GPSLocalPts(Index).latitude < 0 Then
                CheckBox1.Checked = True
            Else
                CheckBox1.Checked = False
            End If

            If GPSLocalPts(Index).longitude > 0 Then
                CheckBox2.Checked = True
            Else
                CheckBox2.Checked = False
            End If

            LocalLatTextBox.Text = latdeg & latmin & latsec
            LocalLongTextBox.Text = longdeg & longmin & longsec
            LocalHeightTextBox.Text = (Decimal.Round(GPSLocalPts(Index).height, 3)).ToString("f3")
        End If
    End Sub

    Private Sub DEC2DMS(ByVal arg1 As Decimal, ByRef deg As String, ByRef min As String, ByRef sec As String)
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
        var6 *= 100000D

        deg = var1.ToString("#00")
        min = var4.ToString("00")
        sec = var6.ToString("0000000")

    End Sub

    Private Sub ComputeButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComputeButton.Click
        Call computeLocalPoints()
    End Sub

    Private Sub computeLocalPoints()
        Dim dlatd, dlatm, dlats, dlongd, dlongm, dlongs, dheight As Decimal
        Dim slatd As String = LocalLatTextBox.Text.Substring(0, 2)
        Dim slatm As String = LocalLatTextBox.Text.Substring(3, 2)
        Dim slats As String = LocalLatTextBox.Text.Substring(6, 8)
        Dim slongd As String = LocalLongTextBox.Text.Substring(0, 3)
        Dim slongm As String = LocalLongTextBox.Text.Substring(4, 2)
        Dim slongs As String = LocalLongTextBox.Text.Substring(7, 8)
        Dim GeodeticTest As Boolean = True
        Dim LocalTest As Boolean = True

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
            dheight = Decimal.Parse(LocalHeightTextBox.Text)
        Catch ex As Exception
            dheight = 0D
            GeodeticTest = False
        End Try

        GeodeticCurv(0) = dlatd + dlatm / 60D + dlats / 3600D
        GeodeticCurv(1) = -1 * (dlongd + dlongm / 60D + dlongs / 3600D)
        GeodeticCurv(2) = dheight

        localDetailsSet.latOrigin = "Latitude = " & dlatd.ToString("#0") & Chr(176) & " " & dlatm.ToString("00") & "' " & dlats.ToString("00.00000") & """"
        localDetailsSet.longOrigin = "Longitude = " & dlongd.ToString("#0") & Chr(176) & " " & dlongm.ToString("00") & "' " & dlongs.ToString("00.00000") & """"
        localDetailsSet.ellhOrigin = "Ellipsoid Height = " & dheight.ToString("##.000") & " m"

        If CheckBox1.Checked Then
            GeodeticCurv(0) *= -1
            localDetailsSet.latOrigin &= " S"
        Else
            localDetailsSet.latOrigin &= " N"
        End If

        If CheckBox2.Checked Then
            GeodeticCurv(1) *= -1
            localDetailsSet.longOrigin &= " E"
        Else
            localDetailsSet.longOrigin &= " W"
        End If

        GeodeticXYZ = curv2cart(GeodeticCurv)

        Dim sN As String = NTextBox.Text
        Dim sE As String = ETextBox.Text
        Dim sElev As String = ElevTextBox.Text

        Try
            LocalNEel(0) = Decimal.Parse(sN)
        Catch ex As Exception
            LocalTest = False
        End Try

        Try
            LocalNEel(1) = Decimal.Parse(sE)
        Catch ex As Exception
            LocalTest = False
        End Try

        Try
            LocalNEel(2) = Decimal.Parse(sElev)
        Catch ex As Exception
            LocalTest = False
        End Try

        localDetailsSet.NOrigin = "Northing = " & LocalNEel(0).ToString("#.000") & " m"
        localDetailsSet.EOrigin = "Easting = " & LocalNEel(1).ToString("#.000") & " m"
        localDetailsSet.ElevOrigin = "Elevation = " & LocalNEel(2).ToString("#.000") & " m"

        If UTMRadioButton.Checked = True Then
            GeodeticTest = True
            LocalTest = True
        End If

        If GeodeticTest Then
            If LocalTest Then

                R1.data(1, 1) = 1D
                R1.data(1, 2) = 0D
                R1.data(1, 3) = 0D
                R1.data(2, 1) = 0D
                R1.data(2, 2) = Math.Cos((90 - GeodeticCurv(0)) * Math.PI / 180D)
                R1.data(2, 3) = Math.Sin((90 - GeodeticCurv(0)) * Math.PI / 180D)
                R1.data(3, 1) = 0D
                R1.data(3, 2) = -1 * Math.Sin((90 - GeodeticCurv(0)) * Math.PI / 180D)
                R1.data(3, 3) = Math.Cos((90 - GeodeticCurv(0)) * Math.PI / 180D)

                R3.data(1, 1) = Math.Cos((90 + GeodeticCurv(1)) * Math.PI / 180D)
                R3.data(1, 2) = Math.Sin((90 + GeodeticCurv(1)) * Math.PI / 180D)
                R3.data(1, 3) = 0D
                R3.data(2, 1) = -1 * Math.Sin((90 + GeodeticCurv(1)) * Math.PI / 180D)
                R3.data(2, 2) = Math.Cos((90 + GeodeticCurv(1)) * Math.PI / 180D)
                R3.data(2, 3) = 0D
                R3.data(3, 1) = 0D
                R3.data(3, 2) = 0D
                R3.data(3, 3) = 1D

                Dim LocalCoords(1) As Decimal
                PointList.Rows.Clear()
                If UTMRadioButton.Checked = False Then
                    GPSLocalBeenCalcd = True
                End If
                Dim localizationTest As Boolean = True
                Dim errorMessageDisplayed As Boolean = False
                Dim UTMZone As String

                Dim scale As Decimal = 0.0

                If EGHRadioButton.Checked = True Then
                    Dim MerRC As Decimal = WGS84_a * (1 - WGS84_e2) / (1 - WGS84_e2 * (Math.Sin(GeodeticCurv(0) * Math.PI / 180)) ^ 2) ^ (3 / 2)
                    Dim PVRC As Decimal = WGS84_a / (1 - WGS84_e2 * (Math.Sin(GeodeticCurv(0) * Math.PI / 180)) ^ 2) ^ (1 / 2)
                    Dim AvgR As Decimal = Math.Sqrt(MerRC * PVRC)
                    If topconRadioButton.Checked = True Or leicaRadioButton.Checked = True Then
                        scale = (AvgR + GeodeticCurv(2)) / AvgR
                        If leicaRadioButton.Checked = True Then
                            scale = Decimal.Round(scale, 8I)
                        End If
                    ElseIf trimbleRadioButton.Checked = True Or tdsRadioButton.Checked = True Then
                        scale = (PVRC + GeodeticCurv(2)) / PVRC
                    End If
                    localDetailsSet.scaleFactor = "Scale Factor = 1.0000000000"
                    localDetailsSet.elevFactor = "Elevation Factor = " & scale.ToString("#.0000000000")
                    localDetailsSet.combFactor = "Combined Scale Factor = " & scale.ToString("#.0000000000")
                ElseIf PGHRadioButton.Checked = True Then
                    Try
                        Dim projH As Decimal = Decimal.Parse(PGHTextBox.Text)
                        Dim MerRC As Decimal = WGS84_a * (1 - WGS84_e2) / (1 - WGS84_e2 * (Math.Sin(GeodeticCurv(0) * Math.PI / 180)) ^ 2) ^ (3 / 2)
                        Dim PVRC As Decimal = WGS84_a / (1 - WGS84_e2 * (Math.Sin(GeodeticCurv(0) * Math.PI / 180)) ^ 2) ^ (1 / 2)
                        Dim AvgR As Decimal = Math.Sqrt(MerRC * PVRC)
                        If topconRadioButton.Checked = True Or leicaRadioButton.Checked = True Then
                            scale = (AvgR + projH) / AvgR
                            If leicaRadioButton.Checked = True Then
                                scale = Decimal.Round(scale, 8I)
                            End If
                        ElseIf trimbleRadioButton.Checked = True Or tdsRadioButton.Checked = True Then
                            scale = (PVRC + projH) / PVRC
                        End If
                        localDetailsSet.scaleFactor = "Scale Factor = 1.0000000000"
                        localDetailsSet.elevFactor = "Elevation Factor = " & scale.ToString("#.0000000000")
                        localDetailsSet.combFactor = "Combined Scale Factor = " & scale.ToString("#.0000000000")
                    Catch ex As Exception
                        localizationTest = False
                    End Try
                ElseIf USFRadioButton.Checked = True Then
                    Try
                        scale = Decimal.Parse(USFTextBox.Text)
                        localDetailsSet.scaleFactor = "Scale Factor = N/A"
                        localDetailsSet.elevFactor = "Elevation Factor = N/A"
                        localDetailsSet.combFactor = "Combined Scale Factor = " & scale.ToString("#.0000000000")
                    Catch ex As Exception
                        localizationTest = False
                    End Try
                End If
                Dim projType As Integer = 0
                Dim baseNvalue As Decimal = -9999D

                If orthoRadioButton.Checked Then
                    projType = 1
                ElseIf stereoRadioButton.Checked Then
                    projType = 2
                ElseIf transMerRadioButton.Checked Then
                    projType = 3
                End If

                If applyGeoidCheckBox.Checked = True Then
                    If UTMRadioButton.Checked = False And localizationTest = True Then
                        baseNvalue = geoidBYNfileReader.getInterpolatedNfromGeoidModelData(GeodeticCurv(0), GeodeticCurv(1), GeodeticCurv(0), GeodeticCurv(1), scale, projType)
                    End If
                End If

                For i As Integer = 0 To GPSLocalPts.Length - 1
                    UTMZone = String.Empty
                    'localizationTest = True
                    If orthoRadioButton.Checked Then
                        localDetailsSet.scaleFactor = "Scale Factor = 1.0000000000"
                        localDetailsSet.elevFactor = "Elevation Factor = 1.0000000000"
                        localDetailsSet.combFactor = "Combined Scale Factor = 1.0000000000"
                        localDetailsSet.projType = "Type = Orthographic"
                        LocalCoords = calcLocalOrtho(GPSLocalPts(i).latitude, GPSLocalPts(i).longitude, GPSLocalPts(i).height)
                    ElseIf stereoRadioButton.Checked Then
                        localDetailsSet.projType = "Type = Oblique Stereographic"
                        LocalCoords = calcLocalStereo(scale, GeodeticCurv(0), GeodeticCurv(1), GPSLocalPts(i).latitude, GPSLocalPts(i).longitude)
                    ElseIf transMerRadioButton.Checked Then
                        localDetailsSet.projType = "Type = Transverse Mercator"
                        LocalCoords = calcLocalTransMer(scale, GeodeticCurv(0), GeodeticCurv(1), GPSLocalPts(i).latitude, GPSLocalPts(i).longitude)
                    ElseIf UTMRadioButton.Checked Then
                        LocalCoords = calcUTM(GPSLocalPts(i).latitude, GPSLocalPts(i).longitude, localizationTest, UTMZone)
                    End If
                    GPSLocalPts(i).northing = LocalCoords(0)
                    GPSLocalPts(i).easting = LocalCoords(1)

                    Dim geoidString As String

                    Dim geoidModelCorrection As Decimal = 0
                    If applyGeoidCheckBox.Checked = True Then
                        If UTMRadioButton.Checked = False And localizationTest = True Then
                            Dim remoteNvalue As Decimal = geoidBYNfileReader.getInterpolatedNfromGeoidModelData(GPSLocalPts(i).latitude, GPSLocalPts(i).longitude, GeodeticCurv(0), GeodeticCurv(1), scale, projType)
                            If baseNvalue <> -9999D And remoteNvalue <> -9999D Then
                                geoidString = "Yes"
                                geoidModelCorrection = remoteNvalue - baseNvalue
                            Else
                                geoidString = "Missing Data"
                            End If
                        Else
                            geoidString = "No"
                        End If
                    Else
                        geoidString = "No"
                    End If

                    GPSLocalPts(i).elevation = LocalNEel(2) + (GPSLocalPts(i).height - GeodeticCurv(2)) - geoidModelCorrection

                    Dim latString As String = DecToDMS(GPSLocalPts(i).latitude, 1)
                    Dim longString As String = DecToDMS(GPSLocalPts(i).longitude, 2)

                    Dim NString As String = (Decimal.Round(GPSLocalPts(i).northing, 3)).ToString("f3")
                    Dim EString As String = (Decimal.Round(GPSLocalPts(i).easting, 3)).ToString("f3")
                    Dim ElevString As String = (Decimal.Round(GPSLocalPts(i).elevation, 3)).ToString("f3")

                    Dim bracket1 As String = " ("
                    Dim bracket2 As String = ")"

                    If UTMRadioButton.Checked = False Then
                        bracket1 = String.Empty
                        bracket2 = String.Empty
                    Else
                        ElevString = String.Empty
                    End If

                    If localizationTest = False Then

                        NString = String.Empty
                        EString = String.Empty
                        ElevString = String.Empty
                        bracket1 = String.Empty
                        bracket2 = String.Empty
                        UTMZone = String.Empty

                        If errorMessageDisplayed = False Then
                            MessageBox.Show("Unable to compute local coordinates, please check all localization parameters", "COMPUTATION ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            errorMessageDisplayed = True
                        End If
                        localDetailsButton.Enabled = False
                    Else
                        localDetailsButton.Enabled = True
                    End If

                    Dim geoData() As String = {GPSLocalPts(i).ptNum.ToString, GPSLocalPts(i).desc, "", latString, longString, (Decimal.Round(GPSLocalPts(i).height, 3)).ToString("f3"), "", NString & bracket1 & UTMZone & bracket2, EString & bracket1 & UTMZone & bracket2, ElevString, geoidString}
                    PointList.Rows.Add(geoData)
                Next
                exportButton.Enabled = True
            Else
                MessageBox.Show("A local coordinate you specified is incorrect or missing.", "Problem with Local Coordinate(s)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        Else
            MessageBox.Show("A geodetic coordinate you specified is incorrect or missing", "Problem with Geodetic Coordinate(s)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
        PointList.ClearSelection()
    End Sub

    Friend Function calcLocalOrtho(ByVal Lat As Decimal, ByVal Lon As Decimal, ByVal Height As Decimal) As Decimal()
        Dim CurvCoords() As Decimal = {Lat, Lon, Height}
        Dim ECEFcoords() As Decimal = curv2cart(CurvCoords)

        Dim ECEF_Diff As New Matrix(3, 1)
        Dim Local_Coords As New Matrix

        ECEF_Diff.data(1, 1) = ECEFcoords(0) - GeodeticXYZ(0)
        ECEF_Diff.data(2, 1) = ECEFcoords(1) - GeodeticXYZ(1)
        ECEF_Diff.data(3, 1) = ECEFcoords(2) - GeodeticXYZ(2)

        Local_Coords = R1 * R3 * ECEF_Diff

        Dim retValues(1) As Decimal
        retValues(0) = Local_Coords.data(2, 1) + LocalNEel(0)
        retValues(1) = Local_Coords.data(1, 1) + LocalNEel(1)

        Return retValues
    End Function

    Friend Function calcLocalStereo(ByVal scale As Decimal, ByVal BaseLat As Decimal, ByVal BaseLong As Decimal, ByVal PtLat As Decimal, ByVal PtLong As Decimal) As Decimal()
        Dim retValues(1) As Decimal

        Dim X1 As Decimal = 2 * Math.Atan(Math.Sqrt(((1 + Math.Sin(BaseLat * Math.PI / 180)) / (1 - Math.Sin(BaseLat * Math.PI / 180))) * ((1 - Math.Sqrt(WGS84_e2) * Math.Sin(BaseLat * Math.PI / 180)) / (1 + Math.Sqrt(WGS84_e2) * Math.Sin(BaseLat * Math.PI / 180))) ^ (Math.Sqrt(WGS84_e2)))) - Math.PI / 2
        Dim m1 As Decimal = Math.Cos(BaseLat * Math.PI / 180) / Math.Sqrt(1 - WGS84_e2 * (Math.Sin(BaseLat * Math.PI / 180)) ^ 2)
        Dim X As Decimal = 2 * Math.Atan(Math.Sqrt(((1 + Math.Sin(PtLat * Math.PI / 180)) / (1 - Math.Sin(PtLat * Math.PI / 180))) * ((1 - Math.Sqrt(WGS84_e2) * Math.Sin(PtLat * Math.PI / 180)) / (1 + Math.Sqrt(WGS84_e2) * Math.Sin(PtLat * Math.PI / 180))) ^ (Math.Sqrt(WGS84_e2)))) - Math.PI / 2
        Dim m As Decimal = Math.Cos(PtLat * Math.PI / 180) / Math.Sqrt(1 - WGS84_e2 * (Math.Sin(PtLat * Math.PI / 180)) ^ 2)
        Dim A As Decimal = 2 * WGS84_a * scale * m1 / (Math.Cos(X1) * (1 + Math.Sin(X1) * Math.Sin(X) + Math.Cos(X1) * Math.Cos(X) * Math.Cos(PtLong * Math.PI / 180 - BaseLong * Math.PI / 180)))

        retValues(0) = A * (Math.Cos(X1) * Math.Sin(X) - Math.Sin(X1) * Math.Cos(X) * Math.Cos(PtLong * Math.PI / 180 - BaseLong * Math.PI / 180)) + LocalNEel(0)
        retValues(1) = A * Math.Cos(X) * Math.Sin(PtLong * Math.PI / 180 - BaseLong * Math.PI / 180) + LocalNEel(1)

        Return retValues
    End Function

    Friend Function calcLocalTransMer(ByVal scale As Decimal, ByVal BaseLat As Decimal, ByVal BaseLong As Decimal, ByVal PtLat As Decimal, ByVal PtLong As Decimal) As Decimal()
        Dim retValues(1) As Decimal

        Dim Mo As Decimal = WGS84_a * ((1 - WGS84_e2 / 4 - 3 * WGS84_e2 * WGS84_e2 / 64 - 5 * WGS84_e2 * WGS84_e2 * WGS84_e2 / 256) * _
                            (BaseLat * Math.PI / 180) - (3 * WGS84_e2 / 8 + 3 * WGS84_e2 * WGS84_e2 / 32 + 45 * WGS84_e2 * _
                            WGS84_e2 * WGS84_e2 / 1024) * Math.Sin(2 * BaseLat * Math.PI / 180) + (15 * WGS84_e2 * WGS84_e2 / 256 + 45 * _
                            WGS84_e2 * WGS84_e2 * WGS84_e2 / 1024) * Math.Sin(4 * BaseLat * Math.PI / 180) - (35 * WGS84_e2 * WGS84_e2 * WGS84_e2 / 3072) * _
                            Math.Sin(6 * BaseLat * Math.PI / 180))

        Dim M As Decimal = WGS84_a * ((1 - WGS84_e2 / 4 - 3 * WGS84_e2 * WGS84_e2 / 64 - 5 * WGS84_e2 * WGS84_e2 * WGS84_e2 / 256) * _
                            (PtLat * Math.PI / 180) - (3 * WGS84_e2 / 8 + 3 * WGS84_e2 * WGS84_e2 / 32 + 45 * WGS84_e2 * _
                            WGS84_e2 * WGS84_e2 / 1024) * Math.Sin(2 * PtLat * Math.PI / 180) + (15 * WGS84_e2 * WGS84_e2 / 256 + 45 * _
                            WGS84_e2 * WGS84_e2 * WGS84_e2 / 1024) * Math.Sin(4 * PtLat * Math.PI / 180) - (35 * WGS84_e2 * WGS84_e2 * WGS84_e2 / 3072) * _
                            Math.Sin(6 * PtLat * Math.PI / 180))

        Dim T As Decimal = (Math.Tan(PtLat * Math.PI / 180)) ^ 2
        Dim C As Decimal = WGS84_e2 * (Math.Cos(PtLat * Math.PI / 180)) ^ 2 / (1 - WGS84_e2)
        Dim A As Decimal = (PtLong * Math.PI / 180 - BaseLong * Math.PI / 180) * Math.Cos(PtLat * Math.PI / 180)
        Dim v As Decimal = WGS84_a / Math.Sqrt(1 - WGS84_e2 * (Math.Sin(PtLat * Math.PI / 180)) ^ 2)

        Dim WGS84_b As Decimal = WGS84_a * Math.Sqrt(1 - WGS84_e2)
        Dim secE2 As Decimal = (WGS84_a ^ 2 - WGS84_b ^ 2) / WGS84_b ^ 2

        retValues(0) = LocalNEel(0) + scale * (M - Mo + v * Math.Tan(PtLat * Math.PI / 180) * ((A ^ 2) / 2 + (5 - T + 9 * C + 4 * C ^ 2) * (A ^ 4) / 24 + (61 - 58 * T + T ^ 2 + 600 * C - 330 * secE2) * (A ^ 6) / 720))
        retValues(1) = LocalNEel(1) + scale * v * (A + (1 - T + C) * (A ^ 3) / 6 + (5 - 18 * T + T ^ 2 + 72 * C - 58 * secE2) * (A ^ 5) / 120)

        Return retValues
    End Function

    Private Function calcUTM(ByVal PtLat As Decimal, ByVal PtLong As Decimal, ByRef test As Boolean, ByRef Zone As String) As Decimal()
        Dim retValues(1) As Decimal

        Dim BaseLat As Decimal = 0
        Dim BaseLong As Decimal = 0
        Dim zoneString As String = String.Empty

        If autoRadioButton.Checked = True Then
            If PtLong >= -180 And PtLong < -174 Then
                BaseLong = -177 : zoneString = "1"
            ElseIf PtLong >= -174 And PtLong < -168 Then
                BaseLong = -171 : zoneString = "2"
            ElseIf PtLong >= -168 And PtLong < -162 Then
                BaseLong = -165 : zoneString = "3"
            ElseIf PtLong >= -162 And PtLong < -156 Then
                BaseLong = -159 : zoneString = "4"
            ElseIf PtLong >= -156 And PtLong < -150 Then
                BaseLong = -153 : zoneString = "5"
            ElseIf PtLong >= -150 And PtLong < -144 Then
                BaseLong = -147 : zoneString = "6"
            ElseIf PtLong >= -144 And PtLong < -138 Then
                BaseLong = -141 : zoneString = "7"
            ElseIf PtLong >= -138 And PtLong < -132 Then
                BaseLong = -135 : zoneString = "8"
            ElseIf PtLong >= -132 And PtLong < -126 Then
                BaseLong = -129 : zoneString = "9"
            ElseIf PtLong >= -126 And PtLong < -120 Then
                BaseLong = -123 : zoneString = "10"
            ElseIf PtLong >= -120 And PtLong < -114 Then
                BaseLong = -117 : zoneString = "11"
            ElseIf PtLong >= -114 And PtLong < -108 Then
                BaseLong = -111 : zoneString = "12"
            ElseIf PtLong >= -108 And PtLong < -102 Then
                BaseLong = -105 : zoneString = "13"
            ElseIf PtLong >= -102 And PtLong < -96 Then
                BaseLong = -99 : zoneString = "14"
            ElseIf PtLong >= -96 And PtLong < -90 Then
                BaseLong = -93 : zoneString = "15"
            ElseIf PtLong >= -90 And PtLong < -84 Then
                BaseLong = -87 : zoneString = "16"
            ElseIf PtLong >= -84 And PtLong < -78 Then
                BaseLong = -81 : zoneString = "17"
            ElseIf PtLong >= -78 And PtLong < -72 Then
                BaseLong = -75 : zoneString = "18"
            ElseIf PtLong >= -72 And PtLong < -66 Then
                BaseLong = -69 : zoneString = "19"
            ElseIf PtLong >= -66 And PtLong < -60 Then
                BaseLong = -63 : zoneString = "20"
            ElseIf PtLong >= -60 And PtLong < -54 Then
                BaseLong = -57 : zoneString = "21"
            ElseIf PtLong >= -54 And PtLong < -48 Then
                BaseLong = -51 : zoneString = "22"
            ElseIf PtLong >= -48 And PtLong < -42 Then
                BaseLong = -45 : zoneString = "23"
            ElseIf PtLong >= -42 And PtLong < -36 Then
                BaseLong = -39 : zoneString = "24"
            ElseIf PtLong >= -36 And PtLong < -30 Then
                BaseLong = -33 : zoneString = "25"
            ElseIf PtLong >= -30 And PtLong < -24 Then
                BaseLong = -27 : zoneString = "26"
            ElseIf PtLong >= -24 And PtLong < -18 Then
                BaseLong = -21 : zoneString = "27"
            ElseIf PtLong >= -18 And PtLong < -12 Then
                BaseLong = -15 : zoneString = "28"
            ElseIf PtLong >= -12 And PtLong < -6 Then
                BaseLong = -9 : zoneString = "29"
            ElseIf PtLong >= -6 And PtLong < 0 Then
                BaseLong = -3 : zoneString = "30"
            ElseIf PtLong >= 0 And PtLong < 6 Then
                BaseLong = 3 : zoneString = "31"
            ElseIf PtLong >= 6 And PtLong < 12 Then
                BaseLong = 9 : zoneString = "32"
            ElseIf PtLong >= 12 And PtLong < 18 Then
                BaseLong = 15 : zoneString = "33"
            ElseIf PtLong >= 18 And PtLong < 24 Then
                BaseLong = 21 : zoneString = "34"
            ElseIf PtLong >= 24 And PtLong < 30 Then
                BaseLong = 27 : zoneString = "35"
            ElseIf PtLong >= 30 And PtLong < 36 Then
                BaseLong = 33 : zoneString = "36"
            ElseIf PtLong >= 36 And PtLong < 42 Then
                BaseLong = 39 : zoneString = "37"
            ElseIf PtLong >= 42 And PtLong < 48 Then
                BaseLong = 45 : zoneString = "38"
            ElseIf PtLong >= 48 And PtLong < 54 Then
                BaseLong = 51 : zoneString = "39"
            ElseIf PtLong >= 54 And PtLong < 60 Then
                BaseLong = 57 : zoneString = "40"
            ElseIf PtLong >= 60 And PtLong < 66 Then
                BaseLong = 63 : zoneString = "41"
            ElseIf PtLong >= 66 And PtLong < 72 Then
                BaseLong = 69 : zoneString = "42"
            ElseIf PtLong >= 72 And PtLong < 78 Then
                BaseLong = 75 : zoneString = "43"
            ElseIf PtLong >= 78 And PtLong < 84 Then
                BaseLong = 81 : zoneString = "44"
            ElseIf PtLong >= 84 And PtLong < 90 Then
                BaseLong = 87 : zoneString = "45"
            ElseIf PtLong >= 90 And PtLong < 96 Then
                BaseLong = 93 : zoneString = "46"
            ElseIf PtLong >= 96 And PtLong < 102 Then
                BaseLong = 99 : zoneString = "47"
            ElseIf PtLong >= 102 And PtLong < 108 Then
                BaseLong = 105 : zoneString = "48"
            ElseIf PtLong >= 108 And PtLong < 114 Then
                BaseLong = 111 : zoneString = "49"
            ElseIf PtLong >= 114 And PtLong < 120 Then
                BaseLong = 117 : zoneString = "50"
            ElseIf PtLong >= 120 And PtLong < 126 Then
                BaseLong = 123 : zoneString = "51"
            ElseIf PtLong >= 126 And PtLong < 132 Then
                BaseLong = 129 : zoneString = "52"
            ElseIf PtLong >= 132 And PtLong < 138 Then
                BaseLong = 135 : zoneString = "53"
            ElseIf PtLong >= 138 And PtLong < 144 Then
                BaseLong = 141 : zoneString = "54"
            ElseIf PtLong >= 144 And PtLong < 150 Then
                BaseLong = 147 : zoneString = "55"
            ElseIf PtLong >= 150 And PtLong < 156 Then
                BaseLong = 153 : zoneString = "56"
            ElseIf PtLong >= 156 And PtLong < 162 Then
                BaseLong = 159 : zoneString = "57"
            ElseIf PtLong >= 162 And PtLong < 168 Then
                BaseLong = 165 : zoneString = "58"
            ElseIf PtLong >= 168 And PtLong < 174 Then
                BaseLong = 171 : zoneString = "59"
            ElseIf PtLong >= 174 And PtLong <= 180 Then
                BaseLong = 177 : zoneString = "60"
            End If
        ElseIf zoneRadioButton.Checked = True Then
            Try
                Dim zoneInt As Integer = Integer.Parse(zoneTextBox.Text)
                If zoneInt < 1 Or zoneInt > 60 Then
                    test = False
                Else
                    zoneString = zoneInt.ToString
                    BaseLong = -183 + zoneInt * 6
                End If
            Catch ex As Exception
                test = False
            End Try
        End If

        Dim scale As Decimal = 0.9996

        Dim Mo As Decimal = WGS84_a * ((1 - WGS84_e2 / 4 - 3 * WGS84_e2 * WGS84_e2 / 64 - 5 * WGS84_e2 * WGS84_e2 * WGS84_e2 / 256) * _
                            (BaseLat * Math.PI / 180) - (3 * WGS84_e2 / 8 + 3 * WGS84_e2 * WGS84_e2 / 32 + 45 * WGS84_e2 * _
                            WGS84_e2 * WGS84_e2 / 1024) * Math.Sin(2 * BaseLat * Math.PI / 180) + (15 * WGS84_e2 * WGS84_e2 / 256 + 45 * _
                            WGS84_e2 * WGS84_e2 * WGS84_e2 / 1024) * Math.Sin(4 * BaseLat * Math.PI / 180) - (35 * WGS84_e2 * WGS84_e2 * WGS84_e2 / 3072) * _
                            Math.Sin(6 * BaseLat * Math.PI / 180))

        Dim M As Decimal = WGS84_a * ((1 - WGS84_e2 / 4 - 3 * WGS84_e2 * WGS84_e2 / 64 - 5 * WGS84_e2 * WGS84_e2 * WGS84_e2 / 256) * _
                            (PtLat * Math.PI / 180) - (3 * WGS84_e2 / 8 + 3 * WGS84_e2 * WGS84_e2 / 32 + 45 * WGS84_e2 * _
                            WGS84_e2 * WGS84_e2 / 1024) * Math.Sin(2 * PtLat * Math.PI / 180) + (15 * WGS84_e2 * WGS84_e2 / 256 + 45 * _
                            WGS84_e2 * WGS84_e2 * WGS84_e2 / 1024) * Math.Sin(4 * PtLat * Math.PI / 180) - (35 * WGS84_e2 * WGS84_e2 * WGS84_e2 / 3072) * _
                            Math.Sin(6 * PtLat * Math.PI / 180))

        Dim T As Decimal = (Math.Tan(PtLat * Math.PI / 180)) ^ 2
        Dim C As Decimal = WGS84_e2 * (Math.Cos(PtLat * Math.PI / 180)) ^ 2 / (1 - WGS84_e2)
        Dim A As Decimal = (PtLong * Math.PI / 180 - BaseLong * Math.PI / 180) * Math.Cos(PtLat * Math.PI / 180)
        Dim v As Decimal = WGS84_a / Math.Sqrt(1 - WGS84_e2 * (Math.Sin(PtLat * Math.PI / 180)) ^ 2)

        Dim WGS84_b As Decimal = WGS84_a * Math.Sqrt(1 - WGS84_e2)
        Dim secE2 As Decimal = (WGS84_a ^ 2 - WGS84_b ^ 2) / WGS84_b ^ 2

        Dim FalseNorthing As Decimal = 0
        Dim NorS As String = "N"

        If PtLat < 0 Then
            FalseNorthing = 10000000
            NorS = "S"
        End If

        Zone = zoneString & NorS
        retValues(0) = FalseNorthing + scale * (M - Mo + v * Math.Tan(PtLat * Math.PI / 180) * ((A ^ 2) / 2 + (5 - T + 9 * C + 4 * C ^ 2) * (A ^ 4) / 24 + (61 - 58 * T + T ^ 2 + 600 * C - 330 * secE2) * (A ^ 6) / 720))
        retValues(1) = 500000 + scale * v * (A + (1 - T + C) * (A ^ 3) / 6 + (5 - 18 * T + T ^ 2 + 72 * C - 58 * secE2) * (A ^ 5) / 120)

        Return retValues
    End Function

    Private Sub exportButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exportButton.Click
        Dim SaveResult As DialogResult = SaveFileDialog1.ShowDialog
        If SaveResult = Windows.Forms.DialogResult.OK Then
            If SaveFileDialog1.FileName.Substring(SaveFileDialog1.FileName.Length - 4, 4).ToUpper <> ".CSV" Then
                SaveFileDialog1.FileName &= ".csv"
            End If
            Dim exportFile As New StreamWriter(SaveFileDialog1.FileName)

            For i As Integer = 0 To PointList.RowCount - 1
                exportFile.WriteLine(PointList.Rows(i).Cells(0).Value.ToString & "," & PointList.Rows(i).Cells(7).Value.ToString & "," & PointList.Rows(i).Cells(8).Value.ToString & "," & PointList.Rows(i).Cells(9).Value.ToString & "," & PointList.Rows(i).Cells(1).Value.ToString)
                'exportFile.WriteLine(GPSLocalPts(i).ptNum.ToString & "," & Decimal.Round(GPSLocalPts(i).northing, 3).ToString & "," & Decimal.Round(GPSLocalPts(i).easting, 3).ToString & "," & ElevString & "," & GPSLocalPts(i).desc)
            Next
            exportFile.Close()
            MessageBox.Show("Export of the local coordinates file is complete!", "EXPORT", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim myDialogResult As DialogResult = OpenFileDialog2.ShowDialog()

        If myDialogResult <> Windows.Forms.DialogResult.Cancel Then
            TextBox2.Text = OpenFileDialog2.FileName
            Button6.Enabled = True
        End If
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        If TextBox2.Text <> String.Empty Then

            Try
                Dim al3FileReader As New StreamReader(TextBox2.Text)

                Try
                    Dim almanacRecord As String = String.Empty
                    Dim NumRecords As Integer = 0
                    Dim TOA_Weeks As Integer = 0
                    Dim TOA_Seconds As Integer = 0
                    Dim Almanac(31, 12) As String
                    Dim PRN_Num As Integer = 0
                    Dim Ecc As Decimal = 0
                    Dim Incl As Decimal = 0
                    Dim RORA As Decimal = 0
                    Dim SemiMajor As Decimal = 0
                    Dim OMEGA As Decimal = 0
                    Dim AOP As Decimal = 0
                    Dim MeanAnom As Decimal = 0
                    Dim af0 As Decimal = 0
                    Dim af1 As Decimal = 0
                    Dim healthI As Integer = -9999
                    Dim healthS As String = String.Empty

                    almanacRecord = al3FileReader.ReadLine
                    NumRecords = Integer.Parse(almanacRecord.Substring(0, 2))

                    almanacRecord = al3FileReader.ReadLine
                    TOA_Weeks = Integer.Parse(almanacRecord.Substring(0, 4))
                    TOA_Weeks += 1024
                    TOA_Seconds = Integer.Parse(almanacRecord.Substring(4))

                    For i As Integer = 1 To NumRecords
                        almanacRecord = al3FileReader.ReadLine  'chew an empty line

                        almanacRecord = al3FileReader.ReadLine
                        PRN_Num = Integer.Parse(almanacRecord)
                        PRN_Num -= 1

                        almanacRecord = al3FileReader.ReadLine  'chew SVN number
                        almanacRecord = al3FileReader.ReadLine  'chew Satellite URA Number

                        almanacRecord = al3FileReader.ReadLine
                        Ecc = ExpString2Decimal(almanacRecord.Substring(0, 21))
                        Incl = ExpString2Decimal(almanacRecord.Substring(21, 22))
                        Incl = (0.3 + Incl) * GPS_PI
                        RORA = ExpString2Decimal(almanacRecord.Substring(43))
                        RORA *= GPS_PI

                        almanacRecord = al3FileReader.ReadLine
                        SemiMajor = ExpString2Decimal(almanacRecord.Substring(0, 21))
                        SemiMajor *= SemiMajor
                        OMEGA = ExpString2Decimal(almanacRecord.Substring(21, 22))
                        OMEGA *= GPS_PI
                        AOP = ExpString2Decimal(almanacRecord.Substring(43))
                        AOP *= GPS_PI

                        almanacRecord = al3FileReader.ReadLine
                        MeanAnom = ExpString2Decimal(almanacRecord.Substring(0, 21))
                        MeanAnom *= GPS_PI
                        af0 = ExpString2Decimal(almanacRecord.Substring(21, 22))
                        af1 = ExpString2Decimal(almanacRecord.Substring(43))

                        almanacRecord = al3FileReader.ReadLine
                        healthI = Integer.Parse(almanacRecord)

                        If healthI = 0 Then
                            healthS = "Good"
                        Else
                            healthS = "Bad"
                        End If

                        almanacRecord = al3FileReader.ReadLine  'chew Satellite Configuration

                        Almanac(PRN_Num, 0) = PRN_Num.ToString
                        Almanac(PRN_Num, 1) = TOA_Weeks.ToString
                        Almanac(PRN_Num, 2) = TOA_Seconds.ToString
                        Almanac(PRN_Num, 3) = af0.ToString("f8")
                        Almanac(PRN_Num, 4) = af1.ToString("f13")
                        Almanac(PRN_Num, 5) = Ecc.ToString("f8")
                        Almanac(PRN_Num, 6) = SemiMajor.ToString("f3")
                        Almanac(PRN_Num, 7) = MeanAnom.ToString("f8")
                        Almanac(PRN_Num, 8) = AOP.ToString("f8")
                        Almanac(PRN_Num, 9) = OMEGA.ToString("f8")
                        Almanac(PRN_Num, 10) = RORA.ToString("f13")
                        Almanac(PRN_Num, 11) = Incl.ToString("f8")
                        Almanac(PRN_Num, 12) = healthS
                    Next
                    al3FileReader.Close()
                    DataGrid.Rows.Clear()

                    For j As Integer = 0 To 31
                        Dim PRN As Integer = j + 1
                        If Almanac(j, 0) <> Nothing Then
                            Dim almanacdata() As String = {PRN.ToString, Almanac(j, 1), Almanac(j, 2), Almanac(j, 3), Almanac(j, 4), Almanac(j, 5), Almanac(j, 6), Almanac(j, 7), Almanac(j, 8), Almanac(j, 9), Almanac(j, 10), Almanac(j, 11), Almanac(j, 12)}
                            DataGrid.Rows.Add(almanacdata)
                        Else
                            Dim almanacdata() As String = {PRN.ToString, "N/A"}
                            DataGrid.Rows.Add(almanacdata)
                        End If
                    Next

                    Dim endTracks() As String = {"END", "OF", "ALMANAC", "DATA"}
                    DataGrid.Rows.Add(endTracks)

                Catch
                    MessageBox.Show("A problem occured importing the selected SEM Almanac File, the format may be incorrect" & vbNewLine & "Use the link provided to download a new SEM Almanac File from the Official Civilian GPS Website", "SEM ALMANAC FORMAT PROBLEM", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Catch
                MessageBox.Show("A problem occured opening the selected SEM Almanac File" & vbNewLine & "Try using the link provided to download a new SEM Almanac File from the Official Civilian GPS Website", "SEM ALMANAC FILE PROBLEM", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Function ExpString2Decimal(ByVal RINEX_String As String) As Decimal
        Dim RINEX_String_NoSpaces As String = vbNullString
        Dim RINEX_String_Number As String = vbNullString
        Dim RINEX_Number As Decimal = 0D
        Dim RINEX_Exp As Integer = 0I
        Dim i As Integer
        Dim test As Boolean = False

        For i = 0 To RINEX_String.Length - 1
            If RINEX_String.Substring(i, 1) <> " " Then
                test = True
            End If
        Next

        If test = True Then
            Dim boolStartExp As Boolean = False
            For i = 0 To RINEX_String.Length - 1
                If RINEX_String.Chars(i) <> " " Then
                    If RINEX_String.Chars(i) = "D" Or RINEX_String.Chars(i) = "d" Or RINEX_String.Chars(i) = "E" Or RINEX_String.Chars(i) = "e" Then
                        RINEX_String_NoSpaces &= "D"
                    Else
                        RINEX_String_NoSpaces &= RINEX_String.Chars(i)
                    End If
                End If
            Next

            RINEX_String_Number = RINEX_String_NoSpaces.Substring(0, RINEX_String_NoSpaces.IndexOf("D"))
            RINEX_Number = Convert.ToDecimal(RINEX_String_Number)
            RINEX_Exp = Convert.ToInt16(RINEX_String_NoSpaces.Substring(RINEX_String_NoSpaces.IndexOf("D") + 1, RINEX_String_NoSpaces.Length - RINEX_String_Number.Length - 1))

            For i = 1 To Math.Abs(RINEX_Exp)
                If RINEX_Exp < 0 Then
                    RINEX_Number /= 10D
                Else
                    RINEX_Number *= 10D
                End If
            Next
            Return RINEX_Number
        Else
            Return -9999D
        End If

    End Function

    Private Sub downloadAlmanacCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles downloadAlmanacCheckBox.CheckedChanged
        downloadAlmanacGroupBox.Enabled = downloadAlmanacCheckBox.Checked
    End Sub

    Private Sub cbbCOMPorts_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbbCOMPorts.SelectedIndexChanged
        Button5.Enabled = False
        Button4.Enabled = False
        Label43.Text = "No Garmin GPS Device Connected"
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Button4.Enabled = False
        Button5.Enabled = False
        If cbbCOMPorts.SelectedIndex <> -1 Then

            GPS_Connected = False
            If serialPort2 Is Nothing Then
            Else
                serialPort2.Dispose()
            End If

            serialPort2 = New IO.Ports.SerialPort
            With serialPort2
                .PortName = cbbCOMPorts.Items(cbbCOMPorts.SelectedIndex)
                .BaudRate = 9600
                .Parity = IO.Ports.Parity.None
                .DataBits = 8
                .StopBits = IO.Ports.StopBits.One
                .ReadTimeout = 3000I
                ' .Encoding = System.Text.Encoding.Unicode
            End With

            Dim portTest As Boolean = True
            If serialPort2.IsOpen = False Then
                Try
                    serialPort2.Open()
                Catch
                    MessageBox.Show("There is a problem communicating through the COM port you selected." & vbNewLine & "It may be in use by another application or if you are using a USB to Serial adapter try unplugging and re-plugging it in and try again.", "COM PORT PROBLEM", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    portTest = False
                End Try
            End If

            If portTest Then
                Dim PIDnData() As Byte = {C2B(254), C2B(0)}
                Dim Sum As Byte = CalcSum(PIDnData)
                Dim sendInit() As Byte = {C2B(16), C2B(254), C2B(0), Sum, C2B(16), C2B(3)}

                ErrorFlag = False
                timeOutErrorFlag = False
                LastCommand(0) = 254
                LastCommand(1) = 0
                serialPort2.Write(sendInit, 0, sendInit.Length)
                calcTime(timeOfLastWrite)
                Application.DoEvents()
                System.Threading.Thread.Sleep(2000)
                'serialPort2.Close()
                If GPS_Connected = True Then
                    Button4.Enabled = True
                    Button5.Enabled = True
                    Label43.Text = "** Garmin GPS Device Connected **"
                    'Label2.Font = New Font("calibri", 9, FontStyle.Bold)
                    Label43.Refresh()
                    MessageBox.Show("Garmin GPS device found on selected COM Port" & vbNewLine & vbNewLine & _
                                        "Product ID: " & ProductID.ToString & vbNewLine & "Firmware Version: " & SoftwareVersion.ToString & vbNewLine & _
                                        "Self Description: " & DeviceInfo, "COMMUNICATION ACKNOWLEDGED")
                Else
                    Button4.Enabled = False
                    Button5.Enabled = False
                    Label43.Text = "No Garmin GPS Device Connected"
                    'Label2.Font = New Font("calibri", 9)
                    Label43.Refresh()

                    MessageBox.Show("There is a problem with the communication on the selected COM port!" & vbNewLine & vbNewLine & _
                                        "Please check the following..." & vbNewLine & _
                                        vbTab & "- Garmin is powered on" & vbNewLine & _
                                        vbTab & "- Garmin set to transfer in GARMIN serial data format" & vbNewLine & _
                                        vbTab & "- Serial cable correctly attached to Garmin" & vbNewLine & _
                                        vbTab & "- Serial cable correctly attached to PC" & vbNewLine & _
                                        vbTab & "- Correct PC COM port selected from the list", "NO COMMUMICATION", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    serialPort2.Close()
                End If
            End If
        Else
            MessageBox.Show("You must select which COM port the Garmin GPS device is connected to.", "NO SERIAL PORT SELECTED")
        End If
    End Sub

    Private Function C2B(ByVal num As Integer) As Byte
        Return Convert.ToByte(num)
    End Function

    Private Function CalcSum(ByVal bytes() As Byte) As Byte

        Dim res1 As Integer = 0
        Dim res2 As Integer = 0
        Dim res3 As Integer = 0
        Dim res4 As Integer = 0

        For i As Integer = 0 To bytes.Length - 1
            res1 += bytes(i)
        Next

        res2 = res1 And &HFF
        res3 = res2 Xor &HFF
        res4 = res3 + 1

        If res4 > 255 Then
            res4 = 0
        End If
        Return C2B(res4)
    End Function

    Private Sub calcTime(ByRef time As Long)

        Dim currentTime As Date
        currentTime = DateTime.Now

        Dim CalendarDate As String = currentTime.Month & "/" & currentTime.Day & "/" & currentTime.Year.ToString("0000")
        Dim Hours As Integer = currentTime.Hour
        Dim Minutes As Integer = currentTime.Minute
        Dim Seconds As Decimal = currentTime.Second + currentTime.Millisecond / 1000
        'input format must be calendarDate ex. 2/23/1981
        Dim result(0 To 1) As Decimal
        Dim monthInt, dayInt, YearInt As Integer
        Dim dayStringTemp As String

        monthInt = Convert.ToInt32(CalendarDate.Substring(0, CalendarDate.IndexOf("/")))
        dayStringTemp = CalendarDate.Substring(CalendarDate.IndexOf("/") + 1)
        dayInt = Convert.ToInt32(dayStringTemp.Substring(0, dayStringTemp.IndexOf("/")))
        YearInt = Convert.ToInt32(dayStringTemp.Substring(dayStringTemp.IndexOf("/") + 1))

        Dim completedDays As Integer = (YearInt - 1980I) * 365I

        Dim i As Integer
        Dim completedLeapDays As Integer = 0I
        Dim TestForLeapYearBoolean As Boolean

        For i = 1980I To YearInt - 1I
            TestForLeapYearBoolean = False
            If i Mod 4I = 0I Then
                TestForLeapYearBoolean = True
                If i Mod 100I = 0I Then
                    TestForLeapYearBoolean = False
                    If i Mod 400I = 0I Then
                        TestForLeapYearBoolean = True
                    End If
                End If
            End If
            If TestForLeapYearBoolean Then
                completedLeapDays += 1I
            End If
        Next

        completedDays = completedDays + completedLeapDays - 5I

        TestForLeapYearBoolean = False
        If YearInt Mod 4I = 0I Then
            TestForLeapYearBoolean = True
            If YearInt Mod 100I = 0I Then
                TestForLeapYearBoolean = False
                If YearInt Mod 400I = 0I Then
                    TestForLeapYearBoolean = True
                End If
            End If
        End If

        Dim completedDaysInCurrentYear As Integer = 0I
        Select Case monthInt
            Case 1
                completedDaysInCurrentYear = dayInt - 1
            Case 2
                completedDaysInCurrentYear = 31I + dayInt - 1
            Case 3
                If TestForLeapYearBoolean = True Then
                    completedDaysInCurrentYear = 31I + 29I + dayInt - 1
                Else
                    completedDaysInCurrentYear = 31I + 28I + dayInt - 1
                End If
            Case 4
                If TestForLeapYearBoolean = True Then
                    completedDaysInCurrentYear = 31I + 29I + 31I + dayInt - 1
                Else
                    completedDaysInCurrentYear = 31I + 28I + 31I + dayInt - 1
                End If
            Case 5
                If TestForLeapYearBoolean = True Then
                    completedDaysInCurrentYear = 31I + 29I + 31I + 30I + dayInt - 1
                Else
                    completedDaysInCurrentYear = 31I + 28I + 31I + 30I + dayInt - 1
                End If
            Case 6
                If TestForLeapYearBoolean = True Then
                    completedDaysInCurrentYear = 31I + 29I + 31I + 30I + 31I + dayInt - 1
                Else
                    completedDaysInCurrentYear = 31I + 28I + 31I + 30I + 31I + dayInt - 1
                End If
            Case 7
                If TestForLeapYearBoolean = True Then
                    completedDaysInCurrentYear = 31I + 29I + 31I + 30I + 31I + 30I + dayInt - 1
                Else
                    completedDaysInCurrentYear = 31I + 28I + 31I + 30I + 31I + 30I + dayInt - 1
                End If
            Case 8
                If TestForLeapYearBoolean = True Then
                    completedDaysInCurrentYear = 31I + 29I + 31I + 30I + 31I + 30I + 31I + dayInt - 1
                Else
                    completedDaysInCurrentYear = 31I + 28I + 31I + 30I + 31I + 30I + 31I + dayInt - 1
                End If
            Case 9
                If TestForLeapYearBoolean = True Then
                    completedDaysInCurrentYear = 31I + 29I + 31I + 30I + 31I + 30I + 31I + 31I + dayInt - 1
                Else
                    completedDaysInCurrentYear = 31I + 28I + 31I + 30I + 31I + 30I + 31I + 31I + dayInt - 1
                End If
            Case 10
                If TestForLeapYearBoolean = True Then
                    completedDaysInCurrentYear = 31I + 29I + 31I + 30I + 31I + 30I + 31I + 31I + 30I + dayInt - 1
                Else
                    completedDaysInCurrentYear = 31I + 28I + 31I + 30I + 31I + 30I + 31I + 31I + 30I + dayInt - 1
                End If
            Case 11
                If TestForLeapYearBoolean = True Then
                    completedDaysInCurrentYear = 31I + 29I + 31I + 30I + 31I + 30I + 31I + 31I + 30I + 31I + dayInt - 1
                Else
                    completedDaysInCurrentYear = 31I + 28I + 31I + 30I + 31I + 30I + 31I + 31I + 30I + 31I + dayInt - 1
                End If
            Case 12
                If TestForLeapYearBoolean = True Then
                    completedDaysInCurrentYear = 31I + 29I + 31I + 30I + 31I + 30I + 31I + 31I + 30I + 31I + 30I + dayInt - 1
                Else
                    completedDaysInCurrentYear = 31I + 28I + 31I + 30I + 31I + 30I + 31I + 31I + 30I + 31I + 30I + dayInt - 1
                End If
        End Select

        completedDays += completedDaysInCurrentYear

        Dim completedSeconds As Decimal = Convert.ToDecimal(completedDays) * 86400D + Convert.ToDecimal(Hours) * 3600D + Convert.ToDecimal(Minutes) * 60D + Seconds
        Dim gpsWeeksDecimal As Decimal = completedSeconds / 604800D
        Dim gpsWeeksDecimalTrunc As Decimal = Decimal.Truncate(gpsWeeksDecimal)
        Dim gpsSecondsDecimal As Decimal = (gpsWeeksDecimal - gpsWeeksDecimalTrunc) * 604800D

        time = Convert.ToInt64(gpsWeeksDecimalTrunc * 604800) + Convert.ToInt64(gpsSecondsDecimal)

    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim ShutOffData() As Byte = {C2B(10), C2B(2), C2B(8), C2B(0)}
        Dim ShutOffSum As Byte = CalcSum(ShutOffData)
        Dim ShutOff() As Byte = {C2B(16), C2B(10), C2B(2), C2B(8), C2B(0), ShutOffSum, C2B(16), C2B(3)}

        Dim boolOpenOK As Boolean = True
        If serialPort2.IsOpen = False Then
            Try
                serialPort2.Open()
            Catch
                boolOpenOK = False
            End Try
        End If

        If boolOpenOK Then
            LastCommand(0) = 10
            LastCommand(1) = 8
            abortTransfer()
            System.Threading.Thread.Sleep(500)
            serialPort2.Write(ShutOff, 0, ShutOff.Length)
            System.Threading.Thread.Sleep(500)
            serialPort2.Close()
            Button4.Enabled = False
            Button5.Enabled = False
            Label43.Text = "No Garmin GPS Device Connected"
            Label43.Refresh()
        End If
    End Sub

    Private Sub abortTransfer()
        Dim AbortData() As Byte = {C2B(10), C2B(2), C2B(0), C2B(0)}
        Dim AbortSum As Byte = CalcSum(AbortData)
        Dim AbortCurrent() As Byte = {C2B(16), C2B(10), C2B(2), C2B(0), C2B(0), AbortSum, C2B(16), C2B(3)}

        LastCommand(0) = 10
        LastCommand(1) = 0
        Try
            serialPort2.Write(AbortCurrent, 0, AbortCurrent.Length)
        Catch
        End Try
        calcTime(timeOfLastWrite)
        Application.DoEvents()
    End Sub

    Private Delegate Sub commEvent()

    Private Sub Rx(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles serialPort2.DataReceived

        If GPS_Connected = False Then
            Dim del As commEvent
            del = New commEvent(AddressOf serialPort_ReceivedData)
            del.Invoke()
        ElseIf AlmanacDownload = True Then
            Try
                If timeOutErrorFlag = False Or ErrorFlag = False Then
                    DataGrid.Invoke(New commEvent(AddressOf serialPort_ReceivedData))

                    If almanacCounter > AlmanacPoints Then
                        myTimer.Stop()
                        If AlmanacPoints > 0 Then
                            MessageBox.Show("Finished Downloading Almanac Data from your Garmin GPS Device", "DOWNLOAD COMPLETE")
                            serialPort2.Close()
                            serialPort2.Dispose()
                            Button5.Invoke(New commEvent(AddressOf enableButton5))
                            AlmanacDownload = False
                        Else
                            MessageBox.Show("There are no Almanac Records stored within your Garmin GPS Device", "NO ALMANAC TO DOWNLOAD")
                            serialPort2.Close()
                            serialPort2.Dispose()
                            Button2.Invoke(New commEvent(AddressOf enableButton5))
                            AlmanacDownload = False
                        End If
                    End If
                End If
            Catch timeOutError As TimeoutException
                timeOutErrorFlag = True
            Catch indexRange As ArgumentOutOfRangeException
                ErrorFlag = True
            End Try
        End If
    End Sub

    Private Sub serialPort_ReceivedData()

        If timeOutErrorFlag = False And ErrorFlag = False Then

            Try
                'Application.DoEvents()

                If GPS_Connected = False Then

                    For j As Integer = 1 To 10
                        Dim ProductData1() As Byte = getPacket()

                        If ProductData1(1) = C2B(6) And ProductData1(3) = C2B(254) Then

                            Dim ProductData2() As Byte = getPacket()
                            Dim storage(1) As Byte

                            storage(0) = ProductData2(3)
                            storage(1) = ProductData2(4)
                            ProductID = BitConverter.ToUInt16(storage, 0)

                            storage(0) = ProductData2(5)
                            storage(1) = ProductData2(6)
                            SoftwareVersion = Convert.ToDouble(BitConverter.ToInt16(storage, 0))
                            SoftwareVersion /= 100

                            DeviceInfo = String.Empty

                            Dim i As Integer = 7

                            While True
                                If ProductData2(i) = 0 Then
                                    Exit While
                                End If
                                DeviceInfo &= Convert.ToChar(ProductData2(i))
                                i += 1
                            End While

                            sendACK(ProductData2(1))

                        ElseIf ProductData1(1) = C2B(253) Then
                            getProtocol(ProductData1)
                            sendACK(ProductData1(1))
                            GPS_Connected = True
                            Exit For
                        Else
                            'Dim PIDnData() As Byte = {C2B(254), C2B(0)}
                            'Dim Sum As Byte = CalcSum(PIDnData)
                            'Dim sendInit() As Byte = {C2B(16), C2B(254), C2B(0), Sum, C2B(16), C2B(3)}

                            'LastCommand(0) = 254
                            'LastCommand(1) = 0
                            'serialPort2.Write(sendInit, 0, sendInit.Length)
                            'calcTime(timeOfLastWrite)
                            Application.DoEvents()
                        End If
                    Next
                Else
                    'byte arrays used to store received packet data
                    Dim Data1() As Byte = getPacket()
                    Dim Data2() As Byte

                    'device information packet
                    If Data1(1) = C2B(6) And Data1(3) = C2B(254) Then
                        Data2 = getPacket()
                        sendACK(Data2(1))

                        'device protocols packet
                    ElseIf Data1(1) = C2B(253) Then
                        getProtocol(Data1)
                        sendACK(Data1(1))
                        If checkAlmanacProtocols() And AlmanacDownload Then
                            LastCommand(0) = 10
                            LastCommand(1) = 1
                            serialPort2.Write(command, 0, command.Length)
                            calcTime(timeOfLastWrite)
                            Application.DoEvents()
                        ElseIf checkAlmanacProtocols() = False And AlmanacDownload Then
                            MessageBox.Show("The Garmin GPS Device connected does not utilize the Almanac transfer protocol implemented by this application." & vbNewLine & _
                            "Unable to download Almanac data from this Garmin GPS Device, sorry for the inconvenience!", "ALMANAC TRANSFER PROTOCOL NOT RECOGNIZED", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            abortTransfer()
                        End If

                        'ACK packet (device successfully received our last communication)
                    ElseIf Data1(1) = C2B(6) Then
                        'ACK packet (means device successfully received our request to transfer data)
                        If Data1(3) = C2B(10) Then
                            If LastCommand(0) = C2B(10) And LastCommand(1) = C2B(1) Then
                                'System.Threading.Thread.Sleep(500)
                                Data2 = getPacket()
                                If Data2.Length = 1 Then
                                    abortTransfer()
                                    ErrorFlag = True
                                    myTimer.Stop()

                                    MessageBox.Show("The connected Garmin GPS Device is not responding to the Almanac Transfer Protocol." & vbNewLine & _
                                    "Some Garmin GPS receivers require that GPS is currently ""ON"" within the unit before Almanac data can be downloaded!" & _
                                    vbNewLine & "Ensure your unit is set to, ""Use with GPS On"" and re-attempt to download the Almanac", "ENSURE GPS IS ON", MessageBoxButtons.OK, MessageBoxIcon.Warning)

                                    Button5.Enabled = True

                                    'Dim SendAlmanReq() As Byte = {C2B(10), C2B(2), C2B(1), C2B(0)}
                                    'Dim Sum2 As Byte = CalcSum(SendAlmanReq)

                                    'command(0) = C2B(16)
                                    'command(1) = C2B(10)
                                    'command(2) = C2B(2)
                                    'command(3) = C2B(1)
                                    'command(4) = C2B(0)
                                    'command(5) = Sum2
                                    'command(6) = C2B(16)
                                    'command(7) = C2B(3)

                                    'LastCommand(0) = 10
                                    'LastCommand(1) = 1
                                    'serialPort2.Write(command, 0, command.Length)
                                    'calcTime(timeOfLastWrite)
                                    'System.Threading.Thread.Sleep(500)
                                    'Application.DoEvents()

                                    'number of records packet (Almanac)
                                ElseIf Data2(1) = C2B(27) Then
                                    Dim records(1) As Byte
                                    records(0) = Data2(3)
                                    records(1) = Data2(4)
                                    AlmanacPoints = BitConverter.ToUInt16(records, 0)
                                    sendACK(Data2(1))
                                End If
                            End If
                        End If
                        'almanac record
                    ElseIf Data1(1) = C2B(31) Then
                        almanacCounter += 1

                        Dim week_num As UInt16
                        Dim toa As Single
                        Dim af0 As Single
                        Dim af1 As Single
                        Dim e As Single
                        Dim a As Single
                        Dim mean_Anom As Single
                        Dim omega As Single
                        Dim OMEGA0 As Single
                        Dim OMEGA_dot As Single
                        Dim i As Single
                        Dim PRN As Byte
                        Dim health As Byte

                        If Alman_Data_Type = 500 Or Alman_Data_Type = 501 Then
                            PRN = Convert.ToByte(almanacCounter)
                            week_num = BitConverter.ToUInt16(Data1, 3)
                            toa = BitConverter.ToSingle(Data1, 5)
                            af0 = BitConverter.ToSingle(Data1, 9)
                            af1 = BitConverter.ToSingle(Data1, 13)
                            e = BitConverter.ToSingle(Data1, 17)
                            a = BitConverter.ToSingle(Data1, 21)
                            a *= a
                            mean_Anom = BitConverter.ToSingle(Data1, 25)
                            omega = BitConverter.ToSingle(Data1, 29)
                            OMEGA0 = BitConverter.ToSingle(Data1, 33)
                            OMEGA_dot = BitConverter.ToSingle(Data1, 37)
                            i = BitConverter.ToSingle(Data1, 41)

                            If Alman_Data_Type = 501 Then
                                health = Data1(45)
                            Else
                                health = 255
                            End If

                        ElseIf Alman_Data_Type = 550 Or Alman_Data_Type = 551 Then
                            PRN = Convert.ToByte(Data1(3))
                            PRN += 1    'listed PRNs are 0 based for PRN # 1
                            week_num = BitConverter.ToUInt16(Data1, 4)
                            toa = BitConverter.ToSingle(Data1, 6)
                            af0 = BitConverter.ToSingle(Data1, 10)
                            af1 = BitConverter.ToSingle(Data1, 14)
                            e = BitConverter.ToSingle(Data1, 18)
                            a = BitConverter.ToSingle(Data1, 22)
                            a *= a
                            mean_Anom = BitConverter.ToSingle(Data1, 26)
                            omega = BitConverter.ToSingle(Data1, 30)
                            OMEGA0 = BitConverter.ToSingle(Data1, 34)
                            OMEGA_dot = BitConverter.ToSingle(Data1, 38)
                            i = BitConverter.ToSingle(Data1, 42)

                            If Alman_Data_Type = 501 Then
                                health = Data1(46)
                            Else
                                health = 255
                            End If
                        End If

                        If week_num <> 65535 Then
                            week_num += 1024
                            Dim healthString As String
                            If health = 0 Then
                                healthString = "Good"
                            ElseIf health = 255 Then
                                healthString = "Unknown"
                            Else
                                healthString = "Bad"
                            End If
                            Dim almanacdata() As String = {PRN.ToString, week_num.ToString, toa.ToString, af0.ToString("f8"), af1.ToString("f13"), e.ToString("f8"), a.ToString("f3"), mean_Anom.ToString("f8"), omega.ToString("f8"), OMEGA0.ToString("f8"), OMEGA_dot.ToString("f13"), i.ToString("f8"), healthString}
                            DataGrid.Rows.Add(almanacdata)
                        Else
                            Dim almanacdata() As String = {PRN.ToString, "N/A"}
                            DataGrid.Rows.Add(almanacdata)
                        End If
                        sendACK(Data1(1))

                        'transfer complete packet found
                    ElseIf Data1(1) = C2B(12) Then
                        almanacCounter += 1
                        Dim endTracks() As String = {"END", "OF", "ALMANAC", "DATA"}
                        DataGrid.Rows.Add(endTracks)
                        sendACK(Data1(1))

                        'used for debuggin purposes
                    Else
                        'printPacket(Data1, "Unrecognized Packet")
                        'MessageBox.Show("Unrecognized Packet ID received", "Packet ID: " & Data1(1).ToString & " 2")
                    End If
                End If

            Catch
                ErrorFlag = True
            End Try
        End If
    End Sub

    Private Sub myTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles myTimer.Tick
        Dim currentTime As Long = 0
        calcTime(currentTime)

        If timeOfLastWrite <> -9999 Then
            If currentTime - timeOfLastWrite > 3 Then
                myTimer.Stop()
                timeOutErrorFlag = True
                ErrorFlag = True
                abortTransfer()
                MessageBox.Show("A timeout error has occured! The application has been waiting for more than 3 seconds to receive data from your Garmin GPS Device." & vbNewLine & vbNewLine & _
                "The download has been cancelled!" & vbNewLine & vbNewLine & "Please check the connection between your Garmin GPS Device and this PC before attempting the download again.", "COMMUNICATION TIMEOUT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Button2.Enabled = True
                ProgressBar1.Value = ProgressBar1.Minimum
                Button5.Enabled = True
                Button4.Enabled = True
                AlmanacDownload = False
            End If
        End If
    End Sub

    Private Function checkAlmanacProtocols() As Boolean

        Dim protocol1 As Boolean = False    'L1 Protocal Test
        Dim protocol2 As Boolean = False    'A10 Protocal Test
        Dim protocol3 As Boolean = False    'A500 Protocal Test
        Dim protocol4 As Boolean = False    'D500/D501/D550/D551 Protocal Test

        For i As Integer = 0 To ProtocolArray.Length - 1
            If ProtocolArray(i).tag.ToUpper = "L" And ProtocolArray(i).Data = Convert.ToUInt16(1) Then
                protocol1 = True
            End If
            If ProtocolArray(i).tag.ToUpper = "A" And ProtocolArray(i).Data = Convert.ToUInt16(10) Then
                protocol2 = True
            End If
            If ProtocolArray(i).tag.ToUpper = "A" And ProtocolArray(i).Data = Convert.ToUInt16(500) Then
                protocol3 = True
            End If
            If ProtocolArray(i).tag.ToUpper = "D" And ProtocolArray(i).Data = Convert.ToUInt16(500) Then
                protocol4 = True
                Alman_Data_Type = 500
            End If
            If ProtocolArray(i).tag.ToUpper = "D" And ProtocolArray(i).Data = Convert.ToUInt16(501) Then
                protocol4 = True
                Alman_Data_Type = 501
            End If
            If ProtocolArray(i).tag.ToUpper = "D" And ProtocolArray(i).Data = Convert.ToUInt16(550) Then
                protocol4 = True
                Alman_Data_Type = 550
            End If
            If ProtocolArray(i).tag.ToUpper = "D" And ProtocolArray(i).Data = Convert.ToUInt16(551) Then
                protocol4 = True
                Alman_Data_Type = 551
            End If
        Next
        Dim result As Boolean = False

        If protocol1 And protocol2 And protocol3 And protocol4 Then
            result = True
        Else
            myTimer.Stop()
        End If

        Return result

    End Function

    Private Sub getProtocol(ByVal packet() As Byte)

        Dim size As Integer = packet(2) / 3
        Dim byteArray(1) As Byte

        ReDim ProtocolArray(size - 1)
        Dim element As Integer = 0

        For i As Integer = 3 To (3 * size) Step 3
            ProtocolArray(element).tag = Convert.ToChar(packet(i))
            byteArray(0) = packet(i + 1)
            byteArray(1) = packet(i + 2)
            ProtocolArray(element).Data = BitConverter.ToUInt16(byteArray, 0)
            element += 1
        Next
    End Sub

    Private Function getPacket() As Byte()

        Dim Data(0) As Byte
        Dim indivByte As Byte
        Dim counter As Integer = 0
        Dim boolTest As Boolean = True
        Dim firstByte As Byte

        While boolTest
            Try
                If counter > 0 Then
                    ReDim Preserve Data(Data.Length)
                    indivByte = serialPort2.ReadByte()
                Else
                    firstByte = serialPort2.ReadByte
                    indivByte = firstByte
                End If

                If firstByte = 16 Then
                    Data(counter) = indivByte

                    If indivByte = 16 Then
                        Dim followUpByte As Byte = serialPort2.ReadByte()
                        If followUpByte = 3 Then
                            ReDim Preserve Data(Data.Length)
                            Data(counter + 1) = 3
                            boolTest = False
                        ElseIf followUpByte = 16 Then
                        Else
                            ReDim Preserve Data(Data.Length)
                            Data(counter + 1) = followUpByte
                            counter += 1
                        End If
                    End If
                    counter += 1
                Else
                    ReDim Data(3)
                    Data(0) = 0
                    Data(1) = 0
                    Data(2) = 0
                    Data(3) = 0
                    ErrorFlag = True
                    timeOutErrorFlag = True
                    Application.DoEvents()
                    Exit While
                End If
            Catch
                Exit While
            End Try

        End While
        Return Data

    End Function

    Private Sub printPacket(ByVal packet() As Byte, ByVal title As String)
        Dim dataString As String = String.Empty

        For i As Integer = 1 To packet.Length
            dataString &= packet(i - 1).ToString & "   "
            If i Mod 4 = 0 Then
                dataString &= vbNewLine
            End If
        Next
        MessageBox.Show(dataString, title)
    End Sub

    Private Sub sendACK(ByVal packetID As Byte)

        Dim extraDLE As Integer = 0

        If packetID = C2B(16) Then
            extraDLE = 1
        End If

        Dim ACKInit() As Byte = {C2B(6), C2B(2), packetID, C2B(0)}
        Dim sum As Byte = CalcSum(ACKInit)

        If sum = C2B(16) Then
            extraDLE += 1
        End If

        Dim ACK(7 + extraDLE) As Byte
        ACK(0) = C2B(16)
        ACK(1) = C2B(6)
        ACK(2) = C2B(2)
        ACK(3) = packetID

        Dim update1 As Integer = 0
        If packetID = C2B(16) Then
            ACK(4) = C2B(16)
            ACK(5) = C2B(0)
            update1 = 1
        Else
            ACK(4) = C2B(0)
        End If

        ACK(5 + update1) = sum

        If sum = C2B(16) Then
            ACK(6 + update1) = C2B(16)
            ACK(7 + update1) = C2B(16)
            ACK(8 + update1) = C2B(3)
        Else
            ACK(6 + update1) = C2B(16)
            ACK(7 + update1) = C2B(3)
        End If

        'LastCommand(0) = 6
        'LastCommand(1) = packetID
        serialPort2.Write(ACK, 0, ACK.Length)
        calcTime(timeOfLastWrite)
        Application.DoEvents()

    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        DataGrid.Rows.Clear()
        Button5.Enabled = False
        myTimer.Start()
        AlmanacDownload = True

        timeOutErrorFlag = False
        ErrorFlag = False
        almanacCounter = 0

        If serialPort2.IsOpen = False Then
            serialPort2.Open()
        End If

        abortTransfer()

        Dim PIDnData() As Byte = {C2B(254), C2B(0)}
        Dim Sum As Byte = CalcSum(PIDnData)
        Dim sendInit() As Byte = {C2B(16), C2B(254), C2B(0), Sum, C2B(16), C2B(3)}
        Dim SendAlmanReq() As Byte = {C2B(10), C2B(2), C2B(1), C2B(0)}
        Dim Sum2 As Byte = CalcSum(SendAlmanReq)

        command(0) = C2B(16)
        command(1) = C2B(10)
        command(2) = C2B(2)
        command(3) = C2B(1)
        command(4) = C2B(0)
        command(5) = Sum2
        command(6) = C2B(16)
        command(7) = C2B(3)

        LastCommand(0) = 254
        LastCommand(1) = 0
        serialPort2.Write(sendInit, 0, sendInit.Length)
        calcTime(timeOfLastWrite)
        Application.DoEvents()
    End Sub

    Private Sub enableButton5()
        Button5.Enabled = True
    End Sub

    Private Sub IonoCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IonoCheckBox.CheckedChanged
        ionGroupBox.Enabled = IonoCheckBox.Checked
        If IonoCheckBox.Checked = False Then
            TGDCheckBox.Enabled = True
        Else
            If IonoFreeRadioButton.Checked = True Then
                TGDCheckBox.Enabled = False
            Else
                TGDCheckBox.Enabled = True
            End If
        End If
    End Sub

    Private Sub IonoFreeRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IonoFreeRadioButton.CheckedChanged
        TGDCheckBox.Enabled = Not IonoFreeRadioButton.Checked
    End Sub

    Private Sub TGDCheckBox_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TGDCheckBox.EnabledChanged
        TGDCheckBox.Checked = TGDCheckBox.Enabled
    End Sub

    Private Sub satOptionsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles satOptionsButton.Click, satOptions2Button.Click
        SatOptions.ShowDialog()
    End Sub

    Private Sub orthoRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles orthoRadioButton.CheckedChanged
        If orthoRadioButton.Checked Then
            sfGroupBox.Enabled = False
            sfGroupBox.Text = "Combined Scale Factor = 1.0000"
            If GPSLocalBeenCalcd = True And PointList.RowCount <> 0 Then
                Call computeLocalPoints()
            End If
        Else
            sfGroupBox.Enabled = True
            sfGroupBox.Text = "Combined Scale Factor"
        End If
    End Sub

    Private Sub transMerRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles transMerRadioButton.CheckedChanged
        If transMerRadioButton.Checked And GPSLocalBeenCalcd = True And PointList.RowCount <> 0 Then
            Call computeLocalPoints()
        End If
    End Sub

    Private Sub stereoRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles stereoRadioButton.CheckedChanged
        If stereoRadioButton.Checked And GPSLocalBeenCalcd = True And PointList.RowCount <> 0 Then
            Call computeLocalPoints()
        End If
    End Sub

    Private Sub UTMRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UTMRadioButton.CheckedChanged
        If UTMRadioButton.Checked Then
            utmZoneGroupBox.Enabled = True
            sfGroupBox.Enabled = False
            sfGroupBox.Text = "Combined Scale Factor = 0.9996"
            If PointList.RowCount <> 0 Then
                Call computeLocalPoints()
            End If
        Else
            utmZoneGroupBox.Enabled = False
            sfGroupBox.Enabled = True
            sfGroupBox.Text = "Combined Scale Factor"
        End If
    End Sub

    Private Sub PGHRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PGHRadioButton.CheckedChanged
        If PGHRadioButton.Checked Then
            PGHTextBox.Enabled = True
            PGHLabel.Enabled = True
            PGHTextBox.Focus()
            PGHTextBox.SelectAll()
        Else
            PGHTextBox.Enabled = False
            PGHLabel.Enabled = False
        End If
    End Sub

    Private Sub USFRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles USFRadioButton.CheckedChanged
        If USFRadioButton.Checked Then
            USFTextBox.Enabled = True
            USFTextBox.Focus()
            USFTextBox.SelectAll()
        Else
            USFTextBox.Enabled = False
        End If
    End Sub

    Private Sub PointList_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles PointList.LostFocus
        PointList.ClearSelection()
    End Sub

    Private Sub zoneRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles zoneRadioButton.CheckedChanged
        If zoneRadioButton.Checked Then
            zoneTextBox.Enabled = True
            Label2.Enabled = True
            zoneTextBox.Focus()
            zoneTextBox.SelectAll()
            Call UTMRadioButton_CheckedChanged(sender, e)
        Else
            zoneTextBox.Enabled = False
            Label2.Enabled = False
        End If
    End Sub

    Private Sub zoneTextBox_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles zoneTextBox.LostFocus
        If zoneTextBox.Text = String.Empty Or Integer.Parse(zoneTextBox.Text) < 0 Or Integer.Parse(zoneTextBox.Text) > 60 Then
            zoneTextBox.Text = "13"
        End If
    End Sub

    Private Sub autoRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles autoRadioButton.CheckedChanged
        Call UTMRadioButton_CheckedChanged(sender, e)
    End Sub

    Private Sub zoneTextBox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles zoneTextBox.TextChanged
        If zoneTextBox.Text <> String.Empty Then
            Call UTMRadioButton_CheckedChanged(sender, e)
        End If
    End Sub

    Structure localizationDetails
        Dim latOrigin As String
        Dim longOrigin As String
        Dim ellhOrigin As String
        Dim NOrigin As String
        Dim EOrigin As String
        Dim ElevOrigin As String
        Dim projType As String
        Dim scaleFactor As String
        Dim elevFactor As String
        Dim combFactor As String
        Dim Norientation As String
        Dim rotation As String
    End Structure

    Private Sub initLocalizationDetails(ByVal localizationSet As localizationDetails)
        localizationSet.combFactor = "N/A"
        localizationSet.elevFactor = "N/A"
        localizationSet.ElevOrigin = "N/A"
        localizationSet.ellhOrigin = "N/A"
        localizationSet.EOrigin = "N/A"
        localizationSet.latOrigin = "N/A"
        localizationSet.longOrigin = "N/A"
        localizationSet.Norientation = "N/A"
        localizationSet.NOrigin = "N/A"
        localizationSet.projType = "N/A"
        localizationSet.rotation = "N/A"
        localizationSet.scaleFactor = "N/A"

    End Sub

    Private Sub localDetailsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles localDetailsButton.Click
        If UTMRadioButton.Checked = True Then
            localDetailsSet.latOrigin = "Latitude = 0" & Chr(176) & " 00' 00.00000"""
            localDetailsSet.longOrigin = "Longitude = Potentially Various"
            localDetailsSet.ellhOrigin = "Ellipsoid Height = 0.000 m"
            localDetailsSet.NOrigin = "Northing = 0 m (N) or 10,000,000 m (S)"
            localDetailsSet.EOrigin = "Easting = 500,000 m for each zone"
            localDetailsSet.ElevOrigin = "Elevation = N/A"

            If zoneRadioButton.Checked = True Then

                Dim zoneInt As Integer = Integer.Parse(zoneTextBox.Text)
                Dim BaseLong As Integer = -183 + zoneInt * 6
                localDetailsSet.longOrigin = "Longitude = " & BaseLong.ToString("##0") & Chr(176) & " 00' 00.00000"""
                localDetailsSet.EOrigin = "Easting = 500,000 m"
            End If

            localDetailsSet.scaleFactor = "Scale Factor = 0.9996000000"
            localDetailsSet.elevFactor = "Elevation Factor = 1.0000000000"
            localDetailsSet.combFactor = "Combined Scale Factor = 0.9996000000"
            localDetailsSet.Norientation = "North Axis = Geodetic North"
            localDetailsSet.rotation = "Rotation = 00" & Chr(176) & " 00' 00"""
            localDetailsSet.projType = "Type = Transverse Mercator"

            localDetails.ShowDialog()
        ElseIf GPSLocalBeenCalcd = True Then
            localDetailsSet.Norientation = "North Axis = Geodetic North"
            localDetailsSet.rotation = "Rotation = 00" & Chr(176) & " 00' 00"""

            localDetails.ShowDialog()
        Else
            MessageBox.Show("Not all localization parameters have been specified" & vbNewLine & "and hence no details are available", "NO LOCALIZATION DETAILS", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub geoidModelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles geoidModelButton.Click
        geoidBYNfileReader.ShowDialog()
    End Sub

    Private Sub topconRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles topconRadioButton.CheckedChanged
        If topconRadioButton.Checked = True Then
            EGHRadioButton.Checked = True
            stereoRadioButton.Checked = True
            If GPSLocalBeenCalcd = True And PointList.RowCount <> 0 And CommercialSoftwareSelection = 4 Then
                Call computeLocalPoints()
            End If
            CommercialSoftwareSelection = 1
        End If
    End Sub

    Private Sub trimbleRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles trimbleRadioButton.CheckedChanged
        If trimbleRadioButton.Checked = True Then
            PGHRadioButton.Checked = True
            transMerRadioButton.Checked = True
            If GPSLocalBeenCalcd = True And PointList.RowCount <> 0 And CommercialSoftwareSelection = 3 Then
                Call computeLocalPoints()
            End If
            CommercialSoftwareSelection = 2
        End If
    End Sub

    Private Sub leicaRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles leicaRadioButton.CheckedChanged
        If leicaRadioButton.Checked = True Then
            EGHRadioButton.Checked = True
            transMerRadioButton.Checked = True
            If GPSLocalBeenCalcd = True And PointList.RowCount <> 0 And CommercialSoftwareSelection = 2 Then
                Call computeLocalPoints()
            End If
            CommercialSoftwareSelection = 3
        End If
    End Sub

    Private Sub tdsRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tdsRadioButton.CheckedChanged
        If tdsRadioButton.Checked = True Then
            EGHRadioButton.Checked = True
            stereoRadioButton.Checked = True
            If GPSLocalBeenCalcd = True And PointList.RowCount <> 0 And CommercialSoftwareSelection = 1 Then
                Call computeLocalPoints()
            End If
            CommercialSoftwareSelection = 4
        End If
    End Sub

    Private Sub inverseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles inverseButton.Click
        inverseForm.ShowDialog()
    End Sub

    Private Sub GroupBox3_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles GroupBox3.DoubleClick
        LocalLatTextBox.Text = "50241381424"
        LocalLongTextBox.Text = "105330491677"
        LocalHeightTextBox.Text = "553.347"
        NTextBox.Text = "10000.000"
        ETextBox.Text = "10000.000"
        ElevTextBox.Text = "573.036"
    End Sub

    Private Sub displayC1ObsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles displayC1ObsButton.Click
        Dim numObs As Integer = SelectedObsFile.NumCols(ObsEpochsComboBox.SelectedIndex)

        Dim c1ObsNum As Integer = 0

        For i As Integer = 1 To numObs
            If SelectedObsFile.Obs_Type(i - 1).ToUpper = "C1" Then
                c1ObsNum = i
                Exit For
            End If
        Next

        If c1ObsNum <> 0 Then
            c1ObsForm.c1ObsDataGridView.Rows.Clear()
            Dim numRows As Integer = SelectedObsFile.NumRows(ObsEpochsComboBox.SelectedIndex)
            For i As Integer = 5 To numRows
                Dim SatNum As Integer = SelectedObsFile.ObsData(i - 1, 0, ObsEpochsComboBox.SelectedIndex)
                If SelectedObsFile.ObsData(i - 1, 0, ObsEpochsComboBox.SelectedIndex) <> -9999 AndAlso PRNs2UseBoolean(SatNum) Then
                    Dim obsData() As String = {SelectedObsFile.ObsData(i - 1, 0, ObsEpochsComboBox.SelectedIndex).ToString, SelectedObsFile.ObsData(i - 1, c1ObsNum, ObsEpochsComboBox.SelectedIndex).ToString}
                    If obsData(0).Length = 1 Then
                        obsData(0) = "0" & obsData(0)
                    End If
                    c1ObsForm.c1ObsDataGridView.Rows.Add(obsData)
                End If
            Next
            c1ObsForm.c1ObsDataGridView.Sort(c1ObsForm.c1ObsDataGridView.Columns(0), System.ComponentModel.ListSortDirection.Ascending)
            c1ObsForm.Show()
        Else
            MessageBox.Show("The C/A Observations on L1 for this epoch could not be found", "MISSING C/A OBSERVATIONS", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub

    Private Sub satsXYZButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles satsXYZButton.Click
        Dim i, j, k, m, z As Integer
        Dim SatNum As Integer
        Dim PRN As Integer
        Dim ReceptionTime(1), ReceptionTime2(1), EphemerisTime(1) As Decimal
        Dim TimeDiffFromReference As Decimal
        Dim Eph2Use As New RINEX_Eph
        Dim pseudorange, transmitDistance As Decimal
        Dim pseudorangeIndex As Integer = -9999I
        Dim ECEF_XYZ_SatPosition(0 To 3) As Decimal
        Dim ECEF_XYZ_SatPosition2(0 To 3) As Decimal
        Dim TempRINEX_Nav As New RINEX_Nav
        Dim Sat_Clock_Corr, Rel_Corr, TGD_Corr As Decimal

        Dim CartPosition(2) As Decimal
        Dim CurvPosition() As Decimal

        CartPosition(0) = SelectedObsFile.Approx_X
        CartPosition(1) = SelectedObsFile.Approx_Y
        CartPosition(2) = SelectedObsFile.Approx_Z
        CurvPosition = cart2curv(CartPosition)

        'define rotation matrices to convert ECEF to Local E,N,Up
        Dim R1, R3 As New Matrix(3, 3)
        R1.data(1, 1) = 1D
        R1.data(1, 2) = 0D
        R1.data(1, 3) = 0D
        R1.data(2, 1) = 0D
        R1.data(2, 2) = Math.Cos((90 - CurvPosition(0)) * Math.PI / 180D)
        R1.data(2, 3) = Math.Sin((90 - CurvPosition(0)) * Math.PI / 180D)
        R1.data(3, 1) = 0D
        R1.data(3, 2) = -1 * Math.Sin((90 - CurvPosition(0)) * Math.PI / 180D)
        R1.data(3, 3) = Math.Cos((90 - CurvPosition(0)) * Math.PI / 180D)

        R3.data(1, 1) = Math.Cos((90 + CurvPosition(1)) * Math.PI / 180D)
        R3.data(1, 2) = Math.Sin((90 + CurvPosition(1)) * Math.PI / 180D)
        R3.data(1, 3) = 0D
        R3.data(2, 1) = -1 * Math.Sin((90 + CurvPosition(1)) * Math.PI / 180D)
        R3.data(2, 2) = Math.Cos((90 + CurvPosition(1)) * Math.PI / 180D)
        R3.data(2, 3) = 0D
        R3.data(3, 1) = 0D
        R3.data(3, 2) = 0D
        R3.data(3, 3) = 1D

        'locate the C/A code ("C1") pseudorange observable
        For m = 0 To 17
            If SelectedObsFile.Obs_Type(m).ToUpper = "C1" Then
                pseudorangeIndex = m
            End If
        Next

        If pseudorangeIndex <> -9999I Then
            z = ObsEpochsComboBox.SelectedIndex

            ReceptionTime(0) = SelectedObsFile.ObsData(0, 0, z)
            ReceptionTime(1) = SelectedObsFile.ObsData(0, 1, z)

            ReceptionTime2(0) = ReceptionTime(0)
            ReceptionTime2(1) = ReceptionTime(1) + 10

            If ReceptionTime2(1) >= 604800 Then
                ReceptionTime2(1) -= 604800
                ReceptionTime2(0) += 1
            End If

            allSatsXYZForm.allSatsXYZDataGridView.Rows.Clear()
            For i = 4 To SelectedObsFile.NumRows(z) - 1
                TimeDiffFromReference = 1000000000000.0
                pseudorange = 0
                transmitDistance = 0
                SatNum = SelectedObsFile.ObsData(i, 0, z)

                If SatNum <> -9999I AndAlso PRNs2UseBoolean(SatNum) Then
                    Eph2Use = New RINEX_Eph
                    For j = 0 To SelectedNavFile.EphemerisV.GetLength(1) - 1
                        For k = 0 To SelectedNavFile.EphemerisV.GetLength(0) - 1
                            PRN = SelectedNavFile.EphemerisV(k, j).PRN
                            If SatNum = PRN Then
                                EphemerisTime(0) = SelectedNavFile.EphemerisV(k, j).Toe_Week
                                EphemerisTime(1) = SelectedNavFile.EphemerisV(k, j).Toe
                                If Math.Abs((EphemerisTime(0) * 604800D + EphemerisTime(1)) - (ReceptionTime(0) * 604800D + ReceptionTime(1))) < TimeDiffFromReference Then
                                    TimeDiffFromReference = Math.Abs((EphemerisTime(0) * 604800D + EphemerisTime(1)) - (ReceptionTime(0) * 604800D + ReceptionTime(1)))
                                    Eph2Use = SelectedNavFile.EphemerisV(k, j)
                                End If
                            End If
                        Next
                    Next

                    pseudorange = SelectedObsFile.ObsData(i, (pseudorangeIndex + 1), z)

                    If pseudorange <> -9999D And pseudorange <> 0D Then
                        If Eph2Use.PRN <> -9999I Then
                            Sat_Clock_Corr = 0

                            If receptionRadioButton.Checked Then
                                transmitDistance = 0D
                            Else
                                transmitDistance = pseudorange
                            End If
                            Dim delta As Decimal = 1000
                            Dim shortStop As Boolean = False
                            While delta > 0.1 And Not shortStop
                                ECEF_XYZ_SatPosition = TempRINEX_Nav.ComputeSatellitePositionFromEphemeris(Sat_Clock_Corr, Rel_Corr, TGD_Corr, Eph2Use, ReceptionTime, transmitDistance, EarthRotCheckBox.Checked, SatClockCheckBox.Checked, True, True)
                                If receptionRadioButton.Checked Then
                                    shortStop = True
                                End If
                                If ECEF_XYZ_SatPosition(0) = 0 Then 'good position returned
                                    Dim newRange As Decimal = Math.Sqrt((ECEF_XYZ_SatPosition(1) - SelectedObsFile.Approx_X) ^ 2 + (ECEF_XYZ_SatPosition(2) - SelectedObsFile.Approx_Y) ^ 2 + (ECEF_XYZ_SatPosition(3) - SelectedObsFile.Approx_Z) ^ 2)
                                    delta = Math.Abs(newRange - transmitDistance)
                                    transmitDistance = newRange
                                Else
                                    Exit While
                                End If
                            End While

                            If ECEF_XYZ_SatPosition(0) = 0 Then
                                Dim E_N_Up, Temp As Matrix
                                Dim diff As New Matrix(3, 1)
                                diff.data(1, 1) = ECEF_XYZ_SatPosition(1) - SelectedObsFile.Approx_X
                                diff.data(2, 1) = ECEF_XYZ_SatPosition(2) - SelectedObsFile.Approx_Y
                                diff.data(3, 1) = ECEF_XYZ_SatPosition(3) - SelectedObsFile.Approx_Z
                                Temp = R3 * diff
                                E_N_Up = R1 * Temp

                                Dim Elev, Azimuth As Decimal
                                Elev = (Math.Atan2(E_N_Up.data(3, 1), Math.Sqrt(E_N_Up.data(2, 1) ^ 2D + E_N_Up.data(1, 1) ^ 2D))) * 180D / Math.PI
                                Azimuth = (Math.Atan2(E_N_Up.data(1, 1), E_N_Up.data(2, 1))) * 180D / Math.PI

                                If Azimuth < 0 Then
                                    Azimuth += 360D
                                End If

                                'code to check to see if a satellite is rising or setting
                                Dim elevString As String = String.Empty
                                ECEF_XYZ_SatPosition2 = TempRINEX_Nav.ComputeSatellitePositionFromEphemeris(Sat_Clock_Corr, Rel_Corr, TGD_Corr, Eph2Use, ReceptionTime2, transmitDistance, EarthRotCheckBox.Checked, SatClockCheckBox.Checked, True, True)

                                If ECEF_XYZ_SatPosition(0) = 0 Then
                                    Dim E_N_Up2, Temp2 As Matrix
                                    Dim diff2 As New Matrix(3, 1)
                                    diff2.data(1, 1) = ECEF_XYZ_SatPosition2(1) - SelectedObsFile.Approx_X
                                    diff2.data(2, 1) = ECEF_XYZ_SatPosition2(2) - SelectedObsFile.Approx_Y
                                    diff2.data(3, 1) = ECEF_XYZ_SatPosition2(3) - SelectedObsFile.Approx_Z
                                    Temp2 = R3 * diff2
                                    E_N_Up2 = R1 * Temp2

                                    Dim Elev2 As Decimal
                                    Elev2 = (Math.Atan2(E_N_Up2.data(3, 1), Math.Sqrt(E_N_Up2.data(2, 1) ^ 2D + E_N_Up2.data(1, 1) ^ 2D))) * 180D / Math.PI

                                    If Elev2 >= Elev Then
                                        elevString = Elev.ToString("f2") & " " & Convert.ToChar(&H1403)
                                    Else
                                        elevString = Elev.ToString("f2") & " " & Convert.ToChar(&H1401)
                                    End If
                                Else
                                    elevString = Elev.ToString("f2") & " ?"
                                End If

                                Dim messageString() As String = {SatNum.ToString("d2"), ECEF_XYZ_SatPosition(1).ToString("f3"), ECEF_XYZ_SatPosition(2).ToString("f3"), ECEF_XYZ_SatPosition(3).ToString("f3"), Azimuth.ToString("f2"), elevString}
                                allSatsXYZForm.allSatsXYZDataGridView.Rows.Add(messageString)
                            End If
                        End If
                    End If
                End If
            Next
            allSatsXYZForm.allSatsXYZDataGridView.Sort(allSatsXYZForm.allSatsXYZDataGridView.Columns(0), System.ComponentModel.ListSortDirection.Ascending)
            allSatsXYZForm.Show()
        End If
    End Sub

    Private Sub exportObsButton_Click(sender As Object, e As EventArgs) Handles exportObsButton.Click

        Dim SaveResult As DialogResult = SaveFileDialog2.ShowDialog
        If SaveResult = Windows.Forms.DialogResult.OK Then

            Dim exportFile As New StreamWriter(SaveFileDialog2.FileName)

            Dim i, q, startAt, stopAt, SatNum As Integer
            Dim ReceptionTime(1) As Decimal

            startAt = ObsEpochsComboBox.SelectedIndex
            stopAt = ObsEpochsComboBox.SelectedIndex + EndObsEpochsComboBox.SelectedIndex

            Dim numObs As Integer = SelectedObsFile.NumCols(startAt)

            Dim c1ObsNum As Integer = 0
            Dim p1ObsNum As Integer = 0
            Dim p2ObsNum As Integer = 0
            Dim l1ObsNum As Integer = 0
            Dim l2ObsNum As Integer = 0
            Dim d1ObsNum As Integer = 0
            Dim d2ObsNum As Integer = 0

            For j As Integer = 1 To numObs
                If SelectedObsFile.Obs_Type(j - 1).ToUpper = "C1" Then
                    c1ObsNum = j
                End If
                If SelectedObsFile.Obs_Type(j - 1).ToUpper = "P1" Then
                    p1ObsNum = j
                End If
                If SelectedObsFile.Obs_Type(j - 1).ToUpper = "P2" Then
                    p2ObsNum = j
                End If
                If SelectedObsFile.Obs_Type(j - 1).ToUpper = "L1" Then
                    l1ObsNum = j
                End If
                If SelectedObsFile.Obs_Type(j - 1).ToUpper = "L2" Then
                    l2ObsNum = j
                End If
                If SelectedObsFile.Obs_Type(j - 1).ToUpper = "D1" Then
                    d1ObsNum = j
                End If
                If SelectedObsFile.Obs_Type(j - 1).ToUpper = "D2" Then
                    d2ObsNum = j
                End If
            Next

            Dim satList As New List(Of Integer)
            For q = startAt To stopAt
                For a As Integer = 4 To SelectedObsFile.NumRows(q) - 1
                    Dim satPRN As Integer = SelectedObsFile.ObsData(a, 0, q)
                    If satPRN <> -9999I AndAlso PRNs2UseBoolean(satPRN) Then
                        If Not satList.Contains(satPRN) Then
                            satList.Add(satPRN)
                        End If
                    End If
                Next
            Next

            satList.Sort()
            For a As Integer = 0 To satList.Count - 1
                exportFile.Write(satList.Item(a).ToString & ",,C1,P1,P2,L1,L2,D1,D2,,")
            Next
            exportFile.WriteLine()

            For q = startAt To stopAt
                Dim SatObs As New Matrix(32, 8) 'last column keeps track if a satellite used found within this epoch (0=NO,1=YES)
                ReceptionTime(0) = SelectedObsFile.ObsData(0, 0, q)
                ReceptionTime(1) = SelectedObsFile.ObsData(0, 1, q)

                For i = 4 To SelectedObsFile.NumRows(q) - 1
                    SatNum = SelectedObsFile.ObsData(i, 0, q)

                    If SatNum <> -9999 AndAlso PRNs2UseBoolean(SatNum) Then
                        SatObs.data(SatNum, 8) = 1
                        If c1ObsNum <> 0 Then
                            SatObs.data(SatNum, 1) = SelectedObsFile.ObsData(i, c1ObsNum, q)
                        End If
                        If p1ObsNum <> 0 Then
                            SatObs.data(SatNum, 2) = SelectedObsFile.ObsData(i, p1ObsNum, q)
                        End If
                        If p2ObsNum <> 0 Then
                            SatObs.data(SatNum, 3) = SelectedObsFile.ObsData(i, p2ObsNum, q)
                        End If
                        If l1ObsNum <> 0 Then
                            SatObs.data(SatNum, 4) = SelectedObsFile.ObsData(i, l1ObsNum, q)
                        End If
                        If l2ObsNum <> 0 Then
                            SatObs.data(SatNum, 5) = SelectedObsFile.ObsData(i, l2ObsNum, q)
                        End If
                        If d1ObsNum <> 0 Then
                            SatObs.data(SatNum, 6) = SelectedObsFile.ObsData(i, d1ObsNum, q)
                        End If
                        If d2ObsNum <> 0 Then
                            SatObs.data(SatNum, 7) = SelectedObsFile.ObsData(i, d2ObsNum, q)
                        End If
                    End If
                Next

                For j As Integer = 0 To satList.Count - 1
                    Dim satPRN As Integer = satList.Item(j)
                    If SatObs.data(satPRN, 8) = 1 Then
                        exportFile.Write(Math.Round(ReceptionTime(0), 0).ToString & "," & Math.Round(ReceptionTime(1), 3).ToString & ",")
                        exportFile.Write(SatObs.data(satPRN, 1).ToString & "," & SatObs.data(satPRN, 2).ToString & "," & SatObs.data(satPRN, 3).ToString & "," & SatObs.data(satPRN, 4).ToString & "," & SatObs.data(satPRN, 5).ToString & "," & SatObs.data(satPRN, 6).ToString & "," & SatObs.data(satPRN, 7).ToString & ",,")
                    Else
                        exportFile.Write(",,,,,,,,,,")
                    End If
                Next
                exportFile.WriteLine()
            Next
            exportFile.Close()
            MessageBox.Show("Export of the observations file is complete!", "EXPORT", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Dim refStatNav As New RINEX_Nav
    Dim refStatObs As New RINEX_Obs
    Dim remStatNav As New RINEX_Nav
    Dim remStatObs As New RINEX_Obs
    Dim refStatEpochs As New List(Of String)
    Dim remStatEpochs As New List(Of String)
    Dim inLoadData As Boolean
    Dim commonSats As List(Of Integer)

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim OpenResult As DialogResult = OpenFileDialog3.ShowDialog
        If OpenResult = Windows.Forms.DialogResult.OK Then
            TextBox3.Text = OpenFileDialog3.FileName
            refStatNav.Reset()
            refStatObs.Reset()
            baselineGroupBox.Enabled = False
            firstCommonEpochComboBox.Items.Clear()
            lastCommonEpochComboBox.Items.Clear()
            Dim navFileName As String = OpenFileDialog3.FileName.Substring(0, OpenFileDialog3.FileName.Length - 1) & "n"
            If File.Exists(navFileName) Then
                ProgressBar2.Visible = True
                refStatNav.ReadFile(navFileName, ProgressBar2)
                refStatObs.ReadFile(OpenFileDialog3.FileName, ProgressBar2, True)
                'ProgressBar2.Visible = False
                ProgressBar2.Value = ProgressBar2.Minimum
                CheckBox3.CheckState = CheckState.Checked
                ProgressBar2.Refresh()

                For i As Integer = 0 To refStatObs.NumEpochs - 1
                    refStatEpochs.Add(refStatObs.ObsData(0, 0, i) & ": " & Decimal.Round(refStatObs.ObsData(0, 1, i), 2))
                Next

            Else
                TextBox3.Text = ""
                CheckBox3.CheckState = CheckState.Unchecked
                MessageBox.Show("Matching Reference Station RINEX Navigation File does not exist, no Reference Station data has been loaded", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
            If CheckBox3.Checked And CheckBox4.Checked Then
                'loadButton.Enabled = True
                Call loadButton_Click(sender, e)
            Else
                loadButton.Enabled = False
            End If
        End If
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Dim OpenResult As DialogResult = OpenFileDialog3.ShowDialog
        If OpenResult = Windows.Forms.DialogResult.OK Then
            TextBox4.Text = OpenFileDialog3.FileName
            remStatNav.Reset()
            remStatObs.Reset()
            baselineGroupBox.Enabled = False
            firstCommonEpochComboBox.Items.Clear()
            lastCommonEpochComboBox.Items.Clear()
            Dim navFileName As String = OpenFileDialog3.FileName.Substring(0, OpenFileDialog3.FileName.Length - 1) & "n"
            If File.Exists(navFileName) Then
                ProgressBar2.Visible = True
                remStatNav.ReadFile(navFileName, ProgressBar2)
                remStatObs.ReadFile(OpenFileDialog3.FileName, ProgressBar2, True)
                'ProgressBar2.Visible = False
                ProgressBar2.Value = ProgressBar2.Minimum
                CheckBox4.CheckState = CheckState.Checked
                ProgressBar2.Refresh()

                For i As Integer = 0 To remStatObs.NumEpochs - 1
                    remStatEpochs.Add(remStatObs.ObsData(0, 0, i) & ": " & Decimal.Round(remStatObs.ObsData(0, 1, i), 2))
                Next

            Else
                TextBox4.Text = ""
                CheckBox4.CheckState = CheckState.Unchecked
                MessageBox.Show("Matching Remote Station RINEX Navigation File does not exist, no Remote Station data has been loaded", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
            If CheckBox3.Checked And CheckBox4.Checked Then
                'loadButton.Enabled = True
                Call loadButton_Click(sender, e)
            Else
                loadButton.Enabled = False
            End If
        End If
    End Sub

    Private Sub loadButton_Click(sender As Object, e As EventArgs) Handles loadButton.Click
        remCoordGroupBox.Text = "Remote Station Coordinates (Approx)"
        inLoadData = True
        firstCommonEpochComboBox.Items.Clear()
        lastCommonEpochComboBox.Items.Clear()
        baseSatComboBox.Items.Clear()

        For i As Integer = 0 To refStatEpochs.Count - 1
            If remStatEpochs.Contains(refStatEpochs.Item(i)) Then
                firstCommonEpochComboBox.Items.Add(refStatEpochs.Item(i))
                lastCommonEpochComboBox.Items.Add(refStatEpochs.Item(i))
            End If
        Next
        If firstCommonEpochComboBox.Items.Count > 0 Then
            firstCommonEpochComboBox.SelectedIndex = 0
            lastCommonEpochComboBox.SelectedIndex = lastCommonEpochComboBox.Items.Count - 1
        End If
        refStatXTextBox.Text = refStatObs.Approx_X
        refStatYTextBox.Text = refStatObs.Approx_Y
        refStatZTextBox.Text = refStatObs.Approx_Z
        remStatXTextBox.Text = remStatObs.Approx_X
        remStatYTextBox.Text = remStatObs.Approx_Y
        remStatZTextBox.Text = remStatObs.Approx_Z

        detectSats()
        baselineGroupBox.Enabled = True
        iterationsComboBox.SelectedIndex = 1
        inLoadData = False
    End Sub

    Private Function findIndexofWeekSecondsInRef(ByVal epoch As String, ByRef refStatObsAdj As RINEX_Obs) As Integer
        Dim solution As Integer = -1
        Dim delims(0) As Char
        Dim spaces(0) As Char
        delims(0) = ":"
        spaces(0) = " "
        Dim epochParts() As String = epoch.Split(delims)
        Try
            Dim weeks As Decimal = Decimal.Parse(epochParts(0).Trim(spaces))
            Dim seconds As Decimal = Decimal.Parse(epochParts(1).Trim(spaces))
            seconds = Math.Round(seconds, 3)

            For i As Integer = 0 To refStatObsAdj.NumEpochs - 1
                Dim weeksData As Decimal = refStatObsAdj.ObsData(0, 0, i)
                Dim secondsData As Decimal = refStatObsAdj.ObsData(0, 1, i)
                secondsData = Math.Round(secondsData, 3)
                If weeksData = weeks And secondsData = seconds Then
                    solution = i
                    Exit For
                End If
            Next
            Return solution
        Catch
            Return solution
        End Try
    End Function

    Private Function findIndexofWeekSecondsInRem(ByVal epoch As String, ByRef remStatObsAdj As RINEX_Obs) As Integer
        Dim solution As Integer = -1
        Dim delims(0) As Char
        Dim spaces(0) As Char
        delims(0) = ":"
        spaces(0) = " "
        Dim epochParts() As String = epoch.Split(delims)
        Try
            Dim weeks As Decimal = Decimal.Parse(epochParts(0).Trim(spaces))
            Dim seconds As Decimal = Decimal.Parse(epochParts(1).Trim(spaces))
            seconds = Math.Round(seconds, 3)

            For i As Integer = 0 To remStatObsAdj.NumEpochs - 1
                Dim weeksData As Decimal = remStatObsAdj.ObsData(0, 0, i)
                Dim secondsData As Decimal = remStatObsAdj.ObsData(0, 1, i)
                secondsData = Math.Round(secondsData, 3)
                If weeksData = weeks And secondsData = seconds Then
                    solution = i
                    Exit For
                End If
            Next
            Return solution
        Catch
            Return solution
        End Try
    End Function

    Private Sub firstCommonEpochComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles firstCommonEpochComboBox.SelectedIndexChanged
        Dim i As Integer

        Dim currentSelection As String = lastCommonEpochComboBox.Text
        lastCommonEpochComboBox.Items.Clear()
        For i = firstCommonEpochComboBox.SelectedIndex To firstCommonEpochComboBox.Items.Count - 1
            lastCommonEpochComboBox.Items.Add(firstCommonEpochComboBox.Items(i))
        Next

        If currentSelection <> String.Empty Then
            lastCommonEpochComboBox.SelectedIndex = lastCommonEpochComboBox.Items.IndexOf(currentSelection)
            If lastCommonEpochComboBox.SelectedIndex < 0 Then
                lastCommonEpochComboBox.SelectedIndex = lastCommonEpochComboBox.Items.Count - 1
            End If
        Else
            lastCommonEpochComboBox.SelectedIndex = lastCommonEpochComboBox.Items.Count - 1
        End If

        Dim month, day, year, hour, min, sec, leap_secs As Decimal
        Dim leapCorrected As String = "(GPS Time)"
        Dim refFirstIndex As Integer = findIndexofWeekSecondsInRef(firstCommonEpochComboBox.Items(firstCommonEpochComboBox.SelectedIndex), refStatObs)
        month = refStatObs.ObsData(1, 0, refFirstIndex)
        day = refStatObs.ObsData(1, 1, refFirstIndex)
        year = refStatObs.ObsData(2, 0, refFirstIndex)
        hour = refStatObs.ObsData(2, 1, refFirstIndex)
        min = refStatObs.ObsData(3, 0, refFirstIndex)
        sec = refStatObs.ObsData(3, 1, refFirstIndex)
        leap_secs = 0

        If refStatNav.Leap_Seconds <> -9999I Or refStatObs.Leap_Seconds <> -9999I Then
            leapCorrected = "(UTC Time)"
            leap_secs = Convert.ToDecimal(refStatNav.Leap_Seconds)
            If leap_secs = -9999D Then
                leap_secs = Convert.ToDecimal(refStatObs.Leap_Seconds)
            End If
            sec -= leap_secs

            If sec < 0 Then
                sec += 60
                min -= 1
                If min < 0 Then
                    min += 60
                    hour -= 1
                    If hour < 0 Then
                        hour += 24
                        day -= 1
                        If day < 1 Then
                            If month = 1 Or month = 2 Or month = 4 Or month = 6 Or month = 8 Or month = 9 Or month = 11 Then
                                day += 31
                            ElseIf month = 3 Then
                                Dim leapyearBool As Boolean = False
                                If year Mod 4 = 0 Then
                                    leapyearBool = True
                                    If year Mod 100 = 0 Then
                                        leapyearBool = False
                                        If year Mod 400 = 0 Then
                                            leapyearBool = True
                                        End If
                                    End If
                                End If
                                If leapyearBool Then
                                    day += 29
                                Else
                                    day += 28
                                End If
                            Else
                                day += 30
                            End If
                            month -= 1
                            If month < 1 Then
                                month += 12
                                year -= 1
                            End If
                        End If
                    End If
                End If
            End If
        End If

        Label52.Text = month.ToString & "/" & day.ToString & "/" & year.ToString & "  " & hour.ToString("00") & ":" & min.ToString("00") & ":" & sec.ToString("00")
        Label51.Text = leapCorrected
        Label57.Text = (firstCommonEpochComboBox.Items.IndexOf(lastCommonEpochComboBox.Items(lastCommonEpochComboBox.SelectedIndex)) - firstCommonEpochComboBox.SelectedIndex + 1).ToString
        If Not inLoadData Then
            detectSats()
        End If
    End Sub

    Private Sub lastCommonEpochComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lastCommonEpochComboBox.SelectedIndexChanged
        Dim month, day, year, hour, min, sec, leap_secs As Decimal
        Dim leapCorrected As String = "(GPS Time)"
        Dim refLastIndex As Integer = findIndexofWeekSecondsInRef(lastCommonEpochComboBox.Items(lastCommonEpochComboBox.SelectedIndex), refStatObs)
        month = refStatObs.ObsData(1, 0, refLastIndex)
        day = refStatObs.ObsData(1, 1, refLastIndex)
        year = refStatObs.ObsData(2, 0, refLastIndex)
        hour = refStatObs.ObsData(2, 1, refLastIndex)
        min = refStatObs.ObsData(3, 0, refLastIndex)
        sec = refStatObs.ObsData(3, 1, refLastIndex)
        leap_secs = 0

        If refStatNav.Leap_Seconds <> -9999I Or refStatObs.Leap_Seconds <> -9999I Then
            leapCorrected = "(UTC Time)"
            leap_secs = Convert.ToDecimal(refStatNav.Leap_Seconds)
            If leap_secs = -9999D Then
                leap_secs = Convert.ToDecimal(refStatObs.Leap_Seconds)
            End If
            sec -= leap_secs

            If sec < 0 Then
                sec += 60
                min -= 1
                If min < 0 Then
                    min += 60
                    hour -= 1
                    If hour < 0 Then
                        hour += 24
                        day -= 1
                        If day < 1 Then
                            If month = 1 Or month = 2 Or month = 4 Or month = 6 Or month = 8 Or month = 9 Or month = 11 Then
                                day += 31
                            ElseIf month = 3 Then
                                Dim leapyearBool As Boolean = False
                                If year Mod 4 = 0 Then
                                    leapyearBool = True
                                    If year Mod 100 = 0 Then
                                        leapyearBool = False
                                        If year Mod 400 = 0 Then
                                            leapyearBool = True
                                        End If
                                    End If
                                End If
                                If leapyearBool Then
                                    day += 29
                                Else
                                    day += 28
                                End If
                            Else
                                day += 30
                            End If
                            month -= 1
                            If month < 1 Then
                                month += 12
                                year -= 1
                            End If
                        End If
                    End If
                End If
            End If
        End If

        Label56.Text = month.ToString & "/" & day.ToString & "/" & year.ToString & "  " & hour.ToString("00") & ":" & min.ToString("00") & ":" & sec.ToString("00")
        Label55.Text = leapCorrected
        Label57.Text = (firstCommonEpochComboBox.Items.IndexOf(lastCommonEpochComboBox.Items(lastCommonEpochComboBox.SelectedIndex)) - firstCommonEpochComboBox.SelectedIndex + 1).ToString
        If Not inLoadData Then
            detectSats()
        End If
    End Sub

    Private Sub detectSats()
        commonSats = New List(Of Integer)
        Dim existingSelection As String = String.Empty

        If baseSatComboBox.SelectedIndex <> -1 Then
            existingSelection = baseSatComboBox.SelectedItem.ToString
        End If

        baseSatComboBox.Items.Clear()
        Dim start As Integer = firstCommonEpochComboBox.SelectedIndex
        Dim endd As Integer = firstCommonEpochComboBox.Items.IndexOf(lastCommonEpochComboBox.Items(lastCommonEpochComboBox.SelectedIndex))

        ProgressBar2.Visible = True
        ProgressBar2.Minimum = start
        ProgressBar2.Maximum = endd

        Dim L1IndexRef As Integer = -9999I
        Dim L1IndexRem As Integer = -9999I
        Dim L2IndexRef As Integer = -9999I
        Dim L2IndexRem As Integer = -9999I
        Dim C1IndexRef As Integer = -9999I
        Dim C1IndexRem As Integer = -9999I

        'locate the L1 and L2 observables
        For m As Integer = 0 To 17
            If refStatObs.Obs_Type(m).ToUpper = "L1" Then
                L1IndexRef = m
            End If

            If remStatObs.Obs_Type(m).ToUpper = "L1" Then
                L1IndexRem = m
            End If

            If refStatObs.Obs_Type(m).ToUpper = "L2" Then
                L2IndexRef = m
            End If

            If remStatObs.Obs_Type(m).ToUpper = "L2" Then
                L2IndexRem = m
            End If

            If refStatObs.Obs_Type(m).ToUpper = "C1" Then
                C1IndexRef = m
            End If

            If remStatObs.Obs_Type(m).ToUpper = "C1" Then
                C1IndexRem = m
            End If
        Next
        If L1IndexRef <> -9999I And L1IndexRem <> -9999I And C1IndexRef <> -9999I And C1IndexRem <> -9999I Then
            For i As Integer = start To endd
                Dim commonRefStatSats As New List(Of Integer)
                Dim commonRemStatSats As New List(Of Integer)
                Dim commonSatsEpoch As New List(Of Integer)
                ProgressBar2.Value = i
                If ProgressBar2.Value Mod 25 = 0 Then
                    ProgressBar2.Refresh()
                End If
                Dim refFirstIndex As Integer = findIndexofWeekSecondsInRef(firstCommonEpochComboBox.Items(i), refStatObs)
                Dim remFirstIndex As Integer = findIndexofWeekSecondsInRem(firstCommonEpochComboBox.Items(i), remStatObs)

                For j As Integer = 4 To refStatObs.NumRows(refFirstIndex) - 1
                    Dim satNum As Integer = refStatObs.ObsData(j, 0, refFirstIndex)
                    If satNum <> -9999D AndAlso PRNs2UseBoolean(satNum) Then
                        commonRefStatSats.Add(satNum)
                    End If
                Next
                For j As Integer = 4 To remStatObs.NumRows(remFirstIndex) - 1
                    Dim satNum As Integer = remStatObs.ObsData(j, 0, remFirstIndex)
                    If satNum <> -9999D AndAlso PRNs2UseBoolean(satNum) Then
                        commonRemStatSats.Add(satNum)
                    End If
                Next
                For j As Integer = 0 To commonRefStatSats.Count - 1
                    If commonRemStatSats.Contains(commonRefStatSats.Item(j)) Then
                        commonSatsEpoch.Add(commonRefStatSats.Item(j))
                    End If
                Next

                If i = start Then
                    For j As Integer = 0 To commonSatsEpoch.Count - 1
                        commonSats.Add(commonSatsEpoch.Item(j))
                    Next
                Else
                    Dim newCommonSats As New List(Of Integer)
                    For j As Integer = 0 To commonSatsEpoch.Count - 1
                        If commonSats.Contains(commonSatsEpoch.Item(j)) Then
                            newCommonSats.Add(commonSatsEpoch.Item(j))
                        End If
                    Next
                    commonSats.Clear()
                    For j As Integer = 0 To newCommonSats.Count - 1
                        commonSats.Add(newCommonSats.Item(j))
                    Next
                End If
            Next
            'ProgressBar2.Visible = False
            ProgressBar2.Value = ProgressBar2.Minimum

            Dim highestElev As Decimal = 0
            Dim highestElevIndex As Integer = 0
            Dim z As Integer = findIndexofWeekSecondsInRef(firstCommonEpochComboBox.Items(firstCommonEpochComboBox.SelectedIndex), refStatObs)
            For r As Integer = 0 To commonSats.Count - 1
                Dim i, j, k, m As Integer
                Dim SatNum As Integer
                Dim PRN As Integer
                Dim ReceptionTime(1), EphemerisTime(1) As Decimal
                Dim TimeDiffFromReference As Decimal
                Dim Eph2Use As New RINEX_Eph
                Dim ECEF_XYZ_SatPosition(0 To 3) As Decimal
                Dim TempRINEX_Nav As New RINEX_Nav

                Dim CartPosition(2) As Decimal
                Dim CurvPosition() As Decimal

                CartPosition(0) = refStatObs.Approx_X
                CartPosition(1) = refStatObs.Approx_Y
                CartPosition(2) = refStatObs.Approx_Z
                CurvPosition = cart2curv(CartPosition)

                'define rotation matrices to convert ECEF to Local E,N,Up
                Dim R1, R3 As New Matrix(3, 3)
                R1.data(1, 1) = 1D
                R1.data(1, 2) = 0D
                R1.data(1, 3) = 0D
                R1.data(2, 1) = 0D
                R1.data(2, 2) = Math.Cos((90 - CurvPosition(0)) * Math.PI / 180D)
                R1.data(2, 3) = Math.Sin((90 - CurvPosition(0)) * Math.PI / 180D)
                R1.data(3, 1) = 0D
                R1.data(3, 2) = -1 * Math.Sin((90 - CurvPosition(0)) * Math.PI / 180D)
                R1.data(3, 3) = Math.Cos((90 - CurvPosition(0)) * Math.PI / 180D)

                R3.data(1, 1) = Math.Cos((90 + CurvPosition(1)) * Math.PI / 180D)
                R3.data(1, 2) = Math.Sin((90 + CurvPosition(1)) * Math.PI / 180D)
                R3.data(1, 3) = 0D
                R3.data(2, 1) = -1 * Math.Sin((90 + CurvPosition(1)) * Math.PI / 180D)
                R3.data(2, 2) = Math.Cos((90 + CurvPosition(1)) * Math.PI / 180D)
                R3.data(2, 3) = 0D
                R3.data(3, 1) = 0D
                R3.data(3, 2) = 0D
                R3.data(3, 3) = 1D

                ReceptionTime(0) = refStatObs.ObsData(0, 0, z)
                ReceptionTime(1) = refStatObs.ObsData(0, 1, z)

                For i = 4 To refStatObs.NumRows(z) - 1
                    TimeDiffFromReference = 1000000000000.0
                    SatNum = refStatObs.ObsData(i, 0, z)

                    If SatNum <> -9999I AndAlso PRNs2UseBoolean(SatNum) AndAlso SatNum = commonSats.Item(r) Then
                        Eph2Use = New RINEX_Eph
                        For j = 0 To refStatNav.EphemerisV.GetLength(1) - 1
                            For k = 0 To refStatNav.EphemerisV.GetLength(0) - 1
                                PRN = refStatNav.EphemerisV(k, j).PRN
                                If SatNum = PRN Then
                                    EphemerisTime(0) = refStatNav.EphemerisV(k, j).Toe_Week
                                    EphemerisTime(1) = refStatNav.EphemerisV(k, j).Toe
                                    If Math.Abs((EphemerisTime(0) * 604800D + EphemerisTime(1)) - (ReceptionTime(0) * 604800D + ReceptionTime(1))) < TimeDiffFromReference Then
                                        TimeDiffFromReference = Math.Abs((EphemerisTime(0) * 604800D + EphemerisTime(1)) - (ReceptionTime(0) * 604800D + ReceptionTime(1)))
                                        Eph2Use = refStatNav.EphemerisV(k, j)
                                    End If
                                End If
                            Next
                        Next

                        If Eph2Use.PRN <> -9999I Then
                            ECEF_XYZ_SatPosition = TempRINEX_Nav.ComputeSatellitePositionFromEphemeris(0, 0, 0, Eph2Use, ReceptionTime, 20200000D, True, True, True, True)
                            If ECEF_XYZ_SatPosition(0) = 0 Then
                                Dim E_N_Up, Temp As Matrix
                                Dim diff As New Matrix(3, 1)
                                diff.data(1, 1) = ECEF_XYZ_SatPosition(1) - refStatObs.Approx_X
                                diff.data(2, 1) = ECEF_XYZ_SatPosition(2) - refStatObs.Approx_Y
                                diff.data(3, 1) = ECEF_XYZ_SatPosition(3) - refStatObs.Approx_Z
                                Temp = R3 * diff
                                E_N_Up = R1 * Temp

                                Dim Elev, Azimuth As Decimal
                                Elev = (Math.Atan2(E_N_Up.data(3, 1), Math.Sqrt(E_N_Up.data(2, 1) ^ 2D + E_N_Up.data(1, 1) ^ 2D))) * 180D / Math.PI
                                Azimuth = (Math.Atan2(E_N_Up.data(1, 1), E_N_Up.data(2, 1))) * 180D / Math.PI

                                If Azimuth < 0 Then
                                    Azimuth += 360D
                                End If

                                If Elev > highestElev Then
                                    highestElev = Elev
                                    highestElevIndex = r
                                End If

                                baseSatComboBox.Items.Add(commonSats.Item(r).ToString & " @ " & (Math.Round(Elev, 0)).ToString & Chr(176))
                            End If
                        End If
                    End If
                Next
            Next
            If baseSatComboBox.Items.Count > 0 Then
                If existingSelection = String.Empty Then
                    baseSatComboBox.SelectedIndex = highestElevIndex
                Else
                    If baseSatComboBox.Items.Contains(existingSelection) Then
                        baseSatComboBox.SelectedIndex = baseSatComboBox.Items.IndexOf(existingSelection)
                    Else
                        baseSatComboBox.SelectedIndex = highestElevIndex
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub processButton_Click(sender As Object, e As EventArgs) Handles processButton.Click
        remCoordGroupBox.Text = "Remote Station Coordinates (Approx)"

        If baseSatComboBox.Items.Count > 0 Then
            Dim refstatX, refstatY, refstatZ, remstatX, remstatY, remstatZ As Decimal
            Try
                refstatX = Decimal.Parse(refStatXTextBox.Text)
                Try
                    refstatY = Decimal.Parse(refStatYTextBox.Text)
                    Try
                        refstatZ = Decimal.Parse(refStatZTextBox.Text)
                        Try
                            If remStatXTextBox.Text.Contains(Chr(177)) Then
                                Dim tempStr As String = remStatXTextBox.Text.Substring(0, remStatXTextBox.Text.IndexOf(Chr(177)))
                                remStatXTextBox.Text = tempStr
                            End If
                            remstatX = Decimal.Parse(remStatXTextBox.Text)
                            Try
                                If remStatYTextBox.Text.Contains(Chr(177)) Then
                                    Dim tempStr As String = remStatYTextBox.Text.Substring(0, remStatYTextBox.Text.IndexOf(Chr(177)))
                                    remStatYTextBox.Text = tempStr
                                End If
                                remstatY = Decimal.Parse(remStatYTextBox.Text)
                                Try
                                    If remStatZTextBox.Text.Contains(Chr(177)) Then
                                        Dim tempStr As String = remStatZTextBox.Text.Substring(0, remStatZTextBox.Text.IndexOf(Chr(177)))
                                        remStatZTextBox.Text = tempStr
                                    End If
                                    remstatZ = Decimal.Parse(remStatZTextBox.Text)

                                    Try
                                        processButton.Enabled = False
                                        preprocessingLabel.Visible = False
                                        processButton.Refresh()
                                        preprocessingLabel.Refresh()

                                        Dim refStatObsAdj As New RINEX_Obs
                                        Dim remStatObsAdj As New RINEX_Obs

                                        refStatObsAdj.equals(refStatObs)
                                        remStatObsAdj.equals(remStatObs)

                                        If preprocessCheckBox.Checked Then
                                            preprocessingLabel.Text = "Preprocessing, Please Wait..."
                                            preprocessingLabel.Visible = True
                                            preprocessingLabel.Refresh()
                                            refStatObsAdj.preProcessObs("Reference")
                                            remStatObsAdj.preProcessObs("Remote")
                                            preprocessingLabel.Visible = False
                                            preprocessingLabel.Refresh()
                                        End If

                                        Dim baseSat As Integer = commonSats.Item(baseSatComboBox.SelectedIndex)
                                        Dim start As Integer = firstCommonEpochComboBox.SelectedIndex
                                        Dim endd As Integer = firstCommonEpochComboBox.Items.IndexOf(lastCommonEpochComboBox.Items(lastCommonEpochComboBox.SelectedIndex))
                                        Dim pseudorangeIndexRef As Integer = -9999I
                                        Dim pseudorangeIndexRem As Integer = -9999I

                                        'locate the C/A code ("C1") pseudorange
                                        For m As Integer = 0 To 17
                                            If refStatObsAdj.Obs_Type(m).ToUpper = "C1" Then
                                                pseudorangeIndexRef = m
                                            End If

                                            If remStatObsAdj.Obs_Type(m).ToUpper = "C1" Then
                                                pseudorangeIndexRem = m
                                            End If
                                        Next

                                        Dim numberofiterations As Integer = iterationsComboBox.SelectedIndex
                                        ProgressBar2.Visible = True
                                        ProgressBar2.Minimum = start
                                        ProgressBar2.Maximum = (numberofiterations + 2) * endd

                                        'construct the Differenced Observables
                                        Dim goodData As Boolean = True
                                        Dim DD_OBS(,) As Decimal = constructOBS(goodData, refStatObsAdj, remStatObsAdj)

                                        If pseudorangeIndexRef <> -9999I And pseudorangeIndexRem <> -9999I And goodData Then

                                            'reset the displayed results and column plot options
                                            resultsGrid.Columns.Clear()
                                            plot1ComboBox.Items.Clear()
                                            plot2ComboBox.Items.Clear()
                                            plot3ComboBox.Items.Clear()
                                            plot4ComboBox.Items.Clear()

                                            'setup results columns
                                            If cycleSlipsCheckBox.Checked Then
                                                resultsGrid.ColumnCount = (commonSats.Count - 1) * 8 + 8
                                            Else
                                                resultsGrid.ColumnCount = (commonSats.Count - 1) * 7 + 8
                                            End If

                                            Dim satCombos(commonSats.Count - 2) As String
                                            Dim comboCount As Integer = 0

                                            For i As Integer = 0 To commonSats.Count - 1
                                                Dim currentSat As Integer = commonSats.Item(i)
                                                If currentSat <> baseSat Then
                                                    satCombos(comboCount) = currentSat.ToString & "-" & baseSat.ToString
                                                    comboCount += 1
                                                End If
                                            Next
                                            resultsGrid.Columns(0).HeaderText = "Epoch"
                                            Dim columnCount As Integer = 1
                                            For i As Integer = 0 To commonSats.Count - 2
                                                resultsGrid.Columns(columnCount).HeaderText = ChrW(916) & ChrW(8711) & "L1" & vbNewLine & "(" & satCombos(i) & ") [cyc]"
                                                columnCount += 1
                                            Next
                                            For i As Integer = 0 To commonSats.Count - 2
                                                resultsGrid.Columns(columnCount).HeaderText = ChrW(916) & ChrW(8711) & "C1" & vbNewLine & "(" & satCombos(i) & ") [m]"
                                                columnCount += 1
                                            Next
                                            For i As Integer = 0 To commonSats.Count - 2
                                                resultsGrid.Columns(columnCount).HeaderText = ChrW(916) & ChrW(8711) & ChrW(961) & vbNewLine & "(" & satCombos(i) & ") [m]"
                                                columnCount += 1
                                            Next
                                            For i As Integer = 0 To commonSats.Count - 2
                                                resultsGrid.Columns(columnCount).HeaderText = ChrW(948) & ChrW(916) & ChrW(8711) & "L1" & vbNewLine & "(" & satCombos(i) & ") [cyc]"
                                                columnCount += 1
                                            Next
                                            For i As Integer = 0 To commonSats.Count - 2
                                                resultsGrid.Columns(columnCount).HeaderText = ChrW(948) & ChrW(916) & ChrW(8711) & ChrW(961) & vbNewLine & "(" & satCombos(i) & ") [cyc]"
                                                columnCount += 1
                                            Next
                                            For i As Integer = 0 To commonSats.Count - 2
                                                resultsGrid.Columns(columnCount).HeaderText = ChrW(916) & ChrW(8711) & "N float" & vbNewLine & "(" & satCombos(i) & ") [cyc]"
                                                columnCount += 1
                                            Next
                                            If c1RadioButton.Checked Then
                                                resultsGrid.Columns(columnCount).HeaderText = "X (DGPS)" & vbNewLine & "[m]"
                                                columnCount += 1
                                                resultsGrid.Columns(columnCount).HeaderText = "Y (DGPS)" & vbNewLine & "[m]"
                                                columnCount += 1
                                                resultsGrid.Columns(columnCount).HeaderText = "Z (DGPS)" & vbNewLine & "[m]"
                                                columnCount += 1
                                            Else
                                                resultsGrid.Columns(columnCount).HeaderText = "X (Float)" & vbNewLine & "[m]"
                                                columnCount += 1
                                                resultsGrid.Columns(columnCount).HeaderText = "Y (Float)" & vbNewLine & "[m]"
                                                columnCount += 1
                                                resultsGrid.Columns(columnCount).HeaderText = "Z (Float)" & vbNewLine & "[m]"
                                                columnCount += 1
                                            End If
                                            resultsGrid.Columns(columnCount).HeaderText = "Ratio" & vbNewLine & "Test"
                                            columnCount += 1
                                            For i As Integer = 0 To commonSats.Count - 2
                                                resultsGrid.Columns(columnCount).HeaderText = ChrW(916) & ChrW(8711) & "N fixed" & vbNewLine & "(" & satCombos(i) & ") [cyc]"
                                                columnCount += 1
                                            Next
                                            resultsGrid.Columns(columnCount).HeaderText = "X (Fixed)" & vbNewLine & "[m]"
                                            columnCount += 1
                                            resultsGrid.Columns(columnCount).HeaderText = "Y (Fixed)" & vbNewLine & "[m]"
                                            columnCount += 1
                                            resultsGrid.Columns(columnCount).HeaderText = "Z (Fixed)" & vbNewLine & "[m]"

                                            If cycleSlipsCheckBox.Checked Then
                                                For i As Integer = 0 To commonSats.Count - 2
                                                    columnCount += 1
                                                    resultsGrid.Columns(columnCount).HeaderText = "Cycle Slips" & vbNewLine & "(" & satCombos(i) & ") [cyc]"
                                                Next
                                            End If
                                            resultsGrid.Refresh()

                                            'setup plot lists with column headers (except no option to plot time as it is always used on the X axis of the plots)
                                            plot1ComboBox.Items.Add("NONE")
                                            plot2ComboBox.Items.Add("NONE")
                                            plot3ComboBox.Items.Add("NONE")
                                            plot4ComboBox.Items.Add("NONE")
                                            For i As Integer = 1 To resultsGrid.ColumnCount - 1
                                                plot1ComboBox.Items.Add(resultsGrid.Columns(i).HeaderText)
                                                plot2ComboBox.Items.Add(resultsGrid.Columns(i).HeaderText)
                                                plot3ComboBox.Items.Add(resultsGrid.Columns(i).HeaderText)
                                                plot4ComboBox.Items.Add(resultsGrid.Columns(i).HeaderText)
                                            Next
                                            plot1ComboBox.SelectedIndex = 0
                                            plot2ComboBox.SelectedIndex = 0
                                            plot3ComboBox.SelectedIndex = 0
                                            plot4ComboBox.SelectedIndex = 0

                                            Dim DD_x As Matrix
                                            Dim CV_DD_x As Matrix
                                            Dim ambigs As Matrix        'experimental
                                            Dim Global_A As Matrix
                                            Dim Global_W As Matrix
                                            Dim previousFitnessTest As Matrix

                                            If c1RadioButton.Checked Then
                                                DD_x = New Matrix(3, 1)
                                                CV_DD_x = New Matrix(3, commonSats.Count + 2)
                                                Global_A = New Matrix((endd - start + 1) * (commonSats.Count - 1), 3)
                                                Global_W = New Matrix((endd - start + 1) * (commonSats.Count - 1), 1)
                                            ElseIf l1RadioButton.Checked Then
                                                DD_x = New Matrix(commonSats.Count + 2, 1)
                                                CV_DD_x = New Matrix(commonSats.Count + 2, commonSats.Count + 2)
                                                Global_A = New Matrix((endd - start + 1) * (commonSats.Count - 1), commonSats.Count + 2)
                                                Global_W = New Matrix((endd - start + 1) * (commonSats.Count - 1), 1)
                                                previousFitnessTest = New Matrix((commonSats.Count - 1), 1)
                                            ElseIf c1l1RadioButton.Checked Then
                                                DD_x = New Matrix(commonSats.Count + 2, 1)
                                                CV_DD_x = New Matrix(commonSats.Count + 2, commonSats.Count + 2)
                                                Global_A = New Matrix(2 * (endd - start + 1) * (commonSats.Count - 1), commonSats.Count + 2)
                                                Global_W = New Matrix(2 * (endd - start + 1) * (commonSats.Count - 1), 1)
                                                previousFitnessTest = New Matrix((commonSats.Count - 1), 1)
                                            ElseIf l1fRadioButton.Checked Then
                                                ''''''''''''''''''''''''''''''''''''''''''''''experimental''''''''''''''''''''''''''''''''''''''''''''''
                                                DD_x = New Matrix(3, 1)
                                                CV_DD_x = New Matrix(3, commonSats.Count + 2)
                                                Global_A = New Matrix((endd - start + 1) * (commonSats.Count - 1), 3)
                                                Global_W = New Matrix((endd - start + 1) * (commonSats.Count - 1), 1)
                                                ambigs = New Matrix(commonSats.Count - 1, 1)
                                                ambigs.data(1, 1) = 30
                                                ambigs.data(2, 1) = -30
                                                ambigs.data(3, 1) = -17
                                                ambigs.data(4, 1) = 20
                                                ambigs.data(5, 1) = -4
                                            End If

                                            'set initial values for the unknowns (remote station position and double differenced ambiguities)
                                            DD_x.data(1, 1) = remstatX
                                            DD_x.data(2, 1) = remstatY
                                            DD_x.data(3, 1) = remstatZ
                                            'zero set for initial double difference ambiguities (if applicable)

                                            Dim cycleSlipsFound As Boolean = False

                                            'iterations loop                                            
                                            For f As Integer = 0 To numberofiterations

                                                Dim DD_N As Matrix
                                                Dim DD_U As Matrix
                                                Dim DD_delta As Matrix
                                                Dim CL As Matrix
                                                Dim P As Matrix
                                                Dim codeStdDev As Decimal = 3
                                                Dim phaseStdDev As Decimal = 0.01

                                                If c1RadioButton.Checked Then
                                                    DD_N = New Matrix(3, 3, True)
                                                    DD_U = New Matrix(3, 1)
                                                    CL = New Matrix(commonSats.Count - 1, commonSats.Count - 1)
                                                    For i As Integer = 1 To commonSats.Count - 1
                                                        For j As Integer = 1 To commonSats.Count - 1
                                                            If i = j Then
                                                                CL.data(i, j) = codeStdDev ^ 2 * 4
                                                            Else
                                                                CL.data(i, j) = codeStdDev ^ 2 * 2
                                                            End If
                                                        Next
                                                    Next
                                                    P = CL.Inverse
                                                ElseIf l1RadioButton.Checked Then
                                                    DD_N = New Matrix((commonSats.Count - 1) + 3, (commonSats.Count - 1) + 3, True)
                                                    DD_U = New Matrix((commonSats.Count - 1) + 3, 1)
                                                    CL = New Matrix(commonSats.Count - 1, commonSats.Count - 1)
                                                    For i As Integer = 1 To commonSats.Count - 1
                                                        For j As Integer = 1 To commonSats.Count - 1
                                                            If i = j Then
                                                                CL.data(i, j) = phaseStdDev ^ 2 * 4
                                                            Else
                                                                CL.data(i, j) = phaseStdDev ^ 2 * 2
                                                            End If
                                                        Next
                                                    Next
                                                    P = CL.Inverse
                                                    'P.printAll("P", True, 6)
                                                ElseIf c1l1RadioButton.Checked Then
                                                    DD_N = New Matrix((commonSats.Count - 1) + 3, (commonSats.Count - 1) + 3, True)
                                                    DD_U = New Matrix((commonSats.Count - 1) + 3, 1)
                                                    CL = New Matrix(2 * (commonSats.Count - 1), 2 * (commonSats.Count - 1))
                                                    For i As Integer = 1 To commonSats.Count - 1
                                                        For j As Integer = 1 To commonSats.Count - 1
                                                            If i = j Then
                                                                CL.data(i, j) = phaseStdDev ^ 2 * 4
                                                            Else
                                                                CL.data(i, j) = phaseStdDev ^ 2 * 2
                                                            End If
                                                        Next
                                                    Next
                                                    For i As Integer = commonSats.Count To 2 * (commonSats.Count - 1)
                                                        For j As Integer = commonSats.Count To 2 * (commonSats.Count - 1)
                                                            If i = j Then
                                                                CL.data(i, j) = codeStdDev ^ 2 * 4
                                                            Else
                                                                CL.data(i, j) = codeStdDev ^ 2 * 2
                                                            End If
                                                        Next
                                                    Next
                                                    P = CL.Inverse
                                                ElseIf l1fRadioButton.Checked Then
                                                    DD_N = New Matrix(3, 3, True)
                                                    DD_U = New Matrix(3, 1)
                                                    CL = New Matrix(commonSats.Count - 1, commonSats.Count - 1)
                                                    For i As Integer = 1 To commonSats.Count - 1
                                                        For j As Integer = 1 To commonSats.Count - 1
                                                            If i = j Then
                                                                CL.data(i, j) = phaseStdDev ^ 2 * 4
                                                            Else
                                                                CL.data(i, j) = phaseStdDev ^ 2 * 2
                                                            End If
                                                        Next
                                                    Next
                                                    P = CL.Inverse
                                                End If

                                                Dim previousRhos(commonSats.Count - 1) As Decimal

                                                'epochs loop
                                                For x As Integer = start To endd
                                                    Dim z1 As Integer = findIndexofWeekSecondsInRef(firstCommonEpochComboBox.Items(x), refStatObsAdj)
                                                    Dim z2 As Integer = findIndexofWeekSecondsInRem(firstCommonEpochComboBox.Items(x), remStatObsAdj)
                                                    Dim goodEpoch As Boolean = True

                                                    ProgressBar2.Value = f * endd + x + endd
                                                    If ProgressBar2.Value Mod 25 = 0 Then
                                                        ProgressBar2.Refresh()
                                                    End If

                                                    'structure has changed since initial programming, hence a lot of empty space in these storage matrix
                                                    Dim SatObs As New Matrix(commonSats.Count - 1, 7)
                                                    Dim BaseSatObs As New Matrix(1, 7)

                                                    Dim satCount As Integer = 1
                                                    Dim resultsString(resultsGrid.ColumnCount - 1) As String
                                                    resultsString(0) = firstCommonEpochComboBox.Items(x)
                                                    columnCount = 1

                                                    'satellites loop
                                                    For r As Integer = 0 To commonSats.Count - 1
                                                        Dim satToFind As Integer = commonSats(r)

                                                        Dim i, j, k As Integer
                                                        Dim SatNum As Integer
                                                        Dim PRN As Integer
                                                        Dim ReceptionTime(1), EphemerisTime(1) As Decimal
                                                        Dim TimeDiffFromReference As Decimal
                                                        Dim Eph2Use As New RINEX_Eph
                                                        Dim pseudorangeRef, pseudorangeRem, transmitDistance As Decimal

                                                        Dim ECEF_XYZ_SatPosition(0 To 3) As Decimal
                                                        Dim TempRINEX_Nav As New RINEX_Nav
                                                        Dim Sat_Clock_Corr, Rel_Corr, TGD_Corr As Decimal

                                                        ReceptionTime(0) = refStatObsAdj.ObsData(0, 0, z1)
                                                        ReceptionTime(1) = refStatObsAdj.ObsData(0, 1, z1)

                                                        For i = 4 To remStatObsAdj.NumRows(z2) - 1
                                                            pseudorangeRem = 0
                                                            SatNum = remStatObsAdj.ObsData(i, 0, z2)
                                                            If SatNum = satToFind Then
                                                                pseudorangeRem = remStatObsAdj.ObsData(i, (pseudorangeIndexRem + 1), z2)
                                                                If pseudorangeRem = -9999D Or pseudorangeRem = 0D Then
                                                                    goodEpoch = False
                                                                End If
                                                                Exit For
                                                            End If
                                                        Next

                                                        If goodEpoch Then
                                                            For i = 4 To refStatObsAdj.NumRows(z1) - 1
                                                                TimeDiffFromReference = 1000000000000.0
                                                                pseudorangeRef = 0
                                                                transmitDistance = 0
                                                                SatNum = refStatObsAdj.ObsData(i, 0, z1)

                                                                If SatNum <> -9999I AndAlso SatNum = satToFind Then
                                                                    Eph2Use = New RINEX_Eph
                                                                    For j = 0 To refStatNav.EphemerisV.GetLength(1) - 1
                                                                        For k = 0 To refStatNav.EphemerisV.GetLength(0) - 1
                                                                            PRN = refStatNav.EphemerisV(k, j).PRN
                                                                            If SatNum = PRN Then
                                                                                EphemerisTime(0) = refStatNav.EphemerisV(k, j).Toe_Week
                                                                                EphemerisTime(1) = refStatNav.EphemerisV(k, j).Toe
                                                                                If Math.Abs((EphemerisTime(0) * 604800D + EphemerisTime(1)) - (ReceptionTime(0) * 604800D + ReceptionTime(1))) < TimeDiffFromReference Then
                                                                                    TimeDiffFromReference = Math.Abs((EphemerisTime(0) * 604800D + EphemerisTime(1)) - (ReceptionTime(0) * 604800D + ReceptionTime(1)))
                                                                                    Eph2Use = refStatNav.EphemerisV(k, j)
                                                                                End If
                                                                            End If
                                                                        Next
                                                                    Next

                                                                    pseudorangeRef = refStatObsAdj.ObsData(i, (pseudorangeIndexRef + 1), z1)

                                                                    If pseudorangeRef = -9999D Or pseudorangeRef = 0D Then
                                                                        goodEpoch = False
                                                                    End If

                                                                    If goodEpoch Then
                                                                        If Eph2Use.PRN <> -9999I Then
                                                                            Sat_Clock_Corr = 0

                                                                            transmitDistance = pseudorangeRef
                                                                            ECEF_XYZ_SatPosition = TempRINEX_Nav.ComputeSatellitePositionFromEphemeris(Sat_Clock_Corr, Rel_Corr, TGD_Corr, Eph2Use, ReceptionTime, transmitDistance, False, True, True, True)
                                                                            Dim delta As Decimal = 1000
                                                                            While delta > 0.1
                                                                                ECEF_XYZ_SatPosition = TempRINEX_Nav.ComputeSatellitePositionFromEphemeris(Sat_Clock_Corr, Rel_Corr, TGD_Corr, Eph2Use, ReceptionTime, transmitDistance, True, True, True, True)
                                                                                If ECEF_XYZ_SatPosition(0) = 0 Then 'good position returned
                                                                                    Dim newRange As Decimal = Math.Sqrt((ECEF_XYZ_SatPosition(1) - refstatX) ^ 2 + (ECEF_XYZ_SatPosition(2) - refstatY) ^ 2 + (ECEF_XYZ_SatPosition(3) - refstatZ) ^ 2)
                                                                                    delta = Math.Abs(newRange - transmitDistance)
                                                                                    transmitDistance = newRange
                                                                                Else
                                                                                    Exit While
                                                                                End If
                                                                            End While

                                                                            If ECEF_XYZ_SatPosition(0) = 0 Then 'good position returned
                                                                                If SatNum = baseSat Then
                                                                                    'BaseSatObs.data(1, 1) = L1rem
                                                                                    'BaseSatObs.data(1, 2) = L1ref
                                                                                    'BaseSatObs.data(1, 3) = pseudorangeRem
                                                                                    'BaseSatObs.data(1, 4) = pseudorangeRef
                                                                                    BaseSatObs.data(1, 5) = ECEF_XYZ_SatPosition(1)
                                                                                    BaseSatObs.data(1, 6) = ECEF_XYZ_SatPosition(2)
                                                                                    BaseSatObs.data(1, 7) = ECEF_XYZ_SatPosition(3)
                                                                                Else
                                                                                    'SatObs.data(satCount, 1) = L1rem
                                                                                    'SatObs.data(satCount, 2) = L1ref
                                                                                    'SatObs.data(satCount, 3) = pseudorangeRem
                                                                                    'SatObs.data(satCount, 4) = pseudorangeRef
                                                                                    SatObs.data(satCount, 5) = ECEF_XYZ_SatPosition(1)
                                                                                    SatObs.data(satCount, 6) = ECEF_XYZ_SatPosition(2)
                                                                                    SatObs.data(satCount, 7) = ECEF_XYZ_SatPosition(3)
                                                                                    satCount += 1
                                                                                End If
                                                                                Exit For
                                                                            End If
                                                                        End If
                                                                    End If
                                                                End If
                                                            Next
                                                        End If
                                                    Next

                                                    If goodEpoch Then
                                                        Dim DD_L1 As Matrix
                                                        Dim DD_w As Matrix
                                                        Dim DD_A As Matrix
                                                        Dim cellColour As Color

                                                        If c1RadioButton.Checked Then
                                                            DD_L1 = New Matrix(SatObs.nRows, 1)
                                                            DD_w = New Matrix(DD_L1.nRows, DD_L1.nCols)
                                                            DD_A = New Matrix(DD_L1.nRows, 3)
                                                        ElseIf l1RadioButton.Checked Then
                                                            DD_L1 = New Matrix(SatObs.nRows, 1)
                                                            DD_w = New Matrix(DD_L1.nRows, DD_L1.nCols)
                                                            DD_A = New Matrix(DD_L1.nRows, SatObs.nRows + 3)
                                                        ElseIf c1l1RadioButton.Checked Then
                                                            DD_L1 = New Matrix(2 * SatObs.nRows, 1)
                                                            DD_w = New Matrix(DD_L1.nRows, DD_L1.nCols)
                                                            DD_A = New Matrix(DD_L1.nRows, SatObs.nRows + 3)
                                                        ElseIf l1fRadioButton.Checked Then
                                                            DD_L1 = New Matrix(SatObs.nRows, 1)
                                                            DD_w = New Matrix(DD_L1.nRows, DD_L1.nCols)
                                                            DD_A = New Matrix(DD_L1.nRows, 3)
                                                        End If

                                                        Dim rho_ref_basesat, rho_rem_basesat, rho_ref_sat, rho_rem_sat As Decimal

                                                        For i As Integer = 1 To SatObs.nRows

                                                            rho_ref_basesat = Math.Sqrt((BaseSatObs.data(1, 5) - refstatX) ^ 2 + (BaseSatObs.data(1, 6) - refstatY) ^ 2 + (BaseSatObs.data(1, 7) - refstatZ) ^ 2)
                                                            rho_rem_basesat = Math.Sqrt((BaseSatObs.data(1, 5) - DD_x.data(1, 1)) ^ 2 + (BaseSatObs.data(1, 6) - DD_x.data(2, 1)) ^ 2 + (BaseSatObs.data(1, 7) - DD_x.data(3, 1)) ^ 2)
                                                            rho_ref_sat = Math.Sqrt((SatObs.data(i, 5) - refstatX) ^ 2 + (SatObs.data(i, 6) - refstatY) ^ 2 + (SatObs.data(i, 7) - refstatZ) ^ 2)
                                                            rho_rem_sat = Math.Sqrt((SatObs.data(i, 5) - DD_x.data(1, 1)) ^ 2 + (SatObs.data(i, 6) - DD_x.data(2, 1)) ^ 2 + (SatObs.data(i, 7) - DD_x.data(3, 1)) ^ 2)

                                                            'double differences L1, C1, rho
                                                            resultsString(i) = Math.Round(DD_OBS(x - start + 1, i - 1), 4).ToString
                                                            resultsString(i + SatObs.nRows) = Math.Round(DD_OBS(x - start + 1, 3 * SatObs.nRows + i - 1), 4).ToString
                                                            resultsString(i + 2 * SatObs.nRows) = Math.Round((rho_rem_sat - rho_rem_basesat - (rho_ref_sat - rho_ref_basesat)), 4).ToString

                                                            'triple difference L1, rho and difference between them if cycle slip detection is enabled
                                                            If x - start >= 1 Then
                                                                resultsString(i + 3 * SatObs.nRows) = Math.Round(DD_OBS(x - start + 1, 1 * SatObs.nRows + i - 1), 4).ToString
                                                                resultsString(i + 4 * SatObs.nRows) = Math.Round(((rho_rem_sat - rho_rem_basesat - (rho_ref_sat - rho_ref_basesat)) - previousRhos(i)) / LAMBDA_L1, 4).ToString
                                                                If cycleSlipsCheckBox.Checked Then
                                                                    Dim magnitude As Decimal = (((rho_rem_sat - rho_rem_basesat - (rho_ref_sat - rho_ref_basesat)) - previousRhos(i)) / LAMBDA_L1 - DD_OBS(x - start + 1, 1 * SatObs.nRows + i - 1))
                                                                    resultsString(i + 7 * SatObs.nRows + 7) = Math.Round(magnitude, 3).ToString
                                                                    If f = numberofiterations Then
                                                                        If Math.Abs(magnitude) >= 1D Then   '1 cycle threshold
                                                                            cycleSlipsFound = True
                                                                        End If
                                                                    End If
                                                                End If
                                                            End If

                                                            previousRhos(i) = (rho_rem_sat - rho_rem_basesat - (rho_ref_sat - rho_ref_basesat))

                                                            If c1RadioButton.Checked Then
                                                                DD_L1.data(i, 1) = DD_OBS(x - start + 1, 3 * SatObs.nRows + i - 1)
                                                                DD_w.data(i, 1) = DD_L1.data(i, 1) - (rho_rem_sat - rho_rem_basesat - (rho_ref_sat - rho_ref_basesat))
                                                                Global_W.data((x - start) * SatObs.nRows + i, 1) = DD_w.data(i, 1)
                                                            ElseIf l1RadioButton.Checked Then
                                                                DD_L1.data(i, 1) = DD_OBS(x - start + 1, i - 1)
                                                                DD_w.data(i, 1) = DD_L1.data(i, 1) * LAMBDA_L1 - (rho_rem_sat - rho_rem_basesat - (rho_ref_sat - rho_ref_basesat) + LAMBDA_L1 * DD_x.data(i + 3, 1))
                                                                Global_W.data((x - start) * SatObs.nRows + i, 1) = DD_w.data(i, 1)
                                                            ElseIf c1l1RadioButton.Checked Then
                                                                DD_L1.data(i, 1) = DD_OBS(x - start + 1, i - 1)
                                                                DD_L1.data(SatObs.nRows + i, 1) = DD_OBS(x - start + 1, 3 * SatObs.nRows + i - 1)
                                                                DD_w.data(i, 1) = DD_L1.data(i, 1) * LAMBDA_L1 - (rho_rem_sat - rho_rem_basesat - (rho_ref_sat - rho_ref_basesat) + LAMBDA_L1 * DD_x.data(i + 3, 1))
                                                                DD_w.data(SatObs.nRows + i, 1) = DD_L1.data(SatObs.nRows + i, 1) - (rho_rem_sat - rho_rem_basesat - (rho_ref_sat - rho_ref_basesat))
                                                                Global_W.data((x - start) * SatObs.nRows + i, 1) = DD_w.data(i, 1)
                                                                Global_W.data((x - start + 1) * SatObs.nRows + i, 1) = DD_w.data(i, 1)
                                                            ElseIf l1fRadioButton.Checked Then
                                                                DD_L1.data(i, 1) = DD_OBS(x - start + 1, i - 1)
                                                                DD_w.data(i, 1) = DD_L1.data(i, 1) * LAMBDA_L1 - (rho_rem_sat - rho_rem_basesat - (rho_ref_sat - rho_ref_basesat) + LAMBDA_L1 * ambigs.data(i, 1))
                                                                Global_W.data((x - start) * SatObs.nRows + i, 1) = DD_w.data(i, 1)
                                                            End If

                                                            DD_A.data(i, 1) = (DD_x.data(1, 1) - SatObs.data(i, 5)) / rho_rem_sat - (DD_x.data(1, 1) - BaseSatObs.data(1, 5)) / rho_rem_basesat
                                                            DD_A.data(i, 2) = (DD_x.data(2, 1) - SatObs.data(i, 6)) / rho_rem_sat - (DD_x.data(2, 1) - BaseSatObs.data(1, 6)) / rho_rem_basesat
                                                            DD_A.data(i, 3) = (DD_x.data(3, 1) - SatObs.data(i, 7)) / rho_rem_sat - (DD_x.data(3, 1) - BaseSatObs.data(1, 7)) / rho_rem_basesat
                                                            Global_A.data((x - start) * SatObs.nRows + i, 1) = DD_A.data(i, 1)
                                                            Global_A.data((x - start) * SatObs.nRows + i, 2) = DD_A.data(i, 2)
                                                            Global_A.data((x - start) * SatObs.nRows + i, 3) = DD_A.data(i, 3)

                                                            If c1l1RadioButton.Checked Then
                                                                DD_A.data(SatObs.nRows + i, 1) = DD_A.data(i, 1)
                                                                DD_A.data(SatObs.nRows + i, 2) = DD_A.data(i, 2)
                                                                DD_A.data(SatObs.nRows + i, 3) = DD_A.data(i, 3)
                                                                Global_A.data((x - start + 1) * SatObs.nRows + i, 1) = DD_A.data(i, 1)
                                                                Global_A.data((x - start + 1) * SatObs.nRows + i, 2) = DD_A.data(i, 2)
                                                                Global_A.data((x - start + 1) * SatObs.nRows + i, 3) = DD_A.data(i, 3)
                                                            End If

                                                            If l1RadioButton.Checked Or c1l1RadioButton.Checked Then
                                                                For j As Integer = 1 To DD_L1.nRows
                                                                    If j = i AndAlso j <= SatObs.nRows Then
                                                                        DD_A.data(i, j + 3) = LAMBDA_L1
                                                                        Global_A.data((x - start) * SatObs.nRows + i, j + 3) = LAMBDA_L1
                                                                        Exit For
                                                                    End If
                                                                Next
                                                            End If

                                                        Next    'satellite loop

                                                        Dim DD_Ni As Matrix = DD_A.Transpose() * P * DD_A
                                                        Dim DD_Ui As Matrix = DD_A.Transpose * P * DD_w

                                                        'summation of normals
                                                        DD_N = DD_N + DD_Ni
                                                        DD_U = DD_U + DD_Ui

                                                        If x - start >= 1 And f = numberofiterations Then
                                                            Dim Ninv As New Matrix(1, 1, True)
                                                            Ninv.matrixReDim(DD_N.nRows, DD_N.nCols, True)
                                                            Ninv = DD_N.Inverse

                                                            Dim delta_i As Matrix = Ninv * DD_U
                                                            Dim Xi As Matrix = DD_x + delta_i
                                                            For i As Integer = 1 To 3
                                                                resultsString(6 * SatObs.nRows + i) = Math.Round(Xi.data(i, 1), 4).ToString
                                                            Next
                                                            If l1RadioButton.Checked Or c1l1RadioButton.Checked Then
                                                                For i As Integer = 1 To SatObs.nRows
                                                                    resultsString(5 * SatObs.nRows + i) = Math.Round(Xi.data(i + 3, 1), 4).ToString
                                                                Next
                                                                Dim passFail As Boolean
                                                                Dim FixAmbs, ss, Pos_Fixed, Pos_CV As Matrix
                                                                Try
                                                                    lambda(Xi, Ninv, passFail, FixAmbs, ss, Pos_Fixed, Pos_CV)
                                                                Catch ex As Exception
                                                                    passFail = False
                                                                End Try

                                                                If passFail = True Then
                                                                    Dim ratio As Decimal = Math.Round((ss.data(2, 1) / ss.data(1, 1)), 3)
                                                                    resultsString(6 * SatObs.nRows + 4) = ratio.ToString
                                                                    For i As Integer = 1 To SatObs.nRows
                                                                        resultsString(6 * SatObs.nRows + 4 + i) = FixAmbs.data(i, 1).ToString
                                                                    Next
                                                                    cellColour = Color.Red
                                                                    If ratio >= 3D Then
                                                                        cellColour = Color.LightGreen
                                                                        For i As Integer = 1 To 3
                                                                            resultsString(7 * SatObs.nRows + 4 + i) = Math.Round(Pos_Fixed.data(i, 1), 4).ToString
                                                                        Next
                                                                    End If
                                                                End If
                                                            End If
                                                        End If
                                                        If f = numberofiterations Then
                                                            resultsGrid.Rows.Add(resultsString)
                                                            If x - start >= 1 Then
                                                                resultsGrid.Rows(x - start).Cells(6 * SatObs.nRows + 4).Style.BackColor = cellColour
                                                                If cycleSlipsCheckBox.Checked Then
                                                                    For k As Integer = resultsString.Length - SatObs.nRows To resultsString.Length - 1
                                                                        If resultsString(k) <> String.Empty Then
                                                                            Dim cycleValue As Decimal = Decimal.Parse(resultsString(k))
                                                                            If Math.Abs(cycleValue) >= 1D Then
                                                                                resultsGrid.Rows(x - start).Cells(k).Style.BackColor = Color.Red
                                                                            End If
                                                                        End If
                                                                    Next
                                                                End If
                                                            End If
                                                        End If
                                                    End If
                                                Next    'epochs loop

                                                DD_delta = DD_N.Inverse * DD_U
                                                Dim Global_V As Matrix = Global_A * DD_delta - Global_W
                                                Dim apost As Decimal = 0
                                                Dim obs As Integer = commonSats.Count - 1
                                                If c1l1RadioButton.Checked Then
                                                    obs += obs
                                                End If
                                                For i As Integer = start To endd
                                                    Dim lilV As New Matrix(obs, 1)
                                                    For j As Integer = 1 To obs
                                                        Dim indVindex As Integer = (i - start) * (commonSats.Count - 1) + j
                                                        lilV.data(j, 1) = Global_V.data(indVindex, 1)
                                                    Next
                                                    apost += (lilV.Transpose * P * lilV).toScalar
                                                Next
                                                apost /= (Global_A.nRows - Global_A.nCols)
                                                DD_x = DD_x + DD_delta
                                                apost = 1
                                                CV_DD_x = apost * DD_N.Inverse

                                            Next    'iterations loop

                                            ProgressBar2.Value = ProgressBar2.Minimum
                                            ProgressBar2.Refresh()
                                            preprocessingLabel.Text = "Displaying Results..."
                                            preprocessingLabel.Visible = True
                                            preprocessingLabel.Refresh()
                                            resultsGrid.AutoResizeColumns()
                                            resultsGrid.AutoResizeRows()

                                            If cycleSlipsCheckBox.Checked Then
                                                If cycleSlipsFound Then
                                                    preprocessingLabel.Text = "** CYCLE SLIPS DETECTED **"
                                                Else
                                                    preprocessingLabel.Text = "No cycle slips detected"
                                                End If
                                            Else
                                                preprocessingLabel.Visible = False
                                            End If
                                            preprocessingLabel.Refresh()

                                            Dim stdDevs As Matrix = CV_DD_x.getDiagonal.Sqrt

                                            remStatXTextBox.Text = Math.Round(DD_x.data(1, 1), 4).ToString & "  " & Chr(177) & "  " & Math.Round(stdDevs.data(1, 1), 4).ToString
                                            remStatYTextBox.Text = Math.Round(DD_x.data(2, 1), 4).ToString & "  " & Chr(177) & "  " & Math.Round(stdDevs.data(2, 1), 4).ToString
                                            remStatZTextBox.Text = Math.Round(DD_x.data(3, 1), 4).ToString & "  " & Chr(177) & "  " & Math.Round(stdDevs.data(3, 1), 4).ToString

                                            remCoordGroupBox.Text = "Remote Station Coordinates (DGPS)"

                                            If l1RadioButton.Checked Or c1l1RadioButton.Checked Then
                                                remCoordGroupBox.Text = "Remote Station Coordinates (FLOAT)"
                                                Dim passFail As Boolean
                                                Dim FixAmbs, ss, Pos_Fixed, Pos_CV As Matrix
                                                lambda(DD_x, CV_DD_x, passFail, FixAmbs, ss, Pos_Fixed, Pos_CV)

                                                If passFail = True Then
                                                    Dim ratio As Decimal = ss.data(2, 1) / ss.data(1, 1)
                                                    If ratio >= 3 Then
                                                        Dim stdDevsF As Matrix = Pos_CV.getDiagonal.Sqrt

                                                        remCoordGroupBox.Text = "Remote Station Coordinates (FIXED)"
                                                        remStatXTextBox.Text = Math.Round(Pos_Fixed.data(1, 1), 4).ToString & "  " & Chr(177) & "  " & Math.Round(stdDevsF.data(1, 1), 4).ToString
                                                        remStatYTextBox.Text = Math.Round(Pos_Fixed.data(2, 1), 4).ToString & "  " & Chr(177) & "  " & Math.Round(stdDevsF.data(1, 1), 4).ToString
                                                        remStatZTextBox.Text = Math.Round(Pos_Fixed.data(3, 1), 4).ToString & "  " & Chr(177) & "  " & Math.Round(stdDevsF.data(1, 1), 4).ToString
                                                    Else

                                                    End If
                                                Else
                                                    MessageBox.Show("Unable to determine integer ambiguity candidates")
                                                End If
                                            End If
                                        Else
                                            MessageBox.Show("L1 pseudorange and/or phase observations could not be found within the reference station and/or remote station RINEX data", "PROCESSING FAILED", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                        End If
                                    Catch ex As Exception
                                        MessageBox.Show("An unknown error occurred during processing", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                Catch ex As Exception
                                    MessageBox.Show("Invalid Remote Station Z coordinate", "INVALID DATA", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                End Try
                            Catch ex As Exception
                                MessageBox.Show("Invalid Remote Station Y coordinate", "INVALID DATA", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            End Try
                        Catch ex As Exception
                            MessageBox.Show("Invalid Remote Station X coordinate", "INVALID DATA", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End Try
                    Catch ex As Exception
                        MessageBox.Show("Invalid Reference Station Z coordinate", "INVALID DATA", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End Try
                Catch ex As Exception
                    MessageBox.Show("Invalid Reference Station Y coordinate", "INVALID DATA", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Try
            Catch ex As Exception
                MessageBox.Show("Invalid Reference Station X coordinate", "INVALID DATA", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Try
        Else
            MessageBox.Show("No common satellites where observed during the specified period of time between the specified reference station and remote station", "PROCESSING FAILED", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If
        processButton.Enabled = True
        processButton.Refresh()
    End Sub

    Private Sub lambda(ByVal floatResults As Matrix, ByVal floatCV As Matrix, ByRef passFail As Boolean, ByRef fixedAmbs As Matrix, ByRef sumSquares As Matrix, ByRef Pos_Fixed As Matrix, ByRef Pos_CV As Matrix)

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '   NOTICE: The following implementation of the LAMBDA decorrelation method along with      '
        '           the Modified LAMBDA (MLAMBDA) search technique was ported by me (Ryan Brazeal)  '
        '           from the C++ based RTKLIB project (http://www.rtklib.com/). The outstanding     '
        '           work on the RTKLIB project is respectfully noted here and sincerely thanked!    '
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        'INPUTS
        Dim b As New Matrix(3, 1)
        Dim a As New Matrix(floatResults.nRows - 3, 1)
        Dim Qba As New Matrix(3, floatResults.nRows - 3)
        Dim Qaa As New Matrix(floatResults.nRows - 3, floatResults.nRows - 3, True)
        Dim Qbb As New Matrix(3, 3)

        'OUTPUTS
        Dim m As Integer = 2    'number of candidate solutions to return
        Dim F As New Matrix(a.nRows, m)  'fixed candidates
        Dim s As New Matrix(m, 1)    'sum of squared residuals of fixed candidates

        'RETURNS
        passFail = False
        fixedAmbs = New Matrix(a.nRows, m)
        sumSquares = New Matrix(m, 1)
        Pos_Fixed = New Matrix(3, 1)
        Pos_CV = New Matrix(3, 3)

        'populate matrices
        b.data(1, 1) = floatResults.data(1, 1)
        b.data(2, 1) = floatResults.data(2, 1)
        b.data(3, 1) = floatResults.data(3, 1)

        For i As Integer = 4 To floatResults.nRows
            a.data(i - 3, 1) = floatResults.data(i, 1)
        Next

        For i As Integer = 1 To 3
            For j As Integer = 4 To floatResults.nRows
                Qba.data(i, j - 3) = floatCV.data(i, j)
            Next
        Next

        For i As Integer = 4 To floatResults.nRows
            For j As Integer = 4 To floatResults.nRows
                Qaa.data(i - 3, j - 3) = floatCV.data(i, j)
            Next
        Next

        For i As Integer = 1 To 3
            For j As Integer = 1 To 3
                Qbb.data(i, j) = floatCV.data(i, j)
            Next
        Next

        '''''''''''''''''''''LD decomposition'''''''''''''''''''''''''
        Dim aVal As Decimal
        Dim continueState As Boolean = True
        Dim n As Integer = Qaa.nRows
        Dim L As New Matrix(n, n)
        Dim D As New Matrix(n, n)
        Dim AA As New Matrix(n, n)
        AA.equals(Qaa)

        For i As Integer = n To 1 Step -1
            If AA.data(i, i) > 0 Then
                D.data(i, i) = AA.data(i, i)
                aVal = Math.Sqrt(D.data(i, i))
                For j As Integer = 1 To i
                    L.data(i, j) = AA.data(i, j) / aVal
                Next
                For j As Integer = 1 To i - 1
                    For k As Integer = 1 To j
                        AA.data(j, k) -= L.data(i, k) * L.data(i, j)
                    Next
                Next
                For j As Integer = 1 To i
                    L.data(i, j) /= L.data(i, i)
                Next
            Else
                continueState = False
            End If
        Next

        If continueState Then
            'L.printAll()
            'D.printAll()

            Dim bigZ As New Matrix(n, n, True)
            bigZ = bigZ.makeIdentity()

            Dim del As Decimal

            Dim i As Integer
            Dim j As Integer = n - 1
            Dim k As Integer = n - 1

            While (j >= 1)
                If j <= k Then
                    For i = j + 1 To n
                        ''''''''''''''''''''''Gauss transformation''''''''''''''''
                        Dim tester = L.data(i, j)
                        Dim mu As Integer = Convert.ToInt32(Math.Round((Math.Floor(L.data(i, j) + 0.5D)), 0))
                        If mu <> 0I Then
                            For kk As Integer = i To n
                                L.data(kk, j) -= Convert.ToDecimal(mu) * L.data(kk, i)
                            Next
                            For kk As Integer = 1 To n
                                bigZ.data(kk, j) -= Convert.ToDecimal(mu) * bigZ.data(kk, i)
                            Next
                        End If

                    Next
                End If
                del = D.data(j, j) + L.data(j + 1, j) ^ 2 * D.data(j + 1, j + 1)
                If del + 0.000001 < D.data(j + 1, j + 1) Then
                    ''''''''''''''Permutation''''''''''''''''
                    Dim eta, lam, a0, a1 As Decimal

                    eta = D.data(j, j) / del
                    lam = D.data(j + 1, j + 1) * L.data(j + 1, j) / del
                    D.data(j, j) = eta * D.data(j + 1, j + 1)
                    D.data(j + 1, j + 1) = del
                    For kk As Integer = 1 To j - 1
                        a0 = L.data(j, kk)
                        a1 = L.data(j + 1, kk)
                        L.data(j, kk) = -L.data(j + 1, j) * a0 + a1
                        L.data(j + 1, kk) = eta * a0 + lam * a1
                    Next
                    L.data(j + 1, j) = lam
                    For kk As Integer = j + 2 To n
                        Dim temp As Decimal = L.data(kk, j)
                        L.data(kk, j) = L.data(kk, j + 1)
                        L.data(kk, j + 1) = temp
                    Next
                    For kk As Integer = 1 To n
                        Dim temp As Decimal = bigZ.data(kk, j)
                        bigZ.data(kk, j) = bigZ.data(kk, j + 1)
                        bigZ.data(kk, j + 1) = temp
                    Next
                    k = j
                    j = n - 1
                Else
                    j -= 1
                End If
            End While

            'transformed ambiguities
            Dim zs As Matrix = bigZ.Transpose * a

            Dim zn As New Matrix(n, m)

            ''''''''''''''''''Modified LAMBDA search approach'''''''''''''''''''''
            Dim loopMax As Integer = 10000

            i = 0
            j = 0
            Dim c, nn, imax As Integer
            nn = 1
            imax = 1

            Dim newdist, y As Decimal
            Dim maxdist As Decimal = 1.0E+28

            Dim bigS As New Matrix(n, n)
            Dim dist As New Matrix(n, 1)
            Dim zb As New Matrix(n, 1)
            Dim z As New Matrix(n, 1)
            Dim stepp As New Matrix(n, 1)

            k = n
            dist.data(k, 1) = 0D
            zb.data(k, 1) = zs.data(k, 1)
            z.data(k, 1) = Math.Round((Math.Floor(zb.data(k, 1) + 0.5D)))
            y = zb.data(k, 1) - z.data(k, 1)
            If y <= 0D Then
                stepp.data(k, 1) = -1D
            Else
                stepp.data(k, 1) = 1D
            End If
            For c = 1 To loopMax
                newdist = dist.data(k, 1) + y ^ 2 / D.data(k, k)
                If newdist < maxdist Then
                    If k <> 1I Then
                        k -= 1
                        dist.data(k, 1) = newdist
                        For i = 1 To k
                            bigS.data(k, i) = bigS.data(k + 1, i) + (z.data(k + 1, 1) - zb.data(k + 1, 1)) * L.data(k + 1, i)
                        Next
                        zb.data(k, 1) = zs.data(k, 1) + bigS.data(k, k)
                        z.data(k, 1) = Math.Round((Math.Floor(zb.data(k, 1) + 0.5D)))
                        y = zb.data(k, 1) - z.data(k, 1)
                        If y <= 0D Then
                            stepp.data(k, 1) = -1D
                        Else
                            stepp.data(k, 1) = 1D
                        End If
                    Else
                        If nn < m + 1 Then
                            If nn = 1I Or newdist > s.data(imax, 1) Then
                                imax = nn
                            End If
                            For i = 1 To n
                                zn.data(i, nn) = z.data(i, 1)
                            Next
                            s.data(nn, 1) = newdist
                            nn += 1
                        Else
                            If newdist < s.data(imax, 1) Then
                                For i = 1 To n
                                    zn.data(i, imax) = z.data(i, 1)
                                Next
                                s.data(imax, 1) = newdist
                                imax = 1
                                For i = 1 To m
                                    If s.data(imax, 1) < s.data(i, 1) Then
                                        imax = i
                                    End If
                                Next
                            End If
                            maxdist = s.data(imax, 1)
                        End If
                        z.data(1, 1) += stepp.data(1, 1)
                        y = zb.data(1, 1) - z.data(1, 1)
                        If stepp.data(1, 1) <= 0D Then
                            stepp.data(1, 1) = -stepp.data(1, 1) + 1D
                        Else
                            stepp.data(1, 1) = -stepp.data(1, 1) - 1D
                        End If
                    End If
                Else
                    If k = n Then
                        Exit For
                    Else
                        k += 1
                        z.data(k, 1) += stepp.data(k, 1)
                        y = zb.data(k, 1) - z.data(k, 1)
                        If stepp.data(k, 1) <= 0D Then
                            stepp.data(k, 1) = -stepp.data(k, 1) + 1D
                        Else
                            stepp.data(k, 1) = -stepp.data(k, 1) - 1D
                        End If
                    End If
                End If
            Next

            For i = 1 To m - 1
                For j = i + 1 To m
                    If s.data(i, 1) < s.data(j, 1) Then
                        Continue For
                    End If
                    Dim temp As Decimal = s.data(i, 1)
                    s.data(i, 1) = s.data(j, 1)
                    s.data(j, 1) = temp
                    For k = 1 To n
                        Dim temp2 As Decimal = zn.data(k, i)
                        zn.data(k, i) = zn.data(k, j)
                        zn.data(k, j) = temp2
                    Next
                Next
            Next

            If c < loopMax Then
                Dim bZt As New Matrix(bigZ.nCols, bigZ.nRows, True)
                bZt = bigZ.Transpose

                F = bZt.Inverse * zn

                For ii As Integer = 1 To F.nRows
                    For jj As Integer = 1 To F.nCols
                        F.data(ii, jj) = Math.Round(F.data(ii, jj), 0)
                    Next
                Next

                Dim b_fixed As Matrix = b - Qba * Qaa.Inverse * (a - F.getColumn(1))
                Dim Qbb_fixed As Matrix = Qbb - Qba * Qaa.Inverse * Qba.Transpose

                fixedAmbs.equals(F)
                sumSquares.equals(s)
                Pos_Fixed.equals(b_fixed)
                Pos_CV.equals(Qbb_fixed)
                passFail = True
            End If
        End If
    End Sub

    Private Function constructOBS(ByRef returnStatus As Boolean, ByVal refStatObsAdj As RINEX_Obs, ByRef remStatObsAdj As RINEX_Obs) As Decimal(,)
        Dim dummyReturn(0, 0) As Decimal
        If baseSatComboBox.Items.Count > 0 Then
            Dim baseSat As Integer = commonSats.Item(baseSatComboBox.SelectedIndex)
            Dim start As Integer = firstCommonEpochComboBox.SelectedIndex
            Dim endd As Integer = firstCommonEpochComboBox.Items.IndexOf(lastCommonEpochComboBox.Items(lastCommonEpochComboBox.SelectedIndex))
            If endd > start Then

                Dim L1IndexRef As Integer = -9999I
                Dim L1IndexRem As Integer = -9999I
                Dim L2IndexRef As Integer = -9999I
                Dim L2IndexRem As Integer = -9999I
                Dim C1IndexRef As Integer = -9999I
                Dim C1IndexRem As Integer = -9999I

                'locate the L1 and L2 observables
                For m As Integer = 0 To 17
                    If refStatObsAdj.Obs_Type(m).ToUpper = "L1" Then
                        L1IndexRef = m
                    End If

                    If remStatObsAdj.Obs_Type(m).ToUpper = "L1" Then
                        L1IndexRem = m
                    End If

                    If refStatObsAdj.Obs_Type(m).ToUpper = "L2" Then
                        L2IndexRef = m
                    End If

                    If remStatObsAdj.Obs_Type(m).ToUpper = "L2" Then
                        L2IndexRem = m
                    End If

                    If refStatObsAdj.Obs_Type(m).ToUpper = "C1" Then
                        C1IndexRef = m
                    End If

                    If remStatObsAdj.Obs_Type(m).ToUpper = "C1" Then
                        C1IndexRem = m
                    End If
                Next

                '1 extra row for PRNs of satellites, last column contains double differenced C1 pseudoranges
                Dim DD_L1((endd - start + 1), 4 * (commonSats.Count - 2) + 3) As Decimal

                If L1IndexRef <> -9999I And L1IndexRem <> -9999I And C1IndexRef <> -9999I And C1IndexRem <> -9999I Then 'And L2IndexRef <> -9999I And L2IndexRem <> -9999I Then
                    For x As Integer = start To endd
                        Dim satCount As Integer = -1
                        ProgressBar2.Value = x
                        If ProgressBar2.Value Mod 25 = 0 Then
                            ProgressBar2.Refresh()
                        End If
                        Dim refIndex As Integer = findIndexofWeekSecondsInRef(firstCommonEpochComboBox.Items(x), refStatObsAdj)
                        Dim remIndex As Integer = findIndexofWeekSecondsInRem(firstCommonEpochComboBox.Items(x), remStatObsAdj)

                        For r As Integer = 0 To commonSats.Count - 1
                            Dim satToFind As Integer = commonSats(r)
                            If satToFind <> baseSat Then
                                satCount += 1
                                DD_L1(0, satCount) = satToFind

                                Dim i, z1, z2 As Integer
                                Dim SatNum As Integer
                                Dim L1ref, L1rem, L1ref_base, L1rem_base As Decimal
                                'Dim L2ref, L2rem, L2ref_base, L2rem_base As Decimal
                                Dim C1ref, C1rem, C1ref_base, C1rem_base As Decimal

                                z1 = refIndex
                                z2 = remIndex

                                For i = 4 To remStatObsAdj.NumRows(z2) - 1
                                    L1rem_base = 0D
                                    'L2rem_base = 0D
                                    C1rem_base = 0D

                                    SatNum = remStatObsAdj.ObsData(i, 0, z2)
                                    If SatNum = baseSat Then
                                        L1rem_base = remStatObsAdj.ObsData(i, (L1IndexRem + 1), z2)
                                        'L2rem_base = remstatobsadj.ObsData(i, (L2IndexRem + 1), z2)
                                        C1rem_base = remStatObsAdj.ObsData(i, (C1IndexRem + 1), z2)
                                        Exit For
                                    End If
                                Next

                                For i = 4 To remStatObsAdj.NumRows(z2) - 1
                                    L1rem = 0D
                                    'L2rem = 0D
                                    C1rem = 0D

                                    SatNum = remStatObsAdj.ObsData(i, 0, z2)
                                    If SatNum = satToFind Then
                                        L1rem = remStatObsAdj.ObsData(i, (L1IndexRem + 1), z2)
                                        'L2rem = remstatobsadj.ObsData(i, (L2IndexRem + 1), z2)
                                        C1rem = remStatObsAdj.ObsData(i, (C1IndexRem + 1), z2)
                                        Exit For
                                    End If
                                Next

                                For i = 4 To refStatObsAdj.NumRows(z1) - 1
                                    L1ref_base = 0D
                                    'L2ref_base = 0D
                                    C1ref_base = 0D

                                    SatNum = refStatObsAdj.ObsData(i, 0, z1)
                                    If SatNum = baseSat Then
                                        L1ref_base = refStatObsAdj.ObsData(i, (L1IndexRef + 1), z1)
                                        'L2ref_base = refstatobsadj.ObsData(i, (L2IndexRef + 1), z1)
                                        C1ref_base = refStatObsAdj.ObsData(i, (C1IndexRef + 1), z1)
                                        Exit For
                                    End If
                                Next

                                For i = 4 To refStatObsAdj.NumRows(z1) - 1
                                    L1ref = 0D
                                    'L2ref = 0D
                                    C1ref = 0D

                                    SatNum = refStatObsAdj.ObsData(i, 0, z1)
                                    If SatNum = satToFind Then
                                        L1ref = refStatObsAdj.ObsData(i, (L1IndexRef + 1), z1)
                                        'L2ref = refstatobsadj.ObsData(i, (L2IndexRef + 1), z1)
                                        C1ref = refStatObsAdj.ObsData(i, (C1IndexRef + 1), z1)
                                        Exit For
                                    End If
                                Next

                                If L1rem_base <> 0D AndAlso L1rem <> 0D AndAlso L1ref_base <> 0D AndAlso L1ref <> 0D Then
                                    If C1rem_base <> 0D AndAlso C1rem <> 0D AndAlso C1ref_base <> 0D AndAlso C1ref <> 0D Then
                                        'If L2rem_base <> 0D AndAlso L2rem <> 0D AndAlso L2ref_base <> 0D AndAlso L2ref <> 0D Then

                                        DD_L1(x - start + 1, satCount) = L1rem - L1rem_base - (L1ref - L1ref_base)
                                        DD_L1(x - start + 1, satCount + 3 * (commonSats.Count - 2) + 3) = C1rem - C1rem_base - (C1ref - C1ref_base)

                                        If x > start Then   'triple differences, generates spikes
                                            DD_L1(x - start + 1, satCount + (commonSats.Count - 2) + 1) = DD_L1(x - start + 1, satCount) - DD_L1(x - start, satCount)
                                        End If

                                        If x > start + 1 Then 'difference in triple differences (triple difference rate), generates sparks
                                            DD_L1(x - start + 1, satCount + 2 * (commonSats.Count - 2) + 2) = DD_L1(x - start + 1, satCount + (commonSats.Count - 2) + 1) - DD_L1(x - start, satCount + (commonSats.Count - 2) + 1)
                                        End If

                                        'Else
                                        'DD_L1(x - start + 1, satCount) = -999999999.999D
                                        'End If
                                    Else
                                        DD_L1(x - start + 1, satCount) = -999999999.999D
                                    End If
                                Else
                                    DD_L1(x - start + 1, satCount) = -999999999.999D
                                End If
                            End If
                        Next
                    Next
                    'experimental cycle slip detect and fixing process

                    'If cycleSlipsCheckBox.Checked Then
                    '    For i As Integer = 1 To DD_L1.GetLength(0) - 1
                    '        For j As Integer = 0 To commonSats.Count - 2
                    '            If Math.Abs(DD_L1(i, j + 2 * (commonSats.Count - 2) + 2)) > 1D Then    '1 cycle threshold
                    '                Dim adjustment As Decimal = Math.Round(DD_L1(i, j + 2 * (commonSats.Count - 2) + 2), 0)
                    '                For k As Integer = i To DD_L1.GetLength(0) - 1
                    '                    Dim oldValue As Decimal = DD_L1(k, j)
                    '                    If oldValue <> -999999999.999D Then
                    '                        DD_L1(k, j) = oldValue - adjustment
                    '                    End If
                    '                Next
                    '                If i <> DD_L1.GetLength(0) - 1 Then
                    '                    DD_L1(i + 1, j + 2 * (commonSats.Count - 2) + 2) = 0
                    '                End If
                    '            End If
                    '        Next
                    '    Next
                    'End If

                    'ProgressBar2.Visible = False
                    returnStatus = True
                    Return DD_L1
                Else
                    returnStatus = False
                    Return dummyReturn
                End If
            Else
                'MessageBox.Show("More than a single epoch of data is need to analyze cycle slips", "NOT ENOUGH DATA", MessageBoxButtons.OK, MessageBoxIcon.Information)
                returnStatus = False
                Return dummyReturn
            End If
        Else
            'MessageBox.Show("There are no common satellites between these sets of observations hence cycle slips cannot be detected", "NOT ENOUGH DATA", MessageBoxButtons.OK, MessageBoxIcon.Information)
            returnStatus = False
            Return dummyReturn
        End If
    End Function

    Private Sub plotFiguresButton_Click(sender As Object, e As EventArgs) Handles plotFiguresButton.Click
        Dim start As Integer = firstCommonEpochComboBox.SelectedIndex
        Dim endd As Integer = firstCommonEpochComboBox.Items.IndexOf(lastCommonEpochComboBox.Items(lastCommonEpochComboBox.SelectedIndex))

        Dim epochData As New Matrix(endd - start + 1, 6)

        For i As Integer = start To endd
            Dim timeString As String = resultsGrid.Rows(i - start).Cells(0).Value.ToString
            Dim delim(0) As Char
            delim(0) = ":"
            Dim splitString() As String = timeString.Split(delim)
            splitString(0) = splitString(0).Trim()
            splitString(1) = splitString(1).Trim()

            epochData.data(i - start + 1, 6) = Decimal.Parse(splitString(0))
            epochData.data(i - start + 1, 1) = Decimal.Parse(splitString(1))

            If resultsGrid.Rows(i - start).Cells(plot1ComboBox.SelectedIndex).Value Is Nothing Then
                epochData.data(i - start + 1, 2) = -999999999.999999999D
            Else
                If resultsGrid.Rows(i - start).Cells(plot1ComboBox.SelectedIndex).Value.ToString <> String.Empty Then
                    If plot1ComboBox.SelectedIndex <> 0 Then
                        epochData.data(i - start + 1, 2) = Decimal.Parse(resultsGrid.Rows(i - start).Cells(plot1ComboBox.SelectedIndex).Value.ToString)
                    Else
                        epochData.data(i - start + 1, 2) = 0D
                    End If

                Else
                    epochData.data(i - start + 1, 2) = -999999999.999999999D
                End If
            End If

            If resultsGrid.Rows(i - start).Cells(plot2ComboBox.SelectedIndex).Value Is Nothing Then
                epochData.data(i - start + 1, 3) = -999999999.999999999D
            Else
                If resultsGrid.Rows(i - start).Cells(plot2ComboBox.SelectedIndex).Value.ToString <> String.Empty Then
                    If plot2ComboBox.SelectedIndex <> 0 Then
                        epochData.data(i - start + 1, 3) = Decimal.Parse(resultsGrid.Rows(i - start).Cells(plot2ComboBox.SelectedIndex).Value.ToString)
                    Else
                        epochData.data(i - start + 1, 3) = 0D
                    End If
                Else
                    epochData.data(i - start + 1, 3) = -999999999.999999999D
                End If
            End If

            If resultsGrid.Rows(i - start).Cells(plot3ComboBox.SelectedIndex).Value Is Nothing Then
                epochData.data(i - start + 1, 4) = -999999999.999999999D
            Else
                If resultsGrid.Rows(i - start).Cells(plot3ComboBox.SelectedIndex).Value.ToString <> String.Empty Then
                    If plot3ComboBox.SelectedIndex <> 0 Then
                        epochData.data(i - start + 1, 4) = Decimal.Parse(resultsGrid.Rows(i - start).Cells(plot3ComboBox.SelectedIndex).Value.ToString)
                    Else
                        epochData.data(i - start + 1, 4) = 0D
                    End If
                Else
                    epochData.data(i - start + 1, 4) = -999999999.999999999D
                End If
            End If

            If resultsGrid.Rows(i - start).Cells(plot4ComboBox.SelectedIndex).Value Is Nothing Then
                epochData.data(i - start + 1, 5) = -999999999.999999999D
            Else
                If resultsGrid.Rows(i - start).Cells(plot4ComboBox.SelectedIndex).Value.ToString <> String.Empty Then
                    If plot4ComboBox.SelectedIndex <> 0 Then
                        epochData.data(i - start + 1, 5) = Decimal.Parse(resultsGrid.Rows(i - start).Cells(plot4ComboBox.SelectedIndex).Value.ToString)
                    Else
                        epochData.data(i - start + 1, 5) = 0D
                    End If
                Else
                    epochData.data(i - start + 1, 5) = -999999999.999999999D
                End If
            End If
        Next

        Dim Time_Values, Week_Values, Plot1_Values, Plot2_Values, Plot3_Values, Plot4_Values As Matrix
        Dim MaxMinTime(,), MaxMinPlot1(), MaxMinPlot2(), MaxMinPlot3(), MaxMinPlot4() As Decimal

        Time_Values = epochData.getColumn(1)
        Week_Values = epochData.getColumn(6)
        Plot1_Values = epochData.getColumn(2)
        Plot2_Values = epochData.getColumn(3)
        Plot3_Values = epochData.getColumn(4)
        Plot4_Values = epochData.getColumn(5)

        MaxMinTime = ComputeMaxMinTime(Time_Values, Week_Values)
        MaxMinPlot1 = ComputeMaxMin(Plot1_Values)
        MaxMinPlot2 = ComputeMaxMin(Plot2_Values)
        MaxMinPlot3 = ComputeMaxMin(Plot3_Values)
        MaxMinPlot4 = ComputeMaxMin(Plot4_Values)

        epochData = epochData.matrixReDim(endd - start + 7, 6, True)

        epochData.data(epochData.nRows - 5, 1) = MaxMinTime(1, 0)
        epochData.data(epochData.nRows - 5, 2) = MaxMinTime(1, 1)

        epochData.data(epochData.nRows - 4, 1) = MaxMinTime(0, 0)
        epochData.data(epochData.nRows - 4, 2) = MaxMinTime(0, 1)

        epochData.data(epochData.nRows - 3, 1) = MaxMinPlot1(0)
        epochData.data(epochData.nRows - 3, 2) = MaxMinPlot1(1)

        epochData.data(epochData.nRows - 2, 1) = MaxMinPlot2(0)
        epochData.data(epochData.nRows - 2, 2) = MaxMinPlot2(1)

        epochData.data(epochData.nRows - 1, 1) = MaxMinPlot3(0)
        epochData.data(epochData.nRows - 1, 2) = MaxMinPlot3(1)

        epochData.data(epochData.nRows, 1) = MaxMinPlot4(0)
        epochData.data(epochData.nRows, 2) = MaxMinPlot4(1)

        diffPlots.DOP_Data = epochData
        diffPlots.plot1Label = plot1ComboBox.Items(plot1ComboBox.SelectedIndex)
        diffPlots.plot2Label = plot2ComboBox.Items(plot2ComboBox.SelectedIndex)
        diffPlots.plot3Label = plot3ComboBox.Items(plot3ComboBox.SelectedIndex)
        diffPlots.plot4Label = plot4ComboBox.Items(plot4ComboBox.SelectedIndex)

        For i As Integer = 1 To epochData.nRows
            If epochData.data(i, 2) = -999999999.999999999D Then
                epochData.data(i, 2) = MaxMinPlot1(1)
            End If
            If epochData.data(i, 3) = -999999999.999999999D Then
                epochData.data(i, 3) = MaxMinPlot2(1)
            End If
            If epochData.data(i, 4) = -999999999.999999999D Then
                epochData.data(i, 4) = MaxMinPlot3(1)
            End If
            If epochData.data(i, 5) = -999999999.999999999D Then
                epochData.data(i, 5) = MaxMinPlot4(1)
            End If
        Next

        diffPlots.ShowDialog()
    End Sub

End Class
