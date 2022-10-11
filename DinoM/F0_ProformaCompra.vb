Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports System.Drawing
Imports DevComponents.DotNetBar.Controls
Imports System.Threading
Imports System.Drawing.Text
Imports System.Reflection
Imports System.Runtime.InteropServices
Public Class F0_ProformaCompra
    Dim _Inter As Integer = 0

#Region "Variables Globales"
    Dim _CodProveedor As Integer = 0
    Dim _numiCatCosto As Integer = 0
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
    Public Table_Producto As DataTable
    Dim _PorcentajeUtil As Double = 0 '' En esta varible obtendre de la libreria el porcentaje de utilidad para la venta 
    Dim _estadoPor As Integer ''En esta variable me dira si sera visible o no las columnas de porcentaje de utilidad y precio de venta
    Dim Lote As Boolean = False
    Dim dtProductoGoblal As DataTable = Nothing
#End Region

#Region "Metodos Privados"
    Private Sub _IniciarTodo()
        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        _prValidarLote()
        _prObtenerPorcentajeUtilidad()
        'Me.WindowState = FormWindowState.Maximized
        _prCargarProformaCompra()
        _prInhabiliitar()
        grProforma.Focus()
        _prAsignarPermisos()
        Me.Text = "PROFORMA"
        PanelDetalle.Height = 250
        MSuperTabControl.SelectedTabIndex = 0
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
    Public Sub _prObtenerPorcentajeUtilidad()
        ''''En este procedimiento obtendre el porcentaje de utilidad que esta en la tabla de configuraciones
        Dim dt As DataTable = L_fnPorcUtilidad()
        If (dt.Rows.Count > 0) Then
            _PorcentajeUtil = dt.Rows(0).Item("PorcUtil")
            _estadoPor = dt.Rows(0).Item("VerPorc")
        End If

    End Sub

    Private Sub _prInhabiliitar()
        tbCodigo.ReadOnly = True
        tbProveedor.ReadOnly = True
        tbCodProv.ReadOnly = True
        tbObservacion.ReadOnly = True
        tbFecha.IsInputReadOnly = True
        tbFecha.Enabled = False
        tbNitProv.ReadOnly = True

        ''''''''''
        btnModificar.Enabled = True
        btnGrabar.Enabled = False
        btnNuevo.Enabled = True
        btnEliminar.Enabled = True

        tbSubtotalC.IsInputReadOnly = True
        tbMdesc.IsInputReadOnly = True
        tbtotal.IsInputReadOnly = True

        grProforma.Enabled = True
        PanelNavegacion.Enabled = True
        grdetalle.RootTable.Columns("img").Visible = False

        btnAgregar.Visible = False
    End Sub
    Private Sub _prhabilitar()
        grProforma.Enabled = False
        tbCodigo.ReadOnly = False
        ''  tbCliente.ReadOnly = False  por que solo podra seleccionar Cliente
        ''  tbVendedor.ReadOnly = False
        tbObservacion.ReadOnly = False
        tbFecha.IsInputReadOnly = False

        btnGrabar.Enabled = True
        tbNitProv.ReadOnly = False

        tbMdesc.IsInputReadOnly = False
        btnAgregar.Visible = True

    End Sub
    Public Sub _prFiltrar()
        'cargo el buscador
        Dim _Mpos As Integer
        _prCargarProformaCompra()
        If grProforma.RowCount > 0 Then
            _Mpos = 0
            grProforma.Row = _Mpos
        Else
            _Limpiar()
            LblPaginacion.Text = "0/0"
        End If
    End Sub
    Private Sub _Limpiar()
        tbCodigo.Clear()
        tbProveedor.Clear()
        tbNitProv.Clear()
        tbObservacion.Clear()

        _CodProveedor = 0
        tbFecha.Value = Now.Date
        tbCodProv.Clear()
        tbProveedor.Focus()

        _prCargarDetalle(-1)
        CType(grdetalle.DataSource, DataTable).Rows.Clear()
        MSuperTabControl.SelectedTabIndex = 0

        tbPdesc.Value = 0
        tbMdesc.Value = 0
        tbtotal.Value = 0
        tbSubtotalC.Value = 0
        With grdetalle.RootTable.Columns("img")
            .Width = 80
            .Caption = "Eliminar"
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = True
        End With

        _prAddDetalleProforma()
        Table_Producto = Nothing

    End Sub
    Public Sub _prMostrarRegistro(_N As Integer)

        With grProforma
            tbCodigo.Text = .GetValue("pcnumi")
            tbFecha.Value = .GetValue("pcfdoc")
            _CodProveedor = .GetValue("pcty4prov")
            tbProveedor.Text = .GetValue("proveedor")
            tbNitProv.Text = .GetValue("yddctnum")
            tbObservacion.Text = .GetValue("pcobs")
            tbCodProv.Text = .GetValue("pcty4prov").ToString + "-" + .GetValue("ydcod").ToString

            lbFecha.Text = CType(.GetValue("pcfact"), Date).ToString("dd/MM/yyyy")
            lbHora.Text = .GetValue("pchact").ToString
            lbUsuario.Text = .GetValue("pcuact").ToString


        End With

        _prCargarDetalle(tbCodigo.Text)
        tbMdesc.Value = grProforma.GetValue("pcdesc")

        tbtotal.Value = grProforma.GetValue("total")
        tbSubtotalC.Value = tbtotal.Value + tbMdesc.Value

        LblPaginacion.Text = Str(grProforma.Row + 1) + "/" + grProforma.RowCount.ToString

    End Sub

    Private Sub _prCargarDetalle(_numi As String)
        Dim dt As New DataTable
        dt = L_fnDetalleProformaCompra(_numi)
        grdetalle.DataSource = dt
        grdetalle.RetrieveStructure()
        grdetalle.AlternatingColors = True

        If (Lote = True) Then
            With grdetalle.RootTable.Columns("pdlote")
                .Width = 150
                .Caption = "LOTE"
                .Visible = True
                .MaxLength = 50
            End With
            With grdetalle.RootTable.Columns("pdfechavenc")
                .Width = 120
                .Caption = "FECHA VENC."
                .Visible = True
                .FormatString = "dd/MM/yyyy"
            End With
        Else
            With grdetalle.RootTable.Columns("pdlote")
                .Width = 150
                .Caption = "LOTE"
                .Visible = False
                .MaxLength = 50
            End With
            With grdetalle.RootTable.Columns("pdfechavenc")
                .Width = 120
                .Caption = "FECHA VENC."
                .Visible = False
                .FormatString = "dd/MM/yyyy"
            End With
        End If

        With grdetalle.RootTable.Columns("pdnumi")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = False
        End With

        With grdetalle.RootTable.Columns("pdtv1numi")
            .Width = 90
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("pdty5prod")
            .Width = 90
            .Caption = "Item"
            .Visible = True
        End With
        With grdetalle.RootTable.Columns("CodigoFabrica")
            .Caption = "Cod.Fabrica"
            .Width = 120
            .WordWrap = True
            .MaxLines = 2
            .Visible = True

        End With
        With grdetalle.RootTable.Columns("CodigoMarca")
            .Caption = "Cod.Marca"
            .Width = 120
            .WordWrap = True
            .MaxLines = 2
            .Visible = True

        End With

        With grdetalle.RootTable.Columns("Medida")
            .Caption = "Medida"
            .Width = 120
            .WordWrap = True
            .MaxLines = 2
            .Visible = True

        End With

        With grdetalle.RootTable.Columns("Marca")
            .Caption = "Marca"
            .Width = 120
            .WordWrap = True
            .MaxLines = 2
            .Visible = True
        End With
        With grdetalle.RootTable.Columns("Procedencia")
            .Caption = "Procedencia"
            .Width = 120
            .WordWrap = True
            .MaxLines = 2
            .Visible = True
        End With

        With grdetalle.RootTable.Columns("producto")
            .Caption = "Descripcion"
            .Width = 400
            .WordWrap = True
            .MaxLines = 3
            .Visible = True

        End With
        With grdetalle.RootTable.Columns("pdest")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With

        With grdetalle.RootTable.Columns("pdcmin")
            .Width = 110
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad"
        End With
        With grdetalle.RootTable.Columns("pdumin")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("unidad")
            .Width = 80
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .Caption = "Unidad".ToUpper
        End With
        With grdetalle.RootTable.Columns("pdpcost")
            .Width = 140
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "P.CostoUn.($)"
        End With
        If (_estadoPor = 1) Then
            With grdetalle.RootTable.Columns("pdutven")
                .Width = 110
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatString = "0.00"
                .Caption = "Utilidad (%)".ToUpper
            End With
            With grdetalle.RootTable.Columns("pdprven")
                .Width = 120
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatString = "0.00"
                .Caption = "Precio Venta".ToUpper
            End With
        Else
            With grdetalle.RootTable.Columns("pdutven")
                .Width = 120
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatString = "0.00"
                .Caption = "Utilidad.".ToUpper
            End With
            With grdetalle.RootTable.Columns("pdprven")
                .Width = 120
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatString = "0.00"
                .Caption = "Precio Venta.".ToUpper
            End With
        End If

        With grdetalle.RootTable.Columns("pdptot")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Sub Total ($)"
        End With
        With grdetalle.RootTable.Columns("pdobs")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("pdfact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("pdhact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("pduact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("estado")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("costo")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("img")
            .Width = 80
            .Caption = "Eliminar".ToUpper
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("venta")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("pdpFacturado")
            .Width = 140
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "P.Facturado Bs"
        End With
        With grdetalle.RootTable.Columns("pdpPublico")
            .Width = 140
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "P.Público Bs."
        End With
        With grdetalle.RootTable.Columns("pdpMecanico")
            .Width = 140
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "P.Mecánico Bs."
        End With
        With grdetalle
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007

            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges

        End With
    End Sub


    Private Sub _prCargarProformaCompra()
        Dim dt As New DataTable
        dt = L_fnGeneralProformaCompra()
        grProforma.DataSource = dt
        grProforma.RetrieveStructure()
        grProforma.AlternatingColors = True


        With grProforma.RootTable.Columns("pcnumi")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = True
        End With

        With grProforma.RootTable.Columns("pcfdoc")
            .Width = 90
            .Visible = True
            .Caption = "FECHA"
        End With

        With grProforma.RootTable.Columns("pcty4prov")
            .Width = 160
            .Visible = False
        End With
        With grProforma.RootTable.Columns("ydcod")
            .Width = 160
            .Visible = False
        End With
        With grProforma.RootTable.Columns("proveedor")
            .Width = 250
            .Visible = True
            .Caption = "proveedor".ToUpper
        End With
        With grProforma.RootTable.Columns("yddctnum")
            .Width = 100
            .Visible = False
            .Caption = "Ci/Nit".ToUpper
        End With
        With grProforma.RootTable.Columns("pcest")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProforma.RootTable.Columns("pcobs")
            .Width = 200
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "OBSERVACION"
        End With
        With grProforma.RootTable.Columns("pcdesc")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProforma.RootTable.Columns("total")
            .Width = 150
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "TOTAL"
            .FormatString = "0.00"
        End With
        With grProforma.RootTable.Columns("pcfact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProforma.RootTable.Columns("pchact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProforma.RootTable.Columns("pcuact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grProforma
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With

        If (dt.Rows.Count <= 0) Then
            _prCargarDetalle(-1)
        End If
    End Sub
    Private Sub _prCargarProductos(_cliente As String)

        Dim dtname As DataTable = L_fnNameLabel()


        If (IsNothing(dtProductoGoblal)) Then
            dtProductoGoblal = L_fnListarProductosCompra(1, 73)
        End If


        Dim frm As F0_DetalleProforma
        frm = New F0_DetalleProforma(dtProductoGoblal, CType(grdetalle.DataSource, DataTable), dtname)


        frm.ShowDialog()
        dtProductoGoblal = frm.dtProductoAll

    End Sub
    Private Sub _prAddDetalleProforma()

        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.delete, 28, 28)
        img.Save(Bin, Imaging.ImageFormat.Png)
        CType(grdetalle.DataSource, DataTable).Rows.Add(_fnSiguienteNumi() + 1, 0, 0, "", "", "", "", "", "", 0, 0, 0, "",
                                                        0, "20500101", CDate("2050/01/01"), 0, 0, 0, "", Now.Date, "", "", 0, 0, 0, 0, Bin.GetBuffer, 0, 0)
    End Sub

    Public Function _fnSiguienteNumi()
        Dim dt As DataTable = CType(grdetalle.DataSource, DataTable)
        Dim rows() As DataRow = dt.Select("pdnumi=MAX(pdnumi)")
        If (rows.Count > 0) Then
            Return rows(rows.Count - 1).Item("pdnumi")
        End If
        Return 1
    End Function
    Public Function _fnAccesible()
        Return tbFecha.IsInputReadOnly = False
    End Function
    Private Sub _HabilitarProductos()
        _prCargarProductos(73) ''''Aqui poner el Primer Precio de Costo
    End Sub
    Private Sub _DesHabilitarProductos()

        PanelTotal.Visible = True
        PanelInferior.Visible = True


        grdetalle.Select()
        grdetalle.Col = 5
        grdetalle.Row = grdetalle.RowCount - 1

    End Sub
    Public Sub _fnObtenerFilaDetalle(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("pdnumi")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next

    End Sub

    Public Function _fnExisteProducto(idprod As Integer) As Boolean
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("pdty5prod")
            Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("estado")
            If (_idprod = idprod And estado >= 0) Then

                Return True
            End If
        Next
        Return False
    End Function
    Public Sub P_PonerTotal(rowIndex As Integer)
        If (rowIndex < grdetalle.RowCount) Then

            Dim lin As Integer = grdetalle.GetValue("pdnumi")
            Dim pos As Integer = -1
            _fnObtenerFilaDetalle(pos, lin)
            Dim cant As Double = grdetalle.GetValue("pdcmin")
            Dim uni As Double = grdetalle.GetValue("pdpcost")
            Dim pFacturado As Double

            If (pos >= 0) Then
                Dim TotalUnitario As Double = cant * uni
                'grDetalle.SetValue("lcmdes", montodesc)

                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdptot") = TotalUnitario
                grdetalle.SetValue("pdptot", TotalUnitario)
                Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado")
                If (estado = 1) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If
                'CType(grdetalle.DataSource, DataTable).Rows(pos).Item("cbprven") = (uni + (uni * (grdetalle.GetValue("cbutven") / 100))) * tbTipoCambio.Value
                'grdetalle.SetValue("cbprven", (uni + (uni * (grdetalle.GetValue("cbutven") / 100))) * tbTipoCambio.Value)
                'CType(grdetalle.DataSource, DataTable).Rows(pos).Item("venta") = (uni + (uni * (grdetalle.GetValue("cbutven") / 100))) * tbTipoCambio.Value
                'grdetalle.SetValue("cbprven", (uni + (uni * (grdetalle.GetValue("cbutven") / 100))) * tbTipoCambio.Value)
            End If

            _prCalcularPrecioTotal()
        End If



    End Sub
    Public Sub _prCalcularPrecioTotal()
        Dim ret As Double
        Dim montodesc As Double = tbMdesc.Value
        Dim pordesc As Double = ((montodesc * 100) / grdetalle.GetTotal(grdetalle.RootTable.Columns("pdptot"), AggregateFunction.Sum))
        tbPdesc.Value = pordesc
        tbtotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("pdptot"), AggregateFunction.Sum) - montodesc
        'Agregado para que Muestre el Subtotal 
        tbSubtotalC.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("pdptot"), AggregateFunction.Sum)


    End Sub
    Public Sub _prEliminarFila()
        If (grdetalle.Row >= 0) Then
            If (grdetalle.RowCount >= 2) Then
                Dim estado As Integer = grdetalle.GetValue("estado")
                Dim pos As Integer = -1
                Dim lin As Integer = grdetalle.GetValue("pdnumi")
                _fnObtenerFilaDetalle(pos, lin)
                If (estado = 0) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = -2

                End If
                If (estado = 1) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = -1
                End If
                grdetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grdetalle.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))
                _prCalcularPrecioTotal()
                grdetalle.Select()
                grdetalle.Col = 5
                grdetalle.Row = grdetalle.RowCount - 1
            End If
        End If


    End Sub
    Public Function _ValidarCampos() As Boolean
        If (_CodProveedor <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor Seleccione un Proveedor con Ctrl+Enter".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbProveedor.Focus()
            Return False

        End If

        If (grdetalle.RowCount = 1) Then
            grdetalle.Row = grdetalle.RowCount - 1
            If (grdetalle.GetValue("pdty5prod") = 0) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "Por Favor Seleccione  un detalle de producto".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                Return False
            End If

        End If


        ''Controla que no se metan un mismo producto con el mismo lote y fecha de vencimiento
        Dim dt1 As DataTable = CType(grdetalle.DataSource, DataTable)
        For i As Integer = 0 To grdetalle.RowCount - 1 Step 1
            Dim _idprod As String = CType(grdetalle.DataSource, DataTable).Rows(i).Item("pdty5prod")
            Dim _Lote As String = CType(grdetalle.DataSource, DataTable).Rows(i).Item("pdlote")
            Dim _Fecha As String = CType(grdetalle.DataSource, DataTable).Rows(i).Item("pdfechavenc")
            Dim _estado As String = 0

            Dim query = dt1.Select("pdty5prod='" + _idprod + "' And pdlote='" + _Lote + "' And pdfechavenc='" + _Fecha + "' And estado>='" + _estado + "'")

            If query.Count >= 2 Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "No puede registrar mas de un producto con el mismo lote y fecha de vencimiento, favor modificar".ToUpper, img, 4000, eToastGlowColor.Red, eToastPosition.BottomCenter)

                Return False
            End If
        Next

        Return True
    End Function

    Public Sub _GuardarNuevo()
        Try

            Dim res As Boolean = L_fnGrabarProformaCompra("", tbFecha.Value.ToString("yyyy/MM/dd"), _CodProveedor, tbObservacion.Text, tbMdesc.Value,
                                                  tbtotal.Value, CType(grdetalle.DataSource, DataTable))
            If res Then

                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
                ToastNotification.Show(Me, "Código de Proforma ".ToUpper + tbCodigo.Text + " Grabada con éxito.".ToUpper,
                                          img, 3000,
                                          eToastGlowColor.Green,
                                          eToastPosition.TopCenter
                                          )

                _prCargarProformaCompra()
                _Limpiar()
            Else
                Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
                ToastNotification.Show(Me, "La Proforma no pudo ser insertada".ToUpper, img, 3000, eToastGlowColor.Red, eToastPosition.BottomCenter)

            End If
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub


    Private Sub _prGuardarModificado()

        Dim res As Boolean = L_fnModificarProformaCompra(tbCodigo.Text, tbFecha.Value.ToString("yyyy/MM/dd"), _CodProveedor, tbObservacion.Text, tbMdesc.Value,
                                                  tbtotal.Value, CType(grdetalle.DataSource, DataTable))
        If res Then

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Proforma ".ToUpper + tbCodigo.Text + " Modificada con éxito.".ToUpper,
                                      img, 3000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )
            _prCargarProformaCompra()

            _prSalir()


        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La Proforma no pudo ser Modificada".ToUpper, img, 3000, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If
    End Sub
    Private Sub _prSalir()
        If btnGrabar.Enabled = True Then
            _prInhabiliitar()
            If grProforma.RowCount > 0 Then
                _prMostrarRegistro(0)
            End If
        Else

            Me.Close()
            _modulo.Select()
        End If
    End Sub
    Public Sub _prCargarIconELiminar()
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim Bin As New MemoryStream
            Dim img As New Bitmap(My.Resources.delete, 28, 28)
            img.Save(Bin, Imaging.ImageFormat.Png)
            CType(grdetalle.DataSource, DataTable).Rows(i).Item("img") = Bin.GetBuffer
            grdetalle.RootTable.Columns("img").Visible = True
        Next

    End Sub
    Public Sub _PrimerRegistro()
        Dim _MPos As Integer
        If grProforma.RowCount > 0 Then
            _MPos = 0
            ''   _prMostrarRegistro(_MPos)
            grProforma.Row = _MPos
        End If
    End Sub

    Private Sub P_GenerarReporteProforma()
        'Dim dt As DataTable = L_fnNotaCompras(tbCodigo.Text)
        ''Dim dt2 = L_DatosEmpresa("1")
        'Dim _TotalLi As Decimal
        'Dim _Literal, _TotalDecimal, _TotalDecimal2, moneda As String

        ''Literal 
        '_TotalLi = dt.Rows(0).Item("total")
        '_TotalDecimal = _TotalLi - Math.Truncate(_TotalLi)
        '_TotalDecimal2 = CDbl(_TotalDecimal) * 100



        '_Literal = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(_TotalLi) - CDbl(_TotalDecimal)) + "  " + IIf(_TotalDecimal2.Equals("0"), "00", _TotalDecimal2) + "/100 " + moneda


        'If Not IsNothing(P_Global.Visualizador) Then
        '    P_Global.Visualizador.Close()
        'End If

        'P_Global.Visualizador = New Visualizador

        'Dim objrep As New R_NotaCompra
        'objrep.SetDataSource(dt)

        'objrep.SetParameterValue("Literal", _Literal)


        'P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        'P_Global.Visualizador.ShowDialog() 'Comentar
        'P_Global.Visualizador.BringToFront()

        ''P_Global.Visualizador.CRV1.ReportSource = objrep
        ''P_Global.Visualizador.Show()
        ''P_Global.Visualizador.BringToFront()
    End Sub
#End Region


#Region "Eventos Formulario"
    Private Sub F0_Ventas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _IniciarTodo()
        btnNuevo.PerformClick()
        tbProveedor.Focus()
    End Sub
    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        _Limpiar()
        _prhabilitar()

        btnNuevo.Enabled = False
        btnModificar.Enabled = False
        btnEliminar.Enabled = False
        btnGrabar.Enabled = True
        PanelNavegacion.Enabled = False



    End Sub
    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        _prSalir()

    End Sub
    Private Sub MostrarMensajeError(mensaje As String)
        ToastNotification.Show(Me,
                               mensaje.ToUpper,
                               My.Resources.WARNING,
                               5000,
                               eToastGlowColor.Red,
                               eToastPosition.TopCenter)

    End Sub
    Private Sub MostrarMensajeOk(mensaje As String)
        ToastNotification.Show(Me,
                               mensaje.ToUpper,
                               My.Resources.OK,
                               5000,
                               eToastGlowColor.Green,
                               eToastPosition.TopCenter)
    End Sub
    Private Sub tbProveedor_KeyDown(sender As Object, e As KeyEventArgs) Handles tbProveedor.KeyDown
        Try
            If (_fnAccesible()) Then
                If e.KeyData = Keys.Control + Keys.Enter Then

                    Dim dt As DataTable

                    dt = L_fnListarProveedores()
                    '              a.ydnumi, a.ydcod, a.yddesc, a.yddctnum, a.yddirec
                    ',a.ydtelf1 ,a.ydfnac 
                    If dt.Rows.Count = 0 Then
                        Throw New Exception("Lista de proveedores vacia")
                    End If
                    Dim listEstCeldas As New List(Of Modelo.Celda)
                    listEstCeldas.Add(New Modelo.Celda("ydnumi,", True, "COD ORIG.", 90))
                    listEstCeldas.Add(New Modelo.Celda("ydcod", True, "COD PROV.", 90))
                    listEstCeldas.Add(New Modelo.Celda("yddesc", True, "NOMBRE", 280))
                    listEstCeldas.Add(New Modelo.Celda("yddctnum", True, "N. Documento".ToUpper, 150))
                    listEstCeldas.Add(New Modelo.Celda("yddirec", True, "DIRECCION", 220))
                    listEstCeldas.Add(New Modelo.Celda("ydtelf1", True, "Telefono".ToUpper, 200))
                    listEstCeldas.Add(New Modelo.Celda("ydfnac", False, "F.Nacimiento".ToUpper, 150, "MM/dd,YYYY"))
                    Dim ef = New Efecto
                    ef.tipo = 3
                    ef.dt = dt
                    ef.SeleclCol = 2
                    ef.listEstCeldas = listEstCeldas
                    ef.alto = 50
                    ef.ancho = 350
                    ef.Context = "Seleccione Proveedor".ToUpper
                    ef.ShowDialog()
                    Dim bandera As Boolean = False
                    bandera = ef.band
                    If (bandera = True) Then
                        Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                        _CodProveedor = Row.Cells("ydnumi").Value
                        tbProveedor.Text = Row.Cells("yddesc").Value
                        'tbCodProv.Text = (Row.Cells("ydnumi").Value + " ' - '" + Row.Cells("ydcod").Value).ToString
                        tbCodProv.Text = Row.Cells("ydnumi").Text + "-" + Row.Cells("ydcod").Text
                        tbNitProv.Text = Row.Cells("yddctnum").Value
                        tbObservacion.Focus()
                    End If
                End If
            End If
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub



    Private Sub grdetalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grdetalle.EditingCell
        If (_fnAccesible()) Then

            If (_estadoPor = 0) Then
                If (e.Column.Index = grdetalle.RootTable.Columns("pdcmin").Index Or e.Column.Index = grdetalle.RootTable.Columns("pdpcost").Index) Then
                    e.Cancel = False
                Else
                    e.Cancel = True
                End If
            Else
                If (e.Column.Index = grdetalle.RootTable.Columns("pdcmin").Index Or e.Column.Index = grdetalle.RootTable.Columns("pdpcost").Index Or
                    e.Column.Index = grdetalle.RootTable.Columns("pdprven").Index Or e.Column.Index = grdetalle.RootTable.Columns("pdutven").Index Or
                    e.Column.Index = grdetalle.RootTable.Columns("pdlote").Index Or e.Column.Index = grdetalle.RootTable.Columns("pdfechavenc").Index Or
                    e.Column.Index = grdetalle.RootTable.Columns("pdpFacturado").Index Or e.Column.Index = grdetalle.RootTable.Columns("pdpPublico").Index Or
                    e.Column.Index = grdetalle.RootTable.Columns("pdpMecanico").Index) Then
                    e.Cancel = False
                Else
                    e.Cancel = True
                End If
            End If

        Else
            e.Cancel = True
        End If

    End Sub

    Private Sub grdetalle_Enter(sender As Object, e As EventArgs) Handles grdetalle.Enter

        If (_fnAccesible()) Then
            If (_CodProveedor <= 0) Then
                ToastNotification.Show(Me, "           Antes de Continuar Por favor Seleccione un Proveedor!!             ", My.Resources.WARNING, 4000, eToastGlowColor.Red, eToastPosition.TopCenter)
                tbProveedor.Focus()
                Return
            End If

        End If


    End Sub
    Private Sub grdetalle_KeyDown(sender As Object, e As KeyEventArgs) Handles grdetalle.KeyDown
        If (Not _fnAccesible()) Then
            Return
        End If
        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = grdetalle.Col
            f = grdetalle.Row

            If (grdetalle.Col = grdetalle.RootTable.Columns("pdcmin").Index) Then
                If (grdetalle.GetValue("producto") <> String.Empty) Then

                    _HabilitarProductos()
                Else
                    ToastNotification.Show(Me, "Seleccione un Producto Por Favor", My.Resources.WARNING, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If

            End If
            If (grdetalle.Col = grdetalle.RootTable.Columns("producto").Index) Then
                If (grdetalle.GetValue("producto") <> String.Empty) Then

                    _HabilitarProductos()
                Else
                    ToastNotification.Show(Me, "Seleccione un Producto Por Favor", My.Resources.WARNING, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If

            End If
salirIf:
        End If

        If (e.KeyData = Keys.Control + Keys.Enter And grdetalle.Row >= 0 And
            grdetalle.Col = grdetalle.RootTable.Columns("producto").Index) Then
            Dim indexfil As Integer = grdetalle.Row
            Dim indexcol As Integer = grdetalle.Col
            _HabilitarProductos()

        End If
        If (e.KeyData = Keys.Escape And grdetalle.Row >= 0) Then

            _prEliminarFila()


        End If


    End Sub

    Private Sub grdetalle_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellValueChanged

        Dim lin As Integer = grdetalle.GetValue("pdnumi")
        Dim pos As Integer = -1
        _fnObtenerFilaDetalle(pos, lin)
        If (e.Column.Index = grdetalle.RootTable.Columns("pdcmin").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("pdcmin")) Or grdetalle.GetValue("pdcmin").ToString = String.Empty) Then
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdcmin") = 1
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdptot") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost")
            Else
                If (grdetalle.GetValue("pdcmin") > 0) Then
                    Dim rowIndex As Integer = grdetalle.Row
                    P_PonerTotal(rowIndex)
                Else

                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdcmin") = 1
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdptot") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost")
                    _prCalcularPrecioTotal()
                End If
            End If
        End If

        ''''''''''''''''''''''COSTO  ''''''''''''''''''''''''''''''''''''''''''
        If (e.Column.Index = grdetalle.RootTable.Columns("pdpcost").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("pdpcost")) Or grdetalle.GetValue("pdpcost").ToString = String.Empty) Then
                Dim cantidad As Double = grdetalle.GetValue("pdcmin")
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost")
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdptot") = cantidad * CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost")
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdprven") = _PorcentajeUtil * CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost")

            Else
                If (grdetalle.GetValue("pdpcost") > 0) Then
                    Dim rowIndex As Integer = grdetalle.Row
                    P_PonerTotal(rowIndex)

                    Dim pFacturado As Double
                    Dim uni As Double = grdetalle.GetValue("pdpcost")
                    ''Cálculo de los demás precios
                    pFacturado = ((uni + (uni * 0.25) + (uni * 0.16)) * 2) * 7

                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpFacturado") = Math.Round(pFacturado, 2)
                    grdetalle.SetValue("pdpFacturado", Math.Round(pFacturado, 2))
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpPublico") = Math.Round(pFacturado - (pFacturado * 0.1), 2)
                    grdetalle.SetValue("pdpPublico", Math.Round(pFacturado - (pFacturado * 0.1), 2))
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpMecanico") = Math.Round(pFacturado - (pFacturado * 0.15), 2)
                    grdetalle.SetValue("pdpMecanico", Math.Round(pFacturado - (pFacturado * 0.15), 2))
                Else

                    Dim cantidad As Double = grdetalle.GetValue("pdcmin")
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost")
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdptot") = cantidad * CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost")
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdprven") = _PorcentajeUtil * CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost")
                End If
            End If
        End If

        ''''''''''''''''''''''PRECIO FACTURADO'''''''''''''''''''''''
        If (e.Column.Index = grdetalle.RootTable.Columns("pdpFacturado").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("pdpFacturado")) Or grdetalle.GetValue("pdpFacturado").ToString = String.Empty) Then

                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpFacturado") = 0
                grdetalle.SetValue("pdpFacturado", 0)
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpPublico") = 0
                grdetalle.SetValue("pdpPublico", 0)
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpMecanico") = 0
                grdetalle.SetValue("pdpMecanico", 0)

            Else
                If (grdetalle.GetValue("pdpFacturado") > 0) Then
                    Dim pFacturado As Double

                    ''Cálculo de los demás precios
                    pFacturado = grdetalle.GetValue("pdpFacturado")

                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpPublico") = pFacturado - (pFacturado * 0.1)
                    grdetalle.SetValue("pdpPublico", pFacturado - (pFacturado * 0.1))
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpMecanico") = pFacturado - (pFacturado * 0.15)
                    grdetalle.SetValue("pdpMecanico", pFacturado - (pFacturado * 0.15))
                Else
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpFacturado") = 0
                    grdetalle.SetValue("pdpFacturado", 0)
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpPublico") = 0
                    grdetalle.SetValue("pdpPublico", 0)
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpMecanico") = 0
                    grdetalle.SetValue("pdpMecanico", 0)
                End If
            End If
        End If


        ''''''''''''''''''''''PRECIO PÚBLICO'''''''''''''''''''''''
        If (e.Column.Index = grdetalle.RootTable.Columns("pdpPublico").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("pdpPublico")) Or grdetalle.GetValue("pdpPublico").ToString = String.Empty) Then
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdbpPublico") = 0
                grdetalle.SetValue("pdpPublico", 0)

            End If
        End If
        ''''''''''''''''''''''PRECIO MECÁNICO'''''''''''''''''''''''
        If (e.Column.Index = grdetalle.RootTable.Columns("pdpMecanico").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("pdpMecanico")) Or grdetalle.GetValue("pdpMecanico").ToString = String.Empty) Then
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpMecanico") = 0
                grdetalle.SetValue("pdpMecanico", 0)

            End If
        End If

        ''''''''''''''''''PRECIO VENTA '''''''''   CONTINUARA  '''''''''''''
        'Habilitar solo las columnas de Precio, %, Monto y Observación

        If (e.Column.Index = grdetalle.RootTable.Columns("pdprven").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("pdprven")) Or grdetalle.GetValue("pdprven").ToString = String.Empty) Then

                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdprven") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("venta")
                Dim montodesc As Double = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("venta") - grdetalle.GetValue("pdpcost")
                Dim pordesc As Double = ((montodesc * 100) / CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost"))
            Else
                If (grdetalle.GetValue("pdprven") > 0) Then


                    'Dim montodesc As Double = grdetalle.GetValue("cbprven") - (grdetalle.GetValue("cbpcost") * tbTipoCambio.Value)
                    '    Dim pordesc As Double = ((montodesc * 100) / (grdetalle.GetValue("cbpcost") * tbTipoCambio.Value))
                    '    grdetalle.SetValue("cbutven", pordesc)

                Else

                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdprven") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("venta")
                    Dim montodesc As Double = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("venta") - CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost")
                    Dim pordesc As Double = ((montodesc * 100) / CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost"))
                End If
            End If
        End If



        ''''''''''''''''''PORCENTAJE PRECIO VENTA '''''''''   CONTINUARA  '''''''''''''
        'Habilitar solo las columnas de Precio, %, Monto y Observación

        If (e.Column.Index = grdetalle.RootTable.Columns("pdutven").Index) Then

            Dim venta As Double = IIf(IsDBNull(CType(grdetalle.DataSource, DataTable).Rows(pos).Item("venta")), 0, CType(grdetalle.DataSource, DataTable).Rows(pos).Item("venta"))
            'Dim PrecioCosto As Double = IIf(IsDBNull(grdetalle.GetValue("cbpcost")), 0, (grdetalle.GetValue("cbpcost") * tbTipoCambio.Value))
            If (Not IsNumeric(grdetalle.GetValue("pdutven")) Or grdetalle.GetValue("pdutven").ToString = String.Empty) Then

                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdprven") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("venta")
                Dim montodesc As Double = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("venta") - grdetalle.GetValue("pdpcost")

                Dim pordesc As Double = ((montodesc * 100) / CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost"))

                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdutven") = pordesc
            Else
                If (grdetalle.GetValue("pdutven") > 0) Then

                    'Dim porcentaje As Double = grdetalle.GetValue("cbutven")
                    'Dim monto As Double = ((grdetalle.GetValue("cbpcost") * tbTipoCambio.Value) * (porcentaje / 100))
                    '    Dim precioventa As Double = monto + (grdetalle.GetValue("cbpcost") * tbTipoCambio.Value)
                    '    grdetalle.SetValue("cbprven", precioventa)

                Else

                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdprven") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("venta")
                    Dim montodesc As Double = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("venta") - CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost")
                    Dim pordesc As Double = ((montodesc * 100) / CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdpcost"))
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pdutven") = pordesc
                End If
            End If
        End If
        Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado")
        If (estado = 1) Then
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
        End If



    End Sub
    Private Sub tbPdesc_ValueChanged(sender As Object, e As EventArgs) Handles tbPdesc.ValueChanged
        If (tbPdesc.Focused) Then
            If (Not tbPdesc.Text = String.Empty And Not tbtotal.Text = String.Empty) Then
                If (tbPdesc.Value = 0 Or tbPdesc.Value > 100) Then
                    tbPdesc.Value = 0
                    tbMdesc.Value = 0

                    _prCalcularPrecioTotal()

                Else

                    Dim porcdesc As Double = tbPdesc.Value
                    Dim montodesc As Double = (grdetalle.GetTotal(grdetalle.RootTable.Columns("pdptot"), AggregateFunction.Sum) * (porcdesc / 100))
                    tbMdesc.Value = montodesc
                    tbtotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("pdptot"), AggregateFunction.Sum) - montodesc
                End If


            End If
            If (tbPdesc.Text = String.Empty) Then
                tbMdesc.Value = 0

            End If
        End If
    End Sub

    Private Sub tbMdesc_ValueChanged(sender As Object, e As EventArgs) Handles tbMdesc.ValueChanged
        If (tbMdesc.Focused) Then

            Dim total As Double = tbtotal.Value
            If (Not tbMdesc.Text = String.Empty And Not tbMdesc.Text = String.Empty) Then
                If (tbMdesc.Value = 0 Or tbMdesc.Value > total) Then
                    tbMdesc.Value = 0
                    tbPdesc.Value = 0
                    _prCalcularPrecioTotal()
                Else
                    Dim montodesc As Double = tbMdesc.Value
                    Dim pordesc As Double = ((montodesc * 100) / grdetalle.GetTotal(grdetalle.RootTable.Columns("pdptot"), AggregateFunction.Sum))
                    tbPdesc.Value = pordesc
                    tbtotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("pdptot"), AggregateFunction.Sum) - montodesc

                End If

            End If

            If (tbMdesc.Text = String.Empty) Then
                tbMdesc.Value = 0

            End If
        End If

    End Sub


    Private Sub grdetalle_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellEdited
        If (e.Column.Index = grdetalle.RootTable.Columns("pdcmin").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("pdcmin")) Or grdetalle.GetValue("pdcmin").ToString = String.Empty) Then
                grdetalle.SetValue("pdcmin", 1)
                grdetalle.SetValue("pdptot", grdetalle.GetValue("pdpcost"))
            Else
                If (grdetalle.GetValue("pdcmin") > 0) Then

                Else
                    grdetalle.SetValue("pdcmin", 1)
                    grdetalle.SetValue("pdptot", grdetalle.GetValue("pdpcost"))
                End If
            End If
        End If
    End Sub
    Private Sub grdetalle_MouseClick(sender As Object, e As MouseEventArgs) Handles grdetalle.MouseClick
        If (Not _fnAccesible()) Then
            Return
        End If

        Try
            If (grdetalle.RowCount >= 2) Then
                If (grdetalle.CurrentColumn.Index = grdetalle.RootTable.Columns("img").Index) Then
                    _prEliminarFila()
                End If
            End If
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
                ''    _prInhabiliitar()

            End If
        End If

    End Sub

    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        Dim dt As DataTable = L_fnVerificarSiExisteEnCompra(tbCodigo.Text)
        If dt.Rows.Count > 0 Then
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "Esta Proforma no se puede modificar porque ya fue utilizada en Compra".ToUpper, img, 3500, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Exit Sub
        End If
        If (grProforma.RowCount > 0) Then
            _prhabilitar()
            btnNuevo.Enabled = False
            btnModificar.Enabled = False
            btnEliminar.Enabled = False
            btnGrabar.Enabled = True

            PanelNavegacion.Enabled = False
            _prCargarIconELiminar()
        End If

    End Sub
    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click


        Dim ef = New Efecto
        ef.tipo = 2
        ef.Context = "¿esta seguro de eliminar el registro?".ToUpper
        ef.Header = "mensaje principal".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean = L_fnEliminarProformaCompra(tbCodigo.Text, mensajeError)
            If res Then

                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
                ToastNotification.Show(Me, "Código de Proforma ".ToUpper + tbCodigo.Text + " eliminada con éxito.".ToUpper,
                                          img, 2000,
                                          eToastGlowColor.Green,
                                          eToastPosition.TopCenter)

                _prFiltrar()

            Else
                Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
                ToastNotification.Show(Me, mensajeError, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If
        End If

    End Sub

    Private Sub grVentas_SelectionChanged(sender As Object, e As EventArgs) Handles grProforma.SelectionChanged
        If (grProforma.RowCount >= 0 And grProforma.Row >= 0) Then
            _prMostrarRegistro(grProforma.Row)
        End If
    End Sub

    Private Sub btnSiguiente_Click(sender As Object, e As EventArgs) Handles btnSiguiente.Click
        Dim _pos As Integer = grProforma.Row
        If _pos < grProforma.RowCount - 1 And _pos >= 0 Then
            _pos = grProforma.Row + 1
            '' _prMostrarRegistro(_pos)
            grProforma.Row = _pos
        End If
    End Sub

    Private Sub btnUltimo_Click(sender As Object, e As EventArgs) Handles btnUltimo.Click
        Dim _pos As Integer = grProforma.Row
        If grProforma.RowCount > 0 Then
            _pos = grProforma.RowCount - 1
            ''  _prMostrarRegistro(_pos)
            grProforma.Row = _pos
        End If
    End Sub

    Private Sub btnAnterior_Click(sender As Object, e As EventArgs) Handles btnAnterior.Click
        Dim _MPos As Integer = grProforma.Row
        If _MPos > 0 And grProforma.RowCount > 0 Then
            _MPos = _MPos - 1
            ''  _prMostrarRegistro(_MPos)
            grProforma.Row = _MPos
        End If
    End Sub

    Private Sub btnPrimero_Click(sender As Object, e As EventArgs) Handles btnPrimero.Click
        _PrimerRegistro()
    End Sub
    Private Sub grVentas_KeyDown(sender As Object, e As KeyEventArgs) Handles grProforma.KeyDown
        If e.KeyData = Keys.Enter Then
            MSuperTabControl.SelectedTabIndex = 0
            grdetalle.Focus()

        End If
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


    Private Sub btnImprimir_Click(sender As Object, e As EventArgs) Handles btnImprimir.Click
        If tbCodigo.Text <> String.Empty Then
            P_GenerarReporteProforma()
        End If

    End Sub

    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        _HabilitarProductos()
        _prCalcularPrecioTotal()
    End Sub


#End Region

End Class