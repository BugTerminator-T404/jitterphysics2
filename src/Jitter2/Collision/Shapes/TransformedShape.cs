/*
 * Jitter2 Physics Library
 * (c) Thorben Linneweber and contributors
 * SPDX-License-Identifier: MIT
 */

using Jitter2.LinearMath;

namespace Jitter2.Collision.Shapes;

/// <summary>
/// Wraps any shape and allows to orientate and translate it.
/// </summary>
public class TransformedShape : RigidBodyShape
{
    private enum TransformationType
    {
        Identity,
        Rotation,
        General
    }

    private JVector translation;
    private JMatrix transformation;
    private TransformationType type;

    /// <summary>
    /// Constructs a transformed shape through an affine transformation define by
    /// a linear map and a translation.
    /// </summary>
    /// <param name="shape">The original shape which should be transformed.</param>
    /// <param name="translation">Shape is translated by this vector.</param>
    /// <param name="transform">A linear map (may include sheer and scale) of the transformation.</param>
    public TransformedShape(RigidBodyShape shape, in JVector translation, in JMatrix transform)
    {
        OriginalShape = shape;
        this.translation = translation;
        transformation = transform;

        AnalyzeTransformation();
        UpdateWorldBoundingBox();
    }

    public TransformedShape(RigidBodyShape shape, JVector translation) :
        this(shape, translation, JMatrix.Identity)
    {
    }

    public RigidBodyShape OriginalShape { get; }

    public JVector Translation
    {
        get => translation;
        set
        {
            translation = value;
            UpdateWorldBoundingBox();
        }
    }

    private void AnalyzeTransformation()
    {
        if (MathHelper.IsRotationMatrix(transformation))
        {
            JMatrix delta = transformation - JMatrix.Identity;
            type = MathHelper.UnsafeIsZero(ref delta) ? TransformationType.Identity : TransformationType.Rotation;
        }
        else
        {
            type = TransformationType.General;
        }
    }

    public JMatrix Transformation
    {
        get => transformation;
        set
        {
            transformation = value;
            AnalyzeTransformation();
            UpdateWorldBoundingBox();
        }
    }

    public override void SupportMap(in JVector direction, out JVector result)
    {
        if (type == TransformationType.Identity)
        {
            OriginalShape.SupportMap(direction, out result);
            result += translation;
        }
        else
        {
            JVector.TransposedTransform(direction, transformation, out JVector dir);
            OriginalShape.SupportMap(dir, out JVector sm);
            JVector.Transform(sm, transformation, out result);
            result += translation;
        }
    }

    public override void CalculateBoundingBox(in JQuaternion orientation, in JVector position, out JBoundingBox box)
    {
        if (type == TransformationType.General)
        {
            // just get the bounding box from the support map
            base.CalculateBoundingBox(orientation, position, out box);
        }
        else
        {
            JQuaternion quat = JQuaternion.CreateFromMatrix(transformation);
            OriginalShape.CalculateBoundingBox(orientation * quat,
                JVector.Transform(translation, orientation) + position, out box);
        }
    }

    public override void GetCenter(out JVector point)
    {
        OriginalShape.GetCenter(out point);
        point = JVector.Transform(point, transformation) + translation;
    }

    public override void CalculateMassInertia(out JMatrix inertia, out JVector com, out Real mass)
    {
        OriginalShape.CalculateMassInertia(out JMatrix originalInertia, out JVector originalCom, out mass);

        com = JVector.Transform(originalCom, transformation) + translation;
        inertia = transformation * JMatrix.Multiply(originalInertia, JMatrix.Transpose(transformation));
        JMatrix pat = mass * (JMatrix.Identity * translation.LengthSquared() - JVector.Outer(translation, translation));
        inertia += pat;
    }
}