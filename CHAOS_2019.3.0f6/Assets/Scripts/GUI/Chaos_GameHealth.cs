using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial
{

    public class Chaos_GameHealth : Bolt.GlobalEventListener
    {
        BoltEntity m_Me;
        IChaos_PlayerState m_MeState;

        [SerializeField]
        TypogenicText text;

        public override void ControlOfEntityGained(BoltEntity arg)
        {
            if (arg.GetComponent<PlayerNetworkHandler>())
            {
                m_Me = arg;
                m_MeState = m_Me.GetState<IChaos_PlayerState>();
                m_MeState.AddCallback("Health", HealthChanged);

                HealthChanged();
            }
        }

        public override void ControlOfEntityLost(BoltEntity arg)
        {
            if (arg.GetComponent<PlayerNetworkHandler>())
            {
                m_MeState.RemoveCallback("Health", HealthChanged);

                m_Me = null;
                m_MeState = null;
            }
        }

        void Update()
        {
            text.transform.position = new Vector3(
                -(Screen.width / 2) + 4,
                -(Screen.height / 2) + 60,
                250f
            );
        }

        void HealthChanged()
        {
            Color c;

            if (m_MeState.Health <= 25)
            {
                c = Color.red;
            }
            else if (m_MeState.Health <= 50)
            {
                c = new Color(1f, 0.5f, 36f / 255f);
            }
            else if (m_MeState.Health <= 75)
            {
                c = Color.yellow;
            }
            else
            {
                c = Color.green;
            }

            text.ColorBottomLeft =
            text.ColorBottomRight =
            text.ColorTopLeft =
            text.ColorTopRight =
              c;

            text.Set("HP: " + m_MeState.Health);
        }
    }

}
