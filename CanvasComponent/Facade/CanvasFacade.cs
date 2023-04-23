﻿using Blazored.Modal;
using Blazored.Modal.Services;
using CanvasComponent.Abstract;
using CanvasComponent.EventArguments;
using CanvasComponent.Model;
using CanvasComponent.Service;
using Excubo.Blazor.Canvas;
using Excubo.Blazor.Canvas.Contexts;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CanvasComponent.Facade
{
    /// <summary>
    /// Facade that puts together all the project parts.
    /// </summary>
    public class CanvasFacade : NotifyPropertyChanged, IDisposable, ICanvasFacade
    {
        #region private
        private DrawByStyleBase drawByStyle;

        private RoomsCreatorBase roomsCreator;

        private IDrawing drawing;
        private INamingService namingService;
        private IImporter importer;

        private Project project;

        private DragAndDropService<ISmartDevice> dragAndDrop;
        #endregion
        #region internal

        internal const string CanvasID = "Canvas";
        #endregion
        #region public
        /// <summary>
        /// DrawingHelper trasforms pixels first from display to canvas then pixels to meters
        /// </summary>
        public IDrawingHelper DrawingHelper { get; set; }

        /// <summary>
        /// Id of the Currently selected style
        /// </summary>
        public int SelectedDrawingStyle
        {
            get => drawByStyle.SelectedDrawingStyle;
            set => drawByStyle.SelectedDrawingStyle = value;
        }

        /// <summary>
        /// Grid mesh size in pixels
        /// </summary>
        public double GridDensity
        {
            get => drawByStyle.GridDensity;
            set => drawByStyle.GridDensity = value;
        }

        /// <summary>
        /// Indicates if the new lines should snap to the grid(even if the grid is not visible)
        /// </summary>
        public bool SnapToGrid
        {
            get => drawByStyle.SnapToGrid;
            set => drawByStyle.SnapToGrid = value;
        }

        /// <summary>
        /// All available drawing styles
        /// </summary>
        public IEnumerable<INamedValue<int>> DrawingTypes
        {
            get => drawByStyle.DrawingStyles.Keys;
        }

        /// <summary>
        /// Indicates from close the new line must be to existing to snap to it 
        /// </summary>
        public double AutoComplete
        {
            get => roomsCreator.AutoComplete;
            set => roomsCreator.AutoComplete = value;
        }

        /// <summary>
        /// All existing rooms
        /// </summary>
        public IEnumerable<Room> Rooms
        {
            get => project.Rooms;
        }

        /// <summary>
        /// Thickness of the drawing line
        /// </summary>
        public double Thickness
        {
            get => drawing.Thickness;
            set => drawing.Thickness = value;
        }

        /// <summary>
        /// Indicates if the grid should be drawn to canvas
        /// </summary>
        public bool ShowGrid
        {
            get => drawing.ShowGrid;
            set => drawing.ShowGrid = value;
        }

        /// <summary>
        /// Indicates if redo is available
        /// </summary>
        public bool CanStepForward
        {
            get => project.CanStepForward;
        }

        /// <summary>
        /// Indicates if undo is available
        /// </summary>
        public bool CanStepBackward
        {
            get => project.CanStepBackward;
        }

        /// <summary>
        /// Collection of all available smart devices
        /// </summary>
        public IEnumerable<ISmartDevice> Devices
        {
            get => project.Devices;
        }

        /// <summary>
        /// Price of the devices in rooms
        /// </summary>
        public double TotalPrice
        {
            get => project.TotalPrice;
        }

        /// <summary>
        /// Reference to the Canvas
        /// </summary>
        public Canvas Canvas
        {
            get => drawing.Canvas;
            set => drawing.Canvas = value;
        }

        /// <summary>
        /// Undo or redo
        /// </summary>
        public event EventHandler<StepEventArgs> Step;
        #endregion


        #region Contructors

        public CanvasFacade(Project project, IJSRuntime js, IModalService modalService)
            : this(new DrawByStyle(), new RoomsCreator(), new Drawing(js), project,
                  new NamingService(modalService), new Importer(js))
        {


        }

        public CanvasFacade(DrawByStyleBase byStyle, RoomsCreatorBase roomsCreator,
            IDrawing drawing, Project project, INamingService namingService, IImporter importer)
        {
            if (byStyle is null)
                throw new ArgumentNullException(nameof(byStyle));
            if (roomsCreator is null)
                throw new ArgumentNullException(nameof(roomsCreator));
            if (drawing is null)
                throw new ArgumentNullException(nameof(drawing));
            if (project is null)
                throw new ArgumentNullException(nameof(project));
            if (namingService is null)
                throw new ArgumentNullException(nameof(namingService));

            drawByStyle = byStyle;
            this.roomsCreator = roomsCreator;
            this.drawing = drawing;
            dragAndDrop = new();
            this.project = project;
            this.namingService = namingService;
            drawByStyle.Draw += DrawCalled;
            drawByStyle.NewLines += roomsCreator.NewLines;
            roomsCreator.RoomsFound += project.OnRoomsFound;
            project.NewRooms += drawing.OnNewRooms;
            drawing.NewRoom += OnNewRoom;
            drawByStyle.PropertyChanged += OnPropertyChanged;
            drawing.PropertyChanged += OnPropertyChanged;
            Step += project.OnStep;
            Step += roomsCreator.OnStep;
            this.importer = importer;
        }
        #endregion
        #region Methods

        /// <summary>
        /// This method initializes and redraws
        /// </summary>
        /// <param name="firstTime"></param>
        /// <returns></returns>
        public async Task OnAfterRender(bool firstTime)
        {
            if (firstTime)
            {
                if (string.IsNullOrEmpty(project.Name))
                    await namingService.ShowInputText(project, "Project name:", ModalPosition.Middle);

                await drawing.Initialize(DrawingHelper, project, drawByStyle, roomsCreator);
                project.Initialize(DrawingHelper);
            }
            await drawing.Draw(0);
        }

        /// <summary>
        /// Method reacts to the event NewRoom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnNewRoom(object sender, NewRoomEventArgs e)
            => await namingService.ShowInputText(e.Room, "Room name:");

        /// <summary>
        /// Method reacts to event Draw
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DrawCalled(object sender, EventArgs e)
            => await drawing.Draw();

        /// <summary>
        /// Method starts drawing or clears canvas
        /// </summary>
        /// <param name="e"></param>
        public void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == 0)
                drawByStyle.MouseDown(DrawingHelper.TransformEventArgs(e));
            else
                drawByStyle.Clear();
        }

        /// <summary>
        /// Draw temporary lines
        /// </summary>
        /// <param name="e"></param>
        public void OnMouseMove(MouseEventArgs e)
            => drawByStyle.MouseMove(DrawingHelper.TransformEventArgs(e));

        /// <summary>
        /// Smart device drag-and-drop initiated
        /// </summary>
        /// <param name="device"></param>
        /// <param name="zone"></param>
        public void OnDragStart(ISmartDevice device, string zone)
            => dragAndDrop.OnDragStart(device, zone);

        public void Dispose()
        {
            drawing?.Dispose();
            drawByStyle.Draw -= DrawCalled;
            drawByStyle.NewLines -= roomsCreator.NewLines;
            roomsCreator.RoomsFound -= project.OnRoomsFound;
            project.NewRooms -= drawing.OnNewRooms;
            drawByStyle.PropertyChanged -= OnPropertyChanged;
            drawing.PropertyChanged -= OnPropertyChanged;
        }

        /// <summary>
        /// Smart device drag-and-drop finished
        /// </summary>
        /// <param name="e"></param>
        /// <param name="zone"></param>
        public void OnDragDrop(DragEventArgs e, string zone)
            => project.OnDragDrop(e, dragAndDrop.OnDrop(zone));


        /// <summary>
        /// Clear - esc, ctrl + z - undo, ctrl + y - redo
        /// </summary>
        /// <param name="e"></param>
        public void OnKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
                drawByStyle.Clear();
            else if (e.CtrlKey && e.Key == "z")
                OnStep(false);
            else if (e.CtrlKey && e.Key == "y")
                OnStep(true);
        }

        /// <summary>
        /// Invokes Step event e.g. redo or undo
        /// </summary>
        /// <param name="forward"></param>
        public void OnStep(bool forward)
             => Step.Invoke(this, new(forward));

        /// <summary>
        /// Downloads project as json
        /// </summary>
        public async void ExportJson()
            => await importer.ExportJson(project);

        /// <summary>
        /// Downloads project as png
        /// </summary>
        public async void ExportPng()
           => await importer.ExportPng(await drawing.GetCanvasAsImage(), project.Name);

        /// <summary>
        /// Imports the project
        /// </summary>
        /// <param name="e"></param>
        public async void ImportJson(InputFileChangeEventArgs e)
            => await importer.Import(project, roomsCreator, e);
        #endregion
    }
}
