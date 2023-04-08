using CanvasComponent.Abstract;
using CanvasComponent.EventArguments;
using CanvasComponent.Extensions;
using CanvasComponent.Model;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanvasComponent.Service
{
    public class RoomsCreator : RoomsCreatorBase
    {
        private SteppingEnumerable<Line> allLines = new();
        public override IEnumerable<Line> AllLines { get => allLines; }
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

                allLines.AddRange(lines);
                OnPropertyChanged(nameof(AllLines));
                DetectClosedComponent();
            }
        }

        protected virtual void DetectClosedComponent()
        {
            Graph graph = new(AllLines);
            var newRooms = graph.CreateRooms().ToList();
            if (newRooms.Count() > 0)
            {
                OnRoomsFound(newRooms);
            }
        }
        public override void OnStep(object sender, StepEventArgs e)
        {
            allLines.Step(e.Forward);
            OnPropertyChanged(nameof(AllLines));
        }
    }


}
