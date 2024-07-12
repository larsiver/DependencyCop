using System.Threading.Tasks;
using Xunit;
using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<Liversen.DependencyCop.DescendantNamespaceAccessAnalyzer, Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace Liversen.DependencyCop
{
    public class DescendantNamespaceAccessAnalyzerTest
    {
        [Fact]
        async Task GivenCodeReferringCodeInDescendantNamespace_WhenAnalyzing_ThenDiagnostics()
        {
            var code = EmbeddedResourceHelpers.GetFromCallingAssembly($"{GetType().FullName}Code.cs");
            var expected = Verify.Diagnostic()
                .WithLocation(5, 62)
                .WithMessage("Do not use type 'Info' from descendant namespace 'DescendantNamespaceAccessAnalyzer.Bank.Account'");

            await Verify.VerifyAnalyzerAsync(code, expected);
        }
    }
}
