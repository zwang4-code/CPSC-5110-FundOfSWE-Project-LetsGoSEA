﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LetsGoSEA.WebSite.Models;
using LetsGoSEA.WebSite.Services;
using System;

namespace LetsGoSEA.WebSite.Pages.Neighborhood
{
    /// <summary>
    /// Create Page Model for the Create Razor Page: adds a new Neighborhood to NeighborhoodModel and JSON file
    /// </summary>
    public class CreateModel : PageModel
    {
        // Data middle tier
        public NeighborhoodService NeighborhoodService { get; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="neighborhoodService">An instance of the data service to use</param>
        public CreateModel(NeighborhoodService neighborhoodService)
        {
            NeighborhoodService = neighborhoodService;
        }

        // The data to show
        public NeighborhoodModel Neighborhood;

        public void OnGet()
        {
            // Create a new NeighborhoodModel object
            // The sole purpose is to show the autopopulated ID, Seattle, and WA fields on the browser
            Neighborhood = NeighborhoodService.CreateID();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Request user input from the form
            var ID = Request.Form["Neighborhood.Id"];       // Request.Form returns String[]
            var ID_int = Int32.Parse(ID[0]);                // Parse ID input into integer 
            var name = Request.Form["Neighborhood.Name"];
            var image = Request.Form["Neighborhood.Image"];
            var shortDesc = Request.Form["Neighborhood.ShortDesc"];

            // Create a new Neighborhood Model object WITH user input
            // This Neighborhood object is different from the object created in OnGet()
            // This object will store user input and eventually convert them to JSON
            Neighborhood = NeighborhoodService.AddData(ID_int, name, image, shortDesc);

            // Redirect to Update page with reference to the new neighborhood
            return RedirectToPage("./Index");
        }

    }
}