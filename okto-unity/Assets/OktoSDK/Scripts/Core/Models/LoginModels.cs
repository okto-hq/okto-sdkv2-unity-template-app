using System;
using Newtonsoft.Json;

namespace OktoSDK
{
    public class LoginOAuthDataModels
    {
        [Serializable]
        public abstract class UserDetailBase
        {
            public abstract string GetDetail();
        }

        [Serializable]
        public class EmailDetail : UserDetailBase
        {
            public string email;
            public override string GetDetail() => email;
        }

        [Serializable]
        public class WhatsAppDetail : UserDetailBase
        {
            public string PhoneNumber;
            public override string GetDetail() => PhoneNumber;
        }

        [Serializable]
        public class TwitterDetail : UserDetailBase
        {
            public string TwitterHandle;
            public override string GetDetail() => TwitterHandle;
        }
    }
} 