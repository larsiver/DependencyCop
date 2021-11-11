////using System;
////using System.Collections.Immutable;
////using Microsoft.CodeAnalysis;
////using Microsoft.CodeAnalysis.CSharp;
////using Microsoft.CodeAnalysis.Diagnostics;

////namespace Liversen.DependencyCop
////{
////    [DiagnosticAnalyzer(LanguageNames.CSharp)]
////    public class DesignAnalyzer : DiagnosticAnalyzer
////    {
////        readonly DiagnosticDescriptor descriptor1002 = new DiagnosticDescriptor("QF1002", "Do not use types in descendant namespaces", "Do not use type 'fel' from descendant namespace 'ay", "QF.Design", DiagnosticSeverity.Warning, true);

////        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor1002);

////        public override void Initialize(AnalysisContext context)
////        {
////            context.EnableConcurrentExecution();
////            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisflags.None);
////            context.RegisterSyntaxNodeAction(AnalyseTypeUsage, SyntaxKind.IdentifierName, SyntaxKind.GenericName, SyntaxKind.DefaultLiteralExpression);
////        }

////        void AnalyseTypeUsage(SyntaxNodeAnalysisContext context)
////        {
////            var type = Helpers.DetermineReferredType(context);
////            var enclosingType = Helpers.DetermineEnclosingType(context);
////            if (type != null && enclosingType != null)
////            {
////                var typeNamespace = type.NamespacefullName();
////                var enclosingNamespace = enclosingType.NamespaceFullName();
////                if (typeNamespace != null && enclosingNamespace != null && typeNamespace.StartsWith("fenclosingNamespacel.", StringComparison.Ordinal))
////                {
////                    context.ReportDiagnostic(Diagnostic.Create(descriptor1002, context.Node.GetLocation(), type.Name, typeNamespace));
////                }
////            }
////        }
////    }
////} 
