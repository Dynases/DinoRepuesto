Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports System.Drawing
Imports DevComponents.DotNetBar.Controls
Imports System.Transactions
Imports DinoM.JanusExtension
Imports DinoM.JanusVerificaciones
Imports DinoM.JanusMetodos
Public Class F0_MovimientoNuevo
    Dim _Inter As Integer = 0
#Region "Variables Globales"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Dim Lote As Boolean = False
    Public _modulo As SideNavItem
    Dim Table_producto As DataTable
    Dim FilaSelectLote As DataRow = Nothing
    Dim dtProductoGoblal As DataTable
    Dim tMovimiento As DataTable
    Dim detalleId As Integer = 0
#End Region
#Region "Metodos Privados"
    Private Sub _IniciarTodo()
        MSuperTabControl.SelectedTabIndex = 0
        _prValidarLote()
        _prCargarComboLibreriaConcepto(cbConcepto)
        _prCargarComboLibreriaDeposito(cbAlmacenOrigen)
        _prCargarComboLibreriaDeposito(cbDepositoDestino)
        _prCargarMovimiento()
        _prInhabiliitar()
        _prAsignarPermisos()
        Me.Text = "MOVIMIENTO PRODUCTOS"
        tbObservacion.MaxLength = 100
        cbConcepto.BackColor = Color.Yellow
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

        grdetalle.RootTable.Columns("stock").Visible = False
        grdetalle.RootTable.Columns("img").Visible = False
        grdetalle.RootTable.Columns("imgAdd").Visible = False

        PanelInferior.Enabled = True
        FilaSelectLote = Nothing
        Panel_AlmacenGrupo.Visible = False
    End Sub
    Private Sub _prhabilitar()
        tbObservacion.ReadOnly = False
        tbFecha.IsInputReadOnly = False
        cbAlmacenOrigen.ReadOnly = False
        grmovimiento.Enabled = False
        If (tbCodigo.Text.Length > 0) Then
            cbAlmacenOrigen.ReadOnly = True
            cbConcepto.ReadOnly = True
        Else
            cbAlmacenOrigen.ReadOnly = False
            cbConcepto.ReadOnly = False
        End If
        If cbConcepto.Value <> 6 Then
            ColArNro(grdetalle, "stock", "Stock", 80, "0.00")
            ColArNro(grdetalle, "stockTotal", "Total", 80, "0.00")
        End If
        btnGrabar.Enabled = True
        'Panel_AlmacenGrupo.Visible = True
    End Sub
    Public Sub _prFiltrar()
        Dim _Mpos As Integer
        _prCargarMovimiento()
        If grmovimiento.RowCount > 0 Then
            _Mpos = 0
            grmovimiento.Row = _Mpos
        Else
            _Limpiar()
            LblPaginacion.Text = "0/0"
        End If
    End Sub
    Private Sub _Limpiar()
        Try
            tbCodigo.Clear()
            tbObservacion.Clear()
            tbFecha.Value = Now.Date
            cargarDetalle(-1)
            With grdetalle.RootTable.Columns("img")
                .Width = 70
                .Caption = "Eliminar"
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = True
            End With
            With grdetalle.RootTable.Columns("imgAdd")
                .Width = 70
                .Caption = "Nuevo"
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = True
            End With
            If cbConcepto.Value <> 6 Then
                ColArNro(grdetalle, "stock", "Stock", 80, "0.00")
                ColArNro(grdetalle, "stockTotal", "Total", 80, "0.00")
            End If
            If (CType(cbAlmacenOrigen.DataSource, DataTable).Rows.Count > 0) Then
                cbAlmacenOrigen.SelectedIndex = 0
            End If
            _prAddDetalleVenta()
            If (CType(cbConcepto.DataSource, DataTable).Rows.Count > 0) Then
                cbConcepto.SelectedIndex = 0
            End If

            cbConcepto.Focus()
            FilaSelectLote = Nothing
            limpiarAlmacenGrupo()
            Panel_AlmacenGrupo.Visible = False
            Panel_AlmacenGrupoTraspaso.Visible = False
            lblTituloAlmacen.Text = "ALMACEN DE DESTINO"
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try

    End Sub
    Public Sub _prMostrarRegistro(_N As Integer)
        Try
            With grmovimiento
                tbCodigo.Text = .GetValue("Id")
                tbFecha.Value = .GetValue("Fecha")
                cbConcepto.Value = .GetValue("ConceptoId")
                tbObservacion.Text = .GetValue("Observacion")
                lbFecha.Text = CType(.GetValue("FechaReg"), Date).ToString("dd/MM/yyyy")
                lbHora.Text = .GetValue("HoraReg").ToString
                lbUsuario.Text = .GetValue("UsuarioReg").ToString
            End With
            cargarDetalle(tbCodigo.Text)
            LblPaginacion.Text = Str(grmovimiento.Row + 1) + "/" + grmovimiento.RowCount.ToString
        Catch ex As Exception
            MostrarMensaje(ex.Message)
        End Try

    End Sub

    Private Sub _prCargarMovimiento()
        tMovimiento = l_obtenerMovimeintos()
        ConfigInicialVinculado(grmovimiento, tMovimiento, "Movimiento")
        ColAL(grmovimiento, "Id", "ITEM", 80)
        ColALFecha(grmovimiento, "Fecha", "FECHA", 90)
        ColNoVisible(grmovimiento, "ConceptoId")
        ColAL(grmovimiento, "Concepto", "Concepto".ToUpper, 200)
        ColAL(grmovimiento, "observacion", "observacion".ToUpper, 250)
        ColNoVisible(grmovimiento, "Estado")
        ColNoVisible(grmovimiento, "FechaReg")
        ColNoVisible(grmovimiento, "HoraReg")
        ColNoVisible(grmovimiento, "UsuarioReg")
        ConfigFinalBasica(grmovimiento)
        If (tMovimiento.Rows.Count <= 0) Then
            cargarDetalle(-1)
        End If
    End Sub
    Private Sub cargarDetalle(movimientoId As String)
        Try
            Dim tdetalle As New DataTable
            tdetalle = l_obtenerDetalleMovimiento(movimientoId)
            ConfigInicialVinculado(grdetalle, tdetalle, "Movimiento")
            ColNoVisible(grdetalle, "Id")
            ColNoVisible(grdetalle, "movimientoId")
            ColAL(grdetalle, "ProductoId", "Item", 60)
            ColAL(grdetalle, "CodigoFabrica", "Cod. Fabrica", 100)
            ColAL(grdetalle, "CodigoMarca", "Cod. Marca", 100)
            ColAL(grdetalle, "Medida", "Medida", 90)
            ColAL(grdetalle, "Marca", "Marca ", 90)
            ColAL(grdetalle, "Procedencia", "Procedencia ", 100)
            ColAL(grdetalle, "producto", "Producto", 200)
            ColArNro(grdetalle, "cantidad", "Cantidad", 100, "0")
            ColCombo(grdetalle, "AlmOrigenId", "Alm. Origen", 140, cbAlmacenOrigen, True)
            ColCombo(grdetalle, "AlmDestinoId", "Alm. Destino", 140, cbDepositoDestino, IIf(cbConcepto.Value = 6, True, False))
            ColNoVisible(grdetalle, "img")
            ColNoVisible(grdetalle, "imgAdd")
            ColNoVisible(grdetalle, "estado")
            ColNoVisible(grdetalle, "stock")
            ColNoVisible(grdetalle, "stockTotal")
            ConfigFinalDetalle(grdetalle)
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try

    End Sub
    Public Sub actualizarSaldoSinLote(ByRef dt As DataTable)
        Dim _dtDetalle As DataTable = CType(grdetalle.DataSource, DataTable)
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim sum As Integer = 0
            Dim codProducto As Integer = dt.Rows(i).Item("Item")
            For j As Integer = 0 To _dtDetalle.Rows.Count - 1 Step 1

                Dim estado As Integer = _dtDetalle.Rows(j).Item("estado")
                If (estado = 0) Then
                    If (codProducto = _dtDetalle.Rows(j).Item("ProductoId")) Then
                        sum = sum + _dtDetalle.Rows(j).Item("Cantidad")
                    End If
                End If
            Next
            dt.Rows(i).Item("stock") = dt.Rows(i).Item("stock") - sum
        Next
    End Sub
    Private Sub _prCargarProductos()
        Dim dtname As DataTable = L_fnNameLabel()
        If dtProductoGoblal Is Nothing Then
            If (Lote = True And cbConcepto.Value <> 1) Then
                dtProductoGoblal = L_prMovimientoListarProductosConLote(cbAlmacenOrigen.Value)
                actualizarSaldoSinLote(dtProductoGoblal)
            Else
                dtProductoGoblal = L_prMovimientoListarProductos(cbAlmacenOrigen.Value)
            End If
        End If
        Dim dtMovimiento As DataTable = dtProductoGoblal.Copy
        dtMovimiento.Rows.Clear()
        Dim detalle As DataTable = CType(grdetalle.DataSource, DataTable)
        For i As Integer = 0 To detalle.Rows.Count - 1

            If (detalle.Rows(i).Item("estado") >= 0) Then
                Dim codigoProducto As Integer = detalle.Rows(i).Item("ProductoId")

                For j As Integer = 0 To dtProductoGoblal.Rows.Count - 1 Step 1
                    If (dtProductoGoblal.Rows(j).Item("Item") = codigoProducto) Then
                        dtProductoGoblal.Rows(j).Item("Cantidad") = detalle.Rows(i).Item("Cantidad")
                        dtMovimiento.ImportRow(dtProductoGoblal.Rows(j))
                    End If
                Next
            End If
        Next
        Dim frm As F0_DetalleMovimiento
        frm = New F0_DetalleMovimiento(dtProductoGoblal, dtMovimiento, dtname)
        frm.lbConcepto.Text = cbConcepto.Text

        frm.ShowDialog()
        dtProductoGoblal = frm.dtProductoAll
        Dim dtProd As DataTable = frm.dtDetalle
        For i As Integer = 0 To dtProd.Rows.Count - 1 Step 1
            InsertarProductosSinLote(dtProd, i)
        Next
        dtMovimiento.Rows.Clear()
    End Sub
    Public Sub actualizarSaldo(ByRef dt As DataTable, CodProducto As Integer)

        Dim _detalle As DataTable = CType(grdetalle.DataSource, DataTable)
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim lote As String = dt.Rows(i).Item("iclot")
            Dim FechaVenc As Date = dt.Rows(i).Item("icfven")
            Dim sum As Integer = 0
            For j As Integer = 0 To _detalle.Rows.Count - 1
                Dim estado As Integer = _detalle.Rows(j).Item("estado")
                If (estado = 0) Then
                    If (lote = _detalle.Rows(j).Item("iclot") And
                        FechaVenc = _detalle.Rows(j).Item("icfvenc") And CodProducto = _detalle.Rows(j).Item("ProductoId")) Then
                        sum = sum + _detalle.Rows(j).Item("Cantidad")
                    End If
                End If
            Next
            dt.Rows(i).Item("stock") = dt.Rows(i).Item("stock") - sum
        Next

    End Sub

    Private Sub _prAddDetalleVenta()
        Try
            Dim Bin As New MemoryStream
            Dim Bin02 As New MemoryStream
            ObtenerImagenAddDetalle(Bin, Bin02)
            'Obtiene el Id Mayor
            Dim idMayor As Integer = ObtenerIdMayor(grdetalle, "Id")
            CType(grdetalle.DataSource, DataTable).Rows.Add(idMayor + 1, 0, 0, "", "", "", "", "", "", 0, gi_userSuc, 2, Bin.GetBuffer(), Bin02.GetBuffer(), 0, 0, 0)
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
    Private Sub MostrarMensaje(mensaje As String)
        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
        ToastNotification.Show(Me,
                               mensaje.ToUpper,
                               img,
                               2000,
                               eToastGlowColor.Red,
                               eToastPosition.TopCenter)

    End Sub
    Public Function _fnAccesible()
        Return tbFecha.IsInputReadOnly = False
    End Function
    Private Sub _HabilitarProductos()
        _prCargarProductos()
    End Sub
    Public Function _fnExisteProducto(idprod As Integer) As Boolean
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("ProductoId")
            Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("estado")
            If (_idprod = idprod And estado >= 0) Then
                Return True
            End If
        Next
        Return False
    End Function
    Public Function _ValidarCampos() As Boolean
        If (cbConcepto.SelectedIndex < 0) Then
            MostrarMensaje("Por Favor Seleccione un Concepto")
            Return False
        End If
        If (cbAlmacenOrigen.SelectedIndex < 0) Then
            MostrarMensaje("Por Favor Seleccione un Deposito")
            cbAlmacenOrigen.Focus()
            Return False
        End If
        If (tbObservacion.Text = String.Empty) Then
            MostrarMensaje("Por introducir una observación")
            tbObservacion.Focus()
            Return False
        End If

        If (cbConcepto.SelectedIndex >= 0) Then
            If (cbConcepto.Value = 6) Then
                If (cbDepositoDestino.SelectedIndex < 0) Then
                    MostrarMensaje("Por Favor Seleccione un Deposito Desitno")
                    cbDepositoDestino.Focus()
                    Return False
                End If
            End If
        End If
        If (grdetalle.RowCount <= 0) Then
            MostrarMensaje("Por Favor Inserte un Detalle")
            grdetalle.Focus()
            Return False
        End If
        If (grdetalle.RowCount = 1) Then
            If (CType(grdetalle.DataSource, DataTable).Rows(0).Item("ProductoId") = 0) Then
                MostrarMensaje("Por Favor Inserte un Detalle")
                grdetalle.Focus()
                Return False
            End If
        End If
        Dim cantidad = jMetodo_SumarFila(grdetalle, "Cantidad")
        If cantidad = 0 Then
            MostrarMensaje("Por Favor inserte una cantidad mayor a 0 en el detalle")
            grdetalle.Focus()
            Return False
        End If
        Return True
    End Function
    Private Sub MostrarMensajeOk(mensaje As String)
        ToastNotification.Show(Me,
                               mensaje.ToUpper,
                               My.Resources.OK,
                               5000,
                               eToastGlowColor.Green,
                               eToastPosition.TopCenter)
    End Sub



    Public Function validarStock(ByRef Mensaje As String) As Boolean


        Dim dt As DataTable = CType(grdetalle.DataSource, DataTable).DefaultView.ToTable(False, "ProductoId", "producto", "Cantidad", "AlmOrigenId", "estado", "stock")
        Dim detalle As DataTable = dt.Copy
        detalle.Rows.Clear()
        'Armo los productos para consultar su inventario
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("estado") >= 0) Then

                detalle.ImportRow(dt.Rows(i))

            End If
        Next
        ''''''''''
        Dim dtInventario As DataTable = L_fnObtenerStockAlmacen(detalle)

        Dim bandera As Boolean = True

        Dim Cadena As String = "Los siguientes productos ya no tienen Stock=  "

        For i As Integer = 0 To detalle.Rows.Count - 1 Step 1
            Dim ProductoId As Integer = detalle.Rows(i).Item("ProductoId")
            Dim AlmacenId As Integer = detalle.Rows(i).Item("AlmOrigenId")
            Dim result As Object
            result = detalle.Compute("SUM(Cantidad)", "ProductoId = " + Str(ProductoId) + " and AlmOrigenId=" + Str(AlmacenId))
            Dim CantidadTotal As Integer = Integer.Parse(result.ToString)


            Dim resultInventario As Object
            resultInventario = dtInventario.Compute("SUM(Cantidad)", "ProductoId = " + Str(ProductoId) + " and AlmacenId=" + Str(AlmacenId))

            Dim CantidadInventario As Double = Double.Parse(resultInventario.ToString)

            If (CantidadTotal > CantidadInventario) Then
                bandera = False
                Cadena += "" + detalle.Rows(i).Item("producto") + "  ,"
            End If

        Next

        Mensaje = Cadena
        Return bandera




    End Function

    Public Sub _GuardarNuevo()
        Try

            Dim Mensaje As String = ""
            Dim Id As String = ""
            Dim resultado As Boolean = False
            If (validarStock(Mensaje)) Then


                Using sb As New TransactionScope
                    resultado = l_MovimientoGuardar(Id, tbFecha.Value.ToString("yyyy/MM/dd"), cbConcepto.Value,
                                                                tbObservacion.Text, CType(grdetalle.DataSource, DataTable))
                    If resultado Then
                        _prCargarMovimiento()
                        _Limpiar()
                        MostrarMensajeOk("Código de Movimiento ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper)
                        sb.Complete()
                    Else
                        MostrarMensajeError("El Movimiento no pudo ser insertado".ToUpper)
                    End If
                End Using
            Else
                MostrarMensajeError(Mensaje.ToUpper)
            End If

        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub
    Private Sub _prGuardarModificado()
        Dim Id As String = ""
        Dim resultado As Boolean = False
        Using sb As New TransactionScope
            resultado = l_MovimientoModificar(tbCodigo.Text, tbFecha.Value.ToString("yyyy/MM/dd"), cbConcepto.Value,
                                                           tbObservacion.Text, CType(grdetalle.DataSource, DataTable))
            If resultado Then
                _prCargarMovimiento()
                _prSalir()
                MostrarMensajeOk("Código de Movimiento ".ToUpper + tbCodigo.Text + " Modificado con Exito.".ToUpper)
                sb.Complete()
            Else
                MostrarMensajeError("El Movimiento no pudo ser modificado".ToUpper)
            End If
        End Using
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
                If grmovimiento.RowCount > 0 Then
                    _prMostrarRegistro(0)
                End If
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

                Dim Bin2 As New MemoryStream
                Dim img2 As New Bitmap(My.Resources.add, 28, 28)
                img2.Save(Bin2, Imaging.ImageFormat.Png)
                CType(grdetalle.DataSource, DataTable).Rows(i).Item("imgAdd") = Bin2.GetBuffer
                grdetalle.RootTable.Columns("imgAdd").Visible = True

            Next
        End If
    End Sub
    Public Sub InsertarProductosSinLote(dt As DataTable, fila As Integer)
        grdetalle.Row = grdetalle.RowCount - 1
        Dim existe As Boolean = _fnExisteProducto(dt.Rows(fila).Item("Item"))

        If (Not existe) Then
            If (grdetalle.GetValue("ProductoId") <> 0) Then
                _prAddDetalleVenta()
                grdetalle.Row = grdetalle.RowCount - 1
            End If
            Dim posicionFila As Integer = ObtenerPosicionFila(grdetalle, "Id", grdetalle.GetValue("id"))

            If ((posicionFila >= 0) And (Not existe)) Then
                SetCelValor(grdetalle, posicionFila, "ProductoId", dt.Rows(fila).Item("Item"))
                SetCelValor(grdetalle, posicionFila, "CodigoFabrica", dt.Rows(fila).Item("CodigoFabrica"))
                SetCelValor(grdetalle, posicionFila, "CodigoMarca", dt.Rows(fila).Item("Marca"))
                SetCelValor(grdetalle, posicionFila, "Medida", dt.Rows(fila).Item("Medida"))
                SetCelValor(grdetalle, posicionFila, "Marca", dt.Rows(fila).Item("Grupo1"))
                SetCelValor(grdetalle, posicionFila, "Procedencia", dt.Rows(fila).Item("Grupo2"))
                SetCelValor(grdetalle, posicionFila, "producto", dt.Rows(fila).Item("yfcdprod1"))
                SetCelValor(grdetalle, posicionFila, "Cantidad", dt.Rows(fila).Item("Cantidad"))
                SetCelValor(grdetalle, posicionFila, "stock", dt.Rows(fila).Item("stock"))
                SetCelValor(grdetalle, posicionFila, "AlmOrigenId", gi_userSuc)
            Else
                'If (existe) Then
                '    MostrarMensaje("El producto ya existe en el detalle")
                'End If
            End If
        End If

    End Sub

    Private Sub P_GenerarReporte()
        Dim dt As DataTable = l_ReporteMovimiento(tbCodigo.Text)


        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If

        P_Global.Visualizador = New Visualizador

        Dim objrep As New R_Movimiento
        objrep.SetDataSource(dt)
        objrep.SetParameterValue("usuario", L_Usuario)

        P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        P_Global.Visualizador.ShowDialog() 'Comentar
        P_Global.Visualizador.BringToFront()


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
            If (e.Column.Index = grdetalle.RootTable.Columns("AlmOrigenId").Index Or
                e.Column.Index = grdetalle.RootTable.Columns("AlmDestinoId").Index) Then
                e.Cancel = False
            Else
                e.Cancel = True
            End If
        Else
            e.Cancel = True
        End If
    End Sub
    Private Sub grdetalle_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellValueChanged
        Try
            'Celda Cantidad
            If (e.Column.Index = CelIndex(grdetalle, "Cantidad")) Then
                Dim cantidad As Integer = grdetalle.GetValue("Cantidad")
                Dim stock As Integer = grdetalle.GetValue("Stock")
                Dim posicion = ObtenerPosicionFila(grdetalle, "Id", grdetalle.GetValue("id"))
                Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(posicion).Item("estado")

                ValidarExistenciaStock(grdetalle, cantidad, stock, posicion, 0, 2)

                If (Not IsNumeric(cantidad) Or
                    cantidad.ToString = String.Empty Or
                    cantidad < 0) Then

                    CType(grdetalle.DataSource, DataTable).Rows(posicion).Item("Cantidad") = 0
                    grdetalle.SetValue("Cantidad", 0)
                    cambiarEstado(posicion, estado)
                Else
                    cambiarEstado(posicion, estado)
                    If (cantidad = 0) Then
                        CType(grdetalle.DataSource, DataTable).Rows(posicion).Item("Cantidad") = 0
                        grdetalle.SetValue("Cantidad", 0)
                        CType(grdetalle.DataSource, DataTable).Rows(posicion).Item("stockTotal") = stock
                        grdetalle.SetValue("stockTotal", stock)
                    Else
                        Dim stockTotal = IIf(cbConcepto.Value = 1,
                                                 stock + cantidad,
                                                 stock - cantidad)
                        CType(grdetalle.DataSource, DataTable).Rows(posicion).Item("stockTotal") = stockTotal
                        grdetalle.SetValue("stockTotal", stockTotal)
                    End If
                End If
            End If

            'Celda Almacen de origen
            If (e.Column.Index = CelIndex(grdetalle, "AlmOrigenId") Or e.Column.Index = CelIndex(grdetalle, "AlmDestinoId")) Then

                Dim posicion = ObtenerPosicionFila(grdetalle, "Id", grdetalle.GetValue("id"))
                If grdetalle.GetValue("AlmOrigenId") = grdetalle.GetValue("AlmDestinoId") Then
                    MostrarMensaje("Debe selecionar un almacen distinto de Origen y Destino")
                    SetCelValor(grdetalle, posicion, "Stock", 0)
                    Return
                End If

                Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(posicion).Item("estado")
                Dim productoId As Integer = grdetalle.GetValue("ProductoId")
                Dim almacenId As Integer = grdetalle.GetValue("AlmOrigenId")
                If productoId <> 0 And almacenId <> 0 Then
                    Dim stock = l_obtenerStockXAlmacenYProducto(almacenId, productoId)
                    SetCelValor(grdetalle, posicion, "Stock", stock)
                    seleccionarCeldaCantidad(posicion)
                    cambiarEstado(posicion, estado)
                End If
            End If
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub
    Public Sub seleccionarCeldaCantidad(posicion As Integer)
        Dim celdadCantidad As Integer = 8
        grdetalle.Select()
        grdetalle.Col = celdadCantidad
        grdetalle.Row = posicion
    End Sub
    Private Sub cargarDetalleAlmacen(grid As GridEX, productoId As Integer, validadarTabla As Boolean, Optional ValidarStock As Boolean = False)
        Try
            Dim tAlmacen As DataTable = l_obtenerAlmacensXIdProducto(productoId)
            If validadarTabla Then
                jVerificar_ExisteFIlaDatatable(tAlmacen)
            End If
            If (ValidarStock) Then

                For i As Integer = 0 To tAlmacen.Rows.Count - 1 Step 1
                    Dim AlmacenId As Integer = tAlmacen.Rows(i).Item("AlmacenId")
                    Dim Stock As Integer = 0
                    Dim dt As DataTable = CType(grdetalle.DataSource, DataTable)

                    For j As Integer = 0 To dt.Rows.Count - 1 Step 1

                        If (dt.Rows(j).Item("ProductoId") = productoId And dt.Rows(j).Item("estado") >= 0 And dt.Rows(j).Item("AlmOrigenId") = AlmacenId) Then
                            Stock += dt.Rows(j).Item("Cantidad")

                        End If

                    Next

                    tAlmacen.Rows(i).Item("stock") = tAlmacen.Rows(i).Item("stock") - Stock



                Next

            End If


            armarDetalleAlmacenGrid(grid, tAlmacen)
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub
    Public Sub limpiarAlmacenGrupo()
        cargarDetalleAlmacen(grAlmacen, -1, False)
        cargarDetalleAlmacen(grAlmacenSalida, -1, False)
        lbProductoId.Text = "0"
        LblProducto.Text = ""
        lbStock.Text = "0"
        detalleId = 0
    End Sub
    Private Sub armarDetalleAlmacenGrid(griex As GridEX, tabla As DataTable)
        ConfigInicialVinculado(griex, tabla, "Almacenes")
        ColAL(griex, "almacenId", "Id", 60)
        ColAL(griex, "almacen", "Almacen", 150)
        ColArNro(griex, "stock", "Stock", 110, "0")
        ColArNro(griex, "cantidad", "Cantidad", 120, "0")
        ConfigFinalDetalle(griex)
    End Sub
    Private Sub cambiarEstado(posicion As Integer, estado As Integer)
        If (estado = 1) Then
            CType(grdetalle.DataSource, DataTable).Rows(posicion).Item("estado") = 2
        End If
    End Sub
    Private Sub grdetalle_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellEdited
        If (e.Column.Index = CelIndex(grdetalle, "Cantidad")) Then
            If (Not IsNumeric(grdetalle.GetValue("Cantidad")) Or
                grdetalle.GetValue("Cantidad").ToString = String.Empty Or
                grdetalle.GetValue("Cantidad") < 0) Then
                grdetalle.SetValue("Cantidad", 0)
            Else
                If (grdetalle.GetValue("Cantidad") > 0) Then
                    Dim stock As Double = grdetalle.GetValue("stock")
                    If (grdetalle.GetValue("Cantidad") > stock And cbConcepto.Value = 2) Then

                        Dim pos As Integer = ObtenerPosicionFila(grdetalle, "Id", grdetalle.GetValue("id"))

                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = stock
                        grdetalle.SetValue("Cantidad", stock)
                        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                        ToastNotification.Show(Me, "La cantidad que se quiere sacar es mayor a la que existe en el stock solo puede Sacar : ".ToUpper + Str(stock).Trim,
                          img,
                          5000,
                          eToastGlowColor.Blue,
                          eToastPosition.BottomLeft)
                    End If
                Else
                    grdetalle.SetValue("Cantidad", 0)
                End If
            End If
        End If
    End Sub

    Private Sub grdetalle_MouseClick(sender As Object, e As MouseEventArgs) Handles grdetalle.MouseClick
        Try
            If (Not _fnAccesible()) Then
                Return
            End If

            If (grdetalle.RowCount >= 2) Then
                If (grdetalle.CurrentColumn.Index = grdetalle.RootTable.Columns("img").Index) Then

                    EliminarFilaDetalle(grdetalle, "Id", "Estado", grdetalle.GetValue("Id"))
                    Panel_AlmacenGrupoTraspaso.Visible = False
                    Panel_AlmacenGrupo.Visible = False
                End If
            End If



            If (grdetalle.CurrentColumn.Index = grdetalle.RootTable.Columns("imgAdd").Index) Then
                _HabilitarProductos()
            End If
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub
    Public Sub _prAplicarCondiccionJanusSeleccion()
        Dim fc2 As GridEXFormatCondition
        fc2 = New GridEXFormatCondition(grdetalle.RootTable.Columns("ProductoId"), ConditionOperator.Equal, IIf(lbProductoId.Text = String.Empty, 0, lbProductoId.Text))
        fc2.FormatStyle.BackColor = Color.DarkSalmon
        fc2.FormatStyle.FontBold = TriState.True
        fc2.FormatStyle.ForeColor = Color.White
        grdetalle.RootTable.FormatConditions.Add(fc2)
        'Dim colores As Color = grdetalle.FormatStyles.

    End Sub
    Public Sub _prAplicarCondiccionJanusDesSeleccion()
        Dim fc2 As GridEXFormatCondition
        fc2 = New GridEXFormatCondition(grdetalle.RootTable.Columns("ProductoId"), ConditionOperator.NotEqual, IIf(lbProductoId.Text = String.Empty, 0, lbProductoId.Text))
        fc2.FormatStyle.BackColor = Color.White
        fc2.FormatStyle.FontBold = TriState.False
        fc2.FormatStyle.ForeColor = Color.Black
        grdetalle.RootTable.FormatConditions.Add(fc2)
        'Dim colores As Color = grdetalle.FormatStyles.

    End Sub
    Private Sub EliminarFilaDetalle(griex As GridEX, key As String, keyEstado As String, valor As Integer)
        Dim colProducto = 7
        EliminarFIla(griex, key, keyEstado, valor)
        grdetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grdetalle.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))
        grdetalle.Select()
        grdetalle.Col = 7
        grdetalle.Row = grdetalle.RowCount - 1
    End Sub
    Private Sub btnGrabar_Click(sender As Object, e As EventArgs) Handles btnGrabar.Click
        If _ValidarCampos() = False Then
            Exit Sub
        End If
        If (tbCodigo.Text = String.Empty) Then
            _GuardarNuevo()
        Else
            _prGuardarModificado()
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
            Dim res As Boolean = l_MovimientoEliminar(tbCodigo.Text)
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
    Private Sub btnPrimero_Click(sender As Object, e As EventArgs) Handles btnPrimero.Click
        navegarPrimerRegistro(grmovimiento)
    End Sub
    Private Sub btnSiguiente_Click(sender As Object, e As EventArgs) Handles btnSiguiente.Click
        navegarSiguienteRegistro(grmovimiento)
    End Sub
    Private Sub btnUltimo_Click(sender As Object, e As EventArgs) Handles btnUltimo.Click
        navegarUltimoRegistro(grmovimiento)
    End Sub

    Private Sub btnAnterior_Click(sender As Object, e As EventArgs) Handles btnAnterior.Click
        navegarAnteriorRegistro(grmovimiento)
    End Sub

    Private Sub grdetalle_Enter(sender As Object, e As EventArgs) Handles grdetalle.Enter
        If (grdetalle.RowCount > 0) Then
            grdetalle.Select()
            SeleccionarColumnaFila(grdetalle, 3, 0)
        End If
    End Sub

    Private Sub cbAlmacen_KeyDown(sender As Object, e As KeyEventArgs) Handles cbAlmacenOrigen.KeyDown, grdetalle.KeyDown
        If (_fnAccesible()) Then
            If e.KeyData = Keys.Enter Then
                grdetalle.Focus()
                SeleccionarColumnaFila(grdetalle, 2, 0)
            End If
        End If
    End Sub

    Private Sub cbConcepto_ValueChanged(sender As Object, e As EventArgs) Handles cbConcepto.ValueChanged
        Panel_AlmacenGrupo.Visible = False
        Panel_AlmacenGrupoTraspaso.Visible = False
        If (cbConcepto.SelectedIndex >= 0) Then
            If (cbConcepto.Value = 6) Then ''''Movimiento 6=Traspaso Salida
                If (CType(cbAlmacenOrigen.DataSource, DataTable).Rows.Count > 1) Then
                    cbDepositoDestino.SelectedIndex = 1
                    If grdetalle.RowCount > 0 Then
                        ColCombo(grdetalle, "AlmDestinoId", "Alm. Destino", 150, cbDepositoDestino, True)
                        ColNoVisible(grdetalle, "stock")
                        ColNoVisible(grdetalle, "stockTotal")
                    End If
                    If (Not _fnAccesible()) Then
                        btnModificar.Enabled = False
                    End If
                Else
                    If (Not _fnAccesible()) Then
                        btnModificar.Enabled = True
                    End If
                End If
            Else
                btnModificar.Enabled = True
                If grdetalle.RowCount > 0 Then
                    ColCombo(grdetalle, "AlmDestinoId", "Alm. Destino", 150, cbDepositoDestino, False)
                    ColArNro(grdetalle, "stock", "Stock", 80, "0.00")
                    ColArNro(grdetalle, "stockTotal", "Total", 80, "0.00")
                End If
                Panel_AlmacenGrupoTraspaso.Visible = False
            End If
            If (_fnAccesible() And tbCodigo.Text = String.Empty) Then
                CType(grdetalle.DataSource, DataTable).Rows.Clear()
                _prAddDetalleVenta()
            End If
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

    Private Sub grmovimiento_SelectionChanged(sender As Object, e As EventArgs) Handles grmovimiento.SelectionChanged
        If (grmovimiento.RowCount >= 0 And grmovimiento.Row >= 0) Then
            _prMostrarRegistro(grmovimiento.Row)
        End If
    End Sub

    Private Sub grAlmacen_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grAlmacen.EditingCell
        If (_fnAccesible()) Then
            If (e.Column.Index = CelIndex(grAlmacen, "Cantidad")) Then
                e.Cancel = False
            Else
                e.Cancel = True
            End If
        Else
            e.Cancel = True
        End If
    End Sub
    Private Sub grdetalle_DoubleClick(sender As Object, e As EventArgs) Handles grdetalle.DoubleClick
        If (_fnAccesible()) Then
            Dim productoId As Integer = grdetalle.GetValue("ProductoId")
            If productoId <> 0 Then
                LblProducto.Text = grdetalle.GetValue("producto")
                lbProductoId.Text = productoId
                cargarDetalleAlmacen(grAlmacen, productoId, True)
                lbStock.Text = jMetodo_SumarFila(grAlmacen, "Stock")
                Panel_AlmacenGrupo.Visible = True

                If cbConcepto.Value = 6 Then
                    Panel_AlmacenGrupoTraspaso.Visible = True
                    cargarDetalleAlmacen(grAlmacenSalida, productoId, True, True)
                End If
                detalleId = grdetalle.GetValue("Id")
                _prAplicarCondiccionJanusSeleccion()
                _prAplicarCondiccionJanusDesSeleccion()
            Else
                MostrarMensajeError("Item vacio")
            End If
        End If
    End Sub

    Private Sub grAlmacen_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grAlmacen.CellValueChanged
    End Sub

    Private Sub ValidarExistenciaStock(grid As GridEX, cantidad As Integer, stockTotal As Integer, posicion As Integer, valor As Integer, concepto As Integer)
        If cbConcepto.Value = concepto Then
            If cantidad > stockTotal Then
                CType(grid.DataSource, DataTable).Rows(posicion).Item("Cantidad") = valor
                grid.SetValue("Cantidad", valor)
                Throw New Exception("La cantidad que se quiere sacar es mayor a la que existe 
                                         en el stock solo puede Sacar : ".ToUpper + Str(stockTotal).Trim)
            End If
        End If

    End Sub
    Private Sub validarExistenciaStockXConceptoTraspaso(grid As GridEX, posicion As Integer, valor As Integer)
        If cbConcepto.Value = 6 Then
            grAlmacen.Update()
            Dim sumaCantidadAlmacenDestino = jMetodo_SumarFila(grAlmacen, "Cantidad")
            Dim cantidad = jMetodo_obtenerPrimerValor(grAlmacenSalida,
                                                      "Cantidad",
                                                      "Cantidad")

            If cantidad < sumaCantidadAlmacenDestino Then
                CType(grid.DataSource, DataTable).Rows(posicion).Item("Cantidad") = valor
                grid.SetValue("Cantidad", valor)
                Throw New Exception("La suma de las cantidades que se quiere ingresar es mayor a la que existe 
                                         en el stock solo puede Sacar : ".ToUpper + Str(cantidad).Trim)
            End If
            Dim almacenSalidaId = jMetodo_obtenerPrimerValor(grAlmacenSalida,
                                                             "Cantidad",
                                                             "AlmacenId")

            If CType(grid.DataSource, DataTable).Rows(posicion).Item("AlmacenId") = almacenSalidaId Then

                CType(grid.DataSource, DataTable).Rows(posicion).Item("Cantidad") = valor
                grid.SetValue("Cantidad", valor)
                Throw New Exception("No se puede poner cantidad al mismo almacen Origen y Destino")
            End If

        End If
    End Sub
    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click

        Dim almacenSalidaId = 0
        almacenSalidaId = jMetodo_obtenerPrimerValor(grAlmacenSalida, "Cantidad", "AlmacenId")

        For Each fila As DataRow In CType(grAlmacen.DataSource, DataTable).Rows
            If fila("Cantidad") > 0 Then
                Dim resultado As Boolean = False
                For Each filadetalle As DataRow In CType(grdetalle.DataSource, DataTable).Rows
                    Dim alamcenId = IIf(cbConcepto.Value = 6,
                                        filadetalle("AlmDestinoId"),
                                        filadetalle("AlmOrigenId"))

                    If filadetalle("productoId") = lbProductoId.Text And
                        alamcenId = fila("AlmacenId") And
                        filadetalle("Estado") >= 0 Then

                        Dim valor As Integer = filadetalle("Id")
                        Dim posicion As Integer = -1
                        ObtenerFilaDetalle(posicion, valor, grdetalle, "Id")

                        If cbConcepto.Value <> 6 Then
                            SetDetalleValor(fila, posicion)
                        Else
                            SetDetalleValorTraspaso(fila, posicion, almacenSalidaId)
                        End If
                        resultado = True

                    End If
                Next
                If resultado = False Then
                    Dim posicion As Integer = ObtenerPosicionFila(grdetalle, "Id",
                                                                  grdetalle.GetValue("id"))

                    Dim filaDetalle As DataRow = CType(grdetalle.DataSource, DataTable).Rows(posicion)
                    CType(grdetalle.DataSource, DataTable).ImportRow(filaDetalle)

                    Dim cantidafila = contarFilas()
                    If cbConcepto.Value <> 6 Then
                        AddDetalleValor(fila, cantidafila)
                    Else
                        AddDetalleValorTraspaso(fila, cantidafila, almacenSalidaId)
                    End If
                End If

            End If
        Next

        If cbConcepto.Value = 6 Then
            cargarDetalleAlmacen(grAlmacenSalida, lbProductoId.Text, True, True)
        End If

        jMetodo_SetValorGrillaCompleta(grAlmacen, "Cantidad", "0")
        jMetodo_SetValorGrillaCompleta(grAlmacenSalida, "Cantidad", "0")
    End Sub

    Private Sub AddDetalleValor(fila As DataRow, cantidafila As Integer)

        Dim idMayor As Integer = ObtenerIdMayor(grdetalle, "Id")
        SetCelValor(grdetalle, cantidafila - 1, "Id", idMayor + 1)
        SetCelValor(grdetalle, cantidafila - 1, "Cantidad", fila("Cantidad"))
        SetCelValor(grdetalle, cantidafila - 1, "AlmOrigenId", fila("AlmacenId"))
        SetCelValor(grdetalle, cantidafila - 1, "Stock", fila("stock"))
        SetCelValor(grdetalle, cantidafila - 1, "Estado", 0)

        Dim stockTotal As Integer = obtenerStockTotal(fila)
        SetCelValor(grdetalle, cantidafila - 1, "StockTotal", stockTotal)


    End Sub
    Private Sub AddDetalleValorTraspaso(fila As DataRow, cantidafila As Integer, almacenSalidaId As Integer)
        Dim idMayor As Integer = ObtenerIdMayor(grdetalle, "Id")
        SetCelValor(grdetalle, cantidafila - 1, "Id", idMayor + 1)
        SetCelValor(grdetalle, cantidafila - 1, "Cantidad", fila("Cantidad"))
        SetCelValor(grdetalle, cantidafila - 1, "AlmOrigenId", almacenSalidaId)
        SetCelValor(grdetalle, cantidafila - 1, "Stock", fila("stock"))
        SetCelValor(grdetalle, cantidafila - 1, "Estado", 0)
        SetCelValor(grdetalle, cantidafila - 1, "AlmDestinoId", fila("AlmacenId"))

        Dim stockTotal As Integer = obtenerStockTotal(fila)
        SetCelValor(grdetalle, cantidafila - 1, "StockTotal", stockTotal)
    End Sub

    Private Sub SetDetalleValor(fila As DataRow, posicion As Integer)
        SetCelValor(grdetalle, posicion, "Cantidad", fila("Cantidad"))
        SetCelValor(grdetalle, posicion, "AlmOrigenId", fila("AlmacenId"))
        SetCelValor(grdetalle, posicion, "Stock", fila("stock"))

        Dim stockTotal As Integer = obtenerStockTotal(fila)
        SetCelValor(grdetalle, posicion, "StockTotal", stockTotal)


    End Sub
    Private Sub SetDetalleValorTraspaso(fila As DataRow, posicion As Integer, almacenSalidaId As Integer)
        SetCelValor(grdetalle, posicion, "Cantidad", fila("Cantidad"))
        SetCelValor(grdetalle, posicion, "AlmOrigenId", almacenSalidaId)
        SetCelValor(grdetalle, posicion, "Stock", fila("stock"))
        SetCelValor(grdetalle, posicion, "AlmDestinoId", fila("AlmacenId"))
        Dim stockTotal As Integer = obtenerStockTotal(fila)

        SetCelValor(grdetalle, posicion, "StockTotal", stockTotal)
    End Sub

    Private Function obtenerStockTotal(fila As DataRow) As Integer
        Dim stockTotal = 0
        If fila("Cantidad") <> 0 Then
            stockTotal = IIf(cbConcepto.Value = 1,
           fila("stock") + fila("Cantidad"),
           fila("stock") - fila("Cantidad"))
        Else
            stockTotal = fila("stock")
        End If

        Return stockTotal
    End Function

    Private Function contarFilas() As Integer
        Dim cantidadFila = 0
        For Each fila As DataRow In CType(grdetalle.DataSource, DataTable).Rows
            cantidadFila = cantidadFila + 1
        Next
        Return cantidadFila
    End Function
    Private Function existeItemAlamacenRepetido(productoId As Integer, almacenId As Integer) As Boolean
        For Each fila As DataRow In CType(grdetalle.DataSource, DataTable).Rows
            If fila("productoId") = productoId And
                fila("AlmOrigenId") = almacenId And
                fila("Estado") >= 0 Then

                If fila("cantidad") = 0 Then
                    Return True
                Else
                    Return False
                End If
            End If
        Next
        Return False
    End Function
    Private Sub ordenarGrid(productoId As String)
        Dim query = From order In CType(grdetalle.DataSource, DataTable).AsEnumerable()
                    Where order.Item("ProductoId") = productoId
                    Select order

    End Sub

    Private Sub grAlmacenSalida_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grAlmacenSalida.EditingCell
        If (_fnAccesible()) Then
            If (e.Column.Index = CelIndex(grAlmacenSalida, "Cantidad")) Then
                e.Cancel = False
            Else
                e.Cancel = True
            End If
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub grAlmacenSalida_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grAlmacenSalida.CellValueChanged

    End Sub


    Private Sub grAlmacen_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grAlmacen.CellEdited
        'Celda Cantidad
        Try
            If (e.Column.Index = CelIndex(grAlmacen, "Cantidad")) Then
                Dim posicion = ObtenerPosicionFila(grAlmacen, "AlmacenId", grAlmacen.GetValue("AlmacenId"))
                If (IsDBNull(grAlmacen.GetValue("Cantidad")) Or grAlmacen.GetValue("Cantidad").ToString.Trim = "") Then

                    CType(grAlmacen.DataSource, DataTable).Rows(posicion).Item("Cantidad") = 0
                    grAlmacen.SetValue("Cantidad", 0)
                    Return
                End If

                Dim cantidad As Integer = grAlmacen.GetValue("Cantidad")

                    If (Not IsNumeric(cantidad) Or
                    cantidad.ToString = String.Empty Or
                    cantidad < 0) Then
                        CType(grAlmacen.DataSource, DataTable).Rows(posicion).Item("Cantidad") = 0
                        grAlmacen.SetValue("Cantidad", 0)
                    Else
                        If cantidad > 0 Then
                            Dim stock As Integer = grAlmacen.GetValue("Stock")
                            ValidarExistenciaStock(grAlmacen, cantidad, stock, posicion, 0, 2)

                            validarExistenciaStockXConceptoTraspaso(grAlmacen, posicion, 0)
                        Else
                            grAlmacen.SetValue("Cantidad", 0)
                        End If
                    End If

                End If
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub

    Private Sub grAlmacenSalida_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grAlmacenSalida.CellEdited
        'Celda Cantidad
        Try
            If (e.Column.Index = CelIndex(grAlmacenSalida, "Cantidad")) Then

                Dim posicion = ObtenerPosicionFila(grAlmacenSalida, "AlmacenId", grdetalle.GetValue("id"))

                If (IsDBNull(grAlmacenSalida.GetValue("Cantidad")) Or grAlmacenSalida.GetValue("Cantidad").ToString.Trim = "") Then
                    CType(grAlmacenSalida.DataSource, DataTable).Rows(posicion).Item("Cantidad") = 0
                    grAlmacenSalida.SetValue("Cantidad", 0)
                    Return
                End If
                Dim cantidad As Integer = grAlmacenSalida.GetValue("Cantidad")
                If (Not IsNumeric(cantidad) Or
                    cantidad.ToString = String.Empty Or
                    cantidad < 0) Then

                    CType(grAlmacenSalida.DataSource, DataTable).Rows(posicion).Item("Cantidad") = 0
                    grAlmacenSalida.SetValue("Cantidad", 0)
                Else
                    If cantidad > 0 Then
                        Dim stock As Integer = grAlmacenSalida.GetValue("Stock")
                        ValidarExistenciaStock(grAlmacenSalida, cantidad, stock, posicion, 0, 6)
                    Else
                        grAlmacenSalida.SetValue("Cantidad", 0)
                    End If
                End If
            End If
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub

    Private Sub grdetalle_ColumnHeaderClick(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.ColumnHeaderClick

        Try
            grdetalle.Focus()

            grdetalle.Col = 1
        Catch ex As Exception

        End Try


    End Sub

    Private Sub btnImprimir_Click(sender As Object, e As EventArgs) Handles btnImprimir.Click
        P_GenerarReporte()
    End Sub

#End Region
End Class