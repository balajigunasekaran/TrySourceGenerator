using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TrySourceGenerator
{
    internal static class RosylynExtensions
    {
        public static IEnumerable<AttributeSyntax> GetAttributes(
            this SyntaxList<AttributeListSyntax> attributes)
        {
            return attributes.SelectMany(y => y.Attributes);
        }

        public static ITypeSymbol GetTypeSymbol(this SyntaxNode node, SemanticModel model)
        {
            return model.GetTypeInfo(node).Type;
        }

        public static bool IsAssignableTo(this SyntaxNode node,
            SemanticModel model, INamedTypeSymbol symbol, SymbolEqualityComparer comparer = null)
        {
            comparer ??= SymbolEqualityComparer.Default;
            var type = node.GetTypeSymbol(model);
            return IsAssignableTo(type, symbol, comparer);
        }

        public static bool IsAssignableTo(this ITypeSymbol type,
            INamedTypeSymbol symbol, SymbolEqualityComparer comparer = null)
        {
            comparer ??= SymbolEqualityComparer.Default;
            return type.Equals(symbol, comparer)
                || type.IsInheriting(symbol, comparer)
                || type.IsImplementing(symbol, comparer);
        }

        private static bool IsImplementing(this ITypeSymbol type,
            INamedTypeSymbol symbol, SymbolEqualityComparer comparer = null)
        {
            comparer ??= SymbolEqualityComparer.Default;
            foreach (var @interface in type.AllInterfaces)
            {
                if (@interface.Equals(symbol, comparer) ||
                    @interface.OriginalDefinition.Equals(symbol, comparer))
                    return true;
                if (IsImplementing(@interface, symbol, comparer))
                    return true;
            }
            return false;
        }


        private static bool IsInheriting(this ITypeSymbol type,
            INamedTypeSymbol symbol, SymbolEqualityComparer comparer = null)
        {
            comparer ??= SymbolEqualityComparer.Default;
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (symbol.Equals(baseType, comparer))
                    return true;
                baseType = baseType.BaseType;
            }
            return false;
        }
    }
}