// See https://aka.ms/new-console-template for more information
using InTheHand.Bluetooth;

Console.WriteLine("Hello, World!");

// My device name is GB03

var discoveredDevices = await Bluetooth.ScanForDevicesAsync(new RequestDeviceOptions() { AcceptAllDevices = true });

var device = discoveredDevices.FirstOrDefault(device => string.Equals(device.Name, "GB03"));

if (device == null)
{
    Console.WriteLine("No device");
    return;
}

if (!device.Gatt.IsConnected)
{
    await device.Gatt.ConnectAsync();
}

var services = await device.Gatt.GetPrimaryServicesAsync();

var serviceUuid = BluetoothUuid.FromGuid(Guid.Parse("0000ae30-0000-1000-8000-00805f9b34fb"));
var txCharacteristicUuid = BluetoothUuid.FromGuid(Guid.Parse("0000ae01-0000-1000-8000-00805f9b34fb"));
var rxCharacteristicUuid = BluetoothUuid.FromGuid(Guid.Parse("0000ae02-0000-1000-8000-00805f9b34fb"));

var service = services.FirstOrDefault(srv => srv.Uuid == serviceUuid);
if (service == null)
{
    Console.WriteLine("No expected Service");
    return;
}

var txCharacteristic = await service.GetCharacteristicAsync(txCharacteristicUuid);
var rxCharacteristic = await service.GetCharacteristicAsync(rxCharacteristicUuid);


Console.WriteLine("Ready for work");
Console.ReadLine();