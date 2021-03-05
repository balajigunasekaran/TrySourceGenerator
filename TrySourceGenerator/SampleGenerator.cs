using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace TrySourceGenerator
{
    [Generator]
    public class SampleGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            //var root = context.Compilation.SyntaxTrees.First().GetRoot();
            //var nodes = root.DescendantNodes();
            //var publicClasses = nodes.OfType<ClassDeclarationSyntax>()
            //    .Where(_ => _.Modifiers.Any(SyntaxKind.PublicKeyword| SyntaxKind.PartialKeyword));
            //foreach (var classSyntax in publicClasses)
            //{
            //    var attributeList =
            //        AttributeList(
            //            SingletonSeparatedList(
            //                Attribute(
            //                    QualifiedName(
            //                        QualifiedName(
            //                            QualifiedName(
            //                                IdentifierName("Microsoft"),
            //                                IdentifierName("AspNetCore")),
            //                            IdentifierName("Mvc")),
            //                        IdentifierName("ApiControllerAttribute")))));

            //    var sourceCode = @"namespace TestSourceLibrary
            //        {

            //            [Microsoft.AspNetCore.Mvc.ApiControllerAttribute]
            //            public partial class Class1
            //            {
            //            }
            //        }";

            //    var newSyntax = classSyntax.AddAttributeLists(attributeList);
            //    //var a = classSyntax.Parent.ReplaceNode(classSyntax, newSyntax);
            //    //root.ReplaceNode()
            //}
            System.Diagnostics.Debugger.Launch();
            var received = context.SyntaxReceiver is SyntaxReceiver receiver && receiver.ClassSyntax.Count > 0;
            if (!received) return;

            //var sourceCode = @"namespace TestSourceLibrary
            //        {

            //            [Microsoft.AspNetCore.Mvc.ApiControllerAttribute]
            //            public partial class Class1
            //            {
            //            }
            //        }";
            //context.AddSource("tes", sourceCode);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }
    }

    public class SyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> ClassSyntax = new ();

        public void OnVisitSyntaxNode(SyntaxNode node)
        {
            if (node is ClassDeclarationSyntax classSyntax &&
                classSyntax.Modifiers.Any(SyntaxKind.PublicKeyword) &&
                classSyntax.Modifiers.Any(SyntaxKind.PartialKeyword) &&
                classSyntax.AttributeLists.Count > 0)
                ClassSyntax.Add(classSyntax);
        }
    }
}