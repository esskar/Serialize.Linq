using System.Reflection;

namespace Serialize.Linq.Factories
{
    public class FactorySettings
    {
        public bool UseRelaxedTypeNames { get; set; } = true;

        public bool AllowPrivateFieldAccess { get; set; }

        public BindingFlags Binding
        {
            get
            {
                if (this.AllowPrivateFieldAccess)
                {
                    return Constants.ALSO_NON_PUBLIC_BINDING;
                }
                else
                {
                    return Constants.PUBLIC_ONLY_BINDING;
                }
            }
        }
    }
}
