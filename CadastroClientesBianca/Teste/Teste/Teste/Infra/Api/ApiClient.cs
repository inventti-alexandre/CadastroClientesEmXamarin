﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AppClientes.Infra.Services;
using AppClientes.Models;
using Newtonsoft.Json;
using Prism.Services;

namespace AppClientes.Infra.Api
{
    public class APIClient : IApiClient
    {

        public APIClient(IPageDialogService pageDialog)
        {
            _pageDialog = pageDialog;
        }

        IPageDialogService _pageDialog;
        public string content { get; set; }

        public async Task<IEnumerable<Client>> GetAsync(string apiRoute)
        {
            try
            {
                var uri = new Uri(string.Format(apiRoute, string.Empty));

                var response = await API_Singleton.Instance.GetAsync(uri);
                
                if (response.IsSuccessStatusCode)
                {
                    content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<Client>>(content);
                }
                else
                {
                    await _pageDialog.DisplayAlertAsync("Ocorreu um erro", "Erro ao processar a URL.", "OK");
                    return null;
                }
            }
            catch (Exception)
            {
                await _pageDialog.DisplayAlertAsync("Ocorreu um erro", "Tente novamente !", "OK");
                return null;
            }
        }

        public string Read_JSON()
        {
            return content;
        }
    }
}