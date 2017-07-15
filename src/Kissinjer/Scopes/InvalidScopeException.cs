﻿using System;

namespace Kissinjer.Scopes {
	internal class InvalidScopeException : Exception {
		public InvalidScopeException() {
		}

		public InvalidScopeException(string message) : base(message) {
		}

		public InvalidScopeException(string message, Exception innerException) : base(message, innerException) {
		}
	}
}