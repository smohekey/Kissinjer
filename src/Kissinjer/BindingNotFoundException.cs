namespace Kissinjer {
	using System;

	public class BindingNotFoundException : Exception {
		public BindingNotFoundException() {
		}

		public BindingNotFoundException(string message) : base(message) {
		}

		public BindingNotFoundException(string message, Exception innerException) : base(message, innerException) {
		}
	}
}