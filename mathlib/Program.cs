namespace ArkenMath
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            int running = 1;
            while (running == 1)
            {
                Console.WriteLine("What is your first number?");
                int n1;
                int.TryParse(Console.ReadLine(), out n1); // use 'out'

                Console.WriteLine("What is your second number?");
                int n2;
                int.TryParse(Console.ReadLine(), out n2); // use 'out'

                var a = new BigInteger(new uint[] { (uint)Math.Abs(n1) }, n1 >= 0 ? 1 : -1);
                var b = new BigInteger(new uint[] { (uint)Math.Abs(n2) }, n2 >= 0 ? 1 : -1);

                var result = a.Add(b); // should be +2
                Console.WriteLine(result); // +[2]
                Console.WriteLine("Enter 1 to go again or 0 to exit");
                running = int.TryParse(Console.ReadLine(), out running) ? running : 0;
                if (running != 1)
                {
                    break;
                }
            }
        }
    }
}
