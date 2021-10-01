using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlockColony.Core.Shared {
	internal sealed class Json : IJson {

		private readonly JsonSerializerOptions _options;

		public Json() {
			_options = new JsonSerializerOptions {
				Converters = {
					new JsonStringEnumConverter()
				},
				PropertyNameCaseInsensitive = true,
				AllowTrailingCommas = true
			};
		}

		T? IJson.Deserialize<T>( string json ) where T: class {
			return JsonSerializer.Deserialize<T>( json, _options );
		}

		string IJson.Serialize<T>( T value ) {
			return JsonSerializer.Serialize( value, _options );
		}
	}
}
