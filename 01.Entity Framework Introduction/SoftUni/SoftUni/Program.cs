using SoftUni.Data;

namespace SoftUni
{
    public class Program
    {
        static void Main(string[] args)
        {
            SoftUniContext dbContext = new SoftUniContext();
            Console.WriteLine("Connect!");
        }
    }
}