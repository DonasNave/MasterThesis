using System.Text.Json.Serialization;
using DTA.Models.Signal;

namespace DTA.Models.JsonSerializers;

[JsonSerializable(typeof(List<DtaSignal>))]
public partial class DtaSignalContext : JsonSerializerContext;