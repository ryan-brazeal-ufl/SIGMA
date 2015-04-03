'''''SIGMA'''''
'By: Ryan Brazeal
'Date: April 2015
'GNU GPL V2 License
'www.rgbi.ca
'www.jrpi.ca

Public Class HeightPlot
    Friend Heights As Matrix
    Friend elevAngle As Decimal
    Friend CentralHeight As Decimal
    Dim controlWarning As Boolean = False

    Private Sub HeightPlot_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        controlWarning = False
    End Sub

    Private Sub HeightPlot_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        CentralHeight = 0
        Dim ScreenBasePt1(1) As Integer
        Dim dashSpacing As Integer
        Dim lineOffsetX, lineOffsetY As Integer
        Dim LinesPerSideX, LinesPerSideY As Integer
        Dim drawingAreaWidth, drawingAreaHeight As Integer
        Dim i, j, k As Integer
        Dim fineBlackPen As New Pen(Color.Black, 1)
        Dim fineBluePen As New Pen(Color.Blue, 1)
        Dim LinePen As New Pen(Color.Red, 3)
        Dim highestX1, highestY1, lowestX1, lowestY1 As Integer
        Dim axisFont As New Font("arial", 12, FontStyle.Regular, GraphicsUnit.Pixel)
        Dim textFont As New Font("arial", 20, FontStyle.Regular, GraphicsUnit.Pixel)
        Dim string_format1, string_format2 As New StringFormat()
        string_format1.Alignment = StringAlignment.Center
        string_format1.LineAlignment = StringAlignment.Center
        string_format2.Alignment = StringAlignment.Far
        string_format2.LineAlignment = StringAlignment.Far


        Dim baseOffsetY As Integer = 210

        ScreenBasePt1(0) = 100
        ScreenBasePt1(1) = 50 + 0 * baseOffsetY

        dashSpacing = 5
        lineOffsetX = 80
        lineOffsetY = 60
        LinesPerSideX = 5
        LinesPerSideY = 5
        drawingAreaWidth = LinesPerSideX * 2 * lineOffsetX
        drawingAreaHeight = LinesPerSideY * 2 * lineOffsetY

        With e.Graphics

            highestX1 = ScreenBasePt1(0) + drawingAreaWidth
            lowestX1 = ScreenBasePt1(0)
            highestY1 = ScreenBasePt1(1) + drawingAreaHeight
            lowestY1 = ScreenBasePt1(1)

            For i = lowestX1 To highestX1 Step lineOffsetX
                For j = lowestY1 To highestY1 - dashSpacing Step dashSpacing
                    .DrawEllipse(fineBlackPen, i, j, 1, 1)
                Next
            Next

            For i = lowestY1 + lineOffsetY To highestY1 - lineOffsetY Step lineOffsetY
                For j = lowestX1 To highestX1 Step dashSpacing
                    .DrawEllipse(fineBlackPen, j, i, 1, 1)
                Next
            Next
            .DrawRectangle(fineBlackPen, lowestX1, lowestY1, drawingAreaWidth, drawingAreaHeight)

            Dim PosScaleX, PosScaleY1, reduction As Decimal
            reduction = 1
            If ((Heights.data(Heights.nRows - 2, 1) * 604800D + Heights.data(Heights.nRows - 1, 1)) - (Heights.data(Heights.nRows - 2, 2) * 604800D + Heights.data(Heights.nRows - 1, 2))) = 0 Then
                PosScaleX = 1000000000000
            Else
                PosScaleX = (drawingAreaWidth / ((Heights.data(Heights.nRows - 2, 1) * 604800D + Heights.data(Heights.nRows - 1, 1)) - (Heights.data(Heights.nRows - 2, 2) * 604800D + Heights.data(Heights.nRows - 1, 2))))
            End If

            If (Heights.data(Heights.nRows, 1) - Heights.data(Heights.nRows, 2)) = 0 Then
                PosScaleY1 = 1000000000000
            Else
                PosScaleY1 = reduction * (drawingAreaHeight / (Heights.data(Heights.nRows, 1) - Heights.data(Heights.nRows, 2)))
            End If

            Dim NewMidPointY As Decimal
            NewMidPointY = (Heights.data(Heights.nRows, 1) + Heights.data(Heights.nRows, 2)) / 2

            Dim roundingNumber As Integer = 1
            If Heights.data(Heights.nRows, 1) - Heights.data(Heights.nRows, 2) <= 1 Then
                roundingNumber = 2
            End If

            Dim drawingX, drawingY1 As Integer
            Dim Hx As Decimal = ScreenBasePt1(0)
            Dim Hy As Decimal = ScreenBasePt1(1) + drawingAreaHeight / 2 - (Heights.data(1, 2) - NewMidPointY) * PosScaleY1

            Dim previousWeek As Integer = Convert.ToInt32(Heights.data(1, 3))

            For i = 1 To Heights.nRows - 4
                If (Heights.data(i, 1) - Heights.data(1, 1)) < 0 Then
                    drawingX = ScreenBasePt1(0) + (Heights.data(i, 1) + 604800 - Heights.data(1, 1)) * PosScaleX
                Else
                    drawingX = ScreenBasePt1(0) + (Heights.data(i, 1) - Heights.data(1, 1)) * PosScaleX
                End If
                drawingY1 = (ScreenBasePt1(1) + drawingAreaHeight / 2) - (Heights.data(i, 2) - NewMidPointY) * PosScaleY1

                If drawingX >= lowestX1 And drawingX <= highestX1 And drawingY1 >= lowestY1 And drawingY1 <= highestY1 Then
                    If Hx >= lowestX1 And Hx <= highestX1 And Hy >= lowestY1 And Hy <= highestY1 Then
                    Else
                        Hx = drawingX
                        Hy = drawingY1
                    End If
                    LinePen.Color = Color.Red
                    LinePen.Width = 2
                    .DrawLine(LinePen, Hx, Hy, drawingX, drawingY1)
                    'LinePen.Color = Color.LightPink
                    'LinePen.Width = 1
                    '.DrawLine(LinePen, Hx, Hy, drawingX, drawingY1)
                    Hx = drawingX
                    Hy = drawingY1
                End If
                If Convert.ToInt32(Heights.data(i, 3)) - previousWeek = 1 Then
                    previousWeek = Convert.ToInt32(Heights.data(i, 3))

                    'drawing a dashed blue line showing start of new gps week
                    For b As Integer = lowestY1 To highestY1 - dashSpacing Step dashSpacing
                        .DrawEllipse(fineBluePen, drawingX, b, 1, 1)
                    Next

                End If
            Next

            If Heights.data(Heights.nRows - 3, 1) = 9999D Then
                drawingY1 = (ScreenBasePt1(1) + drawingAreaHeight / 2) - (Heights.data(Heights.nRows - 3, 2) - NewMidPointY) * PosScaleY1
                LinePen.Color = Color.DarkBlue
                If drawingY1 >= lowestY1 And drawingY1 <= highestY1 Then
                    LinePen.Width = 3
                    LinePen.Color = Color.DarkBlue
                    .DrawLine(LinePen, lowestX1, drawingY1, highestX1, drawingY1)
                    LinePen.Width = 1
                    LinePen.Color = Color.Blue
                    .DrawLine(LinePen, lowestX1, drawingY1, highestX1, drawingY1)
                Else
                    If controlWarning = False Then
                        controlWarning = True
                        MessageBox.Show("The Control Point Elevation you specified is not within the scale of this Plot" & ControlChars.NewLine & _
                        "Either their was too much error in the observations or the Control Point Height was incorrect", "Control Point Elevation can not be drawn", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End If
            End If

            If mainForm.BeenProcessed Then
                Dim processedHeight As Decimal = mainForm.ProcessedSolution.data(3, 1)
                drawingY1 = (ScreenBasePt1(1) + drawingAreaHeight / 2) - (processedHeight - NewMidPointY) * PosScaleY1
                LinePen.Width = 3
                LinePen.Color = Color.DarkBlue
                If drawingY1 >= lowestY1 And drawingY1 <= highestY1 Then
                    LinePen.Color = Color.LimeGreen
                    .DrawLine(LinePen, lowestX1, drawingY1, highestX1, drawingY1)
                    LinePen.Width = 1
                    LinePen.Color = Color.Yellow
                    .DrawLine(LinePen, lowestX1, drawingY1, highestX1, drawingY1)
                End If
            End If

            Dim TimeRange As Decimal = (Heights.data(Heights.nRows - 2, 1) * 604800D + Heights.data(Heights.nRows - 1, 1)) - (Heights.data(Heights.nRows - 2, 2) * 604800D + Heights.data(Heights.nRows - 1, 2))
            If TimeRange < 0 Then
                TimeRange += 604800
            End If
            Dim TimePerLine As Decimal = TimeRange / 10

            .DrawString(Decimal.Round(Heights.data(Heights.nRows - 1, 2), 1), axisFont, Brushes.Black, lowestX1, highestY1 + 15, string_format1)
            For i = 1 To 10
                Dim timeStamp As Decimal = Decimal.Round(Heights.data(Heights.nRows - 1, 2) + i * TimePerLine, 1)
                If timeStamp > 604800 Then
                    timeStamp -= 604800
                End If
                .DrawString(timeStamp.ToString, axisFont, Brushes.Black, lowestX1 + i * lineOffsetX, highestY1 + 15, string_format1)
            Next

            k = 0
            For i = ScreenBasePt1(1) + drawingAreaHeight / 2 To highestY1 Step lineOffsetY
                .DrawString(Decimal.Round(k * lineOffsetY / PosScaleY1 + CentralHeight + NewMidPointY, roundingNumber).ToString, axisFont, Brushes.Black, lowestX1 - 5, i + 8, string_format2)
                k -= 1
            Next

            k = 1
            For i = ScreenBasePt1(1) + drawingAreaHeight / 2 - lineOffsetY To lowestY1 Step -1 * lineOffsetY
                .DrawString(Decimal.Round(k * lineOffsetY / PosScaleY1 + CentralHeight + NewMidPointY, roundingNumber).ToString, axisFont, Brushes.Black, lowestX1 - 5, i + 8, string_format2)
                k += 1
            Next

            .DrawString("GPS Time (s)", axisFont, Brushes.Black, ScreenBasePt1(0) + drawingAreaWidth / 2, highestY1 + 45, string_format1)

            .DrawString("Plot of Calculated Receiver Height Coordinate", textFont, Brushes.Black, ScreenBasePt1(0) + drawingAreaWidth / 2, 25, string_format1)

            .ResetTransform()
            .RotateTransform(-90)
            .TranslateTransform(-1 * (ScreenBasePt1(1) + drawingAreaHeight / 2), 30)
            .DrawString("Height (m)", axisFont, Brushes.Black, 0, 0, string_format1)

        End With

    End Sub
End Class