/***************************************************************************
 *                                                                          
 *    The MondoCore Libraries  	                                            
 *                                                                          
 *      Namespace: MondoCore.MongoDB	                                        
 *           File: MongoDB.cs                                                
 *      Class(es): MongoDB                                                   
 *        Purpose: Wrapper of MongoDB API using IRead/WriteRepository                              
 *                                                                          
 *  Original Author: Jim Lightfoot                                         
 *    Creation Date: 28 Feb 2021                                           
 *                                                                          
 *   Copyright (c) 2021 - Jim Lightfoot, All rights reserved                
 *                                                                          
 *  Licensed under the MIT license:                                         
 *    http://www.opensource.org/licenses/mit-license.php                    
 *                                                                          
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using MongoDB.Driver;

using MondoCore.Data;

namespace MondoCore.MongoDB
{
    /// <summary>
    /// Provides methods to access a MongoDB
    /// </summary>
    public class MongoDB : IDatabase
    {
        protected readonly IMongoDatabase _db;

        public MongoDB(string dbName, string connectionString)
        {
            var client = new MongoClient(connectionString);

            _db = client.GetDatabase(dbName);
        }

        /// <summary>
        /// Create a repository reader for the MongoDB collection
        /// </summary>
        /// <typeparam name="TID">Type of the identifier</typeparam>
        /// <typeparam name="TValue">Type of the value stored in the collection</typeparam>
        /// <param name="repoName">Name of MongoDB collection</param>
        /// <returns>A reader to make read operations</returns>
        public IReadRepository<TID, TValue> GetRepositoryReader<TID, TValue>(string repoName, IIdentifierStrategy<TID> strategy = null) where TValue : IIdentifiable<TID>
        {
            var mongoCollection = _db.GetCollection<TValue>(repoName);

            return new MongoCollectionReader<TID, TValue>(mongoCollection);
        }

       /// <summary>
        /// Create a repository writer for the MongoDB collection
        /// </summary>
        /// <typeparam name="TID">Type of the identifier</typeparam>
        /// <typeparam name="TValue">Type of the value stored in the collection</typeparam>
        /// <param name="repoName">Name of MongoDB collection</param>
        /// <returns>A writer to make writer operations</returns>
        public IWriteRepository<TID, TValue> GetRepositoryWriter<TID, TValue>(string repoName, IIdentifierStrategy<TID> strategy = null) where TValue : IIdentifiable<TID>
        {
            var mongoCollection = _db.GetCollection<TValue>(repoName);

            return new MongoCollectionWriter<TID, TValue>(mongoCollection);
        }
    }
}
