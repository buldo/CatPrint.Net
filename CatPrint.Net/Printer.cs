using InTheHand.Bluetooth;

namespace CatPrint.Net;

public class Printer
{
    private const int Mtu = 248;

    private static readonly BluetoothUuid ServiceUuid = BluetoothUuid.FromGuid(Guid.Parse("0000ae30-0000-1000-8000-00805f9b34fb"));
    private static readonly BluetoothUuid TxCharacteristicUuid = BluetoothUuid.FromGuid(Guid.Parse("0000ae01-0000-1000-8000-00805f9b34fb"));
    private static readonly BluetoothUuid RxCharacteristicUuid = BluetoothUuid.FromGuid(Guid.Parse("0000ae02-0000-1000-8000-00805f9b34fb"));

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

        var service = services.FirstOrDefault(srv => srv.Uuid == ServiceUuid);
        if (service == null)
        {
            Console.WriteLine("No expected Service");
            return;
        }

        _txCharacteristic = await service.GetCharacteristicAsync(TxCharacteristicUuid);
        var rxCharacteristic = await service.GetCharacteristicAsync(RxCharacteristicUuid);
    }

    public async Task SendAsync(Command command)
    {
        await SendBytes(command.AsBytes());
    }

    private async Task SendBytes(byte[] command)
    {
        // 4 bytes required for L2CAP header
        var chunks = command.Chunk(Mtu - 4);
        foreach (var chunk in chunks)
        {
            await _txCharacteristic.WriteValueWithoutResponseAsync(chunk);
        }
    }
}