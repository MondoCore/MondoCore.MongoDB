using Microsoft.VisualStudio.TestTools.UnitTesting;

using MondoCore.Repository.TestHelper;
using MondoCore.TestHelpers;

using MongoDB.Bson;

namespace MondoCore.MongoDB.FunctionalTests
{
    [TestClass]
    public class MongoCollection_ObjectIdTests : RepositoryTestBase<ObjectId>
    {
        public MongoCollection_ObjectIdTests() :

           base(new MondoCore.MongoDB.MongoDB("functionaltests", TestConfiguration.Load().ConnectionString),
                "cars2",
                ()=> ObjectId.GenerateNewId())
        {
        }
    }
}
