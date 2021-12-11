using System;
using System.IO;
namespace LmcSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter file location: ");
            
            var ASM = File.ReadAllLines(Console.ReadLine());
            var temp = new AsmCompiler(ASM);
            try
            {
                temp.Compile();
                temp.DisplayRAMContents();

            }
            catch (AsmException e)
            {
                Console.WriteLine($"{e.GetType()}: {e.Message}");
                Console.Read();
            }

            var computer = new LmcComputer();
            computer.LoadRAM(temp.GetMachineRAM());
            Console.WriteLine("press enter to run");
            Console.ReadLine();
            computer.Run();
            Console.WriteLine("Halted");
        }
    }
}
