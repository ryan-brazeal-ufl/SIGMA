'''''SIGMA'''''
'By: Ryan Brazeal
'Date: April 2015
'GNU GPL V2 License
'www.rgbi.ca
'www.jrpi.ca

Public Class diffPlots
    Friend DOP_Data As Matrix
    Friend plot1Label As String
    Friend plot2Label As String
    Friend plot3Label As String
    Friend plot4Label As String

    Private Sub DOPsPlot_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        Dim ScreenBasePt1(1), ScreenBasePt2(1), ScreenBasePt3(1), ScreenBasePt4(1), PosBasePt(1) As Integer
        Dim dashSpacing As Integer
        Dim lineOffsetX, lineOffsetY As Integer
        Dim LinesPerSideX, LinesPerSideY As Integer
        Dim drawingAreaWidth, drawingAreaHeight As Integer
        Dim i, j, k As Integer
        Dim fineBlackPen As New Pen(Color.Black, 1)
        Dim fineBluePen As New Pen(Color.Blue, 1)
        Dim LinePen As New Pen(Color.Red, 2)
        Dim highestX1, highestY1, lowestX1, lowestY1 As Integer
        Dim highestX2, highestY2, lowestX2, lowestY2 As Integer
        Dim highestX3, highestY3, lowestX3, lowestY3 As Integer
        Dim highestX4, highestY4, lowestX4, lowestY4 As Integer
        Dim rejectedPoints As Integer = 0
        Dim axisFont As New Font("arial", 12, FontStyle.Regular, GraphicsUnit.Pixel)
        Dim textFont As New Font("arial", 20, FontStyle.Regular, GraphicsUnit.Pixel)
        Dim string_format1, string_format2 As New StringFormat()
        string_format1.Alignment = StringAlignment.Center
        string_format1.LineAlignment = StringAlignment.Center
        string_format2.Alignment = StringAlignment.Far
        string_format2.LineAlignment = StringAlignment.Far

        Dim baseOffsetY As Integer = 170

        ScreenBasePt1(0) = 100
        ScreenBasePt1(1) = 50 + 0 * baseOffsetY
        ScreenBasePt2(0) = ScreenBasePt1(0)
        ScreenBasePt2(1) = 50 + 1 * baseOffsetY
        ScreenBasePt3(0) = ScreenBasePt1(0)
        ScreenBasePt3(1) = 50 + 2 * baseOffsetY
        ScreenBasePt4(0) = ScreenBasePt1(0)
        ScreenBasePt4(1) = 50 + 3 * baseOffsetY

        dashSpacing = 5
        lineOffsetX = 70
        lineOffsetY = 25
        LinesPerSideX = 5
        LinesPerSideY = 3
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

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            highestX2 = ScreenBasePt2(0) + drawingAreaWidth
            lowestX2 = ScreenBasePt2(0)
            highestY2 = ScreenBasePt2(1) + drawingAreaHeight
            lowestY2 = ScreenBasePt2(1)

            For i = lowestX2 To highestX2 Step lineOffsetX
                For j = lowestY2 To highestY2 - dashSpacing Step dashSpacing
                    .DrawEllipse(fineBlackPen, i, j, 1, 1)
                Next
            Next

            For i = lowestY2 + lineOffsetY To highestY2 - lineOffsetY Step lineOffsetY
                For j = lowestX2 To highestX2 Step dashSpacing
                    .DrawEllipse(fineBlackPen, j, i, 1, 1)
                Next
            Next
            .DrawRectangle(fineBlackPen, lowestX2, lowestY2, drawingAreaWidth, drawingAreaHeight)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            highestX3 = ScreenBasePt3(0) + drawingAreaWidth
            lowestX3 = ScreenBasePt3(0)
            highestY3 = ScreenBasePt3(1) + drawingAreaHeight
            lowestY3 = ScreenBasePt3(1)

            For i = lowestX3 To highestX3 Step lineOffsetX
                For j = lowestY3 To highestY3 - dashSpacing Step dashSpacing
                    .DrawEllipse(fineBlackPen, i, j, 1, 1)
                Next
            Next

            For i = lowestY3 + lineOffsetY To highestY3 - lineOffsetY Step lineOffsetY
                For j = lowestX3 To highestX3 Step dashSpacing
                    .DrawEllipse(fineBlackPen, j, i, 1, 1)
                Next
            Next
            .DrawRectangle(fineBlackPen, lowestX3, lowestY3, drawingAreaWidth, drawingAreaHeight)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            highestX4 = ScreenBasePt4(0) + drawingAreaWidth
            lowestX4 = ScreenBasePt4(0)
            highestY4 = ScreenBasePt4(1) + drawingAreaHeight
            lowestY4 = ScreenBasePt4(1)

            For i = lowestX4 To highestX4 Step lineOffsetX
                For j = lowestY4 To highestY4 - dashSpacing Step dashSpacing
                    .DrawEllipse(fineBlackPen, i, j, 1, 1)
                Next
            Next

            For i = lowestY4 + lineOffsetY To highestY4 - lineOffsetY Step lineOffsetY
                For j = lowestX4 To highestX4 Step dashSpacing
                    .DrawEllipse(fineBlackPen, j, i, 1, 1)
                Next
            Next
            .DrawRectangle(fineBlackPen, lowestX4, lowestY4, drawingAreaWidth, drawingAreaHeight)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            Dim PosScaleX, PosScaleY1, PosScaleY2, PosScaleY3, PosScaleY4, reduction As Decimal
            reduction = 1
            If ((DOP_Data.data(DOP_Data.nRows - 5, 1) * 604800D + DOP_Data.data(DOP_Data.nRows - 4, 1)) - (DOP_Data.data(DOP_Data.nRows - 5, 2) * 604800D + DOP_Data.data(DOP_Data.nRows - 4, 2))) = 0 Then
                PosScaleX = 1000000000000
            Else
                PosScaleX = reduction * (drawingAreaWidth / ((DOP_Data.data(DOP_Data.nRows - 5, 1) * 604800D + DOP_Data.data(DOP_Data.nRows - 4, 1)) - (DOP_Data.data(DOP_Data.nRows - 5, 2) * 604800D + DOP_Data.data(DOP_Data.nRows - 4, 2))))
            End If

            If (DOP_Data.data(DOP_Data.nRows - 3, 1) - DOP_Data.data(DOP_Data.nRows - 3, 2)) = 0 Then
                PosScaleY1 = 1000000000000
            Else
                PosScaleY1 = reduction * (drawingAreaHeight / (DOP_Data.data(DOP_Data.nRows - 3, 1) - DOP_Data.data(DOP_Data.nRows - 3, 2)))
            End If

            If (DOP_Data.data(DOP_Data.nRows - 2, 1) - DOP_Data.data(DOP_Data.nRows - 2, 2)) = 0 Then
                PosScaleY2 = 1000000000000
            Else
                PosScaleY2 = reduction * (drawingAreaHeight / (DOP_Data.data(DOP_Data.nRows - 2, 1) - DOP_Data.data(DOP_Data.nRows - 2, 2)))
            End If

            If (DOP_Data.data(DOP_Data.nRows - 1, 1) - DOP_Data.data(DOP_Data.nRows - 1, 2)) = 0 Then
                PosScaleY3 = 1000000000000
            Else
                PosScaleY3 = reduction * (drawingAreaHeight / (DOP_Data.data(DOP_Data.nRows - 1, 1) - DOP_Data.data(DOP_Data.nRows - 1, 2)))
            End If

            If (DOP_Data.data(DOP_Data.nRows, 1) - DOP_Data.data(DOP_Data.nRows, 2)) = 0 Then
                PosScaleY4 = 1000000000000
            Else
                PosScaleY4 = reduction * (drawingAreaHeight / (DOP_Data.data(DOP_Data.nRows, 1) - DOP_Data.data(DOP_Data.nRows, 2)))
            End If

            'PosScaleY1 = reduction * (drawingAreaHeight / 12) 'DOP_Data.data(DOP_Data.nRows - 3, 1))

            Dim drawingX, drawingY1, drawingY2, drawingY3, drawingY4 As Integer
            Dim Satx As Decimal = ScreenBasePt1(0)
            Dim Saty As Decimal = ScreenBasePt1(1) + drawingAreaHeight - (DOP_Data.data(1, 2) - DOP_Data.data(DOP_Data.nRows - 3, 2)) * PosScaleY1
            Dim PDOPx As Decimal = ScreenBasePt2(0)
            Dim PDOPy As Decimal = ScreenBasePt2(1) + drawingAreaHeight - (DOP_Data.data(1, 3) - DOP_Data.data(DOP_Data.nRows - 2, 2)) * PosScaleY2
            Dim HDOPx As Decimal = ScreenBasePt3(0)
            Dim HDOPy As Decimal = ScreenBasePt3(1) + drawingAreaHeight - (DOP_Data.data(1, 4) - DOP_Data.data(DOP_Data.nRows - 1, 2)) * PosScaleY3
            Dim VDOPx As Decimal = ScreenBasePt4(0)
            Dim VDOPy As Decimal = ScreenBasePt4(1) + drawingAreaHeight - (DOP_Data.data(1, 5) - DOP_Data.data(DOP_Data.nRows, 2)) * PosScaleY4

            Dim previousWeek As Integer = Convert.ToInt32(DOP_Data.data(1, 6))

            For i = 1 To DOP_Data.nRows - 6
                If (DOP_Data.data(i, 1) - DOP_Data.data(1, 1)) < 0 Then
                    drawingX = ScreenBasePt1(0) + (DOP_Data.data(i, 1) + 604800 - DOP_Data.data(1, 1)) * PosScaleX
                Else
                    drawingX = ScreenBasePt1(0) + (DOP_Data.data(i, 1) - DOP_Data.data(1, 1)) * PosScaleX
                End If
                drawingY1 = (ScreenBasePt1(1) + drawingAreaHeight) - (DOP_Data.data(i, 2) - DOP_Data.data(DOP_Data.nRows - 3, 2)) * PosScaleY1
                drawingY2 = (ScreenBasePt2(1) + drawingAreaHeight) - (DOP_Data.data(i, 3) - DOP_Data.data(DOP_Data.nRows - 2, 2)) * PosScaleY2
                drawingY3 = (ScreenBasePt3(1) + drawingAreaHeight) - (DOP_Data.data(i, 4) - DOP_Data.data(DOP_Data.nRows - 1, 2)) * PosScaleY3
                drawingY4 = (ScreenBasePt4(1) + drawingAreaHeight) - (DOP_Data.data(i, 5) - DOP_Data.data(DOP_Data.nRows, 2)) * PosScaleY4

                LinePen.Color = Color.Red
                If drawingX >= lowestX1 And drawingX <= highestX1 And drawingY1 >= lowestY1 And drawingY1 <= highestY1 Then
                    If Satx >= lowestX1 And Satx <= highestX1 And Saty >= lowestY1 And Saty <= highestY1 Then
                    Else
                        Satx = drawingX
                        Saty = drawingY1
                    End If
                    .DrawLine(LinePen, Satx, Saty, drawingX, drawingY1)
                    Satx = drawingX
                    Saty = drawingY1
                End If

                LinePen.Color = Color.Blue
                If drawingX >= lowestX2 And drawingX <= highestX2 And drawingY2 >= lowestY2 And drawingY2 <= highestY2 Then
                    If PDOPx >= lowestX2 And PDOPx <= highestX2 And PDOPy >= lowestY2 And PDOPy <= highestY2 Then
                    Else
                        PDOPx = drawingX
                        PDOPy = drawingY2
                    End If
                    .DrawLine(LinePen, PDOPx, PDOPy, drawingX, drawingY2)
                    PDOPx = drawingX
                    PDOPy = drawingY2
                End If

                LinePen.Color = Color.Green
                If drawingX >= lowestX3 And drawingX <= highestX3 And drawingY3 >= lowestY3 And drawingY3 <= highestY3 Then
                    If HDOPx >= lowestX3 And HDOPx <= highestX3 And HDOPy >= lowestY3 And HDOPy <= highestY3 Then
                    Else
                        HDOPx = drawingX
                        HDOPy = drawingY3
                    End If
                    .DrawLine(LinePen, HDOPx, HDOPy, drawingX, drawingY3)
                    HDOPx = drawingX
                    HDOPy = drawingY3
                End If

                LinePen.Color = Color.Purple
                If drawingX >= lowestX4 And drawingX <= highestX4 And drawingY4 >= lowestY4 And drawingY4 <= highestY4 Then
                    If VDOPx >= lowestX4 And VDOPx <= highestX4 And VDOPy >= lowestY4 And VDOPy <= highestY4 Then
                    Else
                        VDOPx = drawingX
                        VDOPy = drawingY4
                    End If
                    .DrawLine(LinePen, VDOPx, VDOPy, drawingX, drawingY4)
                    VDOPx = drawingX
                    VDOPy = drawingY4
                End If

                If Convert.ToInt32(DOP_Data.data(i, 6)) - previousWeek = 1 Then
                    previousWeek = Convert.ToInt32(DOP_Data.data(i, 6))

                    'drawing a dashed blue line showing start of new gps week
                    For b As Integer = lowestY1 To highestY1 - dashSpacing Step dashSpacing
                        .DrawEllipse(fineBluePen, drawingX, b, 1, 1)
                    Next

                    For b As Integer = lowestY2 To highestY2 - dashSpacing Step dashSpacing
                        .DrawEllipse(fineBluePen, drawingX, b, 1, 1)
                    Next

                    For b As Integer = lowestY3 To highestY3 - dashSpacing Step dashSpacing
                        .DrawEllipse(fineBluePen, drawingX, b, 1, 1)
                    Next

                    For b As Integer = lowestY4 To highestY4 - dashSpacing Step dashSpacing
                        .DrawEllipse(fineBluePen, drawingX, b, 1, 1)
                    Next

                End If

            Next

            Dim TimeRange As Decimal = ((DOP_Data.data(DOP_Data.nRows - 5, 1) * 604800D + DOP_Data.data(DOP_Data.nRows - 4, 1)) - (DOP_Data.data(DOP_Data.nRows - 5, 2) * 604800D + DOP_Data.data(DOP_Data.nRows - 4, 2)))
            Dim TimePerLine As Decimal = TimeRange / 10

            .DrawString(Decimal.Round(DOP_Data.data(DOP_Data.nRows - 4, 2), 1), axisFont, Brushes.Black, lowestX1, highestY4 + 15, string_format1)
            For i = 1 To 10
                Dim timeStamp As Decimal = Decimal.Round(DOP_Data.data(DOP_Data.nRows - 4, 2) + i * TimePerLine, 1)
                If timeStamp > 604800 Then
                    timeStamp -= 604800
                End If

                .DrawString(timeStamp.ToString, axisFont, Brushes.Black, lowestX1 + i * lineOffsetX, highestY4 + 15, string_format1)
            Next

            k = 0
            For i = highestY4 To lowestY4 Step -1 * lineOffsetY
                .DrawString(Decimal.Round(DOP_Data.data(DOP_Data.nRows, 2) + k * lineOffsetY / PosScaleY4, 3), axisFont, Brushes.Purple, ScreenBasePt4(0) - 15, i + 8, string_format2)
                k += 1
            Next

            k = 0
            For i = highestY3 To lowestY3 Step -1 * lineOffsetY
                .DrawString(Decimal.Round(DOP_Data.data(DOP_Data.nRows - 1, 2) + k * lineOffsetY / PosScaleY3, 3), axisFont, Brushes.Green, ScreenBasePt4(0) - 15, i + 8, string_format2)
                k += 1
            Next

            k = 0
            For i = highestY2 To lowestY2 Step -1 * lineOffsetY
                .DrawString(Decimal.Round(DOP_Data.data(DOP_Data.nRows - 2, 2) + k * lineOffsetY / PosScaleY2, 3), axisFont, Brushes.Blue, ScreenBasePt4(0) - 15, i + 8, string_format2)
                k += 1
            Next

            k = 0
            For i = highestY1 To lowestY1 Step -1 * lineOffsetY
                .DrawString(Decimal.Round(DOP_Data.data(DOP_Data.nRows - 3, 2) + k * lineOffsetY / PosScaleY1, 3), axisFont, Brushes.Red, ScreenBasePt4(0) - 15, i + 8, string_format2)
                k += 1
            Next

            .DrawString("GPS Time (s)", axisFont, Brushes.Black, ScreenBasePt1(0) + drawingAreaWidth / 2, highestY4 + 40, string_format1)

            .DrawString("Differential Processing Results Plots", textFont, Brushes.Black, ScreenBasePt1(0) + drawingAreaWidth / 2, 15, string_format1)
            .DrawString("SIGMA", axisFont, Brushes.Black, ScreenBasePt1(0) + drawingAreaWidth / 2, 35, string_format1)

            .ResetTransform()
            .RotateTransform(-90)
            .TranslateTransform(-1 * (ScreenBasePt1(1) + drawingAreaHeight / 2), 830)
            .DrawString(plot1Label, axisFont, Brushes.Red, 0, 0, string_format1)

            .ResetTransform()
            .RotateTransform(-90)
            .TranslateTransform(-1 * (ScreenBasePt2(1) + drawingAreaHeight / 2), 830)
            .DrawString(plot2Label, axisFont, Brushes.Blue, 0, 0, string_format1)

            .ResetTransform()
            .RotateTransform(-90)
            .TranslateTransform(-1 * (ScreenBasePt3(1) + drawingAreaHeight / 2), 830)
            .DrawString(plot3Label, axisFont, Brushes.Green, 0, 0, string_format1)

            .ResetTransform()
            .RotateTransform(-90)
            .TranslateTransform(-1 * (ScreenBasePt4(1) + drawingAreaHeight / 2), 830)
            .DrawString(plot4Label, axisFont, Brushes.Purple, 0, 0, string_format1)

        End With
    End Sub
End Class