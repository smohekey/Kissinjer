namespace Kissinjer {
	using System;

	public class BindingNotResolvedException : Exception {
		public BindingNotResolvedException() {
		}

		public BindingNotResolvedException(string message) : base(message) {
		}

		public BindingNotResolvedException(string message, Exception innerException) : base(message, innerException) {
		}
	}
}