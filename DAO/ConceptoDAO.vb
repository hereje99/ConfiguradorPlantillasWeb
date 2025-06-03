Imports System.Data.SqlClient
Imports ConfiguradorPlantillasWeb.Models

Public Class ConceptoDAO
    Public Shared Function InsertarConceptosDesdeEncabezados(conn As SqlConnection, tran As SqlTransaction, encabezados As List(Of String)) As List(Of Concepto)
        Dim conceptos As New List(Of Concepto)()

        For Each nombre As String In encabezados
            Using cmd As New SqlCommand("usp_MLQ_InsertarConcepto", conn, tran)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@Nombre", nombre)

                Dim paramOut As New SqlParameter("@Concepto_ID", SqlDbType.Int)
                paramOut.Direction = ParameterDirection.Output
                cmd.Parameters.Add(paramOut)

                cmd.ExecuteNonQuery()

                conceptos.Add(New Concepto With {
                .Concepto_ID = Convert.ToInt32(paramOut.Value),
                .TipoPlantilla_ID = 1,
                .Nombre = nombre
            })
            End Using
        Next

        Return conceptos
    End Function

End Class
