﻿using AppClientes.DAL;
using AppClientes.Infra.Services;
using AppClientes.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace AppClientes.ViewModels
{
	public class ProcurarViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly ClienteService _clienteService;

        public ProcurarViewModel(IPageDialogService pageDialog, ClienteService clienteService)
        {
            Title = "Procurar Clientes";
            TitleButton = "Pesquisar";
            _pageDialog = pageDialog;
            Procurar = new DelegateCommand(ProcurarBD);
            Elementos = _Elementos;
            ListaClientes = ListaItens;
            ListaSelect = new DelegateCommand(ListaClientes_ItemSelectedAsync);
            _clienteService = clienteService;
        }

        public string Title { get; set; }
        public string TitleTipo { get; set; }
        public string TitleButton { get; set; }        
        public int ItemEscolha { get; set; }
        public string ItemProcura { get; set; }
        public Cliente ListaSelected { get; set; }
        IPageDialogService _pageDialog;
        public DelegateCommand Procurar { get; set; }
        public DelegateCommand ListaSelect { get; set; }

        public List<string> _Elementos = new List<string> { "Selecione o tipo de Busca", "Por ID", "Por Nome" };
        public List<string> Elementos
        {
            get { return _Elementos; }
            set
            {
                if (Equals(value, _Elementos)) return;
                _Elementos = value;
            }
        }


        public int ElementosSelectedIndex
        {
            get
            {
                return ItemEscolha;
            }
            set
            {
                if (ItemEscolha != value)
                {
                    ItemEscolha = value;
                }
            }
        }


        private async void ProcurarBD()
        {
            
            if (ItemProcura != null && ItemEscolha != 0)
            {
                if (ItemEscolha.Equals(1))
                {
                    string verifica = "^[0-9]";
                    if (!Regex.IsMatch(ItemProcura, verifica))
                    {
                        await _pageDialog.DisplayAlertAsync("Campo digitado inválido", "Você digitou um campo inválido, digite apenas números !", "OK");
                    }
                    else
                    {
                        ProcuraPorIDAsync();

                    }
                }
                else
                {

                    if (!Regex.IsMatch(ItemProcura, @"^[a-zA-Z]+$"))
                    {
                        await _pageDialog.DisplayAlertAsync("Campo digitado inválido", "Você digitou um campo inválido, digite apenas caracter ! ", "OK");
                    }
                    else
                    {
                        ProcuraPorNomeAsync();
                    }

                }
            }
            else
            {
                await _pageDialog.DisplayAlertAsync("Atenção", "Você esqueceu de preencher o campo de busca ou o tipo de busca", "OK");
            }
        }

        DatabaseContext contexto = new DatabaseContext();

        private async void ProcuraPorIDAsync()
        {
            var busca = _clienteService.ProcuraPorID(Convert.ToInt32(ItemProcura));

            if (busca.Count > 0)
            {
                ListaClientes = busca;
            }
            else
            {
                await _pageDialog.DisplayAlertAsync("Atenção", "Não foi encontrado nada com esse resultado", "OK");
                ListaClientes = busca;
            }

        }

        private async void ProcuraPorNomeAsync()
        {
            var busca = _clienteService.ProcuraPorNome(ItemProcura.ToLower());

            if (busca.Count > 0)
            {
                ListaClientes = busca;                
            }
            else
            {
                await _pageDialog.DisplayAlertAsync("Atenção", "Não foi encontrado nada com esse resultado", "OK");
                ListaClientes = busca;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private List<Cliente> ListaItens;

        public List<Cliente> ListaClientes
        {
            get { return ListaItens; }
            set
            {
                ListaItens = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListaClientes)));
            }
        }

        private async void ListaClientes_ItemSelectedAsync()
        {

            await _pageDialog.DisplayAlertAsync("Detalhes do Cliente", "ID:" + ListaSelected.ClienteID + "\nNome: " + ListaSelected.Nome + "\nIdade: " + ListaSelected.Idade + "\nTelefone: " + ListaSelected.Telefone, "OK");
            ListaSelected = null;            
        }






    }
}







