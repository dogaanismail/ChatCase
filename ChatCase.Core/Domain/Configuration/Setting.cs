using ChatCase.Core.Entities;

namespace ChatCase.Core.Domain.Configuration
{
    public partial class Setting : BaseEntity
    {
        public Setting()
        {

        }

        public Setting(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
