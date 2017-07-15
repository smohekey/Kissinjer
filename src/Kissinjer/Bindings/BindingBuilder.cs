namespace Kissinjer.Bindings {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Kissinjer.Scopes;

	public abstract class BindingBuilder : IBindingBuilder, IEquatable<BindingBuilder> {
		public Container Container { get; }
		public Type ServiceType { get; }

		public HashSet<IBindingSelector> Selectors { get; } = new HashSet<IBindingSelector>();
		public IScopeProvider ScopeProvider { get; private set; }
		
		protected BindingBuilder(Container container, Type serviceType) {
			Container = container;
			ServiceType = serviceType;
		}

		public IBindingBuilder WithSelector(IBindingSelector selector) {
			Selectors.Add(selector);

			return this;
		}

		public IBindingBuilder InSingletonScope() {
			ScopeProvider = Container.GetScopeProvider<SingletonScopeProvider>();

			return this;
		}

		public int GetScore(ParameterInfo parameterInfo) {
			int score = 1;

			foreach(IBindingSelector selector in Selectors) {
				if(selector.Matches(parameterInfo)) {
					score++;
				}
			}

			return score;
		}

		public abstract IBinding Build();

		public bool Equals(BindingBuilder other) {
			return other.ServiceType.Equals(ServiceType)
				&& (
					(ScopeProvider == null && other.ScopeProvider == null)
					|| ScopeProvider.Equals(other.ScopeProvider)
				)
				&& Selectors.SetEquals(other.Selectors);
		}

		public override bool Equals(object obj) {
			BindingBuilder other = obj as BindingBuilder;

			return other != null && Equals(other);
		}

		public override int GetHashCode() {
			return ServiceType.GetHashCode();
		}
	}

	public abstract class BindingBuilder<TService> : BindingBuilder, IBindingBuilder<TService> {
		public BindingBuilder(Container container) : base(container, typeof(TService)) {

		}
	}
}
