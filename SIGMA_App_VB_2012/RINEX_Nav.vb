'''''SIGMA'''''
'By: Ryan Brazeal
'Date: April 2015
'GNU GPL V2 License
'www.rgbi.ca
'www.jrpi.ca

Imports System.IO

Public Class RINEX_Nav
    Private Rinex_VersionV As String
    Private File_TypeV As String
    Private Date_File_CreatedV As String
    Private Ion_A0V As Decimal
    Private Ion_A1V As Decimal
    Private Ion_A2V As Decimal
    Private Ion_A3V As Decimal
    Private Ion_B0V As Decimal
    Private Ion_B1V As Decimal
    Private Ion_B2V As Decimal
    Private Ion_B3V As Decimal
    Private UTC_A0V As Decimal
    Private UTC_A1V As Decimal
    Private UTC_TV As Integer
    Private UTC_WV As Integer
    Private Leap_SecondsV As Integer
    Public EphemerisV(0, 0) As RINEX_Eph
    Private NumSatellitesInFileV As Integer

    ReadOnly Property Rinex_Version() As String
        Get
            Return Rinex_VersionV
        End Get
    End Property
    ReadOnly Property File_Type() As String
        Get
            Return File_TypeV
        End Get
    End Property
    ReadOnly Property Date_File_Created() As String
        Get
            Return Date_File_CreatedV
        End Get
    End Property
    ReadOnly Property Ion_A0() As Decimal
        Get
            Return Ion_A0V
        End Get
    End Property
    ReadOnly Property Ion_A1() As Decimal
        Get
            Return Ion_A1V
        End Get
    End Property
    ReadOnly Property Ion_A2() As Decimal
        Get
            Return Ion_A2V
        End Get
    End Property
    ReadOnly Property Ion_A3() As Decimal
        Get
            Return Ion_A3V
        End Get
    End Property
    ReadOnly Property Ion_B0() As Decimal
        Get
            Return Ion_B0V
        End Get
    End Property
    ReadOnly Property Ion_B1() As Decimal
        Get
            Return Ion_B1V
        End Get
    End Property
    ReadOnly Property Ion_B2() As Decimal
        Get
            Return Ion_B2V
        End Get
    End Property
    ReadOnly Property Ion_B3() As Decimal
        Get
            Return Ion_B3V
        End Get
    End Property
    ReadOnly Property UTC_A0() As Decimal
        Get
            Return UTC_A0V
        End Get
    End Property
    ReadOnly Property UTC_A1() As Decimal
        Get
            Return UTC_A1V
        End Get
    End Property
    ReadOnly Property UTC_T() As Integer
        Get
            Return UTC_TV
        End Get
    End Property
    ReadOnly Property UTC_W() As Integer
        Get
            Return UTC_WV
        End Get
    End Property
    ReadOnly Property Num_Sats_in_File() As Integer
        Get
            Return NumSatellitesInFileV
        End Get
    End Property
    ReadOnly Property Leap_Seconds() As Integer
        Get
            Return Leap_SecondsV
        End Get
    End Property

    Public Sub New()
        Rinex_VersionV = String.Empty
        File_TypeV = String.Empty
        Date_File_CreatedV = String.Empty
        Ion_A0V = -9999D
        Ion_A1V = -9999D
        Ion_A2V = -9999D
        Ion_A3V = -9999D
        Ion_B0V = -9999D
        Ion_B1V = -9999D
        Ion_B2V = -9999D
        Ion_B3V = -9999D
        UTC_A0V = -9999D
        UTC_A1V = -9999D
        UTC_TV = -9999I
        UTC_WV = -9999I
        Leap_SecondsV = -9999I
        EphemerisV(0, 0) = New RINEX_Eph
        NumSatellitesInFileV = 0I
    End Sub

    Public Sub Reset()
        Rinex_VersionV = String.Empty
        File_TypeV = String.Empty
        Date_File_CreatedV = String.Empty
        Ion_A0V = -9999D
        Ion_A1V = -9999D
        Ion_A2V = -9999D
        Ion_A3V = -9999D
        Ion_B0V = -9999D
        Ion_B1V = -9999D
        Ion_B2V = -9999D
        Ion_B3V = -9999D
        UTC_A0V = -9999D
        UTC_A1V = -9999D
        UTC_TV = -9999I
        UTC_WV = -9999I
        Leap_SecondsV = -9999I
        Dim resetEphemeris(0, 0) As RINEX_Eph
        resetEphemeris(0, 0) = New RINEX_Eph
        EphemerisV = resetEphemeris
        NumSatellitesInFileV = 0I
    End Sub

    Private Function RINEXString2Decimal(ByVal RINEX_String As String) As Decimal
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

    Public Sub ReadFile(ByVal FilePath As String, ByRef updateBar As ProgressBar)
        Dim NumLinesInFile As Int64
        StandardizeRINEX(FilePath, NumLinesInFile)
        Dim FileStream As New StreamReader(FilePath)
        Try
            Dim boolStop As Boolean = False
            Dim HeaderLine As String
            While FileStream.Peek <> -1 And Not boolStop
                HeaderLine = FileStream.ReadLine

                Select Case HeaderLine.Substring(60, 20).ToUpper
                    Case "RINEX VERSION / TYPE"
                        Rinex_VersionV = HeaderLine.Substring(0, 9)
                        File_TypeV = HeaderLine.Substring(20, 1)
                    Case "PGM / RUN BY / DATE "
                        Date_File_CreatedV = HeaderLine.Substring(40, 20)
                    Case "COMMENT             "
                    Case "ION ALPHA           "
                        Ion_A0V = RINEXString2Decimal(HeaderLine.Substring(2, 12))
                        Ion_A1V = RINEXString2Decimal(HeaderLine.Substring(14, 12))
                        Ion_A2V = RINEXString2Decimal(HeaderLine.Substring(26, 12))
                        Ion_A3V = RINEXString2Decimal(HeaderLine.Substring(38, 12))
                    Case "ION BETA            "
                        Ion_B0V = RINEXString2Decimal(HeaderLine.Substring(2, 12))
                        Ion_B1V = RINEXString2Decimal(HeaderLine.Substring(14, 12))
                        Ion_B2V = RINEXString2Decimal(HeaderLine.Substring(26, 12))
                        Ion_B3V = RINEXString2Decimal(HeaderLine.Substring(38, 12))
                    Case "DELTA-UTC: A0,A1,T,W"
                        UTC_A0V = RINEXString2Decimal(HeaderLine.Substring(3, 19))
                        UTC_A1V = RINEXString2Decimal(HeaderLine.Substring(22, 19))
                        UTC_TV = Convert.ToInt32(HeaderLine.Substring(41, 9))
                        UTC_WV = Convert.ToInt32(HeaderLine.Substring(50, 9))
                    Case "LEAP SECONDS        "
                        Leap_SecondsV = Convert.ToInt32(HeaderLine.Substring(0, 6))
                    Case "END OF HEADER       "
                        boolStop = True
                End Select
            End While
            If File_TypeV = "N" Then
                Dim LineCount As Integer = 0
                Dim NavLine As String
                Dim cEphRow As Integer
                Dim cEphColumn As Integer
                Dim i, j, k As Integer

                updateBar.Maximum = NumLinesInFile + 1
                While FileStream.Peek <> -1
                    If updateBar.Value <> updateBar.Maximum Then
                        updateBar.Value += 1
                        updateBar.Update()
                    End If

                    NavLine = FileStream.ReadLine
                    LineCount += 1
                    If NavLine.Length = 41I Or NavLine.Length = 60I Or NavLine.Length >= 79I Then
                        Select Case LineCount
                            Case 1
                                If NumSatellitesInFileV = 0 Then
                                    cEphRow = 0I
                                    cEphColumn = 0I
                                    NumSatellitesInFileV += 1

                                    EphemerisV(cEphRow, cEphColumn).PRN = Convert.ToInt32(NavLine.Substring(0, 2))
                                    EphemerisV(cEphRow, cEphColumn).Toc_Year = Convert.ToInt32(NavLine.Substring(3, 2))
                                    EphemerisV(cEphRow, cEphColumn).Toc_Month = Convert.ToInt32(NavLine.Substring(6, 2))
                                    EphemerisV(cEphRow, cEphColumn).Toc_Day = Convert.ToInt32(NavLine.Substring(9, 2))
                                    EphemerisV(cEphRow, cEphColumn).Toc_Hour = Convert.ToInt32(NavLine.Substring(12, 2))
                                    EphemerisV(cEphRow, cEphColumn).Toc_Minute = Convert.ToInt32(NavLine.Substring(15, 2))
                                    EphemerisV(cEphRow, cEphColumn).Toc_Second = Convert.ToDecimal(NavLine.Substring(17, 5))
                                    EphemerisV(cEphRow, cEphColumn).Clock_Bias = RINEXString2Decimal(NavLine.Substring(22, 19))
                                    EphemerisV(cEphRow, cEphColumn).Clock_Drift = RINEXString2Decimal(NavLine.Substring(41, 19))
                                    EphemerisV(cEphRow, cEphColumn).Clock_DriftRate = RINEXString2Decimal(NavLine.Substring(60, 19))
                                Else
                                    Dim boolTester As Boolean = False
                                    For i = 0 To EphemerisV.GetLength(1) - 1
                                        If EphemerisV(0, i).PRN = Convert.ToInt32(NavLine.Substring(0, 2)) Then
                                            Dim NumEphRecords As Integer = 0
                                            For j = 0 To EphemerisV.GetLength(0) - 1
                                                If EphemerisV(j, i).PRN = Convert.ToInt32(NavLine.Substring(0, 2)) Then
                                                    NumEphRecords += 1
                                                End If
                                            Next
                                            If NumEphRecords = EphemerisV.GetLength(0) Then
                                                EphemerisV = My2DReDimPreserve(EphemerisV, EphemerisV.GetLength(0), EphemerisV.GetLength(1) - 1)
                                                EphemerisV(EphemerisV.GetLength(0) - 1, EphemerisV.GetLength(1) - 1) = New RINEX_Eph
                                                cEphRow = EphemerisV.GetLength(0) - 1
                                                cEphColumn = i
                                                boolTester = True
                                            Else
                                                cEphRow = NumEphRecords
                                                cEphColumn = i
                                                boolTester = True
                                            End If
                                        End If
                                    Next

                                    If boolTester = False Then
                                        If cEphRow = 0 Then
                                            EphemerisV = My2DReDimPreserve2(EphemerisV, EphemerisV.GetLength(0) - 1, EphemerisV.GetLength(1))
                                            cEphRow = 0
                                            cEphColumn = EphemerisV.GetLength(1) - 1
                                            NumSatellitesInFileV += 1
                                        Else
                                            EphemerisV = My2DReDimPreserve2(EphemerisV, EphemerisV.GetLength(0) - 1, EphemerisV.GetLength(1))
                                            cEphRow = 0
                                            cEphColumn = EphemerisV.GetLength(1) - 1
                                            NumSatellitesInFileV += 1
                                        End If
                                    End If

                                    EphemerisV(cEphRow, cEphColumn).PRN = Convert.ToInt32(NavLine.Substring(0, 2))
                                    EphemerisV(cEphRow, cEphColumn).Toc_Year = Convert.ToInt32(NavLine.Substring(3, 2))
                                    EphemerisV(cEphRow, cEphColumn).Toc_Month = Convert.ToInt32(NavLine.Substring(6, 2))
                                    EphemerisV(cEphRow, cEphColumn).Toc_Day = Convert.ToInt32(NavLine.Substring(9, 2))
                                    EphemerisV(cEphRow, cEphColumn).Toc_Hour = Convert.ToInt32(NavLine.Substring(12, 2))
                                    EphemerisV(cEphRow, cEphColumn).Toc_Minute = Convert.ToInt32(NavLine.Substring(15, 2))
                                    EphemerisV(cEphRow, cEphColumn).Toc_Second = Convert.ToDecimal(NavLine.Substring(17, 5))
                                    EphemerisV(cEphRow, cEphColumn).Clock_Bias = RINEXString2Decimal(NavLine.Substring(22, 19))
                                    EphemerisV(cEphRow, cEphColumn).Clock_Drift = RINEXString2Decimal(NavLine.Substring(41, 19))
                                    EphemerisV(cEphRow, cEphColumn).Clock_DriftRate = RINEXString2Decimal(NavLine.Substring(60, 19))
                                End If
                            Case 2
                                EphemerisV(cEphRow, cEphColumn).IODE = RINEXString2Decimal(NavLine.Substring(3, 19))
                                EphemerisV(cEphRow, cEphColumn).Crs = RINEXString2Decimal(NavLine.Substring(22, 19))
                                EphemerisV(cEphRow, cEphColumn).Delta_n = RINEXString2Decimal(NavLine.Substring(41, 19))
                                EphemerisV(cEphRow, cEphColumn).M0 = RINEXString2Decimal(NavLine.Substring(60, 19))
                            Case 3
                                EphemerisV(cEphRow, cEphColumn).Cuc = RINEXString2Decimal(NavLine.Substring(3, 19))
                                EphemerisV(cEphRow, cEphColumn).e = RINEXString2Decimal(NavLine.Substring(22, 19))
                                EphemerisV(cEphRow, cEphColumn).Cus = RINEXString2Decimal(NavLine.Substring(41, 19))
                                EphemerisV(cEphRow, cEphColumn).a = Math.Pow(RINEXString2Decimal(NavLine.Substring(60, 19)), 2)
                            Case 4
                                EphemerisV(cEphRow, cEphColumn).Toe = RINEXString2Decimal(NavLine.Substring(3, 19))
                                EphemerisV(cEphRow, cEphColumn).Cic = RINEXString2Decimal(NavLine.Substring(22, 19))
                                EphemerisV(cEphRow, cEphColumn).Big_Omega = RINEXString2Decimal(NavLine.Substring(41, 19))
                                EphemerisV(cEphRow, cEphColumn).Cis = RINEXString2Decimal(NavLine.Substring(60, 19))
                            Case 5
                                EphemerisV(cEphRow, cEphColumn).i0 = RINEXString2Decimal(NavLine.Substring(3, 19))
                                EphemerisV(cEphRow, cEphColumn).Crc = RINEXString2Decimal(NavLine.Substring(22, 19))
                                EphemerisV(cEphRow, cEphColumn).Little_Omega = RINEXString2Decimal(NavLine.Substring(41, 19))
                                EphemerisV(cEphRow, cEphColumn).Omega_DOT = RINEXString2Decimal(NavLine.Substring(60, 19))
                            Case 6
                                EphemerisV(cEphRow, cEphColumn).i_DOT = RINEXString2Decimal(NavLine.Substring(3, 19))
                                EphemerisV(cEphRow, cEphColumn).L2_Codes = RINEXString2Decimal(NavLine.Substring(22, 19))
                                EphemerisV(cEphRow, cEphColumn).Toe_Week = RINEXString2Decimal(NavLine.Substring(41, 19))
                                EphemerisV(cEphRow, cEphColumn).L2_P_Flag = RINEXString2Decimal(NavLine.Substring(60, 19))
                            Case 7
                                EphemerisV(cEphRow, cEphColumn).SV_accuracy = RINEXString2Decimal(NavLine.Substring(3, 19))
                                EphemerisV(cEphRow, cEphColumn).SV_health = RINEXString2Decimal(NavLine.Substring(22, 19))
                                EphemerisV(cEphRow, cEphColumn).TGD = RINEXString2Decimal(NavLine.Substring(41, 19))
                                EphemerisV(cEphRow, cEphColumn).IODC = RINEXString2Decimal(NavLine.Substring(60, 19))
                            Case 8
                                EphemerisV(cEphRow, cEphColumn).Transmission_of_Message = RINEXString2Decimal(NavLine.Substring(3, 19))
                                EphemerisV(cEphRow, cEphColumn).Fit_Interval = RINEXString2Decimal(NavLine.Substring(22, 19))
                                EphemerisV(cEphRow, cEphColumn).Spare1 = RINEXString2Decimal(NavLine.Substring(41, 19))
                                EphemerisV(cEphRow, cEphColumn).Spare2 = RINEXString2Decimal(NavLine.Substring(60, 19))
                                LineCount = 0
                        End Select
                    End If
                End While

                'remove any duplicated ephemeris records based on Time of Ephemeris (Week and seconds)
                Dim baseWeeks As Integer
                Dim baseSeconds As Decimal
                For j = 0 To EphemerisV.GetLength(1) - 1
                    For i = 0 To EphemerisV.GetLength(0) - 2
                        baseWeeks = EphemerisV(i, j).Toe_Week
                        baseSeconds = EphemerisV(i, j).Toe
                        For k = i + 1 To EphemerisV.GetLength(0) - 1
                            If EphemerisV(k, j).Toe_Week = baseWeeks And EphemerisV(k, j).Toe = baseSeconds Then
                                EphemerisV(k, j) = New RINEX_Eph
                            End If
                        Next
                    Next
                Next
            Else
                MessageBox.Show("The RINEX Navigation File was not for NAVSTAR GPS, it was for some other Global Navigation Satellite System and can not be used", "Incorrect RINEX Navigation File type", MessageBoxButtons.OK, MessageBoxIcon.Error)
                mainForm.NavSuccess = False
            End If
        Catch
            MessageBox.Show("RINEX Navigation File is not in a standardized format, unable to read navigation data from this file", "RINEX Navigation Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            mainForm.NavSuccess = False
            FileStream.Close()
        End Try

    End Sub

    Private Function My2DReDimPreserve(ByVal Eph(,) As RINEX_Eph, ByVal newRowIndex As Integer, ByVal newColumnIndex As Integer) As RINEX_Eph(,)
        Dim newEphV(newRowIndex, newColumnIndex) As RINEX_Eph
        Dim i, j As Integer

        For i = 0 To newRowIndex
            For j = 0 To newColumnIndex
                newEphV(i, j) = New RINEX_Eph
                If i < newRowIndex Then
                    newEphV(i, j) = Eph(i, j)
                End If
            Next
        Next
        Return newEphV
    End Function

    Private Function My2DReDimPreserve2(ByVal Eph(,) As RINEX_Eph, ByVal newRowIndex As Integer, ByVal newColumnIndex As Integer) As RINEX_Eph(,)
        Dim newEphV(newRowIndex, newColumnIndex) As RINEX_Eph
        Dim i, j As Integer

        For i = 0 To newRowIndex
            For j = 0 To newColumnIndex
                newEphV(i, j) = New RINEX_Eph
                If j < newColumnIndex Then
                    If Eph(i, j).PRN <> -9999I Then
                        newEphV(i, j) = Eph(i, j)
                    End If
                End If
            Next
        Next
        Return newEphV
    End Function

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

    Public Function ComputeSatellitePositionFromEphemeris(ByRef Clock_Corr As Decimal, ByRef Rel_Corr As Decimal, ByRef TGD_Corr As Decimal, ByRef eph As RINEX_Eph, ByRef receptionTime() As Decimal, ByRef pseudorange As Decimal, ByRef corr_Rot As Boolean, ByRef corr_Sat_Clock As Boolean, ByRef corr_Rel As Boolean, ByRef corr_TGD As Boolean) As Decimal()
        Dim XYZresult(0 To 3) As Decimal
        '(0) is either 0 = solution solved, or 1 = solution not solved or stopped
        '(1) is satellite X coordinate
        '(2) is satellite Y coordinate
        '(3) is satellite Z coordinate

        Dim transmitTime(0 To 1) As Decimal
        'pseudorange = 22774387.506
        corr_Rot = False
        Dim transitTime As Decimal = pseudorange / mainForm.SPEED_LIGHT
        transmitTime(0) = receptionTime(0)
        transmitTime(1) = receptionTime(1) - transitTime

        If transmitTime(1) < 0D Then
            transmitTime(1) += 604800D
            transmitTime(0) -= 1
        End If

        If transmitTime(1) > 604800D Then
            transmitTime(1) -= 604800D
            transmitTime(0) += 1
        End If

        Dim totalTransmitTime As Decimal = transmitTime(0) * 604800D + transmitTime(1)
        Dim timeSinceReferenceEph As Decimal = totalTransmitTime - (eph.Toe_Week * 604800D + eph.Toe)

        Dim calendarTOC As String
        Dim calendarTOCyear As String = eph.Toc_Year

        If calendarTOCyear.Length = 1 Then
            calendarTOCyear = "200" & calendarTOCyear
        End If

        If calendarTOCyear.Length = 2 Then
            calendarTOCyear = "20" & calendarTOCyear
        End If

        If calendarTOCyear.Length = 3 Then
            calendarTOCyear = "2" & calendarTOCyear
        End If

        calendarTOC = eph.Toc_Month & "/" & eph.Toc_Day & "/" & calendarTOCyear

        Dim TOCresult(0 To 1) As Decimal
        Dim temp As Decimal = 0
        TOCresult = ConvertCalendarTimetoGPSTime(calendarTOC, Convert.ToInt32(eph.Toc_Hour), Convert.ToInt32(eph.Toc_Minute), eph.Toc_Second)
        Dim timeSinceReferenceClock As Decimal = totalTransmitTime - (TOCresult(0) * 604800D + TOCresult(1))
        Dim clockCorrection As Decimal = eph.Clock_Bias + eph.Clock_Drift * timeSinceReferenceClock + eph.Clock_DriftRate * Math.Pow(timeSinceReferenceClock, 2)
        'temp = timeSinceReferenceClock - clockCorrection
        'clockCorrection = eph.Clock_Bias + eph.Clock_Drift * temp + eph.Clock_DriftRate * Math.Pow(temp, 2)
        'timeSinceReferenceClock -= clockCorrection
        'timeSinceReferenceEph -= clockCorrection

        Dim testBool1, testBool2 As Boolean
        testBool1 = False
        testBool2 = False

        If timeSinceReferenceEph > 302400D Or timeSinceReferenceEph < -302400D Then
            testBool1 = True
        End If

        If timeSinceReferenceClock > 302400D Or timeSinceReferenceClock < -302400D Then
            testBool2 = True
        End If

        Dim YesNoResult As DialogResult = DialogResult.Yes
        If testBool1 And testBool2 Then
            YesNoResult = MessageBox.Show("The specified Reception Time is NOT within 1 week of the choosen ephemeris reference time and clock reference time!" & _
            ControlChars.NewLine & ControlChars.NewLine & "The calculated results will not be as accurate. Do you want to continue?", "Time Since Reference Ephemeris and Clock Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)


        ElseIf testBool1 And Not testBool2 Then
            YesNoResult = MessageBox.Show("The specified Reception Time is NOT within 1 week of the choosen ephemeris reference time!" & _
                        ControlChars.NewLine & ControlChars.NewLine & "The calculated results will not be as accurate. Do you want to continue?", "Time Since Reference Ephemeris Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)


        ElseIf Not testBool1 And testBool2 Then
            YesNoResult = MessageBox.Show("The specified Reception Time is NOT within 1 week of the choosen clock reference time!" & _
                        ControlChars.NewLine & ControlChars.NewLine & "The calculated results will not be as accurate. Do you want to continue?", "Time Since Reference Clock Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        End If

        If YesNoResult = DialogResult.Yes Then

            Dim n = Math.Sqrt(mainForm.EARTH_U / Math.Pow(eph.a, 3D))
            n += eph.Delta_n

            Dim M = eph.M0 + n * timeSinceReferenceEph

            Dim diffM As Decimal = 1D
            Dim E As Decimal = M
            Dim Mi As Decimal = 0
            Dim counter As Integer = 0

            While Math.Abs(diffM) > 0.000000000001
                Mi = E - eph.e * Math.Sin(E)
                diffM = M - Mi
                E = E + diffM

                If counter = 20I Then
                    MessageBox.Show("Could not converge upon a value for the Eccentric Anomaly after 20 iterations" & ControlChars.NewLine & _
                    "The difference between iterations 19 and 20 was: " & diffM.ToString, "Convergence Criteria not met")
                    Exit While
                End If

                counter += 1
            End While

            Dim relCorrection As Decimal = mainForm.RELATIVISTIC * eph.e * Math.Sqrt(eph.a) * Math.Sin(E)

            'referenced returns
            Clock_Corr = clockCorrection
            Rel_Corr = relCorrection
            TGD_Corr = eph.TGD

            If corr_Sat_Clock = False Then
                clockCorrection = 0
            End If

            If corr_Rel = True Then
                clockCorrection += relCorrection
            End If

            If corr_TGD = True Then
                clockCorrection -= eph.TGD
            End If

            transmitTime(1) -= clockCorrection

            If transmitTime(1) < 0D Then
                transmitTime(1) += 604800D
                transmitTime(0) -= 1
            End If

            If transmitTime(1) > 604800D Then
                transmitTime(1) -= 604800D
                transmitTime(0) += 1
            End If

            'Dim i As Integer
            'For i = 1 To 2
            totalTransmitTime = transmitTime(0) * 604800D + transmitTime(1)
            timeSinceReferenceEph = totalTransmitTime - (eph.Toe_Week * 604800D + eph.Toe)

            M = eph.M0 + n * timeSinceReferenceEph

            diffM = 1D
            E = M
            Mi = 0

            counter = 0
            While Math.Abs(diffM) > 0.000000000001
                Mi = E - eph.e * Math.Sin(E)
                diffM = M - Mi
                E = E + diffM

                If counter = 20I Then
                    Exit While
                End If

                counter += 1
            End While

            Dim fk As Decimal = Math.Atan2((Math.Sqrt((1 - Math.Pow(eph.e, 2))) * Math.Sin(E)), (Math.Cos(E) - eph.e))
            Dim PHI_k As Decimal = eph.Little_Omega + fk

            Dim du As Decimal = eph.Cuc * Math.Cos(2D * PHI_k) + eph.Cus * Math.Sin(2D * PHI_k)
            Dim dr As Decimal = eph.Crc * Math.Cos(2D * PHI_k) + eph.Crs * Math.Sin(2D * PHI_k)
            Dim di As Decimal = eph.Cic * Math.Cos(2D * PHI_k) + eph.Cis * Math.Sin(2D * PHI_k)

            Dim uk As Decimal = PHI_k + du
            Dim rk As Decimal = eph.a * (1 - eph.e * Math.Cos(E)) + dr
            Dim ik As Decimal = eph.i0 + eph.i_DOT * timeSinceReferenceEph + di

            Dim OMEGA_k As Decimal

            If corr_Rot = True Then
                OMEGA_k = eph.Big_Omega + (eph.Omega_DOT - mainForm.EARTH_ROT) * timeSinceReferenceEph - mainForm.EARTH_ROT * (eph.Toe + transitTime)
            Else
                OMEGA_k = eph.Big_Omega + (eph.Omega_DOT - mainForm.EARTH_ROT) * timeSinceReferenceEph - mainForm.EARTH_ROT * (eph.Toe)
            End If

            Dim x_comp As Decimal = rk * Math.Cos(uk)
            Dim y_comp As Decimal = rk * Math.Sin(uk)
            Dim cosOMEGA_k As Decimal = Math.Cos(OMEGA_k)
            Dim sinOMEGA_k As Decimal = Math.Sin(OMEGA_k)
            Dim cosik As Decimal = Math.Cos(ik)
            Dim sinik As Decimal = Math.Sin(ik)

            If CalcSatXYZForm.outputParametersCheckBox.Checked Then
                MessageBox.Show("Transit Time (delta t):  " & transitTime & ControlChars.NewLine & " s" & _
                "Transmit Time (t):  " & transmitTime(1) & " s" & ControlChars.NewLine & _
                "Time since Ephemeris (tk):  " & timeSinceReferenceEph & " " & ControlChars.NewLine & _
                "Corrected Mean Motion (n):  " & n & " " & ControlChars.NewLine & _
                "Mean Anomaly (Mk):  " & M & " " & ControlChars.NewLine & _
                "Eccentric Anomaly (Ek):  " & E & " " & ControlChars.NewLine & _
                "True Anomaly (fk):  " & fk & " " & ControlChars.NewLine & _
                "Argument of Latitude (PHIk):  " & PHI_k & " " & ControlChars.NewLine & _
                "Latitude Correction (duk):  " & du & " " & ControlChars.NewLine & _
                "Radius Correction (drk):  " & dr & " " & ControlChars.NewLine & _
                "Inclination Correction (dik):  " & di & " " & ControlChars.NewLine & _
                "Corrected Argument of Latitude (uk):  " & uk & " " & ControlChars.NewLine & _
                "Corrected Radius (rk):  " & rk & " " & ControlChars.NewLine & _
                "Corrected Inclination (ik):  " & ik & " " & ControlChars.NewLine & _
                "Corrected Longitude of Ascending Node (OMEGAk):  " & OMEGA_k & " " & ControlChars.NewLine, "Solved Calculation Parameters")
            End If

            XYZresult(0) = 0
            XYZresult(1) = x_comp * cosOMEGA_k - y_comp * cosik * sinOMEGA_k
            XYZresult(2) = x_comp * sinOMEGA_k + y_comp * cosik * cosOMEGA_k
            XYZresult(3) = y_comp * sinik
            transitTime = Math.Sqrt((XYZresult(1) - mainForm.SelectedObsFile.Approx_X) ^ 2 + (XYZresult(2) - mainForm.SelectedObsFile.Approx_Y) ^ 2 + (XYZresult(3) - mainForm.SelectedObsFile.Approx_Z) ^ 2)
            'Next
        Else
            XYZresult(0) = 1
            XYZresult(1) = 0
            XYZresult(2) = 0
            XYZresult(3) = 0
        End If
        Return XYZresult
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

End Class
