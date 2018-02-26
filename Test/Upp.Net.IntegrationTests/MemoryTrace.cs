using System;
using System.Collections.Generic;
using Upp.Net.Trace;

namespace Upp.Net.IntegrationTests
{
    internal class MemoryTrace : ITrace
    {
        private readonly List<string> _list = new List<string>();

        public int Count => _list.Count;

        public List<string> GetTrace()
        {
            return new List<string>(_list);
        }

        public void Info(string message, params object[] arguments)
        {
            _list.Add(string.Format(message, arguments));
        }

        public void Error(string message, params object[] arguments)
        {
            _list.Add(string.Format(message, arguments));
        }

        public void Exception(Exception exception)
        {
            _list.Add(exception.ToString());
        }

        public void Debug(string message, params object[] arguments)
        {
            _list.Add(string.Format(message, arguments));
        }

        public void Info(string message)
        {
            _list.Add(message);
        }

        public void Error(string message)
        {
            _list.Add(message);
        }

        public void Debug(string message)
        {
            _list.Add(message);
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, _list);
        }
    }
}