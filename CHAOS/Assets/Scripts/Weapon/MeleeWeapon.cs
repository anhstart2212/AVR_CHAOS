using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt.AdvancedTutorial;

public class MeleeWeapon : Chaos_WeaponBase, IActionCaller
{
    public CombatData combatData;

    public ActionData nextAction;

    public bool actionPending;

    private List<ActionData> queue = new List<ActionData>();

    public float timeBetweenCombo = 1;

    public float timeBetweenComboTimecount;

    public ComboList moveList;

    void Awake()
    {
        
    }

    void Update()
    {
        if (timeBetweenComboTimecount >= 0)
        {
            timeBetweenComboTimecount -= Time.deltaTime;

            if (timeBetweenComboTimecount <= 0)
            {
                queue.Clear();
            }
        }

    }

    public override void BasicAttack()
    {
        this.QueueAction(combatData.BasicAttack);
    }

    public override void HeavyAttack()
    {
        this.QueueAction(combatData.HeavyAttack);
    }

    public override void Parry()
    {
        this.QueueAction(combatData.Parry);
    }

    private void QueueAction(ActionData actionData)
    {
        if (actionData.interruptable)
        {
            this.UseAction(actionData);
        }
        else
        {
            if (actionPending)
            {
                nextAction = actionData;
            }
            else
            {
                this.UseAction(actionData);
            }
        }
    }

    // Token: 0x06000021 RID: 33 RVA: 0x00006585 File Offset: 0x00004985
    protected void UseAction(ActionData actionData)
    {
        AddCombo(actionData);

        if (CheckCombo())
        {
            print("Done Combo!...clearing old action datas");
            queue.Clear();
        }
        else
        {
            actionPending = true;

            timeBetweenComboTimecount = timeBetweenCombo;

            StartCoroutine(actionData.Excecute(this, transform));
        }
    }

    public void Callback(ActionData finishedData)
    {
        actionPending = false;

        if (nextAction != null)
        {
            this.UseAction(nextAction);
            nextAction = null;
        }
    }

    bool CheckCombo()
    {
        if (moveList.Check(queue))
        {
            queue.Clear();
            return true;
        }
        return false;
    }

    void AddCombo(ActionData action)
    {
        queue.Add(action);
    }




    //    public int damage = 1;

    //    [System.Serializable]
    //    public class AttackPoint
    //    {
    //        public float radius;
    //        public Vector3 offset;
    //        public Transform attackRoot;
    //    }

    //    public ParticleSystem hitParticlePrefab;
    //    public LayerMask targetLayers;
    //    public AttackPoint[] attackPoints = new AttackPoint[0];

    //    private Vector3[] m_PreviousPos = null;
    //    private bool m_InAttack;
    //    private static RaycastHit[] s_RaycastHitCache = new RaycastHit[32];
    //    private GameObject m_Owner;
    //    private Vector3 m_Direction;
    //    private const int PARTICLE_COUNT = 10;
    //    private ParticleSystem[] m_ParticlesPool = new ParticleSystem[PARTICLE_COUNT];
    //    private int m_CurrentParticle = 0;
    //    private Damageable.DamageMessage data;

    //    private void Awake()
    //    {
    //        if (hitParticlePrefab != null)
    //        {
    //            for (int i = 0; i < PARTICLE_COUNT; ++i)
    //            {
    //                m_ParticlesPool[i] = Instantiate(hitParticlePrefab);
    //                m_ParticlesPool[i].Stop();
    //            }
    //        }
    //    }

    //    private void FixedUpdate()
    //    {
    //        if (m_InAttack)
    //        {
    //            for (int i = 0; i < attackPoints.Length; ++i)
    //            {
    //                AttackPoint pts = attackPoints[i];

    //                Vector3 worldPos = pts.attackRoot.position + pts.attackRoot.TransformVector(pts.offset);
    //                Vector3 attackVector = worldPos - m_PreviousPos[i];

    //                if (attackVector.magnitude < 0.001f)
    //                {
    //                    // A zero vector for the sphere cast don't yield any result, even if a collider overlap the "sphere" created by radius. 
    //                    // so we set a very tiny microscopic forward cast to be sure it will catch anything overlaping that "stationary" sphere cast
    //                    attackVector = Vector3.forward * 0.0001f;
    //                }


    //                Ray r = new Ray(worldPos, attackVector.normalized);

    //                Debug.DrawRay(worldPos, attackVector.normalized, Color.blue);

    //                int contacts = Physics.SphereCastNonAlloc(r, pts.radius, s_RaycastHitCache, attackVector.magnitude,
    //                    ~0,
    //                    QueryTriggerInteraction.Ignore);

    //                for (int k = 0; k < contacts; ++k)
    //                {
    //                    Collider col = s_RaycastHitCache[k].collider;

    //                    if (col != null && targetLayers == (targetLayers | (1 << col.gameObject.layer)))
    //                    {
    //                        CheckDamage(col, pts);
    //                    }
    //                }

    //                m_PreviousPos[i] = worldPos;
    //            }
    //        }
    //    }

    //    private bool CheckDamage(Collider other, AttackPoint pts)
    //    {
    //        //Debug.Log(other.gameObject.name);
    //        //Damageable d = other.GetComponent<Damageable>();
    //        //if (d == null)
    //        //{
    //        //    Debug.Log("8888888888888");
    //        //    return false;
    //        //}

    //        //if (d.gameObject == m_Owner)
    //        //    return true; //ignore self harm, but do not end the attack (we don't "bounce" off ourselves)


    //        //data.amount = damage;
    //        //data.damager = this;
    //        //data.direction = m_Direction.normalized;
    //        //data.damageSource = m_Owner.transform.position;
    //        //data.stopCamera = false;

    //        //d.ApplyDamage(data);

    //        if (hitParticlePrefab != null)
    //        {
    //            m_ParticlesPool[m_CurrentParticle].transform.position = pts.attackRoot.transform.position;
    //            m_ParticlesPool[m_CurrentParticle].time = 0;
    //            m_ParticlesPool[m_CurrentParticle].Play();
    //            m_CurrentParticle = (m_CurrentParticle + 1) % PARTICLE_COUNT;
    //        }

    //        return true;
    //    }


    //    public void BeginAttack(bool thowingAttack)
    //    {
    //        m_InAttack = true;

    //        m_PreviousPos = new Vector3[attackPoints.Length];

    //        for (int i = 0; i < attackPoints.Length; ++i)
    //        {
    //            Vector3 worldPos = attackPoints[i].attackRoot.position +
    //                               attackPoints[i].attackRoot.TransformVector(attackPoints[i].offset);
    //            m_PreviousPos[i] = worldPos;
    //        }
    //    }

    //    public void EndAttack()
    //    {
    //        m_InAttack = false;
    //    }

    //    public void SetOwner(GameObject owner)
    //    {
    //        m_Owner = owner;
    //    }

    //#if UNITY_EDITOR

    //    private void OnDrawGizmosSelected()
    //    {
    //        for (int i = 0; i < attackPoints.Length; ++i)
    //        {
    //            AttackPoint pts = attackPoints[i];

    //            if (pts.attackRoot != null)
    //            {
    //                Vector3 worldPos = pts.attackRoot.TransformVector(pts.offset);
    //                Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
    //                Gizmos.DrawSphere(pts.attackRoot.position + worldPos, pts.radius);
    //            }
    //        }
    //    }

    //#endif
}
