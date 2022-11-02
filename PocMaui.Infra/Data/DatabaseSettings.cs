using PocMaui.Core.Contract.Infra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocMaui.Infra.Data
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string Connection { get; set; } = string.Empty;
    }
}
