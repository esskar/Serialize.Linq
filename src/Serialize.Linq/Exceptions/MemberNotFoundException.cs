using System;

namespace Serialize.Linq.Exceptions
{
    public class MemberNotFoundException : Exception
    {
        public MemberNotFoundException(string message, Type declaringType, string memberSignature)
            : base(message)
        {
            this.DeclaringType = declaringType;
            this.MemberSignature = memberSignature;
        }

        public Type DeclaringType { get; }

        public string MemberSignature { get; }

        public override string ToString()
        {
            return String.Format("{1}.{0}Declaring Type: '{2}'{0}Member Signature: '{3}'",
                Environment.NewLine,
                this.Message, this.DeclaringType, this.MemberSignature);
        }
    }
}