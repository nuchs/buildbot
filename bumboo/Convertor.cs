namespace Bumboo
{
    using System;
    using Version = GrpcCommon.v1.Version;

    internal static class Convertor
    {
        internal static Version ToVersion(this string source)
        {
            var parts = source.Split(".");

            if (parts.Length == 3 &&
                int.TryParse(parts[0], out int major) &&
                int.TryParse(parts[1], out int minor) &&
                int.TryParse(parts[2], out int patch))
            {
                return new Version
                {
                    Major = major,
                    Minor = minor,
                    Patch = patch,
                };
            }
            else
            {
                throw new ArgumentException($"Invalid version, must be in <major>.<minor>.<patch> format e.g. 1.2.3, actual value : {source}");
            }
        }
    }
}
