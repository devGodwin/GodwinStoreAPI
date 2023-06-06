using AutoMapper;
using GodwinStoreAPI.Data.StoreData;
using GodwinStoreAPI.Model.CustomerModel;
using GodwinStoreAPI.Model.CustomerModel.RequestModel;
using GodwinStoreAPI.Model.CustomerModel.ResponseModel;

namespace GodwinStoreAPI.Mapper.CustomerMapper;

public class CustomerMapper:Profile
{
    public CustomerMapper()
    {
        CreateMap<Customer, RegisterCustomerRequestModel>().ReverseMap();
        CreateMap<Customer, CustomerUpdateModel>().ReverseMap();
        CreateMap<Customer, RegisterCustomerResponseModel>().ReverseMap();
        CreateMap<Customer, CachedCustomer>().ReverseMap();
    }
}