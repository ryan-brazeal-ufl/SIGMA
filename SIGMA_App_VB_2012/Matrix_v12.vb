'VB.NET 2005 MATRIX CLASS
'VERSION 1.2

'Programmed by: Ryan Brazeal
'Date: February, 2008

'Created to help students within the Dept. of Geomatics Technology at the Saskatchewan Institute of Applied Science and Technology (SIAST)
'learn and explore linear algebra and also to help create Least-Squares applications

'UPDATE LOG:

'   - added method to convert a 1x1 matrix to a scalar value of decimal data type   [Feb.12, 2008]
'   - added method to return the diagonal elements of a square matrix as a vector matrix (ie. nRows, 1) [Feb.12, 2008]
'   - added method to return the square root of all the elements within a matrix  [Feb.12, 2008]
'   - removed Matrix Inversion by Cholesky Decomposition due to debugging problem and Gaussian Inversion method works perfect! [Feb.12, 2008]
'   - added redimensioning method to either preserve existing data and simply rezise the matrix or resize and reset all elements = 0 [Feb.26, 2008]
'   - added methods to return an entire row or column from a matrix [Mar.2, 2008]


'Please use an APPROX. ZERO VALUE = 0.000000000001 instead of the values 0.0, especially for VC matrices

Imports Microsoft.VisualBasic
Imports Microsoft

Public Class Matrix
    
    Private nRowsV As Integer               'number of rows in the matrix
    Private nColsV As Integer               'number of columns in the matrix
    Private dataV(0, 0) As Decimal          'array to store individual matrix elements

    'default constructor, creates 1 by 1 array
    Sub New()
        nRowsV = 1
        nColsV = 1
        data(1, 1) = 0
    End Sub

    'construtor with specific # of rows and # of columns (base1 NOT base 0)
    Sub New(ByVal rows As Integer, ByVal cols As Integer)
        nRowsV = rows
        nColsV = cols

        Dim newMatrix(,) As Decimal = My2DReDim(rows, cols)
        dataV = newMatrix
    End Sub

    'used with the New(rows,cols) constructor to create the empty array of the correct size
    Private Function My2DReDim(ByVal rows As Integer, ByVal cols As Integer) As Decimal(,)
        Dim newMatrix(rows - 1, cols - 1) As Decimal
        Dim i, j As Integer

        For i = 0 To rows - 1
            For j = 0 To cols - 1
                newMatrix(i, j) = 0D
            Next
        Next
        Return newMatrix
    End Function

    'gets the number of rows in the matrix (base 1 NOT base 0)
    ReadOnly Property nRows() As Integer
        Get
            Return nRowsV
        End Get
    End Property

    'gets the number of columns in the matrix (base 1 NOT base 0)
    ReadOnly Property nCols() As Integer
        Get
            Return nColsV
        End Get
    End Property

    'get or set an individual matrix element (row and column are base 1 on input NOT base 0)
    Property data(ByVal row As Integer, ByVal col As Integer) As Decimal
        Get
            row -= 1
            col -= 1
            If row < nRowsV And col < nColsV Then
                If row >= 0 And col >= 0 Then
                    Return dataV(row, col)
                Else
                    MessageBox.Show("Attempted to read Matrix data at an index less than zero (ie. Index to Low)", "Index Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                    ErrorSoKill()
                End If
            Else
                MessageBox.Show("Attempted to read Matrix data on a row or column that does not exist (ie. Index to High)", "Index Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                ErrorSoKill()
            End If
        End Get
        Set(ByVal value As Decimal)
            row -= 1
            col -= 1
            If row < nRowsV And col < nColsV Then
                If row >= 0 And col >= 0 Then
                    dataV(row, col) = value
                Else
                    MessageBox.Show("Attempted to set Matrix data at an index less than zero (ie. Index to Low)", "Index Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                    ErrorSoKill()
                End If
            Else
                MessageBox.Show("Attempted to set Matrix data on a row or column that does not exist (ie. Index to High)", "Index Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                ErrorSoKill()
            End If
        End Set
    End Property

    'matrix multiplication
    Shared Operator *(ByVal matrix1 As Matrix, ByVal matrix2 As Matrix) As Matrix
        Dim solutionMatrix As New Matrix(matrix1.nRows, matrix2.nCols)
        If matrix1.nCols = matrix2.nRows Then
            Dim i, j, k As Integer

            For i = 1 To matrix1.nRows
                For j = 1 To matrix2.nCols
                    For k = 1 To matrix1.nCols
                        solutionMatrix.data(i, j) += matrix1.data(i, k) * matrix2.data(k, j)
                    Next
                Next
            Next
        Else
            MessageBox.Show("Attempted to multiply matrices of incorrect size (ie. Columns1 not = Rows2)", "Incorrect Matrix dimensions for Multiplication", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            solutionMatrix.ErrorSoKill()
        End If
        Return solutionMatrix
    End Operator

    'matrix addition
    Shared Operator +(ByVal matrix1 As Matrix, ByVal matrix2 As Matrix) As Matrix
        Dim solutionMatrix As New Matrix(matrix1.nRows, matrix1.nCols)
        If matrix1.nRows = matrix2.nRows And matrix1.nCols = matrix2.nCols Then
            Dim i, j As Integer
            For i = 1 To matrix1.nRows
                For j = 1 To matrix1.nCols
                    solutionMatrix.data(i, j) = matrix1.data(i, j) + matrix2.data(i, j)
                Next
            Next
        Else
            MessageBox.Show("Attempted to add matrices of incorrect size", "Incorrect Matrix dimensions for Addition", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            solutionMatrix.ErrorSoKill()
        End If
        Return solutionMatrix
    End Operator

    'matrix subtraction
    Shared Operator -(ByVal matrix1 As Matrix, ByVal matrix2 As Matrix) As Matrix
        Dim solutionMatrix As New Matrix(matrix1.nRows, matrix1.nCols)
        If matrix1.nRows = matrix2.nRows And matrix1.nCols = matrix2.nCols Then
            Dim i, j As Integer
            For i = 1 To matrix1.nRows
                For j = 1 To matrix1.nCols
                    solutionMatrix.data(i, j) = matrix1.data(i, j) - matrix2.data(i, j)
                Next
            Next
        Else
            MessageBox.Show("Attempted to subtract matrices of incorrect size", "Incorrect Matrix dimensions for Subtraction", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            solutionMatrix.ErrorSoKill()
        End If
        Return solutionMatrix
    End Operator

    'scalar multiplication
    Shared Operator *(ByVal scalar As Decimal, ByVal matrix1 As Matrix) As Matrix
        Dim solutionMatrix As New Matrix(matrix1.nRows, matrix1.nCols)
        Dim i, j As Integer

        For i = 1 To solutionMatrix.nRows
            For j = 1 To solutionMatrix.nCols
                solutionMatrix.data(i, j) = scalar * matrix1.data(i, j)
            Next
        Next
        Return solutionMatrix
    End Operator

    'scalar division
    Shared Operator /(ByVal matrix1 As Matrix, ByVal scalar As Decimal) As Matrix
        Dim solutionMatrix As New Matrix(matrix1.nRows, matrix1.nCols)
        Dim i, j As Integer

        If scalar <> 0 Then
            For i = 1 To solutionMatrix.nRows
                For j = 1 To solutionMatrix.nCols
                    solutionMatrix.data(i, j) = matrix1.data(i, j) / scalar
                Next
            Next
        Else
            MessageBox.Show("Attempted to divide a matrix by scalar zero", "Divide by Zero Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            solutionMatrix.ErrorSoKill()
        End If
        Return solutionMatrix
    End Operator

    'set a matrix to Identity matrix
    Public Function makeIdentity() As Matrix
        Dim solutionMatrix As New Matrix(Me.nRows, Me.nCols)
        Dim i, j As Integer
        For i = 1 To solutionMatrix.nRows
            For j = 1 To solutionMatrix.nCols
                If i = j Then
                    solutionMatrix.data(i, j) = 1
                Else
                    solutionMatrix.data(i, j) = 0
                End If
            Next
        Next
        Return solutionMatrix
    End Function

    'resets (clears) all the elements of a matrix
    Public Function clear() As Matrix
        Dim solutionmatrix As New Matrix(Me.nRows, Me.nCols)
        Return solutionmatrix
    End Function

    'matrix inversion by Gaussian elimination
    Public Function Inverse() As Matrix
        Dim solutionMatrix As New Matrix(Me.nRows, (Me.nCols * 2I))
        Dim inverseMatrix As New Matrix(Me.nRows, Me.nCols)
        Dim boolTest As Boolean = True

        If Me.nRows <> Me.nCols Then
            MessageBox.Show("Attempted to Inverse a matrix which is NOT square", "Matrix Inverse Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            ErrorSoKill()
            boolTest = False
        End If

        If boolTest Then
            Dim i, j As Integer
            For i = 0 To Me.nRows - 1
                For j = 0 To Me.nCols - 1
                    solutionMatrix.data(i + 1, j + 1) = Me.data(i + 1, j + 1)
                Next
            Next

            j = 0
            For i = Me.nCols To solutionMatrix.nCols - 1
                solutionMatrix.data(j + 1, i + 1) = 1D
                j += 1
            Next
            Dim t As Integer
            For i = 0 To Me.nRows - 1
                If solutionMatrix.data(i + 1, i + 1) = 0 Then
                    t = i + 1
                    While (t < Me.nRows AndAlso solutionMatrix.data(t + 1, i + 1) = 0)
                        t += 1
                    End While

                    If t = Me.nRows Then
                        'MessageBox.Show("Attempted to Inverse a matrix which is singular", "Matrix Inverse Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                        'ErrorSoKill()
                        boolTest = False
                    End If

                    If boolTest Then
                        For j = i To solutionMatrix.nCols - 1
                            solutionMatrix.data(i + 1, j + 1) += solutionMatrix.data(t + 1, j + 1) / solutionMatrix.data(t + 1, i + 1)
                        Next
                    End If
                End If
                Dim temp As Decimal = solutionMatrix.data(i + 1, i + 1)
                For j = i To solutionMatrix.nCols - 1
                    solutionMatrix.data(i + 1, j + 1) /= temp
                Next

                For t = 0 To Me.nRows - 1
                    If t <> i Then
                        temp = solutionMatrix.data(t + 1, i + 1)
                        For j = i To solutionMatrix.nCols - 1
                            solutionMatrix.data(t + 1, j + 1) -= temp / solutionMatrix.data(i + 1, i + 1) * solutionMatrix.data(i + 1, j + 1)
                        Next
                    End If
                Next
            Next

            Dim q As Integer
            For i = 0 To inverseMatrix.nRows - 1
                q = 0
                For j = inverseMatrix.nCols To solutionMatrix.nCols - 1
                    inverseMatrix.data(i + 1, q + 1) = solutionMatrix.data(i + 1, j + 1)
                    q += 1
                Next
            Next
        End If
        Return inverseMatrix
    End Function

    'matrix transpose
    Public Function Transpose() As Matrix
        Dim i, j As Integer
        Dim solutionMatrix As New Matrix(Me.nCols, Me.nRows)
        For i = 1 To Me.nRows
            For j = 1 To Me.nCols
                solutionMatrix.data(j, i) = Me.data(i, j)
            Next
        Next
        Return solutionMatrix
    End Function

    '1x1 matrix to scalar decimal
    Public Function toScalar() As Decimal
        Dim result As Decimal
        If Me.nRows = 1 And Me.nCols = 1 Then
            result = Me.data(1, 1)
        Else
            MessageBox.Show("Attempted to convert a non 1x1 matrix to a scalar", "Matrix --> Scalar Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            ErrorSoKill()
        End If
        Return result
    End Function

    'get the diagonal elements of a square matrix (handy for returning variance values from a V-C matrix in least squares)
    Public Function getDiagonal() As Matrix
        Dim solutionMatrix As New Matrix(Me.nRows, 1)
        If Me.nRows = Me.nCols Then
            Dim i As Integer
            For i = 1 To Me.nRows
                solutionMatrix.data(i, 1) = Me.data(i, i)
            Next
        Else
            MessageBox.Show("Attempted to get the diagonal elements of a non square matrix", "Get Diagonal Elements Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            ErrorSoKill()
        End If
        Return solutionMatrix
    End Function

    'takes the square root of the absolute value (positive always) of each element within a matrix 
    Public Function Sqrt() As Matrix
        Dim solutionMatrix As New Matrix(Me.nRows, Me.nCols)
        Dim i, j As Integer
        For i = 1 To Me.nRows
            For j = 1 To Me.nCols
                solutionMatrix.data(i, j) = Math.Sqrt(Math.Abs(Me.data(i, j)))
            Next
        Next
        Return solutionMatrix
    End Function

    'matrix re-dimensioning function, option to preserve the data already inside a Matrix and just make it a different size or reset all the data = 0 
    Public Function matrixReDim(ByVal rows As Integer, ByVal cols As Integer, Optional ByVal Preserve As Boolean = False) As Matrix
        Dim solutionMatrix As New Matrix(rows, cols)
        Dim i, j As Integer
        If Preserve = True Then
            For i = 1 To rows
                For j = 1 To cols
                    If i <= Me.nRows And j <= Me.nCols Then
                        solutionMatrix.data(i, j) = Me.data(i, j)
                    End If
                Next
            Next
        End If
        Return solutionMatrix
    End Function

    'get an entire row of a matrix
    Public Function getRow(ByVal row As Integer) As Matrix
        Dim solutionMatrix As New Matrix(1, Me.nCols)
        Dim i As Integer
        If row > 0 And row <= Me.nRows Then
            For i = 1 To Me.nCols
                solutionMatrix.data(1, i) = Me.data(row, i)
            Next
        Else
            MessageBox.Show("Attempted to get a row that does not exist", "Get Row Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            ErrorSoKill()
        End If
        Return solutionMatrix
    End Function

    'get an entire column of a matrix
    Public Function getColumn(ByVal column As Integer) As Matrix
        Dim solutionMatrix As New Matrix(Me.nRows, 1)
        Dim i As Integer
        If column > 0 And column <= Me.nCols Then
            For i = 1 To Me.nRows
                solutionMatrix.data(i, 1) = Me.data(i, column)
            Next
        Else
            MessageBox.Show("Attempted to get a column that does not exist", "Get Column Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            ErrorSoKill()
        End If
        Return solutionMatrix
    End Function

    'error handling function, kills the execution of the program
    Private Sub ErrorSoKill()
        End
    End Sub

    'matrix printing
    Public Sub printAll(Optional ByVal matrixName As String = "", Optional ByVal formatOutput As Boolean = False, Optional ByVal decimals As Integer = 0)
        Dim message As String = String.Empty
        Dim i, j As Integer

        For i = 1 To nRowsV
            For j = 1 To nColsV
                If formatOutput = True Then
                    message &= (Decimal.Round(data(i, j), decimals)).ToString & ControlChars.Tab
                Else
                    message &= data(i, j).ToString & ControlChars.Tab
                End If
            Next
            message &= ControlChars.NewLine
        Next
        MessageBox.Show(message, matrixName)
    End Sub
End Class
