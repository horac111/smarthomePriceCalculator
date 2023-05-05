﻿using Blazored.Modal;
using Blazored.Modal.Services;

namespace SmartHomeCalculator
{
    internal class TestingNamingService : INamingService
    {
        public async Task<ModalResult> ShowInputText(INamed toName, string text, ModalPosition position = ModalPosition.BottomLeft)
        {
            return await Task.FromResult(ModalResult.Ok());
        }
    }
}