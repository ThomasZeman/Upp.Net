using System;
using Upp.Net.Trace;

namespace Upp.Net.IntegrationTests
{
    public class Trace : ITrace
    {
        public void Info(string message, params object[] arguments)
        {
            System.Diagnostics.Debug.WriteLine(message, arguments);

        }

        public void Error(string message, params object[] arguments)
        {
            System.Diagnostics.Debug.WriteLine(message, arguments);
        }

        public void Exception(Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception.ToString());
        }

        public void Debug(string message, params object[] arguments)
        {
            System.Diagnostics.Debug.WriteLine(message, arguments);
        }

        public void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Error(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}