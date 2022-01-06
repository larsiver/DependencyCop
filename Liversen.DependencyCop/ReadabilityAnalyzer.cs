using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Liversen.DependencyCop
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReadabilityAnalyzer : DiagnosticAnalyzer
    {
        const string Dc1001NamespacePrefixKey = "dotnet_diagnostic.DC1001.namespace_prefix";
        readonly DiagnosticDescriptor descriptor1001 = new DiagnosticDescriptor(
            "DC1001",
            "Using namespace statements must not reference disallowed namespaces",
            "Do not use '{0}' in a using statement, use fully-qualified names",
            "DC.Readability",
            DiagnosticSeverity.Warning,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor1001);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterCompilationStartAction(CompilationStart);
        }

        void CompilationStart(CompilationStartAnalysisContext startContext)
        {
            var optionsProvider = startContext.Options.AnalyzerConfigOptionsProvider;
            optionsProvider.GlobalOptions.TryGetValue(Dc1001NamespacePrefixKey, out var dc1001namespacePrefix);
            startContext.RegisterSyntaxNodeAction(c => AnalyseUsingStatement(c, dc1001namespacePrefix?.Trim()), SyntaxKind.UsingDirective);
        }

        void AnalyseUsingStatement(SyntaxNodeAnalysisContext context, string namespacePrefix)
        {
            if (context.Node is UsingDirectiveSyntax node && string.IsNullOrEmpty(node.StaticKeyword.Text))
            {
                if (string.IsNullOrWhiteSpace(namespacePrefix))
                {
                    throw new ArgumentException($"Rule {descriptor1001.Id} requires setting property {Dc1001NamespacePrefixKey} to a non-empty value in a global analyzer file, see https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/configuration-files.");
                }
                var name = node.Name.ToFullString();
                if (name == namespacePrefix || name.StartsWith($"{namespacePrefix}.", StringComparison.Ordinal))
                {
                    context.ReportDiagnostic(Diagnostic.Create(descriptor1001, node.GetLocation(), name));
                }
            }
        }
    }
}