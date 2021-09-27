using AutoMapper;
using ChatCase.Core.Infrastructure.Mapper;

namespace ChatCase.Business.Configuration.Common
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor
        public MapperConfiguration()
        {

        }

        #endregion

        #region Utilities

        #endregion

        public int Order => 0;
    }
}
