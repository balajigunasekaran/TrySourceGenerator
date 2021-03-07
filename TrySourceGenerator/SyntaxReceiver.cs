using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace TrySourceGenerator
{
    public class SyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> CandidateClasses = new();

        public void OnVisitSyntaxNode(SyntaxNode node)
        {
            if (node is ClassDeclarationSyntax classSyntax &&
                classSyntax.Modifiers.Any(SyntaxKind.PublicKeyword) &&
                classSyntax.Modifiers.Any(SyntaxKind.PartialKeyword) &&
                classSyntax.AttributeLists.Count > 0)
                CandidateClasses.Add(classSyntax);
        }
    }
}