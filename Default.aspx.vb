Imports System.IO
Imports ClosedXML.Excel
Imports ConfiguradorPlantillasWeb
Imports ConfiguradorPlantillasWeb.ConfiguradorPlantillasWeb

Public Class _Default
    Inherits Page

    Protected Sub btnPrevisualizar_Click(sender As Object, e As EventArgs) Handles btnPrevisualizar.Click
        'lblResultado.Text = ""
        'lblResultado.ForeColor = Drawing.Color.Black

        'Try
        '    ' Guardar el archivo si aún no está en memoria
        '    If ViewState("RutaExcel") Is Nothing Then
        '        If FileUpload1.HasFile Then
        '            Dim rutaGuardada As String = ExcelHelper.GuardarArchivoTemporal(FileUpload1)
        '            ViewState("RutaExcel") = rutaGuardada
        '        Else
        '            lblResultado.Text = "Error: primero debe subir un archivo."
        '            lblResultado.ForeColor = Drawing.Color.Red
        '            Return
        '        End If
        '    End If

        '    ' Obtener ruta ya guardada
        '    Dim ruta As String = ViewState("RutaExcel").ToString()
        '    Dim hojaSeleccionada As String = ddlHojas.SelectedValue
        '    Dim filaEncabezado As Integer = Convert.ToInt32(txtFila.Text)

        '    ' Leer encabezados desde Excel (sin trim)
        '    Dim encabezados As List(Of EncabezadoJson) = ValidarPlantilla.ObtenerEncabezadosDesdeExcel(ruta, hojaSeleccionada, filaEncabezado)

        '    ' Buscar coincidencia exacta con alguna plantilla existente
        '    Dim logComparacion As New List(Of String)
        '    Dim resultadoCoincidencia As CoincidenciaResultado = ValidarPlantilla.BuscarPlantillaCoincidente(encabezados, hojaSeleccionada, filaEncabezado, logComparacion)

        '    If resultadoCoincidencia IsNot Nothing AndAlso resultadoCoincidencia.PlantillaID > 0 Then
        '        lblResultado.Text = $"<br/>Coincide con plantilla ID: {resultadoCoincidencia.PlantillaID}"
        '        lblResultado.ForeColor = Drawing.Color.Red
        '    Else
        '        lblResultado.Text = "<br/>No coincide con ninguna plantilla."
        '        lblResultado.ForeColor = Drawing.Color.Black
        '    End If

        '    ' Mostrar log en el TextBox
        '    txtLogComparacion.Text = String.Join(Environment.NewLine, logComparacion)


        '    If resultadoCoincidencia IsNot Nothing AndAlso resultadoCoincidencia.PlantillaID > 0 Then
        '        lblResultado.Text = $"<br/>Coincide con plantilla ID: {resultadoCoincidencia.PlantillaID}"
        '        lblResultado.ForeColor = Drawing.Color.Red
        '    Else
        '        lblResultado.Text = "<br/>No coincide con ninguna plantilla."
        '        lblResultado.ForeColor = Drawing.Color.Black
        '    End If

        'Catch ex As Exception
        '    lblResultado.Text = "Error al procesar el archivo: " & ex.Message
        '    lblResultado.ForeColor = Drawing.Color.Red
        'End Try
    End Sub

    Protected Sub btnSubirArchivo_Click(sender As Object, e As EventArgs) Handles btnSubirArchivo.Click
        lblResultado.Text = ""
        lblResultado.ForeColor = Drawing.Color.Black

        Try
            If FileUpload1.HasFile Then
                ' Guardar archivo y ruta en ViewState
                Dim ruta As String = ExcelHelper.GuardarArchivoTemporal(FileUpload1)
                ViewState("RutaExcel") = ruta

                ' Cargar nombres de hojas
                Using workbook = New ClosedXML.Excel.XLWorkbook(ruta)
                    ddlHojas.Items.Clear()
                    For Each hoja In workbook.Worksheets
                        ddlHojas.Items.Add(New ListItem(hoja.Name))
                    Next
                End Using

                lblResultado.Text = "Archivo cargado correctamente. Ahora seleccione hoja y previsualice."
            Else
                lblResultado.Text = "Debe seleccionar un archivo."
                lblResultado.ForeColor = Drawing.Color.Red
            End If
        Catch ex As Exception
            lblResultado.Text = "Error al cargar el archivo: " & ex.Message
            lblResultado.ForeColor = Drawing.Color.Red
        End Try
    End Sub





End Class
