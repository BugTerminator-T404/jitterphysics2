#if NET6_0_OR_GREATER
global using InterlockedExt = System.Threading.Interlocked;
#else
using System.Runtime.CompilerServices;
namespace System.Threading
{
    public static class InterlockedExt
    {
        public unsafe static ulong Add(ref ulong location1, ulong value)
        {

            long longValue = (long)value;
            long result = Interlocked.Add(
                ref Unsafe.As<ulong, long>(ref location1),
                longValue
            );
            return Unsafe.As<long, ulong>(ref result);
        }

        public static ulong Increment(ref ulong location)
        {
            return Add(ref location, 1UL);
        }

    }
}
#endif