Imports ClosedXML.Excel

Public Class ExcelReader
    'Public Shared Function LeerEncabezadosDesdeExcel(rutaArchivo As String, nombreHoja As String, filaEncabezado As Integer) As List(Of EncabezadoJson)
    '    Dim encabezados As New List(Of EncabezadoJson)

    '    Using wb As New XLWorkbook(rutaArchivo)
    '        Dim ws As IXLWorksheet = Nothing

    '        ' Verificamos que la hoja exista por nombre exacto
    '        If Not wb.Worksheets.Any(Function(s) s.Name = nombreHoja) Then
    '            Throw New Exception($"La hoja '{nombreHoja}' no existe en el archivo.")
    '        End If

    '        ws = wb.Worksheet(nombreHoja)

    '        Dim fila = ws.Row(filaEncabezado)
    '        Dim colIndex As Integer = 1

    '        Do While True
    '            Dim celda = fila.Cell(colIndex)
    '            Dim valor As String = celda.GetValue(Of String)()

    '            ' Si se llega a una celda vacía consecutiva y toda la fila siguiente también está vacía, detener
    '            If String.IsNullOrWhiteSpace(valor) AndAlso ws.Column(colIndex).CellsUsed.Count() = 0 Then
    '                Exit Do
    '            End If

    '            Dim encabezado As New EncabezadoJson With {
    '                .NombreOriginal = valor,
    '                .Posicion = colIndex
    '            }

    '            encabezados.Add(encabezado)
    '            colIndex += 1
    '        Loop
    '    End Using

    '    EncabezadoHelper.GenerarNombresVisuales(encabezados)
    '    Return encabezados
    'End Function
End Class
