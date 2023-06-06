using AutoMapper;
using GodwinStoreAPI.Data.StoreData;
using GodwinStoreAPI.Model.ProductModel.RequestModel;
using GodwinStoreAPI.Model.ProductModel.ResponseModel;

namespace GodwinStoreAPI.Mapper.ProductMapper;

public class ProductMapper:Profile
{
    public ProductMapper()
    {
        CreateMap<Product, ProductRequestModel>().ReverseMap();
        CreateMap<Product, ProductUpdateModel>().ReverseMap();
        CreateMap<Product, ProductResponseModel>().ReverseMap();
    }
}