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
    }
}
