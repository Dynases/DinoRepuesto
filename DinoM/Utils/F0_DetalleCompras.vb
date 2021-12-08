
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports DinoM.JanusExtension
Imports Logica.AccesoLogica
Imports System.IO

Public Class F0_DetalleCompras
    Public dtProductoAll As DataTable
    Public dtDetalle As DataTable
    Public dtname As DataTable
    Public CategoriaPrecio As Integer

    Public detalleSeleccionHabilitado = True

    'Variables para kardex
    Public pedidoId As Integer
    Public producto As String
    Public stock As Decimal
    Public Sub IniciarTodod()
        CargarProductos()
        CargarProductosVentas()
        tbProducto.Focus()
    End Sub

    Public Sub New(dtp As DataTable, dtv As DataTable, dtn As DataTable)
        InitializeComponent()
        dtProductoAll = dtp
        dtDetalle = dtv
        dtname = dtn
    End Sub

    Public Sub CargarProductosVentas()
        Dim bandera As Boolean = False
        Dim dt As DataTable
        If (IsNothing(dtDetalle)) Then
            bandera = True
            grProductoSeleccionado.DataSource = dtProductoAll
        Else
            grProductoSeleccionado.DataSource = dtDetalle
        End If
        grProductoSeleccionado.RetrieveStructure()
        grProductoSeleccionado.AlternatingColors = True
        '      a.cbnumi ,a.cbtv1numi ,a.cbty5prod ,b.yfcdprod1 as producto,a.cbest ,a.cbcmin 
        ',a.cbumin ,Umin .ycdes3 as unidad,a.cbpcost,a.cblote ,a.cbfechavenc ,a.cbptot 
        ',a.cbutven ,a.cbprven   ,a.cbobs ,
        'a.cbfact ,a.cbhact ,a.cbuact,1 as estado,Cast(null as Image) as img,a.cbpcost as costo,a.cbprven as venta
        'If (Lote = True) Then
        '    With grProductoSeleccionado.RootTable.Columns("cblote")
        '        .Width = 150
        '        .Caption = "LOTE"
        '        .Visible = True
        '        .MaxLength = 50
        '    End With
        '    With grProductoSeleccionado.RootTable.Columns("cbfechavenc")
        '        .Width = 120
        '        .Caption = "FECHA VENC."
        '        .Visible = True
        '        .FormatString = "dd/MM/yyyy"
        '    End With
        'Else
        With grProductoSeleccionado.RootTable.Columns("cblote")
            .Width = 150
            .Caption = "LOTE"
            .Visible = False
            .MaxLength = 50
        End With
        With grProductoSeleccionado.RootTable.Columns("cbfechavenc")
            .Width = 120
            .Caption = "FECHA VENC."
            .Visible = False
            .FormatString = "dd/MM/yyyy"
        End With
        'End If


        With grProductoSeleccionado.RootTable.Columns("cbnumi")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = False
        End With

        With grProductoSeleccionado.RootTable.Columns("cbtv1numi")
            .Width = 90
            .Visible = False
        End With
        With grProductoSeleccionado.RootTable.Columns("cbty5prod")
            .Width = 90
            .Visible = False
        End With

        With grProductoSeleccionado.RootTable.Columns("producto")
            .Caption = "Descripcion"
            .Width = 400
            .Visible = True
            .WordWrap = True
            .MaxLines = 2
        End With
        With grProductoSeleccionado.RootTable.Columns("CodigoFabrica")
            .Caption = "Cod.Fabrica"
            .Width = 120
            .WordWrap = True
            .MaxLines = 2
            .Visible = True

        End With
        With grProductoSeleccionado.RootTable.Columns("CodigoMarca")
            .Caption = "Cod.Fabrica"
            .Width = 120
            .WordWrap = True
            .MaxLines = 2
            .Visible = True

        End With

        With grProductoSeleccionado.RootTable.Columns("Medida")
            .Caption = "Medida"
            .Width = 120
            .WordWrap = True
            .MaxLines = 2
            .Visible = True

        End With

        With grProductoSeleccionado.RootTable.Columns("Marca")
            .Caption = "Marca"
            .Width = 120
            .WordWrap = True
            .MaxLines = 2
            .Visible = True

        End With
        With grProductoSeleccionado.RootTable.Columns("Procedencia")
            .Caption = "Procedencia"
            .Width = 120
            .WordWrap = True
            .MaxLines = 2
            .Visible = True

        End With

        With grProductoSeleccionado.RootTable.Columns("cbest")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With

        With grProductoSeleccionado.RootTable.Columns("cbcmin")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad".ToUpper
        End With
        With grProductoSeleccionado.RootTable.Columns("cbumin")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With
        With grProductoSeleccionado.RootTable.Columns("unidad")
            .Width = 80
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "Unidad".ToUpper
        End With
        With grProductoSeleccionado.RootTable.Columns("cbpcost")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Precio U.".ToUpper
        End With
        'If (_estadoPor = 1) Then
        '    With grProductoSeleccionado.RootTable.Columns("cbutven")
        '        .Width = 110
        '        .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '        .Visible = True
        '        .FormatString = "0.00"
        '        .Caption = "Utilidad (%)".ToUpper
        '    End With
        '    With grProductoSeleccionado.RootTable.Columns("cbprven")
        '        .Width = 120
        '        .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '        .Visible = True
        '        .FormatString = "0.00"
        '        .Caption = "Precio Venta".ToUpper
        '    End With
        'Else
        With grProductoSeleccionado.RootTable.Columns("cbutven")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0.00"
            .Caption = "Utilidad.".ToUpper
        End With
        With grProductoSeleccionado.RootTable.Columns("cbprven")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0.00"
            .Caption = "Precio Venta.".ToUpper
        End With
        'End If

        With grProductoSeleccionado.RootTable.Columns("cbptot")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Sub Total".ToUpper
        End With
        With grProductoSeleccionado.RootTable.Columns("cbobs")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProductoSeleccionado.RootTable.Columns("cbfact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProductoSeleccionado.RootTable.Columns("cbhact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProductoSeleccionado.RootTable.Columns("cbuact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProductoSeleccionado.RootTable.Columns("estado")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProductoSeleccionado.RootTable.Columns("costo")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With
        With grProductoSeleccionado.RootTable.Columns("img")
            .Width = 80
            .Caption = "Eliminar".ToUpper
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = False
        End With
        With grProductoSeleccionado.RootTable.Columns("venta")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With
        With grProductoSeleccionado
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With

        If (bandera = True) Then
            CType(grProductoSeleccionado.DataSource, DataTable).Rows.Clear()

        End If
    End Sub
    Public Sub CargarProductos()
        Try

            ' yfnumi	Categoria	CodigoFabrica	Marca	Medida	yfcdprod1	grupo1	grupo2	yhprecio	venta	stock

            ConfigInicialVinculado(grProductos, dtProductoAll, "Productos")
            ColAL(grProductos, "yfnumi", "Item", 50)
            ColAL(grProductos, "Categoria", "Categoria", 150)
            ColAL(grProductos, "CodigoFabrica", "Cod. Fabrica", 150)
            ColAL(grProductos, "CodigoMarca", "Cod. Marca", 150)
            ColAL(grProductos, "Medida", "Medida", 150)
            ColAL(grProductos, "yfcdprod1", "Producto", 350)
            ColAL(grProductos, "grupo1", "Marca", 150)
            ColAL(grProductos, "grupo2", "Procedencia", 150)
            ColArNro(grProductos, "stock", "Stock", 80, "0.00")
            ColArNro(grProductos, "yhprecio", "Precio Costo", 90, "0.00")
            ColArNro(grProductos, "venta", "Precio Venta", 90, "0.00")

            ConfigFinalBasica(grProductos)
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

    Private Sub F0_DetalleVenta_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        IniciarTodod()
        tbProducto.Focus()
    End Sub

    Public Function ExisteProducto(Id As Integer) As Boolean
        Dim dt As DataTable = CType(grProductoSeleccionado.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("cbty5prod") = Id) Then
                Return True
            End If



        Next
        Return False
    End Function

    Private Sub grProductos_KeyDown(sender As Object, e As KeyEventArgs) Handles grProductos.KeyDown

        If (e.KeyData = Keys.Enter) Then
            seleccionarCelda()
        End If
        If e.KeyData = Keys.Escape Then
            Me.Close()

        End If
    End Sub
    Public Sub _fnObtenerFilaDetalle(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(grProductoSeleccionado.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grProductoSeleccionado.DataSource, DataTable).Rows(i).Item("cbnumi")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next

    End Sub
    Private Sub _prAddDetalleVenta()
        '     a.cbnumi ,a.cbtv1numi ,a.cbty5prod ,b.yfcdprod1 as producto,a.cbest ,a.cbcmin 
        ',a.cbumin ,Umin .ycdes3 as unidad,a.cbpcost,a.cblote ,a.cbfechavenc ,a.cbptot 
        ',a.cbutven ,a.cbprven   ,a.cbobs ,
        'a.cbfact ,a.cbhact ,a.cbuact,1 as estado,Cast(null as Image) as img,a.cbpcost as costo,a.cbprven as venta
        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.delete, 28, 28)
        img.Save(Bin, Imaging.ImageFormat.Png)
        CType(grProductoSeleccionado.DataSource, DataTable).Rows.Add(_fnSiguienteNumi() + 1, 0, "", "", "", "", "", 0, "", 0, 0, 0, "",
                                                        0, "20500101", CDate("2050/01/01"), 0, 0, 0, "", Now.Date, "", "", 0, Bin.GetBuffer, 0, 0)
    End Sub
    Public Function _fnSiguienteNumi()
        Dim dt As DataTable = CType(grProductoSeleccionado.DataSource, DataTable)
        Dim rows() As DataRow = dt.Select("cbnumi=MAX(cbnumi)")
        If (rows.Count > 0) Then
            Return rows(rows.Count - 1).Item("cbnumi")
        End If
        Return 1
    End Function
    Private Sub seleccionarCelda()

        Dim _PorcentajeUtil As Double = 0 '' En esta varible obtendre de la libreria el porcentaje de 
        Dim f, c As Integer
        c = grProductos.Col
        f = grProductos.Row
        If (f >= 0) Then

            If (Not ExisteProducto(grProductos.GetValue("yfnumi"))) Then
                Dim pos As Integer = -1
                grProductoSeleccionado.Row = grProductoSeleccionado.RowCount - 1

                If (grProductoSeleccionado.GetValue("cbty5prod") > 0) Then
                    _prAddDetalleVenta()
                End If
                grProductoSeleccionado.Row = grProductoSeleccionado.RowCount - 1

                _fnObtenerFilaDetalle(pos, grProductoSeleccionado.GetValue("cbnumi"))
                If (pos >= 0) Then ''And (Not existe))


                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("CodigoFabrica") = grProductos.GetValue("CodigoFabrica")
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("CodigoMarca") = grProductos.GetValue("CodigoMarca")
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("Medida") = grProductos.GetValue("Medida")
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("Marca") = grProductos.GetValue("grupo1")
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("Procedencia") = grProductos.GetValue("grupo2")



                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbty5prod") = grProductos.GetValue("yfnumi")
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("producto") = grProductos.GetValue("yfcdprod1")
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbumin") = 0
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("unidad") = grProductos.GetValue("Medida")
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost") = grProductos.GetValue("yhprecio")
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbptot") = grProductos.GetValue("yhprecio")
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbcmin") = 1

                    Dim PrecioVenta As Double = IIf(IsDBNull(grProductos.GetValue("venta")), 0, grProductos.GetValue("venta"))
                    If (PrecioVenta > 0) Then
                        CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbprven") = PrecioVenta
                        Dim montodesc As Double = PrecioVenta - grProductos.GetValue("yhprecio")
                        Dim precio As Integer = IIf(IsDBNull(grProductos.GetValue("yhprecio")), 0, grProductos.GetValue("yhprecio"))
                        If (precio = 0) Then
                            CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbutven") = 100
                        Else
                            Dim pordesc As Double = ((montodesc * 100) / grProductos.GetValue("yhprecio"))
                            CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbutven") = pordesc
                        End If


                    Else
                        CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbutven") = _PorcentajeUtil
                        CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbprven") = (grProductos.GetValue("yhprecio") + ((grProductos.GetValue("yhprecio")) * (_PorcentajeUtil / 100)))


                    End If




                End If

            Else
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "El producto ya existe en el detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

                tbProducto.Focus()
            End If
        End If
    End Sub

    Private Sub tbProducto_TextChanged(sender As Object, e As EventArgs) Handles tbProducto.TextChanged
        Dim charSequence As String
        charSequence = tbProducto.Text.ToUpper
        If (charSequence.Trim = String.Empty) Then


            grProductos.DataSource = dtProductoAll.Copy
        Else
            Dim Len As Integer = tbProducto.Text.Length
            Dim Ch As String = tbProducto.Text(Len - 1)
            If (Ch.Trim = String.Empty) Then
                FiltrarProducto()
            End If

        End If

    End Sub

    Private Function FiltrarProducto() As String
        Dim dtProductoCopy As DataTable
        dtProductoCopy = dtProductoAll.Copy
        dtProductoCopy.Rows.Clear()
        Dim dt As DataTable = dtProductoAll.Copy

        Dim charSequence As String
        charSequence = tbProducto.Text.ToUpper
        If (charSequence.Trim <> String.Empty) Then
            Dim cantidad As Integer = 12
            Dim cont As Integer = 12

            'Split con array de delimitadores
            Dim delimitadores() As String = {" ", ".", ",", ";"}
            Dim vectoraux() As String
            vectoraux = charSequence.Split(delimitadores, StringSplitOptions.None)

            'mostrar resultado
            'For Each item As String In vectoraux


            '    Console.WriteLine("'{0}'", item)
            'Next
            Dim cant As Integer = vectoraux.Length
            'ColAL(grProductos, "yfnumi", "Item", 50)
            'ColAL(grProductos, "Categoria", "Categoria", 150)
            'ColAL(grProductos, "CodigoFabrica", "Cod. Fabrica", 150)
            'ColAL(grProductos, "Marca", "Cod. Marca", 150)
            'ColAL(grProductos, "Medida", "Medida", 150)
            'ColAL(grProductos, "yfcdprod1", "Producto", 350)
            'ColAL(grProductos, "grupo1", "Marca", 150)
            'ColAL(grProductos, "grupo2", "Procedencia", 150)
            'ColAL(grProductos, "stock", "Stock", 80)
            'ColAL(grProductos, "yhprecio", "Precio Costo", 90)
            'ColAL(grProductos, "venta", "Precio Venta", 90)
            For i As Integer = 0 To dt.Rows.Count - 1 Step 1
                Dim nombre As String = dt.Rows(i).Item("yfnumi").ToString.ToUpper +
                    " " + dt.Rows(i).Item("Categoria").ToString.ToUpper +
                    " " + dt.Rows(i).Item("CodigoFabrica").ToString.ToUpper +
                    " " + dt.Rows(i).Item("CodigoMarca").ToString.ToUpper +
                    " " + dt.Rows(i).Item("Medida").ToString.ToUpper +
                    " " + dt.Rows(i).Item("grupo2").ToString.ToUpper +
                    " " + dt.Rows(i).Item("grupo1").ToString.ToUpper +
                    " " + dt.Rows(i).Item("yfnumi").ToString.ToUpper
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
            grProductos.DataSource = dtProductoAll.Copy
        End If

        Return charSequence
    End Function

    Private Sub tbProducto_KeyDown(sender As Object, e As KeyEventArgs) Handles tbProducto.KeyDown
        If e.KeyData = Keys.Escape Then
            Me.Close()
        End If
        If e.KeyData = Keys.Down Then
            grProductos.Focus()
        End If
    End Sub

    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        Me.Close()
    End Sub

    Private Sub grProductos_DoubleClick(sender As Object, e As EventArgs) Handles grProductos.DoubleClick
        If detalleSeleccionHabilitado Then
            seleccionarCelda()
        Else
            pedidoId = grProductos.GetValue("item")
            producto = grProductos.GetValue("yfcdprod1")
            stock = grProductos.GetValue("stock")
            Me.Close()
        End If

    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        dtProductoAll = L_prMovimientoListarProductos(1)  ''1=Almacen
        CargarProductos()
    End Sub

    Public Sub P_PonerTotal(rowIndex As Integer)
        If (rowIndex < grProductoSeleccionado.RowCount) Then

            Dim lin As Integer = grProductoSeleccionado.GetValue("cbnumi")
            Dim pos As Integer = -1
            _fnObtenerFilaDetalle(pos, lin)
            Dim cant As Double = grProductoSeleccionado.GetValue("cbcmin")
            Dim uni As Double = grProductoSeleccionado.GetValue("cbpcost")
            If (pos >= 0) Then
                Dim TotalUnitario As Double = cant * uni
                'grProductoSeleccionado.SetValue("lcmdes", montodesc)

                CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbptot") = TotalUnitario
                grProductoSeleccionado.SetValue("cbptot", TotalUnitario)
                Dim estado As Integer = CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("estado")
                If (estado = 1) Then
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If
                CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbprven") = (uni + (uni * (grProductoSeleccionado.GetValue("cbutven") / 100))) * 6.96
                grProductoSeleccionado.SetValue("cbprven", (uni + (uni * (grProductoSeleccionado.GetValue("cbutven") / 100))) * 6.96)
                CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("venta") = (uni + (uni * (grProductoSeleccionado.GetValue("cbutven") / 100))) * 6.96
                grProductoSeleccionado.SetValue("cbprven", (uni + (uni * (grProductoSeleccionado.GetValue("cbutven") / 100))) * 6.96)
            End If
            '_prCalcularPrecioTotal()
        End If



    End Sub
    Private Sub grProductoSeleccionado_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grProductoSeleccionado.CellValueChanged
        Dim _PorcentajeUtil As Double = 0
        Dim lin As Integer = grProductoSeleccionado.GetValue("cbnumi")
        Dim pos As Integer = -1
        _fnObtenerFilaDetalle(pos, lin)
        If (e.Column.Index = grProductoSeleccionado.RootTable.Columns("cbcmin").Index) Then
            If (Not IsNumeric(grProductoSeleccionado.GetValue("cbcmin")) Or grProductoSeleccionado.GetValue("cbcmin").ToString = String.Empty) Then
                CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbcmin") = 1
                CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbptot") = CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost")
            Else
                If (grProductoSeleccionado.GetValue("cbcmin") > 0) Then
                    Dim rowIndex02 As Integer = grProductoSeleccionado.Row
                    P_PonerTotal(rowIndex02)
                Else

                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbcmin") = 1
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbptot") = CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost")
                    '_prCalcularPrecioTotal()
                End If
            End If
        End If

        ''''''''''''''''''''''COSTO  ''''''''''''''''''''''''''''''''''''''''''
        If (e.Column.Index = grProductoSeleccionado.RootTable.Columns("cbpcost").Index) Then
            If (Not IsNumeric(grProductoSeleccionado.GetValue("cbpcost")) Or grProductoSeleccionado.GetValue("cbpcost").ToString = String.Empty) Then
                Dim cantidad As Double = grProductoSeleccionado.GetValue("cbcmin")
                CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost") = CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost")
                CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbptot") = cantidad * CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost")
                CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbprven") = _PorcentajeUtil * CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost")


            Else
                If (grProductoSeleccionado.GetValue("cbpcost") > 0) Then
                    Dim rowIndex01 As Integer = grProductoSeleccionado.Row
                    P_PonerTotal(rowIndex01)
                Else

                    Dim cantidad As Double = grProductoSeleccionado.GetValue("cbcmin")
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost") = CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost")
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbptot") = cantidad * CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost")
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbprven") = _PorcentajeUtil * CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost")
                End If
            End If
        End If

        ''''''''''''''''''PRECIO VENTA '''''''''   CONTINUARA  '''''''''''''
        'Habilitar solo las columnas de Precio, %, Monto y Observación
        '     a.cbnumi ,a.cbtv1numi ,a.cbty5prod ,b.yfcdprod1 as producto,a.cbest ,a.cbcmin ,a.cbumin ,Umin .ycdes3 as unidad,a.cbpcost 
        ',a.cbutven ,a.cbprven  ,a.cbptot ,a.cbobs ,
        'a.cbfact ,a.cbhact ,a.cbuact,1 as escado,Cast(null as Image) as img,costo,venta
        If (e.Column.Index = grProductoSeleccionado.RootTable.Columns("cbprven").Index) Then
            If (Not IsNumeric(grProductoSeleccionado.GetValue("cbprven")) Or grProductoSeleccionado.GetValue("cbprven").ToString = String.Empty) Then

                CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbprven") = CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("venta")
                Dim montodesc As Double = CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("venta") - grProductoSeleccionado.GetValue("cbpcost")
                Dim pordesc As Double = ((montodesc * 100) / CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost"))
            Else
                If (grProductoSeleccionado.GetValue("cbprven") > 0) Then

                    'Dim montodesc As Double = grProductoSeleccionado.GetValue("cbprven") - CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost")
                    'Dim pordesc As Double = ((montodesc * 100) / CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost"))
                    'grProductoSeleccionado.SetValue("cbutven", pordesc)

                    Dim montodesc As Double = grProductoSeleccionado.GetValue("cbprven") - (grProductoSeleccionado.GetValue("cbpcost") * 6.96)
                    Dim pordesc As Double = ((montodesc * 100) / (grProductoSeleccionado.GetValue("cbpcost") * 6.96))
                    grProductoSeleccionado.SetValue("cbutven", pordesc)

                Else

                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbprven") = CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("venta")
                    Dim montodesc As Double = CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("venta") - CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost")
                    Dim pordesc As Double = ((montodesc * 100) / CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost"))
                End If
            End If
        End If



        ''''''''''''''''''PORCENTAJE PRECIO VENTA '''''''''   CONTINUARA  '''''''''''''
        'Habilitar solo las columnas de Precio, %, Monto y Observación
        '     a.cbnumi ,a.cbtv1numi ,a.cbty5prod ,b.yfcdprod1 as producto,a.cbest ,a.cbcmin ,a.cbumin ,Umin .ycdes3 as unidad,a.cbpcost 
        ',a.cbutven ,a.cbprven  ,a.cbptot ,a.cbobs ,
        'a.cbfact ,a.cbhact ,a.cbuact,1 as escado,Cast(null as Image) as img,costo,venta
        If (e.Column.Index = grProductoSeleccionado.RootTable.Columns("cbutven").Index) Then

            Dim venta As Double = IIf(IsDBNull(CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("venta")), 0, CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("venta"))
            Dim PrecioCosto As Double = IIf(IsDBNull(grProductoSeleccionado.GetValue("cbpcost")), 0, (grProductoSeleccionado.GetValue("cbpcost") * 6.96))
            If (Not IsNumeric(grProductoSeleccionado.GetValue("cbutven")) Or grProductoSeleccionado.GetValue("cbutven").ToString = String.Empty) Then

                CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbprven") = CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("venta")
                Dim montodesc As Double = CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("venta") - grProductoSeleccionado.GetValue("cbpcost")

                Dim pordesc As Double = ((montodesc * 100) / CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost"))

                CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbutven") = pordesc
            Else
                If (grProductoSeleccionado.GetValue("cbutven") > 0) Then

                    Dim porcentaje As Double = grProductoSeleccionado.GetValue("cbutven")

                    Dim monto As Double = ((grProductoSeleccionado.GetValue("cbpcost") * 6.96) * (porcentaje / 100))
                    Dim precioventa As Double = monto + (grProductoSeleccionado.GetValue("cbpcost") * 6.96)
                    grProductoSeleccionado.SetValue("cbprven", precioventa)

                Else

                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbprven") = CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("venta")
                    Dim montodesc As Double = CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("venta") - CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost")
                    Dim pordesc As Double = ((montodesc * 100) / CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbpcost"))
                    CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("cbutven") = pordesc
                End If
            End If
        End If
        Dim estado As Integer = CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("estado")
        If (estado = 1) Then
            CType(grProductoSeleccionado.DataSource, DataTable).Rows(pos).Item("estado") = 2
        End If

        Dim rowIndex As Integer = grProductoSeleccionado.Row
        P_PonerTotal(rowIndex)

    End Sub

    Private Sub grProductoSeleccionado_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grProductoSeleccionado.CellEdited
        If (e.Column.Index = grProductoSeleccionado.RootTable.Columns("cbcmin").Index) Then
            If (Not IsNumeric(grProductoSeleccionado.GetValue("cbcmin")) Or grProductoSeleccionado.GetValue("cbcmin").ToString = String.Empty) Then


                grProductoSeleccionado.SetValue("cbcmin", 1)
                grProductoSeleccionado.SetValue("cbptot", grProductoSeleccionado.GetValue("cbpcost"))
            Else
                If (grProductoSeleccionado.GetValue("cbcmin") > 0) Then


                Else

                    grProductoSeleccionado.SetValue("cbcmin", 1)
                    grProductoSeleccionado.SetValue("cbptot", grProductoSeleccionado.GetValue("cbpcost"))

                End If
            End If
        End If
    End Sub
    Private Sub grProductoSeleccionado_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grProductoSeleccionado.EditingCell



        If (e.Column.Index = grProductoSeleccionado.RootTable.Columns("cbcmin").Index Or e.Column.Index = grProductoSeleccionado.RootTable.Columns("cbpcost").Index) Then
            e.Cancel = False
        Else
            e.Cancel = True
        End If




    End Sub


    Private Sub grProductos_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grProductos.EditingCell
        'If (e.Column.Index = CelIndex(grProductos, "CodigoFabrica") Or
        '          e.Column.Index = CelIndex(grProductos, "Marca") Or
        '          e.Column.Index = CelIndex(grProductos, "Medida") Or
        '          e.Column.Index = CelIndex(grProductos, "yfcdprod1")) Then
        '    e.Cancel = False
        'Else
        e.Cancel = True
        'End If
    End Sub
End Class