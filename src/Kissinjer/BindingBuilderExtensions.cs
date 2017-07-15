namespace Kissinjer {
	using System;
	using Kissinjer.Bindings;
	using Kissinjer.Selectors;

	public static class BindingBuilderExtensions {
		public static IBindingBuilder WithAttribute(this IBindingBuilder builder, Attribute attribute) {
			return builder.WithSelector(new AttributeSelector(attribute));
		}

		public static IBindingBuilder WithAttribute<TAttribute>(this IBindingBuilder builder)
			where TAttribute : Attribute {

			return builder.WithAttribute(Activator.CreateInstance<TAttribute>());
		}
	}
}
