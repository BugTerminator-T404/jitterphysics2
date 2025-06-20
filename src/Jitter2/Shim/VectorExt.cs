
#if NET7_0_OR_GREATER
global using VectorExt = System.Runtime.Intrinsics.Vector128;
#else
using System;
using System.Runtime.CompilerServices;
namespace Jitter2.LinearMath
{

    public static class VectorExt
    {
        public static VectorReal Create(float x = 0f, float y = 0f, float z = 0f, float w = 0f)
        {
            return new VectorReal(x, y, z, w);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static float GetElement(this VectorReal value, int element)
        {
            if ((uint)element >= 4) throw new IndexOutOfRangeException();
            return ((float*)Unsafe.AsPointer(ref value))[element];
        }

        public static VectorReal Create(float x = 0f)
        {
            return new VectorReal(x);
        }
        public static bool IsHardwareAccelerated => false;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorReal LessThanOrEqual(VectorReal left, VectorReal right)
        {
            const int mask = -1;
            return new VectorReal(
                left.X <= right.X ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f,
                left.Y <= right.Y ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f,
                left.Z <= right.Z ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f,
                left.W <= right.W ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f
            );
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorReal LessThan(VectorReal left, VectorReal right)
        {
            const int mask = -1;
            return new VectorReal(
                left.X < right.X ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f,
                left.Y < right.Y ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f,
                left.Z < right.Z ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f,
                left.W < right.W ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f
            );
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorReal GreaterThanOrEqual(VectorReal left, VectorReal right)
        {
            const int mask = -1;
            return new VectorReal(
                left.X >= right.X ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f,
                left.Y >= right.Y ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f,
                left.Z >= right.Z ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f,
                left.W >= right.W ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f
            );
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorReal GreaterThan(VectorReal left, VectorReal right)
        {
            const int mask = -1;
            return new VectorReal(
                left.X > right.X ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f,
                left.Y > right.Y ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f,
                left.Z > right.Z ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f,
                left.W > right.W ? Unsafe.As<int, float>(ref Unsafe.AsRef(mask)) : 0f
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
        public static VectorReal LoadUnsafe(ref float source)
        {
            ref byte address = ref Unsafe.As<float, byte>(ref source);
            return Unsafe.ReadUnaligned<VectorReal>(ref address);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll(VectorReal left, VectorReal right, float tolerance = 1e-6f)
        {
            // 恢复带容差的精确比较
            return MathF.Abs(left.X - right.X) <= tolerance &&
                MathF.Abs(left.Y - right.Y) <= tolerance &&
                MathF.Abs(left.Z - right.Z) <= tolerance &&
                MathF.Abs(left.W - right.W) <= tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorReal AsInt32(this VectorReal vector4)
        {
            return new VectorReal(
                Unsafe.As<float, int>(ref vector4.X),
                Unsafe.As<float, int>(ref vector4.Y),
                Unsafe.As<float, int>(ref vector4.Z),
                Unsafe.As<float, int>(ref vector4.W)
            );
        }
    }
}
#endif