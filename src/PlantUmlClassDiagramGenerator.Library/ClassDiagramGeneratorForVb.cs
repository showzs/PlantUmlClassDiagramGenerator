using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace PlantUmlClassDiagramGenerator.Library
{
    public class ClassDiagramGeneratorForVB : VisualBasicSyntaxWalker
    {
        private IList<SyntaxNode> _innerTypeDeclarationNodes;
        private Accessibilities _ignoreMemberAccessibilities;
        private InheritanceRelationshipCollectionForVB _inheritanceRelationsips
            = new InheritanceRelationshipCollectionForVB();
        private TextWriter writer;
        private string indent;
        private int nestingDepth = 0;

        public ClassDiagramGeneratorForVB(TextWriter writer, string indent, Accessibilities ignoreMemberAccessibilities = Accessibilities.None)
        {
            this.writer = writer;
            this.indent = indent;
            _innerTypeDeclarationNodes = new List<SyntaxNode>();
            _ignoreMemberAccessibilities = ignoreMemberAccessibilities;
        }

        public void Generate(SyntaxNode root)
        {
            WriteLine("@startuml");
            GenerateInternal(root);
            WriteLine("@enduml");
        }

        public void GenerateInternal(SyntaxNode root)
        {
            Visit(root);
            GenerateInnerTypeDeclarations();
            foreach (var inheritance in _inheritanceRelationsips)
            {
                WriteLine(inheritance.ToString());
            }
        }

        //public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        //{
        //    VisitTypeDeclaration(node, () => base.VisitInterfaceDeclaration(node));
        //}
        public override void VisitInterfaceBlock(InterfaceBlockSyntax node)
        {
            VisitTypeBlock(node, () => base.VisitInterfaceBlock(node));
        }

        //public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        //{
        //    VisitTypeDeclaration(node, () => base.VisitClassDeclaration(node));
        //}
        public override void VisitClassBlock(ClassBlockSyntax node)
        {
            VisitTypeBlock(node, () => base.VisitClassBlock(node));
        }
        public override void VisitModuleBlock(ModuleBlockSyntax node)
        {
            VisitTypeBlock(node, () => base.VisitModuleBlock(node));
        }

        //public override void VisitStructDeclaration(StructDeclarationSyntax node)
        //{
        //    if (SkipInnerTypeDeclaration(node)) { return; }

        //    _inheritanceRelationsips.AddFrom(node);

        //    var typeName = TypeNameText.From(node);
        //    var name = typeName.Identifier;
        //    var typeParam = typeName.TypeArguments;

        //    WriteLine($"class {name}{typeParam} <<struct>> {{");

        //    nestingDepth++;
        //    base.VisitStructDeclaration(node);
        //    nestingDepth--;

        //    WriteLine("}");
        //}
        public override void VisitStructureBlock(StructureBlockSyntax node)
        {
            if (SkipInnerTypeDeclaration(node)) { return; }

            _inheritanceRelationsips.AddFrom(node);

            var typeName = TypeNameTextForVB.From(node);
            var name = typeName.Identifier;
            var typeParam = typeName.TypeArguments;

            WriteLine($"class {name}{typeParam} <<struct>> {{");

            nestingDepth++;
            base.VisitStructureBlock(node);
            nestingDepth--;

            WriteLine("}");
        }
        
        //public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        //{
        //    if (SkipInnerTypeDeclaration(node)) { return; }

        //    WriteLine($"{node.EnumKeyword} {node.Identifier} {{");

        //    nestingDepth++;
        //    base.VisitEnumDeclaration(node);
        //    nestingDepth--;

        //    WriteLine("}");
        //}
        public override void VisitEnumBlock(EnumBlockSyntax node)
        {
            if (SkipInnerTypeDeclaration(node)) { return; }

            WriteLine($"{node.EnumStatement.EnumKeyword} {node.EnumStatement.Identifier} {{");

            nestingDepth++;
            base.VisitEnumBlock(node);
            nestingDepth--;

            WriteLine("}");
        }

        //public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        //{
        //    if (IsIgnoreMember(node.Modifiers)) { return; }

        //    var modifiers = GetMemberModifiersText(node.Modifiers);
        //    var name = node.Identifier.ToString();
        //    var args = node.ParameterList.Parameters.Select(p => $"{p.Identifier}:{p.Type}");

        //    WriteLine($"{modifiers}{name}({string.Join(", ", args)})");
        //}
        public override void VisitConstructorBlock(ConstructorBlockSyntax node)
        {
            if (IsIgnoreMember(node.SubNewStatement.Modifiers)) { return; }

            var modifiers = GetMemberModifiersText(node.SubNewStatement.Modifiers);
            var name = node.SubNewStatement.NewKeyword.ToString();
            var args = node.SubNewStatement.ParameterList.Parameters.Select(p => $"{p.Identifier}:{p.AsClause?.Type.ToString() ?? ""}");

            WriteLine($"{modifiers}{name}({string.Join(", ", args)})");
        }

        //public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        //{
        //    if (IsIgnoreMember(node.Modifiers)) { return; }

        //    var modifiers = GetMemberModifiersText(node.Modifiers);
        //    var typeName = node.Declaration.Type.ToString();
        //    var variables = node.Declaration.Variables;
        //    foreach (var field in variables)
        //    {
        //        var useLiteralInit = field.Initializer?.Value?.Kind().ToString().EndsWith("LiteralExpression") ?? false;
        //        var initValue = useLiteralInit ? (" = " + field.Initializer.Value.ToString()) : "";

        //        WriteLine($"{modifiers}{field.Identifier} : {typeName}{initValue}");
        //    }
        //}
        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            if (IsIgnoreMember(node.Modifiers)) { return; }

            var modifiers = GetMemberModifiersText(node.Modifiers);
            //var typeName = node.Declarators.Type.ToString();
            //var variables = node.Declaration.Variables;
            foreach (var field in node.Declarators)
            {
                var typeName = field.AsClause?.Type().ToString();
                var useLiteralInit = field.Initializer?.Value?.Kind().ToString().EndsWith("LiteralExpression") ?? false;
                var initValue = useLiteralInit ? (" = " + field.Initializer.Value.ToString()) : "";

                foreach (var name in field.Names)
                {
                    WriteLine($"{modifiers}{name.Identifier} : {typeName}{initValue}");
                }
            }
        }

        //public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        //{
        //    if (IsIgnoreMember(node.Modifiers)) { return; }

        //    var modifiers = GetMemberModifiersText(node.Modifiers);
        //    var name = node.Identifier.ToString();
        //    var typeName = node.Type.ToString();

        //    //Property does not have an accessor is an expression-bodied property. (get only)
        //    var accessorStr = "<<get>>";
        //    if (node.AccessorList != null)
        //    {
        //        var accessor = node.AccessorList.Accessors
        //            .Where(x => !x.Modifiers.Select(y => y.Kind()).Contains(SyntaxKind.PrivateKeyword))
        //            .Select(x => $"<<{(x.Modifiers.ToString() == "" ? "" : (x.Modifiers.ToString() + " "))}{x.Keyword}>>");
        //        accessorStr = string.Join(" ", accessor);
        //    }
        //    var useLiteralInit = node.Initializer?.Value?.Kind().ToString().EndsWith("LiteralExpression") ?? false;
        //    var initValue = useLiteralInit ? (" = " + node.Initializer.Value.ToString()) : "";

        //    WriteLine($"{modifiers}{name} : {typeName} {accessorStr}{initValue}");
        //}
        public override void VisitPropertyBlock(PropertyBlockSyntax node)
        {
            if (IsIgnoreMember(node.PropertyStatement.Modifiers)) { return; }

            var modifiers = GetMemberModifiersText(node.PropertyStatement.Modifiers);
            var name = node.PropertyStatement.Identifier.ToString();
            var typeName = node.PropertyStatement.AsClause?.Type().ToString() ?? "";

            //Property does not have an accessor is an expression-bodied property. (get only)
            var accessorStr = "<<get>>";
            if (node.Accessors != null)
            {
                var accessor = node.Accessors
                    .Where(x => !x.BlockStatement.Modifiers.Select(y => y.Kind()).Contains(SyntaxKind.PrivateKeyword))
                    .Select(x => $"<<{(x.BlockStatement.Modifiers.ToString() == "" ? "" : (x.BlockStatement.Modifiers.ToString() + " "))}{x.BlockStatement.DeclarationKeyword}>>");
                accessorStr = string.Join(" ", accessor);
            }
            var useLiteralInit = node.PropertyStatement.Initializer?.Value?.Kind().ToString().EndsWith("LiteralExpression") ?? false;
            var initValue = useLiteralInit ? (" = " + node.PropertyStatement.Initializer.Value.ToString()) : "";

            WriteLine($"{modifiers}{name} : {typeName} {accessorStr}{initValue}");
            //base.VisitPropertyBlock(node);
        }
        public override void VisitPropertyStatement(PropertyStatementSyntax node)
        {
            if (IsIgnoreMember(node.Modifiers)) { return; }

            var modifiers = GetMemberModifiersText(node.Modifiers);
            var name = node.Identifier.ToString();
            var typeName = node.AsClause?.Type().ToString() ?? "";

            var useLiteralInit = node.Initializer?.Value?.Kind().ToString().EndsWith("LiteralExpression") ?? false;
            var initValue = useLiteralInit ? (" = " + node.Initializer.Value.ToString()) : "";

            WriteLine($"{modifiers}{name} : {typeName} {initValue}");
            //base.VisitPropertyStatement(node);
        }

        //public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        //{
        //    if (IsIgnoreMember(node.Modifiers)) { return; }

        //    var modifiers = GetMemberModifiersText(node.Modifiers);
        //    var name = node.Identifier.ToString();
        //    var returnType = node.ReturnType.ToString();
        //    var args = node.ParameterList.Parameters.Select(p => $"{p.Identifier}:{p.Type}");

        //    WriteLine($"{modifiers}{name}({string.Join(", ", args)}) : {returnType}");
        //}
        public override void VisitMethodBlock(MethodBlockSyntax node)
        {
            var methodStatement = node.BlockStatement as MethodStatementSyntax;
            if (methodStatement == null) { return; }
            if (IsIgnoreMember(node.BlockStatement.Modifiers)) { return; }

            var modifiers = GetMemberModifiersText(methodStatement.Modifiers);
            var name = methodStatement.Identifier.ToString();
            var returnType = methodStatement.AsClause?.Type.ToString() ?? "void";
            var args = methodStatement.ParameterList.Parameters.Select(p => $"{p.Identifier}:{p.AsClause?.Type.ToString() ?? ""}");

            WriteLine($"{modifiers}{name}({string.Join(", ", args)}) : {returnType}");
            //base.VisitMethodBlock(node);
        }

        //public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        //{
        //    WriteLine($"{node.Identifier}{node.EqualsValue},");
        //}
        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            WriteLine($"{node.Identifier}{node.Initializer?.ToString() ?? ""},");
        }

        //public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        //{
        //    if (IsIgnoreMember(node.Modifiers)) { return; }

        //    var modifiers = GetMemberModifiersText(node.Modifiers);
        //    var name = string.Join(",", node.Declaration.Variables.Select(v => v.Identifier));
        //    var typeName = node.Declaration.Type.ToString();

        //    WriteLine($"{modifiers} <<{node.EventKeyword}>> {name} : {typeName} ");
        //}
        public override void VisitEventStatement(EventStatementSyntax node)
        {
            if (IsIgnoreMember(node.Modifiers)) { return; }

            var modifiers = GetMemberModifiersText(node.Modifiers);
            var name = string.Join(",", node.Identifier);
            var typeName = node.AsClause?.Type.ToString() ?? "";

            WriteLine($"{modifiers} <<{node.EventKeyword}>> {name} : {typeName} ");
            //base.VisitEventStatement(node);
        }

        private void WriteLine(string line)
        {
            var space = string.Concat(Enumerable.Repeat(indent, nestingDepth));
            writer.WriteLine(space + line);
        }

        private bool SkipInnerTypeDeclaration(SyntaxNode node)
        {
            if (nestingDepth > 0)
            {
                _innerTypeDeclarationNodes.Add(node);
                return true;
            }
            return false;
        }

        private void GenerateInnerTypeDeclarations()
        {
            foreach (var node in _innerTypeDeclarationNodes)
            {
                var generator = new ClassDiagramGeneratorForVB(writer, indent);
                generator.GenerateInternal(node);

                var outerTypeNode = node.Parent as TypeBlockSyntax;
                var innerTypeNode = node as TypeBlockSyntax;
                if (outerTypeNode != null && innerTypeNode != null)
                {
                    var outerTypeName = TypeNameTextForVB.From(outerTypeNode);
                    var innerTypeName = TypeNameTextForVB.From(innerTypeNode);
                    WriteLine($"{outerTypeName.Identifier} +-- {innerTypeName.Identifier}");
                }
            }
        }

        //private void VisitTypeDeclaration(TypeDeclarationSyntax node, Action visitBase)
        //{
        //    if (SkipInnerTypeDeclaration(node)) { return; }

        //    _inheritanceRelationsips.AddFrom(node);

        //    var modifiers = GetTypeModifiersText(node.Modifiers);
        //    var keyword = (node.Modifiers.Any(SyntaxKind.AbstractKeyword) ? "abstract " : "")
        //        + node.Keyword.ToString();

        //    var typeName = TypeNameText.From(node);
        //    var name = typeName.Identifier;
        //    var typeParam = typeName.TypeArguments;

        //    WriteLine($"{keyword} {name}{typeParam} {modifiers}{{");

        //    nestingDepth++;
        //    visitBase();
        //    nestingDepth--;

        //    WriteLine("}");
        //}
        private void VisitTypeBlock(TypeBlockSyntax node, Action visitBase)
        {
            if (SkipInnerTypeDeclaration(node)) { return; }

            _inheritanceRelationsips.AddFrom(node);

            var modifiers = GetTypeModifiersText(node.BlockStatement.Modifiers);
            var keyword = (node.BlockStatement.Modifiers.Any(SyntaxKind.MustInheritKeyword) ? "abstract " : "")
                + node.BlockStatement.DeclarationKeyword.ToString();

            var typeName = TypeNameTextForVB.From(node);
            var name = typeName.Identifier;
            var typeParam = typeName.TypeArguments;

            WriteLine($"{keyword} {name}{typeParam} {modifiers}{{");

            nestingDepth++;
            visitBase();
            nestingDepth--;

            WriteLine("}");
        }

        private string GetTypeModifiersText(SyntaxTokenList modifiers)
        {
            var tokens = modifiers.Select(token =>
            {
                switch (token.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.PrivateKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.FriendKeyword: //SyntaxKind.InternalKeyword:
                    case SyntaxKind.MustInheritKeyword: //SyntaxKind.AbstractKeyword:
                        return "";
                    default:
                        return $"<<{token.ValueText}>>";
                }
            }).Where(token => token != "");

            var result = string.Join(" ", tokens);
            if (result != string.Empty)
            {
                result += " ";
            };
            return result;
        }

        private bool IsIgnoreMember(SyntaxTokenList modifiers)
        {
            if (_ignoreMemberAccessibilities == Accessibilities.None) { return false; }

            var tokenKinds = modifiers.Select(x => x.Kind()).ToArray();

            if (_ignoreMemberAccessibilities.HasFlag(Accessibilities.ProtectedInternal)
                && tokenKinds.Contains(SyntaxKind.ProtectedKeyword)
                && tokenKinds.Contains(SyntaxKind.FriendKeyword))
            {
                return true;
            }

            if (_ignoreMemberAccessibilities.HasFlag(Accessibilities.Public)
                && tokenKinds.Contains(SyntaxKind.PublicKeyword))
            {
                return true;
            }

            if (_ignoreMemberAccessibilities.HasFlag(Accessibilities.Protected)
                && tokenKinds.Contains(SyntaxKind.ProtectedKeyword))
            {
                return true;
            }

            if (_ignoreMemberAccessibilities.HasFlag(Accessibilities.Internal)
                && tokenKinds.Contains(SyntaxKind.FriendKeyword))
            {
                return true;
            }

            if (_ignoreMemberAccessibilities.HasFlag(Accessibilities.Private)
                && tokenKinds.Contains(SyntaxKind.PrivateKeyword))
            {
                return true;
            }
            return false;
        }

        private string GetMemberModifiersText(SyntaxTokenList modifiers)
        {
            var tokens = modifiers.Select(token =>
            {
                switch (token.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                        return "+";
                    case SyntaxKind.PrivateKeyword:
                        return "-";
                    case SyntaxKind.ProtectedKeyword:
                        return "#";
                    case SyntaxKind.MustInheritKeyword:
                    case SyntaxKind.StaticKeyword:
                        return $"{{{token.ValueText}}}";
                    case SyntaxKind.FriendKeyword:
                    default:
                        return $"<<{token.ValueText}>>";
                }
            });

            var result = string.Join(" ", tokens);
            if (result != string.Empty)
            {
                result += " ";
            };
            return result;
        }
    }

}
