using System;

namespace Upp.Net.Trace
{
    public class TracePrefixFacade : ITrace
    {
        private readonly string _prefix;
        private readonly ITrace _innerTrace;

        public TracePrefixFacade(string prefix, ITrace innerTrace)
        {
            _prefix = prefix;
            _innerTrace = innerTrace;
        }

        public void Info(string message, params object[] arguments)
        {
            _innerTrace.Info(CreateMessage(message), arguments);
        }

        public void Error(string message, params object[] arguments)
        {
            _innerTrace.Error(CreateMessage(message), arguments);
        }

        public void Exception(Exception exception)
        {
            _innerTrace.Exception(exception);
        }

        public void Debug(string message, params object[] arguments)
        {
            _innerTrace.Debug(CreateMessage(message), arguments);
        }

        private string CreateMessage(string message)
        {
            return string.Join(" ", _prefix, message);
        }

        public void Info(string message)
        {
            _innerTrace.Info(CreateMessage(message));
        }

        public void Error(string message)
        {
            _innerTrace.Error(CreateMessage(message));
        }

        public void Debug(string message)
        {
            _innerTrace.Debug(CreateMessage(message));
        }
    }
}
