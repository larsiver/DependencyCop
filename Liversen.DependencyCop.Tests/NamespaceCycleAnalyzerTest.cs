using Shouldly;
using Xunit;

namespace Liversen.DependencyCop
{
    public static class NamespaceCycleAnalyzerTest
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
    }
}
