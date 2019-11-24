using System;

namespace HarvestMoon
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
#if WINDOWS
        [STAThread]
#endif
        static void Main()
        {
#if WINDOWS_UAP
            var factory = new MonoGame.Framework.GameFrameworkViewSource<HarvestMoon>();
            Windows.ApplicationModel.Core.CoreApplication.Run(factory);
#elif WINDOWS
            using (var game = new HarvestMoon())
                game.Run();
#endif
        }
    }
}
