namespace Kissinjer {
	using System;

	public class BindingRecursionException : Exception {
		public BindingRecursionException() {
		}

		public BindingRecursionException(string message) : base(message) {
		}

		public BindingRecursionException(string message, Exception innerException) : base(message, innerException) {
		}
	}
}