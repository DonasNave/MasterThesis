using System.Text.Json.Serialization;
using DTA.Models.Response;

namespace DTA.Models.JsonSerializers;

[JsonSerializable(typeof(PrimesReponse))]
public partial class PrimesResponseContext : JsonSerializerContext;