using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Liversen.DependencyCop
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DesignAnalyzer : DiagnosticAnalyzer
    {
        readonly DiagnosticDescriptor descriptor1002 = new DiagnosticDescriptor(
            "DC1002",
            "Do not use types in descendant namespaces",
            "Do not use type '{0}' from descendant namespace '{1}'",
            "DC.Design",
            DiagnosticSeverity.Warning,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor1002);

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
                    context.ReportDiagnostic(Diagnostic.Create(descriptor1002, context.Node.GetLocation(), type.Name, typeNamespace));
                }
            }
        }
    }
}