using System;
using ColossalFramework;
using ColossalFramework.UI;
using Klyte.ServiceVehiclesManager.Utils;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager
{
    internal class SVMController : Singleton<SVMController>
    {
        internal static UITextureAtlas taSVM;
        private UIView uiView;
        private bool initialized = false;
        private  UIButton openSVMPanelButton;

        public void destroy()
        {
            Destroy(uiView);
            Destroy(openSVMPanelButton);
            initialized = false;
        }

        public void update()
        {
            if (!initialized)
            {
                uiView = FindObjectOfType<UIView>();
                if (!uiView)
                    return;

                UITabstrip toolStrip = uiView.FindUIComponent<UITabstrip>("MainToolstrip");
                SVMUtils.createUIElement(ref openSVMPanelButton, transform);
                //this.openSVMPanelButton.focusedFgSprite = "ToolbarIconGroup6Focused";
                //this.openSVMPanelButton.hoveredFgSprite = "ToolbarIconGroup6Hovered";
                this.openSVMPanelButton.size = new Vector2(49f, 49f);
                this.openSVMPanelButton.name = "ServiceVehiclesManagerButton";
                this.openSVMPanelButton.tooltip = "Service Vehicles Manager (v" + ServiceVehiclesManagerMod.version + ")";
                this.openSVMPanelButton.relativePosition = new Vector3(0f, 5f);
                toolStrip.AddTab("ServiceVehiclesManagerButton", this.openSVMPanelButton.gameObject, null, null);
                openSVMPanelButton.atlas = taSVM;
                openSVMPanelButton.normalBgSprite = "ServiceVehiclesManagerIconSmall";
                openSVMPanelButton.focusedFgSprite = "ToolbarIconGroup6Focused";
                openSVMPanelButton.hoveredFgSprite = "ToolbarIconGroup6Hovered";
                this.openSVMPanelButton.eventButtonStateChanged += delegate (UIComponent c, UIButton.ButtonState s)
                {
                    if (s == UIButton.ButtonState.Focused)
                    {
                        OpenSVMPanel();
                    }
                    else
                    {
                        CloseSVMPanel();
                    }
                };

                initialized = true;
            }

        }

        private void CloseSVMPanel()
        {
            //base.isVisible = false;
            //this.m_button.Unfocus();
        }

        private void OpenSVMPanel()
        {
            ServiceVehiclesManagerMod.instance.showVersionInfoPopup();

            //if (!isVisible)
            //{
            //    base.isVisible = true;
            //    this.m_fastList.DisplayAt(this.m_fastList.listPosition);
            //    this.m_optionPanel.Show(this.m_fastList.rowsData[this.m_fastList.selectedIndex] as VehicleOptions);
            //    this.m_followVehicle.isVisible = (this.m_preview.parent.isVisible = true);
            //    return;
            //}
        }
    }
}