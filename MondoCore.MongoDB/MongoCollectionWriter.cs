/***************************************************************************
 *                                                                          
 *    The MondoCore Libraries  	                                            
 *                                                                          
 *      Namespace: MondoCore.MongoDB	                                        
 *           File: MongoCollectionWriter.cs                                                
 *      Class(es): MongoCollectionWriter                                                   
 *        Purpose: IWriterRepository implentation of MongoDB collection                             
 *                                                                          
 *  Original Author: Jim Lightfoot                                         
 *    Creation Date: 28 Feb 2021                                           
 *                                                                          
 *   Copyright (c) 2021-2024 - Jim Lightfoot, All rights reserved                
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

using MondoCore.Collections;
using MondoCore.Data;

namespace MondoCore.MongoDB
{
    internal class MongoCollectionWriter<TID, TValue> : IWriteRepository<TID, TValue> where TValue : IIdentifiable<TID>
    {
        private readonly IMongoCollection<TValue> _collection;

        internal MongoCollectionWriter(IMongoCollection<TValue> collection)
        {
            _collection = collection;
        }

        #region IWriteRepository

        public async Task<bool> Delete(TID id)
        {
            var filter = Builders<TValue>.Filter.Eq(item => item.Id, id);
            var result = await _collection.DeleteOneAsync(filter);

            return result.DeletedCount == 1;
        }

        public async Task<long> Delete(Expression<Func<TValue, bool>> guard)
        {
            var filter = Builders<TValue>.Filter.Where(guard);
            var result = await _collection.DeleteManyAsync(filter);

            return result.DeletedCount;
        }

        public async Task<TValue> Insert(TValue item)
        {
            if(item.Id.Equals(default(TID)))
                throw new ArgumentException("Item must have a valid id in order to add to collection");

            var options = new InsertOneOptions { BypassDocumentValidation = true };

            await _collection.InsertOneAsync(item, options);

            return item;
        }

        public Task Insert(IEnumerable<TValue> items)
        {
            var options = new InsertManyOptions { BypassDocumentValidation = true };

            return _collection.InsertManyAsync(items, options);
        }

        public async Task<bool> Update(TValue item, Expression<Func<TValue, bool>> guard = null)
        {
            var id     = item.Id;
            var filter = Builders<TValue>.Filter.Eq(obj => obj.Id, id);

            if(guard != null)
                filter =  Builders<TValue>.Filter.And(filter, Builders<TValue>.Filter.Where(guard));

            var result = await _collection.ReplaceOneAsync(filter, item);

            return result.ModifiedCount > 0;
        }

        public async Task<long> Update(object properties, Expression<Func<TValue, bool>> query)
        {
            var props    = properties.ToDictionary().ToList();
            var numProps = props.Count;

            if(numProps == 0)
                return 0;

            var updateDef = Builders<TValue>.Update.Set(props[0].Key, props[0].Value);

            for(var i = 1; i < numProps; ++i)
            { 
                var kv = props[i];

                updateDef = updateDef.Set(kv.Key, kv.Value);
            }

            var filter = Builders<TValue>.Filter.Where(query);
            var result = await _collection.UpdateManyAsync(filter, updateDef);

            return result.ModifiedCount;
        }

        #endregion
    }
}
