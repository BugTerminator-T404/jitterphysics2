/*
 * Jitter2 Physics Library
 * (c) Thorben Linneweber and contributors
 * SPDX-License-Identifier: MIT
 */

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jitter2.LinearMath;

/// <summary>
/// Quaternion Q = Xi + Yj + Zk + W. Uses Hamilton's definition of ij=k.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 4*sizeof(Real))]
public partial struct JQuaternion(Real x, Real y, Real z, Real w) : IEquatable<JQuaternion>
{
    [FieldOffset(0*sizeof(Real))] public Real X = x;
    [FieldOffset(1*sizeof(Real))] public Real Y = y;
    [FieldOffset(2*sizeof(Real))] public Real Z = z;
    [FieldOffset(3*sizeof(Real))] public Real W = w;

    /// <summary>
    /// Gets the identity quaternion (0, 0, 0, 1).
    /// </summary>
    public static JQuaternion Identity => new(0, 0, 0, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="JQuaternion"/> struct.
    /// </summary>
    /// <param name="w">The W component.</param>
    /// <param name="v">The vector component.</param>
    public JQuaternion(Real w, in JVector v) : this(v.X, v.Y, v.Z, w)
    {
    }

    /// <summary>
    /// Adds two quaternions.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <returns>The sum of the two quaternions.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JQuaternion Add(in JQuaternion quaternion1, in JQuaternion quaternion2)
    {
        Add(in quaternion1, in quaternion2, out JQuaternion result);
        return result;
    }

    /// <summary>
    /// Adds two quaternions.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <param name="result">When the method completes, contains the sum of the two quaternions.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Add(in JQuaternion quaternion1, in JQuaternion quaternion2, out JQuaternion result)
    {
        result.X = quaternion1.X + quaternion2.X;
        result.Y = quaternion1.Y + quaternion2.Y;
        result.Z = quaternion1.Z + quaternion2.Z;
        result.W = quaternion1.W + quaternion2.W;
    }

    /// <summary>
    /// Returns the conjugate of a quaternion.
    /// </summary>
    /// <param name="value">The quaternion to conjugate.</param>
    /// <returns>The conjugate of the quaternion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JQuaternion Conjugate(in JQuaternion value)
    {
        JQuaternion quaternion;
        quaternion.X = -value.X;
        quaternion.Y = -value.Y;
        quaternion.Z = -value.Z;
        quaternion.W = value.W;
        return quaternion;
    }

    /// <summary>
    /// Returns the conjugate of the quaternion.
    /// </summary>
    /// <returns>The conjugate of the quaternion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly JQuaternion Conjugate()
    {
        JQuaternion quaternion;
        quaternion.X = -X;
        quaternion.Y = -Y;
        quaternion.Z = -Z;
        quaternion.W = W;
        return quaternion;
    }

    /// <summary>
    /// Returns a string that represents the current quaternion.
    /// </summary>
    public readonly override string ToString()
    {
        return $"X={X:F6}, Y={Y:F6}, Z={Z:F6}, W={W:F6}";
    }

    /// <summary>
    /// Subtracts one quaternion from another.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <returns>The difference of the two quaternions.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JQuaternion Subtract(in JQuaternion quaternion1, in JQuaternion quaternion2)
    {
        Subtract(quaternion1, quaternion2, out JQuaternion result);
        return result;
    }

    /// <summary>
    /// Subtracts one quaternion from another.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <param name="result">When the method completes, contains the difference of the two quaternions.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Subtract(in JQuaternion quaternion1, in JQuaternion quaternion2, out JQuaternion result)
    {
        result.X = quaternion1.X - quaternion2.X;
        result.Y = quaternion1.Y - quaternion2.Y;
        result.Z = quaternion1.Z - quaternion2.Z;
        result.W = quaternion1.W - quaternion2.W;
    }

    /// <summary>
    /// Multiplies two quaternions.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <returns>The product of the two quaternions.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JQuaternion Multiply(in JQuaternion quaternion1, in JQuaternion quaternion2)
    {
        Multiply(quaternion1, quaternion2, out JQuaternion result);
        return result;
    }

    /// <summary>
    /// Calculates the transformation of (1,0,0)^T by the quaternion.
    /// </summary>
    /// <returns>The transformed vector.</returns>
    public readonly JVector GetBasisX()
    {
        JVector result;

        result.X = (Real)1.0 - (Real)2.0 * (Y * Y + Z * Z);
        result.Y = (Real)2.0 * (X * Y + Z * W);
        result.Z = (Real)2.0 * (X * Z - Y * W);

        return result;
    }

    /// <summary>
    /// Calculates the transformation of (0,1,0)^T by the quaternion.
    /// </summary>
    /// <returns>The transformed vector.</returns>
    public readonly JVector GetBasisY()
    {
        JVector result;

        result.X = (Real)2.0 * (X * Y - Z * W);
        result.Y = (Real)1.0 - (Real)2.0 * (X * X + Z * Z);
        result.Z = (Real)2.0 * (Y * Z + X * W);

        return result;
    }

    /// <summary>
    /// Calculates the transformation of (0,0,1)^T by the quaternion.
    /// </summary>
    /// <returns>The transformed vector.</returns>
    public readonly JVector GetBasisZ()
    {
        JVector result;

        result.X = (Real)2.0 * (X * Z + Y * W);
        result.Y = (Real)2.0 * (Y * Z - X * W);
        result.Z = (Real)1.0 - (Real)2.0 * (X * X + Y * Y);

        return result;
    }

    /// <summary>
    /// Creates a quaternion representing a rotation around the X-axis.
    /// </summary>
    /// <param name="radians">The angle of rotation in radians.</param>
    /// <returns>A quaternion representing the rotation.</returns>
    public static JQuaternion CreateRotationX(Real radians)
    {
        Real halfAngle = radians * (Real)0.5;
        (Real sha, Real cha) = MathR.SinCos(halfAngle);
        return new JQuaternion(sha, 0, 0, cha);
    }

    /// <summary>
    /// Creates a quaternion representing a rotation around the Y-axis.
    /// </summary>
    /// <param name="radians">The angle of rotation in radians.</param>
    /// <returns>A quaternion representing the rotation.</returns>
    public static JQuaternion CreateRotationY(Real radians)
    {
        Real halfAngle = radians * (Real)0.5;
        (Real sha, Real cha) = MathR.SinCos(halfAngle);
        return new JQuaternion(0, sha, 0, cha);
    }

    /// <summary>
    /// Creates a quaternion representing a rotation around the Z-axis.
    /// </summary>
    /// <param name="radians">The angle of rotation in radians.</param>
    /// <returns>A quaternion representing the rotation.</returns>
    public static JQuaternion CreateRotationZ(Real radians)
    {
        Real halfAngle = radians * (Real)0.5;
        (Real sha, Real cha) = MathR.SinCos(halfAngle);
        return new JQuaternion(0, 0, sha, cha);
    }

    /// <summary>
    /// Creates a Quaternion from a <b>unit</b> vector and an angle to rotate about the vector.
    /// </summary>
    /// <param name="axis">The unit vector to rotate around.</param>
    /// <param name="angle">The angle of rotation.</param>
    public static JQuaternion CreateFromAxisAngle(in JVector axis, JAngle angle)
    {
        Real halfAngle = (Real)angle * (Real)0.5;
        (Real s, Real c) = MathR.SinCos(halfAngle);
        return new JQuaternion(axis.X * s, axis.Y * s, axis.Z * s, c);
    }

    /// <summary>Converts a <b>unit</b> quaternion to axis–angle form.</summary>
    /// <remarks>
    /// Assumes <paramref name="quaternion"/> is already normalised.
    /// The returned <paramref name="angle"/> is clamped to the shortest arc [0 , π].
    /// </remarks>
    /// <param name="quaternion">Unit quaternion to decompose.</param>
    /// <param name="axis">Receives the unit rotation axis.</param>
    /// <param name="angle">Receives the rotation angle (radians).</param>
    public static void ToAxisAngle(JQuaternion quaternion, out JVector axis, out Real angle)
    {
        Real s = MathR.Sqrt(MathR.Max((Real)0.0, (Real)1.0 - quaternion.W * quaternion.W));

        const Real epsilonSingularity = (Real)1e-6;

        if (s < epsilonSingularity)
        {
            angle = (Real)0.0;
            axis = JVector.UnitX; // Default to X-axis for infinitesimal rotations
            return;
        }

        Real invS = (Real)1.0 / s;
        axis = new JVector(quaternion.X * invS, quaternion.Y * invS, quaternion.Z * invS);
        angle = (Real)2.0 * MathR.Acos(quaternion.W);

        // Enforce the shortest-arc representation (angle between 0 and PI radians).
        if (angle > MathR.PI)
        {
            angle = (Real)2.0 * MathR.PI - angle;
            axis  = -axis;
        }
    }

    /// <summary>
    /// Multiplies two quaternions.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <param name="result">When the method completes, contains the product of the two quaternions.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(in JQuaternion quaternion1, in JQuaternion quaternion2, out JQuaternion result)
    {
        Real r1 = quaternion1.W;
        Real i1 = quaternion1.X;
        Real j1 = quaternion1.Y;
        Real k1 = quaternion1.Z;

        Real r2 = quaternion2.W;
        Real i2 = quaternion2.X;
        Real j2 = quaternion2.Y;
        Real k2 = quaternion2.Z;

        result.W = r1 * r2 - (i1 * i2 + j1 * j2 + k1 * k2);
        result.X = r1 * i2 + r2 * i1 + j1 * k2 - k1 * j2;
        result.Y = r1 * j2 + r2 * j1 + k1 * i2 - i1 * k2;
        result.Z = r1 * k2 + r2 * k1 + i1 * j2 - j1 * i2;
    }

    /// <summary>
    /// Calculates quaternion1* \times quaternion2.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <param name="result">When the method completes, contains the product of the conjugate of the first quaternion and the second quaternion.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ConjugateMultiply(in JQuaternion quaternion1, in JQuaternion quaternion2, out JQuaternion result)
    {
        Real r1 = quaternion1.W;
        Real i1 = -quaternion1.X;
        Real j1 = -quaternion1.Y;
        Real k1 = -quaternion1.Z;

        Real r2 = quaternion2.W;
        Real i2 = quaternion2.X;
        Real j2 = quaternion2.Y;
        Real k2 = quaternion2.Z;

        result.W = r1 * r2 - (i1 * i2 + j1 * j2 + k1 * k2);
        result.X = r1 * i2 + r2 * i1 + j1 * k2 - k1 * j2;
        result.Y = r1 * j2 + r2 * j1 + k1 * i2 - i1 * k2;
        result.Z = r1 * k2 + r2 * k1 + i1 * j2 - j1 * i2;
    }

    /// <summary>
    /// Calculates quaternion1* \times quaternion2.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JQuaternion ConjugateMultiply(in JQuaternion quaternion1, in JQuaternion quaternion2)
    {
        ConjugateMultiply(quaternion1, quaternion2, out JQuaternion result);
        return result;
    }

    /// <summary>
    /// Calculates quaternion1 \times quaternion2*.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <param name="result">When the method completes, contains the product of the first quaternion and the conjugate of the second quaternion.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MultiplyConjugate(in JQuaternion quaternion1, in JQuaternion quaternion2, out JQuaternion result)
    {
        Real r1 = quaternion1.W;
        Real i1 = quaternion1.X;
        Real j1 = quaternion1.Y;
        Real k1 = quaternion1.Z;

        Real r2 = quaternion2.W;
        Real i2 = -quaternion2.X;
        Real j2 = -quaternion2.Y;
        Real k2 = -quaternion2.Z;

        result.W = r1 * r2 - (i1 * i2 + j1 * j2 + k1 * k2);
        result.X = r1 * i2 + r2 * i1 + j1 * k2 - k1 * j2;
        result.Y = r1 * j2 + r2 * j1 + k1 * i2 - i1 * k2;
        result.Z = r1 * k2 + r2 * k1 + i1 * j2 - j1 * i2;
    }

    /// <summary>
    /// Calculates quaternion1 \times quaternion2*.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JQuaternion MultiplyConjugate(in JQuaternion quaternion1, in JQuaternion quaternion2)
    {
        MultiplyConjugate(quaternion1, quaternion2, out JQuaternion result);
        return result;
    }

    /// <summary>
    /// Multiplies a quaternion by a scalar factor.
    /// </summary>
    /// <param name="quaternion1">The quaternion to multiply.</param>
    /// <param name="scaleFactor">The scalar factor.</param>
    /// <returns>The scaled quaternion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JQuaternion Multiply(in JQuaternion quaternion1, Real scaleFactor)
    {
        Multiply(in quaternion1, scaleFactor, out JQuaternion result);
        return result;
    }

    /// <summary>
    /// Multiplies a quaternion by a scalar factor.
    /// </summary>
    /// <param name="quaternion1">The quaternion to multiply.</param>
    /// <param name="scaleFactor">The scalar factor.</param>
    /// <param name="result">When the method completes, contains the scaled quaternion.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(in JQuaternion quaternion1, Real scaleFactor, out JQuaternion result)
    {
        result.W = quaternion1.W * scaleFactor;
        result.X = quaternion1.X * scaleFactor;
        result.Y = quaternion1.Y * scaleFactor;
        result.Z = quaternion1.Z * scaleFactor;
    }

    /// <summary>
    /// Calculates the length of the quaternion.
    /// </summary>
    /// <returns>The length of the quaternion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Real Length()
    {
        return (Real)Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Real LengthSquared()
    {
        return X * X + Y * Y + Z * Z + W * W;
    }

    /// <summary>
    /// Normalizes the quaternion to unit length.
    /// </summary>
    [Obsolete($"In-place Normalize() is deprecated; " +
              $"use the static {nameof(JQuaternion.Normalize)} method or {nameof(JQuaternion.NormalizeInPlace)}.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Normalize()
    {
        Real num2 = X * X + Y * Y + Z * Z + W * W;
        Real num = (Real)1.0 / MathR.Sqrt(num2);
        X *= num;
        Y *= num;
        Z *= num;
        W *= num;
    }

    public static void NormalizeInPlace(ref JQuaternion quaternion)
    {
        Real num2 = quaternion.LengthSquared();
        Real num = (Real)1.0 / MathR.Sqrt(num2);
        quaternion.X *= num;
        quaternion.Y *= num;
        quaternion.Z *= num;
        quaternion.W *= num;
    }

    /// <inheritdoc cref="Normalize()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Normalize(in JQuaternion value, out JQuaternion result)
    {
        Real num2 = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;
        Real num = (Real)1.0 / MathR.Sqrt(num2);
        result.X = value.X * num;
        result.Y = value.Y * num;
        result.Z = value.Z * num;
        result.W = value.W * num;
    }

    /// <inheritdoc cref="Normalize()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JQuaternion Normalize(in JQuaternion value)
    {
        Normalize(value, out JQuaternion result);
        return result;
    }

    /// <summary>
    /// Creates a quaternion from a rotation matrix.
    /// </summary>
    /// <param name="matrix">The rotation matrix.</param>
    /// <returns>A quaternion representing the rotation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JQuaternion CreateFromMatrix(in JMatrix matrix)
    {
        CreateFromMatrix(matrix, out JQuaternion result);
        return result;
    }

    /// <summary>
    /// Creates a quaternion from a rotation matrix.
    /// </summary>
    /// <param name="matrix">The rotation matrix.</param>
    /// <param name="result">When the method completes, contains the quaternion representing the rotation.</param>
    public static void CreateFromMatrix(in JMatrix matrix, out JQuaternion result)
    {
        Real t;

        if (matrix.M33 < 0)
        {
            if (matrix.M11 > matrix.M22)
            {
                t = (Real)1.0 + matrix.M11 - matrix.M22 - matrix.M33;
                result = new JQuaternion(t, matrix.M21 + matrix.M12, matrix.M31 + matrix.M13, matrix.M32 - matrix.M23);
            }
            else
            {
                t = (Real)1.0 - matrix.M11 + matrix.M22 - matrix.M33;
                result = new JQuaternion(matrix.M21 + matrix.M12, t, matrix.M32 + matrix.M23, matrix.M13 - matrix.M31);
            }
        }
        else
        {
            if (matrix.M11 < -matrix.M22)
            {
                t = (Real)1.0 - matrix.M11 - matrix.M22 + matrix.M33;
                result = new JQuaternion(matrix.M13 + matrix.M31, matrix.M32 + matrix.M23, t, matrix.M21 - matrix.M12);
            }
            else
            {
                t = (Real)1.0 + matrix.M11 + matrix.M22 + matrix.M33;
                result = new JQuaternion(matrix.M32 - matrix.M23, matrix.M13 - matrix.M31, matrix.M21 - matrix.M12, t);
            }
        }

        t = (Real)(0.5d / Math.Sqrt(t));
        result.X *= t;
        result.Y *= t;
        result.Z *= t;
        result.W *= t;
    }

    /// <summary>
    /// Multiplies two quaternions.
    /// </summary>
    /// <param name="value1">The first quaternion.</param>
    /// <param name="value2">The second quaternion.</param>
    /// <returns>The product of the two quaternions.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JQuaternion operator *(in JQuaternion value1, in JQuaternion value2)
    {
        Multiply(value1, value2, out JQuaternion result);
        return result;
    }

    /// <summary>
    /// Multiplies a quaternion by a scalar factor.
    /// </summary>
    /// <param name="value1">The scalar factor.</param>
    /// <param name="value2">The quaternion to multiply.</param>
    /// <returns>The scaled quaternion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JQuaternion operator *(Real value1, in JQuaternion value2)
    {
        Multiply(value2, value1, out JQuaternion result);
        return result;
    }

    /// <summary>
    /// Multiplies a quaternion by a scalar factor.
    /// </summary>
    /// <param name="value1">The quaternion to multiply.</param>
    /// <param name="value2">The scalar factor.</param>
    /// <returns>The scaled quaternion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JQuaternion operator *(in JQuaternion value1, Real value2)
    {
        Multiply(value1, value2, out JQuaternion result);
        return result;
    }

    /// <summary>
    /// Adds two quaternions.
    /// </summary>
    /// <param name="value1">The first quaternion.</param>
    /// <param name="value2">The second quaternion.</param>
    /// <returns>The sum of the two quaternions.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JQuaternion operator +(in JQuaternion value1, in JQuaternion value2)
    {
        Add(value1, value2, out JQuaternion result);
        return result;
    }

    /// <summary>
    /// Subtracts one quaternion from another.
    /// </summary>
    /// <param name="value1">The first quaternion.</param>
    /// <param name="value2">The second quaternion.</param>
    /// <returns>The result of subtracting the second quaternion from the first.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JQuaternion operator -(in JQuaternion value1, in JQuaternion value2)
    {
        Subtract(value1, value2, out JQuaternion result);
        return result;
    }

    public readonly bool Equals(JQuaternion other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
    }

    public readonly override bool Equals(object? obj)
    {
        return obj is JQuaternion other && Equals(other);
    }

    public readonly override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

    public static bool operator ==(JQuaternion left, JQuaternion right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(JQuaternion left, JQuaternion right)
    {
        return !(left == right);
    }
}