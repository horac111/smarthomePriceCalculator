﻿using CanvasComponent.EventArguments;
using CanvasComponent.Model;
using CanvasComponent.Service;
using Excubo.Blazor.Canvas;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CanvasComponent.Abstract
{
    /// <summary>
    /// Async EventHandler for application to wait on the completition
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public delegate Task NewRoomDelegate(object sender, NewRoomEventArgs e);

    /// <summary>
    /// Facade that puts together all the project parts.
    /// </summary>
    public interface ICanvasFacade : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Indicates from close the new line must be to existing to snap to it 
        /// </summary>
        double AutoComplete { get; set; }

        /// <summary>
        /// Reference to the Canvas
        /// </summary>
        Canvas Canvas { get; set; }

        /// <summary>
        /// Collection of all available smart devices
        /// </summary>
        IEnumerable<ISmartDevice> Devices { get; }

        /// <summary>
        /// DrawingHelper trasforms pixels first from display to canvas then pixels to meters
        /// </summary>
        IDrawingHelper DrawingHelper { get; set; }

        /// <summary>
        /// All available drawing styles
        /// </summary>
        IEnumerable<INamedValue<int>> DrawingTypes { get; }

        /// <summary>
        /// Grid mesh size in pixels
        /// </summary>
        double GridDensity { get; set; }

        /// <summary>
        /// All existing rooms
        /// </summary>
        IEnumerable<Room> Rooms { get; }

        /// <summary>
        /// Id of the Currently selected style
        /// </summary>
        int SelectedDrawingStyle { get; set; }

        /// <summary>
        /// Indicates if the grid should be drawn to canvas
        /// </summary>
        bool ShowGrid { get; set; }

        /// <summary>
        /// Indicates if the new lines should snap to the grid(even if the grid is not visible)
        /// </summary>
        bool SnapToGrid { get; set; }

        /// <summary>
        /// Thickness of the drawing line
        /// </summary>
        double Thickness { get; set; }

        /// <summary>
        /// Price of the devices in rooms
        /// </summary>
        double TotalPrice { get; }

        /// <summary>
        /// Indicates if redo is available
        /// </summary>
        bool CanStepForward { get; }

        /// <summary>
        /// Indicates if undo is available
        /// </summary>
        bool CanStepBackward { get; }

        #endregion

        #region Methods

        void Dispose();
        Task OnAfterRender(bool firstTime);
        void OnDragDrop(DragEventArgs e, string zone);
        void OnDragStart(ISmartDevice device, string zone);
        void OnMouseDown(MouseEventArgs e);
        void OnMouseMove(MouseEventArgs e);
        void OnKeyDown(KeyboardEventArgs e);
        void OnStep(bool forward);
        void ExportJson();
        void ExportPng();
        void ImportJson(InputFileChangeEventArgs e);
        #endregion
    }
}