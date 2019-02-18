using Klyte.Commons.Utils;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.Utils
{
    public class SVMResourceLoader : KlyteResourceLoader<SVMResourceLoader>
    {
        protected override string prefix => "Klyte.ServiceVehiclesManager.";
    }
}
