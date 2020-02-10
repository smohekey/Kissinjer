namespace Kissinjer.Selectors {
	using System;
	using System.Reflection;
	using System.Linq;
	using Kissinjer.Bindings;

	public class AttributeSelector : IBindingSelector {
		public Attribute Attribute { get; }

		public AttributeSelector(Attribute attribute) {
			Attribute = attribute;
		}

		public bool Matches(ParameterInfo parameterInfo) {
			return parameterInfo.GetCustomAttributes().Any(a => a == Attribute);
		}
	}
}
