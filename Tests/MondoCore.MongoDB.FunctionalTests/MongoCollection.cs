using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB = MondoCore.MongoDB;

using MondoCore.Common;
using MondoCore.Data;

using MondoCore.TestHelpers;

using MondoCore.Repository.TestHelper;

namespace MondoCore.MongoDB.FunctionalTests
{
    [TestClass]
    public class MongoCollectionTests : RepositoryTestBase<Guid>
    {
        public MongoCollectionTests()  
            : base(new MondoCore.MongoDB.MongoDB("functionaltests", TestConfiguration.Load().ConnectionString),
                   "cars",
                   ()=> Guid.NewGuid())
        {
        }
    }
}
