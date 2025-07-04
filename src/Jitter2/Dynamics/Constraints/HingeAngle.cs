/*
 * Jitter2 Physics Library
 * (c) Thorben Linneweber and contributors
 * SPDX-License-Identifier: MIT
 */

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Jitter2.LinearMath;
using Jitter2.Unmanaged;

namespace Jitter2.Dynamics.Constraints;

/// <summary>
/// Constrains two bodies to only allow rotation around a specified axis, removing two angular degrees of freedom, or three if a limit is enforced.
/// </summary>
public unsafe class HingeAngle : Constraint
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HingeAngleData
    {
        internal int _internal;
        public delegate*<ref ConstraintData, void> Iterate;
        public delegate*<ref ConstraintData, Real, void> PrepareForIteration;

        public JHandle<RigidBodyData> Body1;
        public JHandle<RigidBodyData> Body2;

        public Real MinAngle;
        public Real MaxAngle;

        public Real BiasFactor;
        public Real LimitBias;

        public Real LimitSoftness;
        public Real Softness;

        public JVector Axis;
        public JQuaternion Q0;

        public JVector AccumulatedImpulse;
        public JVector Bias;

        public JMatrix EffectiveMass;
        public JMatrix Jacobian;

        public ushort Clamp;
    }

    private JHandle<HingeAngleData> handle;

    protected override void Create()
    {
        CheckDataSize<HingeAngleData>();

        Iterate = &IterateHingeAngle;
        PrepareForIteration = &PrepareForIterationHingeAngle;
        handle = JHandle<ConstraintData>.AsHandle<HingeAngleData>(Handle);
    }

    /// <summary>
    /// Initializes the constraint.
    /// </summary>
    /// <param name="axis">Axis in world space for which relative angular movement is allowed.</param>
    public void Initialize(JVector axis, AngularLimit limit)
    {
        ref HingeAngleData data = ref handle.Data;
        ref RigidBodyData body1 = ref data.Body1.Data;
        ref RigidBodyData body2 = ref data.Body2.Data;

        data.Softness = (Real)0.001;
        data.LimitSoftness = (Real)0.001;
        data.BiasFactor = (Real)0.2;
        data.LimitBias = (Real)0.1;

        data.MinAngle = MathR.Sin((Real)limit.From / (Real)2.0);
        data.MaxAngle = MathR.Sin((Real)limit.To / (Real)2.0);

        data.Axis = JVector.ConjugatedTransform(axis, body2.Orientation);

        JQuaternion q1 = body1.Orientation;
        JQuaternion q2 = body2.Orientation;

        data.Q0 = q2.Conjugate() * q1;
    }

    public AngularLimit Limit
    {
        set
        {
            ref HingeAngleData data = ref handle.Data;
            data.MinAngle = MathR.Sin((Real)value.From / (Real)2.0);
            data.MaxAngle = MathR.Sin((Real)value.To / (Real)2.0);
        }
    }

    public static void PrepareForIterationHingeAngle(ref ConstraintData constraint, Real idt)
    {
        ref HingeAngleData data = ref Unsafe.AsRef<HingeAngleData>(Unsafe.AsPointer(ref constraint));

        ref RigidBodyData body1 = ref data.Body1.Data;
        ref RigidBodyData body2 = ref data.Body2.Data;

        JQuaternion q1 = body1.Orientation;
        JQuaternion q2 = body2.Orientation;

        JVector p0 = MathHelper.CreateOrthonormal(data.Axis);
        JVector p1 = data.Axis % p0;

        JQuaternion quat0 = data.Q0 * q1.Conjugate() * q2;

        JVector error;
        error.X = JVector.Dot(p0, new JVector(quat0.X, quat0.Y, quat0.Z));
        error.Y = JVector.Dot(p1, new JVector(quat0.X, quat0.Y, quat0.Z));
        error.Z = JVector.Dot(data.Axis, new JVector(quat0.X, quat0.Y, quat0.Z));

        data.Clamp = 0;

        JMatrix m0 = (-(Real)(1.0 / 2.0)) * QMatrix.ProjectMultiplyLeftRight(data.Q0 * q1.Conjugate(), q2);

        if (quat0.W < (Real)0.0)
        {
            error *= -(Real)1.0;
            m0 *= -(Real)1.0;
        }

        data.Jacobian.UnsafeGet(0) = JVector.TransposedTransform(p0, m0);
        data.Jacobian.UnsafeGet(1) = JVector.TransposedTransform(p1, m0);
        data.Jacobian.UnsafeGet(2) = JVector.TransposedTransform(data.Axis, m0);

        data.EffectiveMass = JMatrix.TransposedMultiply(data.Jacobian, JMatrix.Multiply(body1.InverseInertiaWorld + body2.InverseInertiaWorld, data.Jacobian));

        data.EffectiveMass.M11 += data.Softness * idt;
        data.EffectiveMass.M22 += data.Softness * idt;
        data.EffectiveMass.M33 += data.LimitSoftness * idt;

        Real maxA = data.MaxAngle;
        Real minA = data.MinAngle;

        if (error.Z > maxA)
        {
            data.Clamp = 1;
            error.Z -= maxA;
        }
        else if (error.Z < minA)
        {
            data.Clamp = 2;
            error.Z -= minA;
        }
        else
        {
            data.AccumulatedImpulse.Z = 0;
            data.EffectiveMass.M33 = 1;
            data.EffectiveMass.M31 = data.EffectiveMass.M13 = 0;
            data.EffectiveMass.M32 = data.EffectiveMass.M23 = 0;

            // TODO: do he have to set them to zero here, explicitly?
            //       does this also has to be done in PointOnLine?
            data.Jacobian.M13 = data.Jacobian.M23 = data.Jacobian.M33 = 0;
        }

        JMatrix.Inverse(data.EffectiveMass, out data.EffectiveMass);

        data.Bias = error * idt;
        data.Bias.X *= data.BiasFactor;
        data.Bias.Y *= data.BiasFactor;
        data.Bias.Z *= data.LimitBias;

        body1.AngularVelocity += JVector.Transform(JVector.Transform(data.AccumulatedImpulse, data.Jacobian), body1.InverseInertiaWorld);
        body2.AngularVelocity -= JVector.Transform(JVector.Transform(data.AccumulatedImpulse, data.Jacobian), body2.InverseInertiaWorld);
    }

    public JAngle Angle
    {
        get
        {
            ref HingeAngleData data = ref handle.Data;
            JQuaternion q1 = data.Body1.Data.Orientation;
            JQuaternion q2 = data.Body2.Data.Orientation;

            JQuaternion quat0 = data.Q0 * q1.Conjugate() * q2;

            if (quat0.W < (Real)0.0)
            {
                quat0 *= -(Real)1.0;
            }

            Real error = JVector.Dot(data.Axis, new JVector(quat0.X, quat0.Y, quat0.Z));
            return (JAngle)((Real)2.0 * MathR.Asin(error));
        }
    }

    public Real Softness
    {
        get => handle.Data.Softness;
        set => handle.Data.Softness = value;
    }

    public Real LimitSoftness
    {
        get => handle.Data.LimitSoftness;
        set => handle.Data.LimitSoftness = value;
    }

    public Real Bias
    {
        get => handle.Data.BiasFactor;
        set => handle.Data.BiasFactor = value;
    }

    public Real LimitBias
    {
        get => handle.Data.LimitBias;
        set => handle.Data.LimitBias = value;
    }

    public JVector Impulse => handle.Data.AccumulatedImpulse;

    public static void IterateHingeAngle(ref ConstraintData constraint, Real idt)
    {
        ref HingeAngleData data = ref Unsafe.AsRef<HingeAngleData>(Unsafe.AsPointer(ref constraint));
        ref RigidBodyData body1 = ref constraint.Body1.Data;
        ref RigidBodyData body2 = ref constraint.Body2.Data;

        JVector jv = JVector.TransposedTransform(body1.AngularVelocity - body2.AngularVelocity, data.Jacobian);

        JVector softness = data.AccumulatedImpulse * idt;
        softness.X *= data.Softness;
        softness.Y *= data.Softness;
        softness.Z *= data.LimitSoftness;

        JVector lambda = -(Real)1.0 * JVector.Transform(jv + data.Bias + softness, data.EffectiveMass);

        JVector origAcc = data.AccumulatedImpulse;

        data.AccumulatedImpulse += lambda;

        if (data.Clamp == 1)
        {
            data.AccumulatedImpulse.Z = MathR.Min(0, data.AccumulatedImpulse.Z);
        }
        else if (data.Clamp == 2)
        {
            data.AccumulatedImpulse.Z = MathR.Max(0, data.AccumulatedImpulse.Z);
        }
        else
        {
            origAcc.Z = 0;
            data.AccumulatedImpulse.Z = 0;
        }

        lambda = data.AccumulatedImpulse - origAcc;

        body1.AngularVelocity += JVector.Transform(JVector.Transform(lambda, data.Jacobian), body1.InverseInertiaWorld);
        body2.AngularVelocity -= JVector.Transform(JVector.Transform(lambda, data.Jacobian), body2.InverseInertiaWorld);
    }
}