using System;
using System.Runtime.Serialization;

namespace Identity.Dapper.Oracle.Exceptions
{
    [Serializable]
    public class NoDapperIdentityConfigurationSectionRegisteredException
        : Exception
    {
        public NoDapperIdentityConfigurationSectionRegisteredException()
            : base()
        {
        }

        public NoDapperIdentityConfigurationSectionRegisteredException(string message)
            : base(message)
        {
        }

        public NoDapperIdentityConfigurationSectionRegisteredException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NoDapperIdentityConfigurationSectionRegisteredException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
