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
			using( var reader = GetText( "valid.terrain.json" ) ) {
				manager.Load( reader );
			}

			Assert.IsNotNull( manager[1] );
			Assert.AreEqual( "test_name", manager[1].IdName );
		}

		private StreamReader GetText( string name ) {
			var assembly = Assembly.GetExecutingAssembly();
			var resourceStream = assembly.GetManifestResourceStream( "Surface.Tests." + name );
			return new StreamReader( resourceStream, Encoding.UTF8 );
		}
	}
}