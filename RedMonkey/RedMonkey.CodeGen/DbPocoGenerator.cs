//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text.RegularExpressions;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using RedMonkey.Config;
//using RedMonkey.SchemaAnalysis;
//using SqlKata;
//using Column = RedMonkey.SchemaAnalysis.Column;
//
//namespace RedMonkey.CodeGen
//{
//    public class DbPocoGenerator
//    {
//        private readonly SchemaReader _schemaReader;
//        private readonly SettingsService _settingsService;
//
//        public DbPocoGenerator(SchemaReader schemaReader, SettingsService settingsService)
//        {
//            _schemaReader = schemaReader;
//            _settingsService = settingsService;
//        }
//
//        public void GenerateCodeFiles(Settings settings = null)
//        {
//            if (settings == null)
//                settings = _settingsService.GetSettings();
//            var targetDir = new DirectoryInfo(settings.DbCodeGen.TargetDirectory);
//            if (!targetDir.Exists)
//                targetDir.Create();
//            var entitiesDir = new DirectoryInfo(Path.Combine(targetDir.FullName, "Entities"));
//            if (!entitiesDir.Exists)
//                entitiesDir.Create();
//            entitiesDir.GetFiles("*.cs").ToList().ForEach(f => f.Delete());
//            var servicesDir = new DirectoryInfo(Path.Combine(targetDir.FullName, "Entities"));
//            if (!servicesDir.Exists)
//                servicesDir.Create();
//            servicesDir.GetFiles("*.cs").ToList().ForEach(f => f.Delete());
//            var tables = _schemaReader.LoadTables();
//            //CreateTablesClass(tables, Path.Combine(targetDir.FullName, "TableNames.cs"), settings.DbCodeGen.NameSpace);
//            foreach (var table in tables.Where(t => !t.Ignore))
//            {
//                if (settings.DbCodeGen.IgnoreTables.Any(i => Regex.IsMatch(table.Name, i)))
//                    continue;
//
//                var entitySourceFile = Path.Combine(entitiesDir.FullName, $"{table.ClassName}.cs");
//                CreateTableClass(table, entitySourceFile, settings.DbCodeGen.NameSpace + ".Entities");
//                var serviceSourceFile = Path.Combine(servicesDir.FullName, $"{table.ClassName}Service.cs");
//                CreateServiceClass(table, serviceSourceFile, settings.DbCodeGen.NameSpace + ".Services");
//            }
//        }
//
//        private void CreateServiceClass(Table table, string serviceSourceFile, string nameSpace)
//        {
//            throw new System.NotImplementedException();
//        }
//
//        private static void CreateTableClass(Table table, string sourceFilePath, string nameSpace)
//        {
//            var cu = SyntaxFactory.CompilationUnit().WithMembers(
//                SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
//                    SyntaxFactory.NamespaceDeclaration(
//                            SyntaxFactory.ParseName(nameSpace))
//                        .WithNamespaceKeyword(
//                            SyntaxFactory.Token(
//                                SyntaxFactory.TriviaList(
//                                    SyntaxFactory.Comment("// ReSharper disable InconsistentNaming"),
//                                    SyntaxFactory.Comment("// ReSharper disable UnusedMember.Global"),
//                                    SyntaxFactory.Comment("// ReSharper disable IdentifierTypo"),
//                                    SyntaxFactory.Comment("// ReSharper disable BuiltInTypeReferenceStyle"),
//                                    SyntaxFactory.Comment("// ReSharper disable RedundantNameQualifier")),
//                                SyntaxKind.NamespaceKeyword,
//                                SyntaxFactory.TriviaList())).WithUsings(
//                            SyntaxFactory.List(
//                                new UsingDirectiveSyntax[]
//                                {
//                                    //UsingDirective(IdentifierName("System")), 
//                                    //UsingDirective(IdentifierName("NodaTime")),
//                                    //UsingDirective(IdentifierName("SqlKata")),
//                                    //UsingDirective(IdentifierName("Util.Types"))
//                                })).WithMembers(
//                            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
//                                SyntaxFactory.ClassDeclaration(table.ClassName)
//                                    .WithModifiers(SyntaxFactory.TokenList(new[] {SyntaxFactory.Token(SyntaxKind.PublicKeyword)}))
//                                    .WithBaseList(
//                                        SyntaxFactory.BaseList(
//                                            SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
//                                                SyntaxFactory.SimpleBaseType(
//                                                    SyntaxFactory.GenericName(SyntaxFactory.Identifier("BaseClass")).WithTypeArgumentList(
//                                                        SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList<TypeSyntax>(SyntaxFactory.IdentifierName(table.ClassName))))))))
//                                    .WithMembers(GetTableClassProperties(table))))));
//
//            File.WriteAllText(sourceFilePath, cu.NormalizeWhitespace().ToFullString());
//        }
//
//        private static SyntaxList<MemberDeclarationSyntax> GetTableClassProperties(Table table)
//        {
//            var list = new SyntaxList<MemberDeclarationSyntax>();
//            list = list.Add(GetTableNameProperty(table.Name));
//            list = list.AddRange(table.Columns.Where(c => !c.Ignore).Select(GetColumnProperty).ToList());
//            list = list.Add(GetColumnNameClass(table));
//            return list;
//        }
//
//        private static ClassDeclarationSyntax GetColumnNameClass(Table table)
//        {
//            var list = new SyntaxList<MemberDeclarationSyntax>();
//            list = list.AddRange(
//                table.Columns.Select(
//                    c => SyntaxFactory.FieldDeclaration(
//                            SyntaxFactory.VariableDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))).WithVariables(
//                                SyntaxFactory.SingletonSeparatedList(
//                                    SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(c.PropertyName)).WithInitializer(
//                                        SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(c.Name)))))))
//                        .WithModifiers(
//                            SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.ConstKeyword)))).ToList());
//            return SyntaxFactory.ClassDeclaration("_Columns").WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword))).WithMembers(list);
//        }
//
//        private static SyntaxList<AttributeListSyntax> GetColumnAttributes(Column column)
//        {
//            var list = new List<AttributeListSyntax>
//            {
//                SyntaxFactory.AttributeList(
//                    SyntaxFactory.SingletonSeparatedList(
//                        SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(typeof(ColumnAttribute).FullName)).WithArgumentList(
//                            SyntaxFactory.AttributeArgumentList(
//                                SyntaxFactory.SingletonSeparatedList(
//                                    SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(column.Name))))))))
//            };
//            if (column.IsPrimaryKey)
//                list.Add(
//                    SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(typeof(KeyAttribute).FullName)))));
//            return new SyntaxList<AttributeListSyntax>(list.ToArray());
//        }
//
//        private static PropertyDeclarationSyntax GetColumnProperty(Column column)
//        {
//            TypeSyntax type = SyntaxFactory.IdentifierName(column.PropertyType.FullName);
//            if (column.IsNullable && column.PropertyType.IsValueType)
//                type = SyntaxFactory.NullableType(type);
//            return SyntaxFactory.PropertyDeclaration(type, SyntaxFactory.Identifier(column.PropertyName))
//                .WithAttributeLists(GetColumnAttributes(column))
//                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword))).WithAccessorList(
//                    SyntaxFactory.AccessorList(
//                        SyntaxFactory.List(
//                            new[]
//                            {
//                                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
//                                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
//                            })));
//        }
//
//        private static FieldDeclarationSyntax GetTableNameProperty(string tableName)
//        {
//            return
//                SyntaxFactory.FieldDeclaration(
//                        SyntaxFactory.VariableDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.StringKeyword, SyntaxFactory.TriviaList(SyntaxFactory.Space)))).WithVariables(
//                            SyntaxFactory.SingletonSeparatedList(
//                                SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(SyntaxFactory.TriviaList(), "_TableName", SyntaxFactory.TriviaList(SyntaxFactory.Space))).WithInitializer(
//                                    SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(tableName)))
//                                        .WithEqualsToken(SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.EqualsToken, SyntaxFactory.TriviaList(SyntaxFactory.Space)))))))
//                    .WithModifiers(
//                        SyntaxFactory.TokenList(
//                            SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.PublicKeyword, SyntaxFactory.TriviaList(SyntaxFactory.Space)),
//                            SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.ConstKeyword, SyntaxFactory.TriviaList(SyntaxFactory.Space))))
//                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.SemicolonToken, SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)));
//        }
//    }
//}

