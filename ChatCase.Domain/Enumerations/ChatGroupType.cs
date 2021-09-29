using System.Runtime.Serialization;

namespace ChatCase.Domain.Enumerations
{
    public enum ChatGroupType
    {
        [EnumMember(Value = "directmessage")]
        DirectMessage,
        [EnumMember(Value = "group")]
        Group,
    }
}
