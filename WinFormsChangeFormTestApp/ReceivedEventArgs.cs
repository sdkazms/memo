using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsTestChangeFormApp
{
    public class ReceivedEventArgs : EventArgs
    {
        public string Message { get; }
        public DateTime Timestamp { get; }

        public ReceivedEventArgs(string message)
        {
            Message = message;
            Timestamp = DateTime.Now;
        }
    }
}
