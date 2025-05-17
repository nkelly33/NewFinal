using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewFinal.Services
{
    public class OutputManager
    {
        private readonly List<string> _outputBuffer; // A list of messages

        public OutputManager()
        {
            _outputBuffer = new List<string>();
        }

        public void Clear()
        {
            Console.Clear();
            _outputBuffer.Clear();
        }

        public void Display()
        {
            foreach (var message in _outputBuffer)
            {
                Console.Write(message); // Write stored messages without color
            }

            _outputBuffer.Clear(); // Clear the buffer after displaying
        }

        public void Write(string message)
        {
            _outputBuffer.Add(message);
        }

        public void WriteLine(string message)
        {
            _outputBuffer.Add(message + Environment.NewLine);
        }
    }
}
