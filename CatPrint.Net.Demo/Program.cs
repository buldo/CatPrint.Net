// See https://aka.ms/new-console-template for more information

using CatPrint.Net;
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

var printer = new Printer(device);
await printer.InitAsync();

var commandsFactory = new CommandsFactory();
await printer.SendAsync(commandsFactory.CreateFeedPaper(80));

Console.WriteLine("Ready for work");
Console.ReadLine();


public class Printer
{
    private const int MTU = 248;
    private readonly BluetoothDevice _device;
    private GattCharacteristic _txCharacteristic;

    public Printer(BluetoothDevice device)
    {
        _device = device;
    }

    public async Task InitAsync()
    {
        if (!_device.Gatt.IsConnected)
        {
            await _device.Gatt.ConnectAsync();
        }

        var services = await _device.Gatt.GetPrimaryServicesAsync();

        var serviceUuid = BluetoothUuid.FromGuid(Guid.Parse("0000ae30-0000-1000-8000-00805f9b34fb"));
        var txCharacteristicUuid = BluetoothUuid.FromGuid(Guid.Parse("0000ae01-0000-1000-8000-00805f9b34fb"));
        var rxCharacteristicUuid = BluetoothUuid.FromGuid(Guid.Parse("0000ae02-0000-1000-8000-00805f9b34fb"));

        var service = services.FirstOrDefault(srv => srv.Uuid == serviceUuid);
        if (service == null)
        {
            Console.WriteLine("No expected Service");
            return;
        }

        _txCharacteristic = await service.GetCharacteristicAsync(txCharacteristicUuid);
        var rxCharacteristic = await service.GetCharacteristicAsync(rxCharacteristicUuid);
    }

    public async Task SendAsync(Command command)
    {
        await SendBytes(command.AsBytes());
    }

    private async Task SendBytes(byte[] command)
    {
        // 4 bytes required for L2CAP header
        var chunks = command.Chunk(MTU - 4);
        foreach (var chunk in chunks)
        {
            await _txCharacteristic.WriteValueWithoutResponseAsync(chunk);
        }
    }
}