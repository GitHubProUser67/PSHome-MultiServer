using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct GetProductsResponse
    {
        
        [TdfMember("PDRL")]
        public List<Product> mProductVector;
        
    }
}
