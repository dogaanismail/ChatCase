using AutoMapper;
using ChatCase.Core.Domain.Chatting;
using ChatCase.Core.Domain.Logging;
using ChatCase.Core.Infrastructure.Mapper;
using ChatCase.Domain.Dto.Response.Chatting;
using ChatCase.Domain.Dto.Response.Logging;

namespace ChatCase.Business.Configuration.Common
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor
        public MapperConfiguration()
        {
            CreateActivityLogMaps();
            CreateChatMaps();
        }

        #endregion

        #region Utilities

        protected virtual void CreateActivityLogMaps()
        {
            CreateMap<ActivityLog, ActivityLogDto>();
        }

        protected virtual void CreateChatMaps()
        {
            CreateMap<Chat, MessageDto>();
        }
        #endregion

        public int Order => 0;
    }
}
