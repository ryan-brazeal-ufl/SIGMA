'''''SIGMA'''''
'By: Ryan Brazeal
'Date: April 2015
'GNU GPL V2 License
'www.rgbi.ca
'www.jrpi.ca

Public Class PositionsPlot
    Friend EpochPositions As Matrix
    Friend CurvPosition(1) As Decimal
    Dim controlWarning As Boolean = False
    Dim messageWarning As Boolean = False

    Private Sub CloseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Close()
    End Sub

    Private Sub PositionsPlot_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        controlWarning = False
        messageWarning = False
    End Sub

    Private Sub PositionsPlot_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

        Dim ScreenBasePt(1), PosBasePt(1) As Integer
        Dim dashSpacing As Integer
        Dim lineOffset As Integer
        Dim LinesPerSide As Integer
        Dim drawingAreaWidth As Integer
        Dim i, j, k As Integer
        Dim fineBlackPen As New Pen(Color.Black, 0)
        Dim positionPen As New Pen(Color.Red, 5)
        Dim controlPtPen As New Pen(Color.Black, 8)
        Dim highestX, highestY, lowestX, lowestY As Integer
        Dim rejectedPoints As Integer = 0
        Dim axisFont As New Font("arial", 12, FontStyle.Regular, GraphicsUnit.Pixel)
        Dim textFont As New Font("arial", 20, FontStyle.Regular, GraphicsUnit.Pixel)

        ScreenBasePt(0) = 340
        ScreenBasePt(1) = 320
        PosBasePt(0) = EpochPositions.data(EpochPositions.nRows, 1)
        PosBasePt(1) = EpochPositions.data(EpochPositions.nRows, 2)

        dashSpacing = 5
        lineOffset = 55
        LinesPerSide = 5
        drawingAreaWidth = LinesPerSide * 2 * lineOffset

        With e.Graphics
            highestX = ScreenBasePt(0) + lineOffset * LinesPerSide
            lowestX = ScreenBasePt(0) - lineOffset * LinesPerSide
            highestY = ScreenBasePt(1) + lineOffset * LinesPerSide
            lowestY = ScreenBasePt(1) - lineOffset * LinesPerSide

            For i = lowestX To highestX Step lineOffset
                For j = lowestY To highestY - dashSpacing Step dashSpacing
                    .DrawEllipse(fineBlackPen, i, j, 1, 1)
                Next
            Next

            For i = lowestY + lineOffset To highestY - lineOffset Step lineOffset
                For j = lowestX To highestX Step dashSpacing
                    .DrawEllipse(fineBlackPen, j, i, 1, 1)
                Next
            Next
            .DrawRectangle(fineBlackPen, lowestX, lowestY, drawingAreaWidth, drawingAreaWidth)

            Dim PosScaleN, PosScaleE, AvgPosSF As Decimal
            If (EpochPositions.data(EpochPositions.nRows - 2, 1) - EpochPositions.data(EpochPositions.nRows - 2, 2)) = 0 Then
                PosScaleN = 1000000000000
            Else
                PosScaleN = drawingAreaWidth / (EpochPositions.data(EpochPositions.nRows - 2, 1) - EpochPositions.data(EpochPositions.nRows - 2, 2))
            End If

            If (EpochPositions.data(EpochPositions.nRows - 1, 1) - EpochPositions.data(EpochPositions.nRows - 1, 2)) = 0 Then
                PosScaleE = 1000000000000
            Else
                PosScaleE = drawingAreaWidth / (EpochPositions.data(EpochPositions.nRows - 1, 1) - EpochPositions.data(EpochPositions.nRows - 1, 2))
            End If

            If PosScaleE < PosScaleN Then
                AvgPosSF = PosScaleE
            Else
                AvgPosSF = PosScaleN
            End If
            AvgPosSF *= 0.99    '1% reduction to scale to hopefully account for some rounding from decimal to integer coordinates errors
            PosScaleE *= 0.99
            PosScaleN *= 0.99

            Dim roundingNumber As Integer = 1
            If EpochPositions.data(EpochPositions.nRows - 2, 1) - EpochPositions.data(EpochPositions.nRows - 2, 2) <= 1 Then
                roundingNumber = 2
                If EpochPositions.data(EpochPositions.nRows - 1, 1) - EpochPositions.data(EpochPositions.nRows - 1, 2) <= 1 Then
                    roundingNumber = 2
                Else
                    roundingNumber = 1
                End If
            End If

            Dim drawingX, drawingY As Integer
            Dim TriPoints(2) As PointF

            For i = 1 To EpochPositions.nRows - 4
                drawingX = ScreenBasePt(0) + (EpochPositions.data(i, 1) - EpochPositions.data(EpochPositions.nRows, 1)) * AvgPosSF
                drawingY = ScreenBasePt(1) - (EpochPositions.data(i, 2) - EpochPositions.data(EpochPositions.nRows, 2)) * AvgPosSF
                If drawingX > lowestX And drawingX < highestX And drawingY > lowestY And drawingY < highestY Then
                    .DrawEllipse(positionPen, drawingX, drawingY, 1, 1)
                Else
                    rejectedPoints += 1
                End If
            Next

            If EpochPositions.data(EpochPositions.nRows - 3, 1) <> -9999D Then
                drawingX = ScreenBasePt(0) + (EpochPositions.data(EpochPositions.nRows - 3, 1) - EpochPositions.data(EpochPositions.nRows, 1)) * AvgPosSF
                drawingY = ScreenBasePt(1) - (EpochPositions.data(EpochPositions.nRows - 3, 2) - EpochPositions.data(EpochPositions.nRows, 2)) * AvgPosSF
                If drawingX > lowestX And drawingX < highestX And drawingY > lowestY And drawingY < highestY Then
                    '.DrawEllipse(controlPtPen, drawingX, drawingY, 1, 1)
                    TriPoints(0).X = drawingX + 6
                    TriPoints(0).Y = drawingY + 3
                    TriPoints(1).X = drawingX - 6
                    TriPoints(1).Y = drawingY + 3
                    TriPoints(2).X = drawingX
                    TriPoints(2).Y = drawingY - 8
                    .FillPolygon(Brushes.Blue, TriPoints, Drawing2D.FillMode.Alternate)
                    Dim myPen As New Pen(Color.DarkBlue, 2)
                    .DrawPolygon(myPen, TriPoints)
                Else
                    rejectedPoints += 1
                    If controlWarning = False Then
                        controlWarning = True
                        MessageBox.Show("The Control Point you specified is not within the scale of this Plot" & ControlChars.NewLine & _
                        "Either their was too much error in the observations or the Control Point position was incorrect", "Control Point position can not be drawn", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End If
            End If

            If mainForm.BeenProcessed = True Then
                drawingX = ScreenBasePt(0) + (mainForm.ProcessedSolution.data(1, 1) - EpochPositions.data(EpochPositions.nRows, 1)) * AvgPosSF
                drawingY = ScreenBasePt(1) - (mainForm.ProcessedSolution.data(2, 1) - EpochPositions.data(EpochPositions.nRows, 2)) * AvgPosSF
                If drawingX > lowestX And drawingX < highestX And drawingY > lowestY And drawingY < highestY Then
                    '.DrawEllipse(controlPtPen, drawingX, drawingY, 1, 1)
                    TriPoints(0).X = drawingX + 6
                    TriPoints(0).Y = drawingY + 3
                    TriPoints(1).X = drawingX - 6
                    TriPoints(1).Y = drawingY + 3
                    TriPoints(2).X = drawingX
                    TriPoints(2).Y = drawingY - 8
                    .FillPolygon(Brushes.Yellow, TriPoints, Drawing2D.FillMode.Winding)
                    Dim myPen As New Pen(Color.LimeGreen, 2)
                    .DrawPolygon(myPen, TriPoints)
                End If
            End If

            Dim string_format1, string_format2 As New StringFormat()
            string_format1.Alignment = StringAlignment.Center
            string_format1.LineAlignment = StringAlignment.Center
            string_format2.Alignment = StringAlignment.Far
            string_format2.LineAlignment = StringAlignment.Far

            Dim DrawingScaleX As Decimal = lineOffset / AvgPosSF 'PosScaleE
            Dim DrawingScaleY As Decimal = lineOffset / AvgPosSF 'PosScaleN

            k = 0
            For i = ScreenBasePt(0) To highestX Step lineOffset
                .DrawString(Decimal.Round(k * DrawingScaleX, roundingNumber).ToString, axisFont, Brushes.Black, i, highestY + 15, string_format1)
                k += 1
            Next

            k = -1
            For i = ScreenBasePt(0) - lineOffset To lowestX Step -1 * lineOffset
                .DrawString(Decimal.Round(k * DrawingScaleX, roundingNumber).ToString, axisFont, Brushes.Black, i, highestY + 15, string_format1)
                k -= 1
            Next

            k = 0
            For i = ScreenBasePt(1) To highestY Step lineOffset
                .DrawString(Decimal.Round(k * DrawingScaleY, roundingNumber).ToString, axisFont, Brushes.Black, lowestX - 5, i + 8, string_format2)
                k -= 1
            Next

            k = 1
            For i = ScreenBasePt(1) - lineOffset To lowestY Step -1 * lineOffset
                .DrawString(Decimal.Round(k * DrawingScaleY, roundingNumber).ToString, axisFont, Brushes.Black, lowestX - 5, i + 8, string_format2)
                k += 1
            Next

            .DrawString("Longitude Scale (m)", axisFont, Brushes.Black, ScreenBasePt(0), highestY + 35, string_format1)

            .DrawString("Plot of Calculated Receiver Coordinates", textFont, Brushes.Black, ScreenBasePt(0), 15, string_format1)
            .DrawString("Centered at: " & mainForm.DecToDMS(CurvPosition(0), 1) & "  " & mainForm.DecToDMS(CurvPosition(1), 2), axisFont, Brushes.Black, ScreenBasePt(0), 33, string_format1)

            .ResetTransform()
            .RotateTransform(-90)
            .TranslateTransform(-1 * ScreenBasePt(1), 20)
            .DrawString("Latitude Scale (m)", axisFont, Brushes.Black, 0, 0, string_format1)


            If messageWarning = False Then
                messageWarning = True
                'type any test debugging message here

            End If
        End With
    End Sub
End Class