Imports Logica.AccesoLogica
Imports DevComponents.DotNetBar
Public Class Pr_HistorialProductos
    Dim _Inter As Integer = 0

    'gb_FacturaIncluirICE

    Public _nameButton As String
    Public _tab As SuperTabItem

    Public Sub _prIniciarTodo()
        tbFechaI.Value = Now.Date
        tbFechaF.Value = Now.Date
        _PMIniciarTodo()
        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        Me.Text = "REPORTE VENTAS VS COSTOS"
        MReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
        _IniciarComponentes()
        _prCargarComboLibreriaProducto(cbProducto)
    End Sub
    Public Sub _IniciarComponentes()
        tbAlmacen.ReadOnly = True
        tbAlmacen.Enabled = False
        CheckTodosAlmacen.CheckValue = True
        CheckTodosProducto.Checked = True

    End Sub
    Private Sub _prCargarComboLibreriaProducto(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_prObtenerProductos()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("idProducto").Width = 60
            .DropDownList.Columns("idProducto").Caption = "ITEM"
            .DropDownList.Columns.Add("Descripcion").Width = 500
            .DropDownList.Columns("Descripcion").Caption = "PRODUCTO"
            .ValueMember = "idProducto"
            .DisplayMember = "Descripcion"
            .DataSource = dt
            .Refresh()
        End With
    End Sub
    Public Sub _prInterpretarDatos(ByRef _dt As DataTable)
        Dim fechaDesde As DateTime = tbFechaI.Value.ToString("dd/MM/yyyy")
        Dim fechaHasta As DateTime = tbFechaF.Value.ToString("dd/MM/yyyy")
        Dim idproducto As Integer = 0

        If cbProducto.SelectedIndex <> -1 Then idproducto = cbProducto.Value

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
            cbProducto.Enabled = True
            cbProducto.BackColor = Color.Gainsboro
            cbProducto.ReadOnly = True
            cbProducto.SelectedIndex = -1

        End If
    End Sub

    Private Sub CheckUnaProducto_CheckValueChanged(sender As Object, e As EventArgs) Handles CheckUnaProducto.CheckValueChanged
        If (CheckUnaProducto.Checked) Then
            CheckTodosProducto.CheckValue = False
            cbProducto.Enabled = True
            cbProducto.BackColor = Color.White
            cbProducto.Focus()
            cbProducto.ReadOnly = False
            If (CType(cbProducto.DataSource, DataTable).Rows.Count > 0) Then
                cbProducto.SelectedIndex = 0
            End If
        End If
    End Sub
End Class