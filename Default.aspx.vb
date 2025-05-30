Imports System.IO
Imports ClosedXML.Excel
Imports ConfiguradorPlantillasWeb
Imports ConfiguradorPlantillasWeb.ConfiguradorPlantillasWeb

Public Class _Default
    Inherits Page

    Protected Sub btnPrevisualizar_Click(sender As Object, e As EventArgs) Handles btnPrevisualizar.Click
        gvEncabezados.DataSource = Nothing
        gvEncabezados.DataBind()

        Dim ruta As String = Server.MapPath("~/App_Data/ExcelTemporal.xlsx")

        If fuArchivoExcel.HasFile Then
            fuArchivoExcel.SaveAs(ruta)
            Session("RutaExcel") = ruta
            ddlHojas.Items.Clear()

            Using workbook = New XLWorkbook(ruta)
                For Each hoja In workbook.Worksheets
                    ddlHojas.Items.Add(hoja.Name)
                Next
            End Using

            If ddlHojas.Items.Count > 0 Then
                ddlHojas.SelectedIndex = 0
            End If

            lblResultado.Text = "Hojas detectadas. Vuelve a presionar Previsualizar."
            Exit Sub
        End If

        If Session("RutaExcel") Is Nothing Then
            lblResultado.Text = "Primero debes cargar un archivo."
            Exit Sub
        End If

        ruta = Session("RutaExcel").ToString()
        Dim hojaSeleccionada As String = ddlHojas.SelectedValue
        Dim filaEncabezado As Integer

        If Not Integer.TryParse(txtFilaEncabezado.Text.Trim(), filaEncabezado) Then
            lblResultado.Text = "Fila de encabezado inválida."
            Exit Sub
        End If

        ' Paso 1: Leer encabezados desde Excel
        Dim encabezados As List(Of EncabezadoJson) = ValidarPlantilla.ObtenerEncabezadosDesdeExcel(ruta, hojaSeleccionada, filaEncabezado)

        ' Paso 2: Verificar si hay coincidencia con la base de datos
        Dim plantillaCoincidente = ValidarPlantilla.BuscarPlantillaCoincidente(encabezados)

        If plantillaCoincidente IsNot Nothing Then
            lblResultado.Text = $"Coincide con plantilla existente (ID: {plantillaCoincidente.PlantillaID})"
        Else
            lblResultado.Text = "No coincide con ninguna plantilla existente."
        End If

        gvEncabezados.DataSource = encabezados
        gvEncabezados.DataBind()
    End Sub



End Class
