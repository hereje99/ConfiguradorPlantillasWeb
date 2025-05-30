<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="ConfiguradorPlantillasWeb._Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Configurador de Plantillas Excel</title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="padding:20px; font-family:Arial; max-width:800px;">
            <h2>Configurador de Plantillas Excel</h2>

            <asp:Label ID="lblArchivo" runat="server" Text="Seleccionar archivo: " />
            <asp:FileUpload ID="fuArchivoExcel" runat="server" />
            <br /><br />

            <asp:Label ID="lblHoja" runat="server" Text="Seleccionar hoja:" />
            <asp:DropDownList ID="ddlHojas" runat="server" Width="300px" />
            <br /><br />

            <asp:Label ID="lblFila" runat="server" Text="Fila de encabezado:" />
            <asp:TextBox ID="txtFilaEncabezado" runat="server" Width="50px" Text="1" />
            <br /><br />

            <asp:Label ID="lblResultado" runat="server" ForeColor="Red" /><br /><br />
            <asp:Button ID="btnPrevisualizar" runat="server" Text="Previsualizar" OnClick="btnPrevisualizar_Click" />
            <br /><br />

            <asp:GridView ID="gvEncabezados" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered">
                <Columns>
                    <asp:BoundField DataField="Posicion" HeaderText="Posición" />
                    <asp:BoundField DataField="NombreOriginalVisual" HeaderText="Nombre original" />
                    <asp:BoundField DataField="NombreFinalVisual" HeaderText="Nombre final" />
                </Columns>
            </asp:GridView>


        </div>
    </form>
</body>
</html>
