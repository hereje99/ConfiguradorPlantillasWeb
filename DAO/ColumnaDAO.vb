Imports System.Data.SqlClient
Imports ConfiguradorPlantillasWeb.ConfiguradorPlantillasExcel.Utils
Imports ConfiguradorPlantillasWeb.Models

Public Class ColumnaDAO
    Public Shared Sub InsertarColumnas(conn As SqlConnection, tran As SqlTransaction, plantillaId As Integer, conceptos As List(Of Concepto))
        Dim posicion As Integer = 1

        For Each concepto In conceptos
            Using cmd As New SqlCommand("usp_MLQ_InsertarColumna", conn, tran)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("@PlantillaId", plantillaId)
                cmd.Parameters.AddWithValue("@ConceptoId", concepto.Concepto_ID)
                cmd.Parameters.AddWithValue("@Posicion", posicion)

                cmd.ExecuteNonQuery()
            End Using

            posicion += 1
        Next
    End Sub

    Public Shared Function ObtenerColumnasPorPlantilla(plantillaId As Integer) As DataTable
        Dim dt As New DataTable()

        Using conn As New SqlConnection(ConexionHelper.ObtenerCadena())
            Using cmd As New SqlCommand("usp_MLQ_ObtenerColumnasPorPlantilla", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@PlantillaID", plantillaId)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        Return dt
    End Function

    Public Shared Function ListarConceptosMaestros() As DataTable
        Dim tabla As New DataTable()

        Using conn As New SqlConnection(ConexionHelper.ObtenerCadena())
            Using cmd As New SqlCommand("usp_MLQ_ListarConceptosMaestros", conn)
                cmd.CommandType = CommandType.StoredProcedure
                conn.Open()

                Using reader = cmd.ExecuteReader()
                    tabla.Load(reader)
                End Using
            End Using
        End Using

        Return tabla
    End Function

    Public Shared Function AsignarConceptoMaestro(plantillaId As Integer, conceptoMaestroId As Integer, posicion As Integer) As String
        Try
            Using conn As New SqlConnection(ConexionHelper.ObtenerCadena())
                Using cmd As New SqlCommand("usp_MLQ_AsignarConceptoMaestroAColumna", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@Plantilla_ID", plantillaId)
                    cmd.Parameters.AddWithValue("@ConceptoMaestro_ID", conceptoMaestroId)
                    cmd.Parameters.AddWithValue("@Posicion", posicion)

                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Return Nothing
        Catch ex As SqlException
            Return ex.Message
        End Try
    End Function

    Public Shared Function QuitarConceptoMaestro(plantillaId As Integer, posicion As Integer) As Boolean
        Try
            Using conn As New SqlConnection(ConexionHelper.ObtenerCadena())
                Using cmd As New SqlCommand("usp_MLQ_QuitarConceptoMaestro", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@Plantilla_ID", plantillaId)
                    cmd.Parameters.AddWithValue("@Posicion", posicion)

                    conn.Open()
                    cmd.ExecuteNonQuery()
                    Return True
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function


End Class
