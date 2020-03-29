using Device.Net;
using EasyLife;
using Hid.Net.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Usb.Net.Windows;

namespace GmmkUtil.GmmkLib
{
    public interface IKeyboard
    {
        IDevice Device { get; set; }

        DeviceListener DeviceListener { get; set; }

        bool Connected { get; set; }

        int DefaultProfile { get; set; }

        void Initialized(object sender, DeviceEventArgs e);
        void Disconnected(object sender, DeviceEventArgs e);
    }

    public class Keyboard : IKeyboard
    {
#if (LIBUSB)
        public const int POLL_MSECS = 6000;
#else
        public const int POLL_MSECS = 3000;
#endif

        public IDevice Device { get; set; }

        public DeviceListener DeviceListener { get; set; }
        public bool Connected { get; set; }

        public int DefaultProfile { get; set; }

        public Keyboard()
        {
            DebugTracer tracer = new DebugTracer();
            DebugLogger logger = new DebugLogger();

            WindowsUsbDeviceFactory.Register(logger, tracer);
            WindowsHidDeviceFactory.Register(logger, tracer);
        }

        public void Initialized(object sender, DeviceEventArgs e)
        {
            //if (Connected) return;
            "Initialized Enter".ConsoleLogLine();
            var device = e.Device;
            $"Setting {DefaultProfile}".ConsoleLogLine();
            var defResponse = device.SetProfile(DefaultProfile)?.Result;
            "Connected = true".ConsoleLogLine();
            Connected = true;
            "Initialized Exit".ConsoleLogLine();
        }

        public void Disconnected(object sender, DeviceEventArgs e)
        {
            "Connected Enter".ConsoleLogLine();
            "Connected = true".ConsoleLogLine();
            Connected = false;
            "Disconnected Exit".ConsoleLogLine();
        }
    }

    public static class KeyboardMethods
    {
        public const string SET_PROFILE = "04dd03042c00000055aaff02450c2f650001{0:x2}08000000000102030405060807090b0a0c0d0e0f10111214000000000000000000000000000000000000000000";

        // Todo!
        //public const string SET_KEY_COLOUR = "0403031103030200{0:x2}{1:x2}{2:x2}0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";

        public static async Task<IKeyboard> Connect(this IKeyboard keyboard)
        {
            List<FilterDeviceDefinition> deviceDefinitions = GetFilterDeviceDefinitions();

            var devices = await DeviceManager.Current.GetDevicesAsync(deviceDefinitions);
            keyboard.Device = devices.FirstOrDefault();
            await keyboard.Device.InitializeAsync();
            return keyboard;
        }

        private static List<FilterDeviceDefinition> GetFilterDeviceDefinitions()
        {
            return new List<FilterDeviceDefinition>
                {
                    new FilterDeviceDefinition{ DeviceType= DeviceType.Hid, VendorId= 3141, UsagePage=65308 }
                };
        }

        public static async Task<byte[]> Send(this IKeyboard keyboard, string hexString)
        {
            var writeBuffer = hexString.ToByteArray();

            return await keyboard.Device.WriteAndReadAsync(writeBuffer);
        }

        public static async Task<byte[]> SetProfile(this IDevice device, int profileNum)
        {
            if (profileNum < 1 || profileNum > 3) throw new ArgumentOutOfRangeException("profileNum", "Must be 1-3 only");
            var writeBuffer = string.Format(SET_PROFILE, profileNum - 1).ToByteArray();

            //Write the data to the device
            return await device.WriteAndReadAsync(writeBuffer);
        }

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

        public static void Listen(this IKeyboard keyboard)
        {
            "Listen Enter".ConsoleLogLine();
            keyboard.Device?.Close();
            keyboard.DeviceListener = new DeviceListener(GetFilterDeviceDefinitions(), Keyboard.POLL_MSECS);
            keyboard.DeviceListener.DeviceInitialized += keyboard.Initialized;
            keyboard.DeviceListener.DeviceDisconnected += keyboard.Disconnected;
            keyboard.DeviceListener.Start();
            "Listen Exit".ConsoleLogLine();
        }

        public static void StopListening(this IKeyboard keyboard)
        {
            "StopListening Enter".ConsoleLogLine();
            keyboard.DeviceListener?.Stop();
            keyboard.DeviceListener?.Dispose();
            keyboard.DeviceListener = null;
            "StopListening Exit".ConsoleLogLine();
        }
    }
}
