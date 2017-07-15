namespace Kissinjer.Scopes {
	public interface IScopeProvider : IScope {
		bool TryGetScope(out IScope scope);
	}
}
