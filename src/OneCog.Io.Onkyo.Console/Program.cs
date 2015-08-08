using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using Input = System.Console;
using Output = System.Console;

namespace OneCog.Io.Onkyo.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine.ParserResult<Options> arguments = CommandLine.Parser.Default.ParseArguments(() => new Options(), args);

            if (!arguments.Errors.Any())
            {
                IscpStream stream = new IscpStream(arguments.Value.HostName, (ushort)arguments.Value.Port, arguments.Value.UnitType);

                Observable
                    .Generate(string.Empty, value => true, value => Input.ReadLine(), value => value)
                    .Where(command => !string.IsNullOrWhiteSpace(command))
                    .Select(command => PacketFactory.Default.CreatePacket(arguments.Value.UnitType, command))
                    .SubscribeOn(TaskPoolScheduler.Default)
                    .Subscribe(stream);

                stream.Subscribe(packet => Output.WriteLine(Encoding.UTF8.GetString(packet.Data)));

                Output.WriteLine("Connecting...");

                using (stream.Connect())
                {
                    Output.WriteLine("Connected");

                    stream.Wait();
                }
            }
            else
            {
                Input.ReadLine();
            }
        }
    }
}
