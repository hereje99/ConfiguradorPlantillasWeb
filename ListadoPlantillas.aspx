<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ListadoPlantillas.aspx.vb" Inherits="ConfiguradorPlantillasWeb.ListadoPlantillas" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Listado de Plantillas</title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="padding: 20px; font-family: Arial, sans-serif;">
            <div style="padding: 20px; font-family: Arial, sans-serif; max-width: 1100px; margin: auto;">
                <h2 style="margin-bottom: 25px;">Plantillas configuradas</h2>

                <asp:GridView ID="gvPlantillas" runat="server" AutoGenerateColumns="False" Width="100%"
                    DataKeyNames="Plantilla_ID" GridLines="Both"
                    OnSelectedIndexChanged="gvPlantillas_SelectedIndexChanged"
                    OnRowCommand="gvPlantillas_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="Plantilla_ID" HeaderText="Plantilla ID" />
                        <asp:BoundField DataField="Nombre" HeaderText="Nombre Plantilla" />
                        <asp:BoundField DataField="HojaNombre" HeaderText="Nombre Hoja" />
                        <asp:BoundField DataField="HojaPosicion" HeaderText="Posición Hoja" />
                        <asp:BoundField DataField="RenglonInicialDatos" HeaderText="Fila inicial Datos" />
                        <asp:BoundField DataField="FechaRegistro" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:CommandField ShowSelectButton="True" SelectText="Ver detalle" />
                        <asp:TemplateField HeaderText="Acción">
                            <ItemTemplate>
                                <asp:Button ID="btnEliminar" runat="server" Text="Eliminar"
                                    CommandName="Eliminar" CommandArgument='<%# Container.DataItemIndex %>'
                                    OnClientClick="return confirmarEliminacion();" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <asp:Label ID="lblMensaje" runat="server" ForeColor="Red" />

                <br />
                <asp:HyperLink ID="lnkRegresar" runat="server" NavigateUrl="ConfiguradorPlantillas.aspx" Font-Bold="True" ForeColor="DarkGreen">
                    ← Regresar al configurador de plantillas
                </asp:HyperLink>
            </div>
        </div>

        <script type="text/javascript">
            function confirmarEliminacion() {
                return confirm("¿Estás seguro de que deseas eliminar esta plantilla?");
            }
        </script>
    </form>
</body>
</html>
