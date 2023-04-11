using System.Collections.Generic;
using System.Linq;

namespace ACO
{
	public class Ant
	{
		public List<int> VisitedVertices { get; } = new List<int> {0};
		public List<int> UsedEdges { get; } = new List<int> {-1};
		public int PathWeight { get; set; }

		public override string ToString()
		{
			return $"Now in vertex #{VisitedVertices.Last()}";
		}
	}
}