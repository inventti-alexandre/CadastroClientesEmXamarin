﻿using AppClientes.Infra;
using AppClientes.Infra.Services;
using AppClientes.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Xamarin.Forms;

namespace AppClientes.ViewModels
{
    public class LocalFileListViewModel : BindableBase
	{
        private readonly IFileSystem _fileSystem;
        private readonly IService _service;

        public LocalFileListViewModel(IFileSystem fileSystem, IService service, IPageDialogService pageDialog)
        {
            Title = "Importar e Exportar Em JSON";
            ImportTitle = "Importar";
            ExportTitle = "Exportar";
            Import = new DelegateCommand(ImportListAsync);
            Export = new DelegateCommand(ExportList);
            Image= "lista.png";
            _fileSystem = fileSystem;
            _service = service;
            _pageDialog = pageDialog;
        }

        public string ImportTitle { get; set; }
        public string ExportTitle { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        IPageDialogService _pageDialog;
        public DelegateCommand Import { get; set; }
        public DelegateCommand Export { get; set; }
        public int CountClients = 0;

        private String CreateDirectory()
        {
            var documents = _fileSystem.GetStoragePath();
            var directoryname = Path.Combine(documents, "List JSON");
            Directory.CreateDirectory(directoryname);
            return directoryname;
        }

        private void ImportListAsync()
        {
            try
            {
                var documents = CreateDirectory();
                var filename = Path.Combine(documents, "clients.json");
                var dataJson = File.ReadAllText(filename);
                IEnumerable<Client> result = JsonConvert.DeserializeObject<IEnumerable<Client>>(dataJson);
                ImportNotificationAsync(result);
            }
            catch
            {
                throw;
            }   
        }

        private void ExportList()
        {
            try
            {
                string json = JsonConvert.SerializeObject(ListingDB());
                CountClients = ListingDB().Count;
                var documents = CreateDirectory();
                var filename = Path.Combine(documents, "clients.json");
                File.WriteAllText(filename, json);
                ExportNotificationAsync(true);
            }
            catch
            {
                ExportNotificationAsync(false);                
            }                    
        }

        private List<Client> ListingDB()
        {
            return _service.AllClient();
        }

        private bool UpdateDB(IEnumerable<Client> listJSON)
        {
            try
            {
                foreach (var i in listJSON)
                {
                    _service.SaveClient(i);
                    CountClients++;
                }

                return true;
            }
            catch
            {
                return false;
            }   
                       
        }

        private async void ImportNotificationAsync(IEnumerable<Client> listClients)
        {
            if (UpdateDB(listClients))
            {
                await _pageDialog.DisplayAlertAsync("Importação", "Importação realizada com sucesso ! Importado: "+ CountClients+ " clientes", "OK");
            }
            else
            {
                await _pageDialog.DisplayAlertAsync("Erro", "Erro ao salvar no banco, tente novamente !", "OK");
            }
        }


        private async void ExportNotificationAsync(bool response)
        {
            if (response)
            {
                await _pageDialog.DisplayAlertAsync("Exportação", "Exportação realizada com sucesso ! Exportado: " +CountClients+ " clientes", "OK");
            }
            else
            {
                await _pageDialog.DisplayAlertAsync("Erro", "Tente novamente a exportação !", "OK" );
            }
        }

    }
}