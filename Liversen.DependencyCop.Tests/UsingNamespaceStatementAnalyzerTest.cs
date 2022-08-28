using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Shouldly;
using Xunit;

namespace Liversen.DependencyCop
{
    public class UsingNamespaceStatementAnalyzerTest
    {
        const string DisallowedNamespacePrefixesString = "UsingNamespaceStatementAnalyzer";

        static (string Name, string Value) GlobalConfig(string globalConfigPropertyNamePrefix) =>
            ("/.globalconfig", $"is_global = true{Environment.NewLine}{globalConfigPropertyNamePrefix}.DC1001_NamespacePrefixes = {DisallowedNamespacePrefixesString}");

        [Theory]
        [InlineData("dotnet_diagnostic")]
        [InlineData("build_property")]
        async Task GivenCodeUsingDisallowedNamespacesConfiguredInGlobalConfig_WhenAnalyzing_ThenDiagnostics(string globalConfigPropertyNamePrefix)
        {
            var code = EmbeddedResourceHelpers.Get(Assembly.GetExecutingAssembly(), $"{GetType().FullName}Code.cs");
            var expected = new DiagnosticResult("DC1001", DiagnosticSeverity.Warning)
                .WithLocation(1, 1)
                .WithMessage("Do not use 'UsingNamespaceStatementAnalyzer.Account' in a using statement, use fully-qualified names");
            var test = new CSharpAnalyzerTest<UsingNamespaceStatementAnalyzer, XUnitVerifier>
            {
                TestState =
                {
                    Sources = { code },
                    ExpectedDiagnostics = { expected },
                    AnalyzerConfigFiles = { GlobalConfig(globalConfigPropertyNamePrefix) }
                }
            };

            await Should.NotThrowAsync(() => test.RunAsync());
        }

        [Fact]
        async Task GivenCodeWithGlobalConfigWithoutDisallowedNamespaces_WhenAnalyzing_ThenDiagnostics()
        {
            var code = EmbeddedResourceHelpers.Get(Assembly.GetExecutingAssembly(), $"{GetType().FullName}Code.cs");
            var expected = new DiagnosticResult("DC1004", DiagnosticSeverity.Warning)
                .WithMessage("A list of disallowed namespaces must be configured for rule DC1001");
            var test = new CSharpAnalyzerTest<UsingNamespaceStatementAnalyzer, XUnitVerifier>
            {
                TestState =
                {
                    Sources = { code },
                    ExpectedDiagnostics = { expected }
                }
            };

            await Should.NotThrowAsync(() => test.RunAsync());
        }
    }
}
