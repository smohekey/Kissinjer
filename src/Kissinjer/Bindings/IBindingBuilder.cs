namespace Kissinjer.Bindings {
	using System;
	using System.Reflection;

	public interface IBindingBuilder {
		Type ServiceType { get; }

		IBindingBuilder InSingletonScope();
		IBindingBuilder WithSelector(IBindingSelector selector);

		int GetScore(ParameterInfo parameterInfo);

		IBinding Build();
	}

	public interface IBindingBuilder<TService> : IBindingBuilder {

	}
}
