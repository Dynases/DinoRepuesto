
Imports Logica.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls
Imports System.ComponentModel
Imports DinoM.JanusExtension

Public Class F1_Productos
    Dim _Inter As Integer = 0
#Region "Variables Locales"
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim Modificado As Boolean = False
    Dim nameImg As String = "Default.jpg"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
    Public Limpiar As Boolean = False  'Bandera para indicar si limpiar todos los datos o mantener datos ya registrados
    Dim dtProductoAll As DataTable
    Dim dtImagenesAll As DataTable
    Dim CategoriaSeleccionada As Integer '' Esta variable es para capturar la categoria seleccionada por el usuario
    Dim TablaImagenes As DataTable
    Dim TablaInventario As DataTable

    Dim gs_DirPrograma As String = ""
    Dim gs_RutaImg As String = ""
    Dim dtLibreria As DataTable = Nothing

    Dim dtPrecioAll As DataTable

    Private bgWorker As New BackgroundWorker

    Dim CategoriaGeneral As Integer = -1 ''Id de la categoria si es que no termina el hilo

    Dim BanderaCarga As Boolean = False

    Dim BanderaClonar As Boolean = False


#End Region
#Region "Metodos Privados"


    Private Sub _prIniciarTodo()
        Me.Text = "PRODUCTOS"
        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)

        btnBuscar.PerformClick()

        _prMaxLength()
        _prCargarNameLabel()
        _prCargarComboLibreria(cbgrupo1, 1, 1)
        _prCargarComboLibreria(cbgrupo2, 1, 2)
        _prCargarComboLibreria(cbgrupo3, 1, 3)
        _prCargarComboLibreria(cbgrupo4, 1, 4)
        _prCargarComboLibreria(cbgrupo5, 1, 7)
        _prCargarComboLibreria(cbUMed, 1, 5)
        _prCargarComboLibreria(cbUniVenta, 1, 6)
        _prCargarComboLibreria(cbUnidMaxima, 1, 6)
        dtPrecioAll = L_fnGeneralProductosDescuentosAll()
        dtImagenesAll = L_prCargarImagenesProductoAll()
        _prAsignarPermisos()
        armarGrillaDetalleProducto(0)
        _PMIniciarTodo()

        'Ocultar/Mostrar ingreso de detalle de producto
        SuperTabItem_DetalleProducto.Visible = gb_DetalleProducto

        Dim blah As New Bitmap(New Bitmap(My.Resources.producto), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico

        _prEliminarContenidoImage()
        JGrM_Buscador.Enabled = True
        SeleccionarCategoria()
    End Sub

    Private Sub MyWorker_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs)
        'Add your codes here for the worker to execute

    End Sub

    Private Sub MyWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs)
        'Add your codes for the worker to execute after finishing the work.
        BanderaCarga = True

    End Sub




    Private Sub _prCrearCarpetaImagenes(carpetaFinal As String)
        Dim rutaDestino As String = RutaGlobal + "\Imagenes\Imagenes Productos\" + carpetaFinal + "\"

        If System.IO.Directory.Exists(RutaGlobal + "\Imagenes\Imagenes Productos\" + carpetaFinal) = False Then
            If System.IO.Directory.Exists(RutaGlobal + "\Imagenes") = False Then
                System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes")
                If System.IO.Directory.Exists(RutaGlobal + "\Imagenes\Imagenes Productos") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes\Imagenes Productos")
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes\Imagenes Productos\" + carpetaFinal + "\")
                End If
            Else
                If System.IO.Directory.Exists(RutaGlobal + "\Imagenes\Imagenes Productos") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes\Imagenes Productos")
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes\Imagenes Productos\" + carpetaFinal + "\")
                Else
                    If System.IO.Directory.Exists(RutaGlobal + "\Imagenes\Imagenes Productos\" + carpetaFinal) = False Then
                        System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes\Imagenes Productos\" + carpetaFinal + "\")
                    End If

                End If
            End If
        End If
    End Sub


    Sub _prEliminarContenidoImage()
        Try
            My.Computer.FileSystem.DeleteDirectory(gs_RutaImg, FileIO.DeleteDirectoryOption.DeleteAllContents)
        Catch ex As Exception

        End Try
    End Sub
    Public Function _fnObtenerNumeroFilasActivas() As Integer
        Dim n As Integer = -1
        For i As Integer = 0 To TablaImagenes.Rows.Count - 1 Step 1
            Dim estado As Integer = TablaImagenes.Rows(i).Item("estado")
            If (estado = 0 Or estado = 1) Then
                n += 1

            End If
        Next
        Return n
    End Function
    Public Sub _prCargarImagen()
        PanelListImagenes.Controls.Clear()

        pbImgProdu.Image = Nothing

        Dim i As Integer = 0
        For Each fila As DataRow In TablaImagenes.Rows
            Dim elemImg As UCLavadero = New UCLavadero
            Dim rutImg = fila.Item("lhima").ToString
            Dim estado As Integer = fila.Item("estado")

            If (estado = 0) Then
                elemImg.pbImg.SizeMode = PictureBoxSizeMode.StretchImage
                Dim bm As Bitmap = Nothing
                Dim by As Byte() = fila.Item("img")
                Dim ms As New MemoryStream(by)
                bm = New Bitmap(ms)


                elemImg.pbImg.Image = bm

                pbImgProdu.SizeMode = PictureBoxSizeMode.StretchImage
                pbImgProdu.Image = bm
                elemImg.pbImg.Tag = i
                elemImg.Dock = DockStyle.Top
                pbImgProdu.Tag = i
                AddHandler elemImg.pbImg.MouseEnter, AddressOf pbImg_MouseEnter

                PanelListImagenes.Controls.Add(elemImg)
                ms.Dispose()

            Else
                If (estado = 1) Then
                    If (File.Exists(RutaGlobal + "\Imagenes\Imagenes Productos\ProductosTodos" + rutImg)) Then
                        Dim bm As Bitmap = New Bitmap(RutaGlobal + "\Imagenes\Imagenes Productos\ProductosTodos" + rutImg)
                        elemImg.pbImg.SizeMode = PictureBoxSizeMode.StretchImage
                        elemImg.pbImg.Image = bm
                        pbImgProdu.SizeMode = PictureBoxSizeMode.StretchImage
                        pbImgProdu.Image = bm
                        elemImg.pbImg.Tag = i
                        elemImg.Dock = DockStyle.Top
                        pbImgProdu.Tag = i
                        AddHandler elemImg.pbImg.MouseEnter, AddressOf pbImg_MouseEnter

                        PanelListImagenes.Controls.Add(elemImg)
                    End If

                End If
            End If




            i += 1
        Next

    End Sub

    Private Sub pbImg_MouseEnter(sender As Object, e As EventArgs)
        Dim pb As PictureBox = CType(sender, PictureBox)
        pbImgProdu.Image = pb.Image
        pbImgProdu.Tag = pb.Tag

    End Sub

    Private Function _fnCopiarImagenRutaDefinida() As String
        'copio la imagen en la carpeta del sistema

        Dim file As New OpenFileDialog()
        'file.InitialDirectory = gs_RutaImg
        file.Filter = "Ficheros JPG o JPEG o PNG|*.jpg;*.jpeg;*.png" &
                      "|Ficheros GIF|*.gif" &
                      "|Ficheros BMP|*.bmp" &
                      "|Ficheros PNG|*.png" &
                      "|Ficheros TIFF|*.tif"
        If file.ShowDialog() = DialogResult.OK Then
            Dim ruta As String = file.FileName
            Dim nombre As String = ""

            If file.CheckFileExists = True Then
                Dim img As New Bitmap(New Bitmap(ruta), 1000, 800)
                Dim a As Object = file.GetType.ToString

                Dim da As String = Str(Now.Day).Trim + Str(Now.Month).Trim + Str(Now.Year).Trim + Str(Now.Hour).Trim + Str(Now.Minute).Trim + Str(Now.Second).Trim

                nombre = "\Imagen_" + da + ".jpg".Trim


                If (_fnActionNuevo()) Then
                    Dim mstream = New MemoryStream()

                    img.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg)

                    TablaImagenes.Rows.Add(0, 0, nombre, mstream.ToArray(), 0)
                    mstream.Dispose()
                    img.Dispose()

                Else
                    Dim mstream = New MemoryStream()

                    img.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg)
                    TablaImagenes.Rows.Add(0, tbCodigo.Text, nombre, mstream.ToArray(), 0)
                    mstream.Dispose()

                End If

                'img.Save(RutaTemporal + nombre, System.Drawing.Imaging.ImageFormat.Jpeg)




            End If
            Return nombre
        End If

        Return "default.jpg"
    End Function

    Public Sub _prGuardarImagenes(_ruta As String)
        PanelListImagenes.Controls.Clear()


        For i As Integer = 0 To TablaImagenes.Rows.Count - 1 Step 1
            Dim estado As Integer = TablaImagenes.Rows(i).Item("estado")
            If (estado = 0) Then

                Dim bm As Bitmap = Nothing
                Dim by As Byte() = TablaImagenes.Rows(i).Item("img")
                Dim ms As New MemoryStream(by)
                bm = New Bitmap(ms)
                Try
                    bm.Save(_ruta + TablaImagenes.Rows(i).Item("lhima"), System.Drawing.Imaging.ImageFormat.Jpeg)
                Catch ex As Exception

                End Try




            End If
            If (estado = -1) Then
                Try
                    Me.pbImgProdu.Image.Dispose()
                    Me.pbImgProdu.Image = Nothing
                    Application.DoEvents()
                    TablaImagenes.Rows(i).Item("img") = Nothing



                    If (File.Exists(_ruta + TablaImagenes.Rows(i).Item("lhima"))) Then
                        My.Computer.FileSystem.DeleteFile(_ruta + TablaImagenes.Rows(i).Item("lhima"))
                    End If

                Catch ex As Exception

                End Try
            End If
        Next
    End Sub



    Private Sub armarGrillaDetalleProducto(numi As Integer)
        Dim dt As New DataTable
        dt = L_fnDetalleProducto(numi)
        dgjDetalleProducto.DataSource = dt
        dgjDetalleProducto.RetrieveStructure()
        dgjDetalleProducto.AlternatingColors = True

        With dgjDetalleProducto.RootTable.Columns("yfanumi")
            .Visible = False
        End With

        With dgjDetalleProducto.RootTable.Columns("yfayfnumi")
            .Visible = False
        End With
        With dgjDetalleProducto.RootTable.Columns("nro")
            .Caption = "Nro."
            .Width = 45
            .Visible = True
        End With
        With dgjDetalleProducto.RootTable.Columns("yfasim")
            .Caption = "Símbolo"
            .Width = 80
            .Visible = False
        End With
        With dgjDetalleProducto.RootTable.Columns("yfadesc")
            .Caption = "Descripción"
            .Width = 400
            .Visible = True
        End With
        With dgjDetalleProducto.RootTable.Columns("estado")
            .Visible = False
        End With
        With dgjDetalleProducto.RootTable.Columns.Add("delete", Janus.Windows.GridEX.ColumnType.Image)
            .HeaderAlignment = TextAlignment.Center
            .Image = New Bitmap(My.Resources.eliminar, New Size(15, 15))
            .Caption = "Quitar".ToUpper
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Width = 80
            .Visible = False
        End With

        With dgjDetalleProducto
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With
    End Sub

    Public Sub _prStyleJanus()
        GroupPanelBuscador.Style.BackColor = Color.FromArgb(13, 71, 161)
        GroupPanelBuscador.Style.BackColor2 = Color.FromArgb(13, 71, 161)
        GroupPanelBuscador.Style.TextColor = Color.White
        JGrM_Buscador.RootTable.HeaderFormatStyle.FontBold = TriState.True
    End Sub
    Public Sub _prCargarNameLabel()
        'Dim dt As DataTable = L_fnNameLabel()
        'If (dt.Rows.Count > 0) Then
        '    lbgrupo1.Text = dt.Rows(0).Item("Grupo 1").ToString + ":"
        '    lbgrupo2.Text = dt.Rows(0).Item("Grupo 2").ToString + ":"
        '    lbgrupo3.Text = dt.Rows(0).Item("Grupo 3").ToString + ":"
        '    lbgrupo4.Text = dt.Rows(0).Item("Grupo 4").ToString + ":"

        'End If

        lbgrupo1.Text = "MARCA :"
        lbgrupo2.Text = "PROCEDENCIA :"
        lbgrupo3.Text = "SUBCATEGORIA :"
        lbgrupo4.Text = "PRESENTACION :"
    End Sub
    Public Sub _prMaxLength()
        'tbCodProd.MaxLength = 25
        'tbCodBarra.MaxLength = 15
        'tbDescPro.MaxLength = 50
        'tbDescCort.MaxLength = 15
        cbgrupo1.MaxLength = 40
        cbgrupo2.MaxLength = 40
        cbgrupo3.MaxLength = 40
        cbgrupo4.MaxLength = 40
        cbUMed.MaxLength = 10
        cbUniVenta.MaxLength = 2
        cbUnidMaxima.MaxLength = 2
    End Sub

    Private Sub _prCargarComboLibreria(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo, cod1 As String, cod2 As String)
        Dim dt As New DataTable
        'If (IsNothing(dtLibreria)) Then

        '    dtLibreria = L_prObtenerTodaLaLibreria()
        'End If


        'dt = dtLibreria.Select("yccod1=" + cod1 + " and yccod2=" + cod2, "ycdes3 asc").CopyToDataTable().DefaultView.ToTable(False, "yccod3", "ycdes3")


        dt = L_prLibreriaClienteLGeneral(cod1, cod2)
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("yccod3").Width = 70
            .DropDownList.Columns("yccod3").Caption = "COD"
            .DropDownList.Columns.Add("ycdes3").Width = 200
            .DropDownList.Columns("ycdes3").Caption = "DESCRIPCION"
            .ValueMember = "yccod3"
            .DisplayMember = "ycdes3"
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
    Private Sub _prCrearCarpetaTemporal()

        If System.IO.Directory.Exists(RutaTemporal) = False Then
            System.IO.Directory.CreateDirectory(RutaTemporal)
        Else
            Try
                My.Computer.FileSystem.DeleteDirectory(RutaTemporal, FileIO.DeleteDirectoryOption.DeleteAllContents)
                My.Computer.FileSystem.CreateDirectory(RutaTemporal)
                'My.Computer.FileSystem.DeleteDirectory(RutaTemporal, FileIO.UIOption.AllDialogs, FileIO.RecycleOption.SendToRecycleBin)
                'System.IO.Directory.CreateDirectory(RutaTemporal)

            Catch ex As Exception

            End Try

        End If

    End Sub
    Private Sub _prCrearCarpetaImagenes()
        Dim rutaDestino As String = RutaGlobal + "\Imagenes\Imagenes ProductoDino\"

        If System.IO.Directory.Exists(RutaGlobal + "\Imagenes\Imagenes ProductoDino\") = False Then
            If System.IO.Directory.Exists(RutaGlobal + "\Imagenes") = False Then
                System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes")
                If System.IO.Directory.Exists(RutaGlobal + "\Imagenes\Imagenes ProductoDino") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes\Imagenes ProductoDino")
                End If
            Else
                If System.IO.Directory.Exists(RutaGlobal + "\Imagenes\Imagenes ProductoDino") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes\Imagenes ProductoDino")

                End If
            End If
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

#End Region
#Region "METODOS SOBRECARGADOS"

    Public Overrides Sub _PMOHabilitar()
        tbCodBarra.ReadOnly = False
        tbCodProd.ReadOnly = False
        tbDescPro.ReadOnly = False
        tbDescDet.ReadOnly = False
        tbMedida.ReadOnly = False
        cbgrupo1.ReadOnly = False
        cbgrupo2.ReadOnly = False
        cbgrupo3.ReadOnly = False
        cbgrupo4.ReadOnly = False
        cbgrupo5.ReadOnly = False  'a solicitud de rosely se bloquea el campo
        cbUMed.ReadOnly = False
        swEstado.IsReadOnly = False
        cbUniVenta.ReadOnly = False
        cbUnidMaxima.ReadOnly = False
        tbConversion.IsInputReadOnly = False
        tbPrecioVentaNormal.IsInputReadOnly = False
        tbPrecioFacturado.IsInputReadOnly = False
        tbCodigoMarca.ReadOnly = False
        tbPrecioMecanico.IsInputReadOnly = False
        tbPrecioCosto.IsInputReadOnly = False
        btnBuscar.Enabled = False
        btGrabarP.Visible = True
        _prCrearCarpetaImagenes()
        _prCrearCarpetaTemporal()
        btnDelete.Visible = True
        btnImagen.Visible = True
        tbStockMinimo.IsInputReadOnly = False
        btExcel.Visible = False
        btnImprimir.Visible = False
        dgjDetalleProducto.AllowEdit = InheritableBoolean.True
        dgjDetalleProducto.RootTable.Columns("delete").Visible = True
        adicionarFilaDetalleProducto()

        tbDesde.MinDate = Now.Date


        ''' Descuentos
        ''' 
        tbDesde.IsInputReadOnly = False
        tbHasta.IsInputReadOnly = False
        tbMontoDesde.IsInputReadOnly = False
        tbMontoHasta.IsInputReadOnly = False
        tbPrecioDescuento.IsInputReadOnly = False


        btGrabarP.Enabled = True
        tbDesde.Value = Now.Date
        tbHasta.Value = "01/01/2050"
        tbMontoDesde.Value = 0
        tbMontoHasta.Value = 0
        tbPrecioDescuento.Value = 0

        JGr_Descuentos.ContextMenuStrip = MenuStripDescuento
    End Sub

    Public Overrides Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbCodBarra.ReadOnly = True
        tbCodProd.ReadOnly = True
        btnBuscar.Enabled = True
        tbDescPro.ReadOnly = True
        tbMedida.ReadOnly = True
        tbDescDet.ReadOnly = True
        cbgrupo1.ReadOnly = True
        cbgrupo2.ReadOnly = True
        cbgrupo3.ReadOnly = True
        btnDelete.Visible = False
        btnImagen.Visible = False
        tbPrecioVentaNormal.IsInputReadOnly = True
        tbCodigoMarca.ReadOnly = True
        tbPrecioMecanico.IsInputReadOnly = True
        tbPrecioCosto.IsInputReadOnly = True
        cbgrupo4.ReadOnly = True
        tbPrecioFacturado.IsInputReadOnly = True
        cbgrupo5.ReadOnly = True
        cbUMed.ReadOnly = True
        swEstado.IsReadOnly = True
        cbUniVenta.ReadOnly = True
        cbUnidMaxima.ReadOnly = True
        tbConversion.IsInputReadOnly = True
        tbStockMinimo.IsInputReadOnly = True


        tbDesde.IsInputReadOnly = True
        tbHasta.IsInputReadOnly = True
        tbMontoDesde.IsInputReadOnly = True
        tbMontoHasta.IsInputReadOnly = True
        tbPrecioDescuento.IsInputReadOnly = True

        btGrabarP.Visible = False
        _prStyleJanus()
        JGrM_Buscador.Focus()
        Limpiar = False
        btExcel.Visible = True
        btnImprimir.Visible = True
        dgjDetalleProducto.AllowEdit = InheritableBoolean.False
        dgjDetalleProducto.RootTable.Columns("delete").Visible = False

        JGr_Descuentos.ContextMenuStrip = Nothing

    End Sub

    Public Overrides Sub _PMOLimpiar()
        If (BanderaClonar = False) Then
            tbCodigo.Clear()
            tbCodBarra.Clear()
            tbCodProd.Clear()
            tbDescPro.Clear()
            tbDescDet.Clear()
            tbMedida.Clear()

            tbPrecioCosto.Value = 0
            tbPrecioMecanico.Value = 0
            tbPrecioVentaNormal.Value = 0
            tbCodigoMarca.Clear()
            tbPrecioFacturado.Value = 0

            If (Limpiar = False) Then
                _prSeleccionarCombo(cbgrupo1)
                _prSeleccionarCombo(cbgrupo2)
                _prSeleccionarCombo(cbgrupo3)
                _prSeleccionarCombo(cbgrupo4)
                '_prSeleccionarCombo(cbgrupo5)
                _prSeleccionarCombo(cbUMed)
                _prSeleccionarCombo(cbUnidMaxima)
                _prSeleccionarCombo(cbUniVenta)
                swEstado.Value = True
                tbConversion.Value = 1

                tbStockMinimo.Value = 0
            End If
            tbCodProd.Focus()
            TablaImagenes = L_prCargarImagenesProducto(-1)
            _prCargarImagen()
            _prEliminarContenidoImage()

            armarGrillaDetalleProducto(0)
            tbPrecioVentaNormal.Value = 0
            tbDesde.Value = Now.Date
            tbHasta.Value = "01/01/2050"
            tbMontoDesde.Value = 0
            tbMontoHasta.Value = 0
            tbPrecioDescuento.Value = 0

            _PCargarGridCategoriasPrecios(-1)


            lbPorcentajeVentaMecanico.Text = "0 %"
            lbPorcentajeVentaPublico.Text = "0 %"
        Else


            tbPrecioCosto.Value = 0
            tbPrecioMecanico.Value = 0
            tbPrecioVentaNormal.Value = 0

            tbPrecioFacturado.Value = 0


            TablaImagenes = L_prCargarImagenesProducto(-1)
            _prCargarImagen()
            _prEliminarContenidoImage()

            armarGrillaDetalleProducto(0)

            tbDesde.Value = Now.Date
            tbHasta.Value = "01/01/2050"
            tbMontoDesde.Value = 0
            tbMontoHasta.Value = 0
            tbPrecioDescuento.Value = 0
            tbStockMinimo.Value = 0
            _PCargarGridCategoriasPrecios(-1)
            BanderaClonar = False
        End If


    End Sub

    Private Sub adicionarFilaDetalleProducto()
        CType(dgjDetalleProducto.DataSource, DataTable).Rows.Add({0, 0, 1, "", "", 0})
        Dim i As Integer = 0
        For Each fila As DataRow In CType(dgjDetalleProducto.DataSource, DataTable).Rows
            fila.Item("nro") = i + 1
            i += 1
        Next
    End Sub

    Public Sub _prSeleccionarCombo(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        If (CType(mCombo.DataSource, DataTable).Rows.Count > 0) Then
            mCombo.SelectedIndex = 0
        Else
            mCombo.SelectedIndex = -1
        End If
    End Sub


    Public Overrides Sub _PMOLimpiarErrores()
        MEP.Clear()
        tbDescPro.BackColor = Color.White
        tbDescDet.BackColor = Color.White

        cbgrupo1.BackColor = Color.White
        cbgrupo2.BackColor = Color.White
        cbgrupo3.BackColor = Color.White
        cbgrupo4.BackColor = Color.White
        cbgrupo5.BackColor = Color.White
    End Sub

    Public Overrides Function _PMOGrabarRegistro() As Boolean

        'ByRef _yfnumi As String, _yfcprod As String,
        '                                      _yfcbarra As String, _yfcdprod1 As String,
        '                                      _yfcdprod2 As String, _yfgr1 As Integer, _yfgr2 As Integer,
        '                                      _yfgr3 As Integer, _yfgr4 As Integer, _yfMed As Integer, _yfumin As Integer,
        '_yfusup As Integer, _yfvsup As Double, _yfsmin As Integer, _yfap As Integer, _yfimg As String


        Dim res As Boolean = L_fnGrabarProducto(tbCodigo.Text, tbCodProd.Text, tbCodBarra.Text, tbDescPro.Text,
                                                tbMedida.Text, cbgrupo1.Value, cbgrupo2.Value, cbgrupo3.Value,
                                                cbgrupo4.Value, cbUMed.Value, cbUniVenta.Value, cbUnidMaxima.Value,
                                                tbConversion.Text,
                                                IIf(tbStockMinimo.Text = String.Empty, 0, tbStockMinimo.Text),
                                                IIf(swEstado.Value = True, 1, 0), nameImg,
                                                quitarUltimaFilaVacia(CType(dgjDetalleProducto.DataSource, DataTable).DefaultView.ToTable(False, "yfanumi", "yfayfnumi", "yfasim", "yfadesc", "estado")), tbDescDet.Text, cbgrupo5.Value, tbPrecioVentaNormal.Value, tbPrecioFacturado.Value, tbPrecioMecanico.Value, tbPrecioCosto.Value, tbCodigoMarca.Text, CType(JGr_Descuentos.DataSource, DataTable), TablaImagenes)


        If res Then
            Modificado = False


            _prCrearCarpetaImagenes("ProductosTodos")
            _prGuardarImagenes(RutaGlobal + "\Imagenes\Imagenes Productos\" + "ProductosTodos" + "\")

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Producto ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )
            tbCodigo.Focus()
            Limpiar = True
            dtPrecioAll = L_fnGeneralProductosDescuentosAll()
            dtImagenesAll = L_prCargarImagenesProductoAll()
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "El producto no pudo ser insertado".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If
        Return res

    End Function

    Public Overrides Function _PMOModificarRegistro() As Boolean
        Dim res As Boolean

        Dim nameImage As String = JGrM_Buscador.GetValue("yfimg")
        If (Modificado = False) Then
            res = L_fnModificarProducto(tbCodigo.Text, tbCodProd.Text, tbCodBarra.Text, tbDescPro.Text, tbMedida.Text, cbgrupo1.Value, cbgrupo2.Value, cbgrupo3.Value, cbgrupo4.Value, cbUMed.Value, cbUniVenta.Value, cbUnidMaxima.Value, tbConversion.Text, IIf(tbStockMinimo.Text = String.Empty, 0, tbStockMinimo.Text), IIf(swEstado.Value = True, 1, 0), nameImage, quitarUltimaFilaVacia(CType(dgjDetalleProducto.DataSource, DataTable).DefaultView.ToTable(False, "yfanumi", "yfayfnumi", "yfasim", "yfadesc", "estado")), tbDescDet.Text, cbgrupo5.Value, tbPrecioVentaNormal.Value, tbPrecioFacturado.Value, tbPrecioMecanico.Value, tbPrecioCosto.Value, tbCodigoMarca.Text, CType(JGr_Descuentos.DataSource, DataTable), TablaImagenes)
        Else
            res = L_fnModificarProducto(tbCodigo.Text, tbCodProd.Text, tbCodBarra.Text, tbDescPro.Text, tbMedida.Text, cbgrupo1.Value, cbgrupo2.Value, cbgrupo3.Value, cbgrupo4.Value, cbUMed.Value, cbUniVenta.Value, cbUnidMaxima.Value, tbConversion.Text, tbStockMinimo.Text, IIf(swEstado.Value = True, 1, 0), nameImg, quitarUltimaFilaVacia(CType(dgjDetalleProducto.DataSource, DataTable).DefaultView.ToTable(False, "yfanumi", "yfayfnumi", "yfasim", "yfadesc", "estado")), tbDescDet.Text, cbgrupo5.Value, tbPrecioVentaNormal.Value, tbPrecioFacturado.Value, tbPrecioMecanico.Value, tbPrecioCosto.Value, tbCodigoMarca.Text, CType(JGr_Descuentos.DataSource, DataTable), TablaImagenes)
        End If
        If res Then


            _prCrearCarpetaImagenes("ProductosTodos")
            _prGuardarImagenes(RutaGlobal + "\Imagenes\Imagenes Productos\" + "ProductosTodos" + "\")
            nameImg = "Default.jpg"

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Producto ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter)
            dtPrecioAll = L_fnGeneralProductosDescuentosAll()
            dtImagenesAll = L_prCargarImagenesProductoAll()

        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "EL producto no pudo ser modificado".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If
        _PMInhabilitar()
        _PMPrimerRegistro()
        _prEliminarContenidoImage()
        Return res
    End Function

    Private Function quitarUltimaFilaVacia(tabla As DataTable) As DataTable
        If tabla.Rows.Count > 0 Then
            If (tabla.Rows(tabla.Rows.Count - 1).Item("yfadesc").ToString() = String.Empty) Then
                tabla.Rows.RemoveAt(tabla.Rows.Count - 1)
                tabla.AcceptChanges()
            End If
        End If
        Return tabla
    End Function


    Public Function _fnActionNuevo() As Boolean
        Return tbCodigo.Text = String.Empty And tbCodBarra.ReadOnly = False
    End Function

    Public Overrides Sub _PMOEliminarRegistro()

        Dim ef = New Efecto


        ef.tipo = 2
        ef.Context = "¿esta seguro de eliminar el registro?".ToUpper
        ef.Header = "mensaje principal".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean = L_fnEliminarProducto(tbCodigo.Text, mensajeError)
            If res Then


                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)

                ToastNotification.Show(Me, "Código de Producto ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper,
                                          img, 2000,
                                          eToastGlowColor.Green,
                                          eToastPosition.TopCenter)

                _PMFiltrar()
            Else
                Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
                ToastNotification.Show(Me, mensajeError, img, 3000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If
        End If


    End Sub
    Public Overrides Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()

        If tbDescPro.Text = String.Empty Then
            tbDescPro.BackColor = Color.Red
            AddHandler tbDescPro.KeyDown, AddressOf TextBox_KeyDown
            MEP.SetError(tbDescPro, "ingrese el descripcion del producto!".ToUpper)
            _ok = False
        Else
            tbDescPro.BackColor = Color.White
            MEP.SetError(tbDescPro, "")
        End If
        'If tbDescCort.Text = String.Empty Then
        '    tbDescCort.BackColor = Color.Red
        '    MEP.SetError(tbDescCort, "ingrese la Descripcion Corta del Producto!".ToUpper)
        '    AddHandler tbDescCort.KeyDown, AddressOf TextBox_KeyDown
        '    _ok = False
        'Else
        '    tbDescCort.BackColor = Color.White
        '    MEP.SetError(tbDescCort, "")
        'End If

        If tbStockMinimo.Text = String.Empty Then
            tbStockMinimo.BackColor = Color.Red
            AddHandler tbStockMinimo.KeyDown, AddressOf TextBox_KeyDown
            MEP.SetError(tbStockMinimo, "ingrese stock minimo del producto!".ToUpper)
            _ok = False
        Else
            If (tbStockMinimo.Value <= 0) Then
                tbStockMinimo.BackColor = Color.Red
                AddHandler tbStockMinimo.KeyDown, AddressOf TextBox_KeyDown
                MEP.SetError(tbStockMinimo, "ingrese stock minimo del producto valido!".ToUpper)
                _ok = False

            Else
                tbStockMinimo.BackColor = Color.White
                MEP.SetError(tbStockMinimo, "")
            End If

        End If
        If cbgrupo1.SelectedIndex <= 0 Then
            cbgrupo1.BackColor = Color.Red
            MEP.SetError(cbgrupo1, "Seleccione Marca del producto diferente a No Existe!".ToUpper)
            _ok = False
        Else
            cbgrupo1.BackColor = Color.White
            MEP.SetError(cbgrupo1, "")
        End If

        If cbgrupo2.SelectedIndex <= 0 Then
            cbgrupo2.BackColor = Color.Red
            MEP.SetError(cbgrupo2, "Seleccione Procedencia del producto diferente a No Existe!".ToUpper)
            _ok = False
        Else
            cbgrupo2.BackColor = Color.White
            MEP.SetError(cbgrupo2, "")
        End If
        If cbgrupo3.SelectedIndex < 0 Then
            cbgrupo3.BackColor = Color.Red
            MEP.SetError(cbgrupo3, "Selecciones grupo del producto!".ToUpper)
            _ok = False
        Else
            cbgrupo3.BackColor = Color.White
            MEP.SetError(cbgrupo3, "")
        End If
        If cbgrupo4.SelectedIndex < 0 Then
            cbgrupo4.BackColor = Color.Red
            MEP.SetError(cbgrupo4, "Selecciones grupo del producto!".ToUpper)
            _ok = False
        Else
            cbgrupo4.BackColor = Color.White
            MEP.SetError(cbgrupo4, "")
        End If
        If cbUMed.SelectedIndex < 0 Then
            cbUMed.BackColor = Color.Red
            MEP.SetError(cbUMed, "Selecciones Unidad De Medida Del Producto!".ToUpper)
            _ok = False
        Else
            cbUMed.BackColor = Color.White
            MEP.SetError(cbUMed, "")
        End If



        MHighlighterFocus.UpdateHighlights()
        Return _ok
    End Function

    Public Overrides Function _PMOGetTablaBuscador() As DataTable
        Dim dtBuscador As DataTable
        If (CategoriaSeleccionada > 0) Then
            dtBuscador = L_fnGeneralProductos(CategoriaSeleccionada)

        Else
            dtBuscador = L_fnGeneralProductos(-1)
        End If


        dtProductoAll = dtBuscador.Copy
        Return dtBuscador
    End Function

    Public Overrides Function _PMOGetListEstructuraBuscador() As List(Of Modelo.Celda)
        Dim listEstCeldas As New List(Of Modelo.Celda)
        'a.yfnumi, a.yfcprod, a.yfcbarra, a.yfcdprod1, a.yfcdprod2, a.yfgr1, a.yfgr2, a.yfgr3, a.yfgr4,
        'a.yfMed, a.yfumin, a.yfusup, a.yfmstk, a.yfclot, a.yfsmin, a.yfap, a.yfimg, a.yffact, a.yfhact, a.yfuact
        listEstCeldas.Add(New Modelo.Celda("yfnumi", True, "ITem".ToUpper, 50))
        listEstCeldas.Add(New Modelo.Celda("grupo5", True, "CATEGORÍA".ToUpper, 90))
        listEstCeldas.Add(New Modelo.Celda("yfCodigoMarca", True, "CodigoMarca", 90))
        listEstCeldas.Add(New Modelo.Celda("yfcprod", True, "Cod.Fabrica", 100))
        listEstCeldas.Add(New Modelo.Celda("yfcdprod2", True, "Medida".ToUpper, 90))
        listEstCeldas.Add(New Modelo.Celda("yfcbarra", False, "Cod.Barra".ToUpper, 140))
        listEstCeldas.Add(New Modelo.Celda("yfdetprod", True, "Descripcion Producto".ToUpper, 360))
        listEstCeldas.Add(New Modelo.Celda("yfgr1", False))
        listEstCeldas.Add(New Modelo.Celda("yfgr2", False))
        listEstCeldas.Add(New Modelo.Celda("yfgr3", False))
        listEstCeldas.Add(New Modelo.Celda("yfgr4", False))
        listEstCeldas.Add(New Modelo.Celda("yfgr5", False))
        listEstCeldas.Add(New Modelo.Celda("yfMed", False))
        listEstCeldas.Add(New Modelo.Celda("yfumin", False))
        listEstCeldas.Add(New Modelo.Celda("yfusup", False))
        listEstCeldas.Add(New Modelo.Celda("yfvsup", False))
        listEstCeldas.Add(New Modelo.Celda("yfmstk", False))
        listEstCeldas.Add(New Modelo.Celda("yfclot", False))
        listEstCeldas.Add(New Modelo.Celda("yfsmin", False))
        listEstCeldas.Add(New Modelo.Celda("yfap", False))
        listEstCeldas.Add(New Modelo.Celda("yfimg", False))
        listEstCeldas.Add(New Modelo.Celda("yffact", False))
        listEstCeldas.Add(New Modelo.Celda("yfhact", False))
        listEstCeldas.Add(New Modelo.Celda("yfuact", False))
        listEstCeldas.Add(New Modelo.Celda("VentaFacturado", True, "V. Facturado", 80, "0.00"))
        listEstCeldas.Add(New Modelo.Celda("VentaNormal", True, "V. Publico", 80, "0.00"))
        listEstCeldas.Add(New Modelo.Celda("VentaMecanico", True, "V. Mecanico", 80, "0.00"))

        listEstCeldas.Add(New Modelo.Celda("PrecioCosto", False))
        listEstCeldas.Add(New Modelo.Celda("yfcdprod1", False, "Descripcion".ToUpper, 150))
        listEstCeldas.Add(New Modelo.Celda("grupo1", True, lbgrupo1.Text.Substring(0, lbgrupo1.Text.Length - 1).ToUpper, 80))
        listEstCeldas.Add(New Modelo.Celda("grupo2", True, lbgrupo2.Text.Substring(0, lbgrupo2.Text.Length - 1).ToUpper, 80))
        listEstCeldas.Add(New Modelo.Celda("grupo3", False, lbgrupo3.Text.Substring(0, lbgrupo3.Text.Length - 1).ToUpper, 100))
        listEstCeldas.Add(New Modelo.Celda("grupo4", False, lbgrupo4.Text.Substring(0, lbgrupo4.Text.Length - 1).ToUpper, 100))
        listEstCeldas.Add(New Modelo.Celda("Umedida", False, "UMedida".ToUpper, 150))
        listEstCeldas.Add(New Modelo.Celda("UnidMin", False, "UniVenta".ToUpper, 150))
        listEstCeldas.Add(New Modelo.Celda("Umax", False, "UniCaja".ToUpper, 150))

        listEstCeldas.Add(New Modelo.Celda("listaAlmacen", True, "Stock".ToUpper, 250))
        Return listEstCeldas
    End Function

    Public Function filtrarImagenes(Id As Integer) As DataTable
        Dim dt As DataTable = dtImagenesAll.Copy
        dt.Rows.Clear()

        For i As Integer = 0 To dtImagenesAll.Rows.Count - 1

            If (dtImagenesAll.Rows(i).Item("idty005") = Id) Then
                dt.ImportRow(dtImagenesAll.Rows(i))
            End If


        Next
        Return dt

    End Function

    Public Overrides Sub _PMOMostrarRegistro(_N As Integer)
        JGrM_Buscador.Row = _MPos
        'a.yfnumi, a.yfcprod, a.yfcbarra, a.yfcdprod1, a.yfcdprod2, a.yfgr1, a.yfgr2, a.yfgr3, a.yfgr4,
        'a.yfMed, a.yfumin, a.yfusup,yfvsup, a.yfmstk, a.yfclot, a.yfsmin, a.yfap, a.yfimg, a.yffact, a.yfhact, a.yfuact
        Dim dt As DataTable = CType(JGrM_Buscador.DataSource, DataTable)
        Try
            tbCodigo.Text = JGrM_Buscador.GetValue("yfnumi").ToString
        Catch ex As Exception
            Exit Sub
        End Try
        With JGrM_Buscador
            tbCodigo.Text = .GetValue("yfnumi").ToString
            tbCodProd.Text = .GetValue("yfcprod").ToString
            tbCodBarra.Text = .GetValue("yfcbarra").ToString
            tbDescPro.Text = .GetValue("yfcdprod1").ToString
            tbMedida.Text = .GetValue("yfcdprod2").ToString
            tbDescDet.Text = .GetValue("yfdetprod").ToString
            cbgrupo1.Value = .GetValue("yfgr1")
            cbgrupo2.Value = .GetValue("yfgr2")
            cbgrupo3.Value = .GetValue("yfgr3")
            cbgrupo4.Value = .GetValue("yfgr4")
            cbgrupo5.Value = .GetValue("yfgr5")
            cbUMed.Value = .GetValue("yfMed")
            tbPrecioFacturado.Value = .GetValue("VentaFacturado")
            tbPrecioVentaNormal.Value = .GetValue("VentaNormal")

            tbPrecioCosto.Value = .GetValue("PrecioCosto")
            tbPrecioMecanico.Value = .GetValue("VentaMecanico")
            tbCodigoMarca.Text = .GetValue("yfCodigoMarca").ToString

            cbUniVenta.Value = .GetValue("yfumin")
            cbUnidMaxima.Value = .GetValue("yfusup")
            tbConversion.Value = .GetValue("yfvsup")
            tbStockMinimo.Text = .GetValue("yfsmin")
            swEstado.Value = .GetValue("yfap")
            lbFecha.Text = CType(.GetValue("yffact"), Date).ToString("dd/MM/yyyy")
            lbHora.Text = .GetValue("yfhact").ToString
            lbUsuario.Text = .GetValue("yfuact").ToString
            _PCargarGridCategoriasPrecios(.GetValue("yfnumi"))
        End With

        Dim Mecanico As Double = 0
        Dim Publico As Double = 0

        If (tbPrecioFacturado.Value > 0) Then
            Mecanico = 100 - ((tbPrecioMecanico.Value * 100) / tbPrecioFacturado.Value)
            Publico = 100 - ((tbPrecioVentaNormal.Value * 100) / tbPrecioFacturado.Value)
        End If



        lbPorcentajeVentaMecanico.Text = Mecanico.ToString("0.00") + " %"
        lbPorcentajeVentaPublico.Text = Publico.ToString("0.00") + " %"

        Dim name As String = JGrM_Buscador.GetValue("yfimg")
        'TablaImagenes = L_prCargarImagenesProducto(tbCodigo.Text)
        TablaImagenes = filtrarImagenes(tbCodigo.Text)
        _prCargarImagen()
        If (gb_DetalleProducto) Then
            armarGrillaDetalleProducto(CInt(tbCodigo.Text))
        End If
        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString
    End Sub
    Public Overrides Sub _PMOHabilitarFocus()

        'With MHighlighterFocus
        '    .SetHighlightOnFocus(tbCodigo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        '    .SetHighlightOnFocus(tbCodProd, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        '    .SetHighlightOnFocus(tbStockMinimo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        '    .SetHighlightOnFocus(tbCodBarra, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

        '    .SetHighlightOnFocus(tbDescPro, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        '    .SetHighlightOnFocus(tbDescCort, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        '    .SetHighlightOnFocus(cbgrupo1, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        '    .SetHighlightOnFocus(cbgrupo2, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        '    .SetHighlightOnFocus(cbgrupo3, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        '    .SetHighlightOnFocus(cbgrupo4, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        '    .SetHighlightOnFocus(cbUMed, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        '    .SetHighlightOnFocus(swEstado, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        '    .SetHighlightOnFocus(cbUniVenta, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        '    .SetHighlightOnFocus(cbUnidMaxima, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        '    .SetHighlightOnFocus(tbConversion, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)


        'End With
    End Sub

#End Region

    Private Sub F1_Productos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub


    Private Sub btnImagen_Click(sender As Object, e As EventArgs) Handles btnImagen.Click
        _fnCopiarImagenRutaDefinida()
        _prCargarImagen()
    End Sub
    Public Sub _prEliminarDirectorio(numi As String)

        '_prGuardarImagenes(RutaGlobal + "\Imagenes\Imagenes RecepcionVehiculo\" + "Recepcion_" + tbNumeroOrden.Text.Trim + "\")
        If (File.Exists(RutaGlobal + "\Imagenes\Imagenes Productos\ProductosTodos")) Then

            Try
                My.Computer.FileSystem.DeleteDirectory(RutaGlobal + "\Imagenes\Imagenes Productos\ProductosTodos", FileIO.UIOption.AllDialogs, FileIO.RecycleOption.SendToRecycleBin)
            Catch ex As Exception

            End Try


        End If


    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim pos As Integer = CType(pbImgProdu.Tag, Integer)
        If (IsDBNull(TablaImagenes)) Then
            Return

        End If
        If (pos >= 0 And TablaImagenes.Rows.Count > 0) Then
            TablaImagenes.Rows(pos).Item("estado") = -1
            _prCargarImagen()
        End If
    End Sub

    Private Sub BtAdicionar_Click(sender As Object, e As EventArgs)
        _fnCopiarImagenRutaDefinida()
        btnGrabar.Focus()
    End Sub

    Private Sub cbgrupo3_ValueChanged(sender As Object, e As EventArgs) Handles cbgrupo3.ValueChanged
        If cbgrupo3.SelectedIndex < 0 And cbgrupo3.Text <> String.Empty Then
            btgrupo3.Visible = True
        Else
            btgrupo3.Visible = False
        End If
    End Sub

    Private Sub cbgrupo4_ValueChanged(sender As Object, e As EventArgs) Handles cbgrupo4.ValueChanged
        If cbgrupo4.SelectedIndex < 0 And cbgrupo4.Text <> String.Empty Then
            btgrupo4.Visible = True
        Else
            btgrupo4.Visible = False
        End If
    End Sub

    Private Sub cbgrupo1_ValueChanged(sender As Object, e As EventArgs) Handles cbgrupo1.ValueChanged
        If cbgrupo1.SelectedIndex < 0 And cbgrupo1.Text <> String.Empty Then
            btgrupo1.Visible = True
        Else
            btgrupo1.Visible = False
        End If
    End Sub

    Private Sub cbgrupo2_ValueChanged(sender As Object, e As EventArgs) Handles cbgrupo2.ValueChanged
        If cbgrupo2.SelectedIndex < 0 And cbgrupo2.Text <> String.Empty Then
            btgrupo2.Visible = True
        Else
            btgrupo2.Visible = False
        End If
    End Sub

    Private Sub cbUMed_ValueChanged(sender As Object, e As EventArgs) Handles cbUMed.ValueChanged
        If cbUMed.SelectedIndex < 0 And cbUMed.Text <> String.Empty Then
            btUMedida.Visible = True
        Else
            btUMedida.Visible = False
        End If
    End Sub

    Private Sub cbUniVenta_ValueChanged(sender As Object, e As EventArgs) Handles cbUniVenta.ValueChanged
        If cbUniVenta.SelectedIndex < 0 And cbUniVenta.Text <> String.Empty Then
            btUniVenta.Visible = True
        Else
            btUniVenta.Visible = False
        End If
    End Sub

    Private Sub cbUnidMaxima_ValueChanged(sender As Object, e As EventArgs) Handles cbUnidMaxima.ValueChanged
        If cbUnidMaxima.SelectedIndex < 0 And cbUnidMaxima.Text <> String.Empty Then
            btUniMaxima.Visible = True
        Else
            btUniMaxima.Visible = False
        End If
    End Sub

    Private Sub btgrupo3_Click(sender As Object, e As EventArgs) Handles btgrupo3.Click
        Dim numi As String = ""

        If L_prLibreriaGrabar(numi, "1", "3", cbgrupo3.Text, "") Then
            _prCargarComboLibreria(cbgrupo3, "1", "3")
            cbgrupo3.SelectedIndex = CType(cbgrupo3.DataSource, DataTable).Rows.Count - 1
        End If
    End Sub

    Private Sub btgrupo1_Click(sender As Object, e As EventArgs) Handles btgrupo1.Click
        Dim numi As String = ""

        If L_prLibreriaGrabar(numi, "1", "1", cbgrupo1.Text, "") Then
            _prCargarComboLibreria(cbgrupo1, "1", "1")
            cbgrupo1.SelectedIndex = CType(cbgrupo1.DataSource, DataTable).Rows.Count - 1
        End If
    End Sub

    Private Sub btgrupo2_Click(sender As Object, e As EventArgs) Handles btgrupo2.Click
        Dim numi As String = ""

        If L_prLibreriaGrabar(numi, "1", "2", cbgrupo2.Text, "") Then
            _prCargarComboLibreria(cbgrupo2, "1", "2")
            cbgrupo2.SelectedIndex = CType(cbgrupo2.DataSource, DataTable).Rows.Count - 1
        End If
    End Sub

    Private Sub btgrupo4_Click(sender As Object, e As EventArgs) Handles btgrupo4.Click
        Dim numi As String = ""

        If L_prLibreriaGrabar(numi, "1", "4", cbgrupo4.Text, "") Then
            _prCargarComboLibreria(cbgrupo4, "1", "4")
            cbgrupo4.SelectedIndex = CType(cbgrupo4.DataSource, DataTable).Rows.Count - 1
        End If
    End Sub

    Private Sub btUMedida_Click(sender As Object, e As EventArgs) Handles btUMedida.Click
        Dim numi As String = ""

        If L_prLibreriaGrabar(numi, "1", "5", cbUMed.Text, "") Then
            _prCargarComboLibreria(cbUMed, "1", "5")
            cbUMed.SelectedIndex = CType(cbUMed.DataSource, DataTable).Rows.Count - 1
        End If
    End Sub

    Private Sub btUniVenta_Click(sender As Object, e As EventArgs) Handles btUniVenta.Click
        Dim numi As String = ""

        If L_prLibreriaGrabar(numi, "1", "6", cbUniVenta.Text, "") Then
            _prCargarComboLibreria(cbUniVenta, "1", "6")
            _prCargarComboLibreria(cbUnidMaxima, "1", "6")
            cbUniVenta.SelectedIndex = CType(cbUniVenta.DataSource, DataTable).Rows.Count - 1
        End If
    End Sub

    Private Sub btUniMaxima_Click(sender As Object, e As EventArgs) Handles btUniMaxima.Click
        Dim numi As String = ""

        If L_prLibreriaGrabar(numi, "1", "6", cbUnidMaxima.Text, "") Then
            _prCargarComboLibreria(cbUnidMaxima, "1", "6")
            _prCargarComboLibreria(cbUniVenta, "1", "6")
            cbUnidMaxima.SelectedIndex = CType(cbUnidMaxima.DataSource, DataTable).Rows.Count - 1
        End If
    End Sub
    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles btExcel.Click
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
                Dim _archivo As String = _ubicacion & "\ListaDeProductos_" & Now.Date.Day &
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

    Private Sub JGrM_Buscador_DoubleClick(sender As Object, e As EventArgs) Handles JGrM_Buscador.DoubleClick
        If (JGrM_Buscador.Row >= 0) Then
            HabilitarDatos()

        End If

    End Sub



    Private Sub JGrM_Buscador_KeyDown(sender As Object, e As KeyEventArgs) Handles JGrM_Buscador.KeyDown

    End Sub

    Private Sub TextBox_KeyDown(sender As Object, e As KeyEventArgs)
        Try
            Dim tb As TextBoxX = CType(sender, TextBoxX)
            If tb.Text = String.Empty Then

            Else
                tb.BackColor = Color.White
                MEP.SetError(tb, "")
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        If btnGrabar.Enabled = True Then

            Dim ef = New Efecto


            ef.tipo = 2
            ef.Header = "¿Los Datos No Se Guardaron Debe Hacer Clic en el Boton Grabar. En Caso de Que no Quiera Guardarlo Confirme Este Mensaje?".ToUpper
            ef.Context = "mensaje principal".ToUpper
            ef.ShowDialog()
            Dim bandera As Boolean = False
            bandera = ef.band
            If (bandera = True) Then
                _PMInhabilitar()
                _PMPrimerRegistro()
            End If


        Else
            MPanelSup.Visible = False
            PanelSuperior.Visible = False
            PanelInferior.Visible = False
            GroupPanelBuscador.Visible = True
            tbProducto.Focus()
        End If
    End Sub

    Private Sub btnImprimir_Click(sender As Object, e As EventArgs) Handles btnImprimir.Click
        P_GenerarReporte()
    End Sub
    Private Sub P_GenerarReporte()
        'Dim dt As DataTable = L_fnReporteproducto()

        'If Not IsNothing(P_Global.Visualizador) Then
        '    P_Global.Visualizador.Close()
        'End If

        'P_Global.Visualizador = New Visualizador

        'Dim objrep As New R_Productos
        ''' GenerarNro(_dt)
        '''objrep.SetDataSource(Dt1Kardex)
        'objrep.SetDataSource(dt)

        'P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        'P_Global.Visualizador.Show() 'Comentar
        'P_Global.Visualizador.BringToFront() 'Comentar

    End Sub

    Public Function ObtenerTablaPrecio(codigoProducto As Integer)

        Dim dt As DataTable = dtPrecioAll.Copy
        dt.Rows.Clear()

        For i As Integer = 0 To dtPrecioAll.Rows.Count - 1 Step 1

            If (codigoProducto = dtPrecioAll.Rows(i).Item("dacanumi")) Then
                dt.ImportRow(dtPrecioAll.Rows(i))
            End If

        Next
        Return dt
    End Function

    Private Sub _PCargarGridCategoriasPrecios(codigoProducto As Integer)
        Dim dtPreciosDesc As DataTable
        ''dtPreciosDesc = L_fnGeneralProductosDescuentos(codigoProducto)
        dtPreciosDesc = ObtenerTablaPrecio(codigoProducto)
        JGr_Descuentos.DataSource = dtPreciosDesc
        JGr_Descuentos.RetrieveStructure()

        With JGr_Descuentos.RootTable.Columns("danumi")
            .Visible = False
        End With
        With JGr_Descuentos.RootTable.Columns("estado")
            .Visible = False
        End With
        With JGr_Descuentos.RootTable.Columns("dacanumi")
            .Caption = "Cod Prod."
            .Width = 50
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .CellStyle.BackColor = Color.AliceBlue
            .Visible = False
        End With
        With JGr_Descuentos.RootTable.Columns("dacant1")
            .Caption = "Desde"
            .Width = 100
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.BackColor = Color.AliceBlue
        End With
        With JGr_Descuentos.RootTable.Columns("estadoDescuento")
            .Caption = "Estado"
            .Width = 100
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center

        End With
        With JGr_Descuentos.RootTable.Columns("dacant2")
            .Caption = "Hasta"
            .Width = 100
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.BackColor = Color.AliceBlue
        End With
        With JGr_Descuentos.RootTable.Columns("dapreciou")
            .Caption = "Precio Un."
            .Width = 120
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.BackColor = Color.AliceBlue
            .FormatString = "0.0000"
        End With
        With JGr_Descuentos.RootTable.Columns("dafinicio")
            .Caption = "Fecha Inicio"
            .Width = 180
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.BackColor = Color.AliceBlue
            .Visible = False
        End With
        With JGr_Descuentos.RootTable.Columns("daffin")
            .Caption = "Fecha Fin"
            .Width = 180
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.BackColor = Color.AliceBlue
            .Visible = False
        End With
        With JGr_Descuentos.RootTable.Columns("estado")
            .Visible = False
        End With
        'Habilitar Filtradores
        With JGr_Descuentos
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            '.FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            '.AllowEdit = InheritableBoolean.False
        End With

        _prAplicarCondiccionDescuento()
    End Sub

    Public Sub _prAplicarCondiccionDescuento()
        Dim fc As GridEXFormatCondition
        fc = New GridEXFormatCondition(JGr_Descuentos.RootTable.Columns("estadoDescuento"), ConditionOperator.Equal, 0)
        'fc.FormatStyle.FontBold = TriState.True
        fc.FormatStyle.ForeColor = Color.Red    'Color.Tan
        JGr_Descuentos.RootTable.FormatConditions.Add(fc)
        Dim fr As GridEXFormatCondition
        fr = New GridEXFormatCondition(JGr_Descuentos.RootTable.Columns("estadoDescuento"), ConditionOperator.Equal, 1)
        fr.FormatStyle.ForeColor = Color.Black
        fr.FormatStyle.BackColor = Color.DodgerBlue
        JGr_Descuentos.RootTable.FormatConditions.Add(fr)
    End Sub

    Private Sub dgjDetalleProducto_EditingCell(sender As Object, e As EditingCellEventArgs) Handles dgjDetalleProducto.EditingCell
        If (e.Column.Index = dgjDetalleProducto.RootTable.Columns("yfadesc").Index) Then
            e.Cancel = False
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub dgjDetalleProducto_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles dgjDetalleProducto.CellEdited
        If (e.Column.Index = dgjDetalleProducto.RootTable.Columns("yfadesc").Index) Then
            If (dgjDetalleProducto.GetValue("yfadesc").ToString.Length > 100) Then
                ToastNotification.Show(Me, "La descripción no tiene que ser mayor a 100 caracteres".ToUpper,
                                      My.Resources.WARNING, 2000,
                                      eToastGlowColor.Red,
                                      eToastPosition.TopCenter
                                      )
                dgjDetalleProducto.SetValue("yfadesc", dgjDetalleProducto.GetValue("yfadesc").ToString.Substring(0, 100))
                dgjDetalleProducto.DataChanged = True
            End If

            'Si el estado es igual a 1, cambiarlo a 2
            If (CInt(dgjDetalleProducto.GetValue("estado")) = 1) Then
                dgjDetalleProducto.SetValue("estado", 2)
                dgjDetalleProducto.DataChanged = True
            End If

            'Si el la ultima fila, agregar una fila nueva
            If (dgjDetalleProducto.Row = dgjDetalleProducto.RowCount - 1) Then
                adicionarFilaDetalleProducto()
            End If
        End If
    End Sub

    Private Sub dgjDetalleProducto_Click(sender As Object, e As EventArgs) Handles dgjDetalleProducto.Click
        If (dgjDetalleProducto.CurrentColumn.Key = "delete") Then
            dgjDetalleProducto.CurrentRow.Delete()
            dgjDetalleProducto.Refetch()
            dgjDetalleProducto.Refresh()
        End If
    End Sub

    Private Sub UsImg_Load(sender As Object, e As EventArgs)

    End Sub


    Private Sub cbgrupo5_ValueChanged(sender As Object, e As EventArgs) Handles cbgrupo5.ValueChanged
        If cbgrupo5.SelectedIndex < 0 And cbgrupo5.Text <> String.Empty Then
            btgrupo5.Visible = True
        Else
            btgrupo5.Visible = False
        End If
    End Sub

    Private Sub btgrupo5_Click(sender As Object, e As EventArgs) Handles btgrupo5.Click
        Dim numi As String = ""

        If L_prLibreriaGrabar(numi, "1", "7", cbgrupo5.Text, "") Then
            _prCargarComboLibreria(cbgrupo5, "1", "7")
            cbgrupo5.SelectedIndex = CType(cbgrupo5.DataSource, DataTable).Rows.Count - 1
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        _Inter = _Inter + 1
        If _Inter = 1 Then
            Me.WindowState = FormWindowState.Maximized

        Else
            Me.Opacity = 100
            Timer1.Enabled = False
        End If
    End Sub

    Private Sub btNuevoP_Click(sender As Object, e As EventArgs)


    End Sub


    Function validarDescuento(ByRef posicion As Integer) As Boolean

        Dim dt As DataTable = CType(JGr_Descuentos.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim estado As Boolean = dt.Rows(i).Item("estadoDescuento")
            Dim estadoRegistro As Integer = dt.Rows(i).Item("estado")
            If (estado = True And estadoRegistro >= 0) Then
                If (tbMontoDesde.Value >= dt.Rows(i).Item("dacant1") And tbMontoDesde.Value <= dt.Rows(i).Item("dacant2")) Then
                    posicion = i
                    Return True
                End If
                If (tbMontoHasta.Value >= dt.Rows(i).Item("dacant1") And tbMontoHasta.Value <= dt.Rows(i).Item("dacant2")) Then
                    posicion = i
                    Return True
                End If
            End If

        Next
        Return False



    End Function


    Private Sub btGrabarP_Click(sender As Object, e As EventArgs) Handles btGrabarP.Click

        If (tbMontoDesde.Value > 0 And tbMontoHasta.Value > 0 And tbPrecioDescuento.Value > 0) Then


            If (tbMontoDesde.Value > tbMontoHasta.Value) Then
                Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
                ToastNotification.Show(Me, "La cantidad Hasta es mayor al Desde".ToUpper, img, 3000, eToastGlowColor.Red, eToastPosition.BottomCenter)

                Return

            End If
            Dim posicion As Integer = -1
            If (Not validarDescuento(posicion)) Then
                _prAddDetalleDescuento()
                tbDesde.Value = Now.Date
                tbHasta.Value = "01/01/2050"
                tbMontoDesde.Value = 0
                tbMontoHasta.Value = 0
                tbPrecioDescuento.Value = 0


            Else
                Dim dt As DataTable = CType(JGr_Descuentos.DataSource, DataTable)

                Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
                ToastNotification.Show(Me, "Ya existe un Descuento Programado con los datos a Insertar, Desde = " + Str(dt.Rows(posicion).Item("dacant1")) + "  Hasta " + Str(dt.Rows(posicion).Item("dacant2")), img, 4500, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If




        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "Debe Rellenar todos los campos para agregar el descuento".ToUpper, img, 3000, eToastGlowColor.Red, eToastPosition.BottomCenter)
        End If

    End Sub
    Private Sub _prAddDetalleDescuento()
        'd.danumi , d.dacanumi, d.dacant1, d.dacant2, d.dapreciou, d.dafinicio, d.daffin, 1 as estado
        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.delete, 28, 28)
        img.Save(Bin, Imaging.ImageFormat.Png)
        CType(JGr_Descuentos.DataSource, DataTable).Rows.Add(_fnSiguienteNumi() + 1, 0, tbMontoDesde.Value, tbMontoHasta.Value, tbPrecioDescuento.Value, tbDesde.Value.ToString("dd/MM/yyyy"), tbHasta.Value.ToString("dd/MM/yyyy"), 1, 0)
    End Sub

    Public Function _fnSiguienteNumi()
        Dim dt As DataTable = CType(JGr_Descuentos.DataSource, DataTable)
        Dim rows() As DataRow = dt.Select("danumi=MAX(danumi)")
        If (rows.Count > 0) Then
            Return rows(rows.Count - 1).Item("danumi")
        End If
        Return 1
    End Function

    Private Sub SuperTabControl_Imagenes_DetalleProducto_SelectedTabChanged(sender As Object, e As SuperTabStripSelectedTabChangedEventArgs) Handles SuperTabControl_Imagenes_DetalleProducto.SelectedTabChanged

    End Sub

    Private Sub tbPrecioCosto_ValueChanged(sender As Object, e As EventArgs) Handles tbPrecioCosto.ValueChanged

        If (tbCodBarra.ReadOnly = False) Then
            Dim PrecioCosto As Double = tbPrecioCosto.Value
            Dim PrecioVentaFactura As Double


            PrecioVentaFactura = ((PrecioCosto + (PrecioCosto * 0.25) + (PrecioCosto * 0.16)) * 2) * 7

            tbPrecioFacturado.Value = PrecioVentaFactura
            tbPrecioVentaNormal.Value = PrecioVentaFactura - (PrecioVentaFactura * 0.1)
            tbPrecioMecanico.Value = PrecioVentaFactura - (PrecioVentaFactura * 0.15)



        End If



    End Sub
    Private Sub tbPrecioFacturado_ValueChanged(sender As Object, e As EventArgs) Handles tbPrecioFacturado.ValueChanged
        If (tbCodBarra.ReadOnly = False) Then
            If (tbPrecioFacturado.Value > 0) Then
                Dim PrecioVentaFactura As Double
                PrecioVentaFactura = tbPrecioFacturado.Value
                tbPrecioVentaNormal.Value = PrecioVentaFactura - (PrecioVentaFactura * 0.1)
                tbPrecioMecanico.Value = PrecioVentaFactura - (PrecioVentaFactura * 0.15)
            End If


        End If

    End Sub
    Private Sub LabelX17_Click(sender As Object, e As EventArgs) Handles LabelX17.Click

    End Sub

    Private Sub tbPrecioVentaNormal_ValueChanged(sender As Object, e As EventArgs) Handles tbPrecioVentaNormal.ValueChanged
        Dim publico As Double = 0
        If (tbPrecioFacturado.Value > 0) Then

            publico = 100 - ((tbPrecioVentaNormal.Value * 100) / tbPrecioFacturado.Value)


        End If

        lbPorcentajeVentaPublico.Text = publico.ToString("0.00") + " %"

    End Sub

    Private Sub tbPrecioMecanico_ValueChanged(sender As Object, e As EventArgs) Handles tbPrecioMecanico.ValueChanged
        Dim mecanico As Double = 0
        If (tbPrecioFacturado.Value > 0) Then

            mecanico = 100 - ((tbPrecioMecanico.Value * 100) / tbPrecioFacturado.Value)


        End If

        lbPorcentajeVentaMecanico.Text = mecanico.ToString("0.00") + " %"
    End Sub

    Private Sub tbHasta_Click(sender As Object, e As EventArgs) Handles tbHasta.Click

    End Sub


    Public Function ObtenerPosicion(ByRef id As Integer) As Integer

        For i As Integer = 0 To CType(JGr_Descuentos.DataSource, DataTable).Rows.Count - 1 Step 1
            If (CType(JGr_Descuentos.DataSource, DataTable).Rows(i).Item("danumi") = id) Then
                Return i
            End If

        Next
        Return -1
    End Function
    Private Sub InhabilitarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InhabilitarToolStripMenuItem.Click
        If (JGr_Descuentos.Row >= 0) Then

            Dim posicion As Integer = -1
            posicion = ObtenerPosicion(JGr_Descuentos.GetValue("danumi"))
            If (posicion >= 0) Then
                CType(JGr_Descuentos.DataSource, DataTable).Rows(posicion).Item("estadoDescuento") = 0
                _prAplicarCondiccionDescuento()
                CType(JGr_Descuentos.DataSource, DataTable).Rows(posicion).Item("estado") = 2
            End If


        End If
    End Sub

    Private Sub tbProducto_TextChanged(sender As Object, e As EventArgs) Handles tbProducto.TextChanged





        Dim charSequence As String
        charSequence = tbProducto.Text.ToUpper
        If (charSequence.Trim = String.Empty) Then


            JGrM_Buscador.DataSource = dtProductoAll.Copy
        Else
            Dim Len As Integer = tbProducto.Text.Length
            Dim Ch As String = tbProducto.Text(Len - 1)
            If (Ch.Trim = String.Empty) Then
                filtrar()
            End If


        End If
    End Sub

    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        MPanelSup.Visible = False
        PanelSuperior.Visible = False
        PanelInferior.Visible = False
        GroupPanelBuscador.Visible = True
        tbProducto.Focus()

    End Sub

    Sub HabilitarDatos()
        MPanelSup.Dock = DockStyle.Fill
        MPanelSup.Visible = True
        PanelSuperior.Visible = True
        PanelInferior.Visible = True
        GroupPanelBuscador.Visible = False
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles btnCategoria.Click
        SeleccionarCategoria()
    End Sub

    Public Sub SeleccionarCategoria()
        Dim dt As DataTable
        Dim idCategoria As Integer = 0
        Dim nombreCategoria As String
        dt = L_fnListarCategoriaVentas()
        dt.Rows.Add(-1, "Todos")


        Dim listEstCeldas As New List(Of Modelo.Celda)
        listEstCeldas.Add(New Modelo.Celda("yccod3,", True, "Codigo", 100))
        listEstCeldas.Add(New Modelo.Celda("ycdes3", True, "Nombre Categoria", 500))

        Dim ef = New Efecto
        ef.tipo = 3
        ef.dt = dt
        ef.SeleclCol = 2
        ef.listEstCeldas = listEstCeldas
        ef.alto = 50
        ef.ancho = 800
        ef.Context = "Seleccione Categoria".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row
            ''yccod3,ycdes3 
            'idCategoria = Row.Cells("yccod3").Value
            'nombreCategoria = Row.Cells("ycdes3").Value



            CategoriaSeleccionada = Row.Cells("yccod3").Value
            CargarDatasourceProducto(CategoriaSeleccionada)
            lblCategoria.Text = Row.Cells("ycdes3").Value
        End If

    End Sub
    Public Sub CargarDatasourceProducto(CategoriaId As Integer)

        If (CategoriaId >= 0) Then
            Dim dt As DataTable = L_fnGeneralProductos(CategoriaId)
            JGrM_Buscador.DataSource = dt
            dtProductoAll = dt
            'If (Not IsNothing(dt.Select("yfgr5=" + Str(CategoriaId)).CopyToDataTable)) Then
            '    JGrM_Buscador.DataSource = dt.Select("yfgr5=" + Str(CategoriaId)).CopyToDataTable
            'Else
            '    dt.Rows.Clear()

            '    JGrM_Buscador.DataSource = dt

            'End If



        End If

    End Sub

    Private Sub ButtonX1_Click_1(sender As Object, e As EventArgs) Handles ButtonX1.Click
        HabilitarDatos()
        btnNuevo.PerformClick()

    End Sub

    Private Sub VerDatosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VerDatosToolStripMenuItem.Click
        If (JGrM_Buscador.Row >= 0) Then
            HabilitarDatos()

        End If


    End Sub
    Public Sub filtrar()
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

            For i As Integer = 0 To dt.Rows.Count - 1 Step 1
                Dim nombre As String = dt.Rows(i).Item("yfcdprod2").ToString.ToUpper +
                    " " + dt.Rows(i).Item("yfcprod").ToString.ToUpper +
                    " " + dt.Rows(i).Item("yfdetprod").ToString.ToUpper +
                    " " + dt.Rows(i).Item("yfcbarra").ToString.ToUpper +
                    " " + dt.Rows(i).Item("yfcdprod1").ToString.ToUpper +
                    " " + dt.Rows(i).Item("yfCodigoMarca").ToString.ToUpper +
                    " " + dt.Rows(i).Item("grupo1").ToString.ToUpper + " " + dt.Rows(i).Item("grupo2").ToString.ToUpper + " " + dt.Rows(i).Item("grupo5").ToString.ToUpper
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
            JGrM_Buscador.DataSource = dtProductoCopy.Copy
        Else
            JGrM_Buscador.DataSource = dtProductoAll.Copy
        End If
    End Sub
    Private Sub ButtonX2_Click_1(sender As Object, e As EventArgs) Handles ButtonX2.Click
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

            For i As Integer = 0 To dt.Rows.Count - 1 Step 1
                Dim nombre As String = dt.Rows(i).Item("yfcdprod2").ToString.ToUpper +
                    " " + dt.Rows(i).Item("yfcprod").ToString.ToUpper +
                    " " + dt.Rows(i).Item("yfcbarra").ToString.ToUpper +
                    " " + dt.Rows(i).Item("yfcdprod1").ToString.ToUpper +
                    " " + dt.Rows(i).Item("yfCodigoMarca").ToString.ToUpper +
                    " " + dt.Rows(i).Item("grupo1").ToString.ToUpper + " " + dt.Rows(i).Item("grupo2").ToString.ToUpper + " " + dt.Rows(i).Item("grupo5").ToString.ToUpper
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
            JGrM_Buscador.DataSource = dtProductoCopy.Copy
        Else
            JGrM_Buscador.DataSource = dtProductoAll.Copy
        End If
    End Sub

    Private Sub ClonarNuevoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClonarNuevoToolStripMenuItem.Click
        BanderaClonar = True
        HabilitarDatos()
        btnNuevo.PerformClick()


    End Sub

    Private Sub JGrM_Buscador_EditingCell(sender As Object, e As EditingCellEventArgs) Handles JGrM_Buscador.EditingCell
        e.Cancel = e.Column.Index <> JGrM_Buscador.RootTable.Columns("yfcprod").Index And
           e.Column.Index <> JGrM_Buscador.RootTable.Columns("yfCodigoMarca").Index And
           e.Column.Index <> JGrM_Buscador.RootTable.Columns("yfcdprod2").Index And
           e.Column.Index <> JGrM_Buscador.RootTable.Columns("yfdetprod").Index
    End Sub

    Private Sub btnAgregar_Click_1(sender As Object, e As EventArgs) Handles btnAgregar.Click
        If CategoriaSeleccionada > 0 Then
            CargarDatasourceProducto(CategoriaSeleccionada)
        End If

    End Sub
End Class