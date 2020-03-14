using CommandLine;
using CommandLine.Text;
using System;

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
                       if (o.ShowAll || o.Profile > 0)
                       {
                           keyboard = new Keyboard();

#if DEBUG
                           if (o.ShowAll)
                           {

                               keyboard?.ShowAllDevices();
                           }
#endif

                           if (o.Profile > 0)
                           {
                               if (o.Profile < 1 || o.Profile > 3) throw new ArgumentOutOfRangeException("Profile", "Valid values: 1-3");
                               var response = keyboard?.Connect()?.Result?.SetProfile(o.Profile)?.Result;
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
    }
}
