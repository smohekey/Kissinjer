namespace Kissinjer.Bindings {
	using System;
	using System.Linq.Expressions;
	using System.Reflection;

	public class FuncBindingBuilder<TService> : BindingBuilder<TService> {
		public Func<TService> CreateFunc { get; }

		public FuncBindingBuilder(Container container, Func<TService> createFunc) : base(container) {
			CreateFunc = createFunc;
		}

		public override IBinding Build() {
			return new Binding<TService>(
				ScopeProvider,
				Expression.Call(
					Expression.Constant(CreateFunc.Target),
					CreateFunc.GetMethodInfo()
				)
			);
		}
	}
}
