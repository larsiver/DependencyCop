using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Verify = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Liversen.DependencyCop.DescendantNamespaceAccessAnalyzer>;

namespace Liversen.DependencyCop
{
    public class DescendantNamespaceAccessAnalyzerTest
    {
        [Fact]
        async Task GivenCodeReferringCodeInDescendantNamespace_WhenAnalyzing_ThenDiagnostics()
        {
            var code = EmbeddedResourceHelpers.Get(Assembly.GetExecutingAssembly(), $"{GetType().FullName}Code.cs");
            var expected = Verify.Diagnostic()
                .WithLocation(5, 62)
                .WithMessage("Do not use type 'Info' from descendant namespace 'DescendantNamespaceAccessAnalyzer.Bank.Account'");

            await Verify.VerifyAnalyzerAsync(code, expected);
        }
    }
}