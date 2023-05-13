using CanvasComponent.Abstract;
using CanvasComponent.Converters;
using CanvasComponent.Model;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CanvasComponent.Service
{
    /// <summary>
    /// Imports and exports the project
    /// </summary>
    public class Importer : IImporter
    {
        private IJSRuntime js;
        private readonly JsonSerializerOptions jsonSettings = new()
        {
            WriteIndented = true,
            IgnoreReadOnlyProperties = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IncludeFields = true
        };

        public Importer(IJSRuntime js)
        {
            this.js = js;
            Task.Run(CreateBlazorDownloadFile);
            jsonSettings.Converters.Add(new ProjectConverter());
            jsonSettings.Converters.Add(new ISmartDeviceConverter());
            jsonSettings.Converters.Add(new LineConverter());
            jsonSettings.Converters.Add(new RoomConverter());
        }

        private async Task CreateBlazorDownloadFile()
        {
            var function = "var tag = document.createElement(\'script\'); tag.innerHTML = function BlazorDownloadFile(filename, contentType, content) { const file = new File([content], filename, { type: contentType }); const exportUrl = URL.createObjectURL(file); const a = document.createElement(\"a\" ); document.body.appendChild(a); a.href = exportUrl; a.download = filename; a.target = \"_self_\"; a.click(); URL.revokeObjectURL(exportUrl); }; document.body.appendChild(tag);";
            await js.InvokeVoidAsync("eval", function);
            function = "var tag = document.createElement(\'script\'); tag.innerHTML = function DonwloadImage(filename, content) { const a = document.createElement(\"a\" ); a.href = eval(content); a.download = filename; a.click(); }; document.body.appendChild(tag);";
            await js.InvokeVoidAsync("eval", function);
        }

        private string GetName(string name, string ext)
            => (string.IsNullOrEmpty(name) ? "project" : name) + ext;

        public async Task ExportJson(Project project)
        {
            
            var json = JsonSerializer.Serialize(project, jsonSettings);
            await js.InvokeVoidAsync("BlazorDownloadFile", GetName(project.Name, ".json"), "text/plain", json);
        }

        public async Task ExportPng(string image, string name)
        {

            await js.InvokeVoidAsync("DonwloadImage", GetName(name, ".png"), image);
        }

        public async Task Import(Project project, IRoomsCreator roomsCreator, InputFileChangeEventArgs e)
        {
            byte[] file;
            using (MemoryStream ms = new())
            {
                await e.File.OpenReadStream().CopyToAsync(ms);
                file = ms.ToArray();
            }
            string json = Encoding.UTF8.GetString(file);
            var newProject = JsonSerializer.Deserialize<Project>(json, jsonSettings);
            project.UpdateFromProject(newProject);
            roomsCreator.UpdateFromLines(newProject.Rooms.SelectMany(x => x.Lines).ToArray());
        }
    }
}
