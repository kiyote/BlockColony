using NUnit.Framework;
using System.Threading;
using BlockColony.Core.Surface;
using Moq;

namespace BlockColony.Core.MapGenerator.Tests {
	[TestFixture]
	public sealed class MapGeneratorTests {
		[Test]
		public void Build_ValidOptions_MapGenerated() {
			Mock<IMap> map = new Mock<IMap>( MockBehavior.Strict );

			Mock<IMapTerrainGenerator> terrainGenerator = new Mock<IMapTerrainGenerator>( MockBehavior.Strict );
			terrainGenerator
				.Setup( g => g.Initialize( ref It.Ref<MapCell>.IsAny ) );
			terrainGenerator
				.Setup( g => g.Build( map.Object ) );

			Mock<IMapFactory> mapFactory = new Mock<IMapFactory>( MockBehavior.Strict );
			mapFactory
				.Setup( f => f.Create( 100, 100, terrainGenerator.Object.Initialize ) )
				.Returns( map.Object );

			MapGeneratorOptions options = new MapGeneratorOptions(
				100,
				100
			);

			IMapGenerator generator = new MapGenerator( mapFactory.Object );
			IMap returnedMap = default;
			using( var gate = new AutoResetEvent( false ) ) {
				generator.Build( options, terrainGenerator.Object, (string message) => { }, ( IMap newMap ) => returnedMap = newMap );
				gate.WaitOne( 1000 ); // Bail after one second just in case.
			}

			Assert.AreSame( returnedMap, map.Object );
		}
	}
}
