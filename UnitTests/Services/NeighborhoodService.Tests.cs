﻿using LetsGoSEA.WebSite.Models;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnitTests.Services
{
    /// <summary>
    /// NeighborhoodServiceTests Tests.
    /// </summary>
    public class NeighborhoodServiceTests
    {
        // Global invalid id property for use in tests. 
        private static int InvalidId = -1;

        // Global valid name property for use in tests. 
        private static string Name = "Bogusland";

        // Global valid image property for use in tests. 
        private static string Image = "http://via.placeholder.com/150";

        // Global valid shortDesc property for use in tests.
        private static string ShortDesc = "Test neighborhood description";

        // Global imgFiles property for use in tests. 
        private static IFormFileCollection ImgFilesNull = null;

        // Global valid Rating for use in AddRatings region.
        private static int ValidRating = 5;

        // Global valid comment input for use in Comments region.
        private static string ValidComment = "Bogus";

        /// <summary>
        /// Global mock FormFileCollection generator creates ImagePath neighborhood
        /// property for use in Images region. 
        /// </summary>
        public FormFileCollection GetImagePath()
        {
            // Setup mock file using a memory stream.
            var content = "Random content";
            var imageFileName = "test.jpg";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            // Create FormFile with desired data.
            var imageFiles = new FormFileCollection();
            IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", imageFileName);
            imageFiles.Add(file);

            return imageFiles;
        }

        #region GetNeighborhoodData

        /// <summary>
        /// Tests GetNeighborhoods returns not null when called. 
        [Test]
        public void GetNeighborhoods_Not_Null_Returns_True()
        {
            // Arrange

            // Act
            var result = TestHelper.NeighborhoodServiceObj.GetNeighborhoods();

            //Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Tests GetNeighborhoods returns an IEnumerable. 
        /// </summary>
        [Test]
        public void GetNeighborhoods_Returns_IEnumerable_Returns_True()
        {
            // Arrange

            // Act
            var result = TestHelper.NeighborhoodServiceObj.GetNeighborhoods();

            //Assert
            Assert.IsInstanceOf(typeof(IEnumerable<NeighborhoodModel>), result);
        }

        /// <summary>
        /// Tests GetNeighborhoodById by retrieving the first neighborhood and confirming not null. 
        /// </summary>
        [Test]
        public void GetNeighborhoodById_Valid_Id_Should_Return_True()
        {
            // Arrange

            // Add the test Neighborhood to the database. 
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, ImgFilesNull);

            //Act
            var result = TestHelper.NeighborhoodServiceObj.GetNeighborhoodById(testNeighborhood.id);

            //Assert
            Assert.NotNull(result);

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }

        /// <summary>
        /// Tests GetNeighborhoodById catches out of bounds input and returns null. 
        /// </summary>
        [Test]
        public void GetNeighborhoodById_Invalid_Id_Should_Return_True()
        {
            // Arrange

            //Act
            var invalidResult = TestHelper.NeighborhoodServiceObj.GetNeighborhoodById(InvalidId);

            //Assert
            Assert.Null(invalidResult);
        }

        #endregion GetNeighborhoodData

        #region Ratings
        /// <summary>
        /// Tests AddRating, null neighborhood should return false.
        /// </summary>
        [Test]
        public void AddRating_Null_Neighborhood_Should_Return_False()
        {
            // Arrange

            // Act
            var result = TestHelper.NeighborhoodServiceObj.AddRating(null, ValidRating);

            // Assert
            Assert.AreEqual(false, result);
        }

        /// <summary>
        /// Test AddRating on Neighborhood with no existing ratings returns true. 
        /// </summary>
        [Test]
        public void AddRating_Valid_Neighborhood_No_Ratings_Returns_True()
        {
            // Arrange

            // Add test neighborhood to database.
            TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, ImgFilesNull);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Act
            var result = TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, ValidRating);

            // Assert
            Assert.AreEqual(true, result);

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }

        /// <summary>
        /// Test AddRating on Neighborhood with existing ratings returns true. 
        /// </summary>
        [Test]
        public void AddRating_Valid_Neighborhood_Existing_Ratings_Returns_True()
        {
            // Arrange

            // Add test neighborhood to database.
            TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, ImgFilesNull);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Add rating. 
            TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, ValidRating);

            // Act
            var result = TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, ValidRating);

            // Assert
            Assert.AreEqual(true, result);

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }

        /// <summary>
        /// Test AddRating on Neighborhood with no existing ratings returns a count of the Ratings
        /// property equal to 1. 
        /// </summary>
        [Test]
        public void AddRating_Valid_Neighborhood_No_Ratings_Count_Equals_1_Returns_True()
        {
            // Arrange

            // Initialize valid test NeighborhoodModel object. 
            // Add test neighborhood to database.
            TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, ImgFilesNull);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Act
            TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, ValidRating);
            var result = testNeighborhood.ratings.Count();

            // Assert
            Assert.AreEqual(1, result);

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }

        /// <summary>
        /// Test AddRating on Neighborhood with existing ratings returns a count of the previous Ratings
        /// property + 1.         
        /// /// </summary>
        [Test]
        public void AddRating_Valid_Neighborhood_Existing_Ratings_Count_Returns_True()
        {
            // Arrange
            // Add test neighborhood to database.
            TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, ImgFilesNull);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Add rating and store existing rating count. 
            TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, ValidRating);
            var existingRatingCount = testNeighborhood.ratings.Count();

            // Act
            TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, ValidRating);
            var result = testNeighborhood.ratings.Count();

            // Assert
            Assert.AreEqual(existingRatingCount + 1, result);

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }

        /// <summary>
        /// Test lower and upperbounds of AddRating input. Out of bounds input (low and high)
        /// should return false. 
        /// </summary>
        [Test]
        public void AddRating_Out_of_Bounds_Input_Return_False()
        {
            // Arrange

            // Invalid ratings outside of 0 - 5. 
            int[] outOfBoundsRatings = new int[4] { -2, -1, 6, 7 };

            // Add test neighborhood to database.
            TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, ImgFilesNull);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Act
            var result1 = TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, outOfBoundsRatings[0]);
            var result2 = TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, outOfBoundsRatings[1]);
            var result3 = TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, outOfBoundsRatings[2]);
            var result4 = TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, outOfBoundsRatings[3]);

            // Assert
            Assert.AreEqual(false, result1);
            Assert.AreEqual(false, result2);
            Assert.AreEqual(false, result3);
            Assert.AreEqual(false, result4);

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }

        /// <summary>
        /// Test AddRating input within valid bounds returns true. 
        /// </summary>
        [Test]
        public void AddRating_In_Bounds_Input_Return_True()
        {
            // Arrange

            // Valid ratings from 0-5. 
            int[] validRatings = new int[6] { 0, 1, 2, 3, 4, 5 };

            // Add test neighborhood to database. 
            TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, ImgFilesNull);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Act
            var result1 = TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, validRatings[0]);
            var result2 = TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, validRatings[1]);
            var result3 = TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, validRatings[2]);
            var result4 = TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, validRatings[3]);
            var result5 = TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, validRatings[4]);
            var result6 = TestHelper.NeighborhoodServiceObj.AddRating(testNeighborhood, validRatings[5]);

            // Assert
            Assert.AreEqual(true, result1);
            Assert.AreEqual(true, result2);
            Assert.AreEqual(true, result3);
            Assert.AreEqual(true, result4);
            Assert.AreEqual(true, result5);
            Assert.AreEqual(true, result6);

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }

        #endregion Ratings

        #region Comments

        /// <summary>
        /// Tests AddComment where a valid neighborhood and valid comment return true and update data successfully.
        /// </summary>
        [Test]
        public void AddComment_Valid_Neighborhood_Valid_Comment_Return_True()
        {
            // Arrange

            // Add test neighborhood to database. 
            TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, ImgFilesNull);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Act
            var result = TestHelper.NeighborhoodServiceObj.AddComment(testNeighborhood, ValidComment);

            // Assert
            Assert.AreEqual(true, result);

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }

        /// <summary>
        /// Test AddComment on Neighborhood with no existing ratings comments a count of the comments
        /// property equal to 1. 
        /// </summary>
        [Test]
        public void AddComment_Valid_Neighborhood_No_Comments_Count_Equals_1_Returns_True()
        {
            // Arrange

            // Add test neighborhood to database. 
            TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, ImgFilesNull);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Act
            TestHelper.NeighborhoodServiceObj.AddComment(testNeighborhood, ValidComment);
            var result = testNeighborhood.comments.Count();

            // Assert
            Assert.AreEqual(1, result);

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }


        /// <summary>
        /// Test AddComments on Neighborhood with existing comments returns a count of the previous
        /// comments property + 1.         
        /// /// </summary>
        [Test]
        public void AddComment_Valid_Neighborhood_Existing_Ratings_Count_Returns_True()
        {
            // Arrange
            // Add test neighborhood to database.
            TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, ImgFilesNull);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Add comment and store existing comment count. 
            TestHelper.NeighborhoodServiceObj.AddComment(testNeighborhood, ValidComment);
            var existingCommentCount = testNeighborhood.comments.Count();

            // Act
            TestHelper.NeighborhoodServiceObj.AddComment(testNeighborhood, ValidComment);
            var result = testNeighborhood.comments.Count();

            // Assert
            Assert.AreEqual(existingCommentCount + 1, result);

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }

        /// <summary>
        /// Tests AddComment returns false given a null comment.
        /// </summary>
        [Test]
        public void AddComment_Null_Comment_Return_False()
        {
            // Arrange
            // Add test neighborhood to database.
            TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, ImgFilesNull);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Act
            var result = TestHelper.NeighborhoodServiceObj.AddComment(testNeighborhood, null);

            // Assert
            Assert.AreEqual(false, result);

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }

        /// <summary>
        /// Tests AddComment returns false given an empty "" comment. 
        /// </summary>
        [Test]
        public void AddComment_Empty_Comment_Return_False()
        {
            // Arrange
            // Add test neighborhood to database.
            TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, ImgFilesNull);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Act
            var result = TestHelper.NeighborhoodServiceObj.AddComment(testNeighborhood, "");

            // Assert
            Assert.AreEqual(false, result);

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }

        /// <summary>
        /// Tests AddComment returns false given a null neighborborhood.  
        /// </summary>
        [Test]
        public void AddComment_Null_Neighborhood_Should_Return_False()
        {
            // Arrange

            // Act
            var result = TestHelper.NeighborhoodServiceObj.AddComment(null, ValidComment);

            // Assert
            Assert.AreEqual(false, result);
        }

        /// <summary>
        /// Test DeleteComment returns false given an invalid commentId. 
        /// </summary>
        [Test]
        public void DeleteComment_Null_Neighborhood_Should_Return_False()
        {
            // Arrange

            // Act
            var result = TestHelper.NeighborhoodServiceObj.DeleteComment(null, ValidComment);

            // Assert
            Assert.AreEqual(false, result);
        }

        /// <summary>
        /// Tests DeleteComment returns a true given valid input parameters. 
        /// </summary>
        [Test]
        public void DeleteComment_ValidNeighborhood_ValidCommentId_Should_Return_True()
        {
            // Arrange

            // Add test neighborhood to database.
            TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, ImgFilesNull);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Add valid commment. 
            TestHelper.NeighborhoodServiceObj.AddComment(testNeighborhood, ValidComment);

            // Store the commentId of the newly stored comment. 
            var commentId = testNeighborhood.comments.Last().CommentId;

            // Act
            var result = TestHelper.NeighborhoodServiceObj.DeleteComment(testNeighborhood, commentId);

            // Assert
            Assert.AreEqual(true, result);
        }

        /// <summary>
        /// Tests that upon DeleteComment, count of comments stored in the neighborhood
        /// has decreased by 1. 
        /// </summary>
        [Test]
        public void DeleteComment_Comments_Count_Decrease_By_1_Returns_True()
        {
            // Arrange

            // Add test neighborhood to database.
            TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, ImgFilesNull);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Add valid commment, store count of comments, store the comment's id. 
            TestHelper.NeighborhoodServiceObj.AddComment(testNeighborhood, ValidComment);
            var commentCount = testNeighborhood.comments.Count();
            var commentId = testNeighborhood.comments.Last().CommentId;

            // Act
            TestHelper.NeighborhoodServiceObj.DeleteComment(testNeighborhood, commentId);

            // Assert
            Assert.AreEqual(commentCount - 1, testNeighborhood.comments.Count());
        }

        /// <summary>
        /// Tests that DeleteComment returns false when given an invalid commendId. 
        /// </summary>
        [Test]
        public void DeleteComment_InvalidId_Should_Return_False()
        {
            // Arrange
            var neighborhoodService = TestHelper.NeighborhoodServiceObj;
            var validNeighborhood = neighborhoodService.GetNeighborhoodById(1);
            string invalidId0 = "-1";
            string invalidId1 = "0";
            string invalidId2 = "bogus";

            // Act
            var result1 = neighborhoodService.DeleteComment(validNeighborhood, invalidId0);
            var result2 = neighborhoodService.DeleteComment(validNeighborhood, invalidId1);
            var result3 = neighborhoodService.DeleteComment(validNeighborhood, invalidId2);

            // Assert
            Assert.AreEqual(false, result1);
            Assert.AreEqual(false, result2);
            Assert.AreEqual(false, result3);
        }

        #endregion Comments

        #region Images
        /// <summary>
        /// Use AddData() function to test that an image file can be successfully uploaded. 
        /// Simulates image upload logic by replicating the steps taken when a new Neighborhood is created with 
        /// a valid image upload. Creates an IFormFile "file" using a mock MemoryStream object "stream" an
        /// adds the IFormFile to a FormFileCollection. 
        /// </summary>
        [Test]
        public void AddData_UploadImage_Valid_Successful()
        {
            // Arrange

            var imagePath = GetImagePath();

            // Act

            // Add test neighborhood to database.
            TestHelper.NeighborhoodServiceObj.AddData(Name, "", ShortDesc, imagePath);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Assert 
            Assert.AreEqual(Name, testNeighborhood.name);
            Assert.AreEqual(0, testNeighborhood.image.Count());
            Assert.AreEqual(ShortDesc, testNeighborhood.shortDesc);
            Assert.AreEqual("image/Neighborhood/test.jpg", testNeighborhood.imagePath);

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }

        /// <summary>
        /// Test GetAllImages returns "no_image.jpg" when called on a neighborhood with null Image and 
        /// ImagePath properties. 
        /// </summary>
        [Test]
        public void GetAllImages_No_URLImage_No_FileImage_CorrectImagePath_Return_True()
        {
            // Arrange

            // Add test neighborhood to database with NO IMAGE URL and NO IMAGE FILE.
            TestHelper.NeighborhoodServiceObj.AddData(Name, null, ShortDesc, null);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Act
            var result = TestHelper.NeighborhoodServiceObj.GetAllImages(testNeighborhood);

            // Assert
            Assert.AreEqual("/image/no_image.jpg", result.First());

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }

        /// <summary>
        /// Test that the count of images is increased by 1 when is called GetAllImages 
        /// on a neighborhood with null Image and ImagePath properties. 
        /// </summary>
        [Test]
        public void GetAllImages_No_URLImage_No_FileImage_Count_Return_True()
        {
            // Arrange

            // Add test neighborhood to database with NO IMAGE URL and NO IMAGE FILE.
            TestHelper.NeighborhoodServiceObj.AddData(Name, null, ShortDesc, null);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Act
            var result = TestHelper.NeighborhoodServiceObj.GetAllImages(testNeighborhood);

            // Assert
            Assert.AreEqual(1, result.Count());

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }

        /// <summary>
        /// Test GetAllImages returns a greater than 0 count of images when the neighborhood has no file images 
        /// and only URL images.
        /// </summary>
        [Test]
        public void GetAllImages_No_FileImage_Count_Return_True()
        {
            // Add test neighborhood to database with only Image property (no ImagePath property). 
            TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, null);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            var numOfURLImage = testNeighborhood.image.Split(",").Length;

            // Act
            var result = TestHelper.NeighborhoodServiceObj.GetAllImages(testNeighborhood);

            // Assert
            Assert.AreEqual(numOfURLImage, result.Count());
        }

        /// <summary>
        /// Test GetAllImages returns all images when called on a neighborhood with valid Image and
        /// ImagePath properties.
        /// </summary>
        [Test]
        public void GetAllImages_Has_URLImage_Has_FileImage_Return_AllImages()
        {
            // Arrange

            var imagePath = GetImagePath();

            // Add test neighborhood to database with NO IMAGE URL and NO IMAGE FILE.
            TestHelper.NeighborhoodServiceObj.AddData(Name, Image, ShortDesc, imagePath);

            // Retrieve test neighborhood.
            var testNeighborhood = TestHelper.NeighborhoodServiceObj.GetNeighborhoods().Last();

            // Store count of images in Image property and count of uploaded images in ImagePath property.
            var countOfURLImage = testNeighborhood.image.Split(",").Length;
            var countOfFileImage = testNeighborhood.imagePath.Split(",").Length;

            // Act
            var result = TestHelper.NeighborhoodServiceObj.GetAllImages(testNeighborhood);

            // Assert 
            Assert.True(countOfURLImage > 0);
            Assert.True(countOfFileImage > 0);
            Assert.AreEqual(countOfURLImage + countOfFileImage, result.Count());

            // TearDown
            TestHelper.NeighborhoodServiceObj.DeleteData(testNeighborhood.id);
        }

        #endregion Images

    }
}