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

        [Fact]
        async Task GivenCodeWithCycle_WhenAnalyzing_ThenDc1003Diagnostics()
        {
            var code = EmbeddedResourceHelpers.Get(Assembly.GetExecutingAssembly(), $"{GetType().FullName}Code.cs");
            var expected = Verify.Diagnostic()
                .WithLocation(14, 28)
                .WithMessage("Break up namespace cycle 'NamespaceCycleAnalyzer.Transaction->NamespaceCycleAnalyzer.Account->NamespaceCycleAnalyzer.Transaction'");

            await Verify.VerifyAnalyzerAsync(code, expected);
        }
    }
}