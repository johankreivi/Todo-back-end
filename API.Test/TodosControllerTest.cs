using Infrastructure.Repositories;
using Moq;

namespace Api.Test
{
    [TestClass]
    public class TodosControllerTest
    {

        private Mock<ITodoRepository> _todoRepositoryMock;

        [TestInitialize]
        public void Setup()
        {
            _todoRepositoryMock = new Mock<ITodoRepository>();
        }
        
    }
}