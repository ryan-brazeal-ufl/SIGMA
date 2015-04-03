'''''SIGMA'''''
'By: Ryan Brazeal
'Date: April 2015
'GNU GPL V2 License
'www.rgbi.ca
'www.jrpi.ca

Public Class SkyPlot
    Friend satData As Matrix
    Friend ONEorALL As Integer

    Friend Sub SkyPlot_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        Dim blackPen As New Pen(Color.Black, 1)
        Dim maskPen As New Pen(Color.Blue, 5)
        Dim satPen As New Pen(Color.Red, 2)
        Dim SatPen2 As New Pen(Color.Red, 15)
        Dim textFont As New Font("arial", 20, FontStyle.Regular, GraphicsUnit.Pixel)
        Dim elevFont As New Font("arial", 12, FontStyle.Regular, GraphicsUnit.Pixel)
        Dim backGroundBrush As New SolidBrush(Me.BackColor)

        'draw elevation mask
        Dim maskX, maskW As Integer
        maskX = 27 + satData.data(1, 1) * 2.7
        maskW = 486 - satData.data(1, 1) * 5.4

        If maskW = 0 Then
            maskW = 1
        End If

        e.Graphics.FillEllipse(Brushes.LightBlue, 27, 27, 486, 486)
        e.Graphics.FillEllipse(backGroundBrush, maskX, maskX, maskW, maskW)
        e.Graphics.DrawEllipse(maskPen, maskX, maskX, maskW, maskW)

        'draw on elevation circles 90 to 0
        e.Graphics.DrawEllipse(blackPen, 27, 27, 486, 486)
        e.Graphics.DrawEllipse(blackPen, 54, 54, 432, 432)
        e.Graphics.DrawEllipse(blackPen, 81, 81, 378, 378)
        e.Graphics.DrawEllipse(blackPen, 108, 108, 324, 324)
        e.Graphics.DrawEllipse(blackPen, 135, 135, 270, 270)
        e.Graphics.DrawEllipse(blackPen, 162, 162, 216, 216)
        e.Graphics.DrawEllipse(blackPen, 189, 189, 162, 162)
        e.Graphics.DrawEllipse(blackPen, 216, 216, 108, 108)
        e.Graphics.DrawEllipse(blackPen, 243, 243, 54, 54)
        e.Graphics.DrawEllipse(blackPen, 270, 270, 1, 1)

        'draw on dashed azimuth lines
        e.Graphics.DrawLine(blackPen, 270, 279, 270, 261)
        e.Graphics.DrawLine(blackPen, 270, 243, 270, 225)
        e.Graphics.DrawLine(blackPen, 270, 207, 270, 189)
        e.Graphics.DrawLine(blackPen, 270, 171, 270, 153)
        e.Graphics.DrawLine(blackPen, 270, 135, 270, 117)
        e.Graphics.DrawLine(blackPen, 270, 99, 270, 81)
        e.Graphics.DrawLine(blackPen, 270, 63, 270, 45)
        e.Graphics.DrawLine(blackPen, 270, 27, 270, 20)
        e.Graphics.DrawLine(blackPen, 270, 297, 270, 315)
        e.Graphics.DrawLine(blackPen, 270, 333, 270, 351)
        e.Graphics.DrawLine(blackPen, 270, 369, 270, 387)
        e.Graphics.DrawLine(blackPen, 270, 405, 270, 423)
        e.Graphics.DrawLine(blackPen, 270, 441, 270, 459)
        e.Graphics.DrawLine(blackPen, 270, 477, 270, 495)
        e.Graphics.DrawLine(blackPen, 270, 513, 270, 520)

        e.Graphics.DrawLine(blackPen, 279, 270, 261, 270)
        e.Graphics.DrawLine(blackPen, 243, 270, 225, 270)
        e.Graphics.DrawLine(blackPen, 207, 270, 189, 270)
        e.Graphics.DrawLine(blackPen, 171, 270, 153, 270)
        e.Graphics.DrawLine(blackPen, 135, 270, 117, 270)
        e.Graphics.DrawLine(blackPen, 99, 270, 81, 270)
        e.Graphics.DrawLine(blackPen, 63, 270, 45, 270)
        e.Graphics.DrawLine(blackPen, 27, 270, 20, 270)
        e.Graphics.DrawLine(blackPen, 297, 270, 315, 270)
        e.Graphics.DrawLine(blackPen, 333, 270, 351, 270)
        e.Graphics.DrawLine(blackPen, 369, 270, 387, 270)
        e.Graphics.DrawLine(blackPen, 405, 270, 423, 270)
        e.Graphics.DrawLine(blackPen, 441, 270, 459, 270)
        e.Graphics.DrawLine(blackPen, 477, 270, 495, 270)
        e.Graphics.DrawLine(blackPen, 513, 270, 520, 270)

        e.Graphics.DrawLine(blackPen, 263, 277, 277, 263)
        e.Graphics.DrawLine(blackPen, 291, 249, 305, 235)
        e.Graphics.DrawLine(blackPen, 319, 221, 333, 207)
        e.Graphics.DrawLine(blackPen, 347, 193, 361, 179)
        e.Graphics.DrawLine(blackPen, 375, 165, 389, 151)
        e.Graphics.DrawLine(blackPen, 403, 137, 417, 123)
        e.Graphics.DrawLine(blackPen, 431, 109, 445, 95)
        e.Graphics.DrawLine(blackPen, 249, 291, 235, 305)
        e.Graphics.DrawLine(blackPen, 221, 319, 207, 333)
        e.Graphics.DrawLine(blackPen, 193, 347, 179, 361)
        e.Graphics.DrawLine(blackPen, 165, 375, 151, 389)
        e.Graphics.DrawLine(blackPen, 137, 403, 123, 417)
        e.Graphics.DrawLine(blackPen, 109, 431, 95, 445)

        e.Graphics.DrawLine(blackPen, 263, 263, 277, 277)
        e.Graphics.DrawLine(blackPen, 291, 291, 305, 305)
        e.Graphics.DrawLine(blackPen, 319, 319, 333, 333)
        e.Graphics.DrawLine(blackPen, 347, 347, 361, 361)
        e.Graphics.DrawLine(blackPen, 375, 375, 389, 389)
        e.Graphics.DrawLine(blackPen, 403, 403, 417, 417)
        e.Graphics.DrawLine(blackPen, 431, 431, 445, 445)
        e.Graphics.DrawLine(blackPen, 249, 249, 235, 235)
        e.Graphics.DrawLine(blackPen, 221, 221, 207, 207)
        e.Graphics.DrawLine(blackPen, 193, 193, 179, 179)
        e.Graphics.DrawLine(blackPen, 165, 165, 151, 151)
        e.Graphics.DrawLine(blackPen, 137, 137, 123, 123)
        e.Graphics.DrawLine(blackPen, 109, 109, 95, 95)

        'draw azimuth text
        e.Graphics.DrawString("0" & Chr(176), textFont, Brushes.Black, 262, 0)
        e.Graphics.DrawString("45" & Chr(176), textFont, Brushes.Black, 442, 73)
        e.Graphics.DrawString("90" & Chr(176), textFont, Brushes.Black, 515, 258)
        e.Graphics.DrawString("135" & Chr(176), textFont, Brushes.Black, 437, 444)
        e.Graphics.DrawString("180" & Chr(176), textFont, Brushes.Black, 251, 520)
        e.Graphics.DrawString("225" & Chr(176), textFont, Brushes.Black, 68, 444)
        e.Graphics.DrawString("270" & Chr(176), textFont, Brushes.Black, 0, 258)
        e.Graphics.DrawString("315" & Chr(176), textFont, Brushes.Black, 68, 73)

        'draw elevation text
        e.Graphics.DrawString("10" & Chr(176), elevFont, Brushes.Black, 270, 54)
        e.Graphics.DrawString("30" & Chr(176), elevFont, Brushes.Black, 270, 108)
        e.Graphics.DrawString("50" & Chr(176), elevFont, Brushes.Black, 270, 162)
        e.Graphics.DrawString("70" & Chr(176), elevFont, Brushes.Black, 270, 216)

        'draw in Satellite positions
        Dim i As Integer
        Dim HDist As Decimal
        Dim SatX, SatY As Decimal
        Dim SatYesNo As New Matrix(33, 1)
        Dim LegendItem As Integer = 2

        e.Graphics.DrawString("SATELLITE PRN", elevFont, Brushes.Black, 630, 5)
        e.Graphics.DrawString("SkyPlot Centered at:", textFont, Brushes.Black, 540, 425)
        e.Graphics.DrawString(mainForm.DecToDMS(satData.data(2, 1), 1), textFont, Brushes.Black, 540, 450)
        e.Graphics.DrawString(mainForm.DecToDMS(satData.data(2, 2), 2), textFont, Brushes.Black, 540, 475)
        e.Graphics.DrawString(Decimal.Round(satData.data(2, 3), 3).ToString & " m", textFont, Brushes.Black, 540, 500)

        For i = 3 To satData.nRows
            'remember form coordinate system very different from E,N,Up system

            'HDist = Math.Cos(satData.data(i, 3) * Math.PI / 180D) * 243    'HDIST on a sphere
            HDist = ((90D - satData.data(i, 3)) / 90D) * 243                    'HDIST on a sphere projected onto a plane 

            SatX = (270D + Math.Sin(satData.data(i, 2) * Math.PI / 180D) * HDist)
            SatY = (270D - Math.Cos(satData.data(i, 2) * Math.PI / 180D) * HDist)

            Select Case satData.data(i, 1)
                Case 1
                    satPen.Color = Color.Red
                    If SatYesNo.data(1, 1) = 0 Then
                        SatYesNo.data(1, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 2
                    satPen.Color = Color.Orange
                    If SatYesNo.data(2, 1) = 0 Then
                        SatYesNo.data(2, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 3
                    satPen.Color = Color.Yellow
                    If SatYesNo.data(3, 1) = 0 Then
                        SatYesNo.data(3, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 4
                    satPen.Color = Color.Green
                    If SatYesNo.data(4, 1) = 0 Then
                        SatYesNo.data(4, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 5
                    satPen.Color = Color.MediumVioletRed
                    If SatYesNo.data(5, 1) = 0 Then
                        SatYesNo.data(5, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 6
                    satPen.Color = Color.Indigo
                    If SatYesNo.data(6, 1) = 0 Then
                        SatYesNo.data(6, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 7
                    satPen.Color = Color.Violet
                    If SatYesNo.data(7, 1) = 0 Then
                        SatYesNo.data(7, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 8
                    satPen.Color = Color.DarkBlue
                    If SatYesNo.data(8, 1) = 0 Then
                        SatYesNo.data(8, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 9
                    satPen.Color = Color.DarkCyan
                    If SatYesNo.data(9, 1) = 0 Then
                        SatYesNo.data(9, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 10
                    satPen.Color = Color.CornflowerBlue
                    If SatYesNo.data(10, 1) = 0 Then
                        SatYesNo.data(10, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 11
                    satPen.Color = Color.DarkGray
                    If SatYesNo.data(11, 1) = 0 Then
                        SatYesNo.data(11, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 12
                    satPen.Color = Color.DarkGreen
                    If SatYesNo.data(12, 1) = 0 Then
                        SatYesNo.data(12, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 13
                    satPen.Color = Color.DarkKhaki
                    If SatYesNo.data(13, 1) = 0 Then
                        SatYesNo.data(13, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 14
                    satPen.Color = Color.DarkMagenta
                    If SatYesNo.data(14, 1) = 0 Then
                        SatYesNo.data(14, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 15
                    satPen.Color = Color.DarkOliveGreen
                    If SatYesNo.data(15, 1) = 0 Then
                        SatYesNo.data(15, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 16
                    satPen.Color = Color.DarkOrange
                    If SatYesNo.data(16, 1) = 0 Then
                        SatYesNo.data(16, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 17
                    satPen.Color = Color.DarkOrchid
                    If SatYesNo.data(17, 1) = 0 Then
                        SatYesNo.data(17, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 18
                    satPen.Color = Color.DarkRed
                    If SatYesNo.data(18, 1) = 0 Then
                        SatYesNo.data(18, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 19
                    satPen.Color = Color.DarkSalmon
                    If SatYesNo.data(19, 1) = 0 Then
                        SatYesNo.data(19, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 20
                    satPen.Color = Color.DarkSeaGreen
                    If SatYesNo.data(20, 1) = 0 Then
                        SatYesNo.data(20, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 21
                    satPen.Color = Color.DarkSlateBlue
                    If SatYesNo.data(21, 1) = 0 Then
                        SatYesNo.data(21, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 22
                    satPen.Color = Color.DarkSlateGray
                    If SatYesNo.data(22, 1) = 0 Then
                        SatYesNo.data(22, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 23
                    satPen.Color = Color.DarkTurquoise
                    If SatYesNo.data(23, 1) = 0 Then
                        SatYesNo.data(23, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 24
                    satPen.Color = Color.DarkViolet
                    If SatYesNo.data(24, 1) = 0 Then
                        SatYesNo.data(24, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 25
                    satPen.Color = Color.DeepPink
                    If SatYesNo.data(25, 1) = 0 Then
                        SatYesNo.data(25, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 26
                    satPen.Color = Color.DeepSkyBlue
                    If SatYesNo.data(26, 1) = 0 Then
                        SatYesNo.data(26, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 27
                    satPen.Color = Color.Firebrick
                    If SatYesNo.data(27, 1) = 0 Then
                        SatYesNo.data(27, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 28
                    satPen.Color = Color.ForestGreen
                    If SatYesNo.data(28, 1) = 0 Then
                        SatYesNo.data(28, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 29
                    satPen.Color = Color.GreenYellow
                    If SatYesNo.data(29, 1) = 0 Then
                        SatYesNo.data(29, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 30
                    satPen.Color = Color.Brown
                    If SatYesNo.data(30, 1) = 0 Then
                        SatYesNo.data(30, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 31
                    satPen.Color = Color.OrangeRed
                    If SatYesNo.data(31, 1) = 0 Then
                        SatYesNo.data(31, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case 32
                    satPen.Color = Color.Purple
                    If SatYesNo.data(32, 1) = 0 Then
                        SatYesNo.data(32, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(satData.data(i, 1).ToString, elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
                Case Else
                    satPen.Color = Color.Black
                    If SatYesNo.data(33, 1) = 0 Then
                        SatYesNo.data(33, 1) = 1
                        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
                        e.Graphics.DrawString(">32", elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
                        LegendItem += 1
                    End If
            End Select

            If ONEorALL = 2 Then
                e.Graphics.DrawEllipse(satPen, SatX, SatY, 2, 2)
            Else
                SatPen2.Color = satPen.Color
                e.Graphics.DrawEllipse(SatPen2, SatX, SatY, 1, 1)
            End If
        Next
        LegendItem += 2
        satPen.Color = Color.Blue
        e.Graphics.DrawLine(satPen, 635, LegendItem * 11 + 5, 675, LegendItem * 11 + 5)
        e.Graphics.DrawString("MASK", elevFont, Brushes.Black, 680, LegendItem * 11 - 2)
    End Sub

    Private Sub CloseButton_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Close()
    End Sub
End Class