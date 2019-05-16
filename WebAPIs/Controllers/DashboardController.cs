using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.CommonData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;
using ViewModel.Model;

namespace WebAPIs.Controllers
{
    /// <summary>
    /// Dashboard controller.
    /// </summary>
    [Route("api")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }


        /// <summary>
        /// Gets count for categories and products.
        /// </summary>
        /// <returns>
        /// Returns count for categories and products and their updated values.
        /// </returns>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(StatisticsModel), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public IResult GetStatistics()
        {
            return _dashboardService.GetDashboard();
        }

    }
}
 