using System.IO;
using System.Reflection;
using System.Text;
using BlockColony.Core.Shared;
using NUnit.Framework;

namespace BlockColony.Core.Surface.Tests {
	[TestFixture]
	public class TerrainManagerTests {
		[Test]
		public void Load_ValidTerrainFile_TerrainLoaded() {
			Json json = new Json();
			var manager = new TerrainManager(json);
			using( StreamReader reader = GetText( "valid.terrain.json" ) ) {
				manager.Load( reader );
			}

			Assert.IsNotNull( manager[1] );
			Assert.AreEqual( "test_name", manager[1].IdName );
		}

		private static StreamReader GetText( string name ) {
			var assembly = Assembly.GetExecutingAssembly();
			Stream resourceStream = assembly.GetManifestResourceStream( "BlockColony.Core.Surface.Tests." + name );
			return new StreamReader( resourceStream, Encoding.UTF8 );
		}
	}
}
