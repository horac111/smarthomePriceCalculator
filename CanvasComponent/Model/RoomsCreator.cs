using CanvasComponent.Abstract;
using CanvasComponent.EventArguments;
using CanvasComponent.Extensions;
using CanvasComponent.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CanvasComponent.Model
{
    public class RoomsCreator : IRoomsCreator
    {
        internal protected List<Line> AllLines { get; set; } = new();

        internal protected List<Room> Rooms { get; private set; } = new();

        internal IEnumerable<Line> LinesWithoutRoom { get => AllLines.Except(Rooms.SelectMany(x => x.Lines)); }

        internal protected double AutoComplete { get; set; } = 5;

        public event EventHandler<NewRoomsEventArgs> NewRooms;

        protected void onNewRoom(IEnumerable<Room> rooms)
        => NewRooms?.Invoke(this, new(rooms));

        protected internal virtual void NewLines(object sender, NewLinesEventArgs e)
        {
            if (e.Lines is not null && e.Lines.Count() > -1)
            {
                var completedPoints = AllLines.Concat(e.Lines)
                    .CloseLines(e.Lines.SelectMany(x => new[] { x.Start, x.End }).ToArray(), AutoComplete);
                List<Line> lines = new();
                for (int i = 0; i < e.Lines.Count() * 2; i += 2)
                    if (!AllLines.Any(x => x.IsPointOnLine(completedPoints[i]) && x.IsPointOnLine(completedPoints[i + 1])))
                        lines.Add(new(completedPoints[i], completedPoints[i + 1]));

                AllLines.AddRange(lines);
                detectClosedComponent();
            }
        }

        protected virtual void detectClosedComponent()
        {
            var graph = createGraph();
            HashSet<Point> visited = new();
            HashSet<Room> newRooms = new();
            while (graph.Keys.Count() > visited.Count)
            {
                var point = graph.Keys.SkipWhile(x => visited.Contains(x)).First();
                createRooms(graph, new(), point, newRooms, visited, default);
            }
            var toAdd = newRooms.Except(Rooms);
            if(toAdd.Count() > 0)
            {
                foreach (Room room in Rooms.Concat(toAdd))
                    foreach (Room room2 in Rooms.Concat(toAdd))
                    {
                        if (!room.Equals(room2) && !room2.Insiders.Contains(room) && room.Contains(room2))
                            room.Insiders.Add(room2);
                    }

                onNewRoom(toAdd);
                Rooms.AddRange(toAdd);
            }
        }

        protected virtual void createRooms(Dictionary<Point, List<Point>> graph, List<Point> path, Point currentPoint,
            HashSet<Room> rooms, HashSet<Point> visited, Point noReturn)
        {
            visited.Add(currentPoint);
            path.Add(currentPoint);
            foreach (var point in graph[currentPoint])
            {
                if(point != noReturn)
                {
                    if (visited.Contains(point) && path.Contains(point))
                    {
                        var newRoom = path.SkipWhile(x => x != point);
                        if(newRoom.Count() >= 3)
                            rooms.Add(createRoom(newRoom.Append(point)));
                        return;
                    }
                    else
                    {
                        createRooms(graph, path, point, rooms, visited, currentPoint);
                        path.Remove(point);
                    }
                        
                }
                    
            }
        }

        protected virtual Room createRoom(IEnumerable<Point> points)
        {
            List<Line> toRoom = new();
            Point previousPoint = default;
            Line currentLine = default;
            foreach (var point in points)
            {
                if (currentLine == default)
                {
                    if (previousPoint != default)
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
            if (!currentLine.IsPointOnExtendedLine(toRoom.First().End))
                toRoom.Add(currentLine);
            else
                toRoom.First().Start = currentLine.Start;
            return new(toRoom);
        }

        protected virtual Dictionary<Point, List<Point>> createGraph()
        {
            var dict = AllLines.Select((x, i) => new {line = x, index = i})
                .ToDictionary(x => x.index, x => new List<Line>() { x.line});
            for (int i = 0; i < AllLines.Count(); i++)
            {
                for (int j = i + 1; j < AllLines.Count(); j++)
                {
                    var firstLine = AllLines[i];
                    var secondLine = AllLines[j];
                    var point = firstLine.Intersection(secondLine);
                    if (point != default)
                    {
                        if(firstLine.End != point && firstLine.Start != point)
                            addToDict(point, dict[i]);
                        if (secondLine.End != point && secondLine.Start != point)
                            addToDict(point, dict[j]);
                    }
                }
            }
            Dictionary<Point, List<Point>> pairs = new();
            foreach (var line in dict.SelectMany(x => x.Value))
            {
                if (pairs.ContainsKey(line.Start))
                    pairs[line.Start].Add(line.End);
                else
                    pairs.Add(line.Start, new List<Point>() { line.End});
                if (pairs.ContainsKey(line.End))
                    pairs[line.End].Add(line.Start);
                else
                    pairs.Add(line.End, new List<Point>() { line.Start });
            }
            return pairs;
        }

        private void addToDict(Point point, List<Line> lines)
        {
            for(int i = 0; i < lines.Count; i++)
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
    }


}
