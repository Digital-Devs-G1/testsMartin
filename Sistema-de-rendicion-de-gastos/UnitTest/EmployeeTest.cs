using Application.DTO.Request;
using Application.DTO.Response;
using Application.Exceptions;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.UseCases;
using Azure;
using Azure.Core;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Repositories.Querys;
using Moq;
using System.Collections;

namespace UnitTest
{
    public class EmployeeTest
    {
        [Fact]
        public async Task TestGetNextApproverEmployeeBadRequest()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            var position = new Position()
            {
                Id = 1,
                Name = "Leader",
                Hierarchy = 1,
                MaxAmount = 100,
                IdCompany = 1
            };
            var response = new Employee
            {
                Id = 1,
                FirstName = "Jhon",
                LastName = "Kenedy",
                DepartamentId = 2,
                SuperiorId = 2,
                PositionId = 1,
                Position = position,
                IsApprover = false
            };
            mockQuery.Setup(q => q.GetEmployee(It.IsAny<int>()))
                .ReturnsAsync(response);
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT && ASSERT
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await service.GetNextApprover(-1, 10));
        }

        [Fact]
        public async Task TestGetNextApproverAmountBadRequest()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            var position = new Position()
            {
                Id = 1,
                Name = "Leader",
                Hierarchy = 1,
                MaxAmount = 100,
                IdCompany = 1
            };
            var response = new Employee
            {
                Id = 1,
                FirstName = "Jhon",
                LastName = "Kenedy",
                DepartamentId = 2,
                SuperiorId = 2,
                PositionId = 1,
                Position = position,
                IsApprover = false
            };
            mockQuery.Setup(q => q.GetEmployee(It.IsAny<int>()))
                .ReturnsAsync(response);
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT && ASSERT
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await service.GetNextApprover(1, -1));
        }

        [Fact]
        public async Task TestGetNextApproverEmployeeNotFound()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            Employee? response = null;
            mockQuery.Setup(q => q.GetEmployee(It.IsAny<int>()))
                .ReturnsAsync(response);
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT && ASSERT
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await service.GetNextApprover(1, 10));
        }

        [Fact]
        public async Task TestGetNextApproverHappyWay()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            var position = new Position()
            {
                Id = 1,
                Name = "Leader",
                Hierarchy = 1,
                MaxAmount = 100,
                IdCompany = 1
            };
            int superiorId = 2;
            var response = new Employee
            {
                Id = 1,
                FirstName = "Jhon",
                LastName = "Kenedy",
                DepartamentId = 2,
                SuperiorId = superiorId,
                PositionId = 1,
                Position = position,
                IsApprover = false
            };
            mockQuery.Setup(q => q.GetEmployee(It.IsAny<int>()))
                .ReturnsAsync(response);
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT
            var result = await service.GetNextApprover(1, 1000);

            //ASSERT
            Assert.Equal(result, superiorId);
        }

        [Fact]
        public async Task TestGetNextApproverBigBoss()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            var position = new Position()
            {
                Id = 1,
                Name = "Leader",
                Hierarchy = 1,
                MaxAmount = 100,
                IdCompany = 1
            };
            var response = new Employee
            {
                Id = 1,
                FirstName = "Jhon",
                LastName = "Kenedy",
                DepartamentId = 2,
                SuperiorId = null,
                PositionId = 1,
                Position = position,
                IsApprover = false
            };
            mockQuery.Setup(q => q.GetEmployee(It.IsAny<int>()))
                .ReturnsAsync(response);
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT
            var result = await service.GetNextApprover(1, 1000);

            //ASSERT
            Assert.Equal(result, 0);
        }

        [Fact]
        public async Task TestEnableHistoryFlagHappyWay()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            var response = new Employee
            {
                Id = 1,
                FirstName = "Jhon",
                LastName = "Kenedy",
                DepartamentId = 2,
                SuperiorId = 2,
                PositionId = 1,
                IsApprover = false
            };
            mockCommand.Setup(command => command.AcceptHistoryFlag(It.IsAny<Employee>()))
                .ReturnsAsync(1);
            mockQuery.Setup(q => q.GetEmployee(It.IsAny<int>()))
                .ReturnsAsync(response);
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT
            var result = await service.EnableHistoryFlag(1);

            //ASSERT
            Assert.Equal(result, 1);
        }

        [Fact]
        public async Task TestEnableHistoryFlagEmployeeIdFormatException()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT && ASSERT
            await Assert.ThrowsAsync<EmployeeIdFormatException>(async () =>
                await service.EnableHistoryFlag(-1));
        }

        [Fact]
        public async Task TestEnableHistoryFlagNotFound()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            Employee? response = null;
            mockQuery.Setup(q => q.GetEmployee(It.IsAny<int>()))
                .ReturnsAsync(response);
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT && ASSERT
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await service.EnableHistoryFlag(1));
        }

        [Fact]
        public async Task TestEnableApprovalsFlagFlagHappyWay()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            var response = new Employee
            {
                Id = 1,
                FirstName = "Jhon",
                LastName = "Kenedy",
                DepartamentId = 2,
                SuperiorId = 2,
                PositionId = 1,
                IsApprover = false
            };
            mockCommand.Setup(command => command.AcceptApprovalsFlagFlag(It.IsAny<Employee>()))
                .ReturnsAsync(1);
            mockQuery.Setup(q => q.GetEmployee(It.IsAny<int>()))
                .ReturnsAsync(response);
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT
            var result = await service.EnableApprovalsFlagFlag(1);

            //ASSERT
            Assert.Equal(result, 1);
        }

        [Fact]
        public async Task TestEnableApprovalsFlagFlagEmployeeIdFormatException()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT && ASSERT
            await Assert.ThrowsAsync<EmployeeIdFormatException>(async () =>
                await service.EnableApprovalsFlagFlag(-1));
        }

        [Fact]
        public async Task TestEnableApprovalsFlagFlagNotFound()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            Employee? response = null;
            mockQuery.Setup(q => q.GetEmployee(It.IsAny<int>()))
                .ReturnsAsync(response);
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT && ASSERT
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await service.EnableApprovalsFlagFlag(1));
        }

        [Fact]
        public async Task TestDisableHistoryFlagHappyWay()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            var response = new Employee
            {
                Id = 1,
                FirstName = "Jhon",
                LastName = "Kenedy",
                DepartamentId = 2,
                SuperiorId = 2,
                PositionId = 1,
                IsApprover = false
            };
            mockCommand.Setup(command => command.DissmisHistoryFlag(It.IsAny<Employee>()))
                .ReturnsAsync(1);
            mockQuery.Setup(q => q.GetEmployee(It.IsAny<int>()))
                .ReturnsAsync(response);
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);
            
            //ACT
            var result = await service.DisableHistoryFlag(1);

            //ASSERT
            Assert.Equal(result, 1);
        }

        [Fact]
        public async Task TestDisableHistoryFlagEmployeeIdFormatException()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            var response = new Employee
            {
                Id = 1,
                FirstName = "Jhon",
                LastName = "Kenedy",
                DepartamentId = 2,
                SuperiorId = 2,
                PositionId = 1,
                IsApprover = false
            };
            mockQuery.Setup(q => q.GetEmployee(It.IsAny<int>()))
                .ReturnsAsync(response);
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT && ASSERT
            await Assert.ThrowsAsync<EmployeeIdFormatException>(async () =>
                await service.DisableHistoryFlag(-1));
        }

        [Fact]
        public async Task TestDisableHistoryFlagNotFound()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            Employee? response = null;
            mockQuery.Setup(q => q.GetEmployee(It.IsAny<int>()))
                .ReturnsAsync(response);
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT && ASSERT
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await service.DisableHistoryFlag(1));
        }

        [Fact]
        public async Task TestDisableApprovalsFlagHappyWay()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            var response = new Employee
            {
                Id = 1,
                FirstName = "Jhon",
                LastName = "Kenedy",
                DepartamentId = 2,
                SuperiorId = 2,
                PositionId = 1,
                IsApprover = false
            };
            mockCommand.Setup(command => command.DissmisApprovalsFlag(It.IsAny<Employee>()))
                .ReturnsAsync(1);
            mockQuery.Setup(q => q.GetEmployee(It.IsAny<int>()))
                .ReturnsAsync(response);
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT
            var result = await service.DisableApprovalsFlag(1);

            //ASSERT
            Assert.Equal(result, 1);
        }

        [Fact]
        public async Task TestDisableApprovalsFlagEmployeeIdFormatException()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT && ASSERT
            await Assert.ThrowsAsync<EmployeeIdFormatException>(async () =>
                await service.DisableApprovalsFlag(-1));
        }

        [Fact]
        public async Task TestDisableApprovalsFlagNotFound()
        {
            //ARRANGE
            var mockQuery = new Mock<IEmployeeQuery>();
            var mockCommand = new Mock<IEmployeeCommand>();
            var mockPositionService = new Mock<IPositionService>();
            var validatorMock = new Mock<IValidator<EmployeeRequest>>();
            Employee? response = null;
            mockQuery.Setup(q => q.GetEmployee(It.IsAny<int>()))
                .ReturnsAsync(response);
            var service = new EmployeeService(mockCommand.Object, mockQuery.Object, mockPositionService.Object, validatorMock.Object);

            //ACT && ASSERT
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await service.DisableApprovalsFlag(1));
        }

        /*
         * GetApprover **
         */

    }
}