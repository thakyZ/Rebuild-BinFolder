using System.Collections.Concurrent;

using Rebuild_BinFolder.Exceptions;

namespace Rebuild_BinFolder.Helpers;

internal class Singletons : IDisposable {
  private static readonly Dictionary<Type, Func<object>> TypeInitializers = new() { };

  private static readonly ConcurrentDictionary<Type, object> ActiveInstances = new();

  public static T Get<T>() {
    return (T)ActiveInstances.GetOrAdd(typeof(T), (objectType) => {
      object newInstance = TypeInitializers.TryGetValue(objectType, out Func<object>? initializer)
        ? initializer()
        : throw new GetSingletonException(objectType, $"No initializer found for Type '{objectType.FullName}'.");

      return newInstance is not T
        ? throw new GetSingletonException(objectType, $"Received invalid result from initializer for type '{objectType.FullName}'")
        : newInstance;
    });
  }

  public static void Register(object newSingleton) {
    if (!ActiveInstances.TryAdd(newSingleton.GetType(), newSingleton)) {
      throw new RegisterSingletonException(newSingleton.GetType(), $"Failed to register new singleton for type {newSingleton.GetType()}");
    }
  }

  private bool _isDisposed;

  protected virtual void Dispose(bool disposing) {
    if (disposing) {
      if (!_isDisposed) {
        foreach (object singleton in ActiveInstances.Values) {
          // Only dispose the disposable objects that we own
          if (singleton is IDisposable disposable) {
            disposable.Dispose();
          }
        }
      }
      ActiveInstances.Clear();
      _isDisposed = true;
    }
  }

  public void Dispose() {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
}
