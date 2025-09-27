namespace ArkenMath
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter the first big integer:");
            string input1 = Console.ReadLine() ?? "0";

            Console.WriteLine("Enter the second big integer:");
            string input2 = Console.ReadLine() ?? "0";
            BigInteger sum = 0;
            try
            {
                BigInteger num1 = new BigInteger(input1);
                BigInteger num2 = new BigInteger(input2);

                sum = num1.Add(num2);

                Console.WriteLine($"The sum is: {sum}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            BigInteger a = new("1234");
            BigInteger b = new("4321");
            sum = a.Add(b);
            Console.WriteLine($"Sum: {sum}");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
