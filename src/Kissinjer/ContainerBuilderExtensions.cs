namespace Kissinjer {
	using System;
	using System.Reflection;
	using Kissinjer.Bindings;

	public static class ContainerBuilderExtensions {
		private static readonly Type __type = typeof(ContainerBuilderExtensions);

		#region Constant Bindings
		private static readonly MethodInfo __bindConstantMethod = __type.GetMethod("BindConstant");

		private static ContainerBuilder BindConstant<TService>(ContainerBuilder builder, TService constant, Action<IBindingBuilder> composeAction) {
			TypeInfo serviceType = typeof(TService).GetTypeInfo();

			if (serviceType.IsGenericTypeDefinition) {
				IGenericBindingBuilder<TService> genericBindingBuilder = new GenericConstantBindingBuilder<TService>(builder, constant);

				builder.WithGenericBindingBuilder(genericBindingBuilder);

				composeAction?.Invoke(genericBindingBuilder);
			} else {
				IBindingBuilder bindingBuilder = new ConstantBindingBuilder<TService>(builder, constant);

				composeAction?.Invoke(bindingBuilder);

				builder.WithBindingBuilder(bindingBuilder);
			}

			return builder;
		}

		public static ContainerBuilder Bind<TService>(this ContainerBuilder builder, TService constant, Action<IBindingBuilder> composeAction = null) {
			return BindConstant(builder, constant, composeAction);
		}

		public static ContainerBuilder Bind(this ContainerBuilder builder, Type serviceType, object constant, Action<IBindingBuilder> composeAction = null) {
			return
				(ContainerBuilder)__bindConstantMethod
				.MakeGenericMethod(serviceType)
				.Invoke(null, new object[] { builder, constant, composeAction });
		}
		#endregion

		#region Type Bindings
		private static readonly MethodInfo __bindTypeMethod = __type.GetMethod("BindType", BindingFlags.Static | BindingFlags.NonPublic);

		private static ContainerBuilder BindType<TService, TImplementation>(ContainerBuilder builder, Action<IBindingBuilder> composeAction) {
			TypeInfo serviceType = typeof(TService).GetTypeInfo();
			TypeInfo implementationType = typeof(TImplementation).GetTypeInfo();

			if (serviceType.IsGenericTypeDefinition) {
				if (!implementationType.IsGenericTypeDefinition
					|| serviceType.GetGenericArguments().Length != implementationType.GetGenericArguments().Length) {

					throw new BindingInvalidException("Generic service types must have a generic implementation with matching argument count.");
				}

				IGenericBindingBuilder<TService> genericBindingBuilder = new GenericTypeBindingBuilder<TService, TImplementation>(builder);

				composeAction?.Invoke(genericBindingBuilder);

				builder.WithGenericBindingBuilder(genericBindingBuilder);
			} else {
				IBindingBuilder bindingBuilder = new TypeBindingBuilder<TService, TImplementation>(builder);

				composeAction?.Invoke(bindingBuilder);

				builder.WithBindingBuilder(bindingBuilder);
			}

			return builder;
		}

		public static ContainerBuilder Bind<TService, TImplementation>(this ContainerBuilder builder, Action<IBindingBuilder> composeAction = null) {
			return BindType<TService, TImplementation>(builder, composeAction);
		}

		public static ContainerBuilder Bind(this ContainerBuilder builder, Type serviceType, Type implementationType, Action<IBindingBuilder> composeAction = null) {
			return
				(ContainerBuilder)__bindTypeMethod
				.MakeGenericMethod(serviceType, implementationType)
				.Invoke(null, new object[] { builder, composeAction });
		}

		public static ContainerBuilder Bind<TService>(this ContainerBuilder builder, Type implementationType, Action<IBindingBuilder> composeAction = null) {
			return builder.Bind(typeof(TService), implementationType, composeAction);
		}
		#endregion

		#region Func Bindings
		private static readonly MethodInfo __bindFuncMethod = __type.GetMethod("BindFunc");

		private static ContainerBuilder BindFunc<TService>(ContainerBuilder builder, Func<TService> func, Action<IBindingBuilder> composeAction) {
			TypeInfo serviceType = typeof(TService).GetTypeInfo();
			TypeInfo implementationType = func.GetMethodInfo().GetGenericArguments()[0].GetTypeInfo();

			if (serviceType.IsGenericTypeDefinition) {
				if (serviceType.GetGenericArguments().Length != implementationType.GetGenericArguments().Length) {

					throw new BindingInvalidException("Generic service types must have a generic func with matching argument count.");
				}

				IGenericBindingBuilder<TService> genericBindingBuilder = new GenericFuncBindingBuilder<TService>(builder, func);

				composeAction?.Invoke(genericBindingBuilder);

				builder.WithGenericBindingBuilder(genericBindingBuilder);
			} else {
				IBindingBuilder bindingBuilder = new FuncBindingBuilder<TService>(builder, func);

				composeAction?.Invoke(bindingBuilder);

				builder.WithBindingBuilder(bindingBuilder);
			}

			return builder;
		}

		public static ContainerBuilder Bind<TService>(this ContainerBuilder builder, Func<TService> func, Action<IBindingBuilder> composeAction = null) {
			return BindFunc(builder, func, composeAction);
		}

		public static ContainerBuilder Bind(this ContainerBuilder builder, Type serviceType, Func<object> func, Action<IBindingBuilder> composeAction = null) {
			return
				(ContainerBuilder)__bindConstantMethod
				.MakeGenericMethod(serviceType)
				.Invoke(null, new object[] { builder, func, composeAction });
		}
		#endregion
	}
}
