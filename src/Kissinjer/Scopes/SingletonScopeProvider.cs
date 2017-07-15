namespace Kissinjer.Scopes {
	using System;
	using System.Collections.Concurrent;

	public class SingletonScopeProvider : IScopeProvider {
		private readonly IScope _scope;

		public SingletonScopeProvider() {
			_scope = new Scope();
		}

		public bool TryGetScope(out IScope scope) {
			scope = _scope;

			return true;
		}

		public TService GetOrAdd<TService>(Func<TService> createService) {
			if(!TryGetScope(out var scope)) {
				throw new InvalidScopeException();
			}

			return scope.GetOrAdd(createService);
		}

		private class Scope : IScope {
			private readonly ConcurrentDictionary<Type, Lazy<object>> _values = new ConcurrentDictionary<Type, Lazy<object>>();
			
			public TService GetOrAdd<TService>(Func<TService> createService) {
				return (TService)_values.GetOrAdd(typeof(TService), _ => {
					return new Lazy<object>(() => createService());
				}).Value;
			}
		}
	}
}
