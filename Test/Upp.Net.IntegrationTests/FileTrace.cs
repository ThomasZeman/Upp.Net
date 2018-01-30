using System;
using System.IO;
using Upp.Net.Trace;

namespace Upp.Net.IntegrationTests
{
    public class FileTrace : ITrace, IDisposable
    {
        private readonly StreamWriter _streamWriter;
        private readonly FastTimeFormatter _fastTimeFormatter = new FastTimeFormatter();

        public FileTrace(string fileName)
        {
            _streamWriter = new StreamWriter(File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read));
        }

        public void Info(string message, params object[] arguments)
        {
            lock (_streamWriter)
            {
                _streamWriter.Write(_fastTimeFormatter.Format(DateTime.Now));
                _streamWriter.WriteLine(message, arguments);
                _streamWriter.Flush();
            }
        }

        public void Error(string message, params object[] arguments)
        {
            lock (_streamWriter)
            {
                _streamWriter.Write(_fastTimeFormatter.Format(DateTime.Now));

                _streamWriter.WriteLine(message, arguments);
                _streamWriter.Flush();
            }
        }

        public void Exception(Exception exception)
        {
            lock (_streamWriter)
            {
                _streamWriter.Write(_fastTimeFormatter.Format(DateTime.Now));
                _streamWriter.WriteLine(exception);
                _streamWriter.Flush();
            }
        }

        public void Debug(string message, params object[] arguments)
        {
            lock (_streamWriter)
            {
                _streamWriter.Write(_fastTimeFormatter.Format(DateTime.Now));

                _streamWriter.WriteLine(message, arguments);
                _streamWriter.Flush();
            }
        }

        public void Dispose()
        {
            lock (_streamWriter)
            {
                _streamWriter.Flush();
                _streamWriter.Dispose();
            }
        }

        public void Info(string message)
        {
            Info(message,new object[0]);
        }

        public void Error(string message)
        {
            Error(message, new object[0]);
        }

        public void Debug(string message)
        {
            Debug(message, new object[0]);
        }
    }
}