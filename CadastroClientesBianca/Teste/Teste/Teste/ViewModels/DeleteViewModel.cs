﻿using AppClientes.Infra;
using AppClientes.Infra.Services;
using AppClientes.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AppClientes.ViewModels
{
	public class DeleteViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IService _clienteService;
        private readonly IFileSystem _fileSystem;
        public DeleteViewModel(IPageDialogService pageDialog, IService clienteService, IFileSystem fileSystem)
        {
            Title = "Excluir Clientes";
            TitleButton = "Pesquisar";
            _pageDialog = pageDialog;
            Search = new DelegateCommand(SearchDB);           
            ListClients = ListItems;
            ListSelect = new DelegateCommand(ListClients_ItemSelectedAsync);
            _fileSystem = fileSystem;
            _clienteService = clienteService;
        }

        private int reference=0;
        public string Title { get; set; }
        public string TitleTipo { get; set; }
        public string TitleButton { get; set; }
        public int Selected { get; set; }
        public string ItemSearch { get; set; }
        IPageDialogService _pageDialog;
        public DelegateCommand Search { get; set; }
        public DelegateCommand ListSelect { get; set; }
        public Client ListSelected { get; set; }


        private List<string> _Elements = new List<string> { "Selecione o tipo de Busca", "Por ID", "Por Nome" };
        public List<string> Elements
        {
            get { return _Elements; }            
        }


        public int ElementsSelectedIndex
        {
            get
            {
                return Selected;
            }
            set
            {
                if (Selected != value)
                {
                    Selected = value;
                }
            }
        }


        private async void SearchDB()
        {

            if (ItemSearch != null && Selected != 0)
            {
                if (Selected.Equals(1))
                {
                    string verifica = "^[0-9]";
                    if (!Regex.IsMatch(ItemSearch, verifica))
                    {
                        await _pageDialog.DisplayAlertAsync("Campo digitado inválido", "Você digitou um campo inválido, digite apenas números !", "OK");
                    }
                    else
                    {
                        SearchID();

                    }
                }
                else
                {

                    if (!Regex.IsMatch(ItemSearch, @"^[a-zA-Z]+$"))
                    {
                        await _pageDialog.DisplayAlertAsync("Campo digitado inválido", "Você digitou um campo inválido, digite apenas caracter ! ", "OK");
                    }
                    else
                    {
                        SearchName();
                    }

                }
            }
            else
            {
                await _pageDialog.DisplayAlertAsync("Atenção", "Você esqueceu de preencher o campo de busca ou o tipo de busca", "OK");
            }
        }


        private async void SearchID()
        {
            var search = _clienteService.SearchID(Convert.ToInt32(ItemSearch));

            if (search.Count > 0)
            {
                ListClients = search;
            }
            else
            {
                if (reference == 0)
                {
                    await _pageDialog.DisplayAlertAsync("Atenção", "Não foi encontrado nada com esse resultado", "OK");
                    ListClients = search;
                }
                else
                {
                    ListClients = search;
                    reference = 0;
                }

            }
        }

        private async void SearchName()
        {
            var search = _clienteService.SearchName(ItemSearch.ToLower());

            if (search.Count > 0)
            {
                ListClients = search;
            }
            else
            {
                if (reference == 0)
                {
                    await _pageDialog.DisplayAlertAsync("Atenção", "Não foi encontrado nada com esse resultado", "OK");
                    ListClients = search;
                }
                else
                {
                    ListClients = search;
                    reference = 0;
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private List<Client> ListItems;

        public List<Client> ListClients
        {
            get { return ListItems; }
            set
            {
                ListItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListClients)));
            }
        }

        private async void ListClients_ItemSelectedAsync()
        {
            if (ListSelected != null)
            {
                var result = await _pageDialog.DisplayAlertAsync("Deseja excluir o cliente abaixo ?", "ID:" + ListSelected.ClientID + "\nNome: " + ListSelected.Name + "\nIdade: " + ListSelected.Age + "\nTelefone: " + ListSelected.Phone, "SIM", "NÃO");

                if (result)
                {
                    DeleteDB(ListSelected.ClientID);                    
                    reference = 1;
                    if (Selected.Equals(1))
                    {
                        SearchID();
                    }
                    else if (Selected.Equals(2))
                    {
                        SearchName();
                    }
                }                
            }
        }

        private async void DeleteDB(int idClient)
        {
            try
            {
                Client c = _clienteService.SearchClient(idClient);
                DeletePhoto(c.PathPhoto);
                if(_clienteService.DeleteClient(c) == true)
                {                    
                    await _pageDialog.DisplayAlertAsync("Exclusão concluída", "Cliente excluído com sucesso ", "OK");
                }                                       
            }
            catch (Exception)
            {
                await _pageDialog.DisplayAlertAsync("Erro", "Cliente já excluído", "OK");
            }
        }

        private void DeletePhoto(string path)
        {
            string destiny = Path.Combine(_fileSystem.GetStoragePath(), "Photos");
            string imageLocal = Path.Combine(destiny, Path.GetFileName(path));
            System.IO.File.Delete(imageLocal);
        }
    }
}