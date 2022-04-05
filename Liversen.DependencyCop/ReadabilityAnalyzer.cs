using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Liversen.DependencyCop
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReadabilityAnalyzer : DiagnosticAnalyzer
    {
        const string Dc1001NamespacePrefixesKey = "dotnet_diagnostic.DC1001.namespace_prefixes";
        readonly DiagnosticDescriptor descriptor1001 = new DiagnosticDescriptor(
            "DC1001",
            "Using namespace statements must not reference disallowed namespaces",
            "Do not use '{0}' in a using statement, use fully-qualified names",
            "DC.Readability",
            DiagnosticSeverity.Warning,
            true,
            helpLinkUri: "https://github.com/larsiver/DependencyCop/blob/main/Liversen.DependencyCop/Documentation/DC1001.md");

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
            optionsProvider.GlobalOptions.TryGetValue(Dc1001NamespacePrefixesKey, out var dc1001namespacePrefix);
            var disallowedNamespacePrefixes = (dc1001namespacePrefix?.Trim() ?? string.Empty)
                .Split(';')
                .Select(s => s.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToImmutableArray();
            startContext.RegisterSyntaxNodeAction(c => AnalyseUsingStatement(c, disallowedNamespacePrefixes), SyntaxKind.UsingDirective);
        }

        void AnalyseUsingStatement(SyntaxNodeAnalysisContext context, ImmutableArray<string> disallowedNamespacePrefixes)
        {
            if (context.Node is UsingDirectiveSyntax node && string.IsNullOrEmpty(node.StaticKeyword.Text))
            {
                var name = node.Name.ToFullString();
                foreach (var disallowedNamespacePrefix in disallowedNamespacePrefixes)
                {
                    if (name == disallowedNamespacePrefix || name.StartsWith($"{disallowedNamespacePrefix}.", StringComparison.Ordinal))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(descriptor1001, node.GetLocation(), name));
                    }
                }
            }
        }
    }
}