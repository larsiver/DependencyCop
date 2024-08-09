using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Shouldly;
using Xunit;
#pragma warning disable CS0618 // Type or member is obsolete

namespace Liversen.DependencyCop
{
    public class UsingNamespaceStatementFixTest
    {
        const string DisallowedNamespacePrefixesString = "UsingNamespaceStatementAnalyzer";

        static (string Name, string Value) GlobalConfig(string globalConfigPropertyNamePrefix) =>
            ("/.globalconfig", $"is_global = true{Environment.NewLine}{globalConfigPropertyNamePrefix}.DC1001_NamespacePrefixes = {DisallowedNamespacePrefixesString}");

        [Fact]
        async Task GivenCodeUsingDisallowedNamespace_WhenCodeFix_ThenExpectedResult()
        {
            var code = EmbeddedResourceHelpers.GetFromCallingAssembly($"{GetType().FullName}Code.cs");
            var expectedCode = EmbeddedResourceHelpers.GetFromCallingAssembly($"{GetType().FullName}ExpectedFixedCode.cs");
            var expected = new DiagnosticResult("DC1001", DiagnosticSeverity.Warning)
                .WithLocation(1, 1)
                .WithMessage("Do not use 'UsingNamespaceStatementAnalyzer.Account' in a using statement, use fully-qualified names");

            CSharpCodeFixTest<UsingNamespaceStatementAnalyzer, UsingNamespaceStatementCodeFixProvider, XUnitVerifier> test = new CSharpCodeFixTest<UsingNamespaceStatementAnalyzer, UsingNamespaceStatementCodeFixProvider, XUnitVerifier>()
            {
                TestState =
                {
                    Sources = { code },
                    MarkupHandling = MarkupMode.Allow,
                    AnalyzerConfigFiles = { GlobalConfig("dotnet_diagnostic") },
                    ExpectedDiagnostics = { expected }
                },

                FixedState =
                {
                    Sources = { expectedCode },
                    MarkupHandling = MarkupMode.Allow,
                    AnalyzerConfigFiles = { GlobalConfig("dotnet_diagnostic") }
                },
            };

            await Should.NotThrowAsync(() => test.RunAsync());
        }
    }
}
