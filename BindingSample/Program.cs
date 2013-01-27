using System;
using Application = System.Windows.Forms.Application;

namespace BindingSample
{
	class Program
	{
        [STAThread]
		static void Main(string[] args)
		{
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
		}
	}
}