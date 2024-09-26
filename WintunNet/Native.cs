using System.Runtime.InteropServices;

namespace WintunNet
{
    internal static class Native
    {
        // Load the DLL manually
        private static IntPtr hModule = LoadLibrary(DetermineWintunDll());

        // Declare delegates for each function
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public delegate IntPtr WintunCreateAdapterDelegate(string name, string description, ref Guid guid);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WintunCloseAdapterDelegate(IntPtr adapter);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr WintunStartSessionDelegate(IntPtr adapter, int capacity);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WintunEndSessionDelegate(IntPtr session);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr WintunReceivePacketDelegate(IntPtr session, out int packetSize);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WintunReleaseReceivePacketDelegate(IntPtr session, IntPtr packet);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr WintunAllocateSendPacketDelegate(IntPtr session, int packetSize);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WintunSendPacketDelegate(IntPtr session, IntPtr packet);

        public static WintunCreateAdapterDelegate WintunCreateAdapter = Marshal.GetDelegateForFunctionPointer<WintunCreateAdapterDelegate>(GetProcAddress(hModule, "WintunCreateAdapter"));
        public static WintunCloseAdapterDelegate WintunCloseAdapter = Marshal.GetDelegateForFunctionPointer<WintunCloseAdapterDelegate>(GetProcAddress(hModule, "WintunCloseAdapter"));
        public static WintunStartSessionDelegate WintunStartSession = Marshal.GetDelegateForFunctionPointer<WintunStartSessionDelegate>(GetProcAddress(hModule, "WintunStartSession"));
        public static WintunEndSessionDelegate WintunEndSession = Marshal.GetDelegateForFunctionPointer<WintunEndSessionDelegate>(GetProcAddress(hModule, "WintunEndSession"));
        public static WintunReceivePacketDelegate WintunReceivePacket = Marshal.GetDelegateForFunctionPointer<WintunReceivePacketDelegate>(GetProcAddress(hModule, "WintunReceivePacket"));
        public static WintunReleaseReceivePacketDelegate WintunReleaseReceivePacket = Marshal.GetDelegateForFunctionPointer<WintunReleaseReceivePacketDelegate>(GetProcAddress(hModule, "WintunReleaseReceivePacket"));
        public static WintunAllocateSendPacketDelegate WintunAllocateSendPacket = Marshal.GetDelegateForFunctionPointer<WintunAllocateSendPacketDelegate>(GetProcAddress(hModule, "WintunAllocateSendPacket"));
        public static WintunSendPacketDelegate WintunSendPacket = Marshal.GetDelegateForFunctionPointer<WintunSendPacketDelegate>(GetProcAddress(hModule, "WintunSendPacket"));

        // Helper functions to load the library and get function pointers
        public static string DetermineWintunDll()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                    return $"{Environment.CurrentDirectory}/wintun/arm64/wintun.dll";
                else
                    return $"{Environment.CurrentDirectory}/wintun/amd64/wintun.dll";
            }
            else
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.Arm)
                    return $"{Environment.CurrentDirectory}/wintun/arm/wintun.dll";
                else
                    return $"{Environment.CurrentDirectory}/wintun/x86/wintun.dll";
            }
        }

        public static IntPtr LoadLibrary(string dllToLoad)
        {
            IntPtr handle = NativeMethods.LoadLibrary(dllToLoad);
            if (handle == IntPtr.Zero)
            {
                throw new Exception($"Failed to load library: {dllToLoad}");
            }
            return handle;
        }

        public static IntPtr GetProcAddress(IntPtr hModule, string procedureName)
        {
            IntPtr procAddress = NativeMethods.GetProcAddress(hModule, procedureName);
            if (procAddress == IntPtr.Zero)
            {
                throw new Exception($"Failed to get function pointer for {procedureName}");
            }
            return procAddress;
        }

        internal static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr LoadLibrary(string lpFileName);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        }
    }
}
