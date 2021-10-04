using System.IO;
using System.Reflection;
using System.Text;
using BlockColony.Core.Shared;
using BlockColony.Core.Surface;
using BlockColony.Core.MapGenerator;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BlockColony.Default.MapGenerator.Tests {

	[TestFixture]
	public class MountainsideTerrainGeneratorTests {

		[Test]
		public void Build_ValidOptions_MapGenerated() {
			IJson json = new Json();
			IMapFactory mapFactory = new MapFactory();
			IRandom random = new FastRandom();
			var manager = new TerrainManager( json );
			var options = default( MapGeneratorOptions );
			var terrainOptions = default( MountainsideTerrainGeneratorOptions );
			using( StreamReader reader = GetText( "generator.json" ) ) {
				options = json.Deserialize<MapGeneratorOptions>( reader.ReadToEnd() );
			}
			using( StreamReader reader = GetText( "terraingenerator.json" ) ) {
				terrainOptions = json.Deserialize<MountainsideTerrainGeneratorOptions>( reader.ReadToEnd() );
			}
			using( StreamReader reader = GetText( "terrain.json" ) ) {
				manager.Load( reader );
			}
			IMapTerrainGenerator generator = new MountainsideTerrainGenerator( random, manager, options, terrainOptions );
			IMap map = mapFactory.Create( options.Rows, options.Columns, generator.Initialize );

			generator.Build( map );

			Assert.IsNotNull( map );

			WriteMapToImage( map );
		}

		private static StreamReader GetText( string name ) {
			var assembly = Assembly.GetExecutingAssembly();
			Stream resourceStream = assembly.GetManifestResourceStream( "BlockColony.Default.MapGenerator.Tests." + name );
			return new StreamReader( resourceStream, Encoding.UTF8 );
		}

		private static void WriteMapToImage( IMap map ) {
			using Image<Rgba32> image = new Image<Rgba32>( map.Columns, map.Rows );
			for( int column = 0; column < map.Columns; column++ ) {
				for( int row = 0; row < map.Rows; row++ ) {
					ref MapCell cell = ref map.GetCell( column, row );
					Rgba32 color = Color.White;
					color = cell.TerrainId switch {
						1 => Color.Blue,
						2 => Color.LawnGreen,
						4 => Color.SandyBrown,
						5 => Color.LightSlateGray,
						6 => Color.SlateGray,
						_ => Color.White,
					};
					image[column, row] = color;
				}
			}
			using var writer = new FileStream( @"C:\temp\image.png", FileMode.Create );
			image.SaveAsPng( writer );
		}
	}
}
