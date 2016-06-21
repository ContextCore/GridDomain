using System;
using System.Collections.Generic;
using GridDomain.Scheduling.Integration;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestLogger : ISoloLogger 
    {
        private static List<string> _errorMessages = new List<string>();
        private static List<string> _debugMessages = new List<string>();
        private static List<string> _infoMessages = new List<string>();

        static TestLogger()
        {
            Init();
        }

        private static void Init()
        {
            _errorMessages = new List<string>();
            _debugMessages = new List<string>();
            _infoMessages = new List<string>();
        }

        public void Debug(string message)
        {
            lock (_debugMessages)
            {
                _debugMessages.Add(message);
            }
        }

        public void Info(string message)
        {
            lock (_infoMessages)
            {
                _infoMessages.Add(message);
            }
        }

        public void Error(Exception error, string message)
        {
            lock (_errorMessages)
            {
                _errorMessages.Add(message);
            }
        }

        public static string[] ErrorMessages
        {
            get
            {
                lock (_errorMessages)
                {
                    return _errorMessages.ToArray();
                }
            }
        }

        public static string[] InfoMessages
        {
            get
            {
                lock (_infoMessages)
                {
                    return _infoMessages.ToArray();
                }
            }
        }

        public static string[] DebugMessages
        {
            get
            {
                lock (_debugMessages)
                {
                    return _debugMessages.ToArray();
                }
            }
        }

        public static void Clear()
        {
            Init();
        }
    }
}