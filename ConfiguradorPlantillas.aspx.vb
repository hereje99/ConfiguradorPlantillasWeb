Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Text
Imports ClosedXML.Excel
Imports ConfiguradorPlantillasWeb.ConfiguradorPlantillasExcel
Imports ConfiguradorPlantillasWeb.ConfiguradorPlantillasExcel.Utils
Imports ConfiguradorPlantillasWeb.Models

Public Class ConfiguradorPlantillas

    Inherits System.Web.UI.Page
    Private hojaPosiciones As New Dictionary(Of String, Integer)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            btnGuardarPlantilla.Enabled = False
        End If
    End Sub

    Protected Sub btnSeleccionarArchivo_Click(sender As Object, e As EventArgs)
        If Not fuArchivo.HasFile Then
            lblMensaje.Text = "Por favor selecciona un archivo."
            Return
        End If

        Try
            ' Guardar archivo temporalmente en servidor
            Dim rutaTemporal As String = Path.Combine(Server.MapPath("~/Temp"), Path.GetFileName(fuArchivo.FileName))
            fuArchivo.SaveAs(rutaTemporal)

            lblArchivoSeleccionado.Text = "Archivo seleccionado: " & Path.GetFileName(fuArchivo.FileName)

            ' Guardar ruta en sesión para usarla después
            Session("archivoExcel") = rutaTemporal

            ' Limpiar estado previo
            txtNombrePlantilla.Text = ""
            txtFilaEncabezado.Text = "1"
            ddlHojas.Items.Clear()
            gvDatos.DataSource = Nothing
            gvDatos.DataBind()
            lblMensaje.Text = ""

            hojaPosiciones.Clear()

            Dim log As New StringBuilder()
            log.AppendLine($"[Prueba] Análisis de posiciones reales del archivo: {Path.GetFileName(rutaTemporal)}")
            Dim indexVisual As Integer = 1

            Using workbook As New XLWorkbook(rutaTemporal)
                For Each sheet In workbook.Worksheets
                    ddlHojas.Items.Add(sheet.Name)
                    hojaPosiciones(sheet.Name) = sheet.Position
                    log.AppendLine($"Visual #{indexVisual} → Nombre: '{sheet.Name}', Posición interna: {sheet.Position}")
                    indexVisual += 1
                Next
            End Using

            File.AppendAllText("log_proceso.txt", log.ToString())

            If ddlHojas.Items.Count > 0 Then
                ddlHojas.SelectedIndex = 0
            End If

        Catch ex As Exception
            lblMensaje.Text = "Error al leer el archivo: " & ex.Message
        End Try
    End Sub

    Protected Sub btnPrevisualizar_Click(sender As Object, e As EventArgs) Handles btnPrevisualizar.Click
        Try
            lblMensaje.Text = ""

            If Session("archivoExcel") Is Nothing OrElse ddlHojas.SelectedValue Is Nothing Then
                ClientScript.RegisterStartupScript(Me.GetType(), "alerta", "alert('Selecciona un archivo y una hoja.');", True)
                Return
            End If

            Dim hoja As String = ddlHojas.SelectedValue
            Dim renglonEncabezado As Integer = Integer.Parse(txtFilaEncabezado.Text)
            Dim renglonInicialDatos As Integer = renglonEncabezado + 1
            Dim hojaPosicion As Integer = If(hojaPosiciones.ContainsKey(hoja), hojaPosiciones(hoja), 1)

            Dim tabla As New DataTable()
            Dim encabezadosJson As New List(Of EncabezadoJson)()
            Dim nombresOriginales As New List(Of String)()
            Dim rutaTemporal As String = Session("archivoExcel")

            If Not File.Exists(rutaTemporal) Then
                ClientScript.RegisterStartupScript(Me.GetType(), "error", "alert('El archivo temporal no está disponible. Vuelve a seleccionarlo.');", True)
                Return
            End If

            Using workbook As New XLWorkbook(rutaTemporal)
                Dim worksheet = workbook.Worksheet(hojaPosicion)
                Dim lastCell = worksheet.RangeUsed()?.RangeAddress.LastAddress
                Dim totalColumnas As Integer = If(lastCell IsNot Nothing, lastCell.ColumnNumber, 0)

                ' 1. Obtener encabezados reales
                For col As Integer = 1 To totalColumnas
                    Dim valor = worksheet.Cell(renglonEncabezado, col).GetString() ' NO Trim
                    Dim nombre = If(String.IsNullOrWhiteSpace(valor), $"SIN_NOMBRE_{col}", valor)
                    nombresOriginales.Add(nombre)

                    encabezadosJson.Add(New EncabezadoJson With {
                    .Nombre = nombre,
                    .Posicion = col
                })
                Next

                ' 2. Generar nombres visuales
                Dim nombresVisuales = EncabezadoHelper.GenerarNombresVisuales(nombresOriginales)

                For Each nombre In nombresVisuales
                    tabla.Columns.Add(nombre)
                Next

                For fila = renglonInicialDatos To renglonInicialDatos + 2
                    Dim nuevaFila = tabla.NewRow()
                    For i = 0 To encabezadosJson.Count - 1
                        Dim colIndex = encabezadosJson(i).Posicion
                        nuevaFila(i) = worksheet.Cell(fila, colIndex).GetString()
                    Next
                    tabla.Rows.Add(nuevaFila)
                Next

                gvDatos.DataSource = tabla
                gvDatos.DataBind()

                ' 3. Validación de estructura
                Dim coincidencia = ValidarPlantilla.BuscarPlantillaCoincidente(hoja, hojaPosicion, renglonInicialDatos, encabezadosJson)

                Dim rutaLog As String = Server.MapPath("~/log_encabezados_actual.txt")
                Using sw As New StreamWriter(rutaLog, False)
                    For Each enc In encabezadosJson
                        sw.WriteLine($"{enc.Posicion:D2}: '{enc.Nombre}'")
                    Next
                End Using

                If coincidencia IsNot Nothing Then
                    Dim configuracionBD = ComparadorEstructuraHelper.ObtenerEncabezadosDesdeBD(coincidencia.HojaNombre, coincidencia.HojaPosicion, coincidencia.RenglonInicialDatos, coincidencia.Plantilla_ID)
                    Dim mismaCantidad = nombresOriginales.Count = configuracionBD.Count
                    Dim mismosEncabezados = CompararListasExactamente(nombresOriginales, configuracionBD)

                    If mismaCantidad AndAlso mismosEncabezados Then
                        Dim mensaje = $"Ya existe una plantilla con esa configuración. Plantilla_ID: {coincidencia.Plantilla_ID}"
                        ClientScript.RegisterStartupScript(Me.GetType(), "info", $"alert('{mensaje}');", True)
                        lblMensaje.Text = mensaje
                        btnGuardarPlantilla.Enabled = False
                        Return
                    Else
                        Dim mensaje = $"Ya existe una plantilla con esa hoja y fila, pero con estructura diferente (ID: {coincidencia.Plantilla_ID})."
                        ClientScript.RegisterStartupScript(Me.GetType(), "info2", $"alert('{mensaje}');", True)
                        lblMensaje.Text = mensaje
                        btnGuardarPlantilla.Enabled = True
                        Return
                    End If
                End If

                btnGuardarPlantilla.Enabled = True
                ClientScript.RegisterStartupScript(Me.GetType(), "info3", $"alert('Total de columnas visualizadas: {tabla.Columns.Count}');", True)

            End Using

        Catch ex As Exception
            ClientScript.RegisterStartupScript(Me.GetType(), "error", $"alert('Error al procesar el archivo:\n{ex.Message}');", True)
            Logger.LogError("[btnPrevisualizar_Click] Error al procesar el archivo: " & ex.ToString())
        End Try
    End Sub


    Protected Sub BtnGuardarPlantilla_Click(sender As Object, e As EventArgs) Handles btnGuardarPlantilla.Click
        'MostrarProcesando(True)

        Try
            Dim nombrePlantilla As String = txtNombrePlantilla.Text
            Dim hojaSeleccionada As String = If(ddlHojas.SelectedValue, "").ToString()
            Dim filaEncabezado As Integer = Convert.ToInt32(txtFilaEncabezado.Text)

            If String.IsNullOrEmpty(nombrePlantilla) OrElse String.IsNullOrEmpty(hojaSeleccionada) Then
                lblMensaje.Text = "Por favor, ingresa el nombre de la plantilla y selecciona una hoja válida."
                Exit Sub
            End If

            Dim hojaPosicion As Integer = If(hojaPosiciones.ContainsKey(hojaSeleccionada), hojaPosiciones(hojaSeleccionada), 1)

            ' Validar ruta del archivo desde Session
            If Session("archivoExcel") Is Nothing Then
                lblMensaje.Text = "No se encontró la ruta del archivo. Vuelve a cargar el archivo."
                Exit Sub
            End If

            Dim rutaArchivo As String = Session("archivoExcel").ToString()

            ' Insertar plantilla
            Dim dto As New Plantilla With {
                    .Nombre = nombrePlantilla,
                    .HojaNombre = hojaSeleccionada,
                    .HojaPosicion = hojaPosicion,
                    .RenglonInicialDatos = filaEncabezado + 1
                    }

            ' Leer encabezados del Excel
            Dim encabezados As List(Of String) = ObtenerEncabezadosExactos(rutaArchivo, hojaSeleccionada, filaEncabezado)
            LoguearEncabezadosParaInsertar(encabezados, Path.GetFileName(rutaArchivo), hojaSeleccionada, filaEncabezado)

            Dim conceptos As List(Of Concepto)
            Dim nuevaPlantillaId As Integer

            Using conn As New SqlConnection(ConexionHelper.ObtenerCadena())
                conn.Open()
                Using tran As SqlTransaction = conn.BeginTransaction()
                    Try
                        ' Insertar plantilla
                        nuevaPlantillaId = PlantillaDAO.InsertarPlantilla(conn, tran, dto)
                        lblMensaje.Text = $"Plantilla guardada correctamente con ID: {nuevaPlantillaId}"

                        ' Insertar conceptos
                        conceptos = ConceptoDAO.InsertarConceptosDesdeEncabezados(conn, tran, encabezados)

                        ' Insertar columnas
                        ColumnaDAO.InsertarColumnas(conn, tran, nuevaPlantillaId, conceptos)

                        tran.Commit()
                    Catch ex As Exception
                        tran.Rollback()
                        lblMensaje.Text = "Error en la inserción: " & ex.Message
                        Exit Sub
                    End Try
                End Using
            End Using

            lblMensaje.Text &= $"<br/>Se insertaron correctamente {conceptos.Count} conceptos y columnas para la plantilla ID {nuevaPlantillaId}."

            ' Limpiar controles
            lblArchivoSeleccionado.Text = ""
            txtNombrePlantilla.Text = ""
            txtFilaEncabezado.Text = "1"
            ddlHojas.Items.Clear()
            gvDatos.DataSource = Nothing
            gvDatos.DataBind()

            btnGuardarPlantilla.Enabled = False
            btnPrevisualizar.Enabled = False

        Finally
            'MostrarProcesando(False)
        End Try
    End Sub

    Private Function ObtenerEncabezadosExactos(rutaArchivo As String, hojaNombre As String, filaEncabezado As Integer) As List(Of String)
        Dim encabezados As New List(Of String)()

        Using workbook As New XLWorkbook(rutaArchivo)
            Dim hoja = workbook.Worksheets.FirstOrDefault(Function(ws) String.Equals(ws.Name.Trim(), hojaNombre.Trim(), StringComparison.OrdinalIgnoreCase))

            If hoja Is Nothing Then
                Throw New Exception($"La hoja '{hojaNombre}' no existe en el archivo.")
            End If

            Dim fila = hoja.Row(filaEncabezado)
            Dim totalColumnas As Integer = If(hoja.Row(filaEncabezado).LastCellUsed()?.Address.ColumnNumber, 0)

            For col As Integer = 1 To totalColumnas
                Dim valor As String = fila.Cell(col).GetString() ' NO Trim
                Dim nombre As String = If(String.IsNullOrWhiteSpace(valor), $"SIN_NOMBRE_{col}", valor)

                encabezados.Add(nombre)
            Next
        End Using

        Return encabezados
    End Function

    Private Sub LoguearEncabezadosParaInsertar(encabezados As List(Of String), archivoExcel As String, hoja As String, filaEncabezado As Integer)
        Try
            Dim ruta As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log_encabezados_a_insertar.txt")

            Using sw As New StreamWriter(ruta, False)
                sw.WriteLine("LOG DE ENCABEZADOS A INSERTAR EN LA BASE DE DATOS")
                sw.WriteLine("Archivo Excel: " & archivoExcel)
                sw.WriteLine("Hoja: " & hoja)
                sw.WriteLine("Fila de encabezado: " & filaEncabezado)
                sw.WriteLine("--------------------------------------------------")

                Dim index As Integer = 1
                For Each encabezado In encabezados
                    ' El uso de comillas ayuda a visualizar espacios
                    sw.WriteLine($"{index:D2}: '{encabezado}'")
                    index += 1
                Next
            End Using

        Catch ex As Exception
            Logger.LogError("Error al guardar el log de encabezados: " & ex.Message)
        End Try
    End Sub

    Private Function CompararListasExactamente(lista1 As List(Of String), lista2 As List(Of String)) As Boolean
        If lista1.Count <> lista2.Count Then
            Return False
        End If

        For i As Integer = 0 To lista1.Count - 1
            If lista1(i) <> lista2(i) Then
                Return False
            End If
        Next

        Return True
    End Function

    Public NotInheritable Class ComparadorDeEstructura

        Private Sub New()
        End Sub

        Public Shared Function CalcularHash(encabezados As List(Of EncabezadoJson)) As String
            Dim partes = encabezados _
                .OrderBy(Function(e) e.Posicion) _
                .Select(Function(e) $"{e.Posicion}:{e.Nombre}") _
                .ToArray()

            Dim texto As String = String.Join("|", partes)
            Using sha As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256.Create()
                Dim bytes As Byte() = Encoding.UTF8.GetBytes(texto)
                Dim hash As Byte() = sha.ComputeHash(bytes)
                Return BitConverter.ToString(hash).Replace("-", "").ToLower()
            End Using
        End Function

        Public Shared Function CalcularHashDesdeTexto(estructuraPlano As String) As String
            Using sha As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256.Create()
                Dim bytes As Byte() = System.Text.Encoding.UTF8.GetBytes(estructuraPlano)
                Dim hash As Byte() = sha.ComputeHash(bytes)
                Return BitConverter.ToString(hash).Replace("-", "").ToLower()
            End Using
        End Function

    End Class


End Class
