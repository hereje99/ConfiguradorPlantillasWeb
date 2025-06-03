Imports ConfiguradorPlantillasWeb.ConfiguradorPlantillasExcel

Namespace Models
    Public Class PlantillaCoincidencia
        Public Property Plantilla_ID As Integer
        Public Property HojaNombre As String
        Public Property HojaPosicion As Integer
        Public Property RenglonInicialDatos As Integer
        Public Property Columnas As List(Of EncabezadoJson)
    End Class
End Namespace
