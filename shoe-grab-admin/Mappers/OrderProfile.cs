using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using ShoeGrabAdminService.Dto;

namespace ShoeGrabAdminService.Mappers;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<OrderProto, OrderDto>()
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToDateTime()));
        CreateMap<OrderItemProto, OrderItemDto>();
        CreateMap<AddressProto, AddressDto>();
        CreateMap<PaymentInfoProto, PaymentInfoDto>()
            .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate.ToDateTime()));
        CreateMap<ProductDto, ProductProto>();
    }
}
