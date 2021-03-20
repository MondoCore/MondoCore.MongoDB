using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB = MondoCore.MongoDB;

using MondoCore.Common;
using MondoCore.Data;
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
