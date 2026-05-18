using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using GIBS.Module.ReportViewer.Repository;

namespace GIBS.Module.ReportViewer.Services
{
    public class ServerReportViewerService : IReportViewerService
    {
        private readonly IReportViewerRepository _ReportViewerRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IDbContextFactory<ReportViewerContext> _contextFactory;
        private readonly IUserPermissions _userPermissions;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogManager _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly Alias _alias;

        private const string SettingDataSource = "ReportViewer:DataSource";
        private const string SettingConnectionString = "ReportViewer:ConnectionString";
        private const string SettingSqlQuery = "ReportViewer:SqlQuery";
        private const string SettingViewMode = "ReportViewer:ViewMode";
        private const string SettingTemplateHeader = "ReportViewer:TemplateHeader";
        private const string SettingTemplate = "ReportViewer:Template";
        private const string SettingTemplateFooter = "ReportViewer:TemplateFooter";
        private const string SettingCacheMinutes = "ReportViewer:CacheDurationMinutes";

        public ServerReportViewerService(IReportViewerRepository ReportViewerRepository, ISettingRepository settingRepository, IDbContextFactory<ReportViewerContext> contextFactory, IUserPermissions userPermissions, ITenantManager tenantManager, IMemoryCache memoryCache, ILogManager logger, IHttpContextAccessor accessor)
        {
            _ReportViewerRepository = ReportViewerRepository;
            _settingRepository = settingRepository;
            _contextFactory = contextFactory;
            _userPermissions = userPermissions;
            _memoryCache = memoryCache;
            _logger = logger;
            _accessor = accessor;
            _alias = tenantManager.GetAlias();
        }

        public Task<List<Models.ReportViewer>> GetReportViewersAsync(int ModuleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
            {
                return Task.FromResult(_ReportViewerRepository.GetReportViewers(ModuleId).ToList());
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ReportViewer Get Attempt {ModuleId}", ModuleId);
                return null;
            }
        }

        public Task<Models.ReportViewer> GetReportViewerAsync(int ReportViewerId, int ModuleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
            {
                return Task.FromResult(_ReportViewerRepository.GetReportViewer(ReportViewerId));
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ReportViewer Get Attempt {ReportViewerId} {ModuleId}", ReportViewerId, ModuleId);
                return null;
            }
        }

        public Task<Models.ReportViewer> AddReportViewerAsync(Models.ReportViewer ReportViewer)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ReportViewer.ModuleId, PermissionNames.Edit))
            {
                ReportViewer = _ReportViewerRepository.AddReportViewer(ReportViewer);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "ReportViewer Added {ReportViewer}", ReportViewer);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ReportViewer Add Attempt {ReportViewer}", ReportViewer);
                ReportViewer = null;
            }
            return Task.FromResult(ReportViewer);
        }

        public Task<Models.ReportViewer> UpdateReportViewerAsync(Models.ReportViewer ReportViewer)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ReportViewer.ModuleId, PermissionNames.Edit))
            {
                ReportViewer = _ReportViewerRepository.UpdateReportViewer(ReportViewer);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "ReportViewer Updated {ReportViewer}", ReportViewer);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ReportViewer Update Attempt {ReportViewer}", ReportViewer);
                ReportViewer = null;
            }
            return Task.FromResult(ReportViewer);
        }

        public Task DeleteReportViewerAsync(int ReportViewerId, int ModuleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.Edit))
            {
                _ReportViewerRepository.DeleteReportViewer(ReportViewerId);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "ReportViewer Deleted {ReportViewerId}", ReportViewerId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ReportViewer Delete Attempt {ReportViewerId} {ModuleId}", ReportViewerId, ModuleId);
            }
            return Task.CompletedTask;
        }

        public async Task<Models.ReportExecutionResult> ExecuteReportAsync(int moduleId, bool bypassCache)
        {
            if (!_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.View))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Report Execute Attempt {ModuleId}", moduleId);
                return new Models.ReportExecutionResult { Success = false, ErrorMessage = "Unauthorized" };
            }

            var settings = LoadSettings(moduleId);
            if (string.IsNullOrWhiteSpace(settings.SqlQuery))
            {
                return new Models.ReportExecutionResult { Success = false, ErrorMessage = "SQL query setting is required." };
            }

            if (settings.DataSource == Models.ReportViewerSettings.DataSourceRemote && string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                return new Models.ReportExecutionResult { Success = false, ErrorMessage = "Connection string is required for remote data source." };
            }

            settings.CacheDurationMinutes = Math.Clamp(settings.CacheDurationMinutes, 0, 1440);
            var cacheKey = BuildCacheKey(moduleId, settings);
            if (!bypassCache && settings.CacheDurationMinutes > 0 && _memoryCache.TryGetValue(cacheKey, out Models.ReportExecutionResult cachedResult))
            {
                return cachedResult;
            }

            try
            {
                var (columns, rows) = await ExecuteQueryAsync(settings);
                var result = new Models.ReportExecutionResult
                {
                    Success = true,
                    Columns = columns,
                    Rows = rows,
                    ViewMode = settings.ViewMode,
                    RenderedTemplate = settings.ViewMode == Models.ReportViewerSettings.ViewModeTemplate ? RenderTemplate(settings.TemplateHeader, settings.Template, settings.TemplateFooter, rows) : string.Empty,
                    GeneratedOnUtc = DateTime.UtcNow
                };

                if (settings.CacheDurationMinutes > 0)
                {
                    _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(settings.CacheDurationMinutes));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Read, ex, "Error executing report {ModuleId}", moduleId);
                return new Models.ReportExecutionResult
                {
                    Success = false,
                    ErrorMessage = "Report execution failed. Verify query and data source settings.",
                    ViewMode = settings.ViewMode
                };
            }
        }

        private Models.ReportViewerSettings LoadSettings(int moduleId)
        {
            var moduleSettings = _settingRepository.GetSettings(EntityNames.Module, moduleId).ToList();
            int.TryParse(_settingRepository.GetSettingValue(moduleSettings, SettingCacheMinutes, "5"), out var cacheMinutes);

            return new Models.ReportViewerSettings
            {
                DataSource = NormalizeValue(_settingRepository.GetSettingValue(moduleSettings, SettingDataSource, Models.ReportViewerSettings.DataSourceLocal)),
                ConnectionString = NormalizeValue(_settingRepository.GetSettingValue(moduleSettings, SettingConnectionString, string.Empty)),
                SqlQuery = NormalizeValue(_settingRepository.GetSettingValue(moduleSettings, SettingSqlQuery, string.Empty)),
                ViewMode = NormalizeValue(_settingRepository.GetSettingValue(moduleSettings, SettingViewMode, Models.ReportViewerSettings.ViewModeTable)),
                TemplateHeader = NormalizeValue(_settingRepository.GetSettingValue(moduleSettings, SettingTemplateHeader, string.Empty)),
                Template = NormalizeValue(_settingRepository.GetSettingValue(moduleSettings, SettingTemplate, string.Empty)),
                TemplateFooter = NormalizeValue(_settingRepository.GetSettingValue(moduleSettings, SettingTemplateFooter, string.Empty)),
                CacheDurationMinutes = cacheMinutes
            };
        }

        private async Task<(List<string> columns, List<Dictionary<string, string>> rows)> ExecuteQueryAsync(Models.ReportViewerSettings settings)
        {
            if (settings.DataSource == Models.ReportViewerSettings.DataSourceRemote)
            {
                await using var remoteConnection = new SqlConnection(settings.ConnectionString);
                return await ExecuteQueryWithConnectionAsync(remoteConnection, settings.SqlQuery);
            }

            await using var db = _contextFactory.CreateDbContext();
            var localConnection = db.Database.GetDbConnection();
            return await ExecuteQueryWithConnectionAsync(localConnection, settings.SqlQuery);
        }

        private static async Task<(List<string> columns, List<Dictionary<string, string>> rows)> ExecuteQueryWithConnectionAsync(IDbConnection connection, string sql)
        {
            if (connection.State != ConnectionState.Open)
            {
                await ((System.Data.Common.DbConnection)connection).OpenAsync();
            }

            using var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            using var reader = await ((System.Data.Common.DbCommand)command).ExecuteReaderAsync();

            var columns = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i));
            }

            var rows = new List<Dictionary<string, string>>();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var column in columns)
                {
                    row[column] = reader[column] == DBNull.Value ? string.Empty : Convert.ToString(reader[column]);
                }
                rows.Add(row);
            }

            return (columns, rows);
        }

        private static string RenderTemplate(string templateHeader, string template, string templateFooter, List<Dictionary<string, string>> rows)
        {
            if (rows.Count == 0)
            {
                return string.Empty;
            }

            var output = new StringBuilder();
            output.Append(templateHeader ?? string.Empty);

            if (!string.IsNullOrWhiteSpace(template))
            {
                foreach (var row in rows)
                {
                    var rendered = template;
                    foreach (var column in row)
                    {
                        rendered = rendered.Replace($"[{column.Key}]", column.Value ?? string.Empty, StringComparison.OrdinalIgnoreCase);
                        rendered = rendered.Replace($"{{{{{column.Key}}}}}", column.Value ?? string.Empty, StringComparison.OrdinalIgnoreCase);
                    }
                    output.Append(rendered);
                }
            }

            output.Append(templateFooter ?? string.Empty);
            return output.ToString();
        }

        private string BuildCacheKey(int moduleId, Models.ReportViewerSettings settings)
        {
            var hashInput = $"{settings.DataSource}|{settings.ConnectionString}|{settings.SqlQuery}|{settings.ViewMode}|{settings.TemplateHeader}|{settings.Template}|{settings.TemplateFooter}";
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(hashInput));
            var hash = Convert.ToHexString(hashBytes);
            return $"ReportViewer:{_alias.SiteId}:{moduleId}:{hash}";
        }

        private static string NormalizeValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value.Replace("[Public]", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("[Private]", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Trim();
        }
    }
}
