using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using InitializeBinFolder.Common.Attributes;
using InitializeBinFolder.Common.Helpers;

namespace InitializeBinFolder.Common.Configuration;

/// <summary>
/// The config file for this application.
/// </summary>
[Serializable]
public class Config {
  /// <summary>
  /// Gets or sets a value that determines the <see cref="Uri" /> for the schema of the <see cref="Config" /> file.
  /// </summary>
  [JsonProperty("$schema")]
  [DefaultValueCallStaticMethod(typeof(Constants), nameof(Constants.GetSchemaUri))]
  public Uri? Schema { get; set; }

  /// <summary>
  /// Gets or sets a value that determines the level of which to log at.
  /// </summary>
  [JsonProperty("log_level")]
  public LogLevel LogLevel { get; set; } = LogLevel.Information;
}
