using CanvasComponent.Model;
using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;

namespace CanvasComponent.Abstract
{
    /// <summary>
    /// Imports and exports the project
    /// </summary>
    public interface IImporter
    {
        Task ExportJson(Project project);
        Task ExportPng(string image, string name);
        Task Import(Project project, IRoomsCreator roomsCreator, InputFileChangeEventArgs e);
    }
}