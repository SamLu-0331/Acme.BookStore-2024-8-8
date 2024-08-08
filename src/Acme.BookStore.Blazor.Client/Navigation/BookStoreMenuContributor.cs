using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Acme.BookStore.Localization;
using Acme.BookStore.Permissions;
using Acme.BookStore.MultiTenancy;
using Volo.Abp.Account.Localization;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.Users;
using Volo.Abp.Identity.Pro.Blazor.Navigation;
using Volo.Abp.AuditLogging.Blazor.Menus;
using Volo.Abp.LanguageManagement.Blazor.Menus;
using Volo.Abp.TextTemplateManagement.Blazor.Menus;
using Volo.Abp.OpenIddict.Pro.Blazor.Menus;
using Volo.Saas.Host.Blazor.Navigation;

namespace Acme.BookStore.Blazor.Client.Navigation;

public class BookStoreMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public BookStoreMenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
        else if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }
private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
{
    var l = context.GetLocalizer<BookStoreResource>();

    context.Menu.Items.Insert(
        0,
        new ApplicationMenuItem(
            "BookStore.Home",
            l["Menu:Home"],
            "/",
            icon: "fas fa-home"
        )
    );

    var bookStoreMenu = new ApplicationMenuItem(
        "BooksStore",
        l["Menu:Hospital"],
        icon: "fa fa-book"
    );

    context.Menu.AddItem(bookStoreMenu);

    //CHECK the PERMISSION
    if (await context.IsGrantedAsync(BookStorePermissions.Books.Default))
    {
        bookStoreMenu.AddItem(new ApplicationMenuItem(
            "BooksStore.Books",
            l["Patient"],
            url: "/books"
        ));
    }
    if (await context.IsGrantedAsync(BookStorePermissions.Authors.Default))
    {
        context.Menu.AddItem(new ApplicationMenuItem(
            "BooksStore.Authors",
            l["Doctor"],
            url: "/authors"
        ));
    }

}
    /*private static async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<BookStoreResource>();
        
        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 5;

        context.Menu.AddItem(new ApplicationMenuItem(
            BookStoreMenus.Home,
            l["Menu:Home"],
            "/",
            icon: "fas fa-home",
            order: 1
        ));
        var bookStoreMenu = new ApplicationMenuItem(
            "BooksStore",
            l["Menu:BookStore"],
            icon: "fa fa-book"
        );

        context.Menu.AddItem(bookStoreMenu);

        //CHECK the PERMISSION
        if (await context.IsGrantedAsync(BookStorePermissions.Books.Default))
        {
            bookStoreMenu.AddItem(new ApplicationMenuItem(
                "BooksStore.Books",
                l["Menu:Books"],
                url: "/books"
            ));
        }

        //HostDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                BookStoreMenus.HostDashboard,
                l["Menu:Dashboard"],
                "/HostDashboard",
                icon: "fa fa-chart-line",
                order: 2
            ).RequirePermissions(BookStorePermissions.Dashboard.Host)
        );

        //TenantDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                BookStoreMenus.TenantDashboard,
                l["Menu:Dashboard"],
                "/Dashboard",
                icon: "fa fa-chart-line",
                order: 2
            ).RequirePermissions(BookStorePermissions.Dashboard.Tenant)
        );

        //Saas
        context.Menu.SetSubItemOrder(SaasHostMenus.GroupName, 3);

        //Administration->Identity
        administration.SetSubItemOrder(IdentityProMenus.GroupName, 1);

        //Administration->OpenId
        administration.SetSubItemOrder(OpenIddictProMenus.GroupName, 2);

        //Administration->Language Management
        administration.SetSubItemOrder(LanguageManagementMenus.GroupName, 3);

        //Administration->Text Template Management
        administration.SetSubItemOrder(TextTemplateManagementMenus.GroupName, 4);

        //Administration->Audit Logs
        administration.SetSubItemOrder(AbpAuditLoggingMenus.GroupName, 5);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 6);
    }*/

    private async Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var accountStringLocalizer = context.GetLocalizer<AccountResource>();
        var authServerUrl = _configuration["AuthServer:Authority"] ?? "";

        context.Menu.AddItem(new ApplicationMenuItem(
            "Account.Manage",
            accountStringLocalizer["MyAccount"],
            $"{authServerUrl.EnsureEndsWith('/')}Account/Manage?returnUrl={_configuration["App:SelfUrl"]}",
            icon: "fa fa-cog",
            order: 1000,
            null).RequireAuthenticated());

        context.Menu.AddItem(new ApplicationMenuItem(
            "Account.SecurityLogs",
            accountStringLocalizer["MySecurityLogs"],
            $"{authServerUrl.EnsureEndsWith('/')}Account/SecurityLogs?returnUrl={_configuration["App:SelfUrl"]}",
            icon: "fa fa-user-shield",
            order: 1001,
            null).RequireAuthenticated());

        await Task.CompletedTask;
    }
}
