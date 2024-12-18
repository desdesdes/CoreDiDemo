﻿using Azure;
using Azure.Data.Tables;

namespace Afas.Bvr.Core.Repository;

/// <threadsafety static="true" instance="true"/>
public class AzureStorageTableRepository : Repository
{
  readonly TableServiceClient _serviceClient;

  public AzureStorageTableRepository(string endpoint, string sasSignature)
  {
    _serviceClient = new TableServiceClient(
      new Uri(endpoint),
      new AzureSasCredential(sasSignature));
  }

  public override async Task Add<TKey, TValue>(TValue newItem)
  {
    var tableName = typeof(TValue).Name;

    await _serviceClient.CreateTableIfNotExistsAsync(tableName);

    var tableClient = _serviceClient.GetTableClient(tableName);

    var tableEntity = new TableEntity(newItem.Id.ToString(), newItem.Id.ToString());

    var properties = typeof(TValue).GetProperties();
    foreach(var prop in properties)
    {
      // skip the id
      if(string.Equals(prop.Name, "ID", StringComparison.OrdinalIgnoreCase))
      {
        continue;
      }

      tableEntity.Add(prop.Name, prop.GetValue(newItem));
    } 

    await tableClient.AddEntityAsync(tableEntity);
  }

  public override async Task Delete<TKey, TValue>(TKey id)
  {
    var tableName = typeof(TValue).Name;

    await _serviceClient.CreateTableIfNotExistsAsync(tableName);

    var tableClient = _serviceClient.GetTableClient(tableName);

    _ = await tableClient.DeleteEntityAsync(id.ToString(), id.ToString());
  }

  public override async Task<TValue?> GetOrDefault<TKey, TValue>(TKey id) where TValue : class
  {
    var tableName = typeof(TValue).Name;

    await _serviceClient.CreateTableIfNotExistsAsync(tableName);

    var tableClient = _serviceClient.GetTableClient(tableName);

    var entity = await tableClient.GetEntityAsync<TableEntity>(id.ToString(), id.ToString());

    if(!entity.HasValue)
    {
      return null;
    }

    var newItem = Activator.CreateInstance<TValue>();
    newItem.Id = id;

    var properties = typeof(TValue).GetProperties();
    foreach(var prop in properties)
    {
      // skip the id
      if(string.Equals(prop.Name, "ID", StringComparison.OrdinalIgnoreCase))
      {
        continue;
      }

      if(entity.Value.TryGetValue(prop.Name, out var value))
      {
        prop.SetValue(newItem, value);
      }
    }

    return newItem;
  }
}
