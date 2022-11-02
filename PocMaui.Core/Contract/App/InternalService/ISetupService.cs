using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocMaui.Core.Contract.App.InternalService
{
    public interface ISetupService
    {
        Task FecharBancoDados();
        Task CriarBancoDados();
        Task<string> ObterSessao();
    }
}
