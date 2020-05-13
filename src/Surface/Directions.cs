using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Surface {
	[JsonConverter( typeof( StringEnumConverter ) )]
	[Flags]
	[SuppressMessage(
		"Design", "CA1028:Enum Storage should be Int32",
		Justification = "Minimized size, plus there aren't more than 8 directions." )]
	public enum Directions : byte {

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
