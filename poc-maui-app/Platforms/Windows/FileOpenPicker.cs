using WindowsFileOpenPicker = Windows.Storage.Pickers.FileOpenPicker;
using PocMaui.Core.Contract.Infra.Platforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poc_maui_app.Platforms.Windows
{
    public sealed class FileOpenPicker : IFileOpenPicker
    {
        public Task OpenFile()
        {
            return Task.CompletedTask;
        }
    }
}
