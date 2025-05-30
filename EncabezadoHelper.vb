Public Class EncabezadoHelper
    Public Shared Sub GenerarNombresVisuales(encabezados As List(Of EncabezadoJson))
        Dim nombresExistentes As New Dictionary(Of String, Integer)

        For Each encabezado In encabezados
            Dim nombre As String = encabezado.NombreOriginal

            If String.IsNullOrWhiteSpace(nombre) Then
                nombre = "SIN_NOMBRE"
            End If

            If Not nombresExistentes.ContainsKey(nombre) Then
                nombresExistentes(nombre) = 1
                encabezado.NombreFinal = nombre
            Else
                Dim contador As Integer = nombresExistentes(nombre)
                nombresExistentes(nombre) += 1
                encabezado.NombreFinal = nombre & "_" & contador.ToString()
            End If
        Next
    End Sub
End Class
