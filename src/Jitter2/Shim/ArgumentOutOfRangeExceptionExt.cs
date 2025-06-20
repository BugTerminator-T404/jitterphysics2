
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
        public static void ThrowIfNegative(float value, string paramName) {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, "Value cannot be negative.");
            }

        }

        public static void ThrowIfNegativeOrZero(float value, string paramName)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, "Value must be greater than zero.");
            }
        }

        public static void ThrowIfGreaterThan(float value, float maxValue, string paramName)
        {
            if (value > maxValue)
            {
                throw new ArgumentOutOfRangeException(paramName, $"Value cannot be greater than {maxValue}.");
            }
        }
    }
}
#endif