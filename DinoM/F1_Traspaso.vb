Imports DevComponents.DotNetBar.Controls
Imports DevComponents.DotNetBar
Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX
Public Class F1_Traspaso
    Public Stock As Decimal
    Public Cantidad As Decimal
    Public Producto As String
    Public banderaTraspaso As Boolean
    Public CategoriaPrecio As Integer = 0
    Public idProducto As Integer = 0
    Public Precio As Integer = 0
    Public dt As DataTable
    Public IdCompra As String

    Private Sub F_Cantidad_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        _prCargarComboLibreriaSucursal(cbSucDestino)
    End Sub

    Private Sub _prCargarComboLibreriaSucursal(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnListarSucursales()
        dt.Rows.RemoveAt(3)
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("aanumi").Width = 60
            .DropDownList.Columns("aanumi").Caption = "COD"
            .DropDownList.Columns.Add("aabdes").Width = 200
            .DropDownList.Columns("aabdes").Caption = "SUCURSAL"
            .ValueMember = "aanumi"
            .DisplayMember = "aabdes"
            .DataSource = dt
            .Refresh()
        End With
    End Sub



    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click

        If (cbSucDestino.SelectedIndex < 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Debe seleccionar una Sucursal de destino!!!", img, 3500, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Exit Sub
        End If

        Dim resultado As Boolean = False

        resultado = l_MovimientoGuardarTraspaso("", Now.Date.ToString("yyyy/MM/dd"), 6, "TRASPASO AUTOMÁTICO ENLAZADO A COMPRA NRO.: " + IdCompra, dt, cbSucOrigen.Value, cbSucDestino.Value,
                                                IdCompra)
        If resultado Then
            'MostrarMensajeOk("Traspaso realizado con éxito.".ToUpper)
            banderaTraspaso = True
            Me.Close()
        Else
            'MostrarMensajeError("El Traspaso no pudo ser insertado".ToUpper)
            banderaTraspaso = False
        End If


    End Sub
    Private Sub MostrarMensajeOk(mensaje As String)
        ToastNotification.Show(Me,
                               mensaje.ToUpper,
                               My.Resources.OK,
                               4000,
                               eToastGlowColor.Green,
                               eToastPosition.TopCenter)
    End Sub
    Private Sub MostrarMensajeError(mensaje As String)
        ToastNotification.Show(Me,
                               mensaje.ToUpper,
                               My.Resources.WARNING,
                               4000,
                               eToastGlowColor.Red,
                               eToastPosition.TopCenter)

    End Sub
    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        banderaTraspaso = False
        Me.Close()
    End Sub
End Class