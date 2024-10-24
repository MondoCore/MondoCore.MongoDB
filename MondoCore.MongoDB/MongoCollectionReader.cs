﻿/***************************************************************************
 *                                                                          
 *    The MondoCore Libraries  	                                            
 *                                                                          
 *      Namespace: MondoCore.MongoDB	                                        
 *           File: MongoCollectionReader.cs                                                
 *      Class(es): MongoCollectionReader                                                   
 *        Purpose: IReadRepository implentation of MongoDB collection                             
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
    internal class MongoCollectionReader<TID, TValue> : IReadRepository<TID, TValue> where TValue : IIdentifiable<TID> 
    {
        private readonly IMongoCollection<TValue> _collection;

        internal MongoCollectionReader(IMongoCollection<TValue> collection)
        {
            _collection = collection;
        }

        public async Task<long> Count(Expression<Func<TValue, bool>> query = null)
        {
            if(query == null)
                return await _collection.EstimatedDocumentCountAsync();

            return await _collection.CountDocumentsAsync(query);
        }

        #region IReadRepository

        public async Task<TValue> Get(TID id)
        {
            var filter = Builders<TValue>.Filter.Eq( item=> item.Id, id );

            var result = await _collection.Find(filter).FirstOrDefaultAsync();
            
            if(result == null)
                throw new NotFoundException();

            return result;
        }

        public async IAsyncEnumerable<TValue> Get(IEnumerable<TID> ids)
        {
            var filter = Builders<TValue>.Filter.In( item=> item.Id, ids);
            
            using(var cursor = await _collection.FindAsync(filter))
            {
                while(await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;

                    foreach(var document in batch)
                    {
                        yield return document;
                    }
                }
            }
        }

        public async IAsyncEnumerable<TValue> Get(Expression<Func<TValue, bool>> query)
        {
            var filter = Builders<TValue>.Filter.Where(query);
            
            using(var cursor = await _collection.FindAsync(filter))
            {
                while(await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;

                    foreach(var document in batch)
                    {
                        yield return document;
                    }
                }
            }
        }

        #region IQueryable<>

        #region IQueryable

        public Type             ElementType => typeof(TValue);
        public Expression       Expression  => _collection.AsQueryable<TValue>().Expression;
        public IQueryProvider   Provider    => _collection.AsQueryable<TValue>().Provider;

        #endregion

        #region IEnumerable<>

        public IEnumerator<TValue> GetEnumerator() => _collection.AsQueryable<TValue>().GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
          => _collection.AsQueryable<TValue>().GetEnumerator();

        #endregion

        #endregion

        #endregion
    }

}
