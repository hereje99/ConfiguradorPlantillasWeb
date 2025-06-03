Imports System.Configuration
Imports System.Data.SqlClient
Imports ConfiguradorPlantillasWeb.ConfiguradorPlantillasExcel.Utils

Public Class ListadoPlantillas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            CargarPlantillas()
        End If
    End Sub

    Private Sub CargarPlantillas()
        Try
            Using conn As New SqlConnection(ConexionHelper.ObtenerCadena())
                conn.Open()
                Dim tabla As DataTable = PlantillaDAO.ListarPlantillas(conn)
                gvPlantillas.DataSource = tabla
                gvPlantillas.DataBind()
            End Using
        Catch ex As Exception
            lblMensaje.Text = $"Error al cargar plantillas: {ex.Message}"
        End Try
    End Sub

    Protected Sub gvPlantillas_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim plantillaId As String = gvPlantillas.SelectedDataKey("Plantilla_ID").ToString()
        Response.Redirect("DetallePlantilla.aspx?id=" & plantillaId)
    End Sub

    Protected Sub gvPlantillas_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If e.CommandName = "Eliminar" Then
            Dim index As Integer = Convert.ToInt32(e.CommandArgument)
            Dim plantillaId As Integer = Convert.ToInt32(gvPlantillas.DataKeys(index).Value)

            Try
                Using conn As New SqlConnection(ConexionHelper.ObtenerCadena())
                    conn.Open()
                    Using tran As SqlTransaction = conn.BeginTransaction()
                        Try
                            PlantillaDAO.EliminarPlantillaYColumnas(conn, tran, plantillaId)
                            tran.Commit()
                            lblMensaje.Text = $"Plantilla ID {plantillaId} eliminada correctamente."
                            CargarPlantillas()
                        Catch ex As Exception
                            tran.Rollback()
                            lblMensaje.Text = $"Error al eliminar plantilla: {ex.Message}"
                        End Try
                    End Using
                End Using
            Catch ex As Exception
                lblMensaje.Text = $"Error de conexión: {ex.Message}"
            End Try
        End If
    End Sub


End Class
