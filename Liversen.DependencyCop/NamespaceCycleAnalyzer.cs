using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Liversen.DependencyCop
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NamespaceCycleAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            "DC1003",
            "Code must not contain namespace cycles",
            "Break up namespace cycle '{0}'",
            "DC.Design",
            DiagnosticSeverity.Warning,
            true,
            helpLinkUri: "https://github.com/larsiver/DependencyCop/blob/main/Liversen.DependencyCop/Documentation/DC1003.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

        public static (string SourceNamespaceReduced, string TargetNamespaceReduced) GetReducedNamespaces(string sourceNamespace, string targetNamespace)
        {
            var sourceNames = sourceNamespace.Split('.');
            var targetNames = targetNamespace.Split('.');
            for (var i = 0; i < Math.Min(sourceNames.Length, targetNames.Length); ++i)
            {
                if (sourceNames[i] != targetNames[i])
                {
                    return (string.Join(".", sourceNames.Take(i + 1)), string.Join(".", targetNames.Take(i + 1)));
                }
            }
            return (string.Empty, string.Empty);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1026:Enable concurrent execution", Justification = "Cannot have two simultaneous threads change state at the same time.")]
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterCompilationStartAction(compilationContext =>
            {
                var inner = new Inner();
                compilationContext.RegisterSyntaxNodeAction(inner.Analyse, SyntaxKind.IdentifierName, SyntaxKind.GenericName, SyntaxKind.DefaultLiteralExpression);
            });
        }

        class Inner
        {
            readonly Dag _dag = new Dag();

            public void Analyse(SyntaxNodeAnalysisContext context)
            {
                var sourceType = Helpers.DetermineEnclosingType(context);
                var targetType = Helpers.DetermineReferredType(context);
                if (sourceType != null && targetType != null && SameAssembly(sourceType, targetType))
                {
                    var sourceNamespace = sourceType.NamespaceFullName();
                    var targetNamespace = targetType.NamespaceFullName();
                    if (sourceNamespace != null && targetNamespace != null && sourceNamespace != targetNamespace)
                    {
                        var (sourceNamespaceReduced, targetNamespaceReduced) = GetReducedNamespaces(sourceNamespace, targetNamespace);
                        var cycle = _dag.TryAddVertex(sourceNamespaceReduced, targetNamespaceReduced);
                        if (cycle != null)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, context.Node.GetLocation(), string.Join("->", cycle)));
                        }
                    }
                }
            }

            static bool SameAssembly(ITypeSymbol left, ITypeSymbol right) =>
                left.ContainingAssembly != null
                && right.ContainingAssembly != null
                && AssemblyIdentityComparer.Default.Compare(
                    left.ContainingAssembly.Identity,
                    right.ContainingAssembly.Identity)
                == AssemblyIdentityComparer.ComparisonResult.Equivalent;
        }
    }
}