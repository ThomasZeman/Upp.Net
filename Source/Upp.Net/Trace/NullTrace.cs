using System;

namespace Upp.Net.Trace
{
    public class NullTrace : ITrace
    {
        public void Info(string message, params object[] arguments)
        {
        }

        public void Error(string message, params object[] arguments)
        {
        }

        public void Exception(Exception exception)
        {
        }

        public void Debug(string message, params object[] arguments)
        {
        }

        public void Info(string message)
        {
            
        }

        public void Error(string message)
        {

        }

        public void Debug(string message)
        {

        }
    }
}