Imports System.Data.SqlClient
Imports ConfiguradorPlantillasWeb.ConfiguradorPlantillasExcel
Imports ConfiguradorPlantillasWeb.ConfiguradorPlantillasExcel.Utils
Imports ConfiguradorPlantillasWeb.Models
Imports System.IO

Public Class PlantillaDAO

    Public Shared Function InsertarPlantilla(conn As SqlConnection, tran As SqlTransaction, plantilla As Plantilla) As Integer
        Dim nuevaPlantillaId As Integer = 0

        Using cmd As New SqlCommand("usp_MLQ_InsertarPlantilla", conn, tran)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("@Nombre", plantilla.Nombre)
            cmd.Parameters.AddWithValue("@Hoja", plantilla.HojaNombre)
            cmd.Parameters.AddWithValue("@HojaPosicion", plantilla.HojaPosicion)
            cmd.Parameters.AddWithValue("@Fila", plantilla.RenglonInicialDatos)

            Dim paramOut As New SqlParameter("@NuevoPlantillaId", SqlDbType.Int)
            paramOut.Direction = ParameterDirection.Output
            cmd.Parameters.Add(paramOut)

            cmd.ExecuteNonQuery()
            nuevaPlantillaId = Convert.ToInt32(paramOut.Value)
        End Using

        Return nuevaPlantillaId
    End Function

    Public Shared Function ListarPlantillas(conn As SqlConnection) As DataTable
        Dim tabla As New DataTable()

        Using cmd As New SqlCommand("usp_MLQ_ListarPlantillas", conn)
            cmd.CommandType = CommandType.StoredProcedure
            Using reader = cmd.ExecuteReader()
                tabla.Load(reader)
            End Using
        End Using

        Return tabla
    End Function

    Public Shared Sub EliminarPlantillaYColumnas(conn As SqlConnection, tran As SqlTransaction, plantillaId As Integer)
        Using cmd As New SqlCommand("usp_MLQ_EliminarPlantillaYColumnas", conn, tran)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@Plantilla_ID", plantillaId)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Shared Function ObtenerPlantillasCoincidentes(hojaNombre As String, hojaPosicion As Integer, renglonInicialDatos As Integer) As List(Of PlantillaCoincidencia)
        Dim lista As New List(Of PlantillaCoincidencia)()
        Dim connStr As String = ConexionHelper.ObtenerCadena()

        Using conn As New SqlConnection(connStr)
            conn.Open()

            Using cmd As New SqlCommand("usp_MLQ_BuscarPlantillasCoincidentes", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@HojaNombre", hojaNombre)
                cmd.Parameters.AddWithValue("@HojaPosicion", hojaPosicion)
                cmd.Parameters.AddWithValue("@RenglonInicialDatos", renglonInicialDatos)

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    Dim plantillas As New Dictionary(Of Integer, PlantillaCoincidencia)()

                    While reader.Read()
                        Dim id As Integer = Convert.ToInt32(reader("Plantilla_ID"))

                        If Not plantillas.ContainsKey(id) Then
                            plantillas(id) = New PlantillaCoincidencia With {
                                .Plantilla_ID = id,
                                .HojaNombre = hojaNombre,
                                .HojaPosicion = hojaPosicion,
                                .RenglonInicialDatos = renglonInicialDatos,
                                .Columnas = New List(Of EncabezadoJson)()
                            }
                        End If

                        Dim nombre As String = reader.GetSqlString(reader.GetOrdinal("Nombre")).Value
                        Dim logEntry As String = $"ID Plantilla {id} - Posición {Convert.ToInt32(reader("Posicion"))}: '{nombre}' (Longitud: {nombre.Length})"
                        File.AppendAllText("log_plantillas_detectadas.txt", logEntry & Environment.NewLine)

                        plantillas(id).Columnas.Add(New EncabezadoJson With {
                            .Nombre = nombre,
                            .Posicion = Convert.ToInt32(reader("Posicion"))
                        })
                    End While

                    lista = plantillas.Values.OrderBy(Function(p) p.Plantilla_ID).ToList()
                End Using
            End Using
        End Using

        File.AppendAllText("log_debug.txt", $"→ Total plantillas recuperadas para filtro: {lista.Count}" & Environment.NewLine)
        Return lista
    End Function

End Class
