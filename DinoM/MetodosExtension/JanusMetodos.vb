Imports System.Drawing
Imports System.IO
Imports DevComponents.DotNetBar
Imports Janus.Windows
Imports Janus.Windows.GridEX
Imports Janus.Windows.GridEX.EditControls
Public Class JanusMetodos
    Public Shared Function jMetodo_SumarFila(grid As GridEX, key As String) As Double
        Dim stock As Double = 0
        grid.UpdateData()
        For Each fila As DataRow In CType(grid.DataSource, DataTable).Rows
            stock += fila(key)
        Next
        Return stock
    End Function
    Public Shared Sub jMetodo_SetValorGrillaCompleta(grid As GridEX, key As String, valor As String)
        For Each fila As DataRow In CType(grid.DataSource, DataTable).Rows
            fila(key) = valor
        Next
    End Sub
    Public Shared Function jMetodo_obtenerPrimerValor(grid As GridEX, key As String, keyValor As String) As Integer
        Dim valor = 0
        For Each fila As DataRow In CType(grid.DataSource, DataTable).Rows
            If fila(key) > 0 Then
                valor = fila(keyValor)
                Exit For
            End If
        Next
        Return valor
    End Function
End Class
