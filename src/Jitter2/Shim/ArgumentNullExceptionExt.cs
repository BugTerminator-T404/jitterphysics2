
#if NET6_0_OR_GREATER
global using ArgumentNullExceptionExt = System.ArgumentNullException;
#else
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    internal class ArgumentNullExceptionExt
    {
        public static void ThrowIfNull(object o, string? name = null)
        {
            if (o == null)
            {
                throw new System.ArgumentNullException(name); // hard to do it differently without C# 10's features
            }
        }
    }
}
#endif