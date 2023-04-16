// See https://aka.ms/new-console-template for more information
using InTheHand.Bluetooth;

Console.WriteLine("Hello, World!");

var isavailable = await Bluetooth.GetAvailabilityAsync();