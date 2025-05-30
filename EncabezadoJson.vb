Public Class EncabezadoJson
    Public Property NombreOriginal As String
    Public Property NombreFinal As String
    Public Property Posicion As Integer
    Public Property ColumnaBD As String
    Public Property Concepto_ID As Integer?
    Public Property ConceptoMaestro_ID As Integer?

    ' Propiedades solo para visualización con espacios representados como ·
    Public ReadOnly Property NombreOriginalVisual As String
        Get
            If NombreOriginal Is Nothing Then Return ""
            Return NombreOriginal.Replace(" ", "·")
        End Get
    End Property

    Public ReadOnly Property NombreFinalVisual As String
        Get
            If NombreFinal Is Nothing Then Return ""
            Return NombreFinal.Replace(" ", "·")
        End Get
    End Property

End Class
