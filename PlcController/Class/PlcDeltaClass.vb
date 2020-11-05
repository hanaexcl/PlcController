Imports System.IO.Ports
Imports System.Threading

Public Class PlcDeltaClass : Implements PlcInterface : Implements IDisposable
    Dim com As System.IO.Ports.SerialPort = Nothing
    Dim addressList As List(Of ADDRESSTABLE) = New List(Of ADDRESSTABLE)

    ''' <summary>
    ''' 通訊位置表
    ''' </summary>
    Structure ADDRESSTABLE
        Dim codeName As String '暫存器類型
        Dim begin As Integer   '初始位置
        Dim range As Integer   '範圍
    End Structure

    ''' <summary>
    ''' 初始化通訊位置表
    ''' </summary>
    Private Sub initializationAddressList()
        Dim tempAddressTable As ADDRESSTABLE

        addressList.Clear()

        tempAddressTable = New ADDRESSTABLE
        tempAddressTable.codeName = "X"
        tempAddressTable.begin = &H400
        tempAddressTable.range = 255
        addressList.Add(tempAddressTable)

        tempAddressTable = New ADDRESSTABLE
        tempAddressTable.codeName = "Y"
        tempAddressTable.begin = &H500
        tempAddressTable.range = 255
        addressList.Add(tempAddressTable)

        tempAddressTable = New ADDRESSTABLE
        tempAddressTable.codeName = "D"
        tempAddressTable.begin = &H1000
        tempAddressTable.range = 4095
        addressList.Add(tempAddressTable)

    End Sub

    ''' <summary>
    ''' 初始化物件
    ''' </summary>
    ''' <param name="Port">通訊埠</param>
    ''' <param name="BaudRate">鮑率</param>
    ''' <param name="Parity">同位元</param>
    ''' <param name="DataBits">資料長度</param>
    ''' <param name="StopBits">停止位元</param>
    Public Sub New(Port As String, BaudRate As Integer, Parity As Parity, DataBits As Integer, StopBits As StopBits)
        Try
            initializationAddressList()
            com = My.Computer.Ports.OpenSerialPort(Port)
            com.BaudRate = BaudRate
            com.Parity = Parity
            com.DataBits = DataBits
            com.StopBits = StopBits
            If Not com.IsOpen Then com.Open()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' 送出命令
    ''' </summary>
    ''' <param name="Slave">站號</param>
    ''' <param name="command">功能碼</param>
    ''' <param name="address">位址</param>
    ''' <param name="databits">資料位元</param>
    ''' <param name="waitBit">最低回應位元(可選)</param>
    ''' <returns>回應資料</returns>
    Function Send(Slave As String, command As String, address As String, databits As String, Optional data As String = "", Optional waitBit As Integer = 1) As String
        'Try
        Dim finallyCommand As String

        finallyCommand = Slave + command.ToUpper + address + databits.ToUpper
        finallyCommand = Chr(&H3A) + finallyCommand + CheckSum(finallyCommand) + vbCrLf

        com.Write(finallyCommand)

        Return WaitResponse(waitBit)
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Function

    ''' <summary>
    ''' 等待回應
    ''' </summary>
    ''' <param name="waitBit">最低回應位元(可選)</param>
    ''' <returns></returns>
    Function WaitResponse(Optional waitBit As Integer = 0) As String
        Do
            Thread.Sleep(10)
            Application.DoEvents()
            If com.BytesToRead >= waitBit Then Exit Do
        Loop
        Return com.ReadExisting()
    End Function

    ''' <summary>
    ''' 檢查碼
    ''' </summary>
    ''' <param name="str">處理字串</param>
    ''' <returns>檢查碼</returns>
    Function CheckSum(str As String) As String
        Dim count As Integer = &H0
        For i = 0 To str.Length - 1 Step 2
            count += Convert.ToInt16(str(i) & str(i + 1), 16)
        Next

        Return Strings.Right("0" + Hex(&HFF - count + 1), 2)
    End Function

    ''' <summary>
    ''' 讀取接點狀態
    ''' </summary>
    ''' <param name="Slave">站號</param>
    ''' <param name="str">接點名稱</param>
    ''' <returns>狀態</returns>
    Function ReadPoint(Slave As String, str As String) As Boolean
        Trace.Write(str)
        Dim result As String = Send(Slave, "02", NameToPoint(str), "0001")
        Trace.Write(result)
        'Mid(result, 8, 2) Convert.ToByte("A") Mod 2 = 1
        Return CInt("&H" & Mid(result, 8, 2)) Mod 2 = 1
        'Return True
    End Function

    ''' <summary>
    ''' 轉換接點對應位置
    ''' </summary>
    ''' <param name="str">接點名稱</param>
    ''' <returns>位置</returns>
    Function NameToPoint(str As String) As String
        Dim result As Integer = &H0
        Dim resultStr As String
        Dim tempTable As ADDRESSTABLE
        tempTable = addressList.Find(Function(x) x.codeName.ToUpper = Mid(str, 1, 1).ToUpper)
        result += tempTable.begin
        result += Convert.ToInt16(Mid(str, 2))
        resultStr = Conversion.Hex(result)
        If Len(resultStr) Mod 2 <> 0 Then resultStr = "0" & resultStr
        Return resultStr
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 偵測多餘的呼叫

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 處置 Managed 狀態 (Managed 物件)。
                com.Dispose()
            End If

            ' TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的 Finalize()。
            ' TODO: 將大型欄位設為 null。
        End If
        disposedValue = True
    End Sub

    ' TODO: 只有當上方的 Dispose(disposing As Boolean) 具有要釋放 Unmanaged 資源的程式碼時，才覆寫 Finalize()。
    'Protected Overrides Sub Finalize()
    '    ' 請勿變更這個程式碼。請將清除程式碼放在上方的 Dispose(disposing As Boolean) 中。
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Visual Basic 加入這個程式碼的目的，在於能正確地實作可處置的模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 請勿變更這個程式碼。請將清除程式碼放在上方的 Dispose(disposing As Boolean) 中。
        Dispose(True)
        ' TODO: 覆寫上列 Finalize() 時，取消下行的註解狀態。
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
