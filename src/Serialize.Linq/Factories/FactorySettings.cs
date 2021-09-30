using System.Reflection;

namespace Serialize.Linq.Factories
{
    public class FactorySettings
    {
        public FactorySettings()
        {
            UseRelaxedTypeNames = true;
        }

        public FactorySettings(bool allowPrivateFieldAccess, bool useRelaxedTypeNames = true)
        {
            AllowPrivateFieldAccess = allowPrivateFieldAccess;
            UseRelaxedTypeNames = useRelaxedTypeNames;
        }

        public bool UseRelaxedTypeNames { get; set; }

        public bool AllowPrivateFieldAccess { get; set; }

        public BindingFlags BindingFlags => this.AllowPrivateFieldAccess ? Constants.ALSO_NON_PUBLIC_BINDING : Constants.PUBLIC_ONLY_BINDING;
    }
}
