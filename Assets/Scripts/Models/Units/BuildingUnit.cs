using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUnit : Unit
{
    public int Size;
    public bool PlaceWood;
    public bool PlaceGold;

    public float SizeBuilding;
    internal float BuildingPercent = 0f;

    internal Vector3 PointerPosition;

    public bool CanCreateUnit = false;
    /// <summary>
    /// unit
    /// </summary>
    internal bool createUnit = false;
    private List<GameObject> queueCreateUnit = new List<GameObject>();
    private List<float> queueCreateUnitProgress = new List<float>();
    internal float unitCreateProgress;
    private int whichPlayerUnit;

    private GameManager gameManager;

    public int UnitToCreatePoints = 0;

    private GameObject fire;
    private GameObject dust;
    private int maxParticles;

    private ArrowShot arrowShot;
    private GameObject nearGameObject;

    private float nextShotTime = 0f;
    private readonly float _nextShotTimeMax = 2f;

    protected override void Start()
    {
        base.Start();
        gameManager = GameObject.FindObjectOfType<GameManager>();

        float h = gameObject.GetComponent<ItemGame>().HeightBuilding;

        fire = Instantiate(gameManager.Fire, new Vector3(transform.position.x, h, transform.position.z), gameManager.Fire.transform.rotation);
        dust = Instantiate(gameManager.Dust, new Vector3(transform.position.x, HeightPosY, transform.position.z), gameManager.Dust.transform.rotation);

        fire.transform.localScale = new Vector3(Size, Size, Size);
        dust.transform.localScale = new Vector3(Size, Size, Size/2);

        fire.SetActive(false);
        dust.SetActive(false);

        maxParticles = gameManager.Fire.GetComponent<ParticleSystem>().main.maxParticles;

        arrowShot = gameObject.GetComponent<ArrowShot>();
    }

    protected override void Update()
    {
        base.Update();

        UpdateCreate();

        if (Hp <= 0)
        {
            Destroy(fire);
            Destroy(dust);
            Destroy(gameObject);
            return;
        }

        UpdateShot();
    }

    private void UpdateShot()
    {
        if (arrowShot == null || BuildingPercent < CreateTime)
            return;

        nextShotTime += Time.deltaTime;
        if (nextShotTime < _nextShotTimeMax)
            return;

        nearGameObject = gameManager.CheckNearEnemy(WhichPlayer, transform.position);
        nextShotTime = 0f;

        if (nearGameObject != null)
        {
            ArrowShot bowShot = GetComponent<ArrowShot>();
            GameObject shot = Instantiate(bowShot.Arrow, transform.position, bowShot.Arrow.transform.rotation, transform);
            //shot.transform.localPosition += new Vector3(0, 10, 0);

            shot.transform.localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.x, 1 / transform.localScale.x);
            shot.transform.SetParent(null);
            shot.transform.position = new Vector3(shot.transform.position.x, 10, shot.transform.position.z);
            shot.GetComponent<Arrow>().SetLandingPlace(nearGameObject.transform.position, AttackPower);
        }
        
    }

    internal void UpdateFire()
    {
        if (Hp == HpMax)
        {
            fire.SetActive(false);
            return;
        }

        if (Hp < HpMax)
        {
            fire.SetActive(true);
            float x = 1f - ((float)Hp / (float)HpMax);
            fire.GetComponent<ParticleSystem>().maxParticles = (int)(x * (float)maxParticles);
            return;
        }
    }

    internal void DustOn()
    {
        dust.SetActive(true);
    }

    internal void DustOff()
    {
        dust.SetActive(false);
    }

    internal void CreateUnit(GameObject unitToCreate)
    {
        createUnit = true;
        queueCreateUnit.Add(unitToCreate);
        queueCreateUnitProgress.Add(0f);
    }

    internal void CancelCreateUnit(int i)
    {
        queueCreateUnit.Remove(queueCreateUnit[i]);
        queueCreateUnitProgress.Remove(queueCreateUnitProgress[i]);

        if (queueCreateUnit.Count == 0 && queueCreateUnitProgress.Count == 0)
        {
            createUnit = false;
        }
    }

    private void UpdateCreate()
    {
        if (!createUnit)
        {
            return;
        }

        if (gameManager._players[WhichPlayer].actualUnitsPoint + queueCreateUnit[0].GetComponent<HumanUnit>().UnitPoint > 
            gameManager._players[WhichPlayer].actualMaxUnitsPoint)
        {
            return;
        }

        queueCreateUnitProgress[0] += Time.deltaTime;

        if (queueCreateUnitProgress[0] > queueCreateUnit[0].GetComponent<Unit>().CreateTime)
        {
            gameManager.UnitCreate(WhichPlayer, queueCreateUnit[0], transform.position + new Vector3(0, 0, -SizeBuilding / 2), PointerPosition);
            queueCreateUnit.Remove(queueCreateUnit[0]);
            queueCreateUnitProgress.Remove(queueCreateUnitProgress[0]);

            if (queueCreateUnit.Count == 0 && queueCreateUnitProgress.Count == 0)
            {
                createUnit = false;
            }
        }
    }

    internal void UpdateProgressInfo(ref Texture2D[] textures, ref float val)
    {
        for (int i = 0; i < textures.Length; i++)
        {
            textures[i] = queueCreateUnit[i].GetComponent<Unit>().Profile;

        }
        val = queueCreateUnitProgress[0] / queueCreateUnit[0].GetComponent<Unit>().CreateTime;
    }

    internal int GetActualQueueLength()
    {
        return queueCreateUnit.Count;
    }

    internal GameObject HowUnitInID(int i)
    {
        return queueCreateUnit[i];
    }

    public void UpdateUnitPoints(int whichPlayer)
    {
        gameManager.UpdateUnitPoints(whichPlayer);
    }
}
