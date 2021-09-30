using System.Collections.Generic;
using System.Linq;

namespace BlockColony.Core.Surface {

	internal sealed class Terrain : ITerrain {

		private readonly SortedList<int, ITerrainAttributes> _attributes;

		public Terrain( int id, string idName, IEnumerable<(int Transition, ITerrainAttributes Attribute)> attributes ) {
			Id = (byte)id;
			IdName = idName;
			_attributes = new SortedList<int, ITerrainAttributes>();
			foreach( (int Transition, ITerrainAttributes Attribute) attribute in attributes ) {
				_attributes.Add( attribute.Transition, attribute.Attribute );
			}
		}

		public byte Id { get; }

		public string IdName { get; }

		public ITerrainAttributes this[ int celcius ] {
			get {
				return _attributes.Last( a => a.Key < celcius ).Value;
			}
		}
	}
}
