namespace Kissinjer {
	using System;
	using Kissinjer.Bindings;

	public static class ContainerExtensions {
		public static IContainer Bind<TService, TImplementation>(this IContainer container, Action<IBindingBuilder> composeAction = null) {
			return container.Bind(typeof(TService), typeof(TImplementation), composeAction);
		}

		public static IContainer Bind<TService>(this IContainer container, Type implementationType, Action<IBindingBuilder> composeAction = null) {
			return container.Bind(typeof(TService), implementationType, composeAction);
		}

		public static IContainer Bind<TService>(this IContainer container, Func<TService> createFunc, Action<IBindingBuilder> composeAction = null) {
			return container.Bind(typeof(TService), createFunc, composeAction);
		}

		public static IContainer Bind<TService>(this IContainer container, TService constant, Action<IBindingBuilder> composeAction = null) {
			return container.Bind(typeof(TService), constant, composeAction);
		}
	}
}
