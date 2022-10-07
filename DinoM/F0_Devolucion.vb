Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports GMap.NET.MapProviders
Imports GMap.NET
Imports GMap.NET.WindowsForms.Markers
Imports GMap.NET.WindowsForms
Imports GMap.NET.WindowsForms.ToolTips
Imports System.Drawing
Imports DevComponents.DotNetBar.Controls
Imports System.Threading
Imports System.Drawing.Text
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Drawing.Printing
Imports CrystalDecisions.Shared
Imports Facturacion

Public Class F0_Devolucion

#Region "Variables Globales"
    Dim _CodCliente As Integer = 0
    Dim _CodEmpleado As Integer = 0
    Dim _codeBar As Integer = 1
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
    Dim dtProductoGoblal As DataTable = Nothing

    Dim SucursalSeleccionada As Integer = 0
    Dim CategoriaPrecioSeleccionada As Integer = 0
    Dim _Inter As Integer = 0
    Dim MostrarDetalleDev As Integer = 1



#End Region

#Region "Metodos Privados"
    Private Sub _IniciarTodo()
        L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        MSuperTabControl.SelectedTabIndex = 0
        Me.WindowState = FormWindowState.Maximized

        _prCargarComboLibreriaSucursal(cbSucursal)

        _prCargarDevolucion()
        _prInhabiliitar()
        'grVentas.Focus()
        Me.Text = "DEVOLUCIONES VENTAS"
        Dim blah As New Bitmap(New Bitmap(My.Resources.compra), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico
        _prAsignarPermisos()
        _prCargarNameLabel()


        tbFechaVenta.IsInputReadOnly = True
        GroupPanel2.Visible = False
        GroupPanel3.Visible = True
    End Sub
    Public Sub _prCargarNameLabel()
        Dim dt As DataTable = L_fnNameLabel()
        If (dt.Rows.Count > 0) Then
            _codeBar = 1 'dt.Rows(0).Item("codeBar")
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

    Private Sub _prAsignarPermisos()

        Dim dtRolUsu As DataTable = L_prRolDetalleGeneral(gi_userRol, _nameButton)

        Dim show As Boolean = dtRolUsu.Rows(0).Item("ycshow")
        Dim add As Boolean = dtRolUsu.Rows(0).Item("ycadd")
        Dim modif As Boolean = dtRolUsu.Rows(0).Item("ycmod")
        Dim del As Boolean = dtRolUsu.Rows(0).Item("ycdel")

        If add = False Then
            btnNuevo.Visible = False
        End If
        If modif = False Then
            btnModificar.Visible = False
        End If
        If del = False Then
            btnEliminar.Visible = False
        End If
    End Sub
    Private Sub _prInhabiliitar()

        tbCodigo.ReadOnly = True
        tbIdVenta.ReadOnly = True
        tbCliente.ReadOnly = True
        tbVendedor.ReadOnly = True
        tbObservacion.ReadOnly = True
        'tbFechaVenta.IsInputReadOnly = True
        'tbFechaVenc.IsInputReadOnly = True
        tbFechaVenc.Enabled = False
        tbFechaDev.IsInputReadOnly = False
        tbFechaDev.Enabled = False
        swTipoVenta.IsReadOnly = True
        btnBuscarVenta.Enabled = False

        btnModificar.Enabled = True
        btnGrabar.Enabled = False
        btnNuevo.Enabled = True
        btnEliminar.Enabled = True


        tbtotal.IsInputReadOnly = True

        grVentas.Enabled = True
        PanelNavegacion.Enabled = True

        cbSucursal.ReadOnly = True


    End Sub
    Private Sub _prhabilitar()

        grVentas.Enabled = False
        tbObservacion.ReadOnly = False
        'tbFechaVenta.IsInputReadOnly = False
        'tbFechaVenc.IsInputReadOnly = False
        'tbFechaDev.IsInputReadOnly = True
        btnBuscarVenta.Enabled = True

        'swTipoVenta.IsReadOnly = False
        btnGrabar.Enabled = True

    End Sub
    Public Sub _prFiltrar()
        'cargo el buscador
        Dim _Mpos As Integer
        _prCargarDevolucion()
        If grDevolucion.RowCount > 0 Then
            _Mpos = 0
            grDevolucion.Row = _Mpos
        Else
            _Limpiar()
            LblPaginacion.Text = "0/0"
        End If
    End Sub
    Private Sub _Limpiar()

        tbCodigo.Clear()
        tbIdVenta.Clear()
        tbCliente.Clear()
        tbVendedor.Clear()
        tbObservacion.Clear()

        swTipoVenta.Value = True
        _CodCliente = 0
        _CodEmpleado = 0
        tbFechaVenta.Value = Now.Date
        tbFechaVenc.Value = Now.Date
        tbFechaDev.Value = Now.Date
        tbFechaVenc.Visible = False
        lbCredito.Visible = False
        MostrarDetalleDev = 1
        _prCargarDetalleVenta(-1)
        MSuperTabControl.SelectedTabIndex = 0

        tbtotal.Value = 0




        If (gi_userSuc > 0) Then
            Dim dt As DataTable = CType(cbSucursal.DataSource, DataTable)
            For i As Integer = 0 To dt.Rows.Count - 1 Step 1

                If (dt.Rows(i).Item("aanumi") = gi_userSuc) Then
                    cbSucursal.SelectedIndex = i
                End If

            Next
        End If
    End Sub
    Public Sub _prMostrarRegistro(_N As Integer)


        With grDevolucion
            tbCodigo.Text = .GetValue("dbnumi")
            tbIdVenta.Text = .GetValue("dbtanumi")
            tbCliente.Text = .GetValue("cliente")
            tbVendedor.Text = .GetValue("vendedor")
            tbFechaVenta.Value = .GetValue("tafdoc")
            cbSucursal.Value = .GetValue("taalm")
            swTipoVenta.Value = .GetValue("tatven")
            tbFechaVenc.Value = .GetValue("tafvcr")
            tbFechaDev.Value = .GetValue("dbfdev")
            tbObservacion.Text = .GetValue("dbobs")

            lbFecha.Text = CType(.GetValue("dbfact"), Date).ToString("dd/MM/yyyy")
            lbHora.Text = .GetValue("dbhact").ToString
            lbUsuario.Text = .GetValue("dbuact").ToString

        End With

        MostrarDetalleDev = 1
        _prCargarDetalleVenta(tbCodigo.Text)

        _prCalcularPrecioTotal()

        LblPaginacion.Text = Str(grDevolucion.Row + 1) + "/" + grDevolucion.RowCount.ToString

    End Sub

    Private Sub _prCargarDetalleVenta(_numi As String)
        Dim dt As New DataTable
        If MostrarDetalleDev = 1 Then
            dt = L_fnDetalleDev(_numi)
        Else
            dt = L_fnDetalleVentaDev(_numi)
        End If

        grdetalle.DataSource = dt
        grdetalle.RetrieveStructure()
        grdetalle.AlternatingColors = True


        With grdetalle.RootTable.Columns("tbnumi")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = False
        End With

        With grdetalle.RootTable.Columns("tbtv1numi")
            .Width = 90
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("tbty5prod")
            .Width = 90
            .Visible = False
        End With

        With grdetalle.RootTable.Columns("Item")
            .Caption = "Item"
            .Width = 90
            .Visible = True
        End With
        With grdetalle.RootTable.Columns("yfcbarra")
            .Caption = "C.Barra"
            .Width = 90
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("CodigoFabrica")
            .Caption = "Cod.Fabrica"
            .Width = 120
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
            .AllowSort = False
        End With
        With grdetalle.RootTable.Columns("CodigoMarca")
            .Caption = "Cod.Marca"
            .Width = 120
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
            .AllowSort = False
        End With
        With grdetalle.RootTable.Columns("Medida")
            .Caption = "Medida"
            .Width = 90
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
            .AllowSort = False
        End With

        With grdetalle.RootTable.Columns("CategoriaProducto")
            .Caption = "Cat.Producto"
            .Width = 100
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = False
            .AllowSort = False

        End With
        With grdetalle.RootTable.Columns("Marca")
            .Caption = "Marca"
            .Width = 100
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
            .AllowSort = False

        End With
        With grdetalle.RootTable.Columns("Procedencia")
            .Caption = "Procedencia"
            .Width = 100
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
            .AllowSort = False
        End With
        With grdetalle.RootTable.Columns("producto")
            .Caption = "Descripción"
            .Width = 320
            .MaxLines = 200
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
            .AllowSort = False

        End With
        With grdetalle.RootTable.Columns("tbest")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("tbPrecioReferencia")
            .Width = 85
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0.00"
            .Caption = "Pre. Fact."
            .AllowSort = False
        End With
        With grdetalle.RootTable.Columns("tbPorcentajeReferencia")
            .Width = 60
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0.00"
            .Caption = "% Dif."
            .AllowSort = False
        End With

        With grdetalle.RootTable.Columns("tbcmin")
            .Width = 75
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad"
            .AllowSort = False
        End With
        With grdetalle.RootTable.Columns("Devolucion")
            .Width = 75
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Devolucion"
            .AllowSort = False
        End With
        With grdetalle.RootTable.Columns("tbumin")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("unidad")
            .Width = 70
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .Caption = "Unidad"
        End With
        With grdetalle.RootTable.Columns("tbpbas")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Precio U."
            .AllowSort = False
        End With
        With grdetalle.RootTable.Columns("tbptot")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .AllowSort = False
            .Caption = "Sub Total"
        End With
        With grdetalle.RootTable.Columns("tbporc")
            .Width = 80
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "P.Desc(%)"
            .AllowSort = False
        End With
        With grdetalle.RootTable.Columns("tbdesc")
            .Width = 80
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "M.Desc"
            .AllowSort = False
        End With
        With grdetalle.RootTable.Columns("tbtotdesc")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Total"
            .AllowSort = False
        End With
        With grdetalle.RootTable.Columns("TotalDev")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Total Devolución"
            .AllowSort = False
        End With
        With grdetalle.RootTable.Columns("tbobs")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("tbpcos")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("tblote")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
            .Caption = "LOTE"
            .AllowSort = False
        End With
        With grdetalle.RootTable.Columns("tbfechaVenc")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
            .Caption = "FECHA VENC."
            .FormatString = "yyyy/MM/dd"
        End With
        With grdetalle.RootTable.Columns("tbptot2")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("tbfact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("tbhact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("tbuact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("estado")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With



        With grdetalle
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .RecordNavigator = True

        End With
    End Sub


    Private Sub _prCargarVenta()
        Dim dt As New DataTable
        dt = L_fnGeneralVentaDev(gi_userSuc)
        grVentas.DataSource = dt
        grVentas.RetrieveStructure()
        grVentas.AlternatingColors = True

        With grVentas.RootTable.Columns("tanumi")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = True
        End With
        With grVentas.RootTable.Columns("taalm")
            .Width = 90
            .Visible = False
        End With
        With grVentas.RootTable.Columns("taCatPrecio")
            .Width = 90
            .Visible = False
        End With
        With grVentas.RootTable.Columns("taproforma")
            .Width = 90
            .Visible = False
        End With
        With grVentas.RootTable.Columns("tafdoc")
            .Width = 90
            .Visible = True
            .Caption = "FECHA"
        End With
        With grVentas.RootTable.Columns("taven")
            .Width = 160
            .Visible = False
        End With
        With grVentas.RootTable.Columns("vendedor")
            .Width = 250
            .Visible = True
            .Caption = "VENDEDOR".ToUpper
        End With
        With grVentas.RootTable.Columns("tatven")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("tafvcr")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("taclpr")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("cliente")
            .Width = 250
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "CLIENTE"
        End With
        With grVentas.RootTable.Columns("tamon")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("moneda")
            .Width = 150
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "MONEDA"
        End With
        With grVentas.RootTable.Columns("taobs")
            .Width = 200
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "OBSERVACION"
        End With
        With grVentas.RootTable.Columns("tadesc")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("taest")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("taice")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("tafact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("tahact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("tauact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("total")
            .Width = 150
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "TOTAL"
            .FormatString = "0.00"
        End With
        With grVentas
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
        End With

        'If (dt.Rows.Count <= 0) Then
        '    _prCargarDetalleVenta(-1)
        'End If
    End Sub

    Private Sub _prCargarDevolucion()
        Dim dt As New DataTable
        dt = L_fnGeneraDevolucion(gi_userSuc)
        grDevolucion.DataSource = dt
        grDevolucion.RetrieveStructure()
        grDevolucion.AlternatingColors = True

        With grDevolucion.RootTable.Columns("dbnumi")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = True
        End With
        With grDevolucion.RootTable.Columns("dbtanumi")
            .Width = 90
            .Visible = True
            .Caption = "ID VENTA"
        End With
        With grDevolucion.RootTable.Columns("dbfdev")
            .Width = 90
            .Visible = True
            .Caption = "FECHA"
        End With
        With grDevolucion.RootTable.Columns("dbobs")
            .Width = 200
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "OBSERVACION"
        End With
        With grDevolucion.RootTable.Columns("dbtotal")
            .Width = 150
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "TOTAL"
            .FormatString = "0.00"
        End With
        With grDevolucion.RootTable.Columns("taalm")
            .Width = 90
            .Visible = False
        End With
        With grDevolucion.RootTable.Columns("tafdoc")
            .Width = 160
            .Visible = False
        End With

        With grDevolucion.RootTable.Columns("vendedor")
            .Width = 250
            .Visible = False
            .Caption = "VENDEDOR".ToUpper
        End With
        With grDevolucion.RootTable.Columns("tatven")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grDevolucion.RootTable.Columns("tafvcr")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grDevolucion.RootTable.Columns("taclpr")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grDevolucion.RootTable.Columns("cliente")
            .Width = 250
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "CLIENTE"
        End With
        With grDevolucion.RootTable.Columns("taCatPrecio")
            .Width = 90
            .Visible = False
        End With

        With grDevolucion.RootTable.Columns("dbfact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grDevolucion.RootTable.Columns("dbhact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grDevolucion.RootTable.Columns("dbuact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grDevolucion
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
        End With

        'If (dt.Rows.Count <= 0) Then
        '    _prCargarDetalleVenta(-1)
        'End If
    End Sub





    Public Function _fnAccesible()
        Return tbObservacion.ReadOnly = False
    End Function

    Public Sub _fnObtenerFilaDetalle(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("tbnumi")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next

    End Sub

    Public Sub P_PonerTotal(rowIndex As Integer)
        If (rowIndex < grdetalle.RowCount) Then

            Dim lin As Integer = grdetalle.GetValue("tbnumi")
            Dim pos As Integer = -1
            _fnObtenerFilaDetalle(pos, lin)
            Dim cant As Double = grdetalle.GetValue("Devolucion")
            Dim uni As Double = grdetalle.GetValue("tbpbas")
            Dim cos As Double = grdetalle.GetValue("tbpcos")
            Dim MontoDesc As Double = grdetalle.GetValue("tbdesc") / grdetalle.GetValue("tbcmin")
            Dim dt As DataTable = CType(grdetalle.DataSource, DataTable)
            If (pos >= 0) Then
                Dim TotalUnitario As Double = cant * uni
                Dim TotalDev As Double = TotalUnitario - MontoDesc
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("TotalDev") = TotalDev
                grdetalle.SetValue("TotalDev", TotalDev)

                'Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado")
                'If (estado = 1) Then
                '    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                'End If
            End If
            _prCalcularPrecioTotal()
        End If


    End Sub
    Public Sub _prCalcularPrecioTotal()

        tbtotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("TotalDev"), AggregateFunction.Sum)

    End Sub

    Public Function _ValidarCampos() As Boolean

        If (tbtotal.Value = 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No puede grabar sin especificar por lo menos en un producto la cantidad que se esta devolviendo, verificar".ToUpper, img, 3500, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Return False
        End If

        If (swTipoVenta.Value = False) Then
            Dim dt As DataTable = L_fnVerificarCreditos(tbIdVenta.Text)
            If dt.Rows(0).Item("Debe") = 0 Then
                Dim ef = New Efecto
                ef.tipo = 2
                ef.Context = "¿esta seguro que quiere hacer la devolución?".ToUpper
                ef.Header = "Los pagos de este crédito están cancelados en su totalidad.".ToUpper
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then
                    Return True
                Else
                    Return False
                End If
            Else
                If tbtotal.Value > dt.Rows(0).Item("Debe") Then
                    Dim ef = New Efecto
                    ef.tipo = 2
                    ef.Context = "¿esta seguro que quiere hacer la devolución?".ToUpper
                    ef.Header = "Ya existe pagos realizados y además el total de devolución es mayor al monto que falta pagar.".ToUpper
                    ef.ShowDialog()
                    Dim bandera As Boolean = False
                    bandera = ef.band
                    If (bandera = True) Then
                        Return True
                    Else
                        Return False

                    End If
                End If
            End If
        End If



        'If (grdetalle.RowCount = 1) Then
        '    grdetalle.Row = grdetalle.RowCount - 1
        '    If (grdetalle.GetValue("tbty5prod") = 0) Then
        '        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
        '        ToastNotification.Show(Me, "Por Favor Seleccione  un detalle de producto".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
        '        Return False
        '    End If
        'End If



        Return True
    End Function



    Public Sub _GuardarNuevo()

        Dim mensaje As String = ""

        Dim numi As String = ""

        Dim res As Boolean = L_fnGrabarDevolucion(numi, tbIdVenta.Text, tbFechaDev.Value.ToString("yyyy/MM/dd"), IIf(swTipoVenta.Value = True, 1, 0), cbSucursal.Value,
                                                  tbObservacion.Text, tbtotal.Value, CType(grdetalle.DataSource, DataTable))

        If res Then


            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Devolución ".ToUpper + tbCodigo.Text + " Grabada con éxito.".ToUpper,
                                      img, 4500,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )
            '_prImiprimirNotaVenta(numi)

            _prCargarDevolucion()

            _Limpiar()


            GroupPanel2.Visible = False
            GroupPanel3.Visible = True

        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La Venta no pudo ser insertado".ToUpper, img, 4500, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If

    End Sub
    Private Sub _prInsertarMontoNuevo(ByRef tabla As DataTable)
        'tabla.Rows.Add(0, tbMontoBs.Value, tbMontoDolar.Value, tbMontoTarej.Value, cbCambioDolar.Text, 0)
    End Sub
    Public Sub _prImiprimirNotaVenta(numi As String)
        Dim ef = New Efecto


        ef.tipo = 2
        ef.Context = "MENSAJE PRINCIPAL".ToUpper
        ef.Header = "¿desea imprimir la nota de venta?".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_GenerarReporte(numi)
        End If
    End Sub
    Public Sub _prImiprimirFacturaPreimpresa(numi As String)
        Dim ef = New Efecto


        ef.tipo = 2
        ef.Context = "MENSAJE PRINCIPAL".ToUpper
        ef.Header = "¿desea imprimir la factura Preimpresa?".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            'P_GenerarReporteFactura(numi)
        End If
    End Sub
    Private Sub _prGuardarModificado()
        Dim res As Boolean '= L_fnModificarVenta(tbCodigo.Text, tbFechaVenta.Value.ToString("yyyy/MM/dd"), _CodEmpleado, IIf(swTipoVenta.Value = True, 1, 0), IIf(swTipoVenta.Value = True, Now.Date.ToString("yyyy/MM/dd"), tbFechaVenc.Value.ToString("yyyy/MM/dd")), _CodCliente, IIf(swMoneda.Value = True, 1, 0), tbObservacion.Text, tbMdesc.Value, tbIce.Value, tbtotal.Value, CType(grdetalle.DataSource, DataTable), cbSucursal.Value, IIf(SwProforma.Value = True, tbProforma.Text, 0), cbPrecio.Value)
        If res Then

            'If (gb_FacturaEmite) Then
            '    L_fnEliminarDatos("TFV001", "fvanumi=" + tbCodigo.Text.Trim)
            '    L_fnEliminarDatos("TFV0011", "fvbnumi=" + tbCodigo.Text.Trim)
            '    P_fnGenerarFactura(tbCodigo.Text.Trim)
            'End If
            _prImiprimirNotaVenta(tbCodigo.Text)

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Venta ".ToUpper + tbCodigo.Text + " Modificado con Exito.".ToUpper,
                                      img, 4500,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )


            _prCargarVenta()

            _prSalir()


        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La Venta no pudo ser Modificada".ToUpper, img, 4500, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If
    End Sub
    Private Sub _prSalir()

        If btnGrabar.Enabled = True Then
            Dim ef = New Efecto
            ef.tipo = 2
            ef.Header = "¿Los Datos No Se Guardaron Debe Hacer Clic en el Boton Grabar. En Caso de Que no Quiera Guardarlo Confirme Este Mensaje?".ToUpper
            ef.Context = "mensaje principal".ToUpper
            ef.ShowDialog()
            Dim bandera As Boolean = False
            bandera = ef.band
            If (bandera = True) Then
                _prInhabiliitar()
                If grDevolucion.RowCount > 0 Then

                    _prMostrarRegistro(0)
                    GroupPanel2.Visible = False
                    GroupPanel3.Visible = True


                End If
            End If
        Else
            _modulo.Select()
            Close()

        End If



    End Sub

    Public Sub _PrimerRegistro()
        Dim _MPos As Integer
        If grDevolucion.RowCount > 0 Then
            _MPos = 0
            ''   _prMostrarRegistro(_MPos)
            grDevolucion.Row = _MPos
        End If
    End Sub




    'Private Sub P_prImprimirFacturar(numi As String, impFactura As Boolean, grabarPDF As Boolean)
    '    Dim _Fecha, _FechaAl As Date
    '    Dim _Ds, _Ds1, _Ds2, _Ds3 As New DataSet
    '    Dim _Autorizacion, _Nit, _Fechainv, _Total, _Key, _Cod_Control, _Hora,
    '        _Literal, _TotalDecimal, _TotalDecimal2 As String
    '    Dim I, _NumFac, _numidosif, _TotalCC As Integer
    '    Dim ice, _Desc, _TotalLi As Decimal
    '    Dim _VistaPrevia As Integer = 0


    '    _Desc = CDbl(tbMdesc.Value)
    '    If Not IsNothing(P_Global.Visualizador) Then
    '        P_Global.Visualizador.Close()
    '    End If

    '    _Fecha = Now.Date '.ToString("dd/MM/yyyy")
    '    _Hora = Now.Hour.ToString + ":" + Now.Minute.ToString
    '    _Ds1 = L_Dosificacion("1", "1", _Fecha)

    '    _Ds = L_Reporte_Factura(numi, numi)
    '    _Autorizacion = _Ds1.Tables(0).Rows(0).Item("sbautoriz").ToString
    '    _NumFac = CInt(_Ds1.Tables(0).Rows(0).Item("sbnfac")) + 1
    '    _Nit = _Ds.Tables(0).Rows(0).Item("fvanitcli").ToString
    '    _Fechainv = Microsoft.VisualBasic.Right(_Fecha.ToShortDateString, 4) +
    '                Microsoft.VisualBasic.Right(Microsoft.VisualBasic.Left(_Fecha.ToShortDateString, 5), 2) +
    '                Microsoft.VisualBasic.Left(_Fecha.ToShortDateString, 2)
    '    _Total = _Ds.Tables(0).Rows(0).Item("fvatotal").ToString
    '    ice = _Ds.Tables(0).Rows(0).Item("fvaimpsi")
    '    _numidosif = _Ds1.Tables(0).Rows(0).Item("sbnumi").ToString
    '    _Key = _Ds1.Tables(0).Rows(0).Item("sbkey")
    '    _FechaAl = _Ds1.Tables(0).Rows(0).Item("sbfal")

    '    Dim maxNFac As Integer = L_fnObtenerMaxIdTabla("TFV001", "fvanfac", "fvaautoriz = " + _Autorizacion)
    '    _NumFac = maxNFac + 1

    '    _TotalCC = Math.Round(CDbl(_Total), MidpointRounding.AwayFromZero)
    '    _Cod_Control = ControlCode.generateControlCode(_Autorizacion, _NumFac, _Nit, _Fechainv, CStr(_TotalCC), _Key)

    '    'Literal 
    '    _TotalLi = _Ds.Tables(0).Rows(0).Item("fvastot") - _Ds.Tables(0).Rows(0).Item("fvadesc")
    '    _TotalDecimal = _TotalLi - Math.Truncate(_TotalLi)
    '    _TotalDecimal2 = CDbl(_TotalDecimal) * 100

    '    'Dim li As String = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(_Total) - CDbl(_TotalDecimal)) + " con " + IIf(_TotalDecimal2.Equals("0"), "00", _TotalDecimal2) + "/100 Bolivianos"
    '    _Literal = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(_TotalLi) - CDbl(_TotalDecimal)) + " con " + IIf(_TotalDecimal2.Equals("0"), "00", _TotalDecimal2) + "/100 Bolivianos"
    '    _Ds2 = L_Reporte_Factura_Cia("1")
    '    QrFactura.Text = _Ds2.Tables(0).Rows(0).Item("scnit").ToString + "|" + Str(_NumFac).Trim + "|" + _Autorizacion + "|" + _Fecha + "|" + _Total + "|" + _TotalLi.ToString + "|" + _Cod_Control + "|" + TbNit.Text.Trim + "|" + ice.ToString + "|0|0|" + Str(_Desc).Trim

    '    L_Modificar_Factura("fvanumi = " + CStr(numi),
    '                        "",
    '                        CStr(_NumFac),
    '                        CStr(_Autorizacion),
    '                        "",
    '                        "",
    '                        "",
    '                        "",
    '                        "",
    '                        "",
    '                        "",
    '                        "",
    '                        "",
    '                        "",
    '                        "",
    '                        "",
    '                        "",
    '                        _Cod_Control,
    '                        _FechaAl.ToString("yyyy/MM/dd"),
    '                        "",
    '                        "",
    '                        CStr(numi))

    '    _Ds = L_Reporte_Factura(numi, numi)

    '    For I = 0 To _Ds.Tables(0).Rows.Count - 1
    '        _Ds.Tables(0).Rows(I).Item("fvaimgqr") = P_fnImageToByteArray(QrFactura.Image)
    '    Next
    '    If (impFactura) Then
    '        _Ds3 = L_ObtenerRutaImpresora("1") ' Datos de Impresion de Facturación
    '        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
    '            P_Global.Visualizador = New Visualizador 'Comentar
    '        End If
    '        Dim objrep As Object = Nothing
    '        If (gi_FacturaTipo = 1) Then
    '            'objrep = New R_FacturaG
    '        ElseIf (gi_FacturaTipo = 2) Then
    '            objrep = New R_FacturaCarta
    '            If (Not _Ds.Tables(0).Rows.Count = gi_FacturaCantidadItems) Then
    '                For index = _Ds.Tables(0).Rows.Count To gi_FacturaCantidadItems - 1
    '                    'Insertamos la primera fila con el saldo Inicial
    '                    Dim f As DataRow = _Ds.Tables(0).NewRow
    '                    f.ItemArray() = _Ds.Tables(0).Rows(0).ItemArray
    '                    f.Item("fvbcant") = -1
    '                    _Ds.Tables(0).Rows.Add(f)
    '                Next
    '            End If
    '        End If

    '        objrep.SetDataSource(_Ds.Tables(0))
    '        objrep.SetParameterValue("Hora", _Hora)
    '        objrep.SetParameterValue("Direccionpr", _Ds2.Tables(0).Rows(0).Item("scdir").ToString)
    '        objrep.SetParameterValue("Telefonopr", _Ds2.Tables(0).Rows(0).Item("sctelf").ToString)
    '        objrep.SetParameterValue("Literal1", _Literal)
    '        objrep.SetParameterValue("Literal2", " ")
    '        objrep.SetParameterValue("Literal3", " ")
    '        objrep.SetParameterValue("NroFactura", _NumFac)
    '        objrep.SetParameterValue("NroAutoriz", _Autorizacion)
    '        objrep.SetParameterValue("ENombre", _Ds2.Tables(0).Rows(0).Item("scneg").ToString) '?
    '        objrep.SetParameterValue("ECasaMatriz", _Ds2.Tables(0).Rows(0).Item("scsuc").ToString)
    '        objrep.SetParameterValue("ECiudadPais", _Ds2.Tables(0).Rows(0).Item("scpai").ToString)
    '        objrep.SetParameterValue("ESFC", _Ds1.Tables(0).Rows(0).Item("sbsfc").ToString)
    '        objrep.SetParameterValue("ENit", _Ds2.Tables(0).Rows(0).Item("scnit").ToString)
    '        objrep.SetParameterValue("EActividad", _Ds2.Tables(0).Rows(0).Item("scact").ToString)
    '        objrep.SetParameterValue("ESms", "''" + _Ds1.Tables(0).Rows(0).Item("sbnota").ToString + "''")
    '        objrep.SetParameterValue("ESms2", "''" + _Ds1.Tables(0).Rows(0).Item("sbnota2").ToString + "''")
    '        objrep.SetParameterValue("EDuenho", _Ds2.Tables(0).Rows(0).Item("scnom").ToString) '?
    '        objrep.SetParameterValue("URLImageLogo", gs_CarpetaRaiz + "\LogoFactura.jpg")
    '        objrep.SetParameterValue("URLImageMarcaAgua", gs_CarpetaRaiz + "\MarcaAguaFactura.jpg")

    '        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
    '            P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
    '            P_Global.Visualizador.ShowDialog() 'Comentar
    '            P_Global.Visualizador.BringToFront() 'Comentar
    '        End If

    '        Dim pd As New PrintDocument()
    '        pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
    '        If (Not pd.PrinterSettings.IsValid) Then
    '            ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
    '                                   My.Resources.WARNING, 5 * 1000,
    '                                   eToastGlowColor.Blue, eToastPosition.BottomRight)
    '        Else
    '            objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString '"EPSON TM-T20II Receipt5 (1)"
    '            objrep.PrintToPrinter(1, False, 1, 1)
    '        End If

    '        If (grabarPDF) Then
    '            'Copia de Factura en PDF
    '            If (Not Directory.Exists(gs_CarpetaRaiz + "\Facturas")) Then
    '                Directory.CreateDirectory(gs_CarpetaRaiz + "\Facturas")
    '            End If
    '            objrep.ExportToDisk(ExportFormatType.PortableDocFormat, gs_CarpetaRaiz + "\Facturas\" + CStr(_NumFac) + "_" + CStr(_Autorizacion) + ".pdf")

    '        End If
    '    End If
    '    L_Actualiza_Dosificacion(_numidosif, _NumFac, numi)
    'End Sub

    Public Function P_fnImageToByteArray(ByVal imageIn As Image) As Byte()
        Dim ms As New System.IO.MemoryStream()
        imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg)
        Return ms.ToArray()
    End Function


    Private Function P_fnValidarCierreCaja() As Boolean
        Dim est As String = L_fnObtenerDatoTabla("TIE001", "ieIdCaja", "ieIdDevolucion=" + tbCodigo.Text.Trim)
        If est > "0" Then
            Return True
        Else
            Return False
        End If

    End Function




    Private Sub P_GenerarReporte(numi As String)
        'Dim dt As DataTable = L_fnVentaNotaDeVenta(numi)
        'If (gb_DetalleProducto) Then
        '    ponerDescripcionProducto(dt)
        'End If
        'Dim total As Decimal = dt.Compute("SUM(Total)", "")
        'Dim totald As Double = (total / 6.96)
        'Dim fechaven As String = dt.Rows(0).Item("fechaventa")
        'If Not IsNothing(P_Global.Visualizador) Then
        '    P_Global.Visualizador.Close()
        'End If
        'Dim ParteEntera As Long
        'Dim ParteDecimal As Decimal
        'Dim pDecimal() As String
        'ParteEntera = Int(total)
        'ParteDecimal = Math.Round(total - ParteEntera, 2)
        'pDecimal = Split(ParteDecimal.ToString, ".")


        'Dim li As String = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(ParteEntera)) + " con " +
        'IIf(pDecimal(1).ToString.Equals("0"), "00", pDecimal(1).ToString) + "/100 Bolivianos"

        'ParteEntera = Int(totald)
        'ParteDecimal = Math.Round(totald - ParteEntera, 2)
        'pDecimal = Split(ParteDecimal.ToString, ".")

        'Dim lid As String = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(ParteEntera)) + " con " +
        'IIf(pDecimal(1).ToString.Equals("0"), "00", pDecimal(1).ToString) + "/100 Dolares"

        'Dim dt2 As DataTable = L_fnNameReporte()

        'P_Global.Visualizador = New Visualizador
        'Dim _FechaAct As String
        'Dim _FechaPar As String
        'Dim _Fecha() As String
        'Dim _Meses() As String = {"Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"}
        '_FechaAct = fechaven
        '_Fecha = Split(_FechaAct, "-")
        '_FechaPar = "Cochabamba, " + _Fecha(0).Trim + " De " + _Meses(_Fecha(1) - 1).Trim + " Del " + _Fecha(2).Trim

        'Dim objrep As New R_NotaVenta_7_5X100
        '    '' GenerarNro(_dt)
        '    ''objrep.SetDataSource(Dt1Kardex)

        '    objrep.SetDataSource(dt)
        '    objrep.SetParameterValue("Literal1", li)
        '    If swTipoVenta.Value = True Then
        '        objrep.SetParameterValue("ENombre", "Nota de Entrega Nro. " + numi)
        '    Else
        '        objrep.SetParameterValue("ENombre", "Nota de Crédito Nro. " + numi)
        '    End If
        '    objrep.SetParameterValue("ECiudadPais", _FechaPar)
        '    objrep.SetParameterValue("Sucursal", cbSucursal.Text)
        '    objrep.SetParameterValue("Observacion", tbObservacion.Text)
        '    P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        '    P_Global.Visualizador.ShowDialog() 'Comentar
        '    P_Global.Visualizador.BringToFront() 'Comentar



    End Sub

    Private Sub ponerDescripcionProducto(ByRef dt As DataTable)
        For Each fila As DataRow In dt.Rows
            Dim numi As Integer = fila.Item("codProducto")
            Dim dtDP As DataTable = L_fnDetalleProducto(numi)
            Dim des As String = fila.Item("producto") + vbNewLine + vbNewLine
            For Each fila2 As DataRow In dtDP.Rows
                des = des + fila2.Item("yfadesc").ToString + vbNewLine
            Next
            fila.Item("producto") = des
        Next
    End Sub



#End Region


#Region "Eventos Formulario"
    Private Sub F0_Ventas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Me.SuspendLayout()
        _IniciarTodo()
        btnNuevo.PerformClick()
        'Me.ResumeLayout()
    End Sub
    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        _Limpiar()
        _prhabilitar()

        btnNuevo.Enabled = False
        btnModificar.Enabled = False
        btnEliminar.Enabled = False
        btnGrabar.Enabled = True
        PanelNavegacion.Enabled = False

        'btnNuevo.Enabled = False
        'btnModificar.Enabled = False
        'btnEliminar.Enabled = False
        'GPanelProductos.Visible = False
        '_prhabilitar()

        '_Limpiar()
    End Sub
    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        _prSalir()

    End Sub


    Private Sub swTipoVenta_ValueChanged(sender As Object, e As EventArgs) Handles swTipoVenta.ValueChanged
        If (swTipoVenta.Value = False) Then
            lbCredito.Visible = True
            tbFechaVenc.Visible = True
        Else
            lbCredito.Visible = False
            tbFechaVenc.Visible = False
        End If
    End Sub

    Private Sub grdetalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grdetalle.EditingCell
        If (_fnAccesible()) Then

            If (e.Column.Index = grdetalle.RootTable.Columns("Devolucion").Index) Then
                e.Cancel = False
            Else
                e.Cancel = True
            End If
        Else
            e.Cancel = True
        End If

    End Sub

    Private Sub grdetalle_Enter(sender As Object, e As EventArgs) Handles grdetalle.Enter

        If (_fnAccesible()) Then

            If (tbIdVenta.Text = String.Empty) Then
                ToastNotification.Show(Me, "Antes de Continuar por favor debe seleccionar una Venta!!  ", My.Resources.WARNING, 3500, eToastGlowColor.Red, eToastPosition.TopCenter)
                tbVendedor.Focus()
                Return

            End If

            grdetalle.Select()
            If _codeBar = 1 Then
                If gb_CodigoBarra Then
                    grdetalle.Col = 4
                    grdetalle.Row = 0
                Else
                    grdetalle.Col = 5
                    grdetalle.Row = 0
                End If
            End If
        End If


    End Sub
    Private Sub grdetalle_KeyDown(sender As Object, e As KeyEventArgs) Handles grdetalle.KeyDown
        If (Not _fnAccesible()) Then
            Return
        End If
    End Sub


    Private Sub grdetalle_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellValueChanged


        If (e.Column.Index = grdetalle.RootTable.Columns("Devolucion").Index) Or (e.Column.Index = grdetalle.RootTable.Columns("Devolucion").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("Devolucion")) Or grdetalle.GetValue("Devolucion").ToString = String.Empty) Then

                Dim lin As Integer = grdetalle.GetValue("tbnumi")
                Dim pos As Integer = -1
                _fnObtenerFilaDetalle(pos, lin)
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Devolucion") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("TotalDev") = 0
                _prCalcularPrecioTotal()

            Else
                If (grdetalle.GetValue("Devolucion") > 0 And IsNumeric(grdetalle.GetValue("Devolucion"))) Then
                    Dim rowIndex As Integer = grdetalle.Row
                    Dim lin As Integer = grdetalle.GetValue("tbnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    P_PonerTotal(rowIndex)

                Else
                    Dim lin As Integer = grdetalle.GetValue("tbnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)

                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Devolucion") = 0
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("TotalDev") = 0
                    _prCalcularPrecioTotal()


                End If
            End If

        End If

    End Sub


    Private Sub grdetalle_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellEdited
        If (e.Column.Index = grdetalle.RootTable.Columns("Devolucion").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("Devolucion")) Or grdetalle.GetValue("Devolucion").ToString = String.Empty) Then
                grdetalle.SetValue("Devolucion", 0)
                grdetalle.SetValue("TotalDev", 0)
            Else
                If (grdetalle.GetValue("Devolucion") > 0) Then

                    Dim cant As Integer = grdetalle.GetValue("tbcmin")

                    If (grdetalle.GetValue("Devolucion") > cant) Then
                        Dim lin As Integer = grdetalle.GetValue("tbnumi")
                        Dim pos As Integer = -1
                        _fnObtenerFilaDetalle(pos, lin)
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Devolucion") = 1
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("TotalDev") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("tbpbas") * 1
                        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                        ToastNotification.Show(Me, "La cantidad de la devolución no debe ser mayor al de la cantidad de la venta".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                        grdetalle.SetValue("Devolucion", 1)
                        grdetalle.SetValue("TotalDev", grdetalle.GetValue("tbpbas") * 1)

                        _prCalcularPrecioTotal()

                    End If

                Else
                    grdetalle.SetValue("Devolucion", 0)
                    grdetalle.SetValue("TotalDev", 0)

                End If
            End If
        End If
    End Sub

    Private Sub grdetalle_ColumnHeaderClick(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.ColumnHeaderClick

        Try
            grdetalle.Focus()

            grdetalle.Col = 1
        Catch ex As Exception

        End Try


    End Sub
    Private Sub btnGrabar_Click(sender As Object, e As EventArgs) Handles btnGrabar.Click
        If _ValidarCampos() = False Then
            Exit Sub
        End If

        If (tbCodigo.Text = String.Empty) Then
            _GuardarNuevo()
        Else
            If (tbCodigo.Text <> String.Empty) Then
                _prGuardarModificado()
                ''    _prInhabiliitar() RODRIGO RLA

            End If
        End If

    End Sub

    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        If (grVentas.RowCount > 0) Then
            If (gb_FacturaEmite) Then
                If (Not P_fnValidarCierreCaja()) Then
                    Dim img As Bitmap = New Bitmap(My.Resources.WARNING, 50, 50)

                    ToastNotification.Show(Me, "No se puede modificar la venta con codigo ".ToUpper + tbCodigo.Text + ", su factura esta anulada.".ToUpper,
                                              img, 2000,
                                              eToastGlowColor.Green,
                                              eToastPosition.TopCenter)
                    Exit Sub
                End If
            End If

            _prhabilitar()
            btnNuevo.Enabled = False
            btnModificar.Enabled = False
            btnEliminar.Enabled = False
            btnGrabar.Enabled = True

            PanelNavegacion.Enabled = False

        End If
    End Sub
    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click


        If (P_fnValidarCierreCaja()) Then
            Dim img As Bitmap = New Bitmap(My.Resources.WARNING, 50, 50)

            ToastNotification.Show(Me, "No se puede eliminar la devolución con código ".ToUpper + tbCodigo.Text + ", tiene enlazado cierre de caja.".ToUpper,
                                          img, 3500,
                                          eToastGlowColor.Green,
                                          eToastPosition.TopCenter)
            Exit Sub
        End If


        Dim ef = New Efecto


        ef.tipo = 2
        ef.Context = "¿esta seguro de eliminar el registro?".ToUpper
        ef.Header = "mensaje principal".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean = L_fnEliminarDevolucion(tbCodigo.Text, mensajeError)
            If res Then


                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)

                ToastNotification.Show(Me, "Código de Devolución ".ToUpper + tbCodigo.Text + " eliminado con éxito.".ToUpper,
                                          img, 2000,
                                          eToastGlowColor.Green,
                                          eToastPosition.TopCenter)

                _prFiltrar()

            Else
                Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
                ToastNotification.Show(Me, mensajeError, img, 3500, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If
        End If

    End Sub



    Private Sub btnSiguiente_Click(sender As Object, e As EventArgs) Handles btnSiguiente.Click
        Dim _pos As Integer = grDevolucion.Row
        If _pos < grDevolucion.RowCount - 1 And _pos >= 0 Then
            _pos = grDevolucion.Row + 1
            '' _prMostrarRegistro(_pos)
            grDevolucion.Row = _pos
        End If
    End Sub

    Private Sub btnUltimo_Click(sender As Object, e As EventArgs) Handles btnUltimo.Click
        Dim _pos As Integer = grDevolucion.Row
        If grDevolucion.RowCount > 0 Then
            _pos = grDevolucion.RowCount - 1
            ''  _prMostrarRegistro(_pos)
            grDevolucion.Row = _pos
        End If
    End Sub

    Private Sub btnAnterior_Click(sender As Object, e As EventArgs) Handles btnAnterior.Click
        Dim _MPos As Integer = grDevolucion.Row
        If _MPos > 0 And grDevolucion.RowCount > 0 Then
            _MPos = _MPos - 1
            ''  _prMostrarRegistro(_MPos)
            grDevolucion.Row = _MPos
        End If
    End Sub

    Private Sub btnPrimero_Click(sender As Object, e As EventArgs) Handles btnPrimero.Click
        _PrimerRegistro()
    End Sub
    Private Sub grVentas_KeyDown(sender As Object, e As KeyEventArgs) Handles grVentas.KeyDown
        If e.KeyData = Keys.Enter Then
            MSuperTabControl.SelectedTabIndex = 0

            tbIdVenta.Text = grVentas.GetValue("tanumi")
            tbCliente.Text = grVentas.GetValue("cliente")
            tbVendedor.Text = grVentas.GetValue("vendedor")
            tbFechaVenta.Value = grVentas.GetValue("tafdoc")
            cbSucursal.Value = grVentas.GetValue("taalm")
            swTipoVenta.Value = grVentas.GetValue("tatven")
            tbFechaVenc.Value = grVentas.GetValue("tafvcr")
            cbPrecio.Value = grVentas.GetValue("taCatPrecio")

            MostrarDetalleDev = 0
            _prCargarDetalleVenta(tbIdVenta.Text)

        End If
    End Sub


    Private Sub btnImprimir_Click(sender As Object, e As EventArgs) Handles btnImprimir.Click
        If (Not _fnAccesible()) Then
            P_GenerarReporte(tbCodigo.Text)
        End If
    End Sub

    Private Sub btnBuscarVenta_Click(sender As Object, e As EventArgs) Handles btnBuscarVenta.Click
        grVentas.Enabled = True
        GroupPanel2.Visible = True
        MSuperTabControl.SelectedTabIndex = 1
        _prCargarVenta()
        GroupPanel3.Visible = False

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

    Private Sub grDevolucion_SelectionChanged(sender As Object, e As EventArgs) Handles grDevolucion.SelectionChanged
        If (grDevolucion.RowCount >= 0 And grDevolucion.Row >= 0) Then
            _prMostrarRegistro(grDevolucion.Row)
        End If
    End Sub


#End Region
End Class