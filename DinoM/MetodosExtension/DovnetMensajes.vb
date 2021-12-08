Imports DevComponents.DotNetBar

Public Class DovnetMensajes
    Public Shared Sub MostrarMensajeError(mensaje As String, control As Control)
        ToastNotification.Show(control,
                               mensaje.ToUpper,
                               My.Resources.WARNING,
                               5000,
                               eToastGlowColor.Red,
                               eToastPosition.TopCenter)

    End Sub
    Public Shared Sub MostrarMensaje(mensaje As String, control As Control)
        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
        ToastNotification.Show(control,
                               mensaje.ToUpper,
                               img,
                               2000,
                               eToastGlowColor.Red,
                               eToastPosition.TopCenter)

    End Sub
End Class
