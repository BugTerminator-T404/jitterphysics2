#if NET6_0_OR_GREATER
global using NativeMemoryExt = System.Runtime.InteropServices.NativeMemory;
#else
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Runtime.InteropServices
{
    internal static class NativeMemoryExt
    {
        public unsafe static void AlignedFree(void* ptr)
        {
            if (ptr == null) return;
            IntPtr originalPtr = Marshal.ReadIntPtr((IntPtr)ptr - sizeof(IntPtr));
            Marshal.FreeHGlobal(originalPtr);
        }
        public unsafe static void Free(void* ptr)
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }

        public unsafe static void* Alloc(nuint byteCount)
        {
            return (byteCount == 0) ? null : (void*)Marshal.AllocHGlobal((IntPtr)(long)byteCount);
        }

        public unsafe static void* AlignedAlloc(nuint size, nuint alignment)
        {
            if (alignment == 0 || (alignment & (alignment - 1)) != 0)
            {
                throw new ArgumentException("Alignment must be power of two", nameof(alignment));
            }

            ulong totalSize = size + (alignment - 1) + (ulong)IntPtr.Size;

            if (IntPtr.Size == 4 && totalSize > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "Allocation size exceeds 2GB limit in 32-bit environment");
            }

            IntPtr originalPtr = Marshal.AllocHGlobal((IntPtr)(long)totalSize);

            ulong baseAddr = (ulong)originalPtr + (ulong)IntPtr.Size;
            ulong alignedAddr = (baseAddr + alignment - 1) & ~(alignment - 1);

            Marshal.WriteIntPtr((IntPtr)(alignedAddr - (ulong)IntPtr.Size), originalPtr);
            return (void*)alignedAddr;
        }
    }
}
#endif