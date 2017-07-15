namespace Kissinjer {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Kissinjer.Bindings;
	using Kissinjer.Scopes;

	public interface IContainer {
		IContainer WithBindingBuilder(IBindingBuilder bindingBuilder);
		IContainer WithGenericBindingBuilder(IGenericBindingBuilder genericBindingBuilder);
		IContainer WithScopeProvider(IScopeProvider scopeProvider);

		IContainer Bind(Type serviceType, Type implementationType, Action<IBindingBuilder> composeAction = null);
		IContainer Bind(Type serviceType, Func<object> createFunc, Action<IBindingBuilder> composeAction = null);
		IContainer Bind(Type serviceType, object constant, Action<IBindingBuilder> composeAction = null);

		IContainer Build();

		object Get(Type serviceType);

		TService Get<TService>();

		bool TryGet(Type serviceType, out object service);

		bool TryGet<TService>(out TService service);

		IEnumerable GetAll(Type serviceType);

		IEnumerable<TService> GetAll<TService>();
	}
}
