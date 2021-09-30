using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared
{
	public class Json : IJson {

		private readonly JsonSerializerOptions _options;

		public Json() {
			_options = new JsonSerializerOptions {
				Converters = {
					new JsonStringEnumConverter()
				}
			};
		}

		T IJson.Deserialize<T>( string json ) {
			return JsonSerializer.Deserialize<T>( json, _options );
		}

		string IJson.Serialize<T>( T value ) {
			return JsonSerializer.Serialize( value, _options );
		}
	}
}
