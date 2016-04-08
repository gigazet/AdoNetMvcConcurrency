using System;
using System.Runtime.Serialization;

namespace ConcurrencyTest.Repository {
    [Serializable]
    public class ConcurrencyException : Exception {
        public ConcurrencyException() {
        }

        public ConcurrencyException(string message) : base(message) {
        }

        public ConcurrencyException(string message, Exception innerException) : base(message, innerException) {
        }

    }
}