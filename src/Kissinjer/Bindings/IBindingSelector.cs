using System.Reflection;

namespace Kissinjer.Bindings {
	public interface IBindingSelector {
		bool Matches(ParameterInfo parameterInfo);
	}
}
