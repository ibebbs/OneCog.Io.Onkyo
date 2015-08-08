using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Console
{
    public class Options
    {
        [Option('h', "host", Required = true, HelpText = "Address of the device to connect to")]
        public string HostName { get; set; }

        [Option('p', "port", Required = false, DefaultValue = 60128, HelpText = "The port on which to connect to the device")]
        public int Port { get; set; }

        [Option('u', "unit", Required = false, DefaultValue = UnitType.Receiver, HelpText = "The type of device to connect to (i.e. Received)")]
        public UnitType UnitType { get; set; }

        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("OneCog.Io.Onkyo.Console", Assembly.GetExecutingAssembly().ImageRuntimeVersion),
                Copyright = new CopyrightInfo("OneCog Solutions", 2014),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("Usage: app -p Someone");
            help.AddOptions(this);
            return help;
        }

    }
}
