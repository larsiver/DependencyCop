using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Shouldly;
using Xunit;
#pragma warning disable CS0618 // Type or member is obsolete

namespace Liversen.DependencyCop.UsingNamespaceStatement.FixProviderScenarios
{
    public class TestExecutor
    {
        const string DisallowedNamespacePrefixesString = "UsingNamespaceStatementAnalyzer";

        static (string Name, string Value) GlobalConfig(string globalConfigPropertyNamePrefix) =>
            ("/.globalconfig", $"is_global = true{Environment.NewLine}{globalConfigPropertyNamePrefix}.DC1001_NamespacePrefixes = {DisallowedNamespacePrefixesString}");

        [Theory]
        [InlineData("SimpleTest")]
        [InlineData("ArrayTest")]
        [InlineData("ListTest")]
        [InlineData("GenericTest")]
        [InlineData("DoubleTest")]
        [InlineData("SubSpaceTest", ".SubSpace")]
        [InlineData("MethodReturn")]
        [InlineData("MethodParameter")]
        [InlineData("MethodParameterWithAttribute")]
        [InlineData("AsyncMethodReturn")]
        [InlineData("ExtensionMethod")]
        [InlineData("ExtensionMethodUsedTwice")]
        [InlineData("AlreadyFullyQualified")]
        async Task GivenCodeUsingDisallowedNamespace_WhenCodeFix_ThenExpectedResult(string testName, string optionalExtraNamespace = null)
        {
            var code = EmbeddedResourceHelpers.GetFromCallingAssembly($"{GetType().Namespace}.{testName}Code.cs");
            var expectedCode = EmbeddedResourceHelpers.GetFromCallingAssembly($"{GetType().Namespace}.{testName}FixedCode.cs");
            var expected = new DiagnosticResult("DC1001", DiagnosticSeverity.Warning)
                .WithLocation(1, 1)
                .WithMessage($"Do not use 'UsingNamespaceStatementAnalyzer.Account{optionalExtraNamespace}' in a using statement, use fully-qualified names");

            CSharpCodeFixTest<Analyzer, FixProvider, DefaultVerifier> test = new CSharpCodeFixTest<Analyzer, FixProvider, DefaultVerifier>()
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

            await Should.NotThrowAsync(async () =>
            {
                try
                {
                    await test.RunAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
        }

        [Fact]
        async Task GivenCodeUsingTwoDisallowedNamespaces_WhenCodeFix_ThenExpectedResult()
        {
            var code = EmbeddedResourceHelpers.GetFromCallingAssembly($"{GetType().Namespace}.TwoUsingStatementsCode.cs");
            var expectedCode = EmbeddedResourceHelpers.GetFromCallingAssembly($"{GetType().Namespace}.TwoUsingStatementsFixedCode.cs");
            var expected1 = new DiagnosticResult("DC1001", DiagnosticSeverity.Warning)
                .WithLocation(1, 1)
                .WithMessage($"Do not use 'UsingNamespaceStatementAnalyzer.Account' in a using statement, use fully-qualified names");
            var expected2 = new DiagnosticResult("DC1001", DiagnosticSeverity.Warning)
                .WithLocation(2, 1)
                .WithMessage($"Do not use 'UsingNamespaceStatementAnalyzer.Customer' in a using statement, use fully-qualified names");

            CSharpCodeFixTest<Analyzer, FixProvider, XUnitVerifier> test = new CSharpCodeFixTest<Analyzer, FixProvider, XUnitVerifier>()
            {
                TestState =
                {
                    Sources = { code },
                    MarkupHandling = MarkupMode.Allow,
                    AnalyzerConfigFiles = { GlobalConfig("dotnet_diagnostic") },
                    ExpectedDiagnostics = { expected1, expected2 }
                },

                FixedState =
                {
                    Sources = { expectedCode },
                    MarkupHandling = MarkupMode.Allow,
                    AnalyzerConfigFiles = { GlobalConfig("dotnet_diagnostic") }
                },
                NumberOfFixAllIterations = 2
            };

            await Should.NotThrowAsync(async () => await test.RunAsync());
        }
    }
}
