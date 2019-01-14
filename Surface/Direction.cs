using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Surface {
	[JsonConverter( typeof( StringEnumConverter ) )]
	[Flags]
    public enum Direction: byte {

		North = 0b0001,

		NorthEast = 0b0011,

		East = 0b0010,

		SouthEast = 0b0110,

		South = 0b0100,

		SouthWest = 0b1100,

		West = 0b1000,

		NorthWest = 0b1001,

		All = 0b1111
    }
}
