using System;

namespace Xspace
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Xspace game = new Xspace())
            {
                game.Run();
            }
        }
    }
#endif
}

