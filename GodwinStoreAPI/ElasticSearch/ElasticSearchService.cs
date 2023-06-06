using Nest;
using Newtonsoft.Json;

namespace GodwinStoreAPI.ElasticSearch;

public class ElasticSearchService:IElasticSearchService
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<ElasticSearchService> _logger;

    public ElasticSearchService(IElasticClient elasticClient, ILogger<ElasticSearchService> logger)
    {
        _elasticClient = elasticClient;
        _logger = logger;
    }
    
    public async Task<bool> AddAsync<T>(T doc) where T : class
    {
        try
        {
            var indexResponse = await _elasticClient.IndexDocumentAsync(doc);
            if (!indexResponse.IsValid)
            {
                _logger.LogError(indexResponse.OriginalException, "An error occured indexing document\n{document}\n{debugInformation}",
                    indexResponse.DebugInformation,JsonConvert.SerializeObject(doc,Formatting.Indented));
            }

            return indexResponse.IsValid;
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error occured adding document\n{doc}",JsonConvert.SerializeObject(doc,Formatting.Indented));
            
            return false;
        }
    }

    public async Task<T> GetByIdAsync<T>(string id) where T : class
    {
        try
        {
            var getResponse = await _elasticClient.GetAsync<T>(id);
            if (!getResponse.IsValid)
            {
                _logger.LogError(getResponse.OriginalException,"An error occured getting document by id{id}\n{debugInformation}"
                    ,id,getResponse.DebugInformation);
            }

            return getResponse.Source;
        }
        catch (Exception e)
        {
           _logger.LogError(e,"An error occured getting records by id\n{id}",id);

           return null;
        }
    }

    public async Task<bool> UpdateAsync<T>(string id, T doc) where T : class
    {
        try
        {
            var updateResponse = await _elasticClient.UpdateAsync<T>(id, x => x.Doc(doc));
            if (!updateResponse.IsValid)
            {
                _logger.LogError(updateResponse.OriginalException,"An error occured updating document by id:{id}\n{debugInformation}",
                    id,updateResponse.DebugInformation);
            }

            return updateResponse.IsValid;
        }
        catch (Exception e)
        {
           _logger.LogError(e,"An error occured updating document by id: {id}",id);

           return false;
        }
    }

    public async Task<bool> DeleteAsync<T>(string id) where T : class
    {
        try
        {
            var deleteResponse = await _elasticClient.DeleteAsync<T>(id);
            if (!deleteResponse.IsValid)
            {
                _logger.LogError(deleteResponse.OriginalException,"An error occured deleting document by id:{id}\n{debugInformation}",
                    id,deleteResponse.DebugInformation);
            }

            return deleteResponse.IsValid;

        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error occured deleting document by id:{id}",id);
            
            return false;
        }
    }

    public async Task<ISearchResponse<T>> SearchAsync<T>(SearchRequest<T> searchRequest) where T : class
    {
        try
        {
            var searchResponse = await _elasticClient.SearchAsync<T>(searchRequest);
            if (!searchResponse.IsValid)
            {
                _logger.LogError(searchResponse.OriginalException,"An error occured searching for document\n{debugInformation}",searchResponse.DebugInformation);
            }

            return searchResponse;
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error occured searching for documents");

            return null;
        }
    }
}