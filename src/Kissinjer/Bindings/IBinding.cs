namespace Kissinjer.Bindings {
	using System;
	using System.Linq.Expressions;

	public interface IBinding {
		Type ServiceType { get; }

		Expression Expression { get; }

		object Invoke();
	}

	public interface IBinding<TService> : IBinding {
		new TService Invoke();
	}
}
