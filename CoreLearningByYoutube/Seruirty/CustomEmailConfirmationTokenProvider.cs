using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CoreLearningByYoutube.Seruirty
{
    public class CustomEmailConfirmationTokenProvider<TUser> :
        DataProtectorTokenProvider<TUser> where TUser : class
    {
        public CustomEmailConfirmationTokenProvider(
            IDataProtectionProvider dataProtectionProvider,
            IOptions<CustomEmailConfirmationTokenProviderOptions> options) 
            : base(dataProtectionProvider, options)
        {

        }
    }
}
