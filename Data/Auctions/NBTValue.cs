using MessagePack;

namespace hypixel
{
    /// <summary>
    /// Mapps values to ids for use in <see cref="NBTLookup"/>
    /// </summary>
    [MessagePackObject]
    public class NBTValue
    {
        [Key(0)]
        public short Id { get; set; }
        [Key(1)]
        public short KeyId { get; set; }
        [Key(2)]
        public string Value { get; set; }

        public NBTValue(short keyId, string value)
        {
            KeyId = keyId;
            Value = value;
        }

        public NBTValue()
        {
        }
    }
}