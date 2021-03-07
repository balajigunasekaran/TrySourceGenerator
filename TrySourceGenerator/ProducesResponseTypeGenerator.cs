using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrySourceGenerator
{
    [Generator]
    public class ProducesResponseTypeGenerator : ISourceGenerator
    {
        const string RouteAttribute = "Microsoft.AspNetCore.Mvc.RouteAttribute";
        const string HttpMethodAttribute = "Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute";

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                var index = 0;
                var routeAttribute = context.Compilation.GetTypeByMetadataName(RouteAttribute);
                if (context.SyntaxReceiver is SyntaxReceiver receiver &&
                    receiver.CandidateClasses.Count > 0)
                {
                    var controllers = receiver.CandidateClasses.Select(x =>
                        ((ClassDeclarationSyntax Class, SemanticModel Model))
                            (x, context.Compilation.GetSemanticModel(x.SyntaxTree)))
                        .Where(x =>
                            x.Class.AttributeLists.GetAttributes().Any(
                                at => at.IsAssignableTo(x.Model, routeAttribute)));
                    foreach (var controller in controllers)
                    {
                        foreach (var method in GetActionMethods(controller))
                        {
                            if (IsReturningEnumerable(method, controller.Model, out var returnType))
                            {
                                var source = GenerateSource(controller.Class, method, returnType);
                                var sourceText = SourceText.From(source, Encoding.UTF8);
                                context.AddSource($"ProducesResponseType_{++index}", sourceText);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debugger.Launch();
            }
        }

        private string GenerateSource(
            ClassDeclarationSyntax @class, MethodDeclarationSyntax method, ITypeSymbol produceResponseType)
        {
            return
                $@"
                {@class.SyntaxTree.GetCompilationUnitRoot().Usings}
                namespace {(@class.Parent as NamespaceDeclarationSyntax).Name}
                {{
                    partial class {@class.Identifier}
                    {{
                        [Microsoft.AspNetCore.Mvc.ProducesResponseType(typeof({produceResponseType.ToDisplayString()}), (System.Int32)System.Net.HttpStatusCode.OK)]
                        public partial {method.ReturnType} {method.Identifier}{method.ParameterList};
                    }}
                }}";
        }

        private bool IsReturningEnumerable(MethodDeclarationSyntax method, SemanticModel model, out ITypeSymbol returnType)
        {
            const string taskType = "System.Threading.Tasks.Task`1";
            returnType = method.ReturnType.GetTypeSymbol(model);
            var taskSymbol = model.Compilation.GetTypeByMetadataName(taskType);
            if (returnType.OriginalDefinition.Equals(taskSymbol, SymbolEqualityComparer.Default))
                returnType = (returnType as INamedTypeSymbol).TypeArguments[0];

            var enumerableType = "System.Collections.Generic.IEnumerable`1";
            var enumerableSymbol = model.Compilation.GetTypeByMetadataName(enumerableType);
            return returnType.OriginalDefinition.IsAssignableTo(enumerableSymbol);
        }

        private IEnumerable<MethodDeclarationSyntax> GetActionMethods(
            (ClassDeclarationSyntax Controller, SemanticModel Model) _)
        {
            var httpMethodAttribute = _.Model.Compilation.GetTypeByMetadataName(HttpMethodAttribute);
            return _.Controller.Members.Where(x =>
                x.Kind() == SyntaxKind.MethodDeclaration &&
                x.Modifiers.Any(SyntaxKind.PublicKeyword) &&
                x.Modifiers.Any(SyntaxKind.PartialKeyword) &&
                x.AttributeLists.GetAttributes().Any(
                    at => at.IsAssignableTo(_.Model, httpMethodAttribute)
                )).Select(x => x as MethodDeclarationSyntax);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }
    }
}