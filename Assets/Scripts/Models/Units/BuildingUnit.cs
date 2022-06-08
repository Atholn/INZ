using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private GameManager gameManager;

    public int UnitToCreatePoints = 0;

    private GameObject fire;
    internal GameObject dust;
    internal GameObject buildingPlace;
    private int maxParticles;

    private ArrowShot arrowShot;
    private GameObject nearGameObject;

    private float nextShotTime = 0f;
    private readonly float _nextShotTimeMax = 2f;

    private readonly Vector3[] arrowPosiotions = new Vector3[]
    {
        new Vector3(0.025f, 0, 0.05f),
        new Vector3(0, -0.025f, 0.05f),
        new Vector3(-0.025f, 0, 0.05f),
        new Vector3(0, 0.025f, 0.05f)
    };

    private readonly Vector3[] arrowRotations = new Vector3[]
    {
        new Vector3(0, -90, 0),
        new Vector3(0, -180, 0),
        new Vector3(180, -90, 0),
        new Vector3(0, 0, 0)
    };

    private readonly Vector3[] arrowShifs = new Vector3[]
    {
        new Vector3(-3, 0, 0),
        new Vector3(0, 0, -3),
        new Vector3(3, 0, 0),
        new Vector3(0, 0, 3)
    };

    protected override void Start()
    {
        base.Start();
        gameManager = GameObject.FindObjectOfType<GameManager>();

        float h = gameObject.GetComponent<ItemGame>().HeightBuilding;
        fire = Instantiate(gameManager.FirePrefab, new Vector3(transform.position.x, h, transform.position.z), gameManager.FirePrefab.transform.rotation);
        fire.transform.localScale = new Vector3(Size, Size, Size);
        fire.SetActive(false);

        maxParticles = gameManager.FirePrefab.GetComponent<ParticleSystem>().main.maxParticles;
        arrowShot = gameObject.GetComponent<ArrowShot>();
    }

    protected override void Update()
    {
        base.Update();

        UpdateCreate();

        if (Hp <= 0)
        {
            Destroy(fire);
            Destroy(gameObject);
            return;
        }

        UpdateShot();
    }

    private void UpdateShot()
    {
        if (arrowShot == null || BuildingPercent < CreateTime || Hp <= 0)
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
            shot.transform.localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.x, 1 / transform.localScale.x);

            Dictionary<int, float> distances = new Dictionary<int, float>();
            for (int i = 0; i < arrowShifs.Length; i++)
                distances.Add(i, Vector3.Distance(nearGameObject.transform.position, transform.transform.position + arrowShifs[i]));

            int index = distances.OrderBy(d => d.Value).FirstOrDefault().Key;
            shot.transform.localPosition = arrowPosiotions[index];
            shot.transform.SetParent(null);
            shot.transform.rotation = Quaternion.Euler(arrowRotations[index]);
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

    internal void DestroyBuildingEffects()
    {
        gameManager.SetNullParentInCircles(buildingPlace);
        Destroy(dust);
        Destroy(buildingPlace);
    }
}
