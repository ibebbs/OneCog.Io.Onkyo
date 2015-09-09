using System.Collections.Generic;
using System.Linq;

namespace OneCog.Io.Onkyo.Common
{
    public class InputSource
    {
        public InputSource(uint id, string name, IEnumerable<string> akas)
        {
            Id = id;
            Name = name;
            Akas = (akas ?? Enumerable.Empty<string>()).ToArray();
        }

        public uint Id { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<string> Akas { get; private set; }
    }

    public static class InputSources
    {
        private static readonly IEnumerable<InputSource> _inputSources = new[] 
        {
            new InputSource(0x00, "VIDEO1", new [] { "VCR/DVR", "STB/DVR" }),
            new InputSource(0x01, "VIDEO2", new [] { "CBL/SAT" }),
            new InputSource(0x02, "VIDEO3", new [] { "GAME/TV", "GAME", "GAME1" }),
            new InputSource(0x03, "VIDEO4", new [] { "AUX1(AUX)" }),
            new InputSource(0x04, "VIDEO5", new [] { "AUX2", "GAME2" }),
            new InputSource(0x05, "VIDEO6", new [] { "PC" }),
            new InputSource(0x06, "VIDEO7", Enumerable.Empty<string>()),
            new InputSource(0x07, "EXTRA1", Enumerable.Empty<string>()),
            new InputSource(0x08, "EXTRA2", Enumerable.Empty<string>()),
            new InputSource(0x09, "EXTRA3", Enumerable.Empty<string>()),
            new InputSource(0x10, "DVD", new [] { "BD/DVD" }),
            new InputSource(0x20, "TAPE(1)", new [] { "TV/TAPE" }),
            new InputSource(0x21, "TAPE2", Enumerable.Empty<string>()),
            new InputSource(0x22, "PHONO", Enumerable.Empty<string>()),
            new InputSource(0x23, "CD", new [] { "TV/CD" }),
            new InputSource(0x24, "FM", Enumerable.Empty<string>()),
            new InputSource(0x25, "AM", Enumerable.Empty<string>()),
            new InputSource(0x26, "TUNER", Enumerable.Empty<string>()),
            new InputSource(0x27, "MUSIC SERVER", new [] { "P4S", "DLNA*2" }),
            new InputSource(0x28, "INTERNET RADIO", new [] { "iRadio Favorite*3" }),
            new InputSource(0x29, "USB/USB(Front)", Enumerable.Empty<string>()),
            new InputSource(0x2A, "USB(Rear)", Enumerable.Empty<string>()),
            new InputSource(0x2B, "NETWORK", new [] { "NET" }),
            new InputSource(0x2C, "USB(toggle)", Enumerable.Empty<string>()),
            new InputSource(0x2D, "Aiplay", Enumerable.Empty<string>()),
            new InputSource(0x40, "Universal PORT", Enumerable.Empty<string>()),
            new InputSource(0x30, "MULTI CH", Enumerable.Empty<string>()),
            new InputSource(0x31, "XM*1", Enumerable.Empty<string>()),
            new InputSource(0x32, "SIRIUS*1", Enumerable.Empty<string>()),
            new InputSource(0x33, "DAB *5", Enumerable.Empty<string>()),
        };

        public static IEnumerable<InputSource> All
        {
            get { return _inputSources; }
        }

        public static bool TryGetFromId(uint id, out InputSource inputSource)
        {
            inputSource = _inputSources.FirstOrDefault(i => i.Id == id);

            return (inputSource != null);
        }
    }
}
