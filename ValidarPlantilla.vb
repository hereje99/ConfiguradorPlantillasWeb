Imports System.Data.SqlClient
Imports ClosedXML.Excel
Imports DocumentFormat.OpenXml.Drawing.Wordprocessing

Public Class ValidarPlantilla

    Public Shared Function ObtenerEncabezadosDesdeExcel(rutaArchivo As String, nombreHoja As String, filaEncabezado As Integer) As List(Of EncabezadoJson)
        Dim encabezados As New List(Of EncabezadoJson)()
        Dim nombresExistentes As New HashSet(Of String)()
        Dim contadorSinNombre As Integer = 1

        Using workbook As New XLWorkbook(rutaArchivo)
            Dim hoja = workbook.Worksheet(nombreHoja)
            Dim fila = hoja.Row(filaEncabezado)
            Dim ultimaColumna As Integer = hoja.RangeUsed().RangeAddress.LastAddress.ColumnNumber

            For i As Integer = 1 To ultimaColumna
                Dim celda = fila.Cell(i)
                Dim nombreOriginal As String = celda.GetString()
                Dim nombreFinal As String = nombreOriginal

                If String.IsNullOrWhiteSpace(nombreOriginal) Then
                    nombreFinal = "SIN_NOMBRE_" & contadorSinNombre
                    contadorSinNombre += 1
                End If

                While nombresExistentes.Contains(nombreFinal)
                    nombreFinal &= "_2"
                End While

                nombresExistentes.Add(nombreFinal)

                encabezados.Add(New EncabezadoJson With {
                    .NombreFinal = "·" & nombreFinal & "·",
                    .NombreOriginal = "·" & nombreOriginal & "·",
                    .Posicion = i
                })
            Next
        End Using

        Return encabezados
    End Function


    Public Shared Function CoincideEstructura(encabezadosArchivo As List(Of EncabezadoJson), encabezadosBD As List(Of EncabezadoJson)) As Boolean
        If encabezadosArchivo.Count <> encabezadosBD.Count Then
            Return False
        End If

        For i = 0 To encabezadosArchivo.Count - 1
            If encabezadosArchivo(i).NombreFinal <> encabezadosBD(i).NombreFinal Then
                Return False
            End If
        Next

        Return True
    End Function


    Public Shared Function BuscarPlantillaCoincidente(encabezadosArchivo As List(Of EncabezadoJson)) As CoincidenciaResultado
        Dim resultado As New CoincidenciaResultado With {
        .Coincide = False
    }

        Dim connStr = ConfigurationManager.ConnectionStrings("BD_SAETA_DEV").ConnectionString

        Using conn As New SqlConnection(connStr)
            conn.Open()

            ' Obtener todas las plantillas disponibles
            Dim cmdPlantillas As New SqlCommand("SELECT DISTINCT Plantilla_ID FROM MLQ_Columna WHERE Activo = 1 ORDER BY Plantilla_ID", conn)
            Dim readerPlantillas = cmdPlantillas.ExecuteReader()

            Dim plantillas As New List(Of Integer)
            While readerPlantillas.Read()
                plantillas.Add(Convert.ToInt32(readerPlantillas("Plantilla_ID")))
            End While
            readerPlantillas.Close()

            ' Comparar cada plantilla contra la estructura cargada
            For Each plantillaID In plantillas
                Dim cmdCols As New SqlCommand("SELECT Nombre, Posicion FROM MLQ_Columna WHERE Plantilla_ID = @PlantillaID AND Activo = 1 ORDER BY Posicion", conn)
                cmdCols.Parameters.AddWithValue("@PlantillaID", plantillaID)

                Dim readerCols = cmdCols.ExecuteReader()
                Dim encabezadosBD As New List(Of EncabezadoJson)

                While readerCols.Read()
                    encabezadosBD.Add(New EncabezadoJson With {
                    .NombreFinal = readerCols("Nombre").ToString(),
                    .NombreOriginal = readerCols("Nombre").ToString(),
                    .Posicion = Convert.ToInt32(readerCols("Posicion"))
                })
                End While
                readerCols.Close()

                ' Verificar coincidencia exacta
                If CoincideEstructura(encabezadosArchivo, encabezadosBD) Then
                    resultado.Coincide = True
                    resultado.PlantillaID = plantillaID

                    ' Obtener nombre de plantilla
                    Dim cmdNombre As New SqlCommand("SELECT Nombre FROM MLQ_Plantilla WHERE Plantilla_ID = @PlantillaID", conn)
                    cmdNombre.Parameters.AddWithValue("@PlantillaID", plantillaID)
                    Dim nombre = cmdNombre.ExecuteScalar()
                    resultado.NombrePlantilla = If(nombre IsNot Nothing, nombre.ToString(), "Desconocido")
                    Exit For
                End If
            Next
        End Using

        Return resultado
    End Function



End Class
