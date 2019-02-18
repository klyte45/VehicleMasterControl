using ColossalFramework;
using ColossalFramework.UI;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.TextureAtlas
{
    public class SVMCommonTextureAtlas : TextureAtlasDescriptor<SVMCommonTextureAtlas, SVMResourceLoader>
    {
        protected override string ResourceName => "UI.Images.sprites.png";
        protected override string CommonName => "ServiceVehiclesManagerSprites";
        protected override string[] SpriteNames => new string[] {
               "ServiceVehiclesManagerIcon","ServiceVehiclesManagerIconSmall","ToolbarIconGroup6Hovered","ToolbarIconGroup6Focused","HelicopterIndicator","RemoveUnwantedIcon","CargoIndicator", "OutsideIndicator", "BioIndicator"
                };
    }
}
