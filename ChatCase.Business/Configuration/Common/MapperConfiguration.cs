using AutoMapper;
using ChatCase.Core.Domain.Logging;
using ChatCase.Core.Infrastructure.Mapper;
using ChatCase.Domain.Dto.Response.Logging;

namespace ChatCase.Business.Configuration.Common
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor
        public MapperConfiguration()
        {
            CreateActivityLogMaps();
        }

        #endregion

        #region Utilities

        protected virtual void CreateActivityLogMaps()
        {
            CreateMap<ActivityLog, ActivityLogDto>();
        }
        #endregion

        public int Order => 0;
    }
}
