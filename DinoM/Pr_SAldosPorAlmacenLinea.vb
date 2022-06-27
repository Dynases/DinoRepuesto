Imports Logica.AccesoLogica
Imports DevComponents.DotNetBar
Imports System.Data.OleDb
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls
Public Class Pr_SAldosPorAlmacenLinea
    Dim _Inter As Integer = 0
    Public _nameButton As String
    Public _tab As SuperTabItem
    Dim bandera As Boolean = False
    Dim RutaGlobal As String = gs_CarpetaRaiz

    Private Function GetDataExcel(
    ByVal fileName As String, ByVal sheetName As String) As DataTable

        ' Comprobamos los parámetros.
        '
        If ((String.IsNullOrEmpty(fileName)) OrElse
          (String.IsNullOrEmpty(sheetName))) Then _
          Throw New ArgumentNullException()

        Try
            Dim extension As String = IO.Path.GetExtension(fileName)

            Dim connString As String = "Data Source=" & fileName

            If (extension = ".xls") Then
                connString &= ";Provider=Microsoft.Jet.OLEDB.4.0;" &
                       "Extended Properties='Excel 8.0;HDR=YES;IMEX=1'"

            ElseIf (extension = ".xlsx") Then
                connString &= ";Provider=Microsoft.ACE.OLEDB.12.0;" &
                       "Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1'"
            Else
                Throw New ArgumentException(
                  "La extensión " & extension & " del archivo no está permitida.")
            End If

            Using conexion As New OleDbConnection(connString)

                Dim sql As String = "SELECT * FROM [" & sheetName & "$]"
                Dim adaptador As New OleDbDataAdapter(sql, conexion)

                Dim dt As New DataTable("Excel")

                adaptador.Fill(dt)

                Return dt

            End Using

        Catch ex As Exception
            Throw

        End Try

    End Function
    Public Sub _prIniciarTodo()
        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        _prCargarComboLibreriaSucursal(cbAlmacen)
        _prCargarComboGrupos(cbGrupos)
        _PMIniciarTodo()
        Me.Text = "SALDOS DE PRODUCTOS"
        MReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
        _IniciarComponentes()
        bandera = True
    End Sub
    Public Sub _IniciarComponentes()



    End Sub
    Public Sub _prInterpretarDatos(ByRef _dt As DataTable)
        If (CheckTodosAlmacen.Checked And checkTodosGrupos.Checked And CheckMayorCero.Checked) Then
            _dt = L_fnTodosAlmacenTodosLineasMayorCero()
        End If
        If (CheckTodosAlmacen.Checked And checkTodosGrupos.Checked And CheckTodos.Checked) Then
            _dt = L_fnTodosAlmacenTodosLineas()
        End If
        If (checkUnaAlmacen.Checked And checkTodosGrupos.Checked And CheckTodos.Checked) Then
            _dt = L_fnUnaAlmacenTodosLineas(cbAlmacen.Value)
        End If
        'un almacen todos mayor a 0
        If (checkUnaAlmacen.Checked And checkTodosGrupos.Checked And CheckMayorCero.Checked) Then
            _dt = L_fnUnaAlmacenTodosLineasMayorCero(cbAlmacen.Value)
        End If
        If (checkUnaGrupo.Checked And CheckTodosAlmacen.Checked) Then
            _dt = L_fnTodosAlmacenUnaLineas(cbGrupos.Value)
        End If
        If (checkUnaAlmacen.Checked And checkUnaGrupo.Checked And CheckTodos.Checked) Then
            _dt = L_fnUnaAlmacenUnaLineas(cbGrupos.Value, cbAlmacen.Value)
        End If
        ' un almacen una linea y mayor a cero
        If (checkUnaAlmacen.Checked And checkUnaGrupo.Checked And CheckMayorCero.Checked) Then
            _dt = L_fnUnaAlmacenUnaLineasMayorCero(cbGrupos.Value, cbAlmacen.Value)
        End If

    End Sub
    Private Sub _prCargarReporte()
        Dim _dt As New DataTable
        _prInterpretarDatos(_dt)
        If (_dt.Rows.Count > 0) Then

            Dim objrep As New R_SaldosPorLinea
            objrep.SetDataSource(_dt)

            objrep.SetParameterValue("usuario", L_Usuario)
            MReportViewer.ReportSource = objrep
            MReportViewer.Show()
            MReportViewer.BringToFront()

            ''Cargar a la Grilla
            JGrM_Buscador.DataSource = _dt
            JGrM_Buscador.RetrieveStructure()
            JGrM_Buscador.AlternatingColors = True

            With JGrM_Buscador.RootTable.Columns("abnumi")
                .Width = 90
                .Visible = False
            End With
            With JGrM_Buscador.RootTable.Columns("almacen")
                .Width = 100
                .Visible = True
                .Caption = "Almacen"
            End With
            With JGrM_Buscador.RootTable.Columns("CodigoProducto")
                .Width = 90
                .Visible = True
                .Caption = "Item"
            End With
            With JGrM_Buscador.RootTable.Columns("CodLinea")
                .Width = 100
                .Visible = False
                .Caption = "Cod. Linea"
            End With
            With JGrM_Buscador.RootTable.Columns("yfcdprod1")
                .Width = 170
                .Caption = "Producto"
                .Visible = True
            End With
            With JGrM_Buscador.RootTable.Columns("yfMed")
                .Width = 100
                .Visible = False
            End With
            With JGrM_Buscador.RootTable.Columns("yfap")
                .Width = 90
                .Visible = False
            End With
            With JGrM_Buscador.RootTable.Columns("iccprod")
                .Width = 120
                .Visible = False
            End With
            With JGrM_Buscador.RootTable.Columns("iccven")
                .Width = 100
                .Caption = "Stock"
                .Visible = True
            End With
            With JGrM_Buscador.RootTable.Columns("yccod3")
                .Width = 50
                .Visible = False
            End With
            With JGrM_Buscador.RootTable.Columns("ycdes3")
                .Width = 100
                .Visible = False
            End With
            With JGrM_Buscador.RootTable.Columns("linea")
                .Width = 100
                .Visible = True
                .Caption = "Categoría"
            End With
            With JGrM_Buscador.RootTable.Columns("presentacion")
                .Width = 380
                .Visible = False
            End With
            With JGrM_Buscador.RootTable.Columns("yfcprod")
                .Width = 100
                .Visible = False
            End With
            With JGrM_Buscador.RootTable.Columns("yhprecio")
                .Width = 90
                .Visible = False
            End With
            With JGrM_Buscador.RootTable.Columns("codFabrica")
                .Width = 110
                .Caption = "Cod. Fabrica"
                .Visible = True
            End With
            With JGrM_Buscador.RootTable.Columns("codMarca")
                .Width = 110
                .Caption = "Cod. Marca"
                .Visible = True
            End With
            With JGrM_Buscador.RootTable.Columns("Medida")
                .Width = 110
                .Caption = "Medida"
                .Visible = True
            End With
            With JGrM_Buscador.RootTable.Columns("marca")
                .Width = 100
                .Caption = "Marca"
                .Visible = True
            End With
            With JGrM_Buscador.RootTable.Columns("procedencia")
                .Width = 100
                .Caption = "Procedencia"
                .Visible = True
            End With

            With JGrM_Buscador
                .DefaultFilterRowComparison = FilterConditionOperator.Contains
                .FilterMode = FilterMode.Automatic
                .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
                .GroupByBoxVisible = False
                .TotalRow = InheritableBoolean.True
                .TotalRowFormatStyle.BackColor = Color.Gold
                .TotalRowPosition = TotalRowPosition.BottomFixed
                'diseño de la grilla
            End With

        Else
            ToastNotification.Show(Me, "NO HAY DATOS PARA LOS PARAMETROS SELECCIONADOS..!!!",
                                       My.Resources.INFORMATION, 2000,
                                       eToastGlowColor.Blue,
                                       eToastPosition.BottomLeft)
            MReportViewer.ReportSource = Nothing
        End If

    End Sub
    'Private Sub btnGenerar_Click(sender As Object, e As EventArgs) Handles btnGenerar.Click
    '    '_prCargarReporte()
    '    Try

    '        Dim dt As DataTable = GetDataExcel( _
    '             "C:\Users\usuario\Google Drive\INFORMACION MARCO ANTONIO\Dinases\Base de Datos\Dino M\Fcia 10102017\Clientes.xlsx", "Hoja1")

    '        If (dt.Rows.Count > 0) Then

    '            For i As Integer = 0 To dt.Rows.Count - 1 Step 1
    '                Dim CodigoCliente As String = dt.Rows(i).Item(0)
    '                Dim RazonSocial As String = IIf(IsDBNull(dt.Rows(i).Item(1)), "", dt.Rows(i).Item(1))
    '                Dim Direccion As String = IIf(IsDBNull(dt.Rows(i).Item(2)), "", dt.Rows(i).Item(2))
    '                Dim Telefono As String = IIf(IsDBNull(dt.Rows(i).Item(3)), "", dt.Rows(i).Item(3))
    '                Dim nombre As String = IIf(IsDBNull(dt.Rows(i).Item(4)), "", dt.Rows(i).Item(4))
    '                Dim Telefono1 As String = ""
    '                Dim Telefono2 As String = ""
    '                If (Telefono.Contains("-")) Then
    '                    Dim index As Integer = Telefono.IndexOf("-")
    '                    Telefono1 = Telefono.Substring(0, index)
    '                    Telefono2 = Telefono.Substring(index + 1)


    '                Else
    '                    Telefono1 = Telefono
    '                End If

    '                Dim res As Boolean = L_fnGrabarCLiente("", CodigoCliente, RazonSocial, nombre, 0, 1, 1, "", Direccion, Telefono1, Telefono2, 70, 1, 0, 0, "", "2017/01/01", "", 1, "", Now.Date.ToString("yyyy/MM/dd"), Now.Date.ToString("yyyy/MM/dd"), "", 1)


    '                If res = False Then
    '                    MsgBox("Pos" + Str(i))

    '                End If
    '            Next


    '        End If


    '    Catch ex As Exception
    '        MessageBox.Show(ex.Message)

    '    End Try
    'End Sub


    Private Sub Pr_VentasAtendidas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()

    End Sub
    Private Sub _prCargarComboLibreriaSucursal(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnListarSucursales()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("aanumi").Width = 60
            .DropDownList.Columns("aanumi").Caption = "COD"
            .DropDownList.Columns.Add("aabdes").Width = 300
            .DropDownList.Columns("aabdes").Caption = "SUCURSAL"
            .ValueMember = "aanumi"
            .DisplayMember = "aabdes"
            .DataSource = dt
            .Refresh()
        End With
        If (dt.Rows.Count > 0) Then
            cbAlmacen.SelectedIndex = 0
        End If
    End Sub
    Private Sub _prCargarComboGrupos(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnObtenerGruposLibreria()
        'a.ylcod2,yldes2
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("yccod3").Width = 60
            .DropDownList.Columns("yccod3").Caption = "COD"
            .DropDownList.Columns.Add("yldes2").Width = 250
            .DropDownList.Columns("yldes2").Caption = "GRUPOS"
            .ValueMember = "yccod3"
            .DisplayMember = "yldes2"
            .DataSource = dt
            .Refresh()
        End With
        If (dt.Rows.Count > 0) Then
            cbGrupos.SelectedIndex = 0
        End If
    End Sub
    Private Sub _prCargarComboLibreriaPrecioCosto(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_prListarPrecioCosto()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("ygnumi").Width = 60
            .DropDownList.Columns("ygnumi").Caption = "COD"
            .DropDownList.Columns.Add("ygdesc").Width = 500
            .DropDownList.Columns("ygdesc").Caption = "SUCURSAL"
            .ValueMember = "ygnumi"
            .DisplayMember = "ygdesc"
            .DataSource = dt
            .Refresh()
        End With
        If (dt.Rows.Count > 0) Then
            cbGrupos.SelectedIndex = 0
        End If
    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click

        Me.Close()

    End Sub

    Private Sub swTipoVenta_ValueChanged(sender As Object, e As EventArgs)
        If (bandera = False) Then
            Return
        End If
        'If (swTipoVenta.Value = True) Then
        '    _prCargarComboLibreriaPrecioVenta(cbGrupos)
        'Else
        '    _prCargarComboLibreriaPrecioCosto(cbGrupos)

        'End If

    End Sub

    Sub _prInhabilitarAlmacen()
        cbAlmacen.Enabled = False
    End Sub
    Sub _prhabilitarAlmacen()
        cbAlmacen.Enabled = True
    End Sub

    Sub _prInhabilitarGrupos()
        cbGrupos.Enabled = False
    End Sub
    Sub _prhabilitarGrupos()
        cbGrupos.Enabled = True
    End Sub
    Private Sub CheckTodosVendedor_CheckValueChanged(sender As Object, e As EventArgs) Handles CheckTodosAlmacen.CheckValueChanged
        If (CheckTodosAlmacen.Checked) Then
            _prInhabilitarAlmacen()
        Else
            _prhabilitarAlmacen()
        End If

    End Sub
    'grup panel stock mayor a cero o todos


    Private Sub checkTodosGrupos_CheckValueChanged(sender As Object, e As EventArgs) Handles checkTodosGrupos.CheckValueChanged
        If (checkTodosGrupos.Checked) Then
            _prInhabilitarGrupos()
        Else
            _prhabilitarGrupos()
        End If
    End Sub

    Private Sub btnGenerar_Click(sender As Object, e As EventArgs) Handles btnGenerar.Click
        _prCargarReporte()
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

    Private Sub btnExportar_Click(sender As Object, e As EventArgs) Handles btnExportar.Click
        _prCrearCarpetaReportes()
        Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
        If (P_ExportarExcel(RutaGlobal + "\Reporte\Reporte Productos")) Then
            ToastNotification.Show(Me, "EXPORTACIÓN DE LISTA DE PRODUCTOS EXITOSA..!!!",
                                       img, 2000,
                                       eToastGlowColor.Green,
                                       eToastPosition.BottomCenter)
        Else
            ToastNotification.Show(Me, "FALLO AL EXPORTACIÓN DE LISTA DE PRODUCTOS..!!!",
                                       My.Resources.WARNING, 2000,
                                       eToastGlowColor.Red,
                                       eToastPosition.BottomLeft)
        End If
    End Sub
    Private Sub _prCrearCarpetaReportes()
        Dim rutaDestino As String = RutaGlobal + "\Reporte\Reporte Productos\"

        If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte Productos\") = False Then
            If System.IO.Directory.Exists(RutaGlobal + "\Reporte") = False Then
                System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte")
                If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte Productos") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte\Reporte Productos")
                End If
            Else
                If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte Productos") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte\Reporte Productos")

                End If
            End If
        End If
    End Sub
    Public Function P_ExportarExcel(_ruta As String) As Boolean
        Dim _ubicacion As String
        'Dim _directorio As New FolderBrowserDialog

        If (1 = 1) Then 'If(_directorio.ShowDialog = Windows.Forms.DialogResult.OK) Then
            '_ubicacion = _directorio.SelectedPath
            _ubicacion = _ruta
            Try
                Dim _stream As Stream
                Dim _escritor As StreamWriter
                Dim _fila As Integer = JGrM_Buscador.GetRows.Length
                Dim _columna As Integer = JGrM_Buscador.RootTable.Columns.Count
                Dim _archivo As String = _ubicacion & "\SaldoDeProductos_" & Now.Date.Day &
                    "." & Now.Date.Month & "." & Now.Date.Year & "_" & Now.Hour & "." & Now.Minute & "." & Now.Second & ".csv"
                Dim _linea As String = ""
                Dim _filadata = 0, columndata As Int32 = 0
                File.Delete(_archivo)
                _stream = File.OpenWrite(_archivo)
                _escritor = New StreamWriter(_stream, System.Text.Encoding.UTF8)

                For Each _col As GridEXColumn In JGrM_Buscador.RootTable.Columns
                    If (_col.Visible) Then
                        _linea = _linea & _col.Caption & ";"
                    End If
                Next
                _linea = Mid(CStr(_linea), 1, _linea.Length - 1)
                _escritor.WriteLine(_linea)
                _linea = Nothing

                'Pbx_Precios.Visible = True
                'Pbx_Precios.Minimum = 1
                'Pbx_Precios.Maximum = Dgv_Precios.RowCount
                'Pbx_Precios.Value = 1

                For Each _fil As GridEXRow In JGrM_Buscador.GetRows
                    For Each _col As GridEXColumn In JGrM_Buscador.RootTable.Columns
                        If (_col.Visible) Then
                            Dim data As String = CStr(_fil.Cells(_col.Key).Value)
                            data = data.Replace(";", ",")
                            _linea = _linea & data & ";"
                        End If
                    Next
                    _linea = Mid(CStr(_linea), 1, _linea.Length - 1)
                    _escritor.WriteLine(_linea)
                    _linea = Nothing
                    'Pbx_Precios.Value += 1
                Next
                _escritor.Close()
                'Pbx_Precios.Visible = False
                Try
                    Dim ef = New Efecto
                    ef._archivo = _archivo

                    ef.tipo = 1
                    ef.Context = "Su archivo ha sido Guardado en la ruta: " + _archivo + vbLf + "DESEA ABRIR EL ARCHIVO?"
                    ef.Header = "PREGUNTA"
                    ef.ShowDialog()
                    Dim bandera As Boolean = False
                    bandera = ef.band
                    If (bandera = True) Then
                        Process.Start(_archivo)
                    End If

                    'If (MessageBox.Show("Su archivo ha sido Guardado en la ruta: " + _archivo + vbLf + "DESEA ABRIR EL ARCHIVO?", "PREGUNTA", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes) Then
                    '    Process.Start(_archivo)
                    'End If
                    Return True
                Catch ex As Exception
                    MsgBox(ex.Message)
                    Return False
                End Try
            Catch ex As Exception
                MsgBox(ex.Message)
                Return False
            End Try
        End If
        Return False
    End Function
End Class