namespace GodwinStoreAPI.ElasticSearch;

public class ElasticSearchConfig
{
    public string BaseUrl { get; set; }
    public string CustomerIndex { get; set; }
    public string OrderIndex { get; set; }
    public string ProductIndex { get; set; }
}