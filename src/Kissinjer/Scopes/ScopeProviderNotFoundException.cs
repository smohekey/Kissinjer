namespace Kissinjer.Scopes {
	using System;

	public class ScopeProviderNotFoundException : Exception {
		public ScopeProviderNotFoundException() {
		}

		public ScopeProviderNotFoundException(string message) : base(message) {
		}

		public ScopeProviderNotFoundException(string message, Exception innerException) : base(message, innerException) {
		}
	}
}