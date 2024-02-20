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
    public class DepartmentTest
    {
        [Fact]
        public async Task TestCreateDepartmentHappyWay()
        {
            //ARRANGE
            var mockQuery = new Mock<IDepartmentQuery>();
            var mockCommand = new Mock<IDepartmentCommand>();
            var validatorMock = new Mock<IValidator<DepartmentRequest>>();
            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<DepartmentRequest>(), default))
                .ReturnsAsync(new ValidationResult());
            mockCommand.Setup(command => command.InsertDepartment(It.IsAny<Department>())).ReturnsAsync(1);
            var service = new DepartmentService(mockQuery.Object, mockCommand.Object, validatorMock.Object);

            var request = new DepartmentRequest
            {
                Name = "DepartmentTest",
                IdCompany = 1
            };

            //ACT
            var amountOfModifiedRegisters = await service.CreateDepartment(request);

            //ASSERT
            Assert.Equal(1, amountOfModifiedRegisters);
        }
        
        [Fact]
        public async Task TestCreateDepartmentEmptyName()
        {
            //ARRANGE
            var mockDepartmentQuery = new Mock<IDepartmentQuery>();
            var mockCompanyQuery = new Mock<ICompanyQuery>();
            mockCompanyQuery.Setup(q => q.ExistCompany(It.IsAny<int>()))
                .ReturnsAsync(true);
            var mockCommand = new Mock<IDepartmentCommand>();
            var validator = new CreateDepartmentValidator(mockCompanyQuery.Object);
            mockCommand.Setup(command => command.InsertDepartment(It.IsAny<Department>())).ReturnsAsync(1);
            var service = new DepartmentService(mockDepartmentQuery.Object, mockCommand.Object, validator);

            var request = new DepartmentRequest
            {
                Name = "",
                IdCompany = 1
            };

            //ACT & ASSERT
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await service.CreateDepartment(request));
        }

        [Fact]
        public async Task TestCreateDepartmentLargeName()
        {
            //ARRANGE
            var mockDepartmentQuery = new Mock<IDepartmentQuery>();
            var mockCompanyQuery = new Mock<ICompanyQuery>();
            mockCompanyQuery.Setup(q => q.ExistCompany(It.IsAny<int>()))
                .ReturnsAsync(true);
            var mockCommand = new Mock<IDepartmentCommand>();
            var validator = new CreateDepartmentValidator(mockCompanyQuery.Object);
            mockCommand.Setup(command => command.InsertDepartment(It.IsAny<Department>())).ReturnsAsync(1);
            var service = new DepartmentService(mockDepartmentQuery.Object, mockCommand.Object, validator);

            var request = new DepartmentRequest
            {
                Name = "123456789123456789123456789123456789123456789123456789123456789",
                IdCompany = 1
            };

            //ACT & ASSERT
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await service.CreateDepartment(request));
        }

        [Fact]
        public async Task TestCreateDepartmentBadCompany()
        {
            //ARRANGE
            var mockDepartmentQuery = new Mock<IDepartmentQuery>();
            var mockCompanyQuery = new Mock<ICompanyQuery>();
            mockCompanyQuery.Setup(q => q.ExistCompany(It.IsAny<int>()))
                .ReturnsAsync(false);
            var mockCommand = new Mock<IDepartmentCommand>();
            var validator = new CreateDepartmentValidator(mockCompanyQuery.Object);
            mockCommand.Setup(command => command.InsertDepartment(It.IsAny<Department>())).ReturnsAsync(1);
            var service = new DepartmentService(mockDepartmentQuery.Object, mockCommand.Object, validator);

            var request = new DepartmentRequest
            {
                Name = "companyName",
                IdCompany = 1
            };

            //ACT & ASSERT
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await service.CreateDepartment(request));
        }

        [Fact]
        public async Task TestGetDepartmentsByCompanyHappyWay()
        {
            //ARRANGE
            var mockQuery = new Mock<IDepartmentQuery>();
            var mockCommand = new Mock<IDepartmentCommand>();
            var list = new List<Department>();
            var response = new Department
            {
                Name = "DepartmentTest",
                IdCompany = 1
            };
            list.Add(response);
            response = new Department
            {
                Name = "DepartmentTest2",
                IdCompany = 2
            };
            list.Add(response);
            var validatorMock = new Mock<IValidator<DepartmentRequest>>();
            mockQuery.Setup(q => q.GetDepartmentsByCompany(It.IsAny<int>()))
                .ReturnsAsync(list); 
            var service = new DepartmentService(mockQuery.Object, mockCommand.Object, validatorMock.Object);

            //ACT
            var result = await service.GetDepartmentsByCompany(1);

            //ASSERT
            Assert.Equal(result.Count, list.Count);
        }


        [Fact]
        public async Task TestGetDepartmentsByCompanyBadRequest()
        {
            //ARRANGE
            var mockQuery = new Mock<IDepartmentQuery>();
            var mockCommand = new Mock<IDepartmentCommand>();
            var list = new List<Department>();
            var validatorMock = new Mock<IValidator<DepartmentRequest>>();
            mockQuery.Setup(q => q.GetDepartmentsByCompany(It.IsAny<int>()))
                .ReturnsAsync(list);
            var service = new DepartmentService(mockQuery.Object, mockCommand.Object, validatorMock.Object);

            //ACT & ASSERT
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await service.GetDepartmentsByCompany(-1));
        }

        [Fact]
        public async Task TestGetDepartmentsByCompanyEmptyList()
        {
            //ARRANGE
            var mockQuery = new Mock<IDepartmentQuery>();
            var mockCommand = new Mock<IDepartmentCommand>();
            var list = new List<Department>();
            var validatorMock = new Mock<IValidator<DepartmentRequest>>();
            mockQuery.Setup(q => q.GetDepartmentsByCompany(It.IsAny<int>()))
                .ReturnsAsync(list);
            var service = new DepartmentService(mockQuery.Object, mockCommand.Object, validatorMock.Object);

            //ACT
            var result = await service.GetDepartmentsByCompany(1);

            //ASSERT
            Assert.Equal(result.Count, list.Count);
        }

        [Fact]
        public async Task TestGetDepartmentHappyWay()
        {
            //ARRANGE
            var mockQuery = new Mock<IDepartmentQuery>();
            var mockCommand = new Mock<IDepartmentCommand>();
            var response = new Department
            {
                Name = "DepartmentTest2",
                IdCompany = 2
            };
            mockQuery.Setup(v => v.GetDepartment(It.IsAny<int>()))
                .ReturnsAsync(response);
            var validatorMock = new Mock<IValidator<DepartmentRequest>>();
            var service = new DepartmentService(mockQuery.Object, mockCommand.Object, validatorMock.Object);

            //ACT
            var result = await service.GetDepartment(1);

            //ASSERT
            Assert.Equal(result.Name, response.Name);
        }

        [Fact]
        public async Task TestGetDepartmentNotFound()
        {
            //ARRANGE
            var mockQuery = new Mock<IDepartmentQuery>();
            var mockCommand = new Mock<IDepartmentCommand>();
            var validatorMock = new Mock<IValidator<DepartmentRequest>>();
            Department? response = null;
            mockQuery.Setup(v => v.GetDepartment(It.IsAny<int>()))
                .ReturnsAsync(response!);
            var service = new DepartmentService(mockQuery.Object, mockCommand.Object, validatorMock.Object);

            //ACT && ASSERT
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await service.GetDepartment(1));
        }

        [Fact]
        public async Task TestGetDepartmentBadRequest()
        {
            //ARRANGE
            var mockQuery = new Mock<IDepartmentQuery>();
            var mockCommand = new Mock<IDepartmentCommand>();
            var validatorMock = new Mock<IValidator<DepartmentRequest>>();
            var service = new DepartmentService(mockQuery.Object, mockCommand.Object, validatorMock.Object);

            //ACT && ASSERT
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await service.GetDepartment(-1));
        }

        [Fact]
        public async Task TestDeleteHappyWay()
        {
            //ARRANGE
            var validatorMock = new Mock<IValidator<DepartmentRequest>>();
            var mockQuery = new Mock<IDepartmentQuery>();
            var mockCommand = new Mock<IDepartmentCommand>(); 
            var queryResponse = new Department
            {
                Name = "DepartmentTest2",
                IdCompany = 2
            };
            mockQuery.Setup(v => v.GetDepartment(It.IsAny<int>()))
                .ReturnsAsync(queryResponse);
            mockCommand.Setup(v => v.DeleteDepartment(It.IsAny<Department>()))
                .ReturnsAsync(1);
            var service = new DepartmentService(mockQuery.Object, mockCommand.Object, validatorMock.Object);

            //ACT
            var amountOfModifiedRegisters = await service.DeleteDepartment(1);

            //ASSERT
            Assert.Equal(1, amountOfModifiedRegisters);
        }

        [Fact]
        public async Task TestDeleteDepartmentBadRequest()
        {
            //ARRANGE
            var mockQuery = new Mock<IDepartmentQuery>();
            var mockCommand = new Mock<IDepartmentCommand>();
            var validatorMock = new Mock<IValidator<DepartmentRequest>>();
            var service = new DepartmentService(mockQuery.Object, mockCommand.Object, validatorMock.Object);

            //ACT && ASSERT
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await service.DeleteDepartment(-1));
        }

        [Fact]
        public async Task TestDeleteDepartmentNotFound()
        {
            //ARRANGE
            var mockQuery = new Mock<IDepartmentQuery>();
            var mockCommand = new Mock<IDepartmentCommand>();
            var validatorMock = new Mock<IValidator<DepartmentRequest>>();
            Department? response = null;
            mockQuery.Setup(v => v.GetDepartment(It.IsAny<int>()))
                .ReturnsAsync(response!);
            var service = new DepartmentService(mockQuery.Object, mockCommand.Object, validatorMock.Object);

            //ACT && ASSERT
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await service.DeleteDepartment(1));
        }
    }
}