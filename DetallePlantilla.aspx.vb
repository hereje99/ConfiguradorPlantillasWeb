Imports System.Data.SqlClient
Imports ConfiguradorPlantillasWeb.ConfiguradorPlantillasExcel.Utils

Public Class DetallePlantilla
    Inherits System.Web.UI.Page

    Private Property PlantillaID As Integer
        Get
            Return If(ViewState("PlantillaID") IsNot Nothing, Convert.ToInt32(ViewState("PlantillaID")), 0)
        End Get
        Set(value As Integer)
            ViewState("PlantillaID") = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Not Integer.TryParse(Request.QueryString("id"), PlantillaID) Then
                lblMensaje.Text = "No se recibió un Plantilla_ID válido en la URL."
                Return
            End If

            lblResumen.Text = $"Mostrando columnas para Plantilla_ID: {PlantillaID}"
            CargarColumnas()
            CargarConceptosMaestros()
        End If
    End Sub

    Private Sub CargarColumnas()
        Try
            Dim dt As DataTable = ColumnaDAO.ObtenerColumnasPorPlantilla(PlantillaID)
            gvColumnas.DataSource = dt
            gvColumnas.DataBind()
        Catch ex As Exception
            lblMensaje.Text = "Error al cargar columnas: " & ex.Message
        End Try
    End Sub

    Private Sub CargarConceptosMaestros()
        ddlConceptosMaestros.Items.Clear()

        Dim tabla As DataTable = ColumnaDAO.ListarConceptosMaestros()

        For Each fila As DataRow In tabla.Rows
            ddlConceptosMaestros.Items.Add(New ListItem(fila("Nombre").ToString(), fila("ConceptoMaestro_ID").ToString()))
        Next
    End Sub

    Protected Sub gvColumnas_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim row As GridViewRow = gvColumnas.SelectedRow
        If row IsNot Nothing Then
            Dim nombre As String = row.Cells(1).Text
            Dim posicion As String = row.Cells(0).Text
            lblSeleccion.Text = $"Columna seleccionada: {nombre} (posición {posicion})"
        End If
    End Sub

    Protected Sub btnAsignar_Click(sender As Object, e As EventArgs)
        If gvColumnas.SelectedIndex < 0 Then
            lblMensaje.Text = "Selecciona una columna del listado primero."
            Return
        End If

        Dim posicion As Integer = Convert.ToInt32(gvColumnas.SelectedDataKey.Value)
        Dim conceptoId As Integer = Convert.ToInt32(ddlConceptosMaestros.SelectedValue)

        Dim errorMsg As String = ColumnaDAO.AsignarConceptoMaestro(PlantillaID, conceptoId, posicion)

        If errorMsg IsNot Nothing Then
            lblMensaje.Text = errorMsg
            Return
        End If

        lblMensaje.Text = "Concepto asignado correctamente."
        CargarColumnas()
    End Sub

    Protected Sub btnQuitar_Click(sender As Object, e As EventArgs)
        If gvColumnas.SelectedIndex < 0 Then
            lblMensaje.Text = "Selecciona una columna para quitarle el concepto maestro."
            Return
        End If

        Dim posicion As Integer = Convert.ToInt32(gvColumnas.SelectedDataKey.Value)

        Dim exito As Boolean = ColumnaDAO.QuitarConceptoMaestro(PlantillaID, posicion)

        If exito Then
            lblMensaje.Text = "Concepto eliminado de la columna."
        Else
            lblMensaje.Text = "Ocurrió un error al quitar el concepto."
        End If

        CargarColumnas()
    End Sub

End Class
