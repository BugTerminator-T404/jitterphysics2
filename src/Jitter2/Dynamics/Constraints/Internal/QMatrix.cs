/*
 * Jitter2 Physics Library
 * (c) Thorben Linneweber and contributors
 * SPDX-License-Identifier: MIT
 */

using System.Runtime.CompilerServices;
using Jitter2.LinearMath;
using Jitter2.Unmanaged;

[assembly: InternalsVisibleTo("JitterTests")]

namespace Jitter2.Dynamics.Constraints;

internal unsafe struct QMatrix
{
    private MemoryHelper.MemBlock16Real mem;

    public Real* Pointer => (Real*)Unsafe.AsPointer(ref mem);

    private static QMatrix Multiply(Real* left, Real* right)
    {
        Unsafe.SkipInit(out QMatrix res);
        Real* result = res.Pointer;

        for (int c = 0; c < 4; c++)
        {
            for (int r = 0; r < 4; r++)
            {
                Real* tt = &result[4 * c + r];
                *tt = 0;
                for (int k = 0; k < 4; k++)
                {
                    *tt += left[4 * k + r] * right[4 * c + k];
                }
            }
        }

        return res;
    }

    public JMatrix Projection()
    {
        Real* m = Pointer;

        return new JMatrix(m[0x5], m[0x9], m[0xD],
            m[0x6], m[0xA], m[0xE],
            m[0x7], m[0xB], m[0xF]);
    }

    public static QMatrix CreateLeftMatrix(in JQuaternion quat)
    {
        Unsafe.SkipInit(out QMatrix result);
        Real* q = result.Pointer;

        q[0x0] = +quat.W;
        q[0x4] = -quat.X;
        q[0x8] = -quat.Y;
        q[0xC] = -quat.Z;
        q[0x1] = +quat.X;
        q[0x5] = +quat.W;
        q[0x9] = -quat.Z;
        q[0xD] = +quat.Y;
        q[0x2] = +quat.Y;
        q[0x6] = +quat.Z;
        q[0xA] = +quat.W;
        q[0xE] = -quat.X;
        q[0x3] = +quat.Z;
        q[0x7] = -quat.Y;
        q[0xB] = +quat.X;
        q[0xF] = +quat.W;

        return result;
    }

    public static QMatrix CreateRightMatrix(in JQuaternion quat)
    {
        Unsafe.SkipInit(out QMatrix result);
        Real* q = result.Pointer;

        q[0x0] = +quat.W;
        q[0x4] = -quat.X;
        q[0x8] = -quat.Y;
        q[0xC] = -quat.Z;
        q[0x1] = +quat.X;
        q[0x5] = +quat.W;
        q[0x9] = +quat.Z;
        q[0xD] = -quat.Y;
        q[0x2] = +quat.Y;
        q[0x6] = -quat.Z;
        q[0xA] = +quat.W;
        q[0xE] = +quat.X;
        q[0x3] = +quat.Z;
        q[0x7] = +quat.Y;
        q[0xB] = -quat.X;
        q[0xF] = +quat.W;

        return result;
    }

    public static QMatrix Multiply(ref QMatrix left, ref QMatrix right)
    {
        fixed (QMatrix* lptr = &left)
        {
            fixed (QMatrix* rptr = &right)
            {
                return Multiply((Real*)lptr, (Real*)rptr);
            }
        }
    }

    public static JMatrix ProjectMultiplyLeftRight(in JQuaternion left, in JQuaternion right)
    {
        Unsafe.SkipInit(out JMatrix res);

        res.M11 = -left.X * right.X + left.W * right.W + left.Z * right.Z + left.Y * right.Y;
        res.M12 = -left.X * right.Y + left.W * right.Z - left.Z * right.W - left.Y * right.X;
        res.M13 = -left.X * right.Z - left.W * right.Y - left.Z * right.X + left.Y * right.W;
        res.M21 = -left.Y * right.X + left.Z * right.W - left.W * right.Z - left.X * right.Y;
        res.M22 = -left.Y * right.Y + left.Z * right.Z + left.W * right.W + left.X * right.X;
        res.M23 = -left.Y * right.Z - left.Z * right.Y + left.W * right.X - left.X * right.W;
        res.M31 = -left.Z * right.X - left.Y * right.W - left.X * right.Z + left.W * right.Y;
        res.M32 = -left.Z * right.Y - left.Y * right.Z + left.X * right.W - left.W * right.X;
        res.M33 = -left.Z * right.Z + left.Y * right.Y + left.X * right.X + left.W * right.W;

        return res;
    }
}