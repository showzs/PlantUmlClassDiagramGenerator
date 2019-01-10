Imports System.ComponentModel

Public Class InputClasses

End Class

Public Class ClassA
    Private ReadOnly intField As Integer = 100
    Private Shared strField As String
    Protected X As Double = 0, Y As Double = 1, Z As Double = 2
    Private list As IList(Of Integer) = New List(Of Integer)()

    Private _propA As Integer
    Protected Property PropA As Integer
        Get
            Return _propA
        End Get
        Private Set(value As Integer)
            _propA = value
        End Set
    End Property
    Private _propB As String
    Protected Friend Property PropB As String
        Get
            Return _propB
        End Get
        Protected Set(value As String)
            _propB = value
        End Set
    End Property
    Friend ReadOnly Property PropC As Double = 3.141592
    Public PropD = Function() "expression-bodied property"

    Public Sub New()

    End Sub
    Shared Sub New()
        strField = "static field"
    End Sub

    Protected Overridable Sub VirtualMethod()

    End Sub
    Public Overrides Function ToString() As String
        Return intField.ToString()
    End Function

    Public Shared Function StaticMethod() As String
        Return strField
    End Function
    Public ExpressonBodiedMethod As Action(Of Integer) = Sub(px As Integer)
                                                             Dim a = px * px
                                                         End Sub
End Class
'Public Class ClassA
'    {
'        Private ReadOnly int intField = 100;
'        Private Static String strField;
'        Protected Double X = 0, Y = 1, Z = 2;
'        Private IList<int> list = New List<int>();

'        Protected int PropA { Get; Private Set; }
'        Protected internal String PropB { Get; Protected Set; }
'        internal double PropC { get; } = 3.141592;
'        Public String PropD => "expression-bodied property";

'        Public ClassA() { }
'        Static ClassA() { strField = "static field"; }

'        Protected virtual void VirtualMethod() { }
'        Public override String ToString()
'        {
'            Return intField.ToString();
'        }

'        Public Static String StaticMethod() { Return strField; }
'        Public void ExpressonBodiedMethod(int x) => x * x;
'    }

Friend MustInherit Class ClassB
    Private field_1 As Integer
    Public MustOverride Property PropA As Integer

    Protected Overridable Function VirtualMethod() As String
        Return "virtual"
    End Function
    Public MustOverride Function AbstractMethod(arg1 As Integer, arg2 As Double) As String
End Class

'internal abstract Class ClassB
'{
'    Private int field_1;
'    Public abstract int PropA { Get; Protected Set; }

'    Protected virtual String VirtualMethod() { Return "virtual"; }
'    Public abstract String AbstractMethod(int arg1, Double arg2);
'}

Friend NotInheritable Class ClassC
    Inherits ClassB
    Implements INotifyPropertyChanged

    Private Shared ReadOnly readonlyField As String = "ReadOnly"

    Private _propA As Integer
    Public Overrides Property PropA As Integer
        Get
            Return _propA
        End Get
        Set(value As Integer)
            _propA = value
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Overrides Function AbstractMethod(arg1 As Integer, arg2 As Double) As String
        Return readonlyField
    End Function

    Protected Overrides Function VirtualMethod() As String
        Return MyBase.VirtualMethod()
    End Function
End Class
'internal sealed Class ClassC : ClassB, INotifyPropertyChanged
'{
'    Private Static ReadOnly String readonlyField = "ReadOnly";
'    Public override int PropA { Get; Protected Set; } = 100;

'    Public Event PropertyChangedEventHandler() PropertyChanged;

'    Private void RaisePropertyChanged(String propertyName) =>
'        PropertyChanged?.Invoke(this, New PropertyChangedEventArgs(propertyName));


'    Public override String AbstractMethod(int arg1, Double arg2)
'    {
'        Return readonlyField;
'    }

'    Protected override String VirtualMethod()
'    {
'        Return base.VirtualMethod();
'    }
'}

Public Structure Vector
    Public ReadOnly Property X As Double
    Public ReadOnly Property Y As Double
    Public ReadOnly Property Z As Double

    Public Sub New(ax As Double, ay As Double, az As Double)
        X = ax
        Y = ay
        Z = az
    End Sub

    Public Sub New(source As Vector)
        Me.New(source.X, source.Y, source.Z)
    End Sub

    Public Shared Operator +(a As Vector, b As Vector)
        Return New Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z)
    End Operator

    Public Shared Operator -(a As Vector, b As Vector)
        Return New Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z)
    End Operator
End Structure
'Public struct Vector
'{
'    Public Double X { Get; }
'    Public Double Y { Get; }
'    Public Double Z { Get; }

'    Public Vector(Double x, double y, double z)
'    {
'        X = x;
'        Y = y;
'        Z = z;
'    }

'    Public Vector(Vector source)
'         this(source.X, source.Y, source.Z)
'    { }

'    Public Static Vector Operator +(Vector a, Vector b)
'    {
'        Return New Vector(
'            a.X + b.X,
'            a.Y + b.Y,
'            a.Z + b.Z);
'    }

'    Public Static Vector Operator -(Vector a, Vector b)
'    {
'        Return New Vector(
'            a.X - b.X,
'            a.Y - b.Y,
'            a.Z - b.Z);
'    }
'}

<Flags>
Public Enum EnumA
    AA = &H1
    BB = &H2
    CC = &H4
    DD = &H8
    EE = &H10
End Enum
'[Flags]
'Enum EnumA
'{
'    AA = 0x0001,
'    BB = 0x0002,
'    CC = 0x0004,
'    DD = 0x0008,
'    EE = 0x0010
'}

Public Class NestedClass
    Public ReadOnly Property A As Integer
    Public ReadOnly Property B As InnerClass
    Public Class InnerClass
        Public ReadOnly Property X As String = "xx"
        Public Sub MethodX()

        End Sub
        Public Structure InnerStruct
            Public ReadOnly Property A As Integer
            Public Sub New(aa As Integer)
                A = aa
            End Sub
        End Structure
    End Class
End Class
'Class NestedClass
'{
'    Public int A { Get; }
'    Public InnerClass B { Get; }
'    Public Class InnerClass
'    {
'        Public String X { Get; } = "xx";
'        Public void MethodX() { }

'        Public struct InnerStruct
'        {
'            Public int A { Get; }
'            Public InnerStruct(int a)
'            {
'                A = 0;
'            }
'        }
'    }
'}
