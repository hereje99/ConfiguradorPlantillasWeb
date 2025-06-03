Imports ConfiguradorPlantillasWeb.ConfiguradorPlantillasExcel.Utils

Public NotInheritable Class ComparadorEstructuraHelper

    Private Sub New()
    End Sub

    Public Shared Function ObtenerEncabezadosDesdeBD(hoja As String, hojaPosicion As Integer, renglonInicialDatos As Integer, plantillaId As Integer) As List(Of String)
        Return ComparadorEstructuraDAO.ObtenerEncabezadosPorPlantilla(plantillaId)
    End Function

End Class