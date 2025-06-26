#if NET6_0_OR_GREATER
global using MathRExt = System.MathF;
#else
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
namespace Jitter2.LinearMath
{
    public static class MathRExt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Real sin, float cos) SinCos(Real angle)
        {
            return (MathR.Sin(angle), MathR.Cos(angle));
        }
    }

}
#endif