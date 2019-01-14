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

namespace MapGenerator.Tests {

	[TestFixture]
	public class MapGeneratorTests {

		private DefaultInitializer _defaultInitializer;

		[OneTimeSetUp]
		public void OneTimeSetUp() {
			_defaultInitializer = new DefaultInitializer();
		}

		[Test]
		public void Build_ValidOptions_MapGenerated() {
			IMapGenerator generator = new Generator();
			var options = default( MapGeneratorOptions );
			using( var reader = GetText( "generator.json" ) ) {
				options = JsonConvert.DeserializeObject<MapGeneratorOptions>( reader.ReadToEnd() );
			}
			var manager = new TerrainManager();
			using( var reader = GetText( "terrain.json" ) ) {
				manager.Load( reader );
			}

			var eventCount = 0;
			var gate = new AutoResetEvent( false );
			generator.MapGenerationCompleted += ( _, __ ) => {
				eventCount++;
				gate.Set();
			};
			generator.Build( manager, options );
			gate.WaitOne( 10000 );

			Assert.AreEqual( 1, eventCount );
			Assert.IsNotNull( generator.Map );

			WriteMapToImage( generator.Map );
		}

		private StreamReader GetText( string name ) {
			var assembly = Assembly.GetExecutingAssembly();
			var resourceStream = assembly.GetManifestResourceStream( "MapGenerator.Tests." + name );
			return new StreamReader( resourceStream, Encoding.UTF8 );
		}

		private class DefaultInitializer : IMapMethod {
			void IMapMethod.Do( ref MapCell cell ) {
				cell.TerrainCost = 100;
				cell.Walkability = (byte)Direction.All;
			}
		}

		private void WriteMapToImage( Map map ) {
			using( Image<Rgba32> image = new Image<Rgba32>( map.Columns, map.Rows ) ) {
				for( int column = 0; column < map.Columns; column++ ) {
					for( int row = 0; row < map.Rows; row++ ) {
						ref var cell = ref map.GetCell( column, row );
						var color = Rgba32.White;
						switch( cell.TerrainId ) {
							case 1:
								color = Rgba32.Blue;
								break;
							case 3:
								color = Rgba32.LawnGreen;
								break;
							case 4:
								color = Rgba32.SandyBrown;
								break;
							case 5:
								color = Rgba32.LightSlateGray;
								break;
							case 6:
								color = Rgba32.SlateGray;
								break;
							default:
								color = Rgba32.White;
								break;
						}
						image[ column, row ] = color;
					}
				}
				using( var writer = new FileStream( @"C:\temp\image.png", FileMode.Create ) ) {
					image.SaveAsPng( writer );
				}
			}
		}
	}
}