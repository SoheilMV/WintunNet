using WintunNet;

Console.Title = "WintunNet Example";

Wintun wintun = new Wintun();
Guid adapterGuid = new Guid("b0527a9a-8740-4c13-af98-18e03bbdd5fb");
IntPtr adapter = wintun.CreateAdapter("WintunNet", "Wintun", ref adapterGuid);
IntPtr session = wintun.StartSession(adapter, 0x20000);

Console.WriteLine($"Adapter: {adapter}");
Console.WriteLine($"Session: {session}");

try
{
    // Attempt to receive a packet in a loop with a delay
    while (true)
    {
        // Receive the package
        byte[]? receivedPacket = wintun.ReceivePacket(session);
        if (receivedPacket != null)
        {
            Console.WriteLine($"Received packet of size: {receivedPacket.Length}");

            // Send package
            wintun.SendPacket(session, receivedPacket);
        }

        // Wait for a short period before the next attempt
        Thread.Sleep(10);
    }
}
catch
{
}
finally
{
    // End the session and close the adapter
    wintun.EndSession(session);
    wintun.CloseAdapter(adapter);
}