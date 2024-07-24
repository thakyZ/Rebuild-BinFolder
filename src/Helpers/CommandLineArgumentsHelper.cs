// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// https://raw.githubusercontent.com/microsoft/vstest/55a7b50eedd71339df8ee1a5796622f76caf91d3/src/Microsoft.TestPlatform.CoreUtilities/Helpers/CommandLineArgumentsHelper.cs

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Rebuild_BinFolder.Helpers;

/// <summary>
/// Helper class for processing arguments passed to a process.
/// </summary>
public static class CommandLineArgumentsHelper {
  /// <summary>
  /// Parse command line arguments to a dictionary.
  /// </summary>
  /// <param name="args">Command line arguments. Ex: <c>{ "--port", "12312", "--parentprocessid", "2312", "--testsourcepath", "C:\temp\1.dll" }</c></param>
  /// <returns>Dictionary of arguments keys and values.</returns>
  [SuppressMessage("Major Code Smell", "S127:\"for\" loop stop conditions should be invariant", Justification = "Unnecessary SonarLint Warning")]
  public static IDictionary<string, string?> GetArgumentsDictionary(string[]? args) {
    var argsDictionary = new Dictionary<string, string?>();
    if (args == null) {
      return argsDictionary;
    }

    for (var i = 0; i < args.Length;) {
      if (args[i].StartsWith('-')) {
        if (i < args.Length - 1 && !args[i + 1].StartsWith('-')) {
          argsDictionary.Add(args[i], args[i + 1]);
          i++;
        } else {
          argsDictionary.Add(args[i], null);
        }
      }
      i++;
    }

    return argsDictionary;
  }

  /// <summary>
  /// Parse the value of an argument as an integer.
  /// </summary>
  /// <param name="argsDictionary">Dictionary of all arguments Ex: <c>{ "--port":"12312", "--parentprocessid":"2312" }</c></param>
  /// <param name="fullName">The full name for required argument. Ex: "--port"</param>
  /// <returns>Value of the argument.</returns>
  /// <exception cref="ArgumentException">Thrown if value of an argument is not an integer.</exception>
  public static int GetIntArgFromDictionary(IDictionary<string, string?> argsDictionary, string fullName) {
    var found = TryGetIntArgFromDictionary(argsDictionary, fullName, out var value);
    return found ? value : 0;
  }

  /// <summary>
  /// Try get the argument and parse the value of an argument as an integer.
  /// </summary>
  /// <param name="argsDictionary">Dictionary of all arguments Ex: <c>{ "--port":"12312", "--parentprocessid":"2312" }</c></param>
  /// <param name="fullName">The full name for required argument. Ex: "--port"</param>
  /// <returns>Value of the argument.</returns>
  /// <exception cref="ArgumentException">Thrown if value of an argument is not an integer.</exception>
  public static bool TryGetIntArgFromDictionary(IDictionary<string, string?> argsDictionary, string fullName, out int value) {
    if (argsDictionary.TryGetValue(fullName, out var optionValue)) {
      return int.TryParse(optionValue, out value);
    }

    value = default;
    return false;
  }

  /// <summary>
  /// Parse the value of an argument as a string.
  /// </summary>
  /// <param name="argsDictionary">Dictionary of all arguments Ex: <c>{ "--port":"12312", "--parentprocessid":"2312" }</c></param>
  /// <param name="fullName">The full name for required argument. Ex: "--port"</param>
  /// <returns>Value of the argument.</returns>
  /// <exception cref="ArgumentException">Thrown if value of an argument is not an integer.</exception>
  public static string? GetStringArgFromDictionary(IDictionary<string, string?> argsDictionary, string fullName) {
    return argsDictionary.TryGetValue(fullName, out var optionValue) ? optionValue : string.Empty;
  }
}
