#if NET6_0_OR_GREATER

#if USE_DOUBLE_PRECISION
global using MathRExt = System.Math;
#else
global using MathRExt = System.MathF;
#endif

global using doubleExt = double;
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

    public static class doubleExt
    {
        public static double Min(double left, double right) => Math.Min(left, right);
    }
}
#endif