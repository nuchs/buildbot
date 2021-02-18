using System;

namespace BuildBot.Eventing
{
    public record BuildVersion(int Major, int Minor, int Patch) : IComparable<BuildVersion>
    {
        public int CompareTo(BuildVersion other)
            => Major.CompareTo(other.Major) != 0 ? Major.CompareTo(other.Major) :
               Minor.CompareTo(other.Minor) != 0 ? Minor.CompareTo(other.Minor) :
               Patch.CompareTo(other.Patch);

        public override string ToString() => $"{Major}.{Minor}.{Patch}";
    }
}
