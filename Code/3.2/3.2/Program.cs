class Program
{
    public static void Main(string[] args)
    {

        Int32[] M = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 5, 6, 3 };
        HashSet<Int32> set = new();

        Console.Write("Данные в массиве М: ");

        for(int i  = 0; i < M.Length; i++)
        {
            Console.Write(M[i] + " ");
        }

        Console.WriteLine();

        for (Int32 i = 0; i < M.Length; i++)
        {
            if (set.Contains(M[i]))
            {
                Console.WriteLine("Копия: " + M[i]);
            }
            else
            {
                set.Add(M[i]);
            }
        }

        Console.ReadLine();
    }
}