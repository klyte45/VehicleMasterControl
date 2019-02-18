using ColossalFramework.Globalization;
using Klyte.Commons.i18n;
using Klyte.ServiceVehiclesManager.Utils;
using System;

namespace Klyte.ServiceVehiclesManager.i18n
{
    public class SVMLocaleUtils : KlyteLocaleUtils<SVMLocaleUtils, SVMResourceLoader>
    {
        public override string prefix => "SVM_";

        protected override string packagePrefix => "Klyte.ServiceVehiclesManager";
    }
}
