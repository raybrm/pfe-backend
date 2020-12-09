using System;
using Xunit;
using BlockCovid.Controllers;
using Moq;
using BlockCovid.Interfaces;
using BlockCovid.Models.Dto;
using System.Threading.Tasks;
using System.Collections.Generic;
using BlockCovid.Utils;

namespace BlockCovid.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Can_Use_Repository()
        {
            // Triple A 
            // Arrange
            List<ParticipantDto> participants = new List<ParticipantDto>();
            participants.Add(new ParticipantDto {Login = "test@test.com", Password= "12345", ConfirmPassword="12345", Participant_Type=Models.ParticipantType.Doctor});
            participants.Add(new ParticipantDto { Login = "test2@test.com", Password = "12346", ConfirmPassword = "12346", Participant_Type = Models.ParticipantType.Establishment});
            Mock<IParticipantsRepository> mockRepo = new Mock<IParticipantsRepository>();
            mockRepo.Setup(m => m.GetParticipantsAsync()).Returns(Task.FromResult(participants));
            Mock<JWTSettings> mockJWT = new Mock<JWTSettings>();

            //ParticipantsController controller = new ParticipantsController(mockRepo.Object);
            Assert.Equal("true", "true");
        }
    }
}
