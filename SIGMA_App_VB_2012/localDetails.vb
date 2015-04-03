'''''SIGMA'''''
'By: Ryan Brazeal
'Date: April 2015
'GNU GPL V2 License
'www.rgbi.ca
'www.jrpi.ca

Public Class localDetails
    Private Sub closeButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles closeButton.Click
        Me.Close()
    End Sub

    Private Sub localDetails_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        Label1.Text = mainForm.localDetailsSet.latOrigin
        Label2.Text = mainForm.localDetailsSet.longOrigin
        Label3.Text = mainForm.localDetailsSet.ellhOrigin
        Label4.Text = mainForm.localDetailsSet.NOrigin
        Label5.Text = mainForm.localDetailsSet.EOrigin
        Label6.Text = mainForm.localDetailsSet.ElevOrigin
        Label7.Text = mainForm.localDetailsSet.scaleFactor
        Label8.Text = mainForm.localDetailsSet.elevFactor
        Label9.Text = mainForm.localDetailsSet.combFactor
        Label10.Text = mainForm.localDetailsSet.Norientation
        Label11.Text = mainForm.localDetailsSet.rotation
        Label12.Text = mainForm.localDetailsSet.projType
    End Sub
End Class