using Device.Net;
using Hid.Net.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            //ShowAllDevices();
            var device = ConnectToKeyboard().Result;

            SetProfile(device, 3);
            Console.ReadLine();
            SetProfile(device, 2);
            Console.ReadLine();
            SetProfile(device, 1);
            Console.ReadLine();
        }

        public const string SET_PROFILE = "04dd03042c00000055aaff02450c2f650001{0:x2}08000000000102030405060807090b0a0c0d0e0f10111214000000000000000000000000000000000000000000";

        public const string SET_KEY_COLOUR = "0403031103030200{0:x2}{1:x2}{2:x2}0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";

        public static async Task<byte[]> WriteAndReadFromDeviceAsync(IDevice device, string hexString)
        {
            var writeBuffer = StringToByteArray(hexString);

            //Write the data to the device
            return await device.WriteAndReadAsync(writeBuffer);
        }

        public static async Task<byte[]> SetProfile(IDevice device, int profileNum)
        {
            if (profileNum < 1 || profileNum > 3) throw new ArgumentOutOfRangeException("profileNum", "Must be 1-3 only");
            var writeBuffer = StringToByteArray(string.Format(SET_PROFILE, profileNum-1));

            //Write the data to the device
            return await device.WriteAndReadAsync(writeBuffer);
        }

        // thanks: https://stackoverflow.com/a/321404/2738122
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        static async Task<IDevice> ConnectToKeyboard()
        {
            List<FilterDeviceDefinition> deviceDefinitions = null;
            deviceDefinitions = new List<FilterDeviceDefinition>
                {
                    new FilterDeviceDefinition{ DeviceType= DeviceType.Hid, VendorId= 3141, UsagePage=65308 }
                };

            var devices = await DeviceManager.Current.GetDevicesAsync(deviceDefinitions);
            var device = devices.FirstOrDefault();
            await device.InitializeAsync();
            return device;

        }

        static void ShowAllDevices()
        {
            var devices = DeviceManager.Current.GetConnectedDeviceDefinitionsAsync(null).Result;
            Console.WriteLine("Currently connected devices: ");
            foreach (var device in devices)
            {

                Console.WriteLine(device.DeviceId);
                if (device == null) continue;
                Console.WriteLine($"    {device.Manufacturer}|{device.ProductName}|{device.Label}|{device.SerialNumber}|{device.Usage}|{device.UsagePage}|{device.VersionNumber}|{device.ProductId}|{device.VendorId}|{device.WriteBufferSize}");
            }


        }
    }
}
