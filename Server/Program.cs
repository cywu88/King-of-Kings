using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Server p = new Server();
            while (true)
            {
                p.Update();
            }
        }
    }
}
