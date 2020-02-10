namespace Kissinjer.Bindings {
	using System;

	public class GenericFuncBindingBuilder : BindingBuilder, IGenericBindingBuilder {
		public Func<object> CreateFunc { get; }

		public GenericFuncBindingBuilder(Container container, Type serviceType, Func<object> createFunc) : base(container, serviceType) {
			CreateFunc = createFunc;
		}
		
		public override IBinding Build() {
			throw new NotImplementedException();
		}

		public IBindingBuilder<TSpecificService> MakeSpecific<TSpecificService>() {
			throw new NotImplementedException();
		}
	}
}
