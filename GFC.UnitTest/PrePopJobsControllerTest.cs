using GFC.Api.Controllers.Configurations.CoreConfiguration;
using GFC.BAL.Interfaces;
using GFC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace GFC.UnitTest
{
    public class PrePopJobsControllerTest
    {
        PrePopJobsController _controller;
        IPrePopJobsBAL _service;

        public PrePopJobsControllerTest()
        {
            _service = new PrePopJobsControllerMock();
            _controller = new PrePopJobsController(_service);
        }

        [Fact]
        public void Get_WhenCalled_ReturnsOkResult()
        {
            // Act
            var okResult = _controller.GetPrePopulationJobs();
            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void Get_WhenCalled_ReturnsAllItems()
        {
            // Act
            var okResult = _controller.GetPrePopulationJobs().Result as OkObjectResult;
            // Assert
            var items = Assert.IsType<List<PrePopJobModel>>(okResult.Value);
            Assert.Equal(3, items.Count);
        }

        [Fact]
        public void Add_InvalidObjectPassed_ReturnsBadRequest()
        {
            // Arrange
            //var nameMissingItem = new PrePopulationJob()
            //{

            //};
            PrePopulationJob testItem = new PrePopulationJob()
            {
                isFilterByModifiedTime = true,
                StartTime = DateTime.Now,
                StopTime = DateTime.Now,
        };
            _controller.ModelState.AddModelError("CacheName", "Required");

            // Act
            var badResponse = _controller.CreatePrePopulationJob(testItem);

            // Assert
            Assert.IsType<BadRequestObjectResult>(badResponse);
        }


        [Fact]
        public void Add_ValidObjectPassed_ReturnsCreatedResponse()
        {
            // Arrange
            PrePopulationJob testItem = new PrePopulationJob()
            {
                CacheName = new string[] { "setting1" },
                isFilterByModifiedTime = true,
                StartTime = DateTime.Now,
                StopTime = DateTime.Now
            
            };

            // Act
            var createdResponse = _controller.CreatePrePopulationJob(testItem);

            // Assert
            Assert.IsType<OkResult>(createdResponse);
        }


        [Fact]
        public void Add_ValidObjectPassed_ReturnedResponseHasCreatedItem()
        {
            // Arrange
            var testItem = new PrePopulationJob()
            {
                CacheName = new string[] { "setting1" },
                isFilterByModifiedTime = true,
                StartTime = DateTime.Now,
                StopTime = DateTime.Now
            };

            // Act
            var createdResponse = _controller.CreatePrePopulationJob(testItem) as CreatedAtActionResult;
            var item = createdResponse.Value as PrePopulationJob;

            // Assert
            Assert.IsType<PrePopulationJob>(item);
            Assert.Equal("PrePop Job Created", item.JobID);
        }
    }
}
