using System.Text.Json.Serialization;
using DTA.Models.Response;

namespace DTA.Models.JsonSerializers;

[JsonSerializable(typeof(FibonacciResponse))]
public partial class FibonacciResponseContext : JsonSerializerContext;