using Application.DTO.Request;
using Application.DTO.Response;
using Application.Exceptions;
using Application.Interfaces.IRepositories;
using Application.UseCases;
using Application.Validators;
using Azure;
using Azure.Core;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Collections;

namespace UnitTest
{
    public class PositionTest
    {
        [Fact]
        public async Task TestGetPositionHappyWay()
        {
            //ARRANGE
            var mockQuery = new Mock<IPositionQuery>();
            var mockCommand = new Mock<IPositionCommand>();
            var validatorMock = new Mock<IValidator<PositionRequest>>();
            var response = new Position
            {
                Name = "Lider",
                Hierarchy = 1,
                MaxAmount = 1000,
                IdCompany = 1
            };
            mockQuery.Setup(v => v.GetPosition(It.IsAny<int>()))
                .ReturnsAsync(response);
            var service = new PositionService(mockQuery.Object, mockCommand.Object, validatorMock.Object);

            //ACT
            var result = await service.GetPosition(1);

            //ASSERT
            Assert.Equal(result.Description, response.Name);
            Assert.Equal(result.Hierarchy, response.Hierarchy);
            Assert.Equal(result.MaxAmount, response.MaxAmount);
        }

        [Fact]
        public async Task TestCreatePositionHappyWay()
        {
            //ARRANGE
            var mockQuery = new Mock<IPositionQuery>();
            var mockCommand = new Mock<IPositionCommand>();
            var validatorMock = new Mock<IValidator<PositionRequest>>();
            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<PositionRequest>(), default))
                .ReturnsAsync(new ValidationResult());
            mockCommand.Setup(command => command.InsertPosition(It.IsAny<Position>())).ReturnsAsync(1);
            var service = new PositionService(mockQuery.Object, mockCommand.Object, validatorMock.Object);
            var request = new PositionRequest
            {
                Description = "Lider",
                Hierarchy = 1,
                MaxAmount = 1000,
                CompanyId = 1
            };

            //ACT
            var amountOfModifiedRegisters = await service.CreatePosition(request);

            //ASSERT
            Assert.Equal(1, amountOfModifiedRegisters);
        }

        [Fact]
        public async Task TestCreatePositionCommandError()
        {
            //ARRANGE
            var mockQuery = new Mock<IPositionQuery>();
            var mockCommand = new Mock<IPositionCommand>();
            var validatorMock = new Mock<IValidator<PositionRequest>>();
            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<PositionRequest>(), default))
                .ReturnsAsync(new ValidationResult());
            mockCommand.Setup(command => command.InsertPosition(It.IsAny<Position>())).ReturnsAsync(0);
            var service = new PositionService(mockQuery.Object, mockCommand.Object, validatorMock.Object);
            var request = new PositionRequest
            {
                Description = "Lider",
                Hierarchy = 1,
                MaxAmount = 1000,
                CompanyId = 1
            };

            //ACT
            var amountOfModifiedRegisters = await service.CreatePosition(request);

            //ASSERT
            Assert.Equal(0, amountOfModifiedRegisters);
        }

        [Fact]
        public async Task TestCreatePositionEmptyDescription()
        {
            //ARRANGE
            var mockQuery = new Mock<IPositionQuery>();
            var mockCompanyQuery = new Mock<ICompanyQuery>();
            mockCompanyQuery.Setup(q => q.ExistCompany(It.IsAny<int>()))
                .ReturnsAsync(true);
            var mockCommand = new Mock<IPositionCommand>();
            var validator = new CreatePositionValidator(mockCompanyQuery.Object);
            var service = new PositionService(mockQuery.Object, mockCommand.Object, validator);

            var request = new PositionRequest
            {
                Description = "",
                Hierarchy = 1,
                MaxAmount = 1000,
                CompanyId = 1
            };

            //ACT & ASSERT
            await Assert.ThrowsAsync<BadRequestException> (async ()=> 
                await service.CreatePosition(request));
        }

        [Fact]
        public async Task TestCreatePositionEmptyHierarchy()
        {
            //ARRANGE
            var mockQuery = new Mock<IPositionQuery>();
            var mockCompanyQuery = new Mock<ICompanyQuery>();
            mockCompanyQuery.Setup(q => q.ExistCompany(It.IsAny<int>()))
                .ReturnsAsync(true);
            var mockCommand = new Mock<IPositionCommand>();
            var validator = new CreatePositionValidator(mockCompanyQuery.Object);
            var service = new PositionService(mockQuery.Object, mockCommand.Object, validator);

            var request = new PositionRequest
            {
                Description = "unaDescripcion",
                Hierarchy = 0,
                MaxAmount = 1000,
                CompanyId = 1
            };

            //ACT & ASSERT
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await service.CreatePosition(request));
        }

        [Fact]
        public async Task TestCreatePositionEmptyMaxAmount()
        {
            //ARRANGE
            var mockQuery = new Mock<IPositionQuery>();
            var mockCompanyQuery = new Mock<ICompanyQuery>();
            mockCompanyQuery.Setup(q => q.ExistCompany(It.IsAny<int>()))
                .ReturnsAsync(true);
            var mockCommand = new Mock<IPositionCommand>();
            var validator = new CreatePositionValidator(mockCompanyQuery.Object);
            var service = new PositionService(mockQuery.Object, mockCommand.Object, validator);

            var request = new PositionRequest
            {
                Description = "unaDescripcion",
                Hierarchy = 1,
                MaxAmount = 0,
                CompanyId = 1
            };

            //ACT & ASSERT
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await service.CreatePosition(request));
        }

        [Fact]
        public async Task TestCreatePositionEmptyCompanyId()
        {
            //ARRANGE
            var mockQuery = new Mock<IPositionQuery>();
            var mockCompanyQuery = new Mock<ICompanyQuery>();
            mockCompanyQuery.Setup(q => q.ExistCompany(It.IsAny<int>()))
                .ReturnsAsync(false);
            var mockCommand = new Mock<IPositionCommand>();
            var validator = new CreatePositionValidator(mockCompanyQuery.Object);
            var service = new PositionService(mockQuery.Object, mockCommand.Object, validator);

            var request = new PositionRequest
            {
                Description = "unaDescripcion",
                Hierarchy = 1,
                MaxAmount = 1000,
                CompanyId = 1
            };

            //ACT & ASSERT
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await service.CreatePosition(request));
        }

        [Fact]
        public async Task TestCreatePositionLargeDescription()
        {
            //ARRANGE
            var mockQuery = new Mock<IPositionQuery>();
            var mockCompanyQuery = new Mock<ICompanyQuery>();
            mockCompanyQuery.Setup(q => q.ExistCompany(It.IsAny<int>()))
                .ReturnsAsync(false);
            var mockCommand = new Mock<IPositionCommand>();
            var validator = new CreatePositionValidator(mockCompanyQuery.Object);
            var service = new PositionService(mockQuery.Object, mockCommand.Object, validator);

            var request = new PositionRequest
            {
                Description = "123456789123456789123456789123456789123456789123456789123456789",
                Hierarchy = 1,
                MaxAmount = 1000,
                CompanyId = 1
            };

            //ACT & ASSERT
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await service.CreatePosition(request));
        }

        [Fact]
        public async Task TestGetPositionsByCompanyHappyWay()
        {
            //ARRANGE
            var mockQuery = new Mock<IPositionQuery>();
            var mockCommand = new Mock<IPositionCommand>();
            var validatorMock = new Mock<IValidator<PositionRequest>>();
            var list = new List<Position>();
            var response = new Position
            {
                Name = "Lider",
                Hierarchy = 1,
                MaxAmount = 1000,
                IdCompany = 1
            };
            list.Add(response);
            mockQuery.Setup(q => q.GetPositionsByCompany(It.IsAny<int>()))
                .ReturnsAsync(list);
            var service = new PositionService(mockQuery.Object, mockCommand.Object, validatorMock.Object);

            //ACT
            var resultList = await service.GetPositionsByCompany(1);

            //ASSERT
            Assert.Equal(resultList.Count, list.Count);
            var result = resultList[0];
            Assert.Equal(result.Description, response.Name);
            Assert.Equal(result.Hierarchy, response.Hierarchy);
            Assert.Equal(result.MaxAmount, response.MaxAmount);
        }


        [Fact]
        public async Task TestGetPositionsByCompanyBadRequest()
        {
            //ARRANGE
            var mockQuery = new Mock<IPositionQuery>();
            var mockCommand = new Mock<IPositionCommand>();
            var validatorMock = new Mock<IValidator<PositionRequest>>();
            var list = new List<Position>();
            mockQuery.Setup(q => q.GetPositionsByCompany(It.IsAny<int>()))
                .ReturnsAsync(list);
            var service = new PositionService(mockQuery.Object, mockCommand.Object, validatorMock.Object);

            //ACT & ASSERT
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await service.GetPositionsByCompany(-1));
        }
    }
}