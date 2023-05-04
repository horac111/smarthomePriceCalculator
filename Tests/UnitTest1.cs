using CanvasComponent.Enums;
using CanvasComponent.Facade;
using CanvasComponent.Model.SmartDevice;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using SmartHomeCalculator;

namespace Tests
{
    public class Tests
    {
        private CanvasFacade facade;
        private List<ISmartDevice> devices;
        private DrawByStyle drawByStyle;
        private RoomsCreator roomsCreator;
        private Project project;

        [SetUp]
        public void Setup()
        {
            devices = new List<ISmartDevice>()
            {
                new DevicesPerMeterSmartDevice()
                {
                    Id = 0,
                    BasePrice = 1,
                    DevicesPerMeter = 1,
                    Price = 1,
                }
            };
            drawByStyle = new DrawByStyle();
            roomsCreator = new RoomsCreator();
            project = new(devices);
            facade = new(drawByStyle, roomsCreator, new TestingDrawing(),
                project, new TestingNamingService(), new TestingImporter());
        }

        [Test]
        public void AddLine()
        {
            facade.SelectedDrawingStyle = (int)DrawingStyle.StraightLine;
            List<Line> lines = new()
            {
                new(new(0,0), new(2,2))
            };
            bool isLine = false;
            drawByStyle.NewLines += (s, e) =>
            {
                Assert.IsTrue(lines.Count == e.Lines.Count());
                for (int i = 0; i < lines.Count; i++)
                    Assert.That(lines[i], Is.EqualTo(e.Lines.ElementAt(i)));
                isLine = true;
            };
            facade.OnMouseDown(new() { ClientX = 0, ClientY = 0, Button = 0 });
            facade.OnMouseDown(new() { ClientX = 2, ClientY = 2, Button = 0 });
            Assert.IsTrue(isLine);
        }

        [Test]
        public void AddRectangle()
        {
            facade.SelectedDrawingStyle = (int)DrawingStyle.Rectangle;
            List<Line> lines = new()
            {
                new(new(0,0), new(0,2)),
                new(new(0,0), new(2,0)),
                new(new(2,0), new(2,2)),
                new(new(0,2), new(2,2)),
            };
            bool isLine = false;
            drawByStyle.NewLines += (s, e) =>
            {
                CollectionAssert.AreEquivalent(lines, e.Lines);
                isLine = true;
            };
            facade.OnMouseDown(new() { ClientX = 0, ClientY = 0, Button = 0 });
            facade.OnMouseDown(new() { ClientX = 2, ClientY = 2, Button = 0 });
            Assert.IsTrue(isLine);
        }

        [Test]
        public void AddPolygon()
        {
            facade.SelectedDrawingStyle = (int)DrawingStyle.StraightLine;
            List<Line> lines = new()
            {
                new(new(0,0), new(2,2))
            };
            bool isLine = false;
            drawByStyle.NewLines += (s, e) =>
            {
                CollectionAssert.AreEquivalent(lines, e.Lines);
                isLine = true;
            };
            facade.OnMouseDown(new() { ClientX = 0, ClientY = 0, Button = 0 });
            facade.OnMouseDown(new() { ClientX = 2, ClientY = 2, Button = 0 });
            Assert.IsTrue(isLine);
        }
    }
}