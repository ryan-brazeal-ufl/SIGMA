'''''SIGMA'''''
'By: Ryan Brazeal
'Date: April 2015
'GNU GPL V2 License
'www.rgbi.ca
'www.jrpi.ca

Public Class CalcSatXYZForm

    Public CurrentEphRecord As New RINEX_Eph

    Private Sub calendarRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles calendarRadioButton.CheckedChanged
        calendarDateTimePicker.Enabled = calendarRadioButton.Checked
        gpsH.Enabled = calendarRadioButton.Checked
        gpsM.Enabled = calendarRadioButton.Checked
        gpsS.Enabled = calendarRadioButton.Checked
        gpsW.Enabled = gpsTimeRadioButton.Checked
        gpsWS.Enabled = gpsTimeRadioButton.Checked
        Label1.Enabled = calendarRadioButton.Checked
        Label2.Enabled = calendarRadioButton.Checked
        Label3.Enabled = calendarRadioButton.Checked
        Label4.Enabled = gpsTimeRadioButton.Checked
        Label5.Enabled = gpsTimeRadioButton.Checked
    End Sub

    Private Sub gpsW_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles gpsW.TextChanged
        If gpsW.Text.Length > 0 Then
            Dim testString As String = gpsW.Text.Substring(gpsW.Text.Length - 1)
            If testString = "0" Or testString = "1" Or testString = "2" Or testString = "3" Or testString = "4" Or testString = "5" Or testString = "6" Or testString = "7" Or testString = "8" Or testString = "9" Then
            Else
                gpsW.Text = String.Empty
            End If
        End If
    End Sub

    Private Sub gpsWS_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles gpsWS.TextChanged
        If gpsWS.Text.Length > 0 Then
            Dim testString As String = gpsWS.Text.Substring(gpsWS.Text.Length - 1)
            If testString = "0" Or testString = "1" Or testString = "2" Or testString = "3" Or testString = "4" Or testString = "5" Or testString = "6" Or testString = "7" Or testString = "8" Or testString = "9" Then
            Else
                gpsWS.Text = String.Empty
            End If
            If gpsWS.Text <> String.Empty Then
                Dim secondsInt As Integer = Convert.ToInt32(gpsWS.Text)
                If secondsInt >= 0 And secondsInt <= 603799 Then
                Else
                    gpsWS.Text = gpsWS.Text.Substring(0, gpsWS.Text.Length - 1)
                    gpsWS.SelectionStart = gpsWS.Text.Length
                End If
            End If
        End If
    End Sub

    Private Sub ReceptionTimeRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReceptionTimeRadioButton.CheckedChanged
        Label6.Enabled = TransmitTimeRadioButton.Checked
        c1TextBox.Enabled = TransmitTimeRadioButton.Checked
        nominalDTCheckBox.Enabled = TransmitTimeRadioButton.Checked
        earthRotationCheckBox.Enabled = TransmitTimeRadioButton.Checked
    End Sub

    Private Sub c1TextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles c1TextBox.TextChanged
        If IsNumeric(c1TextBox.Text) = False Then
            c1TextBox.Text = String.Empty
        End If
    End Sub

    Private Sub solveXYZButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles solveXYZButton.Click
        Dim GPSTime(0 To 1) As Decimal
        Dim c1Decimal As Decimal = 0D

        Dim testBool1, testBool2 As Boolean
        testBool1 = False
        If calendarRadioButton.Checked = True Then
            Dim TempRINEX_Nav As New RINEX_Nav
            GPSTime = TempRINEX_Nav.ConvertCalendarTimetoGPSTime(calendarDateTimePicker.Text, gpsH.Value, gpsM.Value, gpsS.Value)
            testBool1 = True
        Else
            If gpsW.Text <> String.Empty Then
                GPSTime(0) = Convert.ToDecimal(gpsW.Text)
                If gpsWS.Text <> String.Empty Then
                    GPSTime(1) = Convert.ToDecimal(gpsWS.Text)
                    testBool1 = True
                Else
                    MessageBox.Show("Missing the GPS Seconds value for the Reception Time", "Missing Input")
                End If
            Else
                MessageBox.Show("Missing the GPS Weeks value for the Reception Time", "Missing Input")
            End If
        End If

        testBool2 = False
        If ReceptionTimeRadioButton.Checked Then
            c1Decimal = 0D
            testBool2 = True
        Else
            If nominalDTCheckBox.Checked = False Then
                If c1TextBox.Text <> String.Empty Then
                    c1Decimal = Convert.ToDecimal(c1TextBox.Text)
                    testBool2 = True
                Else
                    Dim YesNoResult As DialogResult
                    YesNoResult = MessageBox.Show("You have not entered a value for the observed pseudorange, hence no transit time can be calculated" & ControlChars.NewLine & _
                    "The satellite's position will be calculated at reception time as a result, and no earth rotation correction can be applied (if selected)." & ControlChars.NewLine & ControlChars.NewLine & _
                    "Do you want to continue?", "Transit time Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

                    If YesNoResult = Windows.Forms.DialogResult.Yes Then
                        c1Decimal = 0D
                        testBool2 = True
                    End If
                End If
            Else
                c1Decimal = 0.074D * mainForm.SPEED_LIGHT
                testBool2 = True
            End If
        End If

        If testBool1 And testBool2 Then
            Dim ECEF_XYZ_SatPosition(0 To 3) As Decimal
            Dim TempRINEX_Nav As New RINEX_Nav

            ECEF_XYZ_SatPosition = TempRINEX_Nav.ComputeSatellitePositionFromEphemeris(1D, 1D, 1D, CurrentEphRecord, GPSTime, c1Decimal, earthRotationCheckBox.Checked, SatelliteClockCheckBox.Checked, True, True)

            If ECEF_XYZ_SatPosition(0) = 0 Then
                MessageBox.Show("Earth Centred - Earth Fixed (ECEF) Coordinates for Satellite PRN # " & CurrentEphRecord.PRN & ControlChars.NewLine & _
                "at the GPS Time of " & GPSTime(0).ToString & " Weeks, " & Decimal.Round(GPSTime(1)) & " Seconds" & ControlChars.NewLine & ControlChars.NewLine & _
                "X: " & Decimal.Round(ECEF_XYZ_SatPosition(1), 6I) & " m" & ControlChars.NewLine & _
                "Y: " & Decimal.Round(ECEF_XYZ_SatPosition(2), 6I) & " m" & ControlChars.NewLine & _
                "Z: " & Decimal.Round(ECEF_XYZ_SatPosition(3), 6I) & " m" & ControlChars.NewLine, "WGS84 Satellite Position")
            End If
        End If
    End Sub

    Private Sub gpsH_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles gpsH.Leave
        If gpsH.Text = String.Empty Then
            gpsH.Text = "0"
        End If
    End Sub

    Private Sub gpsM_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles gpsM.Leave
        If gpsM.Text = String.Empty Then
            gpsM.Text = 0D
        End If
    End Sub

    Private Sub gpsS_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles gpsS.Leave
        If gpsS.Text = String.Empty Then
            gpsS.Text = 0D
        End If
    End Sub

    Private Sub nominalDTCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nominalDTCheckBox.CheckedChanged
        Label6.Enabled = Not nominalDTCheckBox.Checked
        c1TextBox.Enabled = Not nominalDTCheckBox.Checked
    End Sub

    Private Sub TransmitTimeRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TransmitTimeRadioButton.CheckedChanged
        nominalDTCheckBox.Checked = False
    End Sub

    Private Sub CalcSatXYZForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        calendarDateTimePicker.Value = Date.Today
    End Sub
End Class