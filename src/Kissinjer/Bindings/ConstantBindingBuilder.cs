namespace Kissinjer.Bindings {
	using System.Linq.Expressions;

	public class ConstantBindingBuilder<TService> : BindingBuilder<TService> {
		public TService Value { get; }

		public ConstantBindingBuilder(Container container, TService value) : base(container) {
			Value = value;
		}

		public override IBinding Build() {
			return new Binding<TService>(ScopeProvider, Expression.Constant(Value, ServiceType));
		}
	}
}
