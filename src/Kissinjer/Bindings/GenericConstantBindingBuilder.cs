namespace Kissinjer.Bindings {
	using System;

	public class GenericConstantBindingBuilder : BindingBuilder, IGenericBindingBuilder {
		public GenericConstantBindingBuilder(Container container, Type serviceType, object value) : base(container, serviceType) {

		}

		public override IBinding Build() {
			throw new NotImplementedException();
		}

		public IBindingBuilder<TSpecificService> MakeSpecific<TSpecificService>() {
			throw new NotImplementedException();
		}
	}
}
