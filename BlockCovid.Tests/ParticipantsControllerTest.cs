using System;
using Xunit;
using BlockCovid.Controllers;
using Moq;
using BlockCovid.Interfaces;
using BlockCovid.Models.Dto;
using BlockCovid.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using BlockCovid.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace BlockCovid.Tests
{
    public class ParticipantsControllerTest
    {
        [Fact]
        public async void Can_Use_Repository()
        {
            // Triple A 
            // Arrange
            Participant participant = new Participant { Login = "test@test.com", Password = "12345", Participant_Type = Models.ParticipantType.Doctor };
            List<ParticipantDto> participants = new List<ParticipantDto>();
            participants.Add(new ParticipantDto {Login = "test@test.com", Password= "12345", ConfirmPassword="12345", Participant_Type=Models.ParticipantType.Doctor});
            participants.Add(new ParticipantDto {Login = "test2@test.com", Password = "12346", ConfirmPassword = "12346", Participant_Type = Models.ParticipantType.Establishment});
            Mock<IParticipantsRepository> mockRepo = new Mock<IParticipantsRepository>();
            mockRepo.Setup(m => m.GetParticipantsAsync()).Returns(Task.FromResult(participants));
            Mock<JWTSettings> mockJWT = new Mock<JWTSettings>();

            ParticipantsController controller = new ParticipantsController(mockRepo.Object, mockJWT.Object);

            //ACT 
            ActionResult<IEnumerable<ParticipantDto>> result = await controller.GetParticipants();
            IEnumerable<ParticipantDto> created = result.Value;

            //Assert
            Assert.Equal(created, participants);
        }

        [Fact]
        public async void InvalidModelTest()
        {
            // Triple A 
            // Arrange
            Mock<IParticipantsRepository> mockRepo = new Mock<IParticipantsRepository>();
            Mock<JWTSettings> mockJWT = new Mock<JWTSettings>();
            Mock<IMapper> mockMapper = new Mock<IMapper>();

            ParticipantsController controller = new ParticipantsController(mockRepo.Object, mockJWT.Object);
           controller.ModelState.AddModelError("Login", "Required"); // need to manually simulate the model state error because the model state validation is only triggered during runtime.
            var model = new ParticipantDto
            {
                Password = "12345",
                ConfirmPassword = "12345",
                Participant_Type = ParticipantType.Doctor
            };

            //ACT
            ActionResult<ParticipantDto> result = await controller.PostParticipant(model);

            //ASSERT
            Assert.IsType<BadRequestObjectResult>(result.Result);

        }

        [Fact]
        public async void BadConfirmPassword()
        {
            // Triple A 
            // Arrange
            Mock<IParticipantsRepository> mockRepo = new Mock<IParticipantsRepository>();
            Mock<JWTSettings> mockJWT = new Mock<JWTSettings>();
      
            ParticipantsController controller = new ParticipantsController(mockRepo.Object, mockJWT.Object);

            var model = new ParticipantDto
            {
                Login = "test@test.com",
                Password = "12345",
                ConfirmPassword = "12346",
                Participant_Type = ParticipantType.Doctor
            };

            //ACT
            var result = await controller.PostParticipant(model);

            //ASSERT
            Assert.IsType<StatusCodeResult>(result.Result);
        }


    }
}
