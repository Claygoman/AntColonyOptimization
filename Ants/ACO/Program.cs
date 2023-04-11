using System;

namespace ACO
{
	internal static class Program
	{
        private static void Main()
		{
            var algorithm = new Algorithm();

			if (!algorithm.LoadMatrixFromFile())
			{
				Console.WriteLine("File cannot be parsed");
				return;
			}

			algorithm.Run();
		}
	}
}