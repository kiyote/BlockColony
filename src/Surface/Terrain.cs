using System.Collections.Generic;
using System.Linq;

namespace Surface {

	public enum TerrainPhase {
		Unknown,
		Solid,
		Liquid,
		Gas
	}

	public interface ITerrainAttributes {

		string Name { get; }

		TerrainPhase Phase { get; }

		int Colour { get; }

		bool Pathable { get; }

		int PathingCost { get; }
	}

	public interface ITerrain {
		byte Id { get; }

		string IdName { get; }

		ITerrainAttributes this[ int celcius ] { get; }
	}

	internal class Terrain : ITerrain {

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
