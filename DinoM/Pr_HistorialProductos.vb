Imports Logica.AccesoLogica
Imports DevComponents.DotNetBar
Public Class Pr_HistorialProductos
    Dim _Inter As Integer = 0

    'gb_FacturaIncluirICE

    Public _nameButton As String
    Public _tab As SuperTabItem
    Dim dtProductoGoblal As DataTable
    Dim Lote As Boolean = False

    Public Sub _prIniciarTodo()
        tbFechaI.Value = Now.Date
        tbFechaF.Value = Now.Date
        _PMIniciarTodo()
        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        Me.Text = "REPORTE HISTORIAL DE PRECIOS"
        MReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
        _IniciarComponentes()

        _prValidarLote()
        tbproducto.Enabled = False
    End Sub
    Public Sub _IniciarComponentes()
        tbAlmacen.ReadOnly = True
        tbAlmacen.Enabled = False
        CheckTodosAlmacen.CheckValue = True
        CheckTodosProducto.Checked = True

    End Sub
    Public Sub _prValidarLote()
        Dim dt As DataTable = L_fnPorcUtilidad()
        If (dt.Rows.Count > 0) Then
            Dim lot As Integer = dt.Rows(0).Item("VerLote")
            If (lot = 1) Then
                Lote = True
            Else
                Lote = False
            End If

        End If
    End Sub

    Public Sub _prInterpretarDatos(ByRef _dt As DataTable)
        Dim fechaDesde As DateTime = tbFechaI.Value.ToString("dd/MM/yyyy")
        Dim fechaHasta As DateTime = tbFechaF.Value.ToString("dd/MM/yyyy")
        Dim idproducto As Integer = 0

        If tbCodigo.Text <> String.Empty Then idproducto = tbCodigo.Text

        _dt = L_prHistorialPreciosProductos(fechaDesde, fechaHasta, idproducto)

    End Sub
    Private Sub _prCargarReporte()
        Dim _dt As New DataTable
        _prInterpretarDatos(_dt)
        If (_dt.Rows.Count > 0) Then

            Dim objrep As New R_HistorialPreciosProductos
            objrep.SetDataSource(_dt)
            Dim fechaI As String = tbFechaI.Value.ToString("dd/MM/yyyy")
            Dim fechaF As String = tbFechaF.Value.ToString("dd/MM/yyyy")
            objrep.SetParameterValue("usuario", L_Usuario)
            objrep.SetParameterValue("fechaI", fechaI)
            objrep.SetParameterValue("fechaF", fechaF)
            MReportViewer.ReportSource = objrep
            MReportViewer.Show()
            MReportViewer.BringToFront()


        Else
            ToastNotification.Show(Me, "NO HAY DATOS PARA LOS PARAMETROS SELECCIONADOS..!!!",
                                       My.Resources.INFORMATION, 2000,
                                       eToastGlowColor.Blue,
                                       eToastPosition.BottomLeft)
            MReportViewer.ReportSource = Nothing
        End If





    End Sub
    Private Sub btnGenerar_Click(sender As Object, e As EventArgs) Handles btnGenerar.Click
        _prCargarReporte()

    End Sub

    Private Sub Pr_VentasAtendidas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()

    End Sub

    Private Sub CheckUnaALmacen_CheckValueChanged(sender As Object, e As EventArgs) Handles CheckUnaALmacen.CheckValueChanged
        If (CheckUnaALmacen.Checked) Then
            CheckTodosAlmacen.CheckValue = False
            tbAlmacen.Enabled = True
            tbAlmacen.BackColor = Color.White
            tbAlmacen.Focus()
            tbAlmacen.ReadOnly = False
            _prCargarComboLibreriaSucursal(tbAlmacen)
            If (CType(tbAlmacen.DataSource, DataTable).Rows.Count > 0) Then
                tbAlmacen.SelectedIndex = 0

            End If
        End If
    End Sub

    Private Sub CheckTodosAlmacen_CheckValueChanged(sender As Object, e As EventArgs) Handles CheckTodosAlmacen.CheckValueChanged
        If (CheckTodosAlmacen.Checked) Then
            CheckUnaALmacen.CheckValue = False
            tbAlmacen.Enabled = True
            tbAlmacen.BackColor = Color.Gainsboro
            tbAlmacen.ReadOnly = True
            _prCargarComboLibreriaSucursal(tbAlmacen)
            CType(tbAlmacen.DataSource, DataTable).Rows.Clear()
            tbAlmacen.SelectedIndex = -1

        End If
    End Sub

    Private Sub _prCargarComboLibreriaSucursal(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnListarSucursales()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("aanumi").Width = 60
            .DropDownList.Columns("aanumi").Caption = "COD"
            .DropDownList.Columns.Add("aabdes").Width = 500
            .DropDownList.Columns("aabdes").Caption = "SUCURSAL"
            .ValueMember = "aanumi"
            .DisplayMember = "aabdes"
            .DataSource = dt
            .Refresh()
        End With
    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click

        Me.Close()

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        _Inter = _Inter + 1
        If _Inter = 1 Then
            Me.WindowState = FormWindowState.Normal

        Else
            Me.Opacity = 100
            Timer1.Enabled = False
        End If
    End Sub

    Private Sub CheckTodosProducto_CheckValueChanged(sender As Object, e As EventArgs) Handles CheckTodosProducto.CheckValueChanged
        If (CheckTodosProducto.Checked) Then
            CheckUnaProducto.CheckValue = False
            tbCodigo.Clear()
            tbproducto.Clear()
            tbCodigo.BackColor = Color.Gainsboro
            tbproducto.BackColor = Color.Gainsboro
            tbCodigo.Enabled = False
            btnBuscar.Enabled = False
        End If
    End Sub

    Private Sub CheckUnaProducto_CheckValueChanged(sender As Object, e As EventArgs) Handles CheckUnaProducto.CheckValueChanged
        If (CheckUnaProducto.Checked) Then
            CheckTodosProducto.CheckValue = False
            tbCodigo.ReadOnly = True
            tbCodigo.BackColor = Color.White
            tbproducto.BackColor = Color.White
            btnBuscar.Enabled = True

        End If
    End Sub

    Private Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        Try
            _prCargarProductos()
        Catch ex As Exception
            tbCodigo.Clear()
            ToastNotification.Show(Me, "DEBE ELEGIR UN PRODUCTO..!!!",
                                       My.Resources.INFORMATION, 2000,
                                       eToastGlowColor.Blue,
                                       eToastPosition.BottomLeft)
        End Try

    End Sub
    Private Sub _prCargarProductos()
        Dim dtname As DataTable = L_fnNameLabel()
        'Obtiene la lista de productos
        If dtProductoGoblal Is Nothing Then
            If (Lote = True) Then
                dtProductoGoblal = L_prMovimientoListarProductosConLote(1)
                ' actualizarSaldoSinLote(dtProductoGoblal)
            Else
                dtProductoGoblal = L_prMovimientoListarProductos(1)
            End If
        End If
        Dim dtMovimiento As DataTable = dtProductoGoblal.Copy
        dtMovimiento.Rows.Clear()
        'Intancia vista 
        Dim frm As F0_DetalleMovimiento
        frm = New F0_DetalleMovimiento(dtProductoGoblal, dtMovimiento, dtname)

        'Envia valores de configuracion
        frm.lbConcepto.Text = ""
        frm.detalleSeleccionHabilitado = False
        frm.GroupPanel1.Visible = False
        frm.btnAgregar.Visible = False
        frm.ShowDialog()
        dtMovimiento.Rows.Clear()
        'Devuelve valores de seleccion
        'dtProductoGoblal.Columns.Item("Stock").
        dtProductoGoblal.Columns.Remove("ListaAlmacen")

        tbCodigo.Text = frm.pedidoId.ToString()
        tbproducto.Text = frm.producto.ToString()

    End Sub
End Class