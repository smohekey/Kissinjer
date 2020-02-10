namespace Kissinjer.Scopes {
	using System;

	public interface IScope {
		TService GetOrAdd<TService>(Func<TService> createService);
	}
}
