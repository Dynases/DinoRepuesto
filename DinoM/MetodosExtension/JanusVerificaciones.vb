Imports System.Drawing
Imports System.IO
Imports DevComponents.DotNetBar
Imports Janus.Windows
Imports Janus.Windows.GridEX
Imports Janus.Windows.GridEX.EditControls
Public Class JanusVerificaciones
    Public Shared Sub jVerificar_ExisteFIlaDatatable(tabla As DataTable)
        If tabla.Rows.Count = 0 Then
            Throw New Exception("No existe datos")
        End If
    End Sub
End Class
