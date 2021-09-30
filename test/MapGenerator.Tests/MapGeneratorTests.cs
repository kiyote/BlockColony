using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Surface;
using Newtonsoft.Json;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using Shared;

namespace MapGenerator.Tests {

	[TestFixture]
	public class MapGeneratorTests {

		[Test]
		public void Build_ValidOptions_MapGenerated() {
			Json json = new Json();
			IMapGenerator generator = new Generator();
			var options = default( MapGeneratorOptions );
			using( StreamReader reader = GetText( "generator.json" ) ) {
				options = JsonConvert.DeserializeObject<MapGeneratorOptions>( reader.ReadToEnd() );
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

		private StreamReader GetText( string name ) {
			var assembly = Assembly.GetExecutingAssembly();
			Stream resourceStream = assembly.GetManifestResourceStream( "MapGenerator." + name );
			return new StreamReader( resourceStream, Encoding.UTF8 );
		}

		private void WriteMapToImage( Map map ) {
			using( Image<Rgba32> image = new Image<Rgba32>( map.Columns, map.Rows ) ) {
				for( int column = 0; column < map.Columns; column++ ) {
					for( int row = 0; row < map.Rows; row++ ) {
						ref MapCell cell = ref map.GetCell( column, row );
						Rgba32 color = Color.White;
						switch (cell.TerrainId) {
							case 1:
								color = Color.Blue;
								break;
							case 2:
								color = Color.LawnGreen;
								break;
							case 4:
								color = Color.SandyBrown;
								break;
							case 5:
								color = Color.LightSlateGray;
								break;
							case 6:
								color = Color.SlateGray;
								break;
							default:
								color = Color.White;
								break;
						}
						image[column, row] = color;
					}
				}
				using( var writer = new FileStream( @"C:\temp\image.png", FileMode.Create ) ) {
					image.SaveAsPng( writer );
				}
			}
		}
	}
}
