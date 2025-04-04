using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Liversen.DependencyCop
{
    /// <summary>
    /// Used to store a violating <see cref="TypeSyntax"/> along with the namespace it is declared in (so we can identify the common namespace).
    /// </summary>
    public class TypeDeclaration
    {
        public TypeDeclaration(string @namespace, TypeSyntax node)
        {
            NameSpace = @namespace;
            Node = node;
        }

        public string NameSpace { get; set; }

        public TypeSyntax Node { get; set; }
    }
}
