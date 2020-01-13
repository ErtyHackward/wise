using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace WiseBlazor.Components
{
    public class RedirectToLogin : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        public static string RedirectUri { get; set; }

        protected override void OnInitialized()
        {
            RedirectUri = NavigationManager.Uri;
            NavigationManager.NavigateTo("login");
        }
    }
}
