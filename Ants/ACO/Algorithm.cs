using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ACO
{
	public class Algorithm
	{
		#region Constants: private

		private const double Alpha = .5;
		private const double Beta = .5;
		private const double Q = 1;
		private const double Rho = .1;

		#endregion

		#region Properties: private

		private int ProblemSize { get; set; } = -1;

		private int IterationsNumber = 0;

		private List<Vertex> Vertices { get; } = new List<Vertex>();

		private List<Edge> Edges { get; } = new List<Edge>();

		private List<Ant> Ants { get; } = new List<Ant>();

		#endregion

		public bool LoadMatrixFromFile()
        {

            DirectoryInfo directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;

            string folderName = "Test\\";
            string folderPath = Path.Combine(directoryInfo.FullName, folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string[] files = Directory.GetFiles(folderPath);

            int selectedFileIdx = 0;
            while (selectedFileIdx <= 0 || selectedFileIdx > files.Length)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {Path.GetFileName(files[i])}");
                }
                int.TryParse(Console.ReadKey().KeyChar.ToString(), out selectedFileIdx);
                Console.Clear();
            }
            string file = Path.Combine(folderPath, files[selectedFileIdx - 1]);

            Console.Write("Сколько итераций? ");
            int.TryParse(Console.ReadLine(), out IterationsNumber);

            using (var sr = new StreamReader(file))
			{
				string line;
				var random = new Random();
				while ((line = sr.ReadLine()) != null)
				{
                    if (line.Length == 0)
                    {
                        continue;
                    }

                    MatchCollection weights = Regex.Matches(line, @"\d+");

                    if (ProblemSize == -1)
                    {
                        ProblemSize = weights.Count;
                    }

                    if (weights.Count != ProblemSize)
                    {
                        return false;
                    }

                    var vertex = new Vertex();
                    for (var i = 0; i < weights.Count; i++)
                    {
                        var weight = Convert.ToInt32(weights[i].Value);
                        if (weight == 0)
                        {
                            vertex.EdgesIndexes.Add(-1);
                            continue;
                        }

                        if (i < Vertices.Count)
                        {
                            vertex.EdgesIndexes.Add(Vertices[i].EdgesIndexes[Vertices.Count]);
                            continue;
                        }

                        double randomPheromone = random.NextDouble();
                        Edges.Add(new Edge
                        {
                            Weight = weight,
                            Pheromones = randomPheromone,
                            Vertices = new List<int> { i, Vertices.Count }
                        });
                        vertex.EdgesIndexes.Add(Edges.Count - 1);
                    }

                    Vertices.Add(vertex);
                }
			}

			return true;
		}

		private void Init(int colonySize)
		{
			for (var i = 0; i < colonySize; i++)
			{
				Ants.Add(new Ant());
			}
		}

		public void Run()
		{
			var random = new Random();
			var bestWeight = int.MaxValue;
			var bestWay = string.Empty;

            Console.Write("Сколько муравьев в колонне? ");
            int.TryParse(Console.ReadLine(), out int colonySize);

			Init(colonySize);

            for (var iter = 0; iter < IterationsNumber; iter++)
			{
				// Испарение ферамонов
				foreach (Edge edge in Edges)
				{
					edge.Pheromones *= 1 - Rho;
				}

				foreach (Ant ant in Ants)
				{
					// Генерация решения
					var n = 0.0;

					for (var i = 0; i < ProblemSize; i++)
					{
						int edgeIdx = Vertices[ant.VisitedVertices.Last()].EdgesIndexes[i];
						if (edgeIdx == -1 || ant.VisitedVertices.Contains(Edges[edgeIdx].Vertices.First(t => t != ant.VisitedVertices.Last())))
						{
							continue;
						}

						n += Math.Pow(Edges[edgeIdx].InvertedWeight, Alpha) *
						     Math.Pow(Edges[edgeIdx].Pheromones, Beta);
					}

					for (var i = 0; i < ProblemSize; i++)
					{
						int edgeIdx = Vertices[ant.VisitedVertices.Last()].EdgesIndexes[i];
						if (edgeIdx == -1 || ant.VisitedVertices.Contains(Edges[edgeIdx].Vertices.First(t => t != ant.VisitedVertices.Last())))
						{
							continue;
						}

						double p = Math.Pow(Edges[edgeIdx].InvertedWeight, Alpha) *
						           Math.Pow(Edges[edgeIdx].Pheromones, Beta) / n;
						if (!(random.NextDouble() <= p))
						{
							continue;
						}

						ant.VisitedVertices.Add(i);
						ant.UsedEdges.Add(edgeIdx);
						ant.PathWeight += Edges[edgeIdx].Weight;
						break;
					}

					// Обновление ферамонов
					if (ant.VisitedVertices.Last() == ProblemSize - 1)
					{
						double delta = Q / ant.PathWeight;
						foreach (int edgeIdx in ant.UsedEdges.Where(edgeIdx => edgeIdx != -1))
						{
							Edges[edgeIdx].Pheromones += delta;
						}

						if (ant.PathWeight < bestWeight)
						{
							bestWeight = ant.PathWeight;
							bestWay = string.Join(", ", ant.VisitedVertices.ToArray());
						}

						ant.VisitedVertices.Clear();
						ant.VisitedVertices.Add(0);
						ant.UsedEdges.Clear();
						ant.UsedEdges.Add(-1);
						ant.PathWeight = 0;
					}
				}

				Console.WriteLine($"#{iter,-4} Наименьший вес: {bestWeight}");
			}

			Console.WriteLine($"\nКротчайший путь: {bestWay}\nНаименьший вес: {bestWeight}");
			Console.ReadKey();
		}
	}
}