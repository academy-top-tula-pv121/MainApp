using System.Reflection;
using System.Runtime.Loader;

namespace MainApp
{
    internal class Program
    {
        static void AppUnload(AssemblyLoadContext context)
        {
            Console.WriteLine("Library is unload");
        }
        static void Square(int number)
        {
            AssemblyLoadContext context = new AssemblyLoadContext("NewSquare", true);
            context.Unloading += AppUnload;

            string assemblyPath = Path.Combine(Directory.GetCurrentDirectory(), "SquareApp.dll");
            
            Assembly assembly = context.LoadFromAssemblyPath(assemblyPath);

            //Assembly assembly = context.LoadFromAssemblyName(AssemblyName.GetAssemblyName("SquareApp.dll"));

            Type? type = assembly.GetType("SquareApp.Program");
            if(type != null)
            {
                //foreach (var m in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic))
                //    Console.WriteLine(m.Name);

                var method = type.GetMethod("Square", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);

                var result = method?.Invoke(null, new object[] { number });

                if (result is int)
                    Console.WriteLine($"\nSquare {number} = {result}");
            }

            Console.WriteLine("\nBefoe unload:");
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                Console.WriteLine(asm.GetName().Name);

            context.Unload();
           

        }
        static void Main(string[] args)
        {
            Square(20);

            GC.Collect();
            //GC.WaitForPendingFinalizers();


            Console.WriteLine("\nAfter unload:");
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                Console.WriteLine(asm.GetName().Name);

            Console.ReadKey();
        }
    }
}