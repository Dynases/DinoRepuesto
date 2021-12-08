Imports System.Drawing
Imports System.IO
Imports DevComponents.DotNetBar
Imports Janus.Windows
Imports Janus.Windows.GridEX
Imports Janus.Windows.GridEX.EditControls

Public Class JanusExtension

#Region "Griex"
    Public Shared Sub ConfigInicialVinculado(grid As GridEX, tabla As DataTable, nombre As String)
        grid.BoundMode = BoundMode.Bound
        grid.SetDataBinding(tabla, nombre)
        grid.RetrieveStructure()

    End Sub
#Region "Columnas"
    Public Shared Sub ColNoVisible(grid As GridEX, key As String)
        With grid.RootTable.Columns(key)
            .Visible = False
            .AllowSort = False
        End With
    End Sub

    Public Shared Sub ColAL(grid As GridEX, key As String, nombre As String, ancho As Integer, Optional tamFuente As Integer = 8)
        With grid.RootTable.Columns(key)
            .CellStyle.TextAlignment = TextAlignment.Near
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .MaxLines = 20
            .Visible = True
            .AllowSort = False
            ColPropiedadesComun(grid, key, nombre, ancho)
        End With
    End Sub
    Public Shared Sub ColCombo(grid As GridEX, key As String, nombre As String, ancho As Integer, combo As MultiColumnCombo, visible As Boolean, Optional tamFuente As Integer = 8)
        With grid.RootTable.Columns(key)
            .CellStyle.TextAlignment = TextAlignment.Near
            .Visible = visible
            .EditType = EditType.MultiColumnCombo
            .DropDown = combo.DropDownList
            .AllowSort = False
            ColPropiedadesComun(grid, key, nombre, ancho)
        End With
    End Sub
    Public Shared Sub ColALFecha(grid As GridEX, key As String, nombre As String, ancho As Integer, Optional tamFuente As Integer = 8)
        With grid.RootTable.Columns(key)
            .CellStyle.TextAlignment = TextAlignment.Near
            .FormatString = "yyyy/MM/dd"
            ColPropiedadesComun(grid, key, nombre, ancho)
        End With
    End Sub
    Public Shared Sub ColAC(grid As GridEX, key As String, nombre As String, ancho As Integer, Optional tamFuente As Integer = 8)
        With grid.RootTable.Columns(key)
            .CellStyle.TextAlignment = TextAlignment.Center
            .Visible = True
            ColPropiedadesComun(grid, key, nombre, ancho)
        End With
    End Sub
    Public Shared Sub ColArNro(grid As GridEX, key As String, nombre As String, ancho As Integer, formato As String, Optional tamFuente As Integer = 8)
        With grid.RootTable.Columns(key)
            .CellStyle.TextAlignment = TextAlignment.Far
            .CellStyle.LineAlignment = TextAlignment.Center
            .FormatString = formato
            .Visible = True
            .AllowSort = False
            ColPropiedadesComun(grid, key, nombre, ancho)
        End With
    End Sub
    Public Shared Sub ColIcon(grid As GridEX, key As String, posicion As Integer, nombre As String, imagen As Image, Optional ancho As Integer = 40)
        With grid.RootTable.Columns(key)
            .Position = posicion
            ColPropiedadesComun(grid, key, nombre, ancho)
        End With
    End Sub
    Public Shared Sub ColIcon(grid As GridEX, key As String, nombre As String, imagen As Image, Optional ancho As Integer = 40)
        With grid.RootTable.Columns(key)
            ColPropiedadesComun(grid, key, nombre, ancho)
        End With
    End Sub
#End Region
#End Region
#Region "Configuracion Final"
    Public Shared Sub ConfigFinalCompleto(grid As GridEX, Optional fijarColumnas As Integer = 0)
        With grid
            .GroupByBoxVisible = True
            .FilterRowButtonStyle = FilterRowButtonStyle.ConditionOperatorDropDown
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .VisualStyle = VisualStyle.Office2007
            .SelectionMode = Janus.Windows.GridEX.SelectionMode.MultipleSelection
            .AlternatingColors = True
            .RecordNavigator = True
            .AllowEdit = InheritableBoolean.False
            .AllowColumnDrag = True
            .AutomaticSort = True
            .RootTable.HeaderLines = 2
            .RootTable.RowHeight = 20
            .CellSelectionMode = CellSelectionMode.SingleCell
            .RowHeaders = InheritableBoolean.True
            .RowHeaderContent = RowHeaderContent.RowIndex
            .GroupByBoxInfoText = "Arrastre un encabezado de columna aquí para agrupar por esa columna"
            If fijarColumnas > 0 Then
                grid.FrozenColumns = fijarColumnas
            End If
        End With
    End Sub
    Public Shared Sub ConfigFinalBasica(grid As GridEX, Optional fijarColumnas As Integer = 0)
        With grid
            .GroupByBoxVisible = False
            .AlternatingColors = True
            '.FilterRowButtonStyle = FilterRowButtonStyle.ConditionOperatorDropDown
            '.DefaultFilterRowComparison = FilterConditionOperator.Contains
            '.FilterMode = FilterMode.Automatic
            '.FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .VisualStyle = VisualStyle.Office2007
            .AllowEdit = InheritableBoolean.False
        End With
    End Sub
    Public Shared Sub ConfigFinalDetalle(grid As GridEX)
        With grid
            .GroupByBoxVisible = False
            .AlternatingColors = True
            .FilterMode = FilterMode.None
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .VisualStyle = VisualStyle.Office2007
        End With
    End Sub
#End Region
#Region "Ayuda"
    Public Shared Function CelValor(Of T)(grid As GridEX, key As String) As T
        Return grid.CurrentRow.Cells(key).Value
    End Function
    Public Shared Sub SetCelValor(grid As GridEX, posicion As Integer, key As String, valor As String)
        CType(grid.DataSource, DataTable).Rows(posicion).Item(key) = valor
    End Sub
    Public Shared Function GetCelValor(Of T)(grid As GridEX, posicion As Integer, key As String) As T
        Return CType(grid.DataSource, DataTable).Rows(posicion).Item(key)
    End Function
    Public Shared Function CelIndex(grid As GridEX, key As String) As Integer
        Return grid.RootTable.Columns(key).Index
    End Function
    Public Shared Sub SeleccionarColumnaFila(grid As GridEX, columna As Integer, fila As Integer)
        grid.Col = columna
        grid.Row = fila
    End Sub

#End Region

#Region "Metodos Privados"
    Public Shared Sub ColPropiedadesComun(grid As GridEX, key As String, nombre As String, ancho As Integer)
        With grid.RootTable.Columns(key)
            .Width = ancho
            .Caption = nombre
            .HeaderAlignment = TextAlignment.Center
        End With
    End Sub

    Public Shared Sub ColIconPropiedadesComun(grid As GridEX, key As String, nombre As String, imagen As Image, ancho As Integer)
        With grid.RootTable.Columns.Add(key, ColumnType.Image)
            .Width = ancho
            .Caption = nombre
            .HeaderAlignment = TextAlignment.Center
            .Image = imagen
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
        End With
    End Sub
    Public Shared Sub ObtenerFilaDetalle(ByRef pos As Integer, numi As Integer, griex As GridEX, key As String)
        For i As Integer = 0 To CType(griex.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(griex.DataSource, DataTable).Rows(i).Item(key)
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next
    End Sub

#End Region
#Region "MultiColumnCombo"
    Public Shared Sub seleccionarPrimero(combo As MultiColumnCombo)
        Try
            combo.SelectedIndex = 0
        Catch ex As Exception

        End Try
    End Sub
#End Region
#Region "Imagenes"
    Public Shared Sub ObtenerImagenAddDetalle(ByRef bin As MemoryStream, Optional ByRef Bin02 As MemoryStream = Nothing)
        Dim img As New Bitmap(My.Resources.delete, 28, 28)
        Dim img02 As New Bitmap(My.Resources.add, 28, 28)
        img.Save(bin, Imaging.ImageFormat.Png)
        img02.Save(Bin02, Imaging.ImageFormat.Png)
    End Sub
#End Region
#Region "Metodos Datatable"
    Public Shared Function ObtenerIdMayor(griex As GridEX, key As String) As Integer
        Dim dt As DataTable = CType(griex.DataSource, DataTable)
        Dim mayor As Integer = 0
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim data As Integer = IIf(IsDBNull(CType(griex.DataSource, DataTable).Rows(i).Item(key)), 0, CType(griex.DataSource, DataTable).Rows(i).Item(key))
            If (data > mayor) Then
                mayor = data
            End If
        Next
        Return mayor
    End Function
    Public Shared Sub EliminarFIla(griex As GridEX, key As String, keyEstado As String, valor As Integer)
        If (griex.Row >= 0) Then
            If (griex.RowCount >= 1) Then
                Dim estado As Integer = griex.GetValue(keyEstado)
                Dim posicion = ObtenerPosicionFila(griex, key, valor)
                If (estado = 0) Then
                    SetCelValor(griex, posicion, keyEstado, -2)
                End If
                If (estado = 1) Then
                    SetCelValor(griex, posicion, keyEstado, -1)
                End If
            End If
        End If
    End Sub
    Public Shared Function ObtenerPosicionFila(griex As GridEX, key As String, valor As Integer) As Integer
        'Dim valor As Integer = griex.GetValue(key)
        Dim posicion As Integer = -1
        ObtenerFilaDetalle(posicion, valor, griex, key)
        Return posicion
    End Function

    Public Shared Sub navegarPrimerRegistro(griex As GridEX)
        If griex.RowCount > 0 Then
            griex.Row = 0
        End If
    End Sub
    Public Shared Sub navegarSiguienteRegistro(griex As GridEX)
        Dim posicion As Integer = griex.Row
        If posicion < griex.RowCount - 1 Then
            griex.Row = griex.Row + 1
        End If
    End Sub
    Public Shared Sub navegarUltimoRegistro(griex As GridEX)
        If griex.RowCount > 0 Then
            griex.Row = griex.RowCount - 1
        End If
    End Sub
    Public Shared Sub navegarAnteriorRegistro(griex As GridEX)
        Dim posicion As Integer = griex.Row
        If posicion > 0 And griex.RowCount > 0 Then
            griex.Row = griex.Row - 1
        End If
    End Sub
#End Region

End Class
