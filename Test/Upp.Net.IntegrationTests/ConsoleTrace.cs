using System;
using Upp.Net.Trace;

namespace Upp.Net.IntegrationTests
{
    public class ConsoleTrace : ITrace
    {
        public void Info(string message, params object[] arguments)
        {
            System.Console.WriteLine(message, arguments);

        }

        public void Error(string message, params object[] arguments)
        {
            System.Console.WriteLine(message, arguments);
        }

        public void Exception(Exception exception)
        {
            System.Console.WriteLine(exception.ToString());
        }

        public void Debug(string message, params object[] arguments)
        {
            System.Console.WriteLine(message, arguments);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message)
        {
            Console.WriteLine(message);            
        }

        public void Debug(string message)
        {
            Console.WriteLine(message);
        }
    }
}