using DotNetty.Common.Utilities;
using Horizon.LIBRARY.Pipeline.Attribute;

namespace Horizon.LIBRARY.Pipeline
{
    public static class Constants
    {
        public static readonly AttributeKey<ScertClientAttribute> SCERT_CLIENT = AttributeKey<ScertClientAttribute>.ValueOf("SCERT_CLIENT");
    }
}