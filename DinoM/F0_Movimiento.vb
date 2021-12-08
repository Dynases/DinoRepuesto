
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
Imports DinoM.JanusExtension
Public Class F0_Movimiento
    Dim _Inter As Integer = 0
#Region "Variables Globales"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Dim Lote As Boolean = False
    Public _modulo As SideNavItem
    Dim Table_producto As DataTable
    Dim FilaSelectLote As DataRow = Nothing
    Dim dtProductoGoblal As DataTable

#End Region
#Region "Metodos Privados"
    Private Sub _IniciarTodo()
        MSuperTabControl.SelectedTabIndex = 0
        '' L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        _prValidarLote()
        'Me.WindowState = FormWindowState.Maximized
        _prCargarComboLibreriaConcepto(cbConcepto)
        _prCargarComboLibreriaDeposito(cbAlmacenOrigen)
        _prCargarComboLibreriaDeposito(cbDepositoDestino)
        _prCargarVenta()
        _prInhabiliitar()
        'Dim blah As New Bitmap(New Bitmap(My.Resources.compra), 20, 20)
        'Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        'Me.Icon = ico
        _prAsignarPermisos()
        Me.Text = "MOVIMIENTO PRODUCTOS"
        tbObservacion.MaxLength = 100
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
    Private Sub _prCargarComboLibreriaDeposito(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnListarDepositos()
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
    Private Sub _prCargarComboLibreriaConcepto(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_prMovimientoConcepto()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("cpnumi").Width = 60
            .DropDownList.Columns("cpnumi").Caption = "COD"
            .DropDownList.Columns.Add("cpdesc").Width = 250
            .DropDownList.Columns("cpdesc").Caption = "CONCEPTO"
            .ValueMember = "cpnumi"
            .DisplayMember = "cpdesc"
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

        cbConcepto.ReadOnly = True
        tbObservacion.ReadOnly = True
        tbFecha.IsInputReadOnly = True
        cbAlmacenOrigen.ReadOnly = True
        ''''''''''
        btnModificar.Enabled = True
        btnGrabar.Enabled = False
        btnNuevo.Enabled = True
        btnEliminar.Enabled = True
        grmovimiento.Enabled = True

        grdetalle.RootTable.Columns("img").Visible = False


        If (GPanelProductos.Visible = True) Then
            _DesHabilitarProductos()
        End If

        PanelInferior.Enabled = True
        FilaSelectLote = Nothing
    End Sub
    Private Sub _prhabilitar()
        'cbConcepto.ReadOnly = False
        tbObservacion.ReadOnly = False
        tbFecha.IsInputReadOnly = False
        cbAlmacenOrigen.ReadOnly = False
        grmovimiento.Enabled = False
        ''  tbCliente.ReadOnly = False  por que solo podra seleccionar Cliente
        ''  tbVendedor.ReadOnly = False
        If (tbCodigo.Text.Length > 0) Then
            cbAlmacenOrigen.ReadOnly = True
            cbConcepto.ReadOnly = True
        Else
            cbAlmacenOrigen.ReadOnly = False
            cbConcepto.ReadOnly = False

        End If
        btnGrabar.Enabled = True
    End Sub
    Public Sub _prFiltrar()
        'cargo el buscador
        Dim _Mpos As Integer
        _prCargarVenta()
        If grmovimiento.RowCount > 0 Then
            _Mpos = 0
            grmovimiento.Row = _Mpos
        Else
            _Limpiar()
            LblPaginacion.Text = "0/0"
        End If
    End Sub
    Private Sub _Limpiar()
        tbCodigo.Clear()
        tbObservacion.Clear()
        tbFecha.Value = Now.Date
        _prCargarDetalleVenta(-1)


        With grdetalle.RootTable.Columns("img")
            .Width = 80
            .Caption = "Eliminar"
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = True
        End With
        With grdetalle.RootTable.Columns("imgAdd")
            .Width = 80
            .Caption = "Nuevo"
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = True
        End With

        If (GPanelProductos.Visible = True) Then
            GPanelProductos.Visible = False
            PanelInferior.Visible = True
        End If
        If (CType(cbAlmacenOrigen.DataSource, DataTable).Rows.Count > 0) Then
            cbAlmacenOrigen.SelectedIndex = 0

        End If
        If (CType(cbConcepto.DataSource, DataTable).Rows.Count > 0) Then
            cbConcepto.SelectedIndex = 0

        End If
        _prAddDetalleVenta()
        cbConcepto.Focus()
        FilaSelectLote = Nothing
    End Sub
    Public Sub _prMostrarRegistro(_N As Integer)
        '      a.ibid ,a.ibfdoc ,a.ibconcep ,b.cpdesc as concepto,a.ibobs ,a.ibest ,a.ibalm ,a.ibiddc 
        ',a.ibfact ,a.ibhact ,a.ibuact,ibdepdest
        With grmovimiento
            tbCodigo.Text = .GetValue("ibid")
            tbFecha.Value = .GetValue("ibfdoc")
            cbConcepto.Value = .GetValue("ibconcep")
            tbObservacion.Text = .GetValue("ibobs")
            lbFecha.Text = CType(.GetValue("ibfact"), Date).ToString("dd/MM/yyyy")
            lbHora.Text = .GetValue("ibhact").ToString
            lbUsuario.Text = .GetValue("ibuact").ToString
            cbAlmacenOrigen.Value = .GetValue("ibalm")
            cbDepositoDestino.Value = IIf(IsDBNull(.GetValue("ibdepdest")), 0, .GetValue("ibdepdest"))
        End With

        _prCargarDetalleVenta(tbCodigo.Text)
        LblPaginacion.Text = Str(grmovimiento.Row + 1) + "/" + grmovimiento.RowCount.ToString

    End Sub

    Private Sub _prCargarDetalleVenta(_numi As String)
        Dim dt As New DataTable
        dt = L_fnDetalleMovimiento(_numi)
        grdetalle.DataSource = dt
        grdetalle.RetrieveStructure()
        grdetalle.AlternatingColors = True
        'a.icid ,a.icibid ,a.iccprod ,b.yfcdprod1  as producto,a.iccant ,
        '	a.iclot ,a.icfvenc ,Cast(null as image ) as img,1 as estado,
        '	(Sum(inv.iccven )+a.iccant  ) as stock
        With grdetalle.RootTable.Columns("icid")
            .Width = 100
            .Caption = "ITEM".ToUpper
            .Visible = True

        End With

        With grdetalle.RootTable.Columns("icibid")
            .Width = 90
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("iccprod")
            .Width = 90
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("CodigoFabrica")
            .Caption = "Cod.Fabrica".ToUpper
            .Width = 100
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
        End With
        With grdetalle.RootTable.Columns("CodigoMarca")
            .Caption = "Cod.Marca".ToUpper
            .Width = 100
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
        End With
        With grdetalle.RootTable.Columns("Medida")
            .Caption = "Medida".ToUpper
            .Width = 90
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
        End With

        With grdetalle.RootTable.Columns("CategoriaProducto")
            .Caption = "Cat.Producto".ToUpper
            .Width = 140
            .MaxLines = 100
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True

        End With
        With grdetalle.RootTable.Columns("producto")
            .Caption = "Productos".ToUpper
            .Width = 220
            .MaxLines = 200
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
        End With
        With grdetalle.RootTable.Columns("iccant")
            .Width = 160
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad"
        End With

        With grdetalle.RootTable.Columns("estado")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("img")
            .Width = 80
            .Caption = "Eliminar"
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("imgAdd")
            .Width = 40
            .Caption = "Nuevo"
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = False
        End With
        If (Lote = True) Then
            With grdetalle.RootTable.Columns("iclot")
                .Width = 120
                .Caption = "lote"
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = True
            End With
            With grdetalle.RootTable.Columns("icfvenc")
                .Width = 120
                .Caption = "FECHA VENC."
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .FormatString = "yyyy/MM/dd"
                .Visible = True
            End With
        Else

            With grdetalle.RootTable.Columns("iclot")
                .Width = 120
                .Caption = "lote"
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = False
            End With
            With grdetalle.RootTable.Columns("icfvenc")
                .Width = 120
                .Caption = "FECHA VENC."
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .FormatString = "yyyy/MM/dd"
                .Visible = False
            End With


        End If
        With grdetalle.RootTable.Columns("stock")
            .Width = 120
            .Caption = "stock"
            .Visible = False
        End With

        With grdetalle
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
        End With
    End Sub

    Private Sub _prCargarVenta()
        Dim dt As New DataTable
        dt = L_fnGeneralMovimiento()
        'grmovimiento.DataSource = dt
        'grmovimiento.RetrieveStructure()
        'grmovimiento.AlternatingColors = True

        ConfigInicialVinculado(grmovimiento, dt, "Movimiento")
        ColAL(grmovimiento, "ibid", "ITEM", 80)

        'With grmovimiento.RootTable.Columns("ibid")
        '    .Width = 100
        '    .Caption = "CODIGO"
        '    .Visible = True

        'End With

        With grmovimiento.RootTable.Columns("ibfdoc")
            .Width = 90
            .Visible = True
            .Caption = "FECHA"
            .FormatString = "yyyy/MM/dd"
        End With
        With grmovimiento.RootTable.Columns("ibconcep")
            .Width = 90
            .Visible = False
        End With

        With grmovimiento.RootTable.Columns("concepto")
            .Width = 160
            .Visible = True
            .Caption = "CONCEPTO"
        End With
        With grmovimiento.RootTable.Columns("ibobs")
            .Width = 250
            .Visible = True
            .Caption = "observacion".ToUpper
        End With


        With grmovimiento.RootTable.Columns("ibest")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grmovimiento.RootTable.Columns("ibalm")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grmovimiento.RootTable.Columns("ibiddc")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        '      a.ibid ,a.ibfdoc ,a.ibconcep ,b.cpdesc as concepto,a.ibobs ,a.ibest ,a.ibalm ,a.ibiddc 
        ',a.ibfact ,a.ibhact ,a.ibuact

        With grmovimiento.RootTable.Columns("ibfact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grmovimiento.RootTable.Columns("ibhact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grmovimiento.RootTable.Columns("ibuact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grmovimiento.RootTable.Columns("ibdepdest")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grmovimiento
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With

        If (dt.Rows.Count <= 0) Then
            _prCargarDetalleVenta(-1)
        End If
    End Sub
    Public Sub actualizarSaldoSinLote(ByRef dt As DataTable)
        'b.yfcdprod1 ,a.iclot ,a.icfven  ,a.iccven 

        'a.Item  ,a.yfcdprod1  ,a.yfcdprod2,Sum(b.iccven ) as stock
        'Dim _detalle As DataTable = CType(grdetalle.DataSource, DataTable)
        Dim _dtDetalle As DataTable = CType(grdetalle.DataSource, DataTable)
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim sum As Integer = 0
            Dim codProducto As Integer = dt.Rows(i).Item("Item")
            For j As Integer = 0 To _dtDetalle.Rows.Count - 1 Step 1

                Dim estado As Integer = _dtDetalle.Rows(j).Item("estado")
                If (estado = 0) Then
                    If (codProducto = _dtDetalle.Rows(j).Item("iccprod")) Then
                        sum = sum + _dtDetalle.Rows(j).Item("iccant")
                    End If
                End If
            Next
            dt.Rows(i).Item("stock") = dt.Rows(i).Item("stock") - sum
        Next

    End Sub
    Private Sub _prCargarProductos()
        Dim dtname As DataTable = L_fnNameLabel()
        ' Using a As New Object]
        If dtProductoGoblal Is Nothing Then
            If (Lote = True And cbConcepto.Value <> 1) Then
                dtProductoGoblal = L_prMovimientoListarProductosConLote(cbAlmacenOrigen.Value)  ''1=Almacen
                actualizarSaldoSinLote(dtProductoGoblal)
                'dtProductoGoblal = dt
            Else
                dtProductoGoblal = L_prMovimientoListarProductos(cbAlmacenOrigen.Value)  ''1=Almacen
                'dtProductoGoblal = dt
            End If
        End If




        Dim dtMovimiento As DataTable = dtProductoGoblal.Copy
        dtMovimiento.Rows.Clear()
        Dim detalle As DataTable = CType(grdetalle.DataSource, DataTable)
        For i As Integer = 0 To detalle.Rows.Count - 1

            If (detalle.Rows(i).Item("estado") >= 0) Then
                Dim codigoProducto As Integer = detalle.Rows(i).Item("iccprod")

                For j As Integer = 0 To dtProductoGoblal.Rows.Count - 1 Step 1

                    If (dtProductoGoblal.Rows(j).Item("Item") = codigoProducto) Then
                        dtProductoGoblal.Rows(j).Item("Cantidad") = detalle.Rows(i).Item("iccant")
                        'dt.Rows(j).Item("yhprecio") = detalle.Rows(i).Item("tbpbas")
                        dtMovimiento.ImportRow(dtProductoGoblal.Rows(j))
                    End If

                Next


            End If


        Next

        Dim frm As F0_DetalleMovimiento

        frm = New F0_DetalleMovimiento(dtProductoGoblal, dtMovimiento, dtname)
        frm.lbConcepto.Text = cbConcepto.Text

        frm.ShowDialog()
        Dim dtProd As DataTable = frm.dtDetalle

        For i As Integer = 0 To dtProd.Rows.Count - 1 Step 1

            InsertarProductosSinLote(dtProd, i)
        Next
        'End Using
        dtMovimiento.Rows.Clear()
    End Sub
    Public Sub actualizarSaldo(ByRef dt As DataTable, CodProducto As Integer)
        'b.yfcdprod1 ,a.iclot ,a.icfven  ,a.iccven 

        '      a.tbnumi ,a.tbtv1numi ,a.tbty5prod ,b.yfcdprod1 as producto,a.tbest ,a.tbcmin ,a.tbumin ,Umin .ycdes3 as unidad,a.tbpbas ,a.tbptot ,a.tbobs ,
        'a.tbpcos,a.tblote ,a.tbfechaVenc , a.tbptot2, a.tbfact ,a.tbhact ,a.tbuact,1 as estado,Cast(null as Image) as img,
        'Cast (0 as decimal (18,2)) as stock
        Dim _detalle As DataTable = CType(grdetalle.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim lote As String = dt.Rows(i).Item("iclot")
            Dim FechaVenc As Date = dt.Rows(i).Item("icfven")
            Dim sum As Integer = 0
            For j As Integer = 0 To _detalle.Rows.Count - 1
                Dim estado As Integer = _detalle.Rows(j).Item("estado")
                If (estado = 0) Then
                    If (lote = _detalle.Rows(j).Item("iclot") And
                        FechaVenc = _detalle.Rows(j).Item("icfvenc") And CodProducto = _detalle.Rows(j).Item("iccprod")) Then
                        sum = sum + _detalle.Rows(j).Item("iccant")
                    End If
                End If
            Next
            dt.Rows(i).Item("stock") = dt.Rows(i).Item("stock") - sum
        Next

    End Sub
    Private Sub _prCargarLotesDeProductos(CodProducto As Integer, nameProducto As String)
        If (cbAlmacenOrigen.SelectedIndex < 0) Then
            Return
        End If
        Dim dt As New DataTable
        GPanelProductos.Text = nameProducto
        dt = L_fnListarLotesPorProductoMovimiento(cbAlmacenOrigen.Value, CodProducto)  ''1=Almacen
        actualizarSaldo(dt, CodProducto)
        grProductos.DataSource = dt
        grProductos.RetrieveStructure()
        grProductos.AlternatingColors = True
        With grProductos.RootTable.Columns("yfcdprod1")
            .Width = 150
            .Visible = False

        End With
        'b.yfcdprod1 ,a.iclot ,a.icfven  ,a.iccven 
        With grProductos.RootTable.Columns("iclot")
            .Width = 150
            .Caption = "LOTE"
            .Visible = True

        End With
        With grProductos.RootTable.Columns("icfven")
            .Width = 160
            .Caption = "FECHA VENCIMIENTO"
            .FormatString = "yyyy/MM/dd"
            .Visible = True

        End With

        With grProductos.RootTable.Columns("stock")
            .Width = 150
            .Visible = True
            .Caption = "Stock"
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With


        With grProductos
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowPosition = TotalRowPosition.BottomFixed
            .VisualStyle = VisualStyle.Office2007
        End With
        _prAplicarCondiccionJanusLote()

    End Sub
    Public Sub _prAplicarCondiccionJanusLote()
        Dim fc As GridEXFormatCondition
        fc = New GridEXFormatCondition(grProductos.RootTable.Columns("stock"), ConditionOperator.Equal, 0)
        fc.FormatStyle.BackColor = Color.Gold
        fc.FormatStyle.FontBold = TriState.True
        fc.FormatStyle.ForeColor = Color.White
        grProductos.RootTable.FormatConditions.Add(fc)

        Dim fc2 As GridEXFormatCondition
        fc2 = New GridEXFormatCondition(grProductos.RootTable.Columns("icfven"), ConditionOperator.LessThanOrEqualTo, Now.Date)
        fc2.FormatStyle.BackColor = Color.Red
        fc2.FormatStyle.FontBold = TriState.True
        fc2.FormatStyle.ForeColor = Color.White
        grProductos.RootTable.FormatConditions.Add(fc2)
    End Sub
    Public Sub _prAplicarCondiccionJanusSinLote()
        Dim fc As GridEXFormatCondition
        fc = New GridEXFormatCondition(grProductos.RootTable.Columns("stock"), ConditionOperator.Equal, 0)
        'fc.FormatStyle.FontBold = TriState.True
        fc.FormatStyle.ForeColor = Color.Tan
        grProductos.RootTable.FormatConditions.Add(fc)
    End Sub
    Private Sub _prAddDetalleVenta()
        Try
            Dim Bin As New MemoryStream
            Dim Bin02 As New MemoryStream
            Dim img As New Bitmap(My.Resources.delete, 28, 28)
            Dim img02 As New Bitmap(My.Resources.add, 28, 28)
            img.Save(Bin, Imaging.ImageFormat.Png)
            img02.Save(Bin02, Imaging.ImageFormat.Png)

            'CType(grdetalle.DataSource, DataTable).Columns.Add("eliminar", Type.GetType("System.Image"))
            'CType(grdetalle.DataSource, DataTable).Columns.Add("nuevo", Type.GetType("System.Byte[]"))
            'CType(grdetalle.DataSource, DataTable).Columns("Eliminar").AllowDBNull = True
            'CType(grdetalle.DataSource, DataTable).Columns("Nuevo").AllowDBNull = True
            'Dim data As Byte() = Bin.ToArray()
            'Dim ms = New MemoryStream(data)
            'Dim imagen = Image.FromStream(ms)

            'CType(grdetalle.DataSource, DataTable).Columns("img").DataType = System.Type.GetType(“System.Image”)
            'CType(grdetalle.DataSource, DataTable).Columns("imgAdd").DataType = System.Type.GetType(“System.Image”)
            'Dim imagena As Byte() = Bin.ToArray()
            ''Dim imagenn As Image = Image.FromStream(Bin)
            ''Dim a As ImageConverter = New ImageConverter()
            'Bin.Write(imagena, 0, imagena.Length)

            CType(grdetalle.DataSource, DataTable).Rows.Add(_fnSiguienteNumi() + 1, 0, 0, "", "", "", "", "", 0, "20500101", CDate("2050/01/01"), Bin.GetBuffer(), Bin02.GetBuffer(), 0, 0)
            'CType(grdetalle.DataSource, DataTable).Rows("Eliminar")
            'CType(grdetalle.DataSource, DataTable).Rows(0).Add("Eliminar", Type.GetType("System.Iamge"))
            'CType(grdetalle.DataSource, DataTable).Columns.Add("Nuevo", Type.GetType("System.Iamge"))
            'CType(grdetalle.DataSource, DataTable).Columns("img") = a.ConvertTo(My.Resources.delete, System.Type.GetType("System.Byte[]"))

            ' ImageConverter.ConvertTo(Properties.Resources._1, System.Type.GetType("System.Byte[]"));
            '      var ImageConverter = New ImageConverter();
            ' row["Column1"] = imageConverter.ConvertTo(Properties.Resources._1, System.Type.GetType("System.Byte[]")); 

            '' Dim bs As System.IO.Stream
            ' Dim b(8000) As Byte

            ' bs = New System.IO.MemoryStream()
            ' b = dsFotos.Tables("Fotos").Rows(0)("Thumb")
            ' bs.Write(b, 0, b.Length)
            ' picImagen.Image = Image.FromStream(bs)



        Catch ex As Exception


            MostrarMensajeError(ex.Message)
        End Try


    End Sub
    Private Sub MostrarMensajeError(mensaje As String)
        ToastNotification.Show(Me,
                               mensaje.ToUpper,
                               My.Resources.WARNING,
                               5000,
                               eToastGlowColor.Red,
                               eToastPosition.TopCenter)

    End Sub
    Public Function _fnSiguienteNumi()
        Dim dt As DataTable = CType(grdetalle.DataSource, DataTable)
        Dim mayor As Integer = 0
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim data As Integer = IIf(IsDBNull(CType(grdetalle.DataSource, DataTable).Rows(i).Item("icid")), 0, CType(grdetalle.DataSource, DataTable).Rows(i).Item("icid"))
            If (data > mayor) Then
                mayor = data

            End If
        Next
        Return mayor
    End Function
    Public Function _fnAccesible()
        Return tbFecha.IsInputReadOnly = False
    End Function
    Private Sub _HabilitarProductos()
        'GPanelProductos.Visible = True

        'PanelInferior.Visible = False
        '_prCargarProductos()
        'grProductos.Focus()
        'grProductos.MoveTo(grProductos.FilterRow)
        'grProductos.Col = 1

        'GPanelProductos.Visible = True
        'PanelInferior.Visible = False
        'PanelInferior.Visible = False
        _prCargarProductos()
        'grProductos.Focus()
        'grProductos.MoveTo(grProductos.FilterRow)
        'grProductos.Col = 2
        'tbProducto.Clear()
        'tbProducto.Focus()

        'GPanelProductos.Height = 350
    End Sub
    Private Sub _DesHabilitarProductos()
        If (GPanelProductos.Visible = True) Then
            GPanelProductos.Visible = False
            PanelInferior.Visible = True
            grdetalle.Select()
            grdetalle.Col = 4
            grdetalle.Row = grdetalle.RowCount - 1
        End If
        FilaSelectLote = Nothing
    End Sub
    Public Sub _fnObtenerFilaDetalle(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("icid")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next

    End Sub

    Public Function _fnExisteProducto(idprod As Integer) As Boolean
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("iccprod")
            Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("estado")
            If (_idprod = idprod And estado >= 0) Then

                Return True
            End If
        Next
        Return False
    End Function
    Public Sub _prEliminarFila()
        If (grdetalle.Row >= 0) Then
            If (grdetalle.RowCount >= 2) Then
                Dim estado As Integer = grdetalle.GetValue("estado")
                Dim pos As Integer = -1
                Dim lin As Integer = grdetalle.GetValue("icid")
                _fnObtenerFilaDetalle(pos, lin)
                If (estado = 0) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = -2

                End If
                If (estado = 1) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = -1
                End If
                grdetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grdetalle.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))

                grdetalle.Select()
                grdetalle.Col = 4
                grdetalle.Row = grdetalle.RowCount - 1
            End If
        End If


    End Sub
    Public Function _ValidarCampos() As Boolean
        If (cbConcepto.SelectedIndex < 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor Seleccione un Concepto".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            cbConcepto.Focus()
            Return False

        End If
        If (cbAlmacenOrigen.SelectedIndex < 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor Seleccione un Deposito".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            cbAlmacenOrigen.Focus()
            Return False
        End If
        If (cbConcepto.SelectedIndex >= 0) Then
            If (cbConcepto.Value = 6) Then
                If (cbDepositoDestino.SelectedIndex < 0) Then
                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, "Por Favor Seleccione un Deposito Desitno".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                    cbDepositoDestino.Focus()
                    Return False
                End If
            End If
        End If
        If (grdetalle.RowCount <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor Inserte un Detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            grdetalle.Focus()

            Return False

        End If
        If (grdetalle.RowCount = 1) Then
            If (CType(grdetalle.DataSource, DataTable).Rows(0).Item("iccprod") = 0) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "Por Favor Inserte un Detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                grdetalle.Focus()

                Return False
            End If
        End If
        Return True
    End Function

    Sub _prGuardarTraspaso()
        Dim numi As String = ""
        Dim res As Boolean = L_prMovimientoChoferGrabar(numi, tbFecha.Value.ToString("yyyy/MM/dd"), cbConcepto.Value, tbObservacion.Text, cbAlmacenOrigen.Value, cbDepositoDestino.Value, 0, CType(grdetalle.DataSource, DataTable))
        If res Then

            Dim numDestino As String = ""
            Dim resDestino As Boolean = L_prMovimientoChoferGrabar(numDestino, tbFecha.Value.ToString("yyyy/MM/dd"), 5, tbObservacion.Text, cbDepositoDestino.Value, cbAlmacenOrigen.Value, numi, CType(grdetalle.DataSource, DataTable))
            If resDestino Then

                _prCargarVenta()

                _Limpiar()
                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
                ToastNotification.Show(Me, "Código de Movimiento ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper,
                                          img, 2000,
                                          eToastGlowColor.Green,
                                          eToastPosition.TopCenter
                                          )
            End If

        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "El Movimiento no pudo ser insertado".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
        End If
    End Sub
    Public Sub _GuardarNuevo()
        'ByRef _ibid As String, _ibfdoc As String, _ibconcep As Integer, _ibobs As String, _almacen As Integer

        If (cbConcepto.Value = 6) Then
            _prGuardarTraspaso()
            Return

        End If
        Dim numi As String = ""
        Dim res As Boolean = L_prMovimientoChoferGrabar(numi, tbFecha.Value.ToString("yyyy/MM/dd"), cbConcepto.Value, tbObservacion.Text, cbAlmacenOrigen.Value, 0, 0, CType(grdetalle.DataSource, DataTable))
        If res Then

            _prCargarVenta()

            _Limpiar()
            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Movimiento ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "El Movimiento no pudo ser insertado".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
        End If


    End Sub
    Private Sub _prGuardarModificado()
        Dim res As Boolean = L_prMovimientoModificar(tbCodigo.Text, tbFecha.Value.ToString("yyyy/MM/dd"), cbConcepto.Value, tbObservacion.Text, cbAlmacenOrigen.Value, CType(grdetalle.DataSource, DataTable))
        If res Then

            _prCargarVenta()

            _prSalir()

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Movimiento ".ToUpper + tbCodigo.Text + " Modificado con Exito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )

        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La Venta no pudo ser Modificada".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If
    End Sub
    Private Sub _prSalir()
        If btnGrabar.Enabled = True Then
            _prInhabiliitar()
            If grmovimiento.RowCount > 0 Then

                _prMostrarRegistro(0)

            End If
        Else

            _modulo.Select()
            Me.Close()

        End If
    End Sub
    Public Sub _prCargarIconELiminar()
        If (cbConcepto.Value <> 3) Then
            For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
                Dim Bin As New MemoryStream
                Dim img As New Bitmap(My.Resources.delete, 28, 28)
                img.Save(Bin, Imaging.ImageFormat.Png)
                CType(grdetalle.DataSource, DataTable).Rows(i).Item("img") = Bin.GetBuffer
                grdetalle.RootTable.Columns("img").Visible = True
            Next
        End If
    End Sub
    Public Sub _PrimerRegistro()
        Dim _MPos As Integer
        If grmovimiento.RowCount > 0 Then
            _MPos = 0
            ''   _prMostrarRegistro(_MPos)
            grmovimiento.Row = _MPos
        End If
    End Sub

    Public Sub InsertarProductosSinLote(dt As DataTable, fila As Integer)
        '      a.icid ,a.icibid ,a.iccprod ,b.yfcdprod1  as producto,a.iccant ,
        'a.iclot ,a.icfvenc ,Cast(null as image ) as img,1 as estado,
        '(Sum(inv.iccven )+a.iccant  ) as stock
        'If (dt.Rows(fila).Item("Stock") <= 0) Then
        '    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
        '    ToastNotification.Show(Me, "El producto no tiene stock disponible".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
        '    Return
        'End If

        Dim pos As Integer = -1
        grdetalle.Row = grdetalle.RowCount - 1
        If (grdetalle.GetValue("iccprod") <> 0) Then
            _prAddDetalleVenta()
            grdetalle.Row = grdetalle.RowCount - 1
        End If
        _fnObtenerFilaDetalle(pos, grdetalle.GetValue("icid"))

        Dim existe As Boolean = _fnExisteProducto(dt.Rows(fila).Item("Item"))
        If ((pos >= 0) And (Not existe)) Then
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("iccprod") = dt.Rows(fila).Item("Item")

            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("CodigoFabrica") = dt.Rows(fila).Item("CodigoFabrica")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("CodigoMarca") = dt.Rows(fila).Item("Marca")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Medida") = dt.Rows(fila).Item("Medida")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("CategoriaProducto") = dt.Rows(fila).Item("Categoria")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("producto") = dt.Rows(fila).Item("yfcdprod1")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("iccant") = dt.Rows(fila).Item("Cantidad")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("stock") = dt.Rows(fila).Item("stock")

            ''    _DesHabilitarProductos()

            '_prAddDetalleVenta()

            '_prCargarProductos(0)
            ''grproducto.RemoveFilters()
            'grProductos.Focus()
            'grProductos.MoveTo(grProductos.FilterRow)
            'grProductos.Col = 1


            tbProducto.Focus()
        Else
            If (existe) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "El producto ya existe en el detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                grProductos.RemoveFilters()
                grProductos.Focus()
                grProductos.MoveTo(grProductos.FilterRow)
                grProductos.Col = 1
            End If

        End If
    End Sub
    Public Sub InsertarProductosConLote()

        'Dim pos As Integer = -1
        'grdetalle.Row = grdetalle.RowCount - 1
        '_fnObtenerFilaDetalle(pos, grdetalle.GetValue("icid"))
        'Dim posProducto As Integer = grproducto.Row
        'FilaSelectLote = CType(grproducto.DataSource, DataTable).Rows(posProducto)



        'If (grproducto.GetValue("stock") > 0) Then
        '    _prCargarLotesDeProductos(grproducto.GetValue("Item"), grproducto.GetValue("yfcdprod1"))
        '    'FilaSelectLote = CType(grproducto.DataSource, DataTable).Rows(0)
        'Else
        '    Dim img As Bitmap = New Bitmap(My.Resources.Mensaje, 50, 50)
        '    ToastNotification.Show(Me, "El Producto: ".ToUpper + grproducto.GetValue("yfcdprod1") + " NO CUENTA CON STOCK DISPONIBLE", img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
        '    FilaSelectLote = Nothing
        'End If

        Dim pos As Integer = -1
        grdetalle.Row = grdetalle.RowCount - 1
        _fnObtenerFilaDetalleProducto(pos, grProductos.GetValue("Item"))
        Dim posProducto As Integer = grProductos.Row
        FilaSelectLote = CType(grProductos.DataSource, DataTable).Rows(pos)
        If (grProductos.GetValue("stock") > 0) Then
            _prCargarLotesDeProductos(grProductos.GetValue("Item"), grProductos.GetValue("yfcdprod1"))
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "El Producto: ".ToUpper + grProductos.GetValue("yfcdprod1") + " NO CUENTA CON STOCK DISPONIBLE", img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            FilaSelectLote = Nothing
        End If
    End Sub
    Public Function _fnExisteProductoConLote(idprod As Integer, lote As String, fechaVenci As Date) As Boolean
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("iccprod")
            Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("estado")
            '          a.tbnumi ,a.tbtv1numi ,a.tbty5prod ,b.yfcdprod1 as producto,a.tbest ,a.tbcmin ,a.tbumin ,Umin .ycdes3 as unidad,a.tbpbas ,a.tbptot ,a.tbobs ,
            'a.tbpcos,a.tblote ,a.tbfechaVenc , a.tbptot2, a.tbfact ,a.tbhact ,a.tbuact,1 as estado,Cast(null as Image) as img,
            'Cast (0 as decimal (18,2)) as stock
            Dim _LoteDetalle As String = CType(grdetalle.DataSource, DataTable).Rows(i).Item("iclot")
            Dim _FechaVencDetalle As Date = CType(grdetalle.DataSource, DataTable).Rows(i).Item("icfvenc")
            If (_idprod = idprod And estado >= 0 And lote = _LoteDetalle And fechaVenci = _FechaVencDetalle) Then

                Return True
            End If
        Next
        Return False
    End Function
    Public Sub _fnObtenerFilaDetalleProducto(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(grProductos.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grProductos.DataSource, DataTable).Rows(i).Item("Item")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next

    End Sub


#End Region

#Region "Eventos Formulario"



    Private Sub F0_Movimiento_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _IniciarTodo()
        btnNuevo.PerformClick()
    End Sub

    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        _Limpiar()
        _prhabilitar()

        btnNuevo.Enabled = False
        btnModificar.Enabled = False
        btnEliminar.Enabled = False
        btnGrabar.Enabled = True
        PanelInferior.Enabled = False
    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        _prSalir()
    End Sub

    Private Sub grdetalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grdetalle.EditingCell
        If (_fnAccesible()) Then

            'Habilitar solo las columnas de Precio, %, Monto y Observación
            If (e.Column.Index = grdetalle.RootTable.Columns("iccant").Index) Then
                e.Cancel = False
            Else
                If ((e.Column.Index = grdetalle.RootTable.Columns("iclot").Index Or
                    e.Column.Index = grdetalle.RootTable.Columns("icfvenc").Index) And (cbConcepto.Value = 1) And
                Lote = True) Then
                    e.Cancel = False
                Else
                    e.Cancel = True
                End If
            End If


        Else
            e.Cancel = True
        End If
    End Sub
    Public Function SeleccionarCategoria(newItem As Boolean) As Integer
        Dim dt As DataTable
        Dim idCategoria As Integer = 0
        Dim nombreCategoria As String
        dt = L_fnListarCategoriaVentas()
        '   yccod3,ycdes3 

        Dim listEstCeldas As New List(Of Modelo.Celda)
        listEstCeldas.Add(New Modelo.Celda("yccod3,", True, "Codigo", 100))
        listEstCeldas.Add(New Modelo.Celda("ycdes3", True, "Nombre Categoria", 500))

        Dim ef = New Efecto
        ef.tipo = 3
        ef.dt = dt
        ef.SeleclCol = 2
        ef.listEstCeldas = listEstCeldas
        ef.alto = 50
        ef.ancho = 500
        ef.Context = "Seleccione Categoria".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row
            ''yccod3,ycdes3 
            idCategoria = Row.Cells("yccod3").Value
            nombreCategoria = Row.Cells("ycdes3").Value
            If (idCategoria > 0) Then
                'If (newItem = True) Then
                '    _prAddDetalleVenta()
                'End If
                _HabilitarProductos()
            End If
        End If
        Return idCategoria
    End Function
    Private Sub grdetalle_KeyDown(sender As Object, e As KeyEventArgs) Handles grdetalle.KeyDown
        If (Not _fnAccesible()) Then
            Return
        End If

        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = grdetalle.Col
            f = grdetalle.Row

            If (grdetalle.Col = grdetalle.RootTable.Columns("iccant").Index) Then
                If (grdetalle.GetValue("producto") <> String.Empty) Then
                    '_prAddDetalleVenta()
                    _HabilitarProductos()
                    ' SeleccionarCategoria(True)
                Else
                    ToastNotification.Show(Me, "Seleccione un Producto Por Favor", My.Resources.WARNING, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If

            End If
            If (grdetalle.Col = grdetalle.RootTable.Columns("producto").Index) Then
                If (grdetalle.GetValue("producto") <> String.Empty) Then
                    '_prAddDetalleVenta()
                    _HabilitarProductos()
                    'SeleccionarCategoria(True)
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
            'SeleccionarCategoria(True)
        End If
        If (e.KeyData = Keys.Escape And grdetalle.Row >= 0) Then

            _prEliminarFila()


        End If



    End Sub


    Private Sub grproducto_KeyDown(sender As Object, e As KeyEventArgs)
        If (Not _fnAccesible()) Then
            Return
        End If
        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = grProductos.Col
            f = grProductos.Row
            If (f >= 0) Then

                If (IsNothing(FilaSelectLote)) Then
                    ''''''''''''''''''''''''
                    If (Lote = True) Then
                        If (cbConcepto.Value = 1) Then ''' Aqui pregunto si es con lote y tambien si es una 
                            ''entrada debe solo insertar solo el producto y no seguir con los lotes 
                            'InsertarProductosSinLote()

                        Else
                            InsertarProductosConLote()
                        End If

                    Else

                        'InsertarProductosSinLote()
                    End If
                    '''''''''''''''
                Else

                    '      a.icid ,a.icibid ,a.iccprod ,b.yfcdprod1  as producto,a.iccant ,
                    'a.iclot ,a.icfvenc ,Cast(null as image ) as img,1 as estado,
                    '(Sum(inv.iccven )+a.iccant  ) as stock

                    'a.Item  ,a.yfcdprod1  ,a.yfcdprod2,Sum(b.iccven ) as stock 
                    '_fnExisteProductoConLote()
                    'b.yfcdprod1 ,a.iclot ,a.icfven  ,Sum(a.iccven)as stock 
                    Dim pos As Integer = -1
                    grdetalle.Row = grdetalle.RowCount - 1
                    _fnObtenerFilaDetalle(pos, grdetalle.GetValue("icid"))
                    Dim numiProd As Integer = FilaSelectLote.Item("Item")
                    Dim lote As String = grProductos.GetValue("iclot")
                    Dim FechaVenc As Date = grProductos.GetValue("icfven")
                    If (Not _fnExisteProductoConLote(numiProd, lote, FechaVenc)) Then
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("iccprod") = FilaSelectLote.Item("Item")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("producto") = FilaSelectLote.Item("yfcdprod1")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("iccant") = 1

                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("stock") = grProductos.GetValue("stock")

                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("iclot") = grProductos.GetValue("iclot")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("icfvenc") = grProductos.GetValue("icfven")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("CodigoFabrica") = FilaSelectLote.Item("CodigoFabrica")

                        FilaSelectLote = Nothing
                        _DesHabilitarProductos()

                    Else
                        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                        ToastNotification.Show(Me, "El producto con el lote ya existe modifique su cantidad".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                    End If



                End If


            End If
        End If
        If e.KeyData = Keys.Escape Then
            CType(grProductos.DataSource, DataTable).Rows.Clear()

            _DesHabilitarProductos()
        End If
    End Sub

    Private Sub grdetalle_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellValueChanged

        If (e.Column.Index = grdetalle.RootTable.Columns("iccant").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("iccant")) Or grdetalle.GetValue("iccant").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1
                Dim lin As Integer = grdetalle.GetValue("icid")
                Dim pos As Integer = -1
                _fnObtenerFilaDetalle(pos, lin)
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("iccant") = 1

                Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado")

                If (estado = 1) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            Else
                If (grdetalle.GetValue("iccant") > 0) Then
                    Dim lin As Integer = grdetalle.GetValue("icid")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado")

                    If (estado = 1) Then
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                Else
                    Dim lin As Integer = grdetalle.GetValue("icid")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("iccant") = 1
                    Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado")

                    If (estado = 1) Then
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                End If
            End If
        End If
    End Sub

    Private Sub grdetalle_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellEdited
        If (e.Column.Index = grdetalle.RootTable.Columns("iccant").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("iccant")) Or grdetalle.GetValue("iccant").ToString = String.Empty) Then

                grdetalle.SetValue("iccant", 1)
            Else
                If (grdetalle.GetValue("iccant") > 0) Then
                    Dim stock As Double = grdetalle.GetValue("stock")
                    If (grdetalle.GetValue("iccant") > stock And cbConcepto.Value <> 1) Then
                        Dim lin As Integer = grdetalle.GetValue("icid")
                        Dim pos As Integer = -1
                        _fnObtenerFilaDetalle(pos, lin)
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("iccant") = stock
                        grdetalle.SetValue("iccant", stock)
                        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                        ToastNotification.Show(Me, "La cantidad que se quiere sacar es mayor a la que existe en el stock solo puede Sacar : ".ToUpper + Str(stock).Trim,
                          img,
                          5000,
                          eToastGlowColor.Blue,
                          eToastPosition.BottomLeft)
                    End If
                Else

                    grdetalle.SetValue("iccant", 1)

                End If
            End If
        End If
    End Sub

    Private Sub grdetalle_MouseClick(sender As Object, e As MouseEventArgs) Handles grdetalle.MouseClick
        If (Not _fnAccesible()) Then
            Return
        End If

        If (grdetalle.RowCount >= 2) Then
            If (grdetalle.CurrentColumn.Index = grdetalle.RootTable.Columns("img").Index) Then
                _prEliminarFila()
            End If
        End If
        Try
            If (grdetalle.CurrentColumn.Index = grdetalle.RootTable.Columns("imgAdd").Index) Then
                _HabilitarProductos()
                ' SeleccionarCategoria(True)
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
        If (grmovimiento.RowCount > 0) Then
            _prhabilitar()
            btnNuevo.Enabled = False
            btnModificar.Enabled = False
            btnEliminar.Enabled = False
            btnGrabar.Enabled = True

            PanelInferior.Enabled = False
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
            Dim res As Boolean = L_prMovimientoEliminar(tbCodigo.Text)
            If res Then


                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)

                ToastNotification.Show(Me, "Código de Movimiento ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper,
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

    Private Sub grmovimiento_SelectionChanged(sender As Object, e As EventArgs) Handles grmovimiento.SelectionChanged
        If (grmovimiento.RowCount >= 0 And grmovimiento.Row >= 0) Then

            _prMostrarRegistro(grmovimiento.Row)
        End If
    End Sub

    Private Sub btnSiguiente_Click(sender As Object, e As EventArgs) Handles btnSiguiente.Click
        Dim _pos As Integer = grmovimiento.Row
        If _pos < grmovimiento.RowCount - 1 Then
            _pos = grmovimiento.Row + 1
            '' _prMostrarRegistro(_pos)
            grmovimiento.Row = _pos
        End If
    End Sub

    Private Sub btnPrimero_Click(sender As Object, e As EventArgs) Handles btnPrimero.Click
        _PrimerRegistro()
    End Sub

    Private Sub btnUltimo_Click(sender As Object, e As EventArgs) Handles btnUltimo.Click
        Dim _pos As Integer = grmovimiento.Row
        If grmovimiento.RowCount > 0 Then
            _pos = grmovimiento.RowCount - 1
            ''  _prMostrarRegistro(_pos)
            grmovimiento.Row = _pos
        End If
    End Sub

    Private Sub btnAnterior_Click(sender As Object, e As EventArgs) Handles btnAnterior.Click
        Dim _MPos As Integer = grmovimiento.Row
        If _MPos > 0 And grmovimiento.RowCount > 0 Then
            _MPos = _MPos - 1
            ''  _prMostrarRegistro(_MPos)
            grmovimiento.Row = _MPos
        End If
    End Sub

    Private Sub grdetalle_Enter(sender As Object, e As EventArgs) Handles grdetalle.Enter
        If (grdetalle.RowCount > 0) Then
            grdetalle.Select()
            grdetalle.Col = 3
            grdetalle.Row = 0
        End If
    End Sub

    Private Sub grmovimiento_KeyDown(sender As Object, e As KeyEventArgs) Handles grmovimiento.KeyDown
        If e.KeyData = Keys.Enter Then
            MSuperTabControl.SelectedTabIndex = 0
            grdetalle.Focus()

        End If
    End Sub

    Private Sub cbAlmacen_KeyDown(sender As Object, e As KeyEventArgs) Handles cbAlmacenOrigen.KeyDown
        If (_fnAccesible()) Then
            If e.KeyData = Keys.Enter Then
                grdetalle.Focus()
                'grdetalle.Select()
                grdetalle.Col = 2
                grdetalle.Row = 0
            End If
        End If
    End Sub

    Private Sub cbConcepto_ValueChanged(sender As Object, e As EventArgs) Handles cbConcepto.ValueChanged


        If (cbConcepto.SelectedIndex >= 0) Then
            If (cbConcepto.Value = 6) Then ''''Movimiento 6=Traspaso Salida
                If (CType(cbAlmacenOrigen.DataSource, DataTable).Rows.Count > 1) Then
                    lbDepositoDestino.Visible = True
                    cbDepositoDestino.Visible = True
                    lbDepositoOrigen.Text = "Deposito Origen"
                    lbDepositoDestino.Text = "Deposito Destino"
                    cbDepositoDestino.SelectedIndex = 1
                    If (Not _fnAccesible()) Then
                        btnModificar.Enabled = False
                    End If
                Else
                    lbDepositoDestino.Visible = False
                    cbDepositoDestino.Visible = False
                    lbDepositoOrigen.Text = "Deposito:"
                    If (Not _fnAccesible()) Then
                        btnModificar.Enabled = True
                    End If
                End If
            Else
                btnModificar.Enabled = True
                lbDepositoDestino.Visible = False
                cbDepositoDestino.Visible = False
                lbDepositoOrigen.Text = "Deposito:"

            End If
            If (_fnAccesible() And tbCodigo.Text = String.Empty) Then
                CType(grdetalle.DataSource, DataTable).Rows.Clear()
                _prAddDetalleVenta()
                _DesHabilitarProductos()
            End If
        End If
    End Sub

    Private Sub cbAlmacenOrigen_ValueChanged(sender As Object, e As EventArgs) Handles cbAlmacenOrigen.ValueChanged
        If (_fnAccesible() And tbCodigo.Text = String.Empty) Then
            CType(grdetalle.DataSource, DataTable).Rows.Clear()
            _prAddDetalleVenta()
            _DesHabilitarProductos()

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

    Private Sub grProductos_DoubleClick(sender As Object, e As EventArgs)

    End Sub

    Private Sub grdetalle_DoubleClick(sender As Object, e As EventArgs) Handles grdetalle.DoubleClick
        If (GPanelProductos.Visible = True) Then
            _DesHabilitarProductos()
        End If
    End Sub

    Private Sub tbProducto_KeyDown(sender As Object, e As KeyEventArgs) Handles tbProducto.KeyDown
        If e.KeyData = Keys.Escape Then
            _DesHabilitarProductos()
            FilaSelectLote = Nothing
        End If
        If e.KeyData = Keys.Down Then
            grProductos.Focus()
        End If
    End Sub

    Private Sub tbProducto_TextChanged(sender As Object, e As EventArgs) Handles tbProducto.TextChanged

        Dim dtProductoCopy As DataTable
        dtProductoCopy = dtProductoGoblal.Copy
        dtProductoCopy.Rows.Clear()
        Dim dt As DataTable = dtProductoGoblal.Copy

        Dim charSequence As String
        charSequence = tbProducto.Text.ToUpper
        If (charSequence.Trim <> String.Empty) Then
            Dim cantidad As Integer = 12
            Dim cont As Integer = 12

            'Split con array de delimitadores
            Dim delimitadores() As String = {" ", ".", ",", ";", "-"}
            Dim vectoraux() As String
            vectoraux = charSequence.Split(delimitadores, StringSplitOptions.None)

            'mostrar resultado
            'For Each item As String In vectoraux


            '    Console.WriteLine("'{0}'", item)
            'Next
            Dim cant As Integer = vectoraux.Length

            For i As Integer = 0 To dt.Rows.Count - 1 Step 1
                Dim nombre As String = dt.Rows(i).Item("yfcdprod1").ToString.ToUpper
                Select Case cant
                    Case 1

                        If (nombre.Trim.Contains(vectoraux(0))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 2
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 3
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 4
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 5
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 6
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 7

                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 8
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 9
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 10
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 11
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 12
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If


                    Case 13
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 14
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 15
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 16
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And nombre.Trim.Contains(vectoraux(15))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 17
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And nombre.Trim.Contains(vectoraux(15)) And nombre.Trim.Contains(vectoraux(16))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 18
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And nombre.Trim.Contains(vectoraux(15)) And nombre.Trim.Contains(vectoraux(16)) And nombre.Trim.Contains(vectoraux(17))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If



                End Select

            Next
            grProductos.DataSource = dtProductoCopy.Copy
        Else
            grProductos.DataSource = dtProductoGoblal.Copy
        End If
    End Sub

    Private Sub grProductos_DoubleClick_1(sender As Object, e As EventArgs) Handles grProductos.DoubleClick
        If (Not _fnAccesible()) Then
            Return
        End If
        Dim f, c As Integer
        c = grProductos.Col
        f = grProductos.Row
        If (f >= 0) Then

            If (IsNothing(FilaSelectLote)) Then
                ''''''''''''''''''''''''
                If (Lote = True) Then
                    If (cbConcepto.Value = 1) Then ''' Aqui pregunto si es con lote y tambien si es una 
                        ''entrada debe solo insertar solo el producto y no seguir con los lotes 
                        'InsertarProductosSinLote()

                    Else
                        InsertarProductosConLote()
                    End If

                Else

                    'InsertarProductosSinLote()
                End If
                '''''''''''''''
            Else

                '      a.icid ,a.icibid ,a.iccprod ,b.yfcdprod1  as producto,a.iccant ,
                'a.iclot ,a.icfvenc ,Cast(null as image ) as img,1 as estado,
                '(Sum(inv.iccven )+a.iccant  ) as stock

                'a.Item  ,a.yfcdprod1  ,a.yfcdprod2,Sum(b.iccven ) as stock 
                '_fnExisteProductoConLote()
                'b.yfcdprod1 ,a.iclot ,a.icfven  ,Sum(a.iccven)as stock 
                Dim pos As Integer = -1
                grdetalle.Row = grdetalle.RowCount - 1
                _fnObtenerFilaDetalle(pos, grdetalle.GetValue("icid"))
                Dim numiProd As Integer = FilaSelectLote.Item("Item")
                Dim lote As String = grProductos.GetValue("iclot")
                Dim FechaVenc As Date = grProductos.GetValue("icfven")
                If (Not _fnExisteProductoConLote(numiProd, lote, FechaVenc)) Then
                    If (grProductos.GetValue("Stock") <= 0) Then
                        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                        ToastNotification.Show(Me, "El producto no tiene stock disponible".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                    End If
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("iccprod") = FilaSelectLote.Item("Item")
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("producto") = FilaSelectLote.Item("yfcdprod1")
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("iccant") = 1

                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("stock") = grProductos.GetValue("stock")

                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("iclot") = grProductos.GetValue("iclot")
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("icfvenc") = grProductos.GetValue("icfven")

                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Laboratorio") = FilaSelectLote.Item("Laboratorio")
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Presentacion") = FilaSelectLote.Item("Presentacion")
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("CodigoFabrica") = FilaSelectLote.Item("CodigoFabrica")

                    FilaSelectLote = Nothing
                    _DesHabilitarProductos()

                Else
                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, "El producto con el lote ya existe modifique su cantidad".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                End If



            End If


        End If

    End Sub

    Private Sub grProductos_KeyDown(sender As Object, e As KeyEventArgs) Handles grProductos.KeyDown
        If (Not _fnAccesible()) Then
            Return
        End If
        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = grProductos.Col
            f = grProductos.Row
            If (f >= 0) Then

                If (IsNothing(FilaSelectLote)) Then
                    ''''''''''''''''''''''''
                    If (Lote = True) Then
                        If (cbConcepto.Value = 1) Then ''' Aqui pregunto si es con lote y tambien si es una 
                            ''entrada debe solo insertar solo el producto y no seguir con los lotes 
                            'InsertarProductosSinLote()

                        Else
                            InsertarProductosConLote()
                        End If

                    Else

                        'InsertarProductosSinLote()
                    End If
                    '''''''''''''''
                Else

                    '      a.icid ,a.icibid ,a.iccprod ,b.yfcdprod1  as producto,a.iccant ,
                    'a.iclot ,a.icfvenc ,Cast(null as image ) as img,1 as estado,
                    '(Sum(inv.iccven )+a.iccant  ) as stock

                    'a.Item  ,a.yfcdprod1  ,a.yfcdprod2,Sum(b.iccven ) as stock 
                    '_fnExisteProductoConLote()
                    'b.yfcdprod1 ,a.iclot ,a.icfven  ,Sum(a.iccven)as stock 
                    Dim pos As Integer = -1
                    grdetalle.Row = grdetalle.RowCount - 1
                    _fnObtenerFilaDetalle(pos, grdetalle.GetValue("icid"))
                    Dim numiProd As Integer = FilaSelectLote.Item("Item")
                    Dim lote As String = grProductos.GetValue("iclot")
                    Dim FechaVenc As Date = grProductos.GetValue("icfven")
                    If (Not _fnExisteProductoConLote(numiProd, lote, FechaVenc)) Then

                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("iccprod") = FilaSelectLote.Item("Item")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("producto") = FilaSelectLote.Item("yfcdprod1")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("iccant") = 1

                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("stock") = grProductos.GetValue("stock")

                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("iclot") = grProductos.GetValue("iclot")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("icfvenc") = grProductos.GetValue("icfven")

                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Laboratorio") = FilaSelectLote.Item("Laboratorio")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Presentacion") = FilaSelectLote.Item("Presentacion")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("CodigoFabrica") = FilaSelectLote.Item("CodigoFabrica")

                        FilaSelectLote = Nothing
                        _DesHabilitarProductos()

                    Else
                        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                        ToastNotification.Show(Me, "El producto con el lote ya existe modifique su cantidad".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                    End If



                End If


            End If
        End If
        If e.KeyData = Keys.Escape Then
            CType(grProductos.DataSource, DataTable).Rows.Clear()

            _DesHabilitarProductos()
            FilaSelectLote = Nothing
        End If
    End Sub
    Private Sub SeleccionarProductosEvento()

    End Sub
#End Region
End Class