using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Security;
using Oqtane.Shared;
using GIBS.Module.ReportViewer.Repository;

namespace GIBS.Module.ReportViewer.Services
{
    public class ServerReportViewerService : IReportViewerService
    {
        private readonly IReportViewerRepository _ReportViewerRepository;
        private readonly IUserPermissions _userPermissions;
        private readonly ILogManager _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly Alias _alias;

        public ServerReportViewerService(IReportViewerRepository ReportViewerRepository, IUserPermissions userPermissions, ITenantManager tenantManager, ILogManager logger, IHttpContextAccessor accessor)
        {
            _ReportViewerRepository = ReportViewerRepository;
            _userPermissions = userPermissions;
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
    }
}
