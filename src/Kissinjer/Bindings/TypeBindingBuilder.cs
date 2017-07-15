namespace Kissinjer.Bindings {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	public class TypeBindingBuilder<TService, TImplementation> : BindingBuilder<TService> {
		public Type ImplementationType { get; } = typeof(TImplementation);

		public TypeBindingBuilder(Container container) : base(container) {

		}

		public override IBinding Build() {
			return new Binding<TService>(ScopeProvider, BuildExpression());
		}

		private Expression BuildExpression() {
			if (!TryResolveConstructor(ImplementationType, out var constructorInfo, out var arguments)) {
				throw new BindingNotResolvedException($"Couldn't find suitable constructor for implementation type {ImplementationType.FullName}");
			}

			return Expression.New(constructorInfo, arguments);
		}

		private bool TryResolveConstructor(Type implType, out ConstructorInfo resolved, out List<Expression> resolvedArguments) {
			foreach (ConstructorInfo constructorInfo in implType.GetConstructors().OrderByDescending(c => c.GetParameters().Length)) {
				List<Expression> argumentExpressions = new List<Expression>();

				bool matched = true;

				foreach (ParameterInfo parameterInfo in constructorInfo.GetParameters()) {
					if(!TryResolveParameter(parameterInfo, out var argumentExpresion)) {
						matched = false;

						break;
					}

					argumentExpressions.Add(argumentExpresion);
				}

				if(matched) {
					resolved = constructorInfo;
					resolvedArguments = argumentExpressions;

					return true;
				}
			}

			resolved = null;
			resolvedArguments = null;

			return false;
		}

		private bool TryResolveParameter(ParameterInfo parameterInfo, out Expression argumentExpression) {
			if(!Container.TryGetBindingBuilders(parameterInfo.ParameterType, out var bindingBuilders)) {
				argumentExpression = null;

				return false;
			}

			int selectedScore = 0;
			IBindingBuilder selected = null;

			foreach(IBindingBuilder bindingBuilder in bindingBuilders) {
				int score = bindingBuilder.GetScore(parameterInfo);

				if(score > selectedScore) {
					selectedScore = score;
					selected = bindingBuilder;
				}
			}

			if(selected == null) {
				argumentExpression = null;

				return false;
			}

			IBinding binding = Container.GetOrBuildBinding(selected);

			argumentExpression = binding.Expression;

			return true;
		}
	}
}
