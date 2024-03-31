using System.Text.Json.Serialization;
using DTA.Models.Signal;

namespace SRS;

[JsonSerializable(typeof(List<Signal>))]
public partial class SrsJsonSerializerContext : JsonSerializerContext;