namespace Illusion.Sample
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    ///     Class Program.
    /// </summary>
    internal class Program
    {
        #region Methods

        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        /// <param name="args">
        ///     The arguments.
        /// </param>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        #endregion
    }
}