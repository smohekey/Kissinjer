namespace Kissinjer {
	using System;

	public class BindingNotUniqueException : Exception {
		public BindingNotUniqueException() {
		}

		public BindingNotUniqueException(string message) : base(message) {
		}

		public BindingNotUniqueException(string message, Exception innerException) : base(message, innerException) {
		}
	}
}