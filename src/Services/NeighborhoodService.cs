﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using LetsGoSEA.WebSite.Models;
using Microsoft.AspNetCore.Hosting;

namespace LetsGoSEA.WebSite.Services
{
    /// <summary>
    /// Mediates communication between a NeighborhoodsController and Neighborhoods Data  
    /// </summary>
    public class NeighborhoodService
    {
        // Constructor
        public NeighborhoodService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        // Getter: Get JSON file from wwwroot
        private IWebHostEnvironment WebHostEnvironment { get; }

        // Store the path of Neighborhoods JSON file (combine the root path, folder name, and file name)
        private string NeighborhoodFileName => Path.Combine(WebHostEnvironment.WebRootPath, "data", "neighborhoods.json");

        // Generate/retrieve a list of NeighborhoodModel objects from JSON file
        public IEnumerable<NeighborhoodModel> GetNeighborhoods()
        {
            // Open Neighborhoods JSON file
            using var jsonFileReader = File.OpenText(NeighborhoodFileName);

            // Read and Deserialize JSON file into an array of NeighborhoodModel objects
            return JsonSerializer.Deserialize<NeighborhoodModel[]>(jsonFileReader.ReadToEnd(),
                // Make case insensitive
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
        }

        /// <summary>
        /// Returns null if passed invalid id.
        /// Returns a single neighborhood corresponding to the id
        /// </summary>
        /// <param name="id">id of the requested neighborhood</param>
        /// <returns>NeighborhoodModel of the requested neighborhood</returns>
        public NeighborhoodModel GetNeighborhoodById(int? id)
        {
            try
            {
                var data = GetNeighborhoods().Where(x => x.Id == id);
                NeighborhoodModel singleNeighborhood = data.ElementAt(0);
                return singleNeighborhood;
            }
            catch (ArgumentOutOfRangeException)
            {
                // If the id passed is invalid, we return null
                return null;
            }
            
        }

        /// <summary>
        /// Save All neighborhood data to storage
        /// </summary>
        /// <param name="neighborhoods">all the neighborhood objects to be saved</param>
        private void SaveData(IEnumerable<NeighborhoodModel> neighborhoods)
        {
            // Re-write all the neighborhood data to JSON file
            using (var outputStream = File.Create(NeighborhoodFileName))
            {
                JsonSerializer.Serialize<IEnumerable<NeighborhoodModel>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    neighborhoods
                );
            }
        }

        /// <summary>
        /// Create a new neighborhood using default values
        /// After create the user can update to set values
        /// </summary>
        /// <returns>"NeighborhoodModel"</returns>
        public NeighborhoodModel CreateData()
        {
            // New Neighborhood to be added
            var data = new NeighborhoodModel()
            {
                // Generate the next valid Id number
                Id = GetNeighborhoods().Count() + 1,

                Name = "",
                Image = "",
                City = "",
                State = "",
                ShortDesc = ""
            };

            // Get the current set, and append the new record to it becuase IEnumerable does not have Add
            var dataSet = GetNeighborhoods();
            dataSet = dataSet.Append(data);

            SaveData(dataSet);

            return data;
        }

        // <summary>
        // Finds record
        // Updates the neighborhood
        // saves record
        // </summary>
        // <param name="data">neighborhood data to be saved</param>
        public NeighborhoodModel UpdateData(NeighborhoodModel data) 
        {
            var neighborhood = GetNeighborhoods();
            var neighborhoodData = neighborhood.FirstOrDefault(x => x.Id.Equals(data.Id));
            if (neighborhoodData == null)
            {
                return null;
            }

            neighborhoodData.Name = data.Name;
            neighborhoodData.Image = data.Image;
            neighborhoodData.City = data.City;
            neighborhoodData.State = data.State;
            neighborhoodData.ShortDesc = data.ShortDesc;

            SaveData(neighborhood);

            return neighborhoodData;
        }

        /// <summary>
        /// Remove the neighborhood record from the system
        /// </summary>
        /// <param name="id">id of the neighborhood to NOT be saved</param>
        /// <returns>the neighborhood object to be deleted</returns>
        public NeighborhoodModel DeleteData(int id)
        {
            // Get the current set
            var dataSet = GetNeighborhoods();

            // Get the record to be deleted
            var data = dataSet.FirstOrDefault(m => m.Id == id);

            // Only save the remaining records in the system
            var newDataSet = GetNeighborhoods().Where(m => m.Id != id);
            SaveData(newDataSet);

            // Return the record to be deleted
            return data;
        }
    }
}