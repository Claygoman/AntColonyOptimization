using System.Collections.Generic;

namespace ACO
{
	public class Edge
	{
		private int _weight;

		public int Weight
		{
			get { return _weight; }
			set
			{
				_weight = value;
				InvertedWeight = 1.0 / value;
			}
		}

		public double InvertedWeight { get; set; }

		public double Pheromones { get; set; }

		public List<int> Vertices { get; set; }

		public override string ToString()
		{
			return $"Weight: {Weight,-4} Pheromones: {Pheromones,-20:0.0000000000}";
		}
	}
}