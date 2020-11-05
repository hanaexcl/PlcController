Imports System.IO.Ports

Public Class Delta_ES2
    Public Port As String
    Public BaudRate As Integer
    Public Parity As Parity
    Public DataBits As Integer
    Public StopBits As StopBits
    Public Slave As String

    Dim Com As PlcDeltaClass

    Private Sub Delta_ES2_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        End
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Button1.Text = "連接" Then
            Try
                Com = New PlcDeltaClass(Port, BaudRate, Parity, DataBits, StopBits)
                Button1.Text = "中斷"
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        Else
            Try
                Com.Dispose()
                Button1.Text = "連接"
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Len(TextBox5.Text) = 1 Then TextBox5.Text = "0" & TextBox5.Text
        Dim result As String = Com.Send(Slave, "02", TextBox1.Text, TextBox5.Text)
        TextBox2.Text = result
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim result As String = Com.Send(Slave, "05", TextBox4.Text, TextBox3.Text)
        Trace.WriteLine(result)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        ListView1.Items.Clear()

        For i = 0 To 31
            Dim a As ListViewItem = New ListViewItem
            a.Checked = Com.ReadPoint(Slave, "Y" & i)
            a.Text = "Y" & ((i \ 8) * 10 + i Mod 8)
            ListView1.Items.Add(a)
        Next
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        ListView1.Items.Clear()

        For i = 0 To 31
            Dim a As ListViewItem = New ListViewItem
            a.Checked = Com.ReadPoint(Slave, "X" & i)
            a.Text = "X" & ((i \ 8) * 10 + i Mod 8)
            ListView1.Items.Add(a)
        Next

    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        TextBox7.Text = Com.ReadPoint(Slave, TextBox8.Text)
    End Sub
End Class
