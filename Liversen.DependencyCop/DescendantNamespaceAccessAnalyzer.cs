using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Liversen.DependencyCop
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DescendantNamespaceAccessAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            "DC1002",
            "Code must not refer code in descendant namespaces",
            "Do not use type '{0}' from descendant namespace '{1}'",
            "DC.Design",
            DiagnosticSeverity.Warning,
            true,
            helpLinkUri: "https://github.com/larsiver/DependencyCop/blob/main/Liversen.DependencyCop/Documentation/DC1002.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyseTypeUsage, SyntaxKind.IdentifierName, SyntaxKind.GenericName, SyntaxKind.DefaultLiteralExpression);
        }

        void AnalyseTypeUsage(SyntaxNodeAnalysisContext context)
        {
            var type = Helpers.DetermineReferredType(context);
            var enclosingType = Helpers.DetermineEnclosingType(context);
            if (type != null && enclosingType != null)
            {
                var typeNamespace = type.NamespaceFullName();
                var enclosingNamespace = enclosingType.NamespaceFullName();
                if (typeNamespace != null && enclosingNamespace != null && typeNamespace.StartsWith($"{enclosingNamespace}.", StringComparison.Ordinal))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, context.Node.GetLocation(), type.Name, typeNamespace));
                }
            }
        }
    }
}
