using System;
using System.Reflection;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Verify = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Liversen.DependencyCop.NamespaceCycleAnalyzer>;

namespace Liversen.DependencyCop
{
    public class NamespaceCycleAnalyzerTest
    {
        [Fact]
        static void GivenUnrelatedNamespaces_WhenReducingNamespaces_ThenSame() =>
            NamespaceCycleAnalyzer.GetReducedNamespaces("Foo", "Bar")
                .ShouldBe(("Foo", "Bar"));

        [Fact]
        static void GivenNamespacesWithSameRoot_WhenReducingNamespaces_ThenReduced() =>
            NamespaceCycleAnalyzer.GetReducedNamespaces("DC.Foo", "DC.Bar")
                .ShouldBe(("DC.Foo", "DC.Bar"));

        [Fact]
        static void GivenDeepNamespacesWithCommonRoot_WhenReducingNamespaces_ThenReduced() =>
            NamespaceCycleAnalyzer.GetReducedNamespaces("DC.Foo.Qwerty", "DC.Bar.Qwerty")
                .ShouldBe(("DC.Foo", "DC.Bar"));

        [Fact]
        static void GivenOneNamespaceChildOfAnotherNamespace_WhenReducingNamespaces_ThenEmptyNamespaces() =>
            NamespaceCycleAnalyzer.GetReducedNamespaces("DC.Foo.Bar", "DC.Foo")
                .ShouldBe((string.Empty, string.Empty));

        // It is non-deterministic which of two different diagnostics is returned.
        [Fact]
        async Task GivenCodeWithCycle_WhenAnalyzing_ThenDiagnostics()
        {
            var code = EmbeddedResourceHelpers.Get($"{GetType().FullName}Code.cs");
            var expected1 = Verify.Diagnostic()
                .WithLocation(14, 28)
                .WithMessage("Break up namespace cycle 'NamespaceCycleAnalyzer.Transaction->NamespaceCycleAnalyzer.Account->NamespaceCycleAnalyzer.Transaction'");
            var expected2 = Verify.Diagnostic()
                .WithLocation(22, 24)
                .WithMessage("Break up namespace cycle 'NamespaceCycleAnalyzer.Account->NamespaceCycleAnalyzer.Transaction->NamespaceCycleAnalyzer.Account'");

            try
            {
                await Verify.VerifyAnalyzerAsync(code, expected1);
            }
            catch (Exception)
            {
                await Verify.VerifyAnalyzerAsync(code, expected2);
            }
        }
    }
}
