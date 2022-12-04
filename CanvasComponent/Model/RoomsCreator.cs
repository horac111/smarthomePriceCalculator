using CanvasComponent.EventArguments;
using CanvasComponent.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace CanvasComponent.Model
{
    public class RoomsCreator : RoomsCreatorBase
    {
        protected internal override void NewLines(object sender, NewLinesEventArgs e)
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
                DetectClosedComponent();
            }
        }

        protected virtual void DetectClosedComponent()
        {
            Graph graph = new(AllLines);
            var newRooms = graph.CreateRooms().ToList();
            var toAdd = newRooms.Except(Rooms);
            if (toAdd.Count() > 0)
            {
                OnNewRooms(toAdd);
                Rooms.AddRange(toAdd);
            }
        }


    }


}
