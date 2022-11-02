using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocMaui.Core.Contract.Infra.Data
{
    public interface IDatabaseSettings
    {
        string Connection { get; set; }
    }
}
