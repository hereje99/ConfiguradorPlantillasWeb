Namespace Models
    Public Class Columna
        Public Property TipoPlantilla_ID As Integer
        Public Property Plantilla_ID As Integer
        Public Property Concepto_ID As Integer

        Public Property TipoDato_ID As Integer? ' Nullable: el campo permite NULL
        Public Property Nombre As String
        Public Property Posicion As Integer

        Public Property EsObligatorio As Boolean
        Public Property Activo As Boolean
        Public Property FechaRegistro As DateTime?
        Public Property UsuarioRegistro As String
        Public Property ConceptoMaestro_ID As Integer?
    End Class
End Namespace
