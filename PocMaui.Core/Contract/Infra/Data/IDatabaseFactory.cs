using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocMaui.Core.Contract.Infra.Data
{
    public interface IDatabaseFactory
    {
        IDbConnection GetDbConnection { get; }
    }
}
