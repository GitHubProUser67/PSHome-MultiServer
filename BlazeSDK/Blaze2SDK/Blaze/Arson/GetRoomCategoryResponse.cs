using Blaze2SDK.Blaze.Rooms;
using Tdf;

namespace Blaze2SDK.Blaze.Arson
{
    [TdfStruct]
    public struct GetRoomCategoryResponse
    {

        [TdfMember("CAT")]
        public RoomCategoryData mCategoryData;

    }
}
