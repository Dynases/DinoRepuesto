Imports DevComponents.DotNetBar.Controls
Imports DevComponents.DotNetBar
Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX
Public Class F1_Cantidad
    Public Stock As Decimal
    Public Cantidad As Decimal
    Public Producto As String
    Public bandera As Boolean
    Public CategoriaPrecio As Integer = 0
    Public idProducto As Integer = 0
    Public Precio As Integer = 0

    Private Sub F_Cantidad_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        tbCantidad.Value = 0

        lbStock.Text = "Stock Disponible = " + Str(Stock)
        lbProducto.Text = Producto
        tbCantidad.Focus()
    End Sub

    Private Sub tbCantidad_Enter(sender As Object, e As EventArgs) Handles tbCantidad.Enter

    End Sub

    Private Sub tbCantidad_KeyDown(sender As Object, e As KeyEventArgs) Handles tbCantidad.KeyDown
        If (e.KeyData = Keys.Escape) Then
            bandera = False
            Me.Close()
        End If
        If (e.KeyData = Keys.Enter) Then

            If (tbCantidad.Value > Stock) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "La cantidad Ingresada " + Str(tbCantidad.Value) + " Es superior a la cantidad Disponible del Producto : " + Str(Stock), img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                Return

            Else
                If (tbCantidad.Value <= 0) Then
                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, "La cantidad debe ser mayor a 0", img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                    Return

                Else
                    If (CategoriaPrecio = 50) Then
                        Dim dt As DataTable = L_fnGeneralProductosDescuentos(idProducto)

                        Dim BanderaPrecio As Boolean = False
                        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
                            If (dt.Rows(i).Item("estadoDescuento") = True) Then
                                If (tbCantidad.Value >= dt.Rows(i).Item("dacant1") And tbCantidad.Value <= dt.Rows(i).Item("dacant2")) Then
                                    Cantidad = tbCantidad.Value
                                    bandera = True
                                    Precio = dt.Rows(i).Item("dapreciou")
                                    BanderaPrecio = True
                                    Me.Close()



                                End If
                            End If


                        Next
                        If (BanderaPrecio = False) Then
                            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 30, 30)
                            ToastNotification.Show(Me, "Cantidad ingresada no pertenece a la categoria mayorista".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                            Return
                        End If


                    Else
                        Cantidad = tbCantidad.Value
                        bandera = True
                        Me.Close()
                    End If

                End If


            End If
        End If

    End Sub

    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        If (tbCantidad.Value > Stock) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "La cantidad Ingresada " + Str(tbCantidad.Value) + " Es superior a la cantidad Disponible del Producto : " + Str(Stock), img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Return

        Else
            If (tbCantidad.Value <= 0) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "La cantidad debe ser mayor a 0", img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                Return

            Else
                If (CategoriaPrecio = 50) Then
                    Dim dt As DataTable = L_fnGeneralProductosDescuentos(idProducto)

                    Dim BanderaPrecio As Boolean = False
                    For i As Integer = 0 To dt.Rows.Count - 1 Step 1
                        If (dt.Rows(i).Item("estadoDescuento") = True) Then
                            If (tbCantidad.Value >= dt.Rows(i).Item("dacant1") And tbCantidad.Value <= dt.Rows(i).Item("dacant2")) Then
                                Cantidad = tbCantidad.Value
                                bandera = True
                                Precio = dt.Rows(i).Item("dapreciou")
                                BanderaPrecio = True
                                Me.Close()



                            End If
                        End If


                    Next
                    If (BanderaPrecio = False) Then
                        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 30, 30)
                        ToastNotification.Show(Me, "Cantidad ingresada no pertenece a la categoria mayorista".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                        Return
                    End If


                Else
                    Cantidad = tbCantidad.Value
                    bandera = True
                    Me.Close()
                End If

            End If


        End If
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        bandera = False
        Me.Close()
    End Sub
End Class