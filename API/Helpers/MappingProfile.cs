using AutoMapper;
using Api.Dto;
using Entity;

namespace Api.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Todo, TodoDto>().ReverseMap();
        }
    }
}
