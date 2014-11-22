using System;

using Illusion.Utility;

namespace BindingEngine.Benchmark
{
    using System.Diagnostics;

    using BindingEngine = Illusion.Utility.BindingEngine;

    class Program
    {
        static void Main(string[] args)
        {
            CompareReflectAndEmitAccess();
            CompareReflectAndEmitBinding();

            Console.ReadLine();
        }

        private static void CompareReflectAndEmitAccess()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("=======  Compare Reflect and Emit Access performance =======");

            var viewModel = new TestViewModel();
            const int count = 80000;

            var reflectManager = new ReflectManager();
            var emitManager = new EmitManager();

            viewModel.TestViewModelEvent += viewModel_TestViewModelEvent;

            // 1. SetProperty
            var sw = new Stopwatch();
            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                reflectManager.SetProperty(viewModel, "Age", i);
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("1. SetProperty Reflect time elapsed {0}", sw.ElapsedMilliseconds);


            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                emitManager.SetProperty(viewModel, "Age", i);
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("1. SetProperty Emit time elapsed {0}", sw.ElapsedMilliseconds);

            // 2. GetProperty
            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                reflectManager.GetProperty(viewModel, "Age");
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("2. GetProperty Reflect time elapsed {0}", sw.ElapsedMilliseconds);


            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                emitManager.GetProperty(viewModel, "Age");
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("2. GetProperty Emit time elapsed {0}", sw.ElapsedMilliseconds);

            // 3. Get Index property
            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                reflectManager.GetIndexProperty(viewModel, 1);
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("3. GetIndexProperty Reflect time elapsed {0}", sw.ElapsedMilliseconds);


            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                emitManager.GetIndexProperty(viewModel, 1);
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("3. GetIndexProperty Emit time elapsed {0}", sw.ElapsedMilliseconds);

            // 4. Set Index property
            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                reflectManager.SetIndexProperty(viewModel, i, i);
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("4. SetIndexProperty Reflect time elapsed {0}", sw.ElapsedMilliseconds);


            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                emitManager.SetIndexProperty(viewModel, i, i);
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("4. SetIndexProperty Emit time elapsed {0}", sw.ElapsedMilliseconds);


            // 5. Create Instance
            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                reflectManager.CreateInstance<TestViewModel>();
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("5. CreateInstance<T> Reflect time elapsed {0}", sw.ElapsedMilliseconds);


            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                emitManager.CreateInstance<TestViewModel>();
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("5. CreateInstance<T> Emit time elapsed {0}", sw.ElapsedMilliseconds);

            // 6. Get method info
            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                reflectManager.GetMethodInfo(typeof(TestViewModel), "SetAge", new object[] { 1 });
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("6. GetMethodInfo Reflect time elapsed {0}", sw.ElapsedMilliseconds);


            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                emitManager.GetMethodInfo(typeof(TestViewModel), "SetAge", new object[] { 1 });
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("6. GetMethodInfo Emit time elapsed {0}", sw.ElapsedMilliseconds);

            // 7. Get Field
            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                reflectManager.GetField(viewModel, "_name");
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("7. GetField Reflect time elapsed {0}", sw.ElapsedMilliseconds);


            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                emitManager.GetField(viewModel, "_name");
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("7. GetField Emit time elapsed {0}", sw.ElapsedMilliseconds);

            // 8. Set Field
            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                reflectManager.SetField(viewModel, "_age", i);
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("8. SetField Reflect time elapsed {0}", sw.ElapsedMilliseconds);


            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                emitManager.SetField(viewModel, "_age", i);
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("8. SetField Emit time elapsed {0}", sw.ElapsedMilliseconds);

            // 9. Invoke Method
            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                reflectManager.InvokeMethod(viewModel, "SetAge", new object[] { i });
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("9. Invoke method Reflect time elapsed {0}", sw.ElapsedMilliseconds);


            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                emitManager.InvokeMethod(viewModel, "SetAge", new object[] { i });
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("9. Invoke method Emit time elapsed {0}", sw.ElapsedMilliseconds);

            // 10. Raise Event
            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                reflectManager.RaiseEvent(viewModel, "TestViewModelEvent", null);
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("10. Raise event Reflect time elapsed {0}", sw.ElapsedMilliseconds);


            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                emitManager.RaiseEvent(viewModel, "TestViewModelEvent", null);
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("10. Raise event Emit time elapsed {0}", sw.ElapsedMilliseconds);
        }

        private static void CompareReflectAndEmitBinding()
        {
            const int count = 2000;

            Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("\n=======  Compare Reflect and Emit Binding performance =======");


            var viewModels = new TestViewModel[count];
            var viewModel2s = new TestViewModel2[count];

            DynamicEngine.SetBindingManager(new ReflectManager());

            for (int i = 0; i < count; i++)
            {
                var viewModel2 = new TestViewModel2();
                var viewModel = new TestViewModel();
                viewModels[i] = viewModel;
                viewModel2s[i] = viewModel2;

                BindingEngine.SetPropertyBinding(viewModel2, testView => testView.Age, viewModel, model => model.Age);
            }

            var sw = new Stopwatch();
            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                viewModels[i].Age = i;
            }

            sw.Stop();

            Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("Reflect manager time elapsed {0}", sw.ElapsedMilliseconds);

            DynamicEngine.SetBindingManager(new EmitManager());

            for (int i = 0; i < count; i++)
            {
                var viewModel2 = new TestViewModel2();
                var viewModel = new TestViewModel();
                viewModels[i] = viewModel;
                viewModel2s[i] = viewModel2;

                BindingEngine.SetPropertyBinding(viewModel2, testView => testView.Age, viewModel, model => model.Age);
            }

            sw = new Stopwatch();
            sw.Reset();
            sw.Start();

            for (int i = 0; i < count; i++)
            {
                viewModels[i].Age = i;
            }

            sw.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("Emit manager time elapsed {0}", sw.ElapsedMilliseconds);

        }

        private static void viewModel_TestViewModelEvent(object sender, TestViewModelEventArgs e)
        {

        }
    }
}
