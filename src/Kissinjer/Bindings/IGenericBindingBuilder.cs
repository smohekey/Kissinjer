namespace Kissinjer.Bindings {
	public interface IGenericBindingBuilder : IBindingBuilder {
		IBindingBuilder<TService> MakeSpecific<TService>();
	}
}
