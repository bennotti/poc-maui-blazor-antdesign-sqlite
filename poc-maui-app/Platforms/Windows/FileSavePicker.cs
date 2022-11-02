using WindowsFileSaverPicker = Windows.Storage.Pickers.FileSavePicker;
using PocMaui.Core.Contract.Infra.Platforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poc_maui_app.Platforms.Windows
{
    public sealed class FileSavePicker : IFileSavePicker
    {
        public async ValueTask SaveFileAsync(string filename, Stream stream)
        {
            var extension = Path.GetExtension(filename);

            var fileSavePicker = new WindowsFileSaverPicker();
            fileSavePicker.SuggestedFileName = filename;
            fileSavePicker.FileTypeChoices.Add(extension, new List<string> { extension });

            if (MauiWinUIApplication.Current.Application.Windows[0].Handler.PlatformView is MauiWinUIWindow window)
            {
                WinRT.Interop.InitializeWithWindow.Initialize(fileSavePicker, window.WindowHandle);
            }

            var result = await fileSavePicker.PickSaveFileAsync();
            if (result != null)
            {
                using (var fileStream = await result.OpenStreamForWriteAsync())
                {
                    fileStream.SetLength(0); // override
                    await stream.CopyToAsync(fileStream);
                }
            }
        }
    }
}
