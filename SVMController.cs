using System;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using Klyte.ServiceVehiclesManager.UI;
using Klyte.ServiceVehiclesManager.Utils;
using Klyte.TransportLinesManager.Extensors;
using Klyte.TransportLinesManager.Utils;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager
{
    internal class SVMController : Singleton<SVMController>
    {
        internal static UITextureAtlas taSVM;
        private UIView uiView;
        private bool initialized = false;
        private UIButton openSVMPanelButton;
        private SVMServiceBuildingDetailPanel m_listPanel;

        public void destroy()
        {
            Destroy(uiView);
            Destroy(openSVMPanelButton);
            initialized = false;
        }

        public void Start()
        {
            uiView = FindObjectOfType<UIView>();
            if (!uiView)
                return;

            UITabstrip toolStrip = uiView.FindUIComponent<UITabstrip>("MainToolstrip");
            SVMUtils.createUIElement(out openSVMPanelButton, null);
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
                    internal_CloseSVMPanel();
                }
            };
            m_listPanel = SVMServiceBuildingDetailPanel.Create();

            var typeTarg = typeof(Redirector<>);

            var instances = from t in Assembly.GetAssembly(typeof(SVMController)).GetTypes()
                            let y = t.BaseType
                            where t.IsClass && !t.IsAbstract && y != null && y.IsGenericType && y.GetGenericTypeDefinition() == typeTarg
                            select t;

            foreach (Type t in instances)
            {
                gameObject.AddComponent(t);
            }


            initialized = true;
        }

        public void Awake()
        {
        }

        public void CloseSVMPanel()
        {
            openSVMPanelButton.SimulateClick();
        }

        private void internal_CloseSVMPanel()
        {
            m_listPanel.GetComponent<UIPanel>().isVisible = false;
            openSVMPanelButton.Unfocus();
            openSVMPanelButton.state = UIButton.ButtonState.Normal;
        }

        private void OpenSVMPanel()
        {
            m_listPanel.GetComponent<UIPanel>().isVisible = true;
        }
    }
}