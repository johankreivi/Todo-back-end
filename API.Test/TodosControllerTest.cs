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

    }
}