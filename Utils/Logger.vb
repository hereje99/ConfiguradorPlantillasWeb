Imports System
Imports System.IO

Namespace ConfiguradorPlantillasExcel.Utils
    Public Module Logger

        Private ReadOnly logErroresPath As String = "log_errores.txt"
        Private ReadOnly logProcesoPath As String = "log_proceso.txt"

        Public Sub LogError(mensaje As String)
            Try
                File.AppendAllText(logErroresPath, $"[{Date.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {mensaje}{Environment.NewLine}")
            Catch
                ' Evita errores si falla el log
            End Try
        End Sub

        Public Sub LogProceso(mensaje As String)
            Try
                File.AppendAllText(logProcesoPath, $"[{Date.Now:yyyy-MM-dd HH:mm:ss}] {mensaje}{Environment.NewLine}")
            Catch
                ' Evita errores si falla el log
            End Try
        End Sub

    End Module
End Namespace
