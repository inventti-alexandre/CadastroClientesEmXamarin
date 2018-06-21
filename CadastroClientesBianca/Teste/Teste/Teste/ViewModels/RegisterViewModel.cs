﻿using AppClientes.Infra.Services;
using AppClientes.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AppClientes.ViewModels
{
	public class RegisterViewModel : BindableBase
	{
        private readonly IService _clienteService;
        public RegisterViewModel(IPageDialogService pageDialog, IService clienteService)
        {
            Title = "Cadastro Clientes";
            TitleName = "Nome";
            TitleAge = "Idade";
            TitlePhone = "Telefone";
            Register = new DelegateCommand<object>(SavingClient);
            _pageDialog = pageDialog;
            _clienteService = clienteService;
        }

        public string Title { get; set; }
        public string TitleName { get; set; }
        public string TitleAge { get; set; }
        public string TitlePhone { get; set; }
        public string NameCli { get; set; }
        public string AgeCli { get; set; }
        public string PhoneCli { get; set; }


        public DelegateCommand<object> Register { get; set; }
        IPageDialogService _pageDialog;

        bool x;

        private void SavingClient(object sender)
        {
            Validate();

            if (x == true)
            {
                Client c = new Client();
                c.Name = NameCli;
                c.Age = Convert.ToInt32(AgeCli);
                c.Phone = PhoneCli;
                SavingDB(c);
            }

        }

        private void SavingDB(Client c)
        {
            try
            {
                _clienteService.SaveClient(c);
                _pageDialog.DisplayAlertAsync("Salvo", "Cliente salvo com sucesso", "OK");
            }
            catch (Exception e)
            {
                _pageDialog.DisplayAlertAsync("Erro", "Ocorreu um erro para salvar: " + e, "OK");
            }
        }

        private async void Validate()
        {

            string tel = "^(?:(?([0-9]{2}))?[-. ]?)?([0-9]{4})[-. ]?([0-9]{4})$";

            if ((NameCli != null) && (AgeCli != null) && (PhoneCli != null))
            {
                if (!Regex.IsMatch(NameCli, @"^[a-zA-Z]"))
                {
                    await _pageDialog.DisplayAlertAsync("ATENÇÃO", "Campo nome inválido,digite apenas caracteres !", "OK");
                }
                else if (Convert.ToInt32(AgeCli) < 0)
                {
                    await _pageDialog.DisplayAlertAsync("ATENÇÃO", "Campo idade inválido, digite valores positivos !", "OK");
                }
                else if (Regex.IsMatch(PhoneCli, tel) == false)
                {
                    await _pageDialog.DisplayAlertAsync("ATENÇÃO", "Campo telefone inválido ! Digite como o exemplo: 3333-3333 ou 33333333", "OK");
                }
                else
                {
                    x = true;
                }
            }
            else
            {
                await _pageDialog.DisplayAlertAsync("Campo vazio", "Verifique se foram preenchidos todos os campos", "OK");
            }

        }
    }
}