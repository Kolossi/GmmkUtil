using Device.Net;
using Hid.Net.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usb.Net.Windows;

namespace GmmkUtil
{
    internal interface IKeyboard
    {
        IDevice Device { get; set; }
    }

    internal class Keyboard : IKeyboard
    {
        public IDevice Device { get; set; }

        public Keyboard()
        {
            DebugTracer tracer = new DebugTracer();
            DebugLogger logger = new DebugLogger();

            WindowsUsbDeviceFactory.Register(logger, tracer);
            WindowsHidDeviceFactory.Register(logger, tracer);
        }
    }

    internal static class KeyboardMethods
    {
        public const string SET_PROFILE = "04dd03042c00000055aaff02450c2f650001{0:x2}08000000000102030405060807090b0a0c0d0e0f10111214000000000000000000000000000000000000000000";

        // Todo!
        //public const string SET_KEY_COLOUR = "0403031103030200{0:x2}{1:x2}{2:x2}0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";

        internal static async Task<IKeyboard> Connect(this IKeyboard keyboard)
        {
            List<FilterDeviceDefinition> deviceDefinitions = null;
            deviceDefinitions = new List<FilterDeviceDefinition>
                {
                    new FilterDeviceDefinition{ DeviceType= DeviceType.Hid, VendorId= 3141, UsagePage=65308 }
                };

            var devices = await DeviceManager.Current.GetDevicesAsync(deviceDefinitions);
            keyboard.Device = devices.FirstOrDefault();
            await keyboard.Device.InitializeAsync();
            return keyboard;
        }

        internal static async Task<byte[]> Send(this IKeyboard keyboard, string hexString)
        {
            var writeBuffer = hexString.ToByteArray();

            return await keyboard.Device.WriteAndReadAsync(writeBuffer);
        }

        public static async Task<byte[]> SetProfile(this IKeyboard keyboard, int profileNum)
        {
            if (profileNum < 1 || profileNum > 3) throw new ArgumentOutOfRangeException("profileNum", "Must be 1-3 only");
            var writeBuffer = string.Format(SET_PROFILE, profileNum - 1).ToByteArray();

            //Write the data to the device
            return await keyboard.Device.WriteAndReadAsync(writeBuffer);
        }

#if DEBUG
        // although the keyboard isn't use, demanding one ensures factories have been initialised
        public static void ShowAllDevices(this IKeyboard keyboard)
        {
            var devices = DeviceManager.Current.GetConnectedDeviceDefinitionsAsync(null).Result;
            Console.WriteLine("Currently connected devices: ");
            foreach (var device in devices)
            {

                Console.WriteLine(device.DeviceId);
                if (device == null) continue;
                Console.WriteLine($"    manu={device.Manufacturer}|prod={device.ProductName}|label={device.Label}|sn={device.SerialNumber}|usagePage={device.UsagePage}|usage={device.Usage}|vers={device.VersionNumber}|prodId={device.ProductId}|vend={device.VendorId}|rbuff={device.ReadBufferSize}|wbuff={device.WriteBufferSize}");
            }
        }
#endif
    }
}
