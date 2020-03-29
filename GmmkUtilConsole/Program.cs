using CommandLine;
using CommandLine.Text;
using EasyLife;
using GmmkUtil.GmmkLib;
using System;
using System.Configuration;
using System.Threading;

namespace GmmkUtil.GmmkUtilConsole
{
    
    class Program
    {
        static ParserResult<Options> ParserResult;
        static void Main(string[] args)
        {
            try
            {
                ParserResult = Parser.Default.ParseArguments<Options>(args);
                ParserResult.WithParsed<Options>(o =>
                       {
                           Keyboard keyboard = null;

                           ConsoleLogger.Enabled = o.Verbose;

                           if (o.ShowAll || o.Profile > 0 || o.DefaultProfile || o.InitProfile > 0)
                           {
                               keyboard = new Keyboard();

                               if (o.ShowAll)
                               {
                                   $"ShowAll".ConsoleLogLine();

                                   keyboard?.ShowAllDevices();
                               }

                               if (o.Profile > 0 && o.DefaultProfile) throw new InvalidOperationException("Cant specify default and numbered profiles");

                               if (o.Profile > 0)
                               {
                                   $"Profile {o.Profile}".ConsoleLogLine();

                                   ValidateProfile(o.Profile);
                                   var profResponse = keyboard?.Connect()?.Result?.Device.SetProfile(o.Profile)?.Result;
                               }
                               else if (o.DefaultProfile)
                               {
                                   $"DefaultProfile {o.DefaultProfile}".ConsoleLogLine();

                                   int defaultProfile;
                                   if (!int.TryParse(ConfigurationManager.AppSettings["DefaultProfile"], out defaultProfile)) ValidateProfile(0);
                                   var defResponse = keyboard?.Connect()?.Result?.Device.SetProfile(defaultProfile)?.Result;
                               }
                               if (o.InitProfile > 0)
                               {
                                   $"InitProfile {o.InitProfile}".ConsoleLogLine();

                                   ValidateProfile(o.InitProfile);
                                   keyboard.DefaultProfile = o.InitProfile;
                                   keyboard.Listen();
                                   while (true) { Thread.Sleep(1000); }
                               }
                           }
                           else
                           {
                               Console.WriteLine(HelpText.AutoBuild(ParserResult, h => h, e => e));
                           }
                       })
                    .WithNotParsed(errs =>
                    {
                        Console.WriteLine("Parser Fail");
                        errs.ConsoleLogLine();
                    });
            }
            catch (Exception ex)
            {
                ConsoleLogger.Enabled = true;
                ex.ConsoleLogLine();
                throw;
            }
        }

        private static void ValidateProfile(int profile)
        {
            if (profile < 1 || profile > 3) throw new ArgumentOutOfRangeException("Profile", "Valid values: 1-3");
        }
    }
}
