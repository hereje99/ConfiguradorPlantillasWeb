Imports ClosedXML.Excel

Public Class ExcelHelper
    Public Shared Function LeerEncabezadosDesdeExcel(rutaArchivo As String, hojaNombre As String, filaInicio As Integer) As List(Of EncabezadoJson)
        Dim encabezados As New List(Of EncabezadoJson)()

        Using workbook = New XLWorkbook(rutaArchivo)
            Dim hoja = workbook.Worksheet(hojaNombre)
            Dim fila = hoja.Row(filaInicio)

            Dim index As Integer = 1
            For Each celda In fila.Cells()
                Dim valorCelda = If(celda.Value?.ToString(), "").Trim()
                If String.IsNullOrWhiteSpace(valorCelda) Then
                    valorCelda = $"SIN_NOMBRE_{index}"
                End If

                encabezados.Add(New EncabezadoJson With {
                    .NombreOriginal = valorCelda,
                    .Posicion = index
                })

                index += 1
            Next
        End Using

        ' Aplicar nombres visuales únicos (como en Form1.cs)
        EncabezadoHelper.GenerarNombresVisuales(encabezados)

        Return encabezados
    End Function
End Class
