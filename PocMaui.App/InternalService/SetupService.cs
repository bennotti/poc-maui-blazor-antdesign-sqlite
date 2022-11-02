using Blazored.LocalStorage;
using PocMaui.Core.Contract.App.InternalService;
using PocMaui.Core.Contract.Infra.Platforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocMaui.App.InternalService
{
    public class SetupService : ISetupService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IFolderPicker _folderPicker;
        public SetupService(
            IFolderPicker folderPicker,
            ILocalStorageService localStorage
        ) {
            _folderPicker = folderPicker;
            _localStorage = localStorage;
        }
        public async Task CriarBancoDados()
        {
            var pickedFolder = await _folderPicker.PickFolder();

            //Console.WriteLine(pickedFolder);
            await _localStorage.SetItemAsync("SESSAO_ATUAL", "John Smith");
        }

        public async Task FecharBancoDados()
        {
            await _localStorage.RemoveItemAsync("SESSAO_ATUAL");
        }

        public async Task<string> ObterSessao()
        {
            await Task.CompletedTask;
            var sessao = await _localStorage.GetItemAsync<string>("SESSAO_ATUAL");
            return sessao;
        }
    }
}
