Imports System.IO
Imports System.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports PlantUmlClassDiagramGenerator.Library

Namespace PlantUmlClassDiagramGeneratorForVBTest
    <TestClass>
    Public Class ClassDiagramGeneratorForVBTest
        <TestMethod>
        Sub TestSub()

        End Sub

        <TestMethod>
        Sub GenerateTest_All()
            Dim code = File.ReadAllText("testData\inputClasses.vb")
            Dim tree = VisualBasicSyntaxTree.ParseText(code)
            Dim root = tree.GetRoot()

            Dim output = New StringBuilder()
            Using writer = New StringWriter(output)
                Dim gen = New ClassDiagramGeneratorForVB(writer, "    ")
                gen.Generate(root)
            End Using


            Dim expected = ConvertNewLineCode(File.ReadAllText("uml\all.puml"), Environment.NewLine)
            Dim actual = output.ToString()
            Console.Write(actual)
            Assert.AreEqual(expected, actual)
        End Sub
        '        [TestMethod]
        'Public void GenerateTest_All()
        '{
        '    var code = File.ReadAllText("testData\\inputClasses.cs");
        '    var tree = CSharpSyntaxTree.ParseText(code);
        '    var root = tree.GetRoot();

        '    var output = New StringBuilder();
        '    Using (var writer = New StringWriter(output))
        '    {
        '        var gen = New ClassDiagramGenerator(writer, "    ");
        '        gen.Generate(root);
        '    }

        '    var expected = ConvertNewLineCode(File.ReadAllText(@"uml\all.puml"), Environment.NewLine);
        '    var actual = output.ToString();
        '    Console.Write(actual);
        '    Assert.AreEqual(expected, actual);
        '}

        <TestMethod>
        Public Sub GenerateTest_Public()
            Dim code = File.ReadAllText("testData\inputClasses.vb")
            Dim tree = VisualBasicSyntaxTree.ParseText(code)
            Dim root = tree.GetRoot()

            Dim output = New StringBuilder()
            Using writer = New StringWriter(output)
                Dim gen = New ClassDiagramGeneratorForVB(writer, "    ",
                        Accessibilities.Private Or Accessibilities.Internal Or
                        Accessibilities.Protected Or Accessibilities.ProtectedInternal)
                gen.Generate(root)
            End Using

            Dim expected = ConvertNewLineCode(File.ReadAllText("uml\public.puml"), Environment.NewLine)
            Dim actual = output.ToString()
            Console.Write(actual)
            Assert.AreEqual(expected, actual)
        End Sub
        '[TestMethod]
        'Public void GenerateTest_Public()
        '{
        '    var code = File.ReadAllText("testData\\inputClasses.cs");
        '    var tree = CSharpSyntaxTree.ParseText(code);
        '    var root = tree.GetRoot();

        '    var output = New StringBuilder();
        '    Using (var writer = New StringWriter(output))
        '    {
        '        var gen = New ClassDiagramGenerator(writer, "    ",
        '            Accessibilities.Private | Accessibilities.Internal
        '            | Accessibilities.Protected | Accessibilities.ProtectedInternal);
        '        gen.Generate(root);
        '    }

        '    var expected = ConvertNewLineCode(File.ReadAllText(@"uml\public.puml"), Environment.NewLine);
        '    var actual = output.ToString();
        '    Console.Write(actual);
        '    Assert.AreEqual(expected, actual);
        '}

        <TestMethod>
        Public Sub GenerateTest_WithoutPrivate()
            Dim code = File.ReadAllText("testData\inputClasses.vb")
            Dim tree = VisualBasicSyntaxTree.ParseText(code)
            Dim root = tree.GetRoot()

            Dim output = New StringBuilder()
            Using writer = New StringWriter(output)
                Dim gen = New ClassDiagramGeneratorForVB(writer, "    ", Accessibilities.Private)
                gen.Generate(root)
            End Using

            Dim expected = ConvertNewLineCode(File.ReadAllText("uml\withoutPrivate.puml"), Environment.NewLine)
            Dim actual = output.ToString()
            Console.Write(actual)
            Assert.AreEqual(expected, actual)
        End Sub
        '[TestMethod]
        'Public void GenerateTest_WithoutPrivate()
        '{
        '    var code = File.ReadAllText("testData\\inputClasses.cs");
        '    var tree = CSharpSyntaxTree.ParseText(code);
        '    var root = tree.GetRoot();

        '    var output = New StringBuilder();
        '    Using (var writer = New StringWriter(output))
        '    {
        '        var gen = New ClassDiagramGenerator(writer, "    ", Accessibilities.Private);
        '        gen.Generate(root);
        '    }

        '    var expected = ConvertNewLineCode(File.ReadAllText(@"uml\withoutPrivate.puml"), Environment.NewLine);
        '    var actual = output.ToString();
        '    Console.Write(actual);
        '    Assert.AreEqual(expected, actual);
        '}

        <TestMethod>
        Public Sub GenerateTest_GenericsTypes()
            Dim code = File.ReadAllText("testData\GenericsType.vb")
            Dim tree = VisualBasicSyntaxTree.ParseText(code)
            Dim root = tree.GetRoot()

            Dim output = New StringBuilder()
            Using writer = New StringWriter(output)
                Dim gen = New ClassDiagramGeneratorForVB(writer, "    ", Accessibilities.Private Or Accessibilities.Internal Or
                         Accessibilities.Protected Or Accessibilities.ProtectedInternal)
                gen.Generate(root)
            End Using

            Dim expected = ConvertNewLineCode(File.ReadAllText("uml\genericsType.puml"), Environment.NewLine)
            Dim actual = output.ToString()
            Console.Write(actual)
            Assert.AreEqual(expected, actual)
        End Sub
        '[TestMethod]
        'Public void GenerateTest_GenericsTypes()
        '{
        '    var code = File.ReadAllText("testData\\GenericsType.cs");
        '    var tree = CSharpSyntaxTree.ParseText(code);
        '    var root = tree.GetRoot();

        '    var output = New StringBuilder();
        '    Using (var writer = New StringWriter(output))
        '    {
        '        var gen = New ClassDiagramGenerator(writer, "    ", Accessibilities.Private | Accessibilities.Internal
        '            | Accessibilities.Protected | Accessibilities.ProtectedInternal);
        '        gen.Generate(root);
        '    }

        '    var expected = ConvertNewLineCode(File.ReadAllText(@"uml\genericsType.puml"), Environment.NewLine);
        '    var actual = output.ToString();
        '    Console.Write(actual);
        '    Assert.AreEqual(expected, actual);
        '}

        Private Function ConvertNewLineCode(text As String, newline As String) As String
            Dim reg = New System.Text.RegularExpressions.Regex("\r\n|\r|\n")
            Return reg.Replace(text, newline)
        End Function

    End Class
End Namespace

