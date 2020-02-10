namespace Kissinjer.Bindings {
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using Kissinjer.Scopes;

	public class Binding<TService> : IBinding<TService> {
		private static readonly MethodInfo __getOrAddMethodInfo = typeof(IScope).GetMethod(nameof(IScope.GetOrAdd));
		
		public Type ServiceType { get; } = typeof(TService);
		
		public IScopeProvider ScopeProvider { get; }

		public Expression Expression { get; }
		private readonly Func<TService> _func;

		public Binding(IScopeProvider scopeProvider, Expression createExpression) {
			ScopeProvider = scopeProvider;
			Expression = GetScopedExpression(createExpression);

			_func = Expression.Lambda<Func<TService>>(Expression).Compile();
		}

		private Expression GetScopedExpression(Expression createExpression) {
			if (ScopeProvider == null) {
				return createExpression;
			}

			return Expression.Call(
				Expression.Constant(ScopeProvider),
				__getOrAddMethodInfo.MakeGenericMethod(ServiceType),
				Expression.Lambda<Func<TService>>(
					createExpression
				)
			);
		}

		public TService Invoke() {
			return _func();
		}

		object IBinding.Invoke() {
			return Invoke();
		}
	}
}
