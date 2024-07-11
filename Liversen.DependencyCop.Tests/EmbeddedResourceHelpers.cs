using System;
using System.IO;
using System.Reflection;

namespace Liversen.DependencyCop
{
    static class EmbeddedResourceHelpers
    {
        public static string Get(Assembly assembly, string resourceName)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new ArgumentException($"Unable to find resource {resourceName} in assembly {assembly.FullName}");
            }

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Get the resource from the currently executing assembly that called this method.
        /// </summary>
        /// <param name="resourceName">Usually just the class name that should be tested.</param>
        /// <returns>The content of the resource as text.</returns>
        public static string Get(string resourceName)
        {
            return Get(Assembly.GetCallingAssembly(), resourceName);
        }
    }
}
