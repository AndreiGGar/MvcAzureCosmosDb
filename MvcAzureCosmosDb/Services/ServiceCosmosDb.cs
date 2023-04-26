using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using MvcAzureCosmosDb.Models;

namespace MvcAzureCosmosDb.Services
{
    public class ServiceCosmosDb
    {
        private CosmosClient client;
        public Container containerCosmos;

        public ServiceCosmosDb(CosmosClient client, Container container)
        {
            this.client = client;
            this.containerCosmos = container;
        }

        public async Task CreateDatabaseAsync()
        {
            ContainerProperties properties = new ContainerProperties("containercoches", "/id");
            await this.client.CreateDatabaseIfNotExistsAsync("vehiculoscosmos");
            await this.client.GetDatabase("vehiculoscosmos").CreateContainerIfNotExistsAsync(properties);
        }

        public async Task InsertVehiculoAsync(Vehiculo car)
        {
            await this.containerCosmos.CreateItemAsync<Vehiculo>(car, new PartitionKey(car.Id));
        }

        public async Task<List<Vehiculo>> GetVehiculosAsync()
        {
            var query = this.containerCosmos.GetItemQueryIterator<Vehiculo>();
            List<Vehiculo> coches = new List<Vehiculo>();
            while (query.HasMoreResults)
            {
                var results = await query.ReadNextAsync();
                coches.AddRange(results); 
            }
            return coches;
        }

        public async Task UpdateVehiculoAsync(Vehiculo car)
        {
            await this.containerCosmos.UpsertItemAsync<Vehiculo>(car, new PartitionKey(car.Id));
        }

        public async Task DeleteVehiculoAsync(string id)
        {
            await this.containerCosmos.DeleteItemAsync<Vehiculo>(id, new PartitionKey(id));
        }

        public async Task<Vehiculo> FindVehiculoAsync(string id)
        {
            ItemResponse<Vehiculo> response = await this.containerCosmos.ReadItemAsync<Vehiculo>(id, new PartitionKey(id));
            return response.Resource;
        }
    }
}
