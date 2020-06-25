using System;

namespace FIFA20_Ultimate_Team_AutoBuyer.Tasks
{
    class HandledException : Exception
    {
        public bool ForceDisconnect { get; }
        public string Message { get; }
        public int Delay { get; }
        public bool ClearSessionID { get; }

        public HandledException(string message = null, bool disconnect = false, int delay = 0, bool clearSessionID = false)
        {
            Delay = delay;
            Message = message;
            ForceDisconnect = disconnect;
            ClearSessionID = clearSessionID;
        }
    }
}