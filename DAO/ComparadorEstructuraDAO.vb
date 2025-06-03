Imports System.Data.SqlClient
Imports ConfiguradorPlantillasWeb.ConfiguradorPlantillasExcel.Utils

Public NotInheritable Class ComparadorEstructuraDAO

    Private Sub New()
    End Sub

    Public Shared Function ObtenerEncabezadosPorPlantilla(plantillaId As Integer) As List(Of String)
        Dim encabezados As New List(Of String)()
        Dim connStr = ConexionHelper.ObtenerCadena()

        Using conn As New SqlConnection(connStr)
            conn.Open()

            Using cmd As New SqlCommand("usp_MLQ_ObtenerEncabezadosPorPlantilla", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@PlantillaId", plantillaId)

                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim nombre = reader.GetSqlString(reader.GetOrdinal("Nombre")).Value
                        encabezados.Add(nombre)
                    End While
                End Using
            End Using
        End Using

        Return encabezados
    End Function

End Class
