Imports System.Data.SqlClient
Imports System.Configuration
Imports System.IO
Imports ConfiguradorPlantillasWeb.ConfiguradorPlantillasExcel.Utils
Imports ConfiguradorPlantillasWeb.Models

Namespace ConfiguradorPlantillasExcel
    Public Class ValidarPlantilla

        Public Shared Function BuscarPlantillaCoincidente(hoja As String, hojaPosicion As Integer,
                                                  filaInicioDatos As Integer, encabezados As List(Of EncabezadoJson)) As PlantillaCoincidencia
            Try
                File.AppendAllText("log_debug.txt", $"→ Filtro: Hoja='{hoja}', Pos={hojaPosicion}, Fila={filaInicioDatos}{Environment.NewLine}")

                Dim posiblesCoincidencias = ObtenerPosiblesCoincidencias(hoja, hojaPosicion, filaInicioDatos)

                ' Filtrar por cantidad de columnas coincidente
                Dim candidatos = posiblesCoincidencias.
                         Where(Function(p) p.Columnas IsNot Nothing AndAlso p.Columnas.Count = encabezados.Count).
                         ToList()

                Dim logComparacion As New System.Text.StringBuilder()
                Dim seEncontroCoincidencia As Boolean = False

                For Each plantilla In candidatos
                    Dim logEstructura As String = Nothing
                    If CoincideEstructura(plantilla.Columnas, encabezados, logEstructura) Then
                        Console.WriteLine(logEstructura)
                        File.WriteAllText("log_comparacion.txt", logEstructura)
                        seEncontroCoincidencia = True
                        Return plantilla
                    Else
                        logComparacion.AppendLine($"→ Comparación con Plantilla_ID: {plantilla.Plantilla_ID}")
                        logComparacion.AppendLine(logEstructura)
                        logComparacion.AppendLine(New String("-"c, 60))
                    End If
                Next

                ' Guardar log de comparaciones si no hubo coincidencias exactas
                If Not seEncontroCoincidencia Then
                    File.WriteAllText("log_comparacion.txt", logComparacion.ToString())

                    ' También registrar en histórico
                    Dim logHistorico As String =
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Validación sin coincidencia exacta:{Environment.NewLine}" &
                $"Hoja: {hoja}, HojaPosición: {hojaPosicion}, RenglónInicioDatos: {filaInicioDatos}{Environment.NewLine}" &
                $"Total columnas en archivo: {encabezados.Count}{Environment.NewLine}" &
                $"Comparaciones realizadas: {candidatos.Count}{Environment.NewLine}" &
                $"Resultado: NO SE ENCONTRÓ PLANTILLA COMPATIBLE{Environment.NewLine}{Environment.NewLine}"

                    File.AppendAllText("log_historico.txt", logHistorico)
                End If

                Return Nothing
            Catch ex As Exception
                Console.WriteLine("Error en BuscarPlantillaCoincidente: " & ex.Message)
                Return Nothing
            End Try
        End Function

        Public Shared Function CoincideEstructura(listaA As List(Of EncabezadoJson),
                                          listaB As List(Of EncabezadoJson),
                                          ByRef logDetalle As String) As Boolean
            Dim log As New System.Text.StringBuilder()
            log.AppendLine("----- Comparación de estructuras (por nombre, ignorando vacíos) -----")

            Dim listaAOrdenada = listaA.OrderBy(Function(x) x.Posicion).ToList()
            Dim listaBOrdenada = listaB.OrderBy(Function(x) x.Posicion).ToList()

            log.AppendLine($"Total A: {listaAOrdenada.Count}, Total B: {listaBOrdenada.Count}")
            log.AppendLine()

            Dim iguales As Boolean = (listaAOrdenada.Count = listaBOrdenada.Count)

            For i As Integer = 0 To Math.Max(listaAOrdenada.Count, listaBOrdenada.Count) - 1
                Dim nombreA As String = If(i < listaAOrdenada.Count, listaAOrdenada(i).Nombre, "(vacío)")
                Dim nombreB As String = If(i < listaBOrdenada.Count, listaBOrdenada(i).Nombre, "(vacío)")
                Dim posA As Integer = If(i < listaAOrdenada.Count, listaAOrdenada(i).Posicion, i + 1)
                Dim posB As Integer = If(i < listaBOrdenada.Count, listaBOrdenada(i).Posicion, i + 1)

                Dim coincide As Boolean = (nombreA = nombreB)

                If Not coincide Then
                    iguales = False
                End If

                Dim marcador As String = If(coincide, "✓", "✗")

                log.AppendLine($"{marcador}  Col: {posA,-2} – '{nombreA,-20}' | Col: {posB,-2} – '{nombreB}'")
            Next

            log.AppendLine()
            log.AppendLine($"¿Coinciden exactamente?: {(If(iguales, "SÍ", "NO"))}")
            logDetalle = log.ToString()

            System.Diagnostics.Debug.WriteLine(logDetalle)
            Return iguales
        End Function

        Private Shared Function ObtenerTodasLasPlantillas(hojaNombre As String, hojaPosicion As Integer, renglonInicialDatos As Integer) As List(Of PlantillaCoincidencia)
            Return PlantillaDAO.ObtenerPlantillasCoincidentes(hojaNombre, hojaPosicion, renglonInicialDatos)
        End Function


        Private Shared Function ObtenerPosiblesCoincidencias(hoja As String, hojaPosicion As Integer, filaInicioDatos As Integer) As List(Of PlantillaCoincidencia)
            Return ObtenerTodasLasPlantillas(hoja, hojaPosicion, filaInicioDatos)
        End Function

    End Class
End Namespace
