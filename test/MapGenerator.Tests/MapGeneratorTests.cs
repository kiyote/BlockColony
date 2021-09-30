using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using BlockColony.Core.Shared;
using BlockColony.Core.Surface;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BlockColony.Core.MapGenerator.Tests {

	[TestFixture]
	public class MapGeneratorTests {

		[Test]
		public void Build_ValidOptions_MapGenerated() {
			IJson json = new Json();
			IMapFactory mapFactory = new MapFactory();
			IRandom random = new FastRandom();
			IMapGenerator generator = new Generator(mapFactory, random);
			var options = default( MapGeneratorOptions );
			using( StreamReader reader = GetText( "generator.json" ) ) {
				options = json.Deserialize<MapGeneratorOptions>( reader.ReadToEnd() );
			}
			var manager = new TerrainManager(json);
			using( StreamReader reader = GetText( "terrain.json" ) ) {
				manager.Load( reader );
			}

			int eventCount = 0;
			using( var gate = new AutoResetEvent( false ) ) {
				generator.MapGenerationCompleted += ( _, __ ) => {
					eventCount++;
					gate.Set();
				};
				generator.Build( manager, options );
				gate.WaitOne( 10000 );
			}

			Assert.AreEqual( 1, eventCount );
			Assert.IsNotNull( generator.Map );

			WriteMapToImage( generator.Map );
		}

		private static StreamReader GetText( string name ) {
			var assembly = Assembly.GetExecutingAssembly();
			Stream resourceStream = assembly.GetManifestResourceStream( "BlockColony.Core.MapGenerator.Tests." + name );
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
