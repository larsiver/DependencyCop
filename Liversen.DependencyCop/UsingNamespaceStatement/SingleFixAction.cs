using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Liversen.DependencyCop.UsingNamespaceStatement
{
    internal class SingleFixAction
    {
        private readonly SemanticModel semanticModel;
        private readonly DocumentEditor editor;
        private readonly string namespaceName;
        private readonly Document document;
        private readonly UsingDirectiveSyntax usingDirective;
        readonly HashSet<string> staticUsings = new HashSet<string>();

        SingleFixAction(SemanticModel semanticModel, DocumentEditor editor, string namespaceName, Document document, UsingDirectiveSyntax usingDirective)
        {
            this.semanticModel = semanticModel;
            this.editor = editor;
            this.namespaceName = namespaceName;
            this.document = document;
            this.usingDirective = usingDirective;
        }

        public static async Task<Document> ApplyFixAsync(UsingDirectiveSyntax usingDirective, Document document, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
            Debug.Assert(usingDirective.Name != null, "The analyzer will not report any using directives where this is null");
            var namespaceName = usingDirective.Name.ToString();

            var fixAction = new SingleFixAction(semanticModel, editor, namespaceName, document, usingDirective);

            return await fixAction.ApplyFix(cancellationToken);
        }

        private static string RemoveCommonNameSpace(string originalNameSpace, string compareNameSpace)
        {
            var common = new StringBuilder();

            var nameSpace1Parts = originalNameSpace.Split('.');
            var nameSpace2Parts = compareNameSpace.Split('.');

            for (int i = 0; i < nameSpace1Parts.Length; i++)
            {
                if (nameSpace1Parts[i] == nameSpace2Parts[i])
                {
                    common.Append(nameSpace1Parts[i] + ".");
                }
                else
                {
                    break;
                }
            }

            return originalNameSpace.Substring(common.Length);
        }

        private async Task<Document> ApplyFix(CancellationToken cancellationToken)
        {
            var classDeclarations = await FindNameSpaceUsagesAsync(cancellationToken);
            FixNameSpaceUsages(classDeclarations, cancellationToken);

            // Remove the using directive.
            editor.RemoveNode(usingDirective);

            return editor.GetChangedDocument();
        }

        private async Task<List<TypeDeclaration>> FindNameSpaceUsagesAsync(CancellationToken cancellationToken)
        {
            var back = new List<TypeDeclaration>();

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            Debug.Assert(root != null, "Should not be null, since it was checked previously");

            // Find all class declarations in the document
            var nameSpaceDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            foreach (var namespaceDeclarationSyntax in nameSpaceDeclarations)
            {
                var typeOuterNamespace = semanticModel.GetDeclaredSymbol(namespaceDeclarationSyntax, cancellationToken).NamespaceFullName();

                if (typeOuterNamespace == namespaceName)
                {
                    continue;
                }

                var typeDeclarations = namespaceDeclarationSyntax.DescendantNodes().OfType<SimpleNameSyntax>();

                // Filter type declarations that are within the specified namespace
                foreach (var typeDecl in typeDeclarations)
                {
                    var declarationInNamespace = typeOuterNamespace;
                    if (declarationInNamespace != namespaceName)
                    {
                        var containingNamespace = typeDecl.GetContainingNamespace(semanticModel);
                        if (containingNamespace != null && containingNamespace == namespaceName)
                        {
                            back.Add(new TypeDeclaration(declarationInNamespace, typeDecl));
                        }
                    }
                }
            }

            return back;
        }

        private void FixNameSpaceUsages(IReadOnlyList<TypeDeclaration> classDeclarations, CancellationToken cancellationToken)
        {
            foreach (var classDecl in classDeclarations)
            {
                var symbolInfo = semanticModel.GetSymbolInfo(classDecl.Node, cancellationToken);
                var symbol = symbolInfo.Symbol;
                if (symbol?.ContainingNamespace != null &&
                    symbol.ContainingNamespace.ToDisplayString() == namespaceName)
                {
                    var fullNameSpace = symbol.ToDisplayString();

                    // This indicates that it is an extension method.
                    if (classDecl.Node.Parent is MemberAccessExpressionSyntax)
                    {
                        FixForExtensionMethod(symbol);
                    }
                    else
                    {
                        QualifyUsageOfType(fullNameSpace, classDecl);
                    }
                }
            }
        }

        private void QualifyUsageOfType(string fullNameSpace, TypeDeclaration classDecl)
        {
            var replace = RemoveCommonNameSpace(fullNameSpace, classDecl.NameSpace);
            NameSyntax qualifiedName = SyntaxFactory.ParseName(replace)
                .WithLeadingTrivia(classDecl.Node.GetLeadingTrivia())
                .WithTrailingTrivia(classDecl.Node.GetTrailingTrivia());

            // At least some namespace already present - maybe even too much.
            if (classDecl.Node.Parent is QualifiedNameSyntax identifierQualifiedNameSyntax)
            {
                if (identifierQualifiedNameSyntax.ToFullString() != qualifiedName.ToFullString())
                {
                    editor.ReplaceNode(identifierQualifiedNameSyntax, qualifiedName);
                }

                // Else do nothing - already qualified as it should be.
            }
            else
            {
                editor.ReplaceNode(classDecl.Node, qualifiedName);
            }
        }

        private void FixForExtensionMethod(ISymbol symbol)
        {
            var staticUsingText = symbol.ContainingType.ToString();

            // Only add the static using once.
            if (staticUsings.Add(staticUsingText))
            {
                var staticUsing = SyntaxFactory.UsingDirective(SyntaxFactory.Token(SyntaxKind.StaticKeyword), null, SyntaxFactory.ParseName(staticUsingText));
                editor.InsertBefore(usingDirective, staticUsing);
            }
        }
    }
}
