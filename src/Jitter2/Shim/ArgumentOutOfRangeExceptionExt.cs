
#if NET6_0_OR_GREATER
global using ArgumentOutOfRangeExceptionExt = System.ArgumentOutOfRangeException;
#else
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class ArgumentOutOfRangeExceptionExt
    {
        public static void ThrowIfNegative(float value, string? paramName = null) {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, "Value cannot be negative.");
            }

        }

        public static void ThrowIfNegativeOrZero(float value, string paramName)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, "Value cannot be negative or zero");
            }
        }

        public static void ThrowIfLessThan(float left, float right, string? paramName = null)
        {
            if (left < right)
            {
                throw new ArgumentOutOfRangeException(paramName, $"Value cannot be less than {right}.");
            }
        }

        public static void ThrowIfLessThanOrEqual(float left, float right, string? paramName = null)
        {
            if (left <= right)
            {
                throw new ArgumentOutOfRangeException(paramName, $"Value cannot be less than {right}.");
            }
        }


        public static void ThrowIfGreaterThan(float left, float right, string? paramName = null)
        {
            if (left > right)
            {
                throw new ArgumentOutOfRangeException(paramName, $"Value cannot be greater than {right}.");
            }
        }

        public static void ThrowIfGreaterThanOrEqual(float left, float right, string? paramName = null)
        {
            if (left >= right)
            {
                throw new ArgumentOutOfRangeException(paramName, $"Value cannot be greater than {right} or equal.");
            }
        }
        
    }
}
#endif