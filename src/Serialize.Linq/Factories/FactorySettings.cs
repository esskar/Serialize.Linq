using System.Reflection;

namespace Serialize.Linq.Factories
{
    public class FactorySettings
    {
        public bool UseRelaxedTypeNames { get; set; } = true;

        public bool AllowPrivateFieldAccess { get; set; }

        public BindingFlags BindingFlags => this.AllowPrivateFieldAccess ? Constants.ALSO_NON_PUBLIC_BINDING : Constants.PUBLIC_ONLY_BINDING;
    }
}
