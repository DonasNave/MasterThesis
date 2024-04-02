using System.Text.Json.Serialization;
using DTA.Models.Files;
using DTA.Models.Response;

namespace DTA.Models.JsonSerializers;

[JsonSerializable(typeof(CommonResponse))]
public partial class CommonResponseContext : JsonSerializerContext;