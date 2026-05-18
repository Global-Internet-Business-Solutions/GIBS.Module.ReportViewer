using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using GIBS.Module.ReportViewer.Services;
using Oqtane.Controllers;
using System.Net;
using System.Threading.Tasks;

namespace GIBS.Module.ReportViewer.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class ReportViewerController : ModuleControllerBase
    {
        private readonly IReportViewerService _ReportViewerService;

        public ReportViewerController(IReportViewerService ReportViewerService, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _ReportViewerService = ReportViewerService;
        }

        [HttpGet("execute/{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<Models.ReportExecutionResult> Execute(int moduleid, bool bypassCache = false)
        {
            if (IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                return await _ReportViewerService.ExecuteReportAsync(moduleid, bypassCache);
            }

            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ReportViewer Execute Attempt {ModuleId}", moduleid);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return new Models.ReportExecutionResult { Success = false, ErrorMessage = "Unauthorized" };
        }

        // GET: api/<controller>?moduleid=x
        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<IEnumerable<Models.ReportViewer>> Get(string moduleid)
        {
            int ModuleId;
            if (int.TryParse(moduleid, out ModuleId) && IsAuthorizedEntityId(EntityNames.Module, ModuleId))
            {
                return await _ReportViewerService.GetReportViewersAsync(ModuleId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ReportViewer Get Attempt {ModuleId}", moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }
        }

        // GET api/<controller>/5
        [HttpGet("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<Models.ReportViewer> Get(int id, int moduleid)
        {
            Models.ReportViewer ReportViewer = await _ReportViewerService.GetReportViewerAsync(id, moduleid);
            if (ReportViewer != null && IsAuthorizedEntityId(EntityNames.Module, ReportViewer.ModuleId))
            {
                return ReportViewer;
            }
            else
            { 
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ReportViewer Get Attempt {ReportViewerId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<Models.ReportViewer> Post([FromBody] Models.ReportViewer ReportViewer)
        {
            if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, ReportViewer.ModuleId))
            {
                ReportViewer = await _ReportViewerService.AddReportViewerAsync(ReportViewer);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ReportViewer Post Attempt {ReportViewer}", ReportViewer);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                ReportViewer = null;
            }
            return ReportViewer;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<Models.ReportViewer> Put(int id, [FromBody] Models.ReportViewer ReportViewer)
        {
            if (ModelState.IsValid && ReportViewer.ReportViewerId == id && IsAuthorizedEntityId(EntityNames.Module, ReportViewer.ModuleId))
            {
                ReportViewer = await _ReportViewerService.UpdateReportViewerAsync(ReportViewer);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ReportViewer Put Attempt {ReportViewer}", ReportViewer);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                ReportViewer = null;
            }
            return ReportViewer;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task Delete(int id, int moduleid)
        {
            Models.ReportViewer ReportViewer = await _ReportViewerService.GetReportViewerAsync(id, moduleid);
            if (ReportViewer != null && IsAuthorizedEntityId(EntityNames.Module, ReportViewer.ModuleId))
            {
                await _ReportViewerService.DeleteReportViewerAsync(id, ReportViewer.ModuleId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ReportViewer Delete Attempt {ReportViewerId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }
    }
}
