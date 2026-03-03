#nullable enable

using System.Threading;
using System.Diagnostics;

namespace AWPS.IoT
{
    public static class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello world!");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}