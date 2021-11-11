using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Liversen.DependencyCop
{
    static class Helpers
    {
        public static ITypeSymbol DetermineReferredType(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            var semanticModel = context.SemanticModel;
            var typeSymbol = semanticModel.GetTypeInfo(node).Type;
            if (typeSymbol != null)
            {
                return typeSymbol;
            }
            var symbolInfo = semanticModel.GetSymbolInfo(node);
            switch (symbolInfo.Symbol)
            {
                case ITypeSymbol symbolInfoTypeSymbol:
                    return symbolInfoTypeSymbol;
                case IMethodSymbol methodSymbol:
                    return methodSymbol.ReturnType;
                default:
                    return null;
            }
        }

        public static ITypeSymbol DetermineEnclosingType(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            var semanticModel = context.SemanticModel;
            var typeDeclarationSyntaxNode = node.AncestorsAndSelf().FirstOrDefault(i => i.IsTypeDeclaration());
            if (typeDeclarationSyntaxNode == null)
            {
                return null;
            }
            return semanticModel.GetDeclaredSymbol(typeDeclarationSyntaxNode) as ITypeSymbol;
        }
    }
}
