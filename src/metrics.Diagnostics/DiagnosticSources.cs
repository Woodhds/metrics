using System;
using System.Diagnostics;

namespace metrics.Diagnostics
{
    public static class DiagnosticSources
    {
        internal class EmptyDisposable : IDisposable
        {
            public void Dispose() { }
        }

        internal static EmptyDisposable SingletonDisposable { get; } = new();
        
        public static IDisposable Diagnose<TState>(this DiagnosticSource source, string operationName, TState state)
        {
            if (!source.IsEnabled(operationName)) return SingletonDisposable;

            return new Diagnostic<TState>(operationName, source, state);
        }

        public static Diagnostic<TState, TStateStop> Diagnose<TState, TStateStop>(this DiagnosticSource source, string operationName, TState state)
        {
            if (!source.IsEnabled(operationName)) return Diagnostic<TState, TStateStop>.Default;

            return new Diagnostic<TState, TStateStop>(operationName, source, state);
        }

        public static Diagnostic<TState, TEndState> Diagnose<TState, TEndState>(this DiagnosticSource source, string operationName, TState state, TEndState endState)
        {
            if (!source.IsEnabled(operationName)) return Diagnostic<TState, TEndState>.Default;

            return new Diagnostic<TState, TEndState>(operationName, source, state)
            {
                EndState = endState
            };
        }
    }
}