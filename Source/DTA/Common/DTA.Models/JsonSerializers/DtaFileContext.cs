using System.Text.Json.Serialization;
using DTA.Models.Files;

namespace DTA.Models.JsonSerializers;

[JsonSerializable(typeof(List<DtaFile>))]
public partial class DtaFileContext : JsonSerializerContext;