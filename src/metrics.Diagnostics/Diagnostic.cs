using System;
using System.Diagnostics;

namespace metrics.Diagnostics
{
    public class Diagnostic<TState> : Diagnostic<TState, TState>
    {
        public Diagnostic(string operationName, DiagnosticSource source, TState state)
            : base(operationName, source, state) =>
            EndState = state;
    }

    public class Diagnostic<TState, TStateEnd> : Activity
    {
        public static Diagnostic<TState, TStateEnd> Default { get; } = new();

        private readonly DiagnosticSource _source;
        private TStateEnd _endState;
        private readonly bool _default;
        private bool _disposed;

        private Diagnostic() : base("__NOOP__") => _default = true;

        public Diagnostic(string operationName, DiagnosticSource source, TState state) : base(operationName)
        {
            _source = source;
            _source.StartActivity(SetStartTime(DateTime.UtcNow), state);
        }

        public TStateEnd EndState
        {
            get => _endState;
            set
            {
                //do not store state on default instance
                if (_default) return;
                _endState = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                //_source can be null if Default instance
                _source?.StopActivity(SetEndTime(DateTime.UtcNow), EndState);
            }

            _disposed = true;

            base.Dispose(disposing);
        }
    }
}