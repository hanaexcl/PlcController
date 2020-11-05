Public Class Start
    Private Sub Start_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.Items.Clear()
        ComboBox1.Items.AddRange(My.Computer.Ports.SerialPortNames.ToArray)

        If ComboBox1.Items.Count = 0 Then
            MsgBox("未偵測到通訊埠")
            End
        End If

        '預設
        PlcType.SelectedIndex = 0
        ComboBox1.SelectedIndex = 1
        TextBox1.Text = "9600"
        ComboBox2.SelectedIndex = 2
        TextBox2.Text = "7"
        ComboBox3.SelectedIndex = 1
        TextBox3.Text = "1"
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Len(TextBox1.Text) = 0 Then
            MsgBox("請輸入" & "鮑率")
            Exit Sub
        End If

        If Len(TextBox2.Text) = 0 Then
            MsgBox("請輸入" & "資料長度")
            Exit Sub
        End If

        If Len(TextBox3.Text) = 0 Then
            MsgBox("請輸入" & "站號")
            Exit Sub
        End If

        If PlcType.Text = "台達" Then
            Dim temp As Delta_ES2 = New Delta_ES2
            temp.Port = ComboBox1.Text
            temp.BaudRate = TextBox1.Text
            temp.Parity = ComboBox2.SelectedIndex
            temp.DataBits = TextBox2.Text
            temp.StopBits = ComboBox3.SelectedIndex
            temp.Slave = TextBox3.Text
            If Len(temp.Slave) = 1 Then temp.Slave = "0" & temp.Slave
            temp.Show()
            Me.Dispose()
        End If
    End Sub
End Class