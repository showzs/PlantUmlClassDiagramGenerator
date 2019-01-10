Public Class GenericsType
    Public ReadOnly Property Value As Object
End Class

Public Class GenericsType(Of T)
    Public ReadOnly Property Value As T
End Class

Public Class GenericsType(Of T1, T2)
    Public ReadOnly Property Value1 As T1
    Public ReadOnly Property Value2 As T2
End Class

Public Class SubClass
    Inherits GenericsType(Of String, Integer)

    Public Overloads ReadOnly Property Value1 As String
    Public Overloads ReadOnly Property Value2 As Integer
End Class

Public Class SubClass(Of T)
    Inherits GenericsType(Of GenericsType(Of Integer), T)

    Public Overloads ReadOnly Property Value1 As GenericsType(Of Integer)
    Public Overloads ReadOnly Property Value2 As T
End Class
