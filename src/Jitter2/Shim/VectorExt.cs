
#if NET7_0_OR_GREATER
global using VectorExt = System.Runtime.Intrinsics.Vector128;
#else
using System;
using System.Runtime.CompilerServices;
namespace Jitter2.LinearMath
{

    public static class VectorExt
    {
        public static VectorReal Create(Real x = 0, Real y = 0, Real z = 0, Real w = 0)
        {
            return new VectorReal(x, y, z, w);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static Real GetElement(this VectorReal value, int element)
        {
            if ((uint)element >= 4) throw new IndexOutOfRangeException();
            return ((Real*)Unsafe.AsPointer(ref value))[element];
        }

        public static VectorReal Create(Real x = 0)
        {
            return new VectorReal(x);
        }
        public static bool IsHardwareAccelerated => false;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorReal LessThanOrEqual(VectorReal left, VectorReal right)
        {
            const int mask = -1;
            return new VectorReal(
                left.X <= right.X ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0,
                left.Y <= right.Y ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0,
                left.Z <= right.Z ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0,
                left.W <= right.W ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0
            );
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorReal LessThan(VectorReal left, VectorReal right)
        {
            const int mask = -1;
            return new VectorReal(
                left.X < right.X ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0,
                left.Y < right.Y ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0,
                left.Z < right.Z ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0,
                left.W < right.W ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0
            );
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorReal GreaterThanOrEqual(VectorReal left, VectorReal right)
        {
            const int mask = -1;
            return new VectorReal(
                left.X >= right.X ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0,
                left.Y >= right.Y ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0,
                left.Z >= right.Z ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0,
                left.W >= right.W ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0
            );
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorReal GreaterThan(VectorReal left, VectorReal right)
        {
            const int mask = -1;
            return new VectorReal(
                left.X > right.X ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0,
                left.Y > right.Y ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0,
                left.Z > right.Z ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0,
                left.W > right.W ? Unsafe.As<int, Real>(ref Unsafe.AsRef(mask)) : 0
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static VectorReal BitwiseAnd(VectorReal left, VectorReal right)
        {

            uint* l = (uint*)&left;
            uint* r = (uint*)&right;
            return new VectorReal(
                BitConverter.ToSingle(BitConverter.GetBytes(l[0] & r[0])),
                BitConverter.ToSingle(BitConverter.GetBytes(l[1] & r[1])),
                BitConverter.ToSingle(BitConverter.GetBytes(l[2] & r[2])),
                BitConverter.ToSingle(BitConverter.GetBytes(l[3] & r[3]))
            );

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static VectorReal BitwiseOr(VectorReal left, VectorReal right)
        {
            uint* l = (uint*)&left;
            uint* r = (uint*)&right;
            return new VectorReal(
                BitConverter.ToSingle(BitConverter.GetBytes(l[0] | r[0])),
                BitConverter.ToSingle(BitConverter.GetBytes(l[1] | r[1])),
                BitConverter.ToSingle(BitConverter.GetBytes(l[2] | r[2])),
                BitConverter.ToSingle(BitConverter.GetBytes(l[3] | r[3]))
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorReal LoadUnsafe(ref Real source)
        {
            ref byte address = ref Unsafe.As<Real, byte>(ref source);
            return Unsafe.ReadUnaligned<VectorReal>(ref address);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll(VectorReal left, VectorReal right, Real tolerance = (Real)1e-6)
        {
            // 恢复带容差的精确比较
            return MathR.Abs(left.X - right.X) <= tolerance &&
                MathR.Abs(left.Y - right.Y) <= tolerance &&
                MathR.Abs(left.Z - right.Z) <= tolerance &&
                MathR.Abs(left.W - right.W) <= tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorReal AsInt32(this VectorReal vector4)
        {
            return new VectorReal(
                Unsafe.As<Real, int>(ref vector4.X),
                Unsafe.As<Real, int>(ref vector4.Y),
                Unsafe.As<Real, int>(ref vector4.Z),
                Unsafe.As<Real, int>(ref vector4.W)
            );
        }
    }
}
#endif