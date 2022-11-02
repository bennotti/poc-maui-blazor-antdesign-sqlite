using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocMaui.Core.Contract.Infra.Platforms
{
    public interface IFileOpenPicker
    {
        Task OpenFile();
    }
}
