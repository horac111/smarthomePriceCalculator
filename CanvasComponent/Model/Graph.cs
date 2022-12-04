using System.Collections.Generic;
using System.Linq;

namespace CanvasComponent.Model
{
    public class Graph
    {
        private Dictionary<Point, bool> blocked;
        private Dictionary<Point, HashSet<Point>> blockChain;
        private Dictionary<Point, List<Point>> scc;
        private Point root;
        private Stack<Point> path = new();
        private List<List<Point>> cycles = new();

        private IEnumerable<Dictionary<Point, List<Point>>> sccs;

        public Graph(IList<Line> lines)
        {
            var graph = CreateGraph(lines);
            sccs = SplitToSCC(graph).ToArray();
        }

        public IEnumerable<Room> CreateRooms()
        {
            foreach (var scc in sccs)
            {
                blocked = scc.ToDictionary(x => x.Key, x => false);
                blockChain = scc.ToDictionary(x => x.Key, x => new HashSet<Point>());
                this.scc = scc;
                foreach (var result in Find())
                    yield return result;
            }
            yield break;
        }

        private void Unblock(Point point)
        {
            blocked[point] = false;
            foreach (var unblock in blockChain[point])
            {
                blockChain[point].Remove(unblock);
                if (blocked[unblock])
                    Unblock(unblock);
            }
        }

        private bool Circuit(Point point)
        {
            bool found = false;
            path.Push(point);
            blocked[point] = true;
            foreach (var nextPoint in scc[point])
            {
                if (nextPoint == root)
                {
                    if (path.Count > 3)
                        cycles.Add(path.ToList());
                    found = true;
                }
                else if (!blocked[nextPoint])
                    if (Circuit(nextPoint))
                        found = true;
            }
            if (found)
                Unblock(point);
            else
                foreach (var key in scc[point])
                    blockChain[key].Add(point);
            path.Pop();
            return found;
        }

        private IEnumerable<Room> Find()
        {
            foreach (var key in scc.Keys.ToList())
            {
                root = key;
                foreach (var toClear in scc.Keys)
                {
                    blockChain[toClear].Clear();
                    blocked[toClear] = false;
                }
                List<List<Point>> circuits = new();
                Circuit(key);
                scc.Remove(key);
                foreach (var keyValuePair in scc)
                    if (keyValuePair.Value.Contains(key))
                        keyValuePair.Value.Remove(key);

            }
            foreach (var cycle in cycles)
                yield return CreateRoom(cycle);
            cycles.Clear();
        }

        protected virtual Dictionary<Point, List<Point>> CreateGraph(IList<Line> lines)
        {
            var dict = lines.Select((x, i) => new { line = x, index = i })
                .ToDictionary(x => x.index, x => new List<Line>() { x.line });
            for (int i = 0; i < lines.Count(); i++)
            {
                for (int j = i + 1; j < lines.Count(); j++)
                {
                    var firstLine = lines[i];
                    var secondLine = lines[j];
                    var point = firstLine.Intersection(secondLine);
                    if (point != default)
                    {
                        if (firstLine.End != point && firstLine.Start != point)
                            AddToList(point, dict[i]);
                        if (secondLine.End != point && secondLine.Start != point)
                            AddToList(point, dict[j]);
                    }
                }
            }
            Dictionary<Point, List<Point>> pairs = new();
            foreach (var line in dict.SelectMany(x => x.Value))
            {
                if (pairs.ContainsKey(line.Start))
                    pairs[line.Start].Add(line.End);
                else
                    pairs.Add(line.Start, new List<Point>() { line.End });
                if (pairs.ContainsKey(line.End))
                    pairs[line.End].Add(line.Start);
                else
                    pairs.Add(line.End, new List<Point>() { line.Start });
            }
            return pairs;
        }

        private void AddToList(Point point, List<Line> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.IsPointOnLine(point))
                {
                    lines.RemoveAt(i);
                    lines.Insert(i, new Line(point, line.End));
                    lines.Insert(i, new Line(line.Start, point));
                    return;
                }
            }
        }

        private IEnumerable<Dictionary<Point, List<Point>>> SplitToSCC(Dictionary<Point, List<Point>> graph)
        {
            if (graph is null || graph.Count == 0)
                yield break;

            HashSet<Point> visited = new();
            Stack<Point> toVisit = new();
            var first = graph.First().Key;
            toVisit.Push(first);
            visited.Add(first);
            do
            {
                var root = toVisit.Pop();
                foreach (var point in graph[root])
                {
                    if (!visited.Contains(point))
                    {
                        toVisit.Push(point);
                        visited.Add(point);
                    }
                }
            } while (toVisit.Count() is not 0);

            yield return new(graph.Where(x => visited.Contains(x.Key)));
            foreach (var result in SplitToSCC(new(graph.Where(x => !visited.Contains(x.Key)))))
                yield return result;
        }

        private Room CreateRoom(IEnumerable<Point> points)
        {
            List<Line> toRoom = new();
            Point previousPoint = points.Last();
            Line currentLine = default;
            foreach (var point in points)
            {
                if (currentLine == default)
                {
                    currentLine = new(previousPoint, point);
                }
                else
                {
                    if (currentLine.IsPointOnExtendedLine(point))
                        currentLine = new(currentLine.Start, point);
                    else
                    {
                        toRoom.Add(currentLine);
                        currentLine = new(currentLine.End, point);
                    }
                }
                previousPoint = point;
            }
            toRoom.Add(currentLine);
            return new(toRoom);
        }
    }
}
