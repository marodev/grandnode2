﻿using Grand.Business.Common.Interfaces.Logging;
using Grand.Business.Common.Interfaces.Security;
using Grand.Business.Common.Services.Security;
using Grand.Business.System.Commands.Models.Security;
using Grand.Business.System.Interfaces.Installation;
using Grand.Domain.Data;
using Grand.Infrastructure.Caching;
using Grand.Infrastructure.Data;
using Grand.Infrastructure.Plugins;
using Grand.SharedKernel.Extensions;
using Grand.Web.Models.Install;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grand.Web.Controllers
{
    public partial class InstallController : Controller
    {
        #region Fields

        private readonly ICacheBase _cacheBase;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IMediator _mediator;

        #endregion

        #region Ctor

        public InstallController(
            ICacheBase cacheBase,
            IHostApplicationLifetime applicationLifetime,
            IServiceProvider serviceProvider,
            IMediator mediator)
        {
            _cacheBase = cacheBase;
            _applicationLifetime = applicationLifetime;
            _serviceProvider = serviceProvider;
            _mediator = mediator;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> Index()
        {
            if (DataSettingsHelper.DatabaseIsInstalled())
                return RedirectToRoute("HomePage");

            var locService = _serviceProvider.GetRequiredService<IInstallationLocalizedService>();

            var installed = await _cacheBase.GetAsync("Installed", async () => { return await Task.FromResult(false); });
            if (installed)
                return View(new InstallModel() { Installed = true });

            var model = new InstallModel
            {
                AdminEmail = "admin@yourstore.com",
                InstallSampleData = false,
                DatabaseConnectionString = "",
                DataProvider = "mongodb",
            };
            foreach (var lang in locService.GetAvailableLanguages())
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Value = Url.RouteUrl("InstallChangeLanguage", new { language = lang.Code }),
                    Text = lang.Name,
                    Selected = locService.GetCurrentLanguage().Code == lang.Code,
                });
            }
            //prepare collation list
            foreach (var col in locService.GetAvailableCollations())
            {
                model.AvailableCollation.Add(new SelectListItem
                {
                    Value = col.Value,
                    Text = col.Name,
                    Selected = locService.GetCurrentLanguage().Code == col.Value,
                });
            }
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Index(InstallModel model)
        {
            if (DataSettingsHelper.DatabaseIsInstalled())
                return RedirectToRoute("HomePage");

            var locService = _serviceProvider.GetRequiredService<IInstallationLocalizedService>();

            if (model.DatabaseConnectionString != null)
                model.DatabaseConnectionString = model.DatabaseConnectionString.Trim();

            string connectionString = "";

            if (model.MongoDBConnectionInfo)
            {
                if (String.IsNullOrEmpty(model.DatabaseConnectionString))
                {
                    ModelState.AddModelError("", locService.GetResource("ConnectionStringRequired"));
                }
                else
                {
                    connectionString = model.DatabaseConnectionString;
                }
            }
            else
            {
                if (String.IsNullOrEmpty(model.MongoDBDatabaseName))
                {
                    ModelState.AddModelError("", locService.GetResource("DatabaseNameRequired"));
                }
                if (String.IsNullOrEmpty(model.MongoDBServerName))
                {
                    ModelState.AddModelError("", locService.GetResource("MongoDBServerNameRequired"));
                }
                string userNameandPassword = "";
                if (!(String.IsNullOrEmpty(model.MongoDBUsername)))
                {
                    userNameandPassword = model.MongoDBUsername + ":" + model.MongoDBPassword + "@";
                }

                connectionString = "mongodb://" + userNameandPassword + model.MongoDBServerName + "/" + model.MongoDBDatabaseName;
            }

            if (!string.IsNullOrEmpty(connectionString))
            {
                try
                {
                    var client = new MongoClient(connectionString);
                    var databaseName = new MongoUrl(connectionString).DatabaseName;
                    var database = client.GetDatabase(databaseName);
                    await database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");

                    var filter = new BsonDocument("name", "GrandNodeVersion");
                    var found = database.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter }).Result;

                    if (found.Any())
                        ModelState.AddModelError("", locService.GetResource("AlreadyInstalled"));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                }
            }
            else
                ModelState.AddModelError("", locService.GetResource("ConnectionStringRequired"));

            if (ModelState.IsValid)
            {
                var settingsManager = new DataSettingsManager();
                try
                {
                    //save settings
                    var settings = new DataSettings
                    {
                        DataConnectionString = connectionString
                    };
                    await settingsManager.SaveSettings(settings);

                    var dataProviderInstance = _serviceProvider.GetRequiredService<BaseDataProviderManager>().LoadDataProvider();
                    dataProviderInstance.InitDatabase();

                    var dataSettingsManager = new DataSettingsManager();
                    var dataProviderSettings = dataSettingsManager.LoadSettings(reloadSettings: true);

                    var installationService = _serviceProvider.GetRequiredService<IInstallationService>();
                    await installationService.InstallData(model.AdminEmail, model.AdminPassword, model.Collation, model.InstallSampleData, model.CompanyName, model.CompanyAddress, model.CompanyPhoneNumber, model.CompanyEmail);

                    //reset cache
                    DataSettingsHelper.ResetCache();

                    PluginManager.ClearPlugins();

                    var pluginsInfo = PluginManager.ReferencedPlugins.ToList();

                    foreach (var pluginInfo in pluginsInfo)
                    {
                        try
                        {
                            var plugin = pluginInfo.Instance<IPlugin>(_serviceProvider);
                            await plugin.Install();
                        }
                        catch (Exception ex)
                        {
                            var _logger = _serviceProvider.GetRequiredService<ILogger>();
                            await _logger.InsertLog(Domain.Logging.LogLevel.Error, "Error during installing plugin " + pluginInfo.SystemName,
                                ex.Message + " " + ex.InnerException?.Message);
                        }
                    }

                    //register default permissions
                    var permissionProviders = new List<Type>();
                    permissionProviders.Add(typeof(PermissionProvider));
                    foreach (var providerType in permissionProviders)
                    {
                        var provider = (IPermissionProvider)Activator.CreateInstance(providerType);
                        await _mediator.Send(new InstallPermissionsCommand() { PermissionProvider = provider });
                    }

                    //restart application
                    await _cacheBase.SetAsync("Installed", true, 120);
                    return View(new InstallModel() { Installed = true });
                }
                catch (Exception exception)
                {
                    //reset cache
                    DataSettingsHelper.ResetCache();
                    await _cacheBase.Clear();

                    System.IO.File.Delete(CommonPath.SettingsPath);

                    ModelState.AddModelError("", string.Format(locService.GetResource("SetupFailed"), exception.Message + " " + exception.InnerException?.Message));
                }
            }

            //prepare language list
            foreach (var lang in locService.GetAvailableLanguages())
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Value = Url.RouteUrl("InstallChangeLanguage", new { language = lang.Code }),
                    Text = lang.Name,
                    Selected = locService.GetCurrentLanguage().Code == lang.Code,
                });
            }

            //prepare collation list
            foreach (var col in locService.GetAvailableCollations())
            {
                model.AvailableCollation.Add(new SelectListItem
                {
                    Value = col.Value,
                    Text = col.Name,
                    Selected = locService.GetCurrentLanguage().Code == col.Value,
                });
            }

            return View(model);
        }

        public virtual IActionResult ChangeLanguage(string language)
        {
            if (DataSettingsHelper.DatabaseIsInstalled())
                return RedirectToRoute("HomePage");

            var locService = _serviceProvider.GetRequiredService<IInstallationLocalizedService>();
            locService.SaveCurrentLanguage(language);

            //Reload the page
            return RedirectToAction("Index", "Install");
        }

        public virtual IActionResult RestartInstall()
        {
            if (DataSettingsHelper.DatabaseIsInstalled())
                return RedirectToRoute("HomePage");

            //stop application
            _applicationLifetime.StopApplication();

            //Redirect to home page
            return RedirectToRoute("HomePage");
        }

        #endregion
    }
}
