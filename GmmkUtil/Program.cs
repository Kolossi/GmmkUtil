using Device.Net;
using Hid.Net.Windows;
using System;
using System.Collections.Generic;
using Usb.Net.Windows;

namespace GmmkUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            DebugTracer tracer = new DebugTracer();
            DebugLogger logger = new DebugLogger();
            WindowsUsbDeviceFactory.Register(logger, tracer);
            WindowsHidDeviceFactory.Register(logger, tracer);
            while (true)
            {
                List<FilterDeviceDefinition> deviceDefinitions = null;
                deviceDefinitions = new List<FilterDeviceDefinition>
                {
                    new FilterDeviceDefinition{ DeviceType= DeviceType.Hid, VendorId= 3141, UsagePage=12 }
                };

                //Get the first available device and connect to it
                var devices = DeviceManager.Current.GetDevicesAsync(deviceDefinitions).Result;
                //var devices = DeviceManager.Current.GetConnectedDeviceDefinitionsAsync(null).Result;
                Console.WriteLine("Currently connected devices: ");
                foreach (var device in devices)
                {

                    Console.WriteLine(device.DeviceId);
                    //var dev = device;
                    var dev = device.ConnectedDeviceDefinition;
                    if (dev == null) continue;
                    Console.WriteLine($"    {dev.Manufacturer}|{dev.ProductName}|{dev.Label}|{dev.SerialNumber}|{dev.Usage}|{dev.UsagePage}|{dev.VersionNumber}|{dev.ProductId}|{dev.VendorId}");
                }
                Console.WriteLine();

                //Console.ReadLine();
            }
        }
    }
}
