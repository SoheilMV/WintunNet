using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace WintunNet
{
    public class Wintun
    {
        private IPEndPoint? _localEndpoint;
        private IPEndPoint? _remoteEndpoint;

        public Wintun()
        {
        }

        public IntPtr CreateAdapter(string name, string tunnelType, ref Guid guid)
        {
            IntPtr adapter = Native.WintunCreateAdapter(name, tunnelType, ref guid);
            if (adapter == IntPtr.Zero)
            {
                throw new Exception("Failed to create Wintun adapter.");
            }
            return adapter;
        }

        public IntPtr StartSession(IntPtr adapter, int capacity)
        {
            IntPtr session = Native.WintunStartSession(adapter, capacity);
            if (session == IntPtr.Zero)
            {
                throw new Exception("Failed to start Wintun session.");
            }
            return session;
        }

        public void EndSession(IntPtr session)
        {
            Native.WintunEndSession(session);
        }

        public byte[]? ReceivePacket(IntPtr session)
        {
            int packetSize;
            IntPtr packet = Native.WintunReceivePacket(session, out packetSize);

            if (packet == IntPtr.Zero)
            {
                return null;
            }

            // Create byte array and copy data from IntPtr
            byte[] buffer = new byte[packetSize];
            Marshal.Copy(packet, buffer, 0, packetSize);

            // Release the packet after receiving the data
            Native.WintunReleaseReceivePacket(session, packet);

            return buffer;
        }

        public IntPtr AllocateSendPacket(IntPtr session, int packetSize)
        {
            return Native.WintunAllocateSendPacket(session, packetSize);
        }

        public void SendPacket(IntPtr session, byte[] data)
        {
            // Allocate the packet
            IntPtr packet = AllocateSendPacket(session, data.Length);

            // Copy the data to the packet
            Marshal.Copy(data, 0, packet, data.Length);

            // Send the packet
            Native.WintunSendPacket(session, packet);
        }

        public void ReleaseSendPacket(IntPtr session, IntPtr packet)
        {
            Native.WintunReleaseReceivePacket(session, packet);
        }

        public void CloseAdapter(IntPtr adapter)
        {
            Native.WintunCloseAdapter(adapter);
        }

        public void SetLocalEndpoint(IPEndPoint localEndpoint)
        {
            _localEndpoint = localEndpoint;
        }

        public void SetRemoteEndpoint(IPEndPoint remoteEndpoint)
        {
            _remoteEndpoint = remoteEndpoint;
        }

        public void ConnectNAT(IntPtr session)
        {
            if (_localEndpoint == null || _remoteEndpoint == null)
            {
                throw new Exception("Local and remote endpoints must be set before connecting.");
            }

            using (var udpClient = new UdpClient())
            {
                udpClient.Client.Bind(_localEndpoint);
                udpClient.Connect(_remoteEndpoint);

                // Send a dummy packet to establish the NAT connection
                udpClient.Send(new byte[1], 1);
            }
        }
    }
}