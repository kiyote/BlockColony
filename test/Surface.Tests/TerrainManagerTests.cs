using System;
using System.IO;
using System.Text;
using System.Reflection;
using NUnit.Framework;

namespace Surface.Tests {
	[TestFixture]
	public class TerrainManagerTests {
		[Test]
		public void Load_ValidTerrainFile_TerrainLoaded() {
			var manager = new TerrainManager();
			using( StreamReader reader = GetText( "valid.terrain.json" ) ) {
				manager.Load( reader );
			}

			Assert.IsNotNull( manager[1] );
			Assert.AreEqual( "test_name", manager[1].IdName );
		}

		private StreamReader GetText( string name ) {
			var assembly = Assembly.GetExecutingAssembly();
			Stream resourceStream = assembly.GetManifestResourceStream( "Surface." + name );
			return new StreamReader( resourceStream, Encoding.UTF8 );
		}
	}
}
