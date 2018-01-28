using System;

namespace Upp.Net.Trace
{
    public interface ITrace
    {
        void Info(string message);
        void Info(string message, params object[] arguments);
        void Error(string message);
        void Error(string message, params object[] arguments);
        void Exception(Exception exception);
        void Debug(string message);
        void Debug(string message, params object[] arguments);
    }
}