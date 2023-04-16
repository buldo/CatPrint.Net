// See https://aka.ms/new-console-template for more information
using InTheHand.Bluetooth;

Console.WriteLine("Hello, World!");
var discoveredDevices = await Bluetooth.ScanForDevicesAsync();
foreach (var device in discoveredDevices)
{
    Console.WriteLine($"{device.Id} --- {device.Name} ---");
}
Console.WriteLine($"found {discoveredDevices?.Count} devices");
