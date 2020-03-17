using CommandLine;
using CommandLine.Text;
using System;
using System.Configuration;

namespace GmmkUtil
{
    
    class Program
    {
        static ParserResult<Options> ParserResult;
        static void Main(string[] args)
        {
            ParserResult = Parser.Default.ParseArguments<Options>(args);
            ParserResult.WithParsed<Options>(o =>
                   {
                       Keyboard keyboard = null;
                       if (o.ShowAll || o.Profile > 0 || o.DefaultProfile || o.InitProfile > 0)
                       {
                           keyboard = new Keyboard();

                           if (o.ShowAll)
                           {

                               keyboard?.ShowAllDevices();
                           }

                           if (o.Profile > 0 && o.DefaultProfile) throw new InvalidOperationException("Cant specify default and numbered profiles");

                           if (o.Profile > 0)
                           {
                               ValidateProfile(o.Profile);
                               var profResponse = keyboard?.Connect()?.Result?.SetProfile(o.Profile)?.Result;
                           }
                           else if (o.DefaultProfile)
                           {
                               int defaultProfile;
                               if (!int.TryParse(ConfigurationManager.AppSettings["DefaultProfile"], out defaultProfile)) ValidateProfile(0);
                               var defResponse = keyboard?.Connect()?.Result?.SetProfile(defaultProfile)?.Result;
                           }
                           else if (o.InitProfile > 0)
                           {
                               ValidateProfile(o.InitProfile);
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
                });
        }

        private static void ValidateProfile(int profile)
        {
            if (profile < 1 || profile > 3) throw new ArgumentOutOfRangeException("Profile", "Valid values: 1-3");
        }
    }
}
