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

await printer.SendAsync(commandsFactory.CreateSetQuality(0x32));
await printer.SendAsync(commandsFactory.CreateSetMode(Mode.Image));

using Image image = Image.Load("Lenna.png");
image.Mutate(x => x
    .BinaryThreshold((float)0.5));
image.Save("out.png");
var l8Image = image.CloneAs<L8>();
var imageBytes = new byte[l8Image.Width * l8Image.Height];
l8Image.CopyPixelDataTo(imageBytes);
var imageLines = imageBytes.Chunk(l8Image.Width);
foreach (var imageLine in imageLines)
{
    var line = imageLine.Select(b => b != 255).ToList();
    //var command = commandsFactory.CreatePrintLine(line);
    var command = commandsFactory.CreatePrintLineCompressed(line);
    await printer.SendAsync(command);
}


await printer.SendAsync(commandsFactory.CreateFeedPaper(80));

Console.WriteLine("Done");
Console.ReadLine();