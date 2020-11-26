using Klyte.ServiceVehiclesManager;

namespace Klyte.Commons
{
    public static class CommonProperties
    {
        public static bool DebugMode => ServiceVehiclesManagerMod.DebugMode;
        public static string Version => ServiceVehiclesManagerMod.Version;
        public static string ModName => ServiceVehiclesManagerMod.Instance.SimpleName;
        public static string Acronym => "SVM";
        public static string ModRootFolder => SVMController.FOLDER_PATH;

        public static string ModIcon { get; } = SVMController.FOLDER_NAME;
        public static string ModDllRootFolder { get; } = ServiceVehiclesManagerMod.RootFolder;
    }
}