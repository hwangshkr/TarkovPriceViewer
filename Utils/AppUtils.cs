using System;
using System.Reflection;

namespace TarkovPriceViewer.Utils
{
    public static class AppUtils
    {
        public static string GetVersion()
        {
            var assembly = typeof(AppUtils).Assembly;
            // First, try to read the informational version, which in SDK-style projects
            // corresponds to the <Version> in the csproj (including suffixes such as -beta.1).
            var infoVersion = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;
            if (!string.IsNullOrWhiteSpace(infoVersion))
            {
                // Trim build metadata suffix (e.g. +commit-hash) if present
                var plusIndex = infoVersion.IndexOf('+');
                if (plusIndex >= 0)
                {
                    infoVersion = infoVersion.Substring(0, plusIndex);
                }

                return $"v{infoVersion}";
            }

            // Fallback: use classic AssemblyVersion if no informational version is defined
            var version = assembly.GetName().Version;
            if (version == null) return "v1.0";

            // If patch is 0, use major.minor (e.g., v2.0)
            // Otherwise use major.minor.patch (e.g., v2.0.1)
            return version.Build == 0
                ? $"v{version.Major}.{version.Minor}"
                : $"v{version.Major}.{version.Minor}.{version.Build}";
        }

        public static string GetVersionWithoutPrefix()
        {
            var tag = GetVersion();
            return string.IsNullOrEmpty(tag) ? tag : tag.TrimStart('v', 'V');
        }
    }
}
