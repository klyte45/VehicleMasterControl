using Klyte.VehiclesMasterControl;

namespace Klyte.Commons
{
    public static class CommonProperties
    {
        public static bool DebugMode => VehiclesMasterControlMod.DebugMode;
        public static string Version => VehiclesMasterControlMod.Version;
        public static string ModName => VehiclesMasterControlMod.Instance.SimpleName;
        public static string Acronym => "VMC";
        public static string ModRootFolder => VMCController.FOLDER_PATH;

        public static string ModIcon { get; } = VMCController.FOLDER_NAME;
        public static string ModDllRootFolder { get; } = VehiclesMasterControlMod.RootFolder;
    }
}