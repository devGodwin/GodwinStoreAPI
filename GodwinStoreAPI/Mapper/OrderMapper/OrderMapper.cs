using AutoMapper;
using GodwinStoreAPI.Data.StoreData;
using GodwinStoreAPI.Model.OrderModel.RequestModel;
using GodwinStoreAPI.Model.OrderModel.ResponseModel;

namespace GodwinStoreAPI.Mapper.OrderMapper;

public class OrderMapper:Profile
{
    public OrderMapper()
    {
        CreateMap<Order, OrderRequestModel>().ReverseMap();
        CreateMap<Order, OrderUpdateModel>().ReverseMap();
        CreateMap<Order, OrderResponseModel>().ReverseMap();
    }
}