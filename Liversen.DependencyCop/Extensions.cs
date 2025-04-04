using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Liversen.DependencyCop
{
    static class Extensions
    {
        public static bool IsTypeDeclaration(this SyntaxNode syntaxNode)
        {
            return syntaxNode is BaseTypeDeclarationSyntax || syntaxNode is DelegateDeclarationSyntax;
        }

        public static string NamespaceFullName(this ITypeSymbol typeSymbol)
        {
            var namespaces = typeSymbol.ContainingNamespace?.ConstituentNamespaces;
            return namespaces.HasValue ? string.Join(".", namespaces.Value) : null;
        }

        // Helper method to find the containing namespace of a given syntax node
        public static string GetContainingNamespace(this TypeSyntax node, SemanticModel semanticModel)
        {
            Debug.Assert(semanticModel != null, nameof(semanticModel) + " != null");
            return semanticModel.GetSymbolInfo(node).Symbol?.ContainingNamespace.ToString();
        }
    }
}
