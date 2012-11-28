using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using Application = System.Windows.Forms.Application;

namespace BindingSample
{
	class Program
	{
        [STAThread]
		static void Main(string[] args)
		{
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
		}

        private static void TestBinding()
        {
            ////1. Test Binding
            //View view1 = new View();
            //View view2 = new View();
            //Person person = new Person();
            //Person2 model2 = new Person2();

            //TextConverter converter = new TextConverter();
            //BindingEngine.SetPropertyBinding(view1, person, "Text", "Value", converter);
            //BindingEngine.SetPropertyBinding(view1, person, "Value", "Value");
            //BindingEngine.SetPropertyBinding(view2, person, "Text", "Value", converter, 2);
            //BindingEngine.SetPropertyBinding(view2, person, "Value", "Value");
            //BindingEngine.SetPropertyBinding(view2, model2, "Value2", "Value");

            //Console.WriteLine("Set Binding to view1 and view2");
            //Console.WriteLine("view1 Text = {0} \t Value = {1} \nview2 Text = {2} \t Value = {3} \t Value2 = {4}", view1.Text, view1.Value, view2.Text, view2.Value, view2.Value2);

            //person.Age = 1;
            //model2.Value = 4;
            //Console.WriteLine("\nSet model.Value = 1, model2 = 4");
            //Console.WriteLine("view1 Text = {0} \t Value = {1} \nview2 Text = {2} \t Value = {3} \t Value2 = {4}", view1.Text, view1.Value, view2.Text, view2.Value, view2.Value2);

            //Console.WriteLine("\nClear binding of view1");
            //BindingEngine.ClearPropertyBinding(view1, person, "Text", "Value");
            //BindingEngine.ClearPropertyBinding(view1, person, "Value", "Value");

            //person.Age = 3;
            //Console.WriteLine("\nSet model.Value = 3");
            //Console.WriteLine("view1 Text = {0} \t Value = {1} \nview2 Text = {2} \t Value = {3}", view1.Text, view1.Value, view2.Text, view2.Value);

            ////2. Test memory leak
            //View view3 = new View();
            //Person model3 = new Person();
            //WeakReference wr = new WeakReference(view3);
            //BindingEngine.SetPropertyBinding(view3, model3, "Text", "Value", converter, null);
            //BindingEngine.SetPropertyBinding(view3, model3, "Value", "Value");

            //Console.WriteLine("\nSet Binding to view3");
            //Console.WriteLine("The view3 is alive : {0}", wr.IsAlive);
            //view3 = null;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();

            //Console.WriteLine("Set view3 = null , after GC Collect");
            //Console.WriteLine("The view3 is alive : {0}", wr.IsAlive);

            //Console.Read();

        }
	}

	public class View
	{
		public string Text { get; set; }
		public int Value { get; set; }
		public int Value2 { get; set; }
	}
}