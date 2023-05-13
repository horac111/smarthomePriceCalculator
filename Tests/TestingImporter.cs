using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasComponentTests
{
    internal class TestingImporter : IImporter
    {
        public async Task ExportJson(Project project)
        {
            await Task.FromResult(true);
        }

        public async Task ExportPng(string image, string name)
        {
            await Task.FromResult(true);
        }

        public async Task Import(Project project, IRoomsCreator roomsCreator, InputFileChangeEventArgs e)
        {
            await Task.FromResult(true);
        }
    }
}
