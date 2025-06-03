<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DetallePlantilla.aspx.vb" Inherits="ConfiguradorPlantillasWeb.DetallePlantilla" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Detalle de Plantilla</title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="padding: 20px; font-family: Arial, sans-serif;">
            <div style="padding: 20px; font-family: Arial, sans-serif; max-width: 1100px; margin: auto;">
                <h2 style="margin-bottom: 25px;">Detalle de Plantilla</h2>

                <asp:Label ID="lblResumen" runat="server" Font-Bold="True" ForeColor="DarkBlue" />
                <br /><br />

                <asp:GridView ID="gvColumnas" runat="server" AutoGenerateColumns="False" Width="100%" GridLines="Both"
                    DataKeyNames="Posicion" OnSelectedIndexChanged="gvColumnas_SelectedIndexChanged">
                    <Columns>
                        <asp:BoundField DataField="Posicion" HeaderText="Posición" />
                        <asp:BoundField DataField="NombreColumna" HeaderText="Nombre Columna" />
                        <asp:BoundField DataField="ConceptoMaestro_ID" HeaderText="ID Concepto Maestro" />
                        <asp:BoundField DataField="NombreConceptoMaestro" HeaderText="Nombre Concepto Maestro" />
                        <asp:CommandField ShowSelectButton="True" SelectText="Seleccionar" />
                    </Columns>
                </asp:GridView>

                <br />

                <asp:Label ID="lblSeleccion" runat="server" Font-Italic="True" ForeColor="Gray" />
                <br /><br />

                <asp:Label ID="lblConcepto" runat="server" Text="Asignar concepto maestro:" />
                <asp:DropDownList ID="ddlConceptosMaestros" runat="server" Width="300px" />
                <asp:Button ID="btnAsignar" runat="server" Text="Asignar" OnClick="btnAsignar_Click" Style="margin-left: 10px;" />
                <asp:Button ID="btnQuitar" runat="server" Text="Quitar concepto" OnClick="btnQuitar_Click" Style="margin-left: 10px;" />

                <br /><br />

                <asp:Label ID="lblMensaje" runat="server" ForeColor="Red" />

                <br />
                <asp:HyperLink ID="lnkRegresar" runat="server" NavigateUrl="ListadoPlantillas.aspx" Font-Bold="True" ForeColor="DarkGreen">
                    ← Regresar al listado
                </asp:HyperLink>
            </div>
        </div>
    </form>
</body>
</html>
