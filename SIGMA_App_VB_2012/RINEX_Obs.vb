'''''SIGMA'''''
'By: Ryan Brazeal
'Date: April 2015
'GNU GPL V2 License
'www.rgbi.ca
'www.jrpi.ca

Imports System.IO

Public Class RINEX_Obs
    Private Rinex_VersionV As String
    Private File_TypeV As String
    Private Sat_SystemV As String
    Private Date_File_CreatedV As String
    Private Marker_NameV As String
    Private Receiver_NumV As String
    Private Receiver_TypeV As String
    Private Receiver_VersionV As String
    Private Antenna_NumV As String
    Private Antenna_TypeV As String
    Private Approx_WGS84_X As Decimal
    Private Approx_WGS84_Y As Decimal
    Private Approx_WGS84_Z As Decimal
    Private Antenna_Offset_H As Decimal
    Private Antenna_Offset_E As Decimal
    Private Antenna_Offset_N As Decimal
    Private Num_Obs_TypeV As Integer
    Private Obs_TypeV(17) As String
    Private Obs_IntervalV As Decimal
    Private Leap_SecondsV As Integer
    Private Obs_RecordsV(0, 0, 0) As Decimal
    Private Dual_FrequencyV As Boolean
    Private File_LoadedV As Boolean
    Private File_Path_And_NameV As String
    Private recordCount As Integer

    ReadOnly Property Approx_X() As Decimal
        Get
            Return Approx_WGS84_X
        End Get
    End Property

    ReadOnly Property Approx_Y() As Decimal
        Get
            Return Approx_WGS84_Y
        End Get
    End Property

    ReadOnly Property Approx_Z() As Decimal
        Get
            Return Approx_WGS84_Z
        End Get
    End Property

    ReadOnly Property ObsData(ByVal row As Integer, ByVal col As Integer, ByVal epoch As Integer)
        Get
            Return Obs_RecordsV(row, col, epoch)
        End Get
    End Property

    ReadOnly Property NumRows(ByVal epoch As Integer)
        Get
            Return Obs_RecordsV.GetUpperBound(0) + 1
        End Get
    End Property

    ReadOnly Property NumCols(ByVal epoch As Integer)
        Get
            Return Obs_RecordsV.GetUpperBound(1) + 1
        End Get
    End Property

    ReadOnly Property Rinex_Version() As String
        Get
            Return Rinex_VersionV
        End Get
    End Property
    
    ReadOnly Property Date_File_Created() As String
        Get
            Return Date_File_CreatedV
        End Get
    End Property

    ReadOnly Property Obs_Type(ByVal index As Integer) As String
        Get
            Return Obs_TypeV(index)
        End Get
    End Property

    ReadOnly Property Interval() As Decimal
        Get
            Return Obs_IntervalV
        End Get
    End Property

    ReadOnly Property Leap_Seconds() As Integer
        Get
            Return Leap_SecondsV
        End Get
    End Property

    ReadOnly Property Sat_System() As String
        Get
            Return Sat_SystemV
        End Get
    End Property

    ReadOnly Property Dual_Frequency() As Boolean
        Get
            Return Dual_FrequencyV
        End Get
    End Property

    ReadOnly Property File_Loaded() As Boolean
        Get
            Return File_LoadedV
        End Get
    End Property

    ReadOnly Property NumEpochs() As Integer
        Get
            Return recordCount
        End Get
    End Property

    Public Sub New()
        Rinex_VersionV = String.Empty
        File_TypeV = String.Empty
        Sat_SystemV = String.Empty
        Date_File_CreatedV = String.Empty
        Marker_NameV = String.Empty
        Receiver_NumV = String.Empty
        Receiver_TypeV = String.Empty
        Receiver_VersionV = String.Empty
        Antenna_NumV = String.Empty
        Antenna_TypeV = String.Empty
        Approx_WGS84_X = -9999D
        Approx_WGS84_Y = -9999D
        Approx_WGS84_Z = -9999D
        Antenna_Offset_H = -9999D
        Antenna_Offset_E = -9999D
        Antenna_Offset_N = -9999D
        Num_Obs_TypeV = -9999I
        Obs_TypeV(0) = String.Empty
        Obs_TypeV(1) = String.Empty
        Obs_TypeV(2) = String.Empty
        Obs_TypeV(3) = String.Empty
        Obs_TypeV(4) = String.Empty
        Obs_TypeV(5) = String.Empty
        Obs_TypeV(6) = String.Empty
        Obs_TypeV(7) = String.Empty
        Obs_TypeV(8) = String.Empty
        Obs_TypeV(9) = String.Empty
        Obs_TypeV(10) = String.Empty
        Obs_TypeV(11) = String.Empty
        Obs_TypeV(12) = String.Empty
        Obs_TypeV(13) = String.Empty
        Obs_TypeV(14) = String.Empty
        Obs_TypeV(15) = String.Empty
        Obs_TypeV(16) = String.Empty
        Obs_TypeV(17) = String.Empty
        Obs_IntervalV = -9999D
        Leap_SecondsV = -9999I
        Obs_RecordsV(0, 0, 0) = -9999D
        Dual_FrequencyV = False
        File_LoadedV = False
        File_Path_And_NameV = ""
        recordCount = 0
    End Sub

    Public Sub Reset()
        Rinex_VersionV = String.Empty
        File_TypeV = String.Empty
        Sat_SystemV = String.Empty
        Date_File_CreatedV = String.Empty
        Marker_NameV = String.Empty
        Receiver_NumV = String.Empty
        Receiver_TypeV = String.Empty
        Receiver_VersionV = String.Empty
        Antenna_NumV = String.Empty
        Antenna_TypeV = String.Empty
        Approx_WGS84_X = -9999D
        Approx_WGS84_Y = -9999D
        Approx_WGS84_Z = -9999D
        Antenna_Offset_H = -9999D
        Antenna_Offset_E = -9999D
        Antenna_Offset_N = -9999D
        Num_Obs_TypeV = -9999I
        Obs_TypeV(0) = String.Empty
        Obs_TypeV(1) = String.Empty
        Obs_TypeV(2) = String.Empty
        Obs_TypeV(3) = String.Empty
        Obs_TypeV(4) = String.Empty
        Obs_TypeV(5) = String.Empty
        Obs_TypeV(6) = String.Empty
        Obs_TypeV(7) = String.Empty
        Obs_TypeV(8) = String.Empty
        Obs_TypeV(9) = String.Empty
        Obs_TypeV(10) = String.Empty
        Obs_TypeV(11) = String.Empty
        Obs_TypeV(12) = String.Empty
        Obs_TypeV(13) = String.Empty
        Obs_TypeV(14) = String.Empty
        Obs_TypeV(15) = String.Empty
        Obs_TypeV(16) = String.Empty
        Obs_TypeV(17) = String.Empty
        Obs_IntervalV = -9999D
        Leap_SecondsV = -9999I
        Dim New_Obs_Records(0, 0, 0) As Decimal
        New_Obs_Records(0, 0, 0) = -9999D
        Obs_RecordsV = New_Obs_Records
        Dual_FrequencyV = False
        File_LoadedV = False
        File_Path_And_NameV = ""
        recordCount = 0
    End Sub

    'sets one RINEX Obs file equal to another RINEX Obs file (but seperate objects)
    Public Overloads Function equals(ByVal obs1 As RINEX_Obs) As RINEX_Obs
        Me.Antenna_NumV = obs1.Antenna_NumV
        Me.Antenna_Offset_E = obs1.Antenna_Offset_E
        Me.Antenna_Offset_H = obs1.Antenna_Offset_H
        Me.Antenna_Offset_N = obs1.Antenna_Offset_N
        Me.Antenna_TypeV = obs1.Antenna_TypeV
        Me.Approx_WGS84_X = obs1.Approx_WGS84_X
        Me.Approx_WGS84_Y = obs1.Approx_WGS84_Y
        Me.Approx_WGS84_Z = obs1.Approx_WGS84_Z
        Me.Date_File_CreatedV = obs1.Date_File_Created
        Me.Dual_FrequencyV = obs1.Dual_FrequencyV
        Me.File_LoadedV = obs1.File_LoadedV
        Me.File_Path_And_NameV = obs1.File_Path_And_NameV
        Me.File_TypeV = obs1.File_TypeV
        Me.Leap_SecondsV = obs1.Leap_SecondsV
        Me.Marker_NameV = obs1.Marker_NameV
        Me.Num_Obs_TypeV = obs1.Num_Obs_TypeV
        Me.Obs_IntervalV = obs1.Obs_IntervalV

        For i As Integer = 0 To obs1.Obs_TypeV.GetLength(0) - 1
            Me.Obs_TypeV(i) = obs1.Obs_TypeV(i)
        Next

        Dim New_Obs_Records(obs1.Obs_RecordsV.GetLength(0) - 1, obs1.Obs_RecordsV.GetLength(1) - 1, obs1.Obs_RecordsV.GetLength(2) - 1) As Decimal
        Obs_RecordsV = New_Obs_Records

        For i As Integer = 0 To obs1.Obs_RecordsV.GetLength(0) - 1
            For j As Integer = 0 To obs1.Obs_RecordsV.GetLength(1) - 1
                For k As Integer = 0 To obs1.Obs_RecordsV.GetLength(2) - 1
                    Me.Obs_RecordsV(i, j, k) = obs1.Obs_RecordsV(i, j, k)
                Next
            Next
        Next

        Me.Receiver_NumV = obs1.Receiver_NumV
        Me.Receiver_TypeV = obs1.Receiver_TypeV
        Me.Receiver_VersionV = obs1.Receiver_VersionV
        Me.recordCount = obs1.recordCount
        Me.Rinex_VersionV = obs1.Rinex_VersionV
        Me.Sat_SystemV = obs1.Sat_SystemV
        Return Me
    End Function

    Public Sub ReadFile(ByVal FilePath As String, ByRef updateBar As ProgressBar, Optional ByVal SuppressMessage As Boolean = False, Optional ByVal SuppressProgress As Boolean = False)
        File_Path_And_NameV = FilePath
        Dim NumLinesInFile As Int64
        StandardizeRINEX(FilePath, NumLinesInFile)
        Dim FileStream As New StreamReader(FilePath)
        'Try
        Dim boolStop As Boolean = False
        Dim HeaderLine As String
        While FileStream.Peek <> -1 And Not boolStop
            HeaderLine = FileStream.ReadLine

            Select Case HeaderLine.Substring(60, 20).ToUpper
                Case "RINEX VERSION / TYPE"
                    Rinex_VersionV = HeaderLine.Substring(0, 9)
                    File_TypeV = HeaderLine.Substring(20, 1)
                    Sat_SystemV = HeaderLine.Substring(40, 1)
                Case "PGM / RUN BY / DATE "
                    Date_File_CreatedV = HeaderLine.Substring(40, 20)
                Case "COMMENT             "
                Case "MARKER NAME         "
                    Marker_NameV = HeaderLine.Substring(0, 60)
                Case "REC # / TYPE / VERS "
                    Receiver_NumV = HeaderLine.Substring(0, 20)
                    Receiver_TypeV = HeaderLine.Substring(20, 20)
                    Receiver_VersionV = HeaderLine.Substring(40, 20)
                Case "ANT # / TYPE        "
                    Antenna_NumV = HeaderLine.Substring(0, 20)
                    Antenna_TypeV = HeaderLine.Substring(20, 20)
                Case "APPROX POSITION XYZ "
                    Approx_WGS84_X = Convert.ToDecimal(HeaderLine.Substring(0, 14))
                    Approx_WGS84_Y = Convert.ToDecimal(HeaderLine.Substring(14, 14))
                    Approx_WGS84_Z = Convert.ToDecimal(HeaderLine.Substring(28, 14))
                Case "ANTENNA: DELTA H/E/N"
                    Antenna_Offset_H = Convert.ToDecimal(HeaderLine.Substring(0, 14))
                    Antenna_Offset_E = Convert.ToDecimal(HeaderLine.Substring(14, 14))
                    Antenna_Offset_N = Convert.ToDecimal(HeaderLine.Substring(28, 14))
                Case "# / TYPES OF OBSERV "
                    Num_Obs_TypeV = Convert.ToInt32(HeaderLine.Substring(0, 6))
                    Obs_TypeV(0) = HeaderLine.Substring(10, 2)
                    Obs_TypeV(1) = HeaderLine.Substring(16, 2)
                    Obs_TypeV(2) = HeaderLine.Substring(22, 2)
                    Obs_TypeV(3) = HeaderLine.Substring(28, 2)
                    Obs_TypeV(4) = HeaderLine.Substring(34, 2)
                    Obs_TypeV(5) = HeaderLine.Substring(40, 2)
                    Obs_TypeV(6) = HeaderLine.Substring(46, 2)
                    Obs_TypeV(7) = HeaderLine.Substring(52, 2)
                    Obs_TypeV(8) = HeaderLine.Substring(58, 2)
                    If Num_Obs_TypeV > 9 Then
                        HeaderLine = FileStream.ReadLine
                        Obs_TypeV(9) = HeaderLine.Substring(58, 2)
                        Obs_TypeV(10) = HeaderLine.Substring(10, 2)
                        Obs_TypeV(11) = HeaderLine.Substring(16, 2)
                        Obs_TypeV(12) = HeaderLine.Substring(22, 2)
                        Obs_TypeV(13) = HeaderLine.Substring(28, 2)
                        Obs_TypeV(14) = HeaderLine.Substring(34, 2)
                        Obs_TypeV(15) = HeaderLine.Substring(40, 2)
                        Obs_TypeV(16) = HeaderLine.Substring(46, 2)
                        Obs_TypeV(17) = HeaderLine.Substring(52, 2)
                    End If

                    'test for dual frequency data
                    Dim P1Test As Boolean = False
                    Dim P2Test As Boolean = False
                    For i As Integer = 0 To Num_Obs_TypeV - 1
                        If Obs_TypeV(i).ToUpper = "P1" Then
                            P1Test = True
                        ElseIf Obs_TypeV(i).ToUpper = "P2" Then
                            P2Test = True
                        End If
                    Next

                    If P1Test And P2Test Then
                        Dual_FrequencyV = True
                    End If

                Case "LEAP SECONDS        "
                    Leap_SecondsV = Convert.ToInt32(HeaderLine.Substring(0, 6))
                Case "INTERVAL            "
                    Obs_IntervalV = Convert.ToDecimal(HeaderLine.Substring(0, 10))
                Case "END OF HEADER       "
                    boolStop = True
            End Select
        End While

        If Sat_SystemV.ToUpper = "G" Or Sat_SystemV.ToUpper = "M" Then
            If Sat_SystemV = "M" Then
                If SuppressMessage = False Then
                    MessageBox.Show("The observation file contains data from other Global Navigation Satellite Systems (ex. GLONASS)." & ControlChars.NewLine & _
                "This other GNSS data will not be used within the solutions.", "Other GNSS data will not be used", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
            'Dim RecordCount As Integer = 0
            Dim Max_Sats_in_Epoch As Integer = 0
            Dim Sats_in_this_epoch As Integer = 0
            Dim ObsLine As String
            Dim i, j, k As Integer

            While FileStream.Peek <> -1
                If recordCount = 899 Then
                    Dim ryan As Integer = 1
                End If
                ObsLine = FileStream.ReadLine
                Dim testString As String = ObsLine.Trim()
                If testString.Length > 9 AndAlso ObsLine.Substring(60, 20).ToUpper <> "COMMENT             " Then
                    Sats_in_this_epoch = Convert.ToInt32(ObsLine.Substring(29, 3))
                    If Sats_in_this_epoch > Max_Sats_in_Epoch Then
                        Max_Sats_in_Epoch = Sats_in_this_epoch
                    End If
                    If Sats_in_this_epoch > 12 Then
                        ObsLine = FileStream.ReadLine
                    End If
                    For i = 1 To Sats_in_this_epoch
                        ObsLine = FileStream.ReadLine
                        If Num_Obs_TypeV > 5 Then
                            ObsLine = FileStream.ReadLine
                        End If
                        If Num_Obs_TypeV > 10 Then
                            ObsLine = FileStream.ReadLine
                        End If
                    Next
                    recordCount += 1
                End If
            End While

            Dim a, b, c As Integer
            a = Max_Sats_in_Epoch + 3
            b = Num_Obs_TypeV
            c = recordCount - 1

            Dim New_Obs_Records(a, b, c) As Decimal
            Obs_RecordsV = New_Obs_Records

            For i = 0 To a
                For j = 0 To b
                    For k = 0 To c
                        Obs_RecordsV(i, j, k) = -9999D
                    Next
                Next
            Next
            FileStream.Close()
            FileStream = New StreamReader(FilePath)

            boolStop = False
            While Not boolStop
                HeaderLine = FileStream.ReadLine
                HeaderLine = HeaderLine.Substring(60, 20).ToUpper
                If HeaderLine = "END OF HEADER       " Then
                    boolStop = True
                End If
            End While

            Dim GPSTime() As Decimal
            Dim GPSDate As String
            Dim hour, mins As Integer
            Dim secs As Decimal
            Dim SatLine As String
            Dim q As Integer = 1
            Dim Sat_Type As String
            Dim MultiLine As String

            recordCount = 0
            updateBar.Maximum = NumLinesInFile / 2 + 1
            While FileStream.Peek <> -1
                If (Not SuppressProgress) AndAlso updateBar.Value < updateBar.Maximum Then
                    updateBar.Value += 1
                    If updateBar.Value = updateBar.Maximum - 1 Then
                        updateBar.Refresh()
                    ElseIf updateBar.Value Mod 10 = 0 Then
                        updateBar.Refresh()
                    End If
                End If

                ObsLine = FileStream.ReadLine
                Dim testString As String = ObsLine.Trim()
                If testString.Length > 9 AndAlso ObsLine.Substring(60, 20).ToUpper <> "COMMENT             " Then
                    GPSDate = ObsLine.Substring(4, 2) & "/" & ObsLine.Substring(7, 2) & "/20" & ObsLine.Substring(1, 2)
                    hour = Convert.ToInt32(ObsLine.Substring(10, 2))
                    mins = Convert.ToInt32(ObsLine.Substring(13, 2))
                    secs = Convert.ToDecimal(ObsLine.Substring(16, 11))
                    GPSTime = ConvertCalendarTimetoGPSTime(GPSDate, hour, mins, secs)
                    'adds GPS times to observations
                    'GPS Week, Seconds (0 - 604800)
                    'GPS Month,Day
                    'GPS Year, Hours
                    'GPS Minutes, Seconds (0 - 60)
                    Obs_RecordsV(0, 0, recordCount) = GPSTime(0)
                    Obs_RecordsV(0, 1, recordCount) = GPSTime(1)
                    Obs_RecordsV(1, 0, recordCount) = Convert.ToDecimal(ObsLine.Substring(4, 2))
                    Obs_RecordsV(1, 1, recordCount) = Convert.ToDecimal(ObsLine.Substring(7, 2))
                    Obs_RecordsV(2, 0, recordCount) = Convert.ToDecimal("20" & ObsLine.Substring(1, 2))
                    Obs_RecordsV(2, 1, recordCount) = Convert.ToDecimal(hour)
                    Obs_RecordsV(3, 0, recordCount) = Convert.ToDecimal(mins)
                    Obs_RecordsV(3, 1, recordCount) = Decimal.Round(secs, 1)
                    Sats_in_this_epoch = Convert.ToInt32(ObsLine.Substring(29, 3))
                    If Sats_in_this_epoch > 12 Then
                        MultiLine = FileStream.ReadLine
                        MultiLine = MultiLine.Substring(32, 48)
                        ObsLine = ObsLine.Substring(0, 68)
                        ObsLine &= MultiLine
                    End If
                    For i = 4 To Sats_in_this_epoch + 3
                        Sat_Type = ObsLine.Substring(32 + (i - 4) * 3, 1)
                        If Sat_Type.ToUpper = "G" Or Sat_Type.ToUpper = " " Then
                            Obs_RecordsV(i, 0, recordCount) = Convert.ToDecimal(ObsLine.Substring(33 + (i - 4) * 3, 2))
                        End If
                        SatLine = FileStream.ReadLine

                        If (Not SuppressProgress) AndAlso updateBar.Value < updateBar.Maximum Then
                            updateBar.Value += 1
                            If updateBar.Value = updateBar.Maximum - 1 Then
                                updateBar.Refresh()
                            ElseIf updateBar.Value Mod 10 = 0 Then
                                updateBar.Refresh()
                            End If
                        End If

                        q = 1
                        For j = 1 To Num_Obs_TypeV
                            If j = 6 Then
                                SatLine = FileStream.ReadLine
                                q = 1
                            End If
                            If j = 11 Then
                                SatLine = FileStream.ReadLine
                                q = 1
                            End If
                            Try
                                Dim measurementString As String = SatLine.Substring(0 + (q - 1) * 16, 14)
                                If measurementString.Trim() <> String.Empty Then
                                    Obs_RecordsV(i, j, recordCount) = Convert.ToDecimal(measurementString)
                                End If
                            Catch
                            End Try
                            q += 1
                        Next
                    Next
                    recordCount += 1
                End If
            End While
            If Obs_IntervalV = -9999D Then
                Obs_IntervalV = Decimal.Round(Obs_RecordsV(0, 1, 1) - Obs_RecordsV(0, 1, 0), 3)
            End If
            File_LoadedV = True
        Else
            MessageBox.Show("RINEX Observation file is not GPS only or a MIXED data file, can not proceed", "RINEX Observation Data Problem", MessageBoxButtons.OK, MessageBoxIcon.Error)
            File_LoadedV = False
            File_Path_And_NameV = ""
        End If

        'Catch
        '    MessageBox.Show("RINEX Observation File is not in a standardized format, unable to read observation data from this file", "RINEX Observation Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    mainForm.ObsSuccess = False
        '    FileStream.Close()
        '    File_LoadedV = False
        '    File_Path_And_NameV = ""
        'End Try
        updateBar.Refresh()
    End Sub

    Public Function ConvertCalendarTimetoGPSTime(ByVal CalendarDate As String, ByVal Hours As Integer, ByVal Minutes As Integer, ByVal Seconds As Decimal) As Decimal()
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

        result(0) = gpsWeeksDecimalTrunc
        result(1) = gpsSecondsDecimal

        Return result
    End Function

    Private Function RINEXString2Decimal(ByVal RINEX_String As String) As Decimal
        Dim RINEX_String_NoSpaces As String = vbNullString
        Dim RINEX_String_Number As String = vbNullString
        Dim RINEX_Number As Decimal = 0D
        Dim RINEX_Exp As Integer = 0I

        Dim i As Integer
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

    End Function

    Private Sub StandardizeRINEX(ByVal FilePath As String, ByRef NumLines As Int64)
        Dim fileReader As New StreamReader(FilePath)
        Dim lineCount As Integer = 0
        Dim indivLine As String
        Dim badLines As Boolean = False

        While fileReader.Peek <> -1
            indivLine = fileReader.ReadLine
            If indivLine.Length < 80I And String.IsNullOrEmpty(indivLine) = False Then
                badLines = True
            End If
            lineCount += 1
        End While
        fileReader.Close()
        NumLines = lineCount

        If badLines = True Then
            Dim readLines(lineCount - 1) As String
            Dim i, j As Integer
            fileReader = New StreamReader(FilePath)

            j = 0
            While fileReader.Peek <> -1
                indivLine = fileReader.ReadLine
                For i = 1 To (80I - indivLine.Length)
                    indivLine &= " "
                Next
                readLines(j) = indivLine
                j += 1
            End While
            fileReader.Close()

            Dim fileWriter As New StreamWriter(FilePath, False)

            For i = 0 To lineCount - 1
                fileWriter.WriteLine(readLines(i))
            Next
            fileWriter.Close()
        End If
    End Sub

    'preprocessing procedure to determine the receiver clock offset at each epoch and then correct the pseudorange and carrier phase observations based on the receiver clock offset and Doppler observation
    Public Sub preProcessObs(ByVal stationName As String)

        Dim navFileName As String = File_Path_And_NameV.Substring(0, File_Path_And_NameV.Length - 1) & "n"
        If File.Exists(navFileName) Then

            Dim dummyprogressbar As New ProgressBar
            Dim selectedNavFile As New RINEX_Nav
            selectedNavFile.ReadFile(navFileName, dummyprogressbar)

            Dim i, j, k, m, z As Integer
            Dim SatNum As Integer
            Dim PRN As Integer
            Dim ReceptionTime(1), EphemerisTime(1) As Decimal
            Dim TimeDiffFromReference As Decimal
            Dim Eph2Use As New RINEX_Eph
            Dim pseudorange, L1, D1, transmitDistance As Decimal
            Dim pseudorangeIndex As Integer = -9999I
            Dim L1Index As Integer = -9999I
            Dim D1Index As Integer = -9999I
            Dim ECEF_XYZ_SatPosition(0 To 3) As Decimal
            Dim TempRINEX_Nav As New RINEX_Nav
            Dim f1 As Decimal = 1575420000
            Dim lambda1 As Decimal = 299792458 / f1

            Dim Sat_Clock_Corr, Rel_Corr, TGD_Corr As Decimal

            'locate the C/A code ("C1"), L1 phase ("L1"), and L1 Doppler ("D1") observables
            For m = 0 To 17
                If Me.Obs_Type(m).ToUpper = "C1" Then
                    pseudorangeIndex = m
                ElseIf Me.Obs_Type(m).ToUpper = "L1" Then
                    L1Index = m
                ElseIf Me.Obs_Type(m).ToUpper = "D1" Then
                    D1Index = m
                End If
            Next

            If pseudorangeIndex <> -9999I AndAlso L1Index <> -9999I AndAlso D1Index <> -9999I Then

                Dim tempXo As New Matrix(4, 1)
                tempXo.data(1, 1) = Me.Approx_WGS84_X
                tempXo.data(2, 1) = Me.Approx_WGS84_Y
                tempXo.data(3, 1) = Me.Approx_WGS84_Z

                For ne As Integer = 0 To recordCount - 1
                    Dim epochCount As Integer = 1
                    Dim newRecord As Integer = 1
                    Dim NumSatsInEpoch As Integer = 0

                    z = ne
                    ReceptionTime(0) = Me.ObsData(0, 0, z)
                    ReceptionTime(1) = Me.ObsData(0, 1, z)

                    Dim stopNum As Integer = Me.NumRows(z) - 1

                    For i = 4 To stopNum
                        SatNum = Me.ObsData(i, 0, z)
                        If SatNum <> -9999I Then
                            NumSatsInEpoch += 1
                        End If
                    Next

                    Dim SatPositions As New Matrix(NumSatsInEpoch, 5)

                    For i = 4 To stopNum
                        TimeDiffFromReference = 1000000000000.0
                        pseudorange = 0
                        transmitDistance = 0
                        SatNum = Me.ObsData(i, 0, z)
                        If SatNum <> -9999I Then
                            Eph2Use = New RINEX_Eph
                            For j = 0 To selectedNavFile.EphemerisV.GetLength(1) - 1
                                For k = 0 To selectedNavFile.EphemerisV.GetLength(0) - 1
                                    PRN = selectedNavFile.EphemerisV(k, j).PRN
                                    If SatNum = PRN Then
                                        EphemerisTime(0) = selectedNavFile.EphemerisV(k, j).Toe_Week
                                        EphemerisTime(1) = selectedNavFile.EphemerisV(k, j).Toe
                                        If Math.Abs((EphemerisTime(0) * 604800D + EphemerisTime(1)) - (ReceptionTime(0) * 604800D + ReceptionTime(1))) < TimeDiffFromReference Then
                                            TimeDiffFromReference = Math.Abs((EphemerisTime(0) * 604800D + EphemerisTime(1)) - (ReceptionTime(0) * 604800D + ReceptionTime(1)))
                                            Eph2Use = selectedNavFile.EphemerisV(k, j)
                                        End If
                                    End If
                                Next
                            Next

                            pseudorange = Me.ObsData(i, (pseudorangeIndex + 1), z)

                            If pseudorange <> -9999D And pseudorange <> 0D Then
                                If Eph2Use.PRN <> -9999I Then
                                    Sat_Clock_Corr = 0

                                    transmitDistance = pseudorange
                                    Dim delta As Decimal = 1000
                                    While delta > 0.1
                                        ECEF_XYZ_SatPosition = TempRINEX_Nav.ComputeSatellitePositionFromEphemeris(Sat_Clock_Corr, Rel_Corr, TGD_Corr, Eph2Use, ReceptionTime, transmitDistance, True, True, True, True)
                                        If ECEF_XYZ_SatPosition(0) = 0 Then 'good position returned
                                            Dim newRange As Decimal = Math.Sqrt((ECEF_XYZ_SatPosition(1) - tempXo.data(1, 1)) ^ 2 + (ECEF_XYZ_SatPosition(2) - tempXo.data(2, 1)) ^ 2 + (ECEF_XYZ_SatPosition(3) - tempXo.data(3, 1)) ^ 2)
                                            delta = Math.Abs(newRange - transmitDistance)
                                            transmitDistance = newRange
                                        Else
                                            Exit While
                                        End If
                                    End While

                                    If ECEF_XYZ_SatPosition(0) = 0 Then
                                        'SatPositions = SatPositions.matrixReDim(newRecord, 5, True)
                                        SatPositions.data(newRecord, 1) = ECEF_XYZ_SatPosition(1)
                                        SatPositions.data(newRecord, 2) = ECEF_XYZ_SatPosition(2)
                                        SatPositions.data(newRecord, 3) = ECEF_XYZ_SatPosition(3)

                                        Sat_Clock_Corr -= TGD_Corr
                                        Sat_Clock_Corr += Rel_Corr
                                        Sat_Clock_Corr *= 299792458
                                        SatPositions.data(newRecord, 4) = pseudorange
                                        SatPositions.data(newRecord, 5) = Sat_Clock_Corr
                                        newRecord += 1
                                    End If
                                End If
                            End If
                        End If
                        'epochCount += 1
                    Next

                    If SatPositions.nRows >= 4 Then
                        Dim A As New Matrix(SatPositions.nRows, 3 + epochCount)
                        Dim w As New Matrix(SatPositions.nRows, 1)
                        Dim L As New Matrix(SatPositions.nRows, 1)
                        Dim Xo As New Matrix(3 + epochCount, 1)
                        Dim N, u, d, v, NInverse As Matrix
                        Dim X As New Matrix(4, 1)
                        Xo.data(1, 1) = tempXo.data(1, 1)
                        Xo.data(2, 1) = tempXo.data(2, 1)
                        Xo.data(3, 1) = tempXo.data(3, 1)
                        Xo.data(4, 1) = tempXo.data(4, 1)
                        Dim stopCondition As Boolean = False
                        Dim threshold As Decimal = 0.001
                        Dim loopCounter As Integer = 1

                        While Not stopCondition
                            Dim range, FXo As Decimal
                            For i = 1 To SatPositions.nRows
                                range = Math.Sqrt((SatPositions.data(i, 1) - Xo.data(1, 1)) ^ 2 + (SatPositions.data(i, 2) - Xo.data(2, 1)) ^ 2 + (SatPositions.data(i, 3) - Xo.data(3, 1)) ^ 2)
                                A.data(i, 1) = -1D * ((SatPositions.data(i, 1) - Xo.data(1, 1)) / (range))
                                A.data(i, 2) = -1D * ((SatPositions.data(i, 2) - Xo.data(2, 1)) / (range))
                                A.data(i, 3) = -1D * ((SatPositions.data(i, 3) - Xo.data(3, 1)) / (range))
                                A.data(i, 4) = 1D
                                FXo = range - SatPositions.data(i, 5) + Xo.data(4, 1)
                                w.data(i, 1) = FXo - SatPositions.data(i, 4)
                            Next

                            N = A.Transpose * A
                            u = A.Transpose * w
                            NInverse = N.Inverse
                            d = -1 * (NInverse * u)
                            X = Xo + d
                            Xo = X
                            tempXo.equals(X)

                            If Math.Abs(d.data(1, 1)) <= threshold And Math.Abs(d.data(2, 1)) <= threshold And Math.Abs(d.data(3, 1)) <= threshold Then
                                stopCondition = True
                            ElseIf loopCounter = 10 Then
                                stopCondition = True
                            End If
                            loopCounter += 1
                        End While
                        X.data(4, 1) /= 299792458

                        'preprocessing adjustment of observations
                        For i = 4 To stopNum
                            SatNum = Me.ObsData(i, 0, z)
                            If SatNum <> -9999I Then
                                Obs_RecordsV(i, (L1Index + 1), z) -= (X.data(4, 1) * (f1 + Obs_RecordsV(i, (D1Index + 1), z)))
                                Obs_RecordsV(i, (pseudorangeIndex + 1), z) -= (X.data(4, 1) * lambda1 * (f1 + Obs_RecordsV(i, (D1Index + 1), z)))
                            End If
                        Next
                    End If
                Next
                'MessageBox.Show("DONE PREPROCESSING")
            Else
                MessageBox.Show("The required observations were not in the RINEX observation file for station: " & stationName & ", hence no preprocessing has been performed", "MISSING DATA", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            MessageBox.Show("Matching RINEX Navigation File for station: " & stationName & "does not exist, no preprocessing of observations was performed", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub
End Class
