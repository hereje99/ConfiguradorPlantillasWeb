Imports System.Configuration

Namespace ConfiguradorPlantillasExcel.Utils
    Public Module ConexionHelper

        Public Property NombreConexionActual As String = "BD_SAETA_DEV"

        Public Function ObtenerCadena() As String
            Return ConfigurationManager.ConnectionStrings(NombreConexionActual).ConnectionString
        End Function

    End Module
End Namespace
