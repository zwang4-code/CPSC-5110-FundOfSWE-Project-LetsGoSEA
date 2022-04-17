﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using LetsGoSEA.WebSite.Models;
using LetsGoSEA.WebSite.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContosoCrafts.WebSite.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NeighborhoodsController : Controller
    {
        public NeighborhoodsController(NeighborhoodService neighborhoodService)
        {
            this.NeighborhoodService = neighborhoodService;
        }
        
        public NeighborhoodService NeighborhoodService { get; }

        [HttpGet]
        public IEnumerable<Neighborhood> Get()
        {
            return NeighborhoodService.GetNeighborhoods();
        }

        [Route("Name")]
        [HttpGet]
        public Neighborhood Get([FromQuery] string name)
        {
            return NeighborhoodService.GetNeighborhoodByName(name);
        }

    }
}