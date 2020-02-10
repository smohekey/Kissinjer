namespace Kissinjer.Bindings {
	using System;
	using System.Reflection;

	public class GenericTypeBindingBuilder : BindingBuilder, IGenericBindingBuilder {
		private static readonly Type __typeBindingBuilderType = typeof(TypeBindingBuilder<,>);

		public Type ImplementationType { get; }
		
		public GenericTypeBindingBuilder(Container container, Type serviceType, Type implementationType) : base(container, serviceType) {
			ImplementationType = implementationType;
		}

		public override IBinding Build() {
			throw new NotImplementedException();
		}

		public IBindingBuilder<TSpecificService> MakeSpecific<TSpecificService>() {
			Type serviceType = typeof(TSpecificService);
			Type specificImplementationType = ImplementationType.MakeGenericType(serviceType.GetGenericArguments());

			return (IBindingBuilder<TSpecificService>)__typeBindingBuilderType
				.MakeGenericType(serviceType, specificImplementationType)
				.GetConstructor(new Type[] { typeof(Container) })
				.Invoke(new object[] { Container });
		}
	}
}
