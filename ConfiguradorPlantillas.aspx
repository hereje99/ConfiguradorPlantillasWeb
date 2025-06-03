<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ConfiguradorPlantillas.aspx.vb" Inherits="ConfiguradorPlantillasWeb.ConfiguradorPlantillas" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Configurador de Plantillas Excel</title>
</head>
<body>
    <form id="form1" runat="server">

        <!-- Overlay que bloquea la página durante la acción -->
        <div id="overlay" style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(255, 255, 255, 0.8); z-index: 9999; display: none; text-align: center; padding-top: 200px; font-size: 20px; font-weight: bold; color: #333;">
            Guardando, por favor espera...
        </div>

        <div style="padding: 20px; font-family: Arial, sans-serif;">

            <div style="padding: 20px; font-family: Arial, sans-serif; max-width: 1100px; margin: auto;">
                <h2 style="margin-bottom: 25px;">Configurador de Plantillas Excel</h2>

                <table style="width: 100%; margin-bottom: 20px;">
                    <tr>
                        <td style="width: 120px;">
                            <asp:Label ID="lblArchivo" runat="server" Text="Archivo:" /></td>
                        <td>
                            <asp:FileUpload ID="fuArchivo" runat="server" />
                            <asp:Button ID="btnSeleccionarArchivo" runat="server" Text="Cargar archivo Excel" OnClick="btnSeleccionarArchivo_Click" Style="margin-left: 10px;" />
                            <br />
                            <asp:Label ID="lblArchivoSeleccionado" runat="server" ForeColor="DarkGreen" Font-Bold="True" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblNombre" runat="server" Text="Nombre de la plantilla:" /></td>
                        <td>
                            <asp:TextBox ID="txtNombrePlantilla" runat="server" Width="400px" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblHoja" runat="server" Text="Hoja:" /></td>
                        <td>
                            <asp:DropDownList ID="ddlHojas" runat="server" Width="250px" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblFila" runat="server" Text="Fila de encabezado:" /></td>
                        <td>
                            <asp:TextBox ID="txtFilaEncabezado" runat="server" Width="50px" Text="1" TextMode="Number" min="1" /></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td style="padding-top: 10px;">
                            <asp:Button ID="btnPrevisualizar" runat="server" Text="Previsualizar" />
                            <asp:Button ID="btnGuardarPlantilla" runat="server" Text="Guardar plantilla" Enabled="false"
                                Style="margin-left: 10px;" OnClientClick="mostrarOverlay();" />
                            <asp:Button ID="btnVerPlantillas" runat="server" Text="Ver plantillas existentes"
                                PostBackUrl="~/ListadoPlantillas.aspx"
                                Style="margin-left: 10px;" />
                        </td>
                    </tr>

                </table>

                <asp:Label ID="lblMensaje" runat="server" ForeColor="Red" Style="display: block; margin-bottom: 15px;" />

                <asp:GridView ID="gvDatos" runat="server" AutoGenerateColumns="True"
                    GridLines="Both"
                    HeaderStyle-BackColor="#f2f2f2"
                    HeaderStyle-Font-Bold="True"
                    BorderColor="#cccccc"
                    BorderStyle="Solid"
                    BorderWidth="1px"
                    CellPadding="5"
                    Font-Size="Small"
                    Width="100%" />
            </div>
        </div>

        <!-- Script para mostrar el overlay -->
        <script type="text/javascript">
            function mostrarOverlay() {
                document.getElementById("overlay").style.display = "block";
            }
        </script>

    </form>
</body>
</html>
