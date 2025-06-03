Imports System.Collections.Generic

Namespace ConfiguradorPlantillasExcel.Utils
    Public Module EncabezadoHelper

        ''' <summary>
        ''' Genera una lista de nombres únicos para mostrar en un DataGridView, a partir de los nombres reales del Excel.
        ''' Si un nombre se repite, se le añade un sufijo incremental (_2, _3, etc.).
        ''' </summary>
        Public Function GenerarNombresVisuales(encabezadosOriginales As List(Of String)) As List(Of String)
            Dim nombresUnicos As New List(Of String)()
            Dim contador As New Dictionary(Of String, Integer)()

            For Each nombre In encabezadosOriginales
                Dim limpio As String = nombre.Trim()

                If Not contador.ContainsKey(limpio) Then
                    contador(limpio) = 1
                    nombresUnicos.Add(limpio)
                Else
                    contador(limpio) += 1
                    nombresUnicos.Add($"{limpio}_{contador(limpio)}")
                End If
            Next

            Return nombresUnicos
        End Function

    End Module
End Namespace
