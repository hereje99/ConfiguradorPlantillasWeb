Namespace Models
    Public Class Plantilla
        Public Property TipoPlantilla_ID As Integer
        Public Property Plantilla_ID As Integer
        Public Property Nombre As String
        Public Property PosicionONombreColumnas As Boolean
        Public Property PosicionONombreHojas As Boolean
        Public Property TieneEncabezado As Boolean
        Public Property RenglonInicialDatos As Integer?
        Public Property HojaNombre As String
        Public Property HojaPosicion As Integer?
        Public Property EncabezadoPosicion As Integer?
        Public Property Activo As Boolean
        Public Property FechaRegistro As DateTime?
        Public Property UsuarioRegistro As String
    End Class
End Namespace
