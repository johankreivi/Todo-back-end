using Api.Dto;
using Api.Controllers;
using AutoMapper;
using Entity;
using Infrastructure.Repositories;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Serilog;

namespace Api.Test
{
    [TestClass]
    public class TodosControllerTest
    {

        private Mock<ITodoRepository> _todoRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IRepository<Todo>> _repositoryMock;
        private TodosController _controller;

        [TestInitialize]
        public void Setup()
        {
            _todoRepositoryMock = new Mock<ITodoRepository>();
            _mapperMock = new Mock<IMapper>();
            _repositoryMock = new Mock<IRepository<Todo>>();

            // Setup a dummy logger to avoid null reference exceptions related to Log.Error.
            Log.Logger = new LoggerConfiguration().CreateLogger();

            _controller = new TodosController(_repositoryMock.Object, _mapperMock.Object);
        }

        [TestMethod]
        public async Task GetTodos_WhenTodosExist_ReturnsOkWithTodos()
        {
                        // Arrange
            var todos = new List<Todo>
            {
                new Todo { Id = 1, Title = "Todo 1"},
                new Todo { Id = 2, Title = "Todo 2"},
                new Todo { Id = 3, Title = "Todo 3"}
            };

            var todosDto = new List<TodoDto>
            {
                new TodoDto { Id = 1, Title = "Todo 1"},
                new TodoDto { Id = 2, Title = "Todo 2"},
                new TodoDto { Id = 3, Title = "Todo 3"}
            };

            _repositoryMock.Setup(x => x.GetAllAsync(1, 10)).ReturnsAsync(todos);
            _mapperMock.Setup(x => x.Map<IEnumerable<TodoDto>>(todos)).Returns(todosDto);

            var controller = new TodosController(_repositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await controller.GetTodos(1, 10);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var actualTodos = okResult.Value as IEnumerable<TodoDto>;
            Assert.IsNotNull(actualTodos);
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(3, actualTodos.Count());
        }

        [TestMethod]
        public async Task GetTodos_WhenNoTodosExist_ReturnsNoContent()
        {
            //Arrange
            var todos = new List<Todo>();
            _repositoryMock.Setup(x => x.GetAllAsync(1, 10)).ReturnsAsync(todos);
            var controller = new TodosController(_repositoryMock.Object, _mapperMock.Object);
            //Act
            var result = await controller.GetTodos(1, 10);
            //Assert
            Assert.IsNotNull(result);
            var noContentResult = result.Result as NoContentResult;
            Assert.IsNotNull(noContentResult);

        }

        [TestMethod]
        public async Task GetTodoCount_ReturnsCorrectCount_WhenNoExceptions()
        {
            // Arrange
            _repositoryMock.Setup(repo => repo.GetCountAsync()).ReturnsAsync(10);

            // Act
            var result = await _controller.GetTodoCount();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.AreEqual(10, okResult.Value);
        }

        [TestMethod]
        public async Task GetTodoCount_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            _repositoryMock.Setup(repo => repo.GetCountAsync()).ThrowsAsync(new Exception("Dummy exception for test"));

            // Act
            var result = await _controller.GetTodoCount();

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while fetching the todo count. Please try again later.", objectResult.Value);
        }

        [TestMethod]
        public async Task CreateTodo_ReturnsTodoDto_WhenSuccessful()
        {
            // Arrange
            var inputTodo = new Todo { Id = 1, Title = "TestTodo" };
            var expectedTodoDto = new TodoDto { Id = 1, Title = "TestTodo" };

            _repositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Todo>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(mapper => mapper.Map<TodoDto>(It.IsAny<Todo>())).Returns(expectedTodoDto);

            // Act
            var result = await _controller.CreateTodo(inputTodo);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.AreEqual(expectedTodoDto, okResult.Value as TodoDto);
        }

        [TestMethod]
        public async Task CreateTodo_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var inputTodo = new Todo { Id = 1, Title = "TestTodo" };

            _repositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Todo>())).ThrowsAsync(new Exception("Dummy exception for test"));

            // Act
            var result = await _controller.CreateTodo(inputTodo);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while creating the todo. Please try again later.", objectResult.Value);
        }

        [TestMethod]
        public async Task UpdateTodo_ShouldReturnUpdatedTodoDto_WhenTodoUpdateIsSuccessful()
        {
            // Arrange
            var sampleTodo = new Todo { Id = 1, Title = "Test Todo", Completed = false };
            var sampleTodoDto = new TodoDto { Id = 1, Title = "Test Todo", Completed = false };

            _repositoryMock.Setup(repo => repo.UpdateAsync(sampleTodo)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<TodoDto>(sampleTodo)).Returns(sampleTodoDto);

            var controller = new TodosController(_repositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await controller.Update(sampleTodo);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var okResult = result as OkObjectResult;
            var returnedTodoDto = okResult.Value as TodoDto;

            Assert.AreEqual(sampleTodoDto.Id, returnedTodoDto.Id);
            Assert.AreEqual(sampleTodoDto.Title, returnedTodoDto.Title);
        }

        [TestMethod]
        public async Task UpdateTodo_ShouldReturnInternalServerError_WhenUpdateThrowsException()
        {
            // Arrange
            var sampleTodo = new Todo { Id = 1, Title = "Test Todo", Completed = false };

            _repositoryMock.Setup(repo => repo.UpdateAsync(sampleTodo)).Throws(new Exception("Dummy exception for testing."));

            var controller = new TodosController(_repositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await controller.Update(sampleTodo);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));

            var errorResult = result as ObjectResult;
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, errorResult.StatusCode);
            Assert.AreEqual("An error occurred while updating the todo. Please try again later.", errorResult.Value);
        }

        [TestMethod]
        public async Task Delete_ReturnsOkResult_WhenEntitySuccessfullyDeleted()
        {
            // Arrange
            int testId = 1;
            _repositoryMock.Setup(repo => repo.DeleteAsync(testId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(testId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int testId = 1;
            _repositoryMock.Setup(repo => repo.DeleteAsync(testId)).ThrowsAsync(new Exception("Some exception"));

            // Act
            var result = await _controller.Delete(testId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
            Assert.AreEqual("An error occurred while deleting the todo. Please try again later.", statusCodeResult.Value);
        }

        [TestMethod]
        public async Task ChangeDeadline_ReturnsOkResult_WhenDeadlineSuccessfullyUpdated()
        {
            // Arrange
            var changeDeadlineRequest = new ChangeDeadlineRequest
            {
                id = 1,
                deadline = DateTime.Now.AddDays(7) // a week from now
            };

            _repositoryMock.Setup(repo => repo.UpdateDeadline(changeDeadlineRequest.id, changeDeadlineRequest.deadline))
                           .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ChangeDeadline(changeDeadlineRequest);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public async Task ChangeDeadline_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var changeDeadlineRequest = new ChangeDeadlineRequest
            {
                id = 1,
                deadline = DateTime.Now.AddDays(7)
            };

            _repositoryMock.Setup(repo => repo.UpdateDeadline(changeDeadlineRequest.id, changeDeadlineRequest.deadline))
                           .ThrowsAsync(new Exception("Some exception"));

            // Act
            var result = await _controller.ChangeDeadline(changeDeadlineRequest);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
            Assert.AreEqual("An error occurred while updating the deadline. Please try again later.", statusCodeResult.Value);
        }
    }
}