using System;
using System.Collections.Generic;
using Upp.Net.Trace;

namespace Upp.Net.IntegrationTests
{
    internal class MemoryTrace : ITrace
    {
        public List<string> List { get; } = new List<string>();

        public void Info(string message, params object[] arguments)
        {
            List.Add(string.Format(message, arguments));
        }

        public void Error(string message, params object[] arguments)
        {
            List.Add(string.Format(message, arguments));
        }

        public void Exception(Exception exception)
        {
            List.Add(exception.ToString());
        }

        public void Debug(string message, params object[] arguments)
        {
            List.Add(string.Format(message, arguments));
        }

        public void Info(string message)
        {
            List.Add(message);
        }

        public void Error(string message)
        {
            List.Add(message);
        }

        public void Debug(string message)
        {
            List.Add(message);
        }
    }
}