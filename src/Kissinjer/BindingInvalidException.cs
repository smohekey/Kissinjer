namespace Kissinjer {
	using System;

	public class BindingInvalidException : Exception {
		public BindingInvalidException() {
		}

		public BindingInvalidException(string message) : base(message) {
		}

		public BindingInvalidException(string message, Exception innerException) : base(message, innerException) {
		}
	}
}